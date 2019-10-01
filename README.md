<dl>
  <a href="https://breadplayer.com/"><img height="150" src="http://i.imgur.com/PNMSGUr.png" title="breadplayer"/></a>
  <a href="https://www.microsoft.com/en-gb/store/p/bread-player/9nblggh42srx/"><img height="80" src="https://assets.windowsphone.com/f2f77ec7-9ba9-4850-9ebe-77e366d08adc/English_Get_it_Win_10_InvariantCulture_Default.png" title="Get it from Windows Store!" alt="Get it from Windows Store!"/></a>
  <a href="https://patreon.com/thecodrr"><img src="http://i.imgur.com/uHXRhpN.png" width="200" height="60" title="donate via patreon" /></a>
  <h1>Bread Player aka <em>Macalifa</em></h1>
  <p>Bread Player, a free and open source music player powered by UWP and C#/.NET with a sleek and polished design built for, and by, the people seeking a better alternative to Groove and Windows Media Player by Microsoft.</p>
</dl> 

| Gitter                                                                                                                                                                                                                   | Build                                                                                                                                                                                                                                                                                      | Suggestions                                                                                                                                         | Social                                                                                                                  | Help us translate! |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------| ------------- |
| [![Join the chat at https://gitter.im/BreadPlayer/Lobby](https://badges.gitter.im/BreadPlayer/Lobby.svg)](https://gitter.im/BreadPlayer/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) | [![beta](https://img.shields.io/badge/beta-v2.7.8-orange.svg)](https://github.com/theweavrs/BreadPlayer/releases/tag/v2.7.8)                                                                                                                                                               | [![suggestions here](https://img.shields.io/badge/give%20your-suggestions%20here-orange.svg)](https://github.com/theweavrs/BreadPlayer/issues/17)   | [![Facebook](https://img.shields.io/badge/like%20us%20on-facebook-blue.svg)](https://www.facebook.com/yourbreadplayer/) | [![Guide here!](https://img.shields.io/badge/follow%20the-guide%20here-green.svg)](https://github.com/theweavrs/BreadPlayer/wiki/I-Want-To-Translate!)
|                                                                                                                                                                                                                          | [![build-status](https://ci.appveyor.com/api/projects/status/hphdwx2riesha37e/branch/master?svg=true)](https://ci.appveyor.com/project/theweavrs/breadplayer)                                                                                                                                            | [![ui related issues](https://img.shields.io/badge/ui%20related-issues%20here-brightgreen.svg)](https://github.com/theweavrs/BreadPlayer/issues/21) |                                                                                                                         |
|                                                                                                                                                                                                                          | [![Nightly-Builds](https://img.shields.io/badge/download-nightly%20build-brightgreen.svg)](https://ci.appveyor.com/project/theweavrs/breadplayer/build/artifacts) |                                                                                                                                                    |                                                                                                                         | 

## Current Status:
#### Beta version has been released and development on the second Beta has started! You can [download the nightly build from here](https://ci.appveyor.com/project/theweavrs/breadplayer/build/artifacts) to check out the new features! 

#### Found a bug? Report it here on github (recommended) or [email me here](mailto:enkaboot@gmail.com). 

### Beta Preview (Video Coming Soon):

### Main Features:
1. Flawlessly plays all major formats (mp3, wav, flac, ogg, aiff etc.)! 
2. Full functional music library with sorting, filtering, search etc.
3. Amazing performance i.e. ability to import 12000 songs in 120s with complete tags and album arts.
4. Playlist import (.m3u, .pls etc). (Export coming very soon.)
5. Other basic music player capabilities such as repeat, shuffle etc.
6. Pickup where you left off.
7. Loading songs from Windows Explorer
8. Drag/Drop songs directly into library.
9. Equalizer/Effects
10. Most Played, Recently Added, Favorites and Now Playing section
11. Prevent screen from locking.
12. Stop playing after this song. _**(thanks to vsarunov)**_
13. Private Playlists
14. Ability to relocate (change location) of a song.
15. Fade in/out when changing the song.
16. Last.fm Scrobbling.
17. Manual adding of Album arts
19. A preview for previous song just like for next song. _**(thanks to vsarunov)**_
20. A notification for upcoming song when the position reaches last 15-10 seconds.
21. All songs



### What Happens Next?
**Development on Second Beta version has started.** 

#### Second-Beta Feature List (Coming Soon):

- [ ] 1. ExploreView (will include recommendation, song streaming, SoundCloud, Spotify etc.)
- [x] 2. NowPlayingView (will show what's playing and the nowplaying queue
- [ ] 3. Windows, Linux & Mac App (Alpha) 
 
 
_Note: All of these features might not reach the next Beta and some might be postponed due to obvious reasons. **Any help regarding these features including testing, research, code contribution, will be highly appreciated.**_

### Libraries used:
1. C#/.NET
2. UWP API (Windows Aniversary Edition 10.0; Build 14393)
2. [BASS](http://www.un4seen.com/bass.html) & [ManagedBass](https://github.com/ManagedBass/ManagedBass) (for audio processing)
3. [LiteDB](https://github.com/mbdavid/LiteDB) (for library managment)
4. Taglib#
5. DBreeze
6. ColorThief
7. IF.Last.fm

### Contributors:
Thanks to these awesome people Project Bread has come this far:

1. Danny [@DannyTalent](https://github.com/DannyTalent)
2. [@Bond-009](https://github.com/Bond-009)
3. [@MightyK1337](https://github.com/MightyK1337)
4. Kai Hildebrandt [@hildebrandt87](https://github.com/hildebrandt87)
5. Vladislav Sarunov [@vsarunov](https://github.com/vsarunov)

_Note: I am not an expert developer and as a result the code-base isn't as professional as it could be. Hence, I will highly appreciate any contribution in any field regarding this project. All suggestions and issue reporting are welcome._

## Build Notes:
1. Make sure you have the necessary tools for building [Windows Universal Apps](https://developer.microsoft.com/en-us/windows/apps).
2. Clone this repo:  `git clone https://github.com/theweavrs/BreadPlayer/`
3. Run `msbuild.cmd` in `scripts` folder.
4. Enjoy!

_If you encounter any error during installation or building please follow [this guide in the wiki](https://github.com/theweavrs/BreadPlayer/wiki/How-To-Build-Bread-Player)._

## Help us translate Bread Player
If you would like to translate Bread Player into your language, please [follow the guide here](https://github.com/theweavrs/BreadPlayer/wiki/I-Want-To-Translate!)
