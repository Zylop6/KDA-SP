
  

# KDA-SoundPlayer

## Version: KDA-SP_v1.0.1

  
This document provides information about KDA-SP v1.0.1. If you are interested in other versions, please visit the releases page and choose the version you prefer. Each version comes with a README file that provides detailed instructions on how to install and use that particular version.

I highly recommend referring to the README file of your chosen version for a smooth installation and seamless user experience. The README file will provide you with all the necessary information, including installation instructions, ChangeLogs, and other notes or details specific to that version.

Feel free to explore the available versions on GitHub and select the one that best suits your needs.

## **Description:**


* **KDA-SP (KDA-Sound player)** is a simple program that allows you to play a goofy sound when you die, get a kill, or assist someone in games like League of Legends.

	I came up with this trivial idea while playing League of Legends with a friend and getting frustrated, and we thought about how annoying it would be to be laughed at every time we died in-game. From that, the idea of developing a sound player that does just that was born: to push you a little closer to madness.

## Installation:
1.  Navigate to the release page of the KDA-SP repository.
    
2. Select the desired version.
    
3. Download the KDA-SP_vX.zip file.
    
4. Extract the file to a location on your computer where it can be easily accessed or removed if necessary.
    
5.  Inside the created folder, you will find a shortcut. Drag this shortcut to your desktop.

6. Launch the program by clicking on the KDA-SP shortcut.

## Instructions:

![KDA-SP_Main](https://github.com/Zylop6/KDA-SP/assets/124086107/5de05bc8-7fc0-415c-bbea-efc085375847)

 **1)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This button enables you to select an area on your screen for scanning 
and detecting your KDA values. Press Enter to confirm your selection.

![KDA-SP_Selecting_area](https://github.com/Zylop6/KDA-SP/assets/124086107/31190770-32d8-4935-be5e-146229aea136)

 When selecting a detection area, make sure that no symbols unrelated to the KDA values are visible. In the case of this example, the KDA display in League of Legends, the sword can be erroneously recognized as a number through OCR, leading to calculation problems and incorrect results.

Additionally, it is advisable to leave some space in the direction where the text expands, in case the values shift in that direction and can no longer be detected.

**2)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This is the start button. It initiates the OCR process and scans your KDA.

**3)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This is the stop button. It halts any ongoing calculations and scans.

**4)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This picture box displays your currently selected area. It refreshes with every scan cycle, primarily to  verify 
if you have chosen the right area or if it covers enough content.

**5)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;These buttons let you choose a ".wav" or ".mp3" file to play when you get a kill, die, or assist.

**6)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;These are the volume controls. Adjust them to make your sounds louder or mute them as needed.

**7)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This checkbox allows you to quickly mute all sounds.

**8)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Here, the titles of your selected sounds are shown.

**9)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Use these buttons to test your sounds. Note that testing sounds while scanning **may cause some audio errors**.

**10)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Your KDA will be displayed here. You can quickly check here if it matches with your ingame values.

**11)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;You can see your KDA-R (Kill-Death-Assist-Ratio) here.

**12)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Here, you can view your KD-R (Kill-Death-Ratio).

**13)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Toggle the picture box to show what the final OCR is "looking at". This may slightly slow down the process but can help you ensure that your area is correctly detected.

**14)** &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This button allows you to enable or disable the Extra section of KDA-SP.
<br /><br />
### The extra menu:
![KDA-SP_Extra](https://github.com/Zylop6/KDA-SP/assets/124086107/27ea031e-9cb0-4f3e-a727-4d2a65b10f30)

**1)** 
* "Raw output" displays the unprocessed output of the OCR, showing the raw data obtained from the Tesseract engine as it analyzes your selected area.
* Clean output" displays your KDA and/or missing values marked by "?" if any of the values are not detected correctly or are nonexistent.
These two outputs are useful for debugging purposes as they allow you to compare the recognized values with the actual output used for KDA-R and KD-R calculations.
___
**2)** 
* "CPU usage" shows the current average CPU usage. This is helpful for determining the optimal scan interval during testing.

* "Memory usage" also aids in detecting memory leaks, which occur when memory is not properly released, leading to excessive RAM usage by the program. Monitoring the memory usage can help identify instances where memory is not being freed correctly, resulting in the program consuming an excessive amount of RAM. This information is valuable for diagnosing and addressing issues related to poor memory release and optimizing the program's memory management.
___
**3)** 
* This slider adjusts the time between scans. It can reduce the latency between the in-game value changes and the corresponding sound being played. However, **extremely low values can cause performance issues or audio errors** on certain machines. The standard value is **125ms** between each scan, resulting in approximately 8 scans per second.
___
<br /><br />

**Notes:**

* **Efficiency**:
Please note that I am **not** a professional programmer, so I have designed the code in a way that makes sense to me. As a result, the code may be **inefficient**, **somewhat janky**, or **spaghetti-like**.

 * **OCR**: 
OCR is not always 100% accurate. The main issue right now is the changing background, which can affect the detection of KDA values. Based on my tests, it works correctly about 90% of the time. If it mistakenly recognizes a value, it will be corrected in the next scan, so false results are less likely. Simply adjusting your camera can fix false detections, but if the problem continues, try selecting the correct detection again.

In some cases, OCR may cause **performance issues** that could affect the gaming experience. Everything works fine on **my machine** and only takes a toll on performance due to my outdated hardware. On any reasonably newer CPUs (i5+), this program should have virtually no noticeable impact.

* ***Everything has been tested on the following build:***

i5 4460 ⠀  
1050TI ⠀  
16GB DDR3 1600MHz 

##


## Support

At the moment, there is no option for financial support. However, this may change in the future. Please refer to the latest version of KDA-SP and its corresponding README file to check for any updates regarding financial support.

### Other ways to support me:
You can also show your support by subscribing to my YouTube channel and following me on other social media platforms. Your engagement and participation are greatly appreciated!

I value any kind of support, whether it's through financial contributions or simply spreading the word about my work. Thank you for your support!




## License and Copyright:

 * Copyright (C) 2023 Zylop6
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
