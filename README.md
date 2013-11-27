EECS-481-MDE
============

To 481 instructors: Our Beta submission consists of files located in folders 1 and 2.

Folder descriptions:

1) SETKeyboard - All the GUI code for the on-screen keyboard is located here.

2) EyeTracking - All the Pupil Tracking and Gaze Estimation classes are located here.

3) Doumentation_gui - Documentation resources related to GUI

4) Documentation_eyetracking - Documentation resources related to eyetracking

5) Documentation - Submitted documents


SETKeyboard:
-------------
**Build Instructions:**
(Using Visual Studio 2013)

**Install Intel Perceptual Computing SDK (Used for text to speech output):**

 1. Navigate to: http://software.intel.com/en-us/vcsource/tools/perceptual-computing-sdk
 2. Select "Download" on the right of the page
 3. Install with default settings (Takes a couple of minutes)
 4. Restart computer

**Build Source:**
 1. Clone master branch to pc
 2. Navigate to EECS-481-MDE/SETKeyboard/
 3. Open SETKeyboard.sln
 4. Select run "Start" button (green play button)
 
**Using**
**Dwell Time**
 * Every button within the application can be fired after being hovered over for a constant amount of time called the "dwell time."
 * Suggested word completions/predictions are displayed in the bar in between the console and the tab bar
 * For the T9 keyboard, characters attributed to the selected button are cycled through by holding the cursor over the button


Eyetracking:
-------------
Build Instructions:

1. Unzip opencv to root "C:/"

2. Computer -> Properties -> Advanced System settings -> Advanced -> Environment Variables -> System Variables -> Path
Add "C:\opencv\install\bin" to Path

3. Open EyeTracking in Visual Studio Express 2012

4. Select Debug and Build

Calibration Instructions:


