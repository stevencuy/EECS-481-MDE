#include <iostream>

#include "GazeEstimator.h"

GazeEstimator::GazeEstimator()
{
}

Coordinate GazeEstimator::calculateGazePosition(Head * head)
{
	printEyeData(head);
	return Coordinate();
}

void GazeEstimator::printEyeData(Head * head)
{
	cout << "left: " << head->eyes->leftPupil.x << " " << head->eyes->leftPupil.y << endl;
	cout << "right: " << head->eyes->rightPupil.x << " " << head->eyes->rightPupil.y << endl;
}