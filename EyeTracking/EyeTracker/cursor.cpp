#include "cursor.h"

Cursor::Cursor() {

}

Cursor::Cursor(Coordinate CursorPosition, cv::Image CursorImage) {
    cursorPosition = CursorPosition;
    cursorImage = CursorImage;
}


void Cursor::printCursor(cv::Image CursorImage) {


}
