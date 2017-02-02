#Release Notes for Bread Player.

##Version 1.2-alpha
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
