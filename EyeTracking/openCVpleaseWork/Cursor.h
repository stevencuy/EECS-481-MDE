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
	int setPosition(Coordinate coor);

private:
	Cursor();
	Cursor(Coordinate CursorPosition);
	Coordinate cursorPosition;
};



#endif /* defined(__EyeTracker__GazeEstimatoea__) */
