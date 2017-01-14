<dl>
  <a href="https://breadplayer.com/"><img height="150" src="http://i.imgur.com/PNMSGUr.png" title="breadplayer"/></a>
  <a href="https://www.microsoft.com/en-gb/store/p/bread-player/9nblggh42srx/"><img height="80" src="https://assets.windowsphone.com/f2f77ec7-9ba9-4850-9ebe-77e366d08adc/English_Get_it_Win_10_InvariantCulture_Default.png" title="Get it from Windows Store!" alt="Get it from Windows Store!"/></a>
  <h1>Bread Player aka <em>Macalifa</em></h1>
  <p>Bread Player, a free and open source music player powered by UWP and C#/.NET with a sleek and polished design built for, and by, the people seeking a better alternative to Groove and Windows Media Player by Microsoft.</p>
</dl> 

[![Facebook](https://img.shields.io/badge/like%20us%20on-facebook-blue.svg)](https://www.facebook.com/yourbreadplayer/)
[![Join the chat at https://gitter.im/BreadPlayer/Lobby](https://badges.gitter.im/BreadPlayer/Lobby.svg)](https://gitter.im/BreadPlayer/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
 
[![suggestions here](https://img.shields.io/badge/give%20your-suggestions%20here-orange.svg)](https://github.com/theweavrs/BreadPlayer/issues/17)
[![ui related issues](https://img.shields.io/badge/ui%20related-issues%20here-brightgreen.svg)](https://github.com/theweavrs/BreadPlayer/issues/21)
 
[![alpha](https://img.shields.io/badge/alpha-v1.1.0-red.svg)](https://github.com/theweavrs/BreadPlayer/releases/tag/v0.1.1-alpha)
![build-status](https://ci.appveyor.com/api/projects/status/hphdwx2riesha37e?svg=true)
[![Nightly-Builds](https://img.shields.io/badge/download-nightly%20build-brightgreen.svg)](https://ci.appveyor.com/api/projects/theweavrs/breadplayer/artifacts/BreadPlayer.Core\AppPackages\BreadPlayer.Core_1.1.0.0_Test\BreadPlayer.Core_1.1.0.0_x86_x64_arm.appxbundle)

##Current Status:
###All Alpha features have been implemented. Bug hunting is now underway. Want to help? [Please grab the stable .appxbundle from here and start away!](https://github.com/theweavrs/BreadPlayer/releases) or [download the nightly build from here](https://ci.appveyor.com/api/projects/theweavrs/breadplayer/artifacts/BreadPlayer.Core\AppPackages\BreadPlayer.Core_1.1.0.0_Test\BreadPlayer.Core_1.1.0.0_x86_x64_arm.appxbundle)

###Found a bug? Report it here on github (recommended) or [email me here](mailto:enkaboot@gmail.com). 

### Alpha Preview (Video):
[![Introducing Bread Player - Alpha Preview](http://i.imgur.com/EEMByA7.png)](https://www.youtube.com/watch?v=xFeIf0GnvaM)
### [Screenshots](https://github.com/theweavrs/BreadPlayer/wiki/Screenshots):
![Imgur](http://i.imgur.com/5lUUhBH.jpg)

###Main Features:
1. Flawlessly plays all major formats (mp3, wav, flac, ogg, aiff etc.)! 
2. Full functional music library with sorting, filtering, search etc.
3. Amazing performance i.e. ability to import 12000 songs in 120s with complete tags and album arts.
4. Playlist import (.m3u, .pls etc). (Export coming very soon.)
5. Other basic music player capabilities such as repeat, shuffle etc.
6. Pickup where you left off.
7. Loading songs from Windows Explorer
8. Drag/Drop songs directly into library.

##About the project:
This project is being developed to acknowledge and rectify the scarcity of Music players in Windows 10 Store and also to provide flawless and feature-rich Music player to the end-user. It is still in a very **experimental stage**. [Go here to get an idea of where the project's going.](https://github.com/theweavrs/BreadPlayer/wiki/Road-to-the-first-release)
###Why the name change?
Basically, the main reason behind the name change is that the previous one didn't make any sense but aside from that it wasn't SEO friendly. Who would _ever_ search for the word _Macalifa_? :D The second reason that is why we chose **Bread Player**, is because it is catchy and it invokes curiousity; furthermore, it has a nice ring to it and it is metaphoric...you know breads and burgers :D

###What Happens Next?
We will probably start the development of **Beta Version** after _Alpha_ is released. **Beta-Feature List will be published soon.** 
###Libraries used:
1. C#/.NET
2. UWP API (Windows Aniversary Edition 10.0; Build 14393)
2. [BASS](http://www.un4seen.com/bass.html) & [ManagedBass](https://github.com/ManagedBass/ManagedBass) (for audio processing)
3. [LiteDB](https://github.com/mbdavid/LiteDB) (for library managment)
4. Taglib#

###Contributors:
Thanks to these awesome people Project Bread has come this far:

1. Danny [@DannyTalent](https://github.com/DannyTalent)
2. [@Bond-009](https://github.com/Bond-009)
3. [@MightyK1337](https://github.com/MightyK1337)
4. Kai Hildebrandt [@hildebrandt87](https://github.com/hildebrandt87)

_Note: I am not an expert developer and as a result the code-base isn't as professional as it could be. Hence, I will highly appreciate any contribution in any field regarding this project. All suggestions and issue reporting are welcome._
##Build Notes:
1. Make sure you have the necessary tools for building [Windows Universal Apps](https://dev.windows.com/en-us/develop/building-universal-Windows-apps).
2. Clone this repo:  `git clone https://github.com/theweavrs/BreadPlayer/`
3. The current version uses the latest Aniversary Edition of UWP (Universal Windows Platform) build 14393 but it is not restricted to that, a lower build version can be used without any problems.

###NOTE:
_The current **alpha-of-the-alpha** version is under heavy development and will undergo thousands of changes before made available to the end-user._
