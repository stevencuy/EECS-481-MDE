#include <iostream>
#include <cmath>
#include <wtypes.h>
#include "GazeEstimator.h"

GazeEstimator::GazeEstimator()
{
}

cv::Point GazeEstimator::calculateGazePosition(Head * head, cv::Point screen_tl, cv::Point screen_br, cv::Point previous)
{
	cv::Point coor;
	cv::Point Pupil((head->eyes->leftPupil.x + head->eyes->rightPupil.x )/ 2, (head->eyes->leftPupil.y + head->eyes->rightPupil.y) / 2);
	//Main logic for Estimation
	int horizontal = 0;
	int vertical = 0;
	getResolution(horizontal, vertical);
	double x = (Pupil.x - screen_tl.x) * (horizontal / abs (screen_tl.x - screen_br.x));
	double y = (Pupil.y - screen_tl.y) * (vertical / abs (screen_tl.y - screen_br.y));
	printf("Estimate: [ %g , %g ]\n", x, y);
	coor.x = (x + previous.x) / 2;
	coor.y = (y + previous.y) / 2;
	printEyeData(head, screen_tl, screen_br);
	return coor;
}

void GazeEstimator::getResolution(int& horizontal, int& vertical)
{
   RECT screen;
   // Get a screen and window
   const HWND window = GetDesktopWindow();
   GetWindowRect(window, &screen);
   horizontal = screen.right;
   vertical = screen.bottom;
}

void GazeEstimator::printEyeData(Head * head, cv::Point screen_tl, cv::Point screen_br)
{
	//cout << "left: " << head->eyes->leftPupil.x << " " << head->eyes->leftPupil.y << endl;
	//cout << "right: " << head->eyes->rightPupil.x << " " << head->eyes->rightPupil.y << endl;
}