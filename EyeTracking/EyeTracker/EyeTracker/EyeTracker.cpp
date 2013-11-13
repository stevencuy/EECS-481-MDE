#include "Head.h"
#include "Eyes.h"
#include "Cursor.h"
#include "GazeEstimator.h"
#include "globals.h"


int main(int argc, char **argv)
{
	Head * head = new Head();
	GazeEstimator gaze = GazeEstimator::getInstance();
	Cursor cursor = Cursor::getInstance();
	CvCapture * capture;
	cv::Mat frame;

	if (!face_cascade.load(face_cascade_name))
	{
		printf("--(!)Error loading\n");
		return -1;
	}

	ellipse(skinCrCbHist, cv::Point(113, 155.6), cv::Size(23.4, 15.2),
          43.0, 0.0, 360.0, cv::Scalar(255, 255, 255), -1);

	capture = cvCaptureFromCAM(1);
	if (capture)
	{
		while (1)
		{
			frame = cvQueryFrame(capture);
			if (!frame.empty())
			{
				head->detectAndDisplay(frame, face_cascade, &debugImage);
			}
			
			imshow(main_window_name, debugImage);

			Coordinate coor = gaze.calculateGazePosition(head);
			cursor.setPosition(coor);
		}
	}
	else
	{
		printf("--(!) Did not detect a camera");
	}

	return 0;
}