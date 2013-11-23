#include <iostream>
#include <cmath>
#include <wtypes.h>
#include "GazeEstimator.h"

GazeEstimator::GazeEstimator()
{
}

cv::Point_<double> GazeEstimator::calculateGazePosition(Head * head, cv::Point_<double> screen_tl, cv::Point_<double> screen_br, std::deque <cv::Point_<double>> &previous_deque)
{
	cv::Point_<double> coor((double)0,(double)0);
	cv::Point_<double> Pupil((double)(head->eyes->leftPupil.x + head->eyes->rightPupil.x )/ 2, (double)(head->eyes->leftPupil.y + head->eyes->rightPupil.y) / 2);
	//Main logic for Estimation
	int horizontal = 0;
	int vertical = 0;
	getResolution(horizontal, vertical);
	double x = (Pupil.x - screen_tl.x) * (horizontal / abs (screen_tl.x - screen_br.x));
	double y = (Pupil.y - screen_tl.y) * (vertical / abs (screen_tl.y - screen_br.y));
	printf("Estimate: [ %g , %g ]\n", x, y);
	coor.x = x;
	coor.y = y;
//	printEyeData(head, screen_tl, screen_br);
	return update_previous_deque(coor, previous_deque);
}

cv::Point_<double> GazeEstimator::update_previous_deque(cv::Point_<double> previous, std::deque <cv::Point_<double>> &previous_deque)
//Manages a dequeue with the previous "span" elements to take the average
//Method for smoothing
{
	unsigned int span = 10;
	double x = 0;
	double y = 0;
	//Manage only 10
	previous_deque.push_back(previous);
	if (previous_deque.size() > span) {
		previous_deque.pop_front();
	}
	//Get Average
	for(unsigned int i = 0; i < previous_deque.size(); ++i) {
		x += previous_deque.front().x;
		y += previous_deque.front().y;
		previous_deque.push_back(previous_deque.front());
		previous_deque.pop_front();
	}
	//Calculate
	cv::Point_<double> est_avg((double) x / previous_deque.size(),(double) y / previous_deque.size());
	return est_avg;
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

void GazeEstimator::printEyeData(Head * head, cv::Point_<double> screen_tl, cv::Point_<double> screen_br)
{
	//cout << "left: " << head->eyes->leftPupil.x << " " << head->eyes->leftPupil.y << endl;
	//cout << "right: " << head->eyes->rightPupil.x << " " << head->eyes->rightPupil.y << endl;
}