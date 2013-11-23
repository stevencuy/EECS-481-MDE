#include <iostream>
#include <cmath>
#include "GazeEstimator.h"

GazeEstimator::GazeEstimator()
{
}

Coordinate GazeEstimator::calculateGazePosition(Head * head, cv::Point screen_tl, cv::Point screen_br)
{
	cv::Point Pupil((head->eyes->leftPupil.x + head->eyes->rightPupil.x )/ 2, (head->eyes->leftPupil.y + head->eyes->rightPupil.y) / 2);
	//Main logic for Estimation
	double x = (Pupil.x - screen_tl.x) * (1920 / abs (screen_tl.x - screen_br.x));
	double y = (Pupil.y - screen_tl.y) * (1080 / abs (screen_tl.y - screen_br.y));
	printf("Estimate: [ %g , %g ]\n", x, y);

	printEyeData(head, screen_tl, screen_br);
	return Coordinate();
}

void GazeEstimator::printEyeData(Head * head, cv::Point screen_tl, cv::Point screen_br)
{
	//cout << "left: " << head->eyes->leftPupil.x << " " << head->eyes->leftPupil.y << endl;
	//cout << "right: " << head->eyes->rightPupil.x << " " << head->eyes->rightPupil.y << endl;
}