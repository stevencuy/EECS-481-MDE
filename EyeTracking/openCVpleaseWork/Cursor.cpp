#include <Windows.h>

#include "Cursor.h"

Cursor::Cursor() {

}

Cursor::Cursor(Coordinate pos)
{
	SetCursorPos(pos.xCoordinate, pos.yCoordinate);
}

int Cursor::setPosition(Coordinate pos)
{
	SetCursorPos(pos.xCoordinate, pos.yCoordinate);

	/*
	INPUT input;
    input.type = INPUT_MOUSE;
    input.mi.dwFlags = (MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE | 
		MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP);
    input.mi.mouseData = 0;
    input.mi.dwExtraInfo = NULL;
    input.mi.time = 0;
    SendInput(1, &input, sizeof(INPUT));
	*/

	return 0;
}