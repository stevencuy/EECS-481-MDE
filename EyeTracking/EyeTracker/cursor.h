#include "globals.h"

Class Cursor{

    public:
        Cursor(Coordinate CursorPosition, cv::Image CursorImage);
        /*
         * Displays cursor on the screen
         */
        void printCursor(cv::Image CursorImage);
        

    private:
        Coordinate cursorPosition;
        cv::Image cursorImage;
};
