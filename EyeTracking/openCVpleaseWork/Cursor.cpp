#include <Windows.h>

#include "Cursor.h"

Cursor::Cursor() {

}

Cursor::Cursor(cv::Point pos)
{
	SetCursorPos(pos.x, pos.y);
}

int Cursor::setPosition(cv::Point pos)
{
	SetCursorPos(pos.x, pos.y);
	return 0;
}