#include <iostream>
#include <Windows.h>
#include <WinUser.h>
#include "Head.h"
#include "Eyes.h"
#include "Cursor.h"
#include "GazeEstimator.h"


int main(int argc, char **argv)
{
	//Hide Console
	HWND console = GetConsoleWindow();
	ShowWindow(console, 0);

	Head * head = new Head();
	GazeEstimator gaze = GazeEstimator::getInstance();
	Cursor cursor = Cursor::getInstance();
	CvCapture * capture;
	cv::Mat frame;
	cv::Point_<double> screen_tl((double)-1,(double)-1);
	cv::Point_<double> screen_br((double)-1,(double)-1);
	std::deque <cv::Point_<double>> previous_deque;
	
	//Moving Average
	cv::Point_<double> previous_average(0,0);

	if (!face_cascade.load(face_cascade_name))
	{
		printf("--(!)Error loading\n");
		return -1;
	}

	capture = cvCaptureFromCAM(1);
	if (capture)
	{
		while (1)
		{
			//Kill with ESC
			if (GetAsyncKeyState(VK_ESCAPE)) {
				break;
			}
			//Capture
			frame = cvQueryFrame(capture);
			cv::flip(frame, frame, 1);
			frame.copyTo(debugImage);
			//Check Draw Text for Calibration Prompts
			if (screen_tl.x == -1) {
				putText(debugImage, "Look at the TOP LEFT corner of your screen and press UP ARROW KEY", cvPoint(10,460),1, 0.9, cvScalar(0, 255, 255), 1, 8, false);
			} else if (screen_br.x == -1) {
				putText(debugImage, "Look at the BOTTOM RIGHT corner of your screen and press DOWN ARROW KEY", cvPoint(10,460),1, 0.9, cvScalar(0, 255, 255), 1, 8, false);
			} else {
				putText(debugImage, "Press ESC to exit", cvPoint(10,460),1, 0.9, cvScalar(0, 255, 255), 1, 8, false);
			}
			
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
			if (GetAsyncKeyState(VK_RIGHT)) { //CLEAR
				screen_br.x = 0;
				screen_br.y = 0;
				screen_tl.x = 0;
				screen_tl.y = 0;
				while(!previous_deque.empty()) {
					previous_deque.pop_back();
				}
			}

			imshow(main_window_name, debugImage);
			cvWaitKey(20);
			if (screen_tl.x != -1 && screen_tl.y != -1 && screen_br.x != -1 && screen_br.y != -1) {
				cv::Point_<double> coor = gaze.calculateGazePosition(head, screen_tl, screen_br, previous_average, previous_deque);
				previous_average = coor;
				if (head->faces.size() > 0)
					cursor.setPosition(coor);
			}
			Sleep(50);
		}
	}
	else
	{
		printf("--(!) Did not detect a camera");
	}

	cvReleaseCapture(&capture);
	DestroyWindow(console);

	return 0;
}
