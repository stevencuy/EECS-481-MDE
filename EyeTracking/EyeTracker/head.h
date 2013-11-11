#include "globals.h"

class Head {

    public:
        Head(cv::Rect Faces);
        void detectAndDisplay(cv::Mat frame);
        
   




    private:
        cv::Rect faces;    

};
