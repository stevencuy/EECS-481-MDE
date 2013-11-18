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
	Coordinate calculateGazePosition(Head * head);


private:
	GazeEstimator();
};

#endif

