#ifndef HEAD_H
#define HEAD_H

#include "globals.h"
#include "constants.h"
#include "Eyes.h"

class Head
{
public:
	Head();
	void detectAndDisplay(cv::Mat frame, cv::CascadeClassifier face_cascade, cv::Mat * debugImage);

private:
	Eyes * eyes;
	cv::Rect faces;
};

#endif /* defined(__EyeTracker__GazeEstimatoea__) */
