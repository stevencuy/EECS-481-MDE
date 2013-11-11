#include "head.h"

Head::Head() {


}

Head::Head(cv::Rect Faces) {
    faces = Faces;
}

void Head::detectAndDisplay(cv:: Mat frame) {
    std::vector<cv::Rect> faces;
    //cv::Mat frame_gray;
    std::vector<cv::Mat> rgbChannels(3);
    cv::split(frame, rgbChannels);
    cv::Mat frame_gray = rgbChannels[2];
      
    //cvtColor( frame, frame_gray, CV_BGR2GRAY );
    //equalizeHist( frame_gray, frame_gray );
    //cv::pow(frame_gray, CV_64F, frame_gray);
    //-- Detect faces
    face_cascade.detectMultiScale( frame_gray, faces, 1.1, 2, 0|CV_HAAR_SCALE_IMAGE|CV_HAAR_FIND_BIGGEST_OBJECT, cv::Size(150, 150) );
    //  findSkin(debugImage);
    
    for( int i = 0; i < faces.size(); i++ )
    {
        rectangle(debugImage, faces[i], 1234);
    }
    //-- Show what you got
    if (faces.size() > 0) {
        findEyes(frame_gray, faces[0]);
    }

}
