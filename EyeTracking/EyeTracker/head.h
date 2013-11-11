#include <opencv2/objdetect/objdetect.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace std;

class Head {

    public:
        Head(cv::Rect Faces);
        void detectAndDisplay(cv::Mat frame);
        
   




    private:
        cv::Rect faces;    

};
