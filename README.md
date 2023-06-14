
  

# KDA-SoundPlayer

### Latest version:  [KDA-SP_v1.0.1](https://github.com/Zylop6/KDA-SP/releases/tag/v1.0.1)

## **Description:**
KDA-SP (KDA SoundPlayer) is a simple program that allows you to play customized sounds for kill, assist, or death events in games like League of Legends.

The idea for this program came to me and a friend while playing League of Legends. We grew tired of the repetitive kill announcements and hearing "You have been slain" repeatedly. Now, with KDA-SP, you can have your own unique sound played when you die, driving you even closer to madness than before!

With some free time on my hands, I decided to develop this program to refresh my knowledge of C# and delve deeper into software development. While KDA-SP may not be a groundbreaking scientific breakthrough, it presented a challenging opportunity for me to improve my programming skills.

Please note that I am not a seasoned programmer. I developed the program based on my own understanding and iteratively fixed issues while working on optimization. There may still be areas where efficiency can be improved.

I sincerely appreciate any constructive critique, as my goal is to continue learning and develop in the most efficient and best-practice manner possible.
`[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)`
## Changelog
Since this is the initial version of KDA-SP, there is currently no Changelog available. However, future versions of the program will include a Changelog documenting the changes, improvements, and bug fixes introduced in each update.
## Installation:
1.  Navigate to the [release page](https://github.com/Zylop6/KDA-SP/releases) of the program.
2.  Choose your desired version from the available releases.
3.  Read the installation instructions provided in the description of the selected version.

## FAQ:

**Q: What games does KDA-SP support?** 

> A: Currently, KDA-SP is designed specifically for games that display
> KDA information in a format that can be recognized by OCR. The program
> has been tested with League of Legends, but it may also work with
> other similar games that have clear and consistent KDA display
> patterns.
> 
**Q: Can I customize the sounds in KDA-SP?****

> A: Yes, you can customize the sounds played by KDA-SP. The program allows you to select your preferred audio files for kill, death, and assist events. You can choose any audio file in formats like mp3 or wav. However, it's important to note that using very long audio files may result in audio overlapping or errors during playback.

**Q: Is KDA-SP compatible with all operating systems?** 

> A: KDA-SP is compatible with most common operating systems, including
> Windows, macOS, and Linux. However, it's always recommended to check
> the program documentation for specific system requirements and
> compatibility details.

**Q: Can KDA-SP be used in fullscreen games?** 

> A: Yes, KDA-SP can be used with fullscreen games as long as the KDA
> values are visible on the screen. The program captures the necessary
> information directly from your screen, regardless of whether the game
> is running in fullscreen mode or windowed mode.

**Q: Does KDA-SP impact game performance?** 

> A: The impact on game performance may vary depending on the settings
> you choose. OCR can require computational resources, but since KDA-SP
> only scans a small area of the screen, the performance impact is
> generally minimal. However, it's advisable to refer to the
> documentation specific to your version of KDA-SP to understand the
> performance settings and optimize them accordingly.

**Q: Can KDA-SP be used to track KDA values in real-time during a game?** 

> A: Yes, KDA-SP can track KDA values in real-time during gameplay. As
> long as the KDA values are visible on the screen, KDA-SP will update
> and display the values based on your configured settings. It's
> important to note that KDA-SP does not save the KDA values as files or
> logs, and it solely processes them to check for changes and trigger
> the appropriate sound effects.

**Q: Is KDA-SP allowed by game developers?** 

> A: The usage of third-party programs like KDA-SP may vary depending on
> the game and its developers. It's important to review the terms of
> service or end-user license agreement (EULA) of the game you intend to
> use KDA-SP with. While the program does not modify the game itself and
> solely operates based on screen capture and OCR, it's always
> recommended to ensure compliance with the game's policies.

**Q: How can I report issues or provide feedback for KDA-SP?** 

>A: You can report any issues or provide feedback for KDA-SP on the program's GitHub repository. Visit the repository's "Issues" section and create a new issue to describe the problem or share your feedback. I will try my best to help with issues and fix them as needed.

## Used libraries:
- [Tesseract](https://github.com/tesseract-ocr/tesseract) 
<br>
##
### Support

At the moment, there is no option for financial support. However, this may change in the future. Please refer to the latest version of KDA-SP and its corresponding README file to check for any updates regarding financial support.

### Other ways to support me:
You can also show your support by subscribing to my YouTube channel and following me on other social media platforms. Your engagement and participation are greatly appreciated!

I value any kind of support, whether it's through financial contributions or simply spreading the word about my work. Thank you for your support!
<br><br>

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
