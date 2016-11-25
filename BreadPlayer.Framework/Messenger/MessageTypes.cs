namespace BreadPlayer.Messengers
{
    /// <summary>
    /// Use an enumeration for the messages to ensure consistency.
    /// 
    /// </summary>
    public enum MessageTypes
    {
        MSG_LIBRARY_LOADED,	// Sent when a Customer is selected for editing
        MSG_PLAY_SONG,
        MSG_DISPOSE,
        MSG_EXECUTE_CMD,
        MSG_ADD_PLAYLIST,
        MSG_ADD_ALBUMS,
        MSG_PLAYLIST_LOADED,
        MSG_UPDATE_SONG_COUNT
        //MSG_CUSTOMER_SAVED				// Sent when a Customer is updated to the repository
    };
}