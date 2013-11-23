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
	int setPosition(cv::Point coor);

private:
	Cursor();
	Cursor(cv::Point CursorPosition);
	cv::Point cursorPosition;
};



#endif /* defined(__EyeTracker__GazeEstimatoea__) */
