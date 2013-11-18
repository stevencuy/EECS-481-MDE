#include <iostream>
#include <queue>

#include "constants.h"
#include "Utility.h"
#include "Eyes.h"

Eyes::Eyes()
{
}

cv::Point Eyes::unscalePoint(cv::Point p, cv::Rect origSize)
{
	float ratio = (((float)kFastEyeWidth)/origSize.width);
	int x = (p.x / ratio);
	int y = (p.y / ratio);
	return cv::Point(x,y);
}

void Eyes::findEyes(cv::Mat frame_gray, cv::Rect face, cv::Mat * debugImage)
{
	cv::Mat faceROI = frame_gray(face);
	cv::Mat debugFace = faceROI;

	if (kSmoothFaceImage) {
		double sigma = kSmoothFaceFactor * face.width;
		GaussianBlur( faceROI, faceROI, cv::Size( 0, 0 ), sigma);
	}

	//-- Find eye regions and draw them
	eye_region_width = face.width * (kEyePercentWidth/100.0);
	eye_region_height = face.width * (kEyePercentHeight/100.0);
	eye_region_top = face.height * (kEyePercentTop/100.0);

	cv::Rect leftEyeRegion(face.width*(kEyePercentSide/100.0),
		eye_region_top,eye_region_width,eye_region_height);
	cv::Rect rightEyeRegion(face.width - eye_region_width - face.width*(kEyePercentSide/100.0),
		eye_region_top,eye_region_width,eye_region_height);

	this->leftEyeRegion = leftEyeRegion;
	this->rightEyeRegion = rightEyeRegion;

	//-- Find Eye Centers
	leftPupil = findEyeCenter(faceROI,leftEyeRegion,"Left Eye");
	rightPupil = findEyeCenter(faceROI,rightEyeRegion,"Right Eye");

	// get corner regions
	cv::Rect leftRightCornerRegion(leftEyeRegion);
	leftRightCornerRegion.width -= leftPupil.x;
	//cout << leftRightCornerRegion.x << endl;
	leftRightCornerRegion.x += leftPupil.x;
	leftRightCornerRegion.height /= 2;
	leftRightCornerRegion.y += leftRightCornerRegion.height / 2;

	cv::Rect leftLeftCornerRegion(leftEyeRegion);
	leftLeftCornerRegion.width = leftPupil.x;
	leftLeftCornerRegion.height /= 2;
	leftLeftCornerRegion.y += leftLeftCornerRegion.height / 2;

	cv::Rect rightLeftCornerRegion(rightEyeRegion);
	rightLeftCornerRegion.width = rightPupil.x;
	rightLeftCornerRegion.height /= 2;
	rightLeftCornerRegion.y += rightLeftCornerRegion.height / 2;

	cv::Rect rightRightCornerRegion(rightEyeRegion);
	rightRightCornerRegion.width -= rightPupil.x;
	rightRightCornerRegion.x += rightPupil.x;
	rightRightCornerRegion.height /= 2;
	rightRightCornerRegion.y += rightRightCornerRegion.height / 2;

	this->leftRightCornerRegion = leftRightCornerRegion;
	this->leftLeftCornerRegion = leftLeftCornerRegion;
	this->rightLeftCornerRegion = rightLeftCornerRegion;
	this->rightRightCornerRegion = rightRightCornerRegion;

	if (debugImage)
	{
		rectangle(debugFace,leftRightCornerRegion,200);
		rectangle(debugFace,leftLeftCornerRegion,200);
		rectangle(debugFace,rightLeftCornerRegion,200);
		rectangle(debugFace,rightRightCornerRegion,200);
	}

	// change eye centers to face coordinates
	rightPupil.x += rightEyeRegion.x;
	rightPupil.y += rightEyeRegion.y;
	leftPupil.x += leftEyeRegion.x;
	leftPupil.y += leftEyeRegion.y;

	if (debugImage)
	{
		circle(debugFace, rightPupil, 3, 1234);
		circle(debugFace, leftPupil, 3, 1234);
	}

	cout << "left: " << leftPupil.x << " " << leftPupil.y << endl;
	cout << "right: " << rightPupil.x << " " << rightPupil.y << endl;

	imshow(face_window_name, faceROI);
}

cv::Point Eyes::findEyeCenter(cv::Mat face, cv::Rect eye, std::string debugWindow)
{
	cv::Mat eyeROIUnscaled = face(eye);
	cv::Mat eyeROI;
	scaleToFastSize(eyeROIUnscaled, eyeROI);
	// draw eye region
	rectangle(face,eye,1234);
	//-- Find the gradient
	cv::Mat gradientX = computeMatXGradient(eyeROI);
	cv::Mat gradientY = computeMatXGradient(eyeROI.t()).t();
	//-- Normalize and threshold the gradient
	// compute all the magnitudes
	cv::Mat mags = matrixMagnitude(gradientX, gradientY);
	//compute the threshold
	double gradientThresh = computeDynamicThreshold(mags, kGradientThreshold);
	//double gradientThresh = kGradientThreshold;
	//double gradientThresh = 0;
	//normalize
	for (int y = 0; y < eyeROI.rows; ++y) {
		double *Xr = gradientX.ptr<double>(y), *Yr = gradientY.ptr<double>(y);
		const double *Mr = mags.ptr<double>(y);
		for (int x = 0; x < eyeROI.cols; ++x) {
			double gX = Xr[x], gY = Yr[x];
			double magnitude = Mr[x];
			if (magnitude > gradientThresh) {
				Xr[x] = gX/magnitude;
				Yr[x] = gY/magnitude;
			} else {
				Xr[x] = 0.0;
				Yr[x] = 0.0;
			}
		}
	}


	//imshow(debugWindow,gradientX);
	//-- Create a blurred and inverted image for weighting
	cv::Mat weight;
	GaussianBlur( eyeROI, weight, cv::Size( kWeightBlurSize, kWeightBlurSize ), 0, 0 );
	for (int y = 0; y < weight.rows; ++y) {
		unsigned char *row = weight.ptr<unsigned char>(y);
		for (int x = 0; x < weight.cols; ++x) {
			row[x] = (255 - row[x]);
		}
	}
	//imshow(debugWindow,weight);
	//-- Run the algorithm!
	cv::Mat outSum = cv::Mat::zeros(eyeROI.rows,eyeROI.cols,CV_64F);
	// for each possible center

	//printf("Eye Size: %ix%i\n",outSum.cols,outSum.rows);
	for (int y = 0; y < weight.rows; ++y) {
		const unsigned char *Wr = weight.ptr<unsigned char>(y);
		const double *Xr = gradientX.ptr<double>(y), *Yr = gradientY.ptr<double>(y);
		for (int x = 0; x < weight.cols; ++x) {
			double gX = Xr[x], gY = Yr[x];
			if (gX == 0.0 && gY == 0.0) {
				continue;
			}
			testPossibleCentersFormula(x, y, Wr[x], gX, gY, outSum);
		}
	}
	// scale all the values down, basically averaging them
	double numGradients = (weight.rows*weight.cols);
	cv::Mat out;
	outSum.convertTo(out, CV_32F,1.0/numGradients);
	//imshow(debugWindow,out);
	//-- Find the maximum point
	cv::Point maxP;
	double maxVal;
	cv::minMaxLoc(out, NULL,&maxVal,NULL,&maxP);
	//-- Flood fill the edges
	if(kEnablePostProcess) {
		cv::Mat floodClone;
		//double floodThresh = computeDynamicThreshold(out, 1.5);
		double floodThresh = maxVal * kPostProcessThreshold;
		cv::threshold(out, floodClone, floodThresh, 0.0f, cv::THRESH_TOZERO);
		if(kPlotVectorField) {
			//plotVecField(gradientX, gradientY, floodClone);
			imwrite("eyeFrame.png",eyeROIUnscaled);
		}
		cv::Mat mask = floodKillEdges(floodClone);
		//imshow(debugWindow + " Mask",mask);
		//imshow(debugWindow,out);
		// redo max
		cv::minMaxLoc(out, NULL,&maxVal,NULL,&maxP,mask);
	}
	return unscalePoint(maxP,eye);
}

void Eyes::scaleToFastSize(const cv::Mat &src,cv::Mat &dst) {
	cv::resize(src, dst, cv::Size(kFastEyeWidth,(((float)kFastEyeWidth)/src.cols) * src.rows));
}

cv::Mat Eyes::computeMatXGradient(const cv::Mat &mat) {
	cv::Mat out(mat.rows,mat.cols,CV_64F);

	for (int y = 0; y < mat.rows; ++y) {
		const uchar *Mr = mat.ptr<uchar>(y);
		double *Or = out.ptr<double>(y);

		Or[0] = Mr[1] - Mr[0];
		for (int x = 1; x < mat.cols - 1; ++x) {
			Or[x] = (Mr[x+1] - Mr[x-1])/2.0;
		}
		Or[mat.cols-1] = Mr[mat.cols-1] - Mr[mat.cols-2];
	}

	return out;
}

void Eyes::testPossibleCentersFormula(int x, int y, unsigned char weight,double gx, double gy, cv::Mat &out) {
	// for all possible centers
	for (int cy = 0; cy < out.rows; ++cy) {
		double *Or = out.ptr<double>(cy);
		for (int cx = 0; cx < out.cols; ++cx) {
			if (x == cx && y == cy) {
				continue;
			}
			// create a vector from the possible center to the gradient origin
			double dx = x - cx;
			double dy = y - cy;
			// normalize d
			double magnitude = sqrt((dx * dx) + (dy * dy));
			dx = dx / magnitude;
			dy = dy / magnitude;
			double dotProduct = dx*gx + dy*gy;
			dotProduct = std::max(0.0,dotProduct);
			// square and multiply by the weight
			if (kEnableWeight) {
				Or[cx] += dotProduct * dotProduct * (weight/kWeightDivisor);
			} else {
				Or[cx] += dotProduct * dotProduct;
			}
		}
	}
}

bool Eyes::floodShouldPushPoint(const cv::Point &np, const cv::Mat &mat) {
	return inMat(np, mat.rows, mat.cols);
}

// returns a mask
cv::Mat Eyes::floodKillEdges(cv::Mat &mat) {
	rectangle(mat,cv::Rect(0,0,mat.cols,mat.rows),255);

	cv::Mat mask(mat.rows, mat.cols, CV_8U, 255);
	std::queue<cv::Point> toDo;
	toDo.push(cv::Point(0,0));
	while (!toDo.empty()) {
		cv::Point p = toDo.front();
		toDo.pop();
		if (mat.at<float>(p) == 0.0f) {
			continue;
		}
		// add in every direction
		cv::Point np(p.x + 1, p.y); // right
		if (floodShouldPushPoint(np, mat)) toDo.push(np);
		np.x = p.x - 1; np.y = p.y; // left
		if (floodShouldPushPoint(np, mat)) toDo.push(np);
		np.x = p.x; np.y = p.y + 1; // down
		if (floodShouldPushPoint(np, mat)) toDo.push(np);
		np.x = p.x; np.y = p.y - 1; // up
		if (floodShouldPushPoint(np, mat)) toDo.push(np);
		// kill it
		mat.at<float>(p) = 0.0f;
		mask.at<uchar>(p) = 0;
	}
	return mask;
}