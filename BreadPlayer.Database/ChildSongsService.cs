using BreadPlayer.Core.Common;
using BreadPlayer.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreadPlayer.Database
{
    public class ChildSongsService
    {
        private string _tablename, _texttablename;
        private IDatabaseService Database
        {
            get; set;
        }
        public ChildSongsService(IDatabaseService database, string tableName, string textTableName)
        {
            Database = database;
            _tablename = tableName;
            _texttablename = textTableName;
        }
        public async Task InsertTracksAsync(IEnumerable<Mediafile> fileCol, IDbRecord pList)
        {
            IEnumerable<ChildSong> playlistSongs
                = fileCol.Select(x => new ChildSong()
                {
                    SongId = x.Id,
                    PlaylistId = pList.Id
                });

            Database.ChangeTable(_tablename, _texttablename);
            await Database.InsertRecords(playlistSongs);
        }
        public void InsertSong(ChildSong file)
        {
            Database.ChangeTable(_tablename, _texttablename);
            Database.InsertRecord(file);
        }
        public async Task RemoveSongAsync(Mediafile file)
        {
            Database.ChangeTable(_tablename, _texttablename);
            var record = await Database.GetRecordByQueryAsync<ChildSong>(string.Format("songid={0}", file.Id));
            await Database.RemoveRecord(record);
        }
        public bool Exists(long id)
        {
            Database.ChangeTable(_tablename, _texttablename);
            return Database.CheckExists(id);
        }
        public Task<IEnumerable<Mediafile>> GetTracksAsync(long parentId)
        {
            return Task.Run(async () =>
            {
                Database.ChangeTable(_tablename, _texttablename);
                var trackIds = (await Database.QueryRecords<ChildSong>(string.Format("pId={0}", parentId).ToLower())).Select(t => t.SongId);
                Database.ChangeTable("Tracks", "TracksText");

                return trackIds.Select(x => Database.GetRecordById<Mediafile>(x));
            });
        }
    }
}
