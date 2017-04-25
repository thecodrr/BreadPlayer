using BreadPlayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class AlbumService : ChildSongsService, IDisposable
    {
        private IDatabaseService Database
        {
            get; set;
        }
        public AlbumService(IDatabaseService database) : base(database, "AlbumSongs", "AlbumSongsText")
        {
            Database = database;
        }
        public async Task InsertAlbums(IEnumerable<Album> albums)
        {
            Database.ChangeTable("Albums", "AlbumsText");
            await Database.InsertRecords(albums);
        }
        public async Task<IEnumerable<Album>> GetAlbumsAsync()
        {
            List<Album> Albums = new List<Album>();
            foreach(var album in await Database.GetRecords<Album>())
            {
                var tracks = await GetTracksAsync(album.Id);
                album.AlbumSongs = new System.Collections.ObjectModel.ObservableCollection<Mediafile>(tracks);
                Albums.Add(album);
            }
            return Albums;
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
