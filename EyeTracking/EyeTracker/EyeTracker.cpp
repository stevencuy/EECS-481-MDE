#include "Head.h"
#include "Eyes.h"
#include "GazeEstimator.h"

int main(int argc, char **argv)
{
	Head * head = new Head;
	GazeEstimator * gaze = GazeEstimator::getInstance();
	Cursor * cursor = Cursor::getInstance();
	
	while (1)
	{
		Coordinate coor = gaze->calculateGazePosition(head);
		cursor->setPosition(coor);
	}
}