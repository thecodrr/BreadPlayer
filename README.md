<dl>
  <a href="https://breadplayer.com/"><img height="150" src="http://i.imgur.com/PNMSGUr.png" title="breadplayer"/></a>
  <a href="https://www.microsoft.com/en-gb/store/p/bread-player/9nblggh42srx/"><img height="80" src="https://assets.windowsphone.com/f2f77ec7-9ba9-4850-9ebe-77e366d08adc/English_Get_it_Win_10_InvariantCulture_Default.png" title="Get it from Windows Store!" alt="Get it from Windows Store!"/></a>
  <h1>Bread Player aka <em>Macalifa</em></h1>
  <p>Bread Player, a free and open source music player powered by UWP and C#/.NET with a sleek and polished design built for, and by, the people seeking a better alternative to Groove and Windows Media Player by Microsoft.</p>
</dl> 

[![Donate|Help Us Grow](https://img.shields.io/badge/Donate-Help%20Us%20Grow-green.svg)](http://blog.breadplayer.com/donate)

[![Facebook](https://img.shields.io/badge/like%20us%20on-facebook-blue.svg)](https://www.facebook.com/yourbreadplayer/)
[![Join the chat at https://gitter.im/BreadPlayer/Lobby](https://badges.gitter.im/BreadPlayer/Lobby.svg)](https://gitter.im/BreadPlayer/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
 
[![suggestions here](https://img.shields.io/badge/give%20your-suggestions%20here-orange.svg)](https://github.com/theweavrs/BreadPlayer/issues/17)
[![ui related issues](https://img.shields.io/badge/ui%20related-issues%20here-brightgreen.svg)](https://github.com/theweavrs/BreadPlayer/issues/21)
 
[![alpha](https://img.shields.io/badge/alpha-v1.3.0-red.svg)](https://github.com/theweavrs/BreadPlayer/releases/tag/v0.1.3.0-alpha)
![build-status](https://ci.appveyor.com/api/projects/status/hphdwx2riesha37e?svg=true)
[![Nightly-Builds](https://img.shields.io/badge/download-nightly%20build-brightgreen.svg)](https://ci.appveyor.com/api/projects/theweavrs/BreadPlayer/artifacts/BreadPlayer.Views.UWP/AppPackages/BreadPlayer.Views.UWP_1.1.0.0_Test/BreadPlayer.Views.UWP_1.1.0.0_x86_x64_arm.appxbundle)

##Current Status:
#### Alpha version has been released and development on the first Beta has started! You can [download the nightly build from here](https://ci.appveyor.com/api/projects/theweavrs/BreadPlayer/artifacts/BreadPlayer.Views.UWP/AppPackages/BreadPlayer.Views.UWP_1.1.0.0_Test/BreadPlayer.Views.UWP_1.1.0.0_x86_x64_arm.appxbundle) to check out the new features! 

#### Found a bug? Report it here on github (recommended) or [email me here](mailto:enkaboot@gmail.com). 

### Alpha Preview (Video):
[![Introducing Bread Player - Alpha Preview](http://i.imgur.com/DOhQP0A.png)](https://www.youtube.com/watch?v=xFeIf0GnvaM)
### [Screenshots](https://github.com/theweavrs/BreadPlayer/wiki/Screenshots):
![Imgur](http://i.imgur.com/5lUUhBH.jpg)

### Main Features:
1. Flawlessly plays all major formats (mp3, wav, flac, ogg, aiff etc.)! 
2. Full functional music library with sorting, filtering, search etc.
3. Amazing performance i.e. ability to import 12000 songs in 120s with complete tags and album arts.
4. Playlist import (.m3u, .pls etc). (Export coming very soon.)
5. Other basic music player capabilities such as repeat, shuffle etc.
6. Pickup where you left off.
7. Loading songs from Windows Explorer
8. Drag/Drop songs directly into library.

### What Happens Next?
**Development on Beta version has started.** 
#### Beta Feature List:

- [ ] 1. Equalizer/Effects
- [x] 2. Most Played, Recently Added, Favorites and Now Playing section
- [ ] 3. Prevent screen from locking.
- [x] 4. Stop playing after this song. _**(thanks to vsarunov)**_
- [ ] 5. .lrc lyrics (Synchronized lyrics) and unsynced lyrics.
- [ ] 6. Ability to hide a specific folder and its songs.
- [x] 7. Private Playlists
- [x] 8. Ability to relocate (change location) of a song.
- [x] 9. Fade in/out when changing the song.
- [x] 10. Last.fm Scrobbling.
- [ ] 11. SoundCloud Support
- [ ] 12. Manual adding of Album arts
- [x] 13. Separate BreadPlayer.Core from BreadPlayer.Views.UWP
- [x] 14. A preview for previous song just like for next song. _**(thanks to vsarunov)**_
- [x] 15. A notification for upcoming song when the position reaches last 15-10 seconds.
- [ ] 16. _Initiate Android Support._
 
_Note: All of these features might not reach the next Beta and some might be postponed due to obvious reasons. **Any help regarding these features including testing, research, code contribution, will be highly appreciated.**_

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
5. Vladislav Sarunov [@vsarunov](https://github.com/vsarunov)

_Note: I am not an expert developer and as a result the code-base isn't as professional as it could be. Hence, I will highly appreciate any contribution in any field regarding this project. All suggestions and issue reporting are welcome._

## Build Notes:
1. Make sure you have the necessary tools for building [Windows Universal Apps](https://dev.windows.com/en-us/develop/building-universal-Windows-apps).
2. Clone this repo:  `git clone https://github.com/theweavrs/BreadPlayer/`
3. Run `msbuild.cmd` in `scripts` folder.
4. Enjoy!

_If you encounter any error during installation or building please follow [this guide in the wiki](https://github.com/theweavrs/BreadPlayer/wiki/How-To-Build-Bread-Player)._
