#include <iostream>
#include <Windows.h>
#include <WinUser.h>
#include "Head.h"
#include "Eyes.h"
#include "Cursor.h"
#include "GazeEstimator.h"


int main(int argc, char **argv)
{
	Head * head = new Head();
	GazeEstimator gaze = GazeEstimator::getInstance();
	Cursor cursor = Cursor::getInstance();
	CvCapture * capture;
	cv::Mat frame;
	cv::Point screen_tl(0,0);
	cv::Point screen_br(0,0);
	Coordinate previous;
	previous.xCoordinate = 0;
	previous.yCoordinate = 0;

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
			//Kill with ESC
			if (GetAsyncKeyState(VK_ESCAPE)) {
				return 0;
			}
			frame = cvQueryFrame(capture);
			cv::flip(frame, frame, 1);
			frame.copyTo(debugImage);
			
			if (!frame.empty())
			{
				head->detectAndDisplay(frame);
			}
			//SET TL BR
			if (GetAsyncKeyState(VK_UP)) { //TL
				screen_tl.x = (head->eyes->leftPupil.x + head->eyes->rightPupil.x) / 2;
				screen_tl.y = (head->eyes->leftPupil.y + head->eyes->rightPupil.y) / 2;
			}
			if (GetAsyncKeyState(VK_DOWN)) { //BR
				screen_br.x = (head->eyes->leftPupil.x + head->eyes->rightPupil.x) / 2;
				screen_br.y = (head->eyes->leftPupil.y + head->eyes->rightPupil.y) / 2;
			}

			imshow(main_window_name, debugImage);
			cvWaitKey(20);
			if (abs(screen_br.x-screen_tl.x) > 0 && abs(screen_br.y-screen_tl.y) > 0) {
				Coordinate coor = gaze.calculateGazePosition(head, screen_tl, screen_br, previous);
				previous = coor;
				cursor.setPosition(coor);
			}
			Sleep(50);
		}
	}
	else
	{
		printf("--(!) Did not detect a camera");
	}

	return 0;
}