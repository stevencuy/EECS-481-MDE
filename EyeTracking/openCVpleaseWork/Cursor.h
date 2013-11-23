#ifndef CURSORS_H
#define CURSORS_H

#include "globals.h"

class Cursor
{
public:
    static Cursor& getInstance()
	{
		static Cursor instance;
		return instance;
	}
	int setPosition(cv::Point_<double> coor);

private:
	Cursor();
	Cursor(cv::Point_<double> CursorPosition);
	cv::Point_<double> cursorPosition;
};



#endif /* defined(__EyeTracker__GazeEstimatoea__) */
