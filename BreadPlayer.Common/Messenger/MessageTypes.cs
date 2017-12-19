namespace BreadPlayer.Messengers
{
    /// <summary>
    /// Use an enumeration for the messages to ensure consistency.
    /// </summary>
    public enum MessageTypes
    {
        MsgLibraryLoaded,	// Sent when a Customer is selected for editing
        MsgPlaySong,
        MsgDispose,
        MsgExecuteCmd,
        MsgAddPlaylist,
        MsgRemovePlaylist,
        MsgAddAlbums,
        MsgPlaylistLoaded,
        MsgUpdateSongCount,
        MsgStopAfterSong,
        MsgImportFolder,
        MsgNavigate,
        MsgSearch,
        MsgChangePlaylist
        //MSG_CUSTOMER_SAVED				// Sent when a Customer is updated to the repository
    }
}