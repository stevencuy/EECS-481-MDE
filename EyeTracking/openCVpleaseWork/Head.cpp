#include "Head.h"
#include "Eyes.h"
#include "Utility.h"

Head::Head()
{
	eyes = new Eyes;
}

void Head::detectAndDisplay(cv::Mat frame, cv::Mat * debugImage)
{
	std::vector<cv::Rect> faces;
	std::vector<cv::Mat> rgbChannels(3);
	cv::split(frame, rgbChannels);
	cv::Mat frame_gray = rgbChannels[2];

	face_cascade.detectMultiScale( frame_gray, faces, 1.1, 2, 0|CV_HAAR_SCALE_IMAGE|CV_HAAR_FIND_BIGGEST_OBJECT, cv::Size(150, 150) );

	if (debugImage)
	{
		for( int i = 0; i < faces.size(); i++ )
		{
			rectangle(*debugImage, faces[i], 1234);
		}
	}

	if (faces.size() > 0)
	{
		eyes->findEyes(frame_gray, faces[0], debugImage);
	}
}