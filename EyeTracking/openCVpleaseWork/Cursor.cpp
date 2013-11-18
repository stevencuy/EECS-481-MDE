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
	return 0;
}