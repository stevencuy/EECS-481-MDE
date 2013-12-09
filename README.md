EECS-481-MDE
============

Github Repo: https://github.com/stevencuy/EECS-481-MDE

 * Github is currently public until the beta score is recieved. Then it will return to private until final release. 

To 481 instructors: Our Beta submission consists of files located in folders 1 and 2.

Please connect an Interactive Gesture Camera to the computer and have it as the only active webcam. 

**Folder descriptions:**

1) SETKeyboard - All the GUI code for the on-screen keyboard is located here.

2) EyeTracking - All the Pupil Tracking and Gaze Estimation classes are located here.

3) Doumentation_gui - Documentation resources related to GUI

4) Documentation_eyetracking - Documentation resources related to eyetracking

5) Documentation - Submitted documents


SETKeyboard:
-------------
**Build Instructions:**
(Using Visual Studio 2012)

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
 * Every button within the application can be fired after being hovered over for a constant amount of time called the "dwell time."
 * Suggested word completions/predictions are displayed in the bar in between the console and the tab bar
 * For the T9 keyboard, characters attributed to the selected button are cycled through by holding the cursor over the button


Eyetracking:
-------------
**Build Instructions:**
(Using Visual Studio 2012 or Express)

**Install opencv:**
 1. Download opencv.zip from https://drive.google.com/file/d/0BzjT9l5eslJuQlJXd2pVQlh5SE0/edit?usp=sharing
 2. Unzip opencv to root "C:/"
 3. Computer -> Properties -> Advanced System settings -> Advanced -> Environment Variables -> System Variables -> Path
    Add "C:\opencv\install\bin" to Path

**Build Source:**
 1. Clone master branch to pc
 2. Navigate to EECS-481-MDE/EyeTracking/
 3. Select run "Start" button (green play button) for debug not release.

**Using**
 * The head must be very still when using the eyetracker
 * After starting the application, without moving the head, look at the top left corner of the screen, focus, and press      **UP ARROW KEY**
 * Then look at the bottom right corner of the screen with your head in the same position and press **DOWN ARROW KEY**
 * At this point the cursor will take control
 * Look around and focus on characters without moving your head dramatically
 * One can recalibrate both corners if the cursor gets thrown off
 * Press **ESC** at any time to kill the program and cursor control.
 * Use the application in proper lighting position and be close enough to the screen.

 	If any issues please contact Steven Uy at stevenuy@umich.edu

Bug Report
https://docs.google.com/a/umich.edu/document/d/1k1pvHtocLwMiVMthVXYbPCYa0DvfDzIZSqlNXdG0Xx0/edit?usp=sharing


----------------
**Team Set: Anthony Chiang, Deng Ke Teo, Bryant Ku, Steven Louie, Mark Mevorah, Jason Terranova, Steven Uy**
