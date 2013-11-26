#ifndef EYES_H
#define EYES_H

#include "globals.h"

class Eyes
{
public:
	Eyes();
	void findEyes(cv::Mat frame_gray, cv::Rect face);

	double eye_region_width;
	double eye_region_height;
	double eye_region_top;

	cv::Rect leftEyeRegion, rightEyeRegion;
	cv::Rect leftRightCornerRegion;
	cv::Rect leftLeftCornerRegion;
	cv::Rect rightLeftCornerRegion;
	cv::Rect rightRightCornerRegion;

	cv::Point_<double> leftPupil, rightPupil;

private:
	cv::Point_<double> findEyeCenter(cv::Mat face, cv::Rect eye, std::string debugWindow);
	cv::Point_<double> unscalePoint(cv::Point_<double> p, cv::Rect origSize);
	cv::Mat computeMatXGradient(const cv::Mat &mat);
	void scaleToFastSize(const cv::Mat &src, cv::Mat &dst);
	void testPossibleCentersFormula(int x, int y, unsigned char weight,double gx, double gy, cv::Mat &out);
	bool floodShouldPushPoint(const cv::Point_<double> &np, const cv::Mat &mat);
	cv::Mat floodKillEdges(cv::Mat &mat);
};

#endif