#include <iostream>
#include <queue>

#include "constants.h"
#include "globals.h"
#include "EyeUtility.h"
#include "Eyes.h"

Eyes::Eyes()
{
	leftPupil.x = -1;
	leftPupil.y = -1;
	rightPupil.x = -1;
	rightPupil.y = -1;
}

cv::Point_<double> Eyes::unscalePoint(cv::Point_<double> p, cv::Rect origSize)
{
	float ratio = (((float)kFastEyeWidth)/origSize.width);
	int x = (p.x / ratio);
	int y = (p.y / ratio);
	return cv::Point(x,y);
}

void Eyes::findEyes(cv::Mat frame_gray, cv::Rect face)
{
	cv::Mat faceROI = frame_gray(face);
	cv::Mat debugFace = faceROI;

	if (kSmoothFaceImage) {
		double sigma = kSmoothFaceFactor * face.width;
		GaussianBlur( faceROI, faceROI, cv::Size( 0, 0 ), sigma);
	}

	//-- Find eye regions and draw them
	eyeRegionWidth = face.width * (kEyePercentWidth/100.0);
	eyeRegionHeight = face.width * (kEyePercentHeight/100.0);
	eyeRegionTop = face.height * (kEyePercentTop/100.0);

	cv::Rect leftEyeRegion(face.width*(kEyePercentSide/100.0),
		eyeRegionTop,eyeRegionWidth,eyeRegionHeight);
	cv::Rect rightEyeRegion(face.width - eyeRegionWidth - face.width*(kEyePercentSide/100.0),
		eyeRegionTop,eyeRegionWidth,eyeRegionHeight);

	this->leftEyeRegion = leftEyeRegion;
	this->rightEyeRegion = rightEyeRegion;

	//Find Eye Centers
	cv::Point lp = getEyeCenter(faceROI,leftEyeRegion,"Left Eye");
	cv::Point rp = getEyeCenter(faceROI,rightEyeRegion,"Right Eye");

	int lxdiff = lp.x - leftPupil.x;
	int rxdiff = rp.x - rightPupil.x;
	int lydiff = lp.y - leftPupil.y;
	int rydiff = rp.y - rightPupil.y;

	if ((abs(lxdiff - rxdiff) < 5) ||
		(abs(lydiff - rydiff) < 5) ||
		(leftPupil.x == -1 && rightPupil.x == -1))
	{
		leftPupil = lp;
		rightPupil = rp;
	}

	// change eye centers to face coordinates
	rightPupil.x += rightEyeRegion.x + face.x;
	rightPupil.y += rightEyeRegion.y + face.y;
	leftPupil.x += leftEyeRegion.x + face.x;
	leftPupil.y += leftEyeRegion.y + face.y;
	cv::Point leftEye_br(leftEyeRegion.br().x + face.x, leftEyeRegion.br().y + face.y);
	cv::Point leftEye_tl(leftEyeRegion.tl().x + face.x, leftEyeRegion.tl().y + face.y);
	cv::Point rightEye_br(rightEyeRegion.br().x + face.x, rightEyeRegion.br().y + face.y);
	cv::Point rightEye_tl(rightEyeRegion.tl().x + face.x, rightEyeRegion.tl().y + face.y);

	rectangle(debugImage, leftEye_br, leftEye_tl, cvScalar(0, 255, 255), 1);
	rectangle(debugImage, rightEye_br, rightEye_tl, cvScalar(0, 255, 255), 1);
	circle(debugImage, rightPupil, 3, cvScalar(0, 255, 255), -1) ;
	circle(debugImage, leftPupil, 3, cvScalar(0, 255, 255), -1) ;
}

cv::Point_<double> Eyes::getEyeCenter(cv::Mat face, cv::Rect eye, std::string debugWindow)
{
	cv::Mat eyeROIUnscaled = face(eye);
	cv::Mat eyeROI;
	scaleToFastSize(eyeROIUnscaled, eyeROI);
	//draw eye region
	rectangle(face,eye,1234);
	//Find gradient
	cv::Mat gradientX = computeMatGradient(eyeROI);
	cv::Mat gradientY = computeMatGradient(eyeROI.t()).t();
	//Compute all the magnitudes
	cv::Mat mags = matrixMagnitude(gradientX, gradientY);
	//compute the threshold
	double gradientThresh = computeDynamicThreshold(mags, kGradientThreshold);
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

	//create a blurred and inverted image for weighting
	cv::Mat weight;
	GaussianBlur( eyeROI, weight, cv::Size( kWeightBlurSize, kWeightBlurSize ), 0, 0 );
	for (int y = 0; y < weight.rows; ++y) {
		unsigned char *row = weight.ptr<unsigned char>(y);
		for (int x = 0; x < weight.cols; ++x) {
			row[x] = (255 - row[x]);
		}
	}
	//Pupil Tracking algorithm
	cv::Mat outSum = cv::Mat::zeros(eyeROI.rows,eyeROI.cols,CV_64F);
	// for each possible center
	for (int y = 0; y < weight.rows; ++y) {
		const unsigned char *Wr = weight.ptr<unsigned char>(y);
		const double *Xr = gradientX.ptr<double>(y), *Yr = gradientY.ptr<double>(y);
		for (int x = 0; x < weight.cols; ++x) {
			double gX = Xr[x], gY = Yr[x];
			if (gX == 0.0 && gY == 0.0) {
				continue;
			}
			centersEquation(x, y, Wr[x], gX, gY, outSum);
		}
	}
	// scale all the values down, basically averaging them
	double numGradients = (weight.rows*weight.cols);
	cv::Mat out;
	outSum.convertTo(out, CV_32F,1.0/numGradients);
	//Find the maximum point
	cv::Point maxP;
	double maxVal;
	cv::minMaxLoc(out, NULL,&maxVal,NULL,&maxP);
	//Smoothing
	if(kEnablePostProcess) {
		cv::Mat floodClone;
		double floodThresh = maxVal * kPostProcessThreshold;
		cv::threshold(out, floodClone, floodThresh, 0.0f, cv::THRESH_TOZERO);
		if(kPlotVectorField) {
			imwrite("eyeFrame.png",eyeROIUnscaled);
		}
		cv::Mat mask = smoothEdges(floodClone);
		cv::minMaxLoc(out, NULL,&maxVal,NULL,&maxP,mask);
	}
	return unscalePoint(maxP,eye);
}

void Eyes::scaleToFastSize(const cv::Mat &src,cv::Mat &dst) {
	cv::resize(src, dst, cv::Size(kFastEyeWidth,(((float)kFastEyeWidth)/src.cols) * src.rows));
}

cv::Mat Eyes::computeMatGradient(const cv::Mat &mat) {
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

void Eyes::centersEquation(int x, int y, unsigned char weight,double gx, double gy, cv::Mat &out) {
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

bool Eyes::postProcessCheck(const cv::Point_<double> &np, const cv::Mat &mat) {
	return inMat(np, mat.rows, mat.cols);
}

cv::Mat Eyes::smoothEdges(cv::Mat &mat) {
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
		if (postProcessCheck(np, mat)) toDo.push(np);
		np.x = p.x - 1; np.y = p.y; // left
		if (postProcessCheck(np, mat)) toDo.push(np);
		np.x = p.x; np.y = p.y + 1; // down
		if (postProcessCheck(np, mat)) toDo.push(np);
		np.x = p.x; np.y = p.y - 1; // up
		if (postProcessCheck(np, mat)) toDo.push(np);
		// kill edges
		mat.at<float>(p) = 0.0f;
		mask.at<uchar>(p) = 0;
	}
	return mask;
}
