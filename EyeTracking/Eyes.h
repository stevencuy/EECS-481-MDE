#ifndef EYES_H
#define EYES_H

#include "globals.h"

class Eyes
{
public:
	Eyes();
	void findEyes(cv::Mat frame_gray, cv::Rect face);

	double eyeRegionWidth;
	double eyeRegionHeight;
	double eyeRegionTop;

	cv::Rect leftEyeRegion, rightEyeRegion;
	cv::Point_<double> leftPupil, rightPupil;

private:
	cv::Point_<double> getEyeCenter(cv::Mat face, cv::Rect eye, std::string debugWindow);
	cv::Point_<double> unscalePoint(cv::Point_<double> p, cv::Rect origSize);
	cv::Mat computeMatGradient(const cv::Mat &mat);
	void scaleToFastSize(const cv::Mat &src, cv::Mat &dst);
	void centersEquation(int x, int y, unsigned char weight,double gx, double gy, cv::Mat &out);
	bool postProcessCheck(const cv::Point_<double> &np, const cv::Mat &mat);
	cv::Mat smoothEdges(cv::Mat &mat);
};

#endif
