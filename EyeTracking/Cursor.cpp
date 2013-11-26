#include <Windows.h>

#include "Cursor.h"

Cursor::Cursor() {

}

Cursor::Cursor(cv::Point_<double> pos)
{
	SetCursorPos(pos.x, pos.y);
}

int Cursor::setPosition(cv::Point_<double> pos)
{
	SetCursorPos(pos.x, pos.y);
	return 0;
}