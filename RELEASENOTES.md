# Release Notes for Bread Player.

## Version 2.7.9-beta:
### What's Fixed:
1. Fixed app crash when performing functions on TSCollection. #225
3. Fixed issue where player was showing the first opened album/artist again and again. #226
4. Fixed app crash when getting next/upcoming song. #227
5. Fixed issue where player couldn't open files from outside Music Library. #228
6. Fixed issue where player couldn't play song by clicking "Play" from context menu. #229
7. Fixed issue where playlists were not opening correctly. #230
8. Fixed issue where some albums and artists missing. #231
9. Fixed crash when audio device was disabled. #232

### What's Improved:
1. Improved Artists Listing UI

## Version 2.7.8-beta:
### What's Fixed:
1. Fixed exception during folder import if file path is longer than 260 characters. #220
3. Fixed thread error when loading music. #219
4. Fixed NullReferenceException when re initializing database. #221
5. Fixed error while trying to play next song. #222
6. Fixed error while navigating back. #223
7. Fixed startup crash due to internetconnectivityhelper.
8. Fixed album and artist loading.
9. Fixed streaming speed.
10. Fixed issue where media servers were not detected.
11. Fixed streaming from portable devices.

### What's New:
1. Added arabic translation.

## Version 2.7.7-beta:
### What's Fixed:
1. Fixed error while getting database transactions. #206
3. Fixed issue where a song would not play. #208
4. Fixed issue where import folder dialog failed to show up on some devices. #212
5. Fixed issues with SMTC. #209, #211, #205
6. Fixed issue where bluetooth audio device would not play. #203
7. Fixed issue where no device would show up in "My Devices" even though the SD Card was connected. #214
8. Fixed crash when inserting records into database. #213
9. Fixed startup crashes. #204, #207
10. Fixed app crash when loading an album. #202
11. Fixed MusixMatch lyrics fetching.
12. Fixed crash when refreshing a folder in Folders.
13. Fixed crash when a log report was sent to Sentry.io

### What's Improved:
1. Improved library navigation.
2. Various UI improvements.

## Version 2.7.6-beta:
### What's Fixed:
1. Fixed all audio glitches. #191
2. Fixed crash on resetting player. #193
3. Fixed issue where playing next song gave an error. #194
4. Fixed crash due to out of memory exceptions. #195
5. Fixed issues with database which caused many crashes. #196
6. Fixed issue where internet status was always not connected causing Lyrics/Bio to not work. #197
7. Fixed crash when opening Now Playing screen. #199
8. Fixed crash when opening app with arguments i.e. playing a song from File Explorer. #201
9. Fixed other crashes and issues.

### What's New:
1. Added FOLDER NAVIGATION.\**
2. Added OneDrive streaming support.*
3. Added Media Server/LAN streaming support.\**

What's Improved:
1. The usual things: performance, UI, animations etc.

_* Currently only streaming is possible no download or anything like that._

_\** LAN/FOLDER NAVIGATION support is largely experimental so expect some bugs and crashes._

## Version 2.7.5-beta:
### What's Fixed:
1. Fixed issue with logger.
2. Fixed issue where equalizer settings were not saved.
3. Fixed a memory leak when opening playlists, albums, artists etc.
4. Fixed issue where notification settings weren't working.
5. Fixed glitches when navigating.
6. Fixed search.

### What's New:
1. Added support for AAC & .MP4
2. Added ability to save customized presets.
3. Updated app's requested capabilites.
4. Added Fast Forward (Hold next song button) and Rewind (Hold previous song button) capabilites to SMTC (the thing that appears when you press volume buttons).
5. Search is now on-demand instead of auto i.e. you will have to press enter when done entering the query.
6. Added German translation.

## Version 2.7.4-beta:
### What's Fixed:
1. Fixed issue where equalizer could only be opened once
2. Fixed issues with equalizer
3. Fixed issues when caching artist art.
4. Added Polish, German and Persian translations
5. Fixed several memory leaks.
6. Fixed choppy audio issue.

## Version 2.7.3-beta:

### What's Fixed:
1. Fixed bug where Play button on Artist Tile did nothing.
2. Fixed issue where importing songs casued OverflowException
3. Fixed startup crash
4. Fixed UI Glitches in Playbar.
5. Fixed Donate options.
6. Fixed duplicate songs appearing on auto library load.
7. Fixed many other minor bugs.

### What's New:
1. Added Musixmatch as a new Lyric source.
2. Added support for Fall Creators Update
3. Added Russian Translation

## Version 2.7.2-beta:
### What's Changed:
1. Bug fixes
2. Notifications are now queued.
3. Notifications now include a "Close" button.
4. Improved performance.
5. Fixed playlist import.
6. Added donate dialog
7. Made Bread Player Free in the store again.

## Version 2.7.1-beta:
### What's Fixed:
1. Fixed issue where navigating backwards through settings minimized the app.
2. Fixed issue where app crashed when song reached the end.
3. Fixed crash when navigating to Bakers (artists).
4. Fixed app hang when navigating to Bakers (artists).
5. Fixed app crash when library was loading.
6. Fixed crash when searching songs.
7. Fixed Show All Toasts Search results UI.
8. Fixed random crashes when navigating around pages with lists.
9. Fixed a critical thread allocation bug.
10. Fixed hang and crash when deleting songs.

## Version 2.7.0-beta:
### What's New:
1. Added "Bakers" menu for artists.
2. Reworked "Toast Collection" user interface
3. Reworked "Settings" user interface
4. Added better "Equalizer" and new "Presets".
5. Added new "Now Baking" screen with "Lyrics" & "Artist Info".
6. Added "Pin to Start" for almost everything.
7. Added ability to "Share" now baking song.
8. Added new "Playlists" user interface.
9. Added better logging and crash analytics.
10. Added more ways you can SUPPORT ME!

### What's Fixed:
**The issues fixed weren't counted nor recorded. But I can assure you, they were MANY.**

### What's Improved:
1. Improved navigation performance.
2. Improved backward navigation.
3. Improved overall performance of the app.
4. Improved User Experience.
5. Improved playlist import performance.

### What's Removed:
1. "Playlists" were removed from HamburgerMenu to "Toast Collection".
2. "Import folders" and "Import playlists" were removed to "Toast Collection".
3. "Recently Played", "Recently Added" & "Most Played" were removed to "Toast History".
4. "My Bakeries" was removed to Settings > Accounts

## Version 2.6.2-beta:
### What's Fixed:
- Fixed issue where library import took too much time.
- Fixed issue where many album arts were not loaded.
- Fixed other bugs.

### What's New:
- Added ability to ignore DRM-Protected songs.
- Added sorting by tracknumber for album songs.

### What's Improved:
- Improved library import

## Version 2.6.1-beta
### What's Fixed:
- Fixed issue where selecting Unsorted returned an empty list.
- Fixed nullreference exception while adding album songs to an existing playlist.
- Fixed bug where sound was coming from the front speakers instead of the back speakers.
- Fixed other crashes and bugs.

## Version 2.6-beta
### What's Fixed:
- Fixed 2 random crashes.
- Fixed invisible jumplist issue when in sort/grouped mode.
- Fixed empty album issue.
- Fixed issue with auto loading of library at startup. 
- Fixed issue where song wasn't played from external speakers/headphones.

### What's New:
- Added support for German Translation (thanks to Armin).
- Added backward navigation support for mobiles. 

### What's Improved:
- Core improvements (shuffle and other things).
- Icon was improved.

## Version 2.5-beta
### What's Fixed:
- Fixed issue where next song to the currently playing song also got added to Recently Eaten.
- Fixed app crash is result of a bug with library import.
- Fixed crash when adding songs to a playlist.
- Fixed app crash when trying to use SMTC on PC.
- Fixed UI bug where Font Capitilization options weren't being displayed.
- Fixed UI glitch in which search results weren't adjusted when screen width was reduced.
- Fixed last.fm scrobbling issue.
- Fixed issue where recently added items weren't displayed.
- Fixed Song Duration display for songs longer than 1 hour.
- Fixed issue where Next Song & Prev Song button didn't work in any playlist.
- Fixed issue where songs couldn't be imported after player reset.
- Fixed issue where playing through all tracks in "Recently Eaten" will then break out of the list. 

### What's New:
- Player foreground will now be adjusted according to the current accent color.
- We also added a new database engine in this release.
- Added "Donate Via Patreon" button.
### What's Improved:
- Improved shuffle.
- Improved UI for both PC & Mobile.

## Version 2.4-beta
### What's Fixed:
- Fixed player startup crashes.
- Fixed tap to play for tablets and touch monitors.

### What's New:
- Removed auto expand to full-screen in mobiles.
- Removed some shortcuts from the app.

## Version 2.3-beta
### What's Fixed:
- Fixed issue with output device not changing when headphones are connected.
- Fixed NowPlayingList loses its ItemTemplate when window size is changed.
- Fixed crash after library load.
- Fixed playlist import.
- Fixed audio stutter when minimizing to background (only mobile).
- Fixed crash when deleting duplicates. 
- And many more bugs were also fixed (which I do not remember yet). :D

### What's New:
- Added new Keyboard Shortcuts ('Keybindings' section in Settings). 
- Added full translation support.
- Added ability to export playlist (only to .m3u & .pls for now).
- Added silent upcoming song notifications. 
- Added translations for Czech and Sinhala languages.
- Added equalizer presets (experimental).
- Added real-time music library updates when filesystem changes (only works when app is running).
- Added 'Contribute' section in settings to help in contributing.
- Added auto playback stop in BreadPlayer when a song is played in Groove Player.
- Added navigate to now playing screen when song is played on mobile.
- Added auto removal of duplicates when importing songs.
- Added navigate to now playing screen when song's tags are clicked in mini player.

### What's Improved:
- Improved hamburger menu list item response on tap/click.
- Improved all animations and transitions.
- Improved navigation performance.
- Improved library import. 
- Improved overall UI and increased readability.
- Improved startup performance by 50% (only PC).

## Version 2.1-beta
### What's improved:
1. 50% performance improvements in mobile
2. Equazlier UI has been greatly improved.
3. The over all UI has also been improved.

### What's fixed:
1. The problem with splash screen will now hopefully be fixed but if it is not, please comment below.
2. We fixed issue where not all album arts were loading
3. We fixed the issue in which the app crashed after song import.
4. We fixed the issue with too slow song import.

### What's New:
1. There is now an enable/disable blur option in the settings.
2. There are a couple of new animations. You will notice.
3. We dumbed the What's new splash screen with a simple dialog.
4. There is now a select all button when duplicates are shown.

### Known Issues:
1. Dark theme apparently doesn't work and the app crashes (fixed will release in the next update).
2. There might be some crashes when importing songs (not confirmed but the app almost always hangs for a bit.)

## Version 2.0-beta
### What's New:
* Smooth Play/Pause Transitions.
* Private Playlists.
* Manually Change Any Tracks Album Art.
* Hardware-Independent 10-Band Equalizer.
* Prevent Screen From Locking.
* Last.fm Scrobbling.
* Favorite Songs List, Most Played and Recently Added List.
* 'Stop After This Song' Function.
* New Redstone 3 Inspired UI Design.
* New Database Engine.

### What's Fixed:
* Fixed Last.fm is not initialized on startup i.e. user is not logged in on startup (Issue #121)
* Fixed Library folders are not added to folder list on app startup. (Issue #120)
* Fixed No menu is opened for playlists in HamburgerMenu in Mobile. (Issue #116)
* Fixed Back Navigation
* Fixed Crash when changing albumart of any song from the list manually (Issue #115)
* Fixed Song chamge is not reflected in the SMTC when player is in background (Issue #114)
* Fixed Playlist Song Is Not Deleted When Delete Button Is Pressed. (Issue #113)
* Fixed Crash when seeking slider to the end. (Issue #111)
* Fixed Performance issue on player startup. (Issue #107)
* Fixed Duplicates window doesn't show title song. (Issue #104)
* Fixed Random Application crash. (Issue #97)
* Fixed nullreferenceexeption when clicking on add playlist button on album item.
* Fixed crash when searching from albumartistview
* Fixed minor bugs with library loading and playback of song.
* Fixed Audio does not play in the background )Issue #63)

### What's Improved:
* Improved Hamburger Menu
* Improved UI/UX of the whole player
* Improved Database Engine and Library Import
* Improved and Cleaned Up Code
* Improved navigation
* Improved playlists and albums.
* Improved everything :D

## Version 1.3-alpha
### What's Fixed:

* Background audio issue was fixed.
* Issue where albums were not being loaded was fixed.

## Version 1.2-alpha
### What's Improved:

* We improved library loading performance and memory allocation.
* Search algorithm was greatly improved.
* User-interface was slightly improved to offer consistent and smoother experience.
  * Library View was greatly improved for bigger screens.    
  * Margins were increased around headings and content for each page.
  * When searching for a song, the library header is changed to display the searched string.
  * UI for smaller screens was improved.
  * Context Menu opening/closing was improved for mobiles.
  * The text when there were no songs was also changed (it was annoying).
  * "*" was removed from the "New Playlist" dialog.
  * The play and add buttons will now remain at the same place no matter the song's title's actual length.
  * The preview for upcoming song was improved.
  * The loading indicator will not show up when there are no albums.  
* Major performance improvements in sorting, grouping, navigation, and scrolling.
	* The amount of objects being drawn for each songs was decreased greatly resulting in better scrolling and loading speed.
	* On startup and when searching, songs are loading into library in "batches" providing faster access and smoother performance.
	* There will be no lag when sorting.
	* The app startup was amazingly improved because nothing other than the library gets loaded now.

### What's New:

* New sorting options added.You can now sort by song folder, song length, song track no. and song year.
* Auto-navigation to "Music Library" when searching from a different page than library was added.
* Multi-selection for songs for both mobiles and PCs was added.
* A new logger was implemented with sending capabilities, and a setting for sending crash analytics to our servers was also added.
* A new UI was implemented to show progress when importing songs.
* Disabling the "Change accent by song album art" now changes the accent back to default instead of being stuck at the same accent as the playing song.
* A new now playing animation was added to the list and the "huge" icon to indicate the now playing song in library view as replaced.
* Icon and splash screen background color was changed to a more darker purple shade (almost black).
* Tooltips were added for each and every button out there.
* Your scroll position when navigating away from music library will now remain unchanged.
* When deleting a playlist, you will now actually see the name of the playlist you are deleting.
* Search button now works when hamburger menu is closed.

### What Was Fixed:

* Fixed crash when deleting songs from playlist.
* Fixed crash when searching long strings.
* Fixed issue in mobile where audio was not playing in background.
* Fixed a problem with SMTC not showing up on mobile.
* Fixed issue where settings were not saved if player crashed.
* Fixed crash when navigating backwards using the back button.
* Fixed many more bugs and issues that I don't currently remember. (Remind me and I will add them here.)
