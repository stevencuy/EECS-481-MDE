#ifndef __EyeTracker__head__
#define __EyeTracker__headr__

#include "globals.h"

class Head {

    public:
        Head(cv::Rect Faces);
        void detectAndDisplay(cv::Mat frame);
        
   




    private:
        cv::Rect faces;    

};




#endif /* defined(__EyeTracker__GazeEstimatoea__) */
