#ifndef GAZE_ESTIMATOR_H
#define GAZE_ESTIMATOR_H

#include "Head.h"

class GazeEstimator 
{
public:
	static GazeEstimator& getInstance()
	{
		static GazeEstimator estimator;
		return estimator;
	}
	cv::Point_<double> calculateGazePosition(Head * head, cv::Point_<double> screen_tl, cv::Point_<double> screen_br, std::deque <cv::Point_<double>> &previous_deque);
	void printEyeData(Head *, cv::Point_<double> screen_tl, cv::Point_<double> screen_br);
	void GazeEstimator::getResolution(int& horizontal, int& vertical);
	cv::Point_<double> update_previous_deque(cv::Point_<double> previous, std::deque <cv::Point_<double>> &previous_deque);

private:
	GazeEstimator();
};

#endif

