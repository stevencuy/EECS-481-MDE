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
	Coordinate calculateGazePosition(Head * head, cv::Point screen_tl, cv::Point screen_br);
	void printEyeData(Head *, cv::Point screen_tl, cv::Point screen_br);


private:
	GazeEstimator();
};

#endif

