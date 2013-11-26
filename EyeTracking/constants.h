#ifndef CONSTANTS_H
#define CONSTANTS_H

//Constants required for pupil tracking

//Debugging
const bool kPlotVectorField = false;

//Size constants
const double kEyePercentTop = 32;
const double kEyePercentSide = 13;
const double kEyePercentHeight = 15;
const double kEyePercentWidth = 30;

//Preprocessing
const bool kSmoothFaceImage = false;
const float kSmoothFaceFactor = 0.005;

//Algorithm Parameters
const double kFastEyeWidth = 50;
const double kWeightBlurSize = 5;
const bool kEnableWeight = false;
const float kWeightDivisor = 150.0;
const double kGradientThreshold = 50.0;

//Postprocessing
const bool kEnablePostProcess = true;
const float kPostProcessThreshold = 0.97;

//Eye Corner
const bool kEnableEyeCorner = false;

#endif
