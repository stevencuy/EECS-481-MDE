#ifndef GLOBALS_H
#define GLOBALS_H

#include <opencv2/objdetect/objdetect.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace std;


/** Global variables */
//-- Note, either copy these two files from opencv/data/haarscascades to your current folder, or change these locations
extern cv::String face_cascade_name;
extern cv::CascadeClassifier face_cascade;
extern std::string main_window_name;
extern std::string face_window_name;
extern cv::RNG rng;
extern cv::Mat debugImage;
extern cv::Mat skinCrCbHist;

struct Coordinate{
    int xCoordinate;
    int yCoordinate;
};

#endif /* defined(__EyeTracker__globals__) */
