using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using IF.Lastfm.Core.Scrobblers;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BreadPlayer.Web.Lastfm
{
    class BreadScrobbler : ScrobblerBase
    {
        public string DatabasePath { get; private set; }

        public BreadScrobbler(ILastAuth auth, string databasePath, HttpClient client = null) : base(auth, client)
        {
            DatabasePath = databasePath;

            CacheEnabled = true;
        }

        public override async Task<IEnumerable<Scrobble>> GetCachedAsync()
        {
            var disk = new FileDiskService(DatabasePath, new FileOptions() { FileMode = FileMode.Exclusive });
            using (var db = new LiteDatabase(disk))
            {
                return await Task.Run(() => 
                {
                    var scrobbles = db.GetCollection<Scrobble>("scrobbles");
                    if (scrobbles.Count() <= 0)
                    {
                        return Enumerable.Empty<Scrobble>();
                    }

                    var cached = scrobbles.FindAll();//.Find(Query.All());
                    var notNull = new List<Scrobble>().Concat(cached);
                    return notNull;
                });               
            }
        }

        protected override Task<LastResponseStatus> CacheAsync(IEnumerable<Scrobble> scrobbles, LastResponseStatus originalResponseStatus)
        {
            // TODO cache originalResponse - reason to cache
            return Task.Run(() =>
            {
                Cache(scrobbles);
                return LastResponseStatus.Cached;
            });
        }

        private void Cache(IEnumerable<Scrobble> scrobbles)
        {
            var disk = new FileDiskService(DatabasePath, new FileOptions() { FileMode = FileMode.Exclusive });
            using (var db = new LiteDatabase(disk))
            {
                var scrobblesCollection = db.GetCollection<Scrobble>("scrobbles");
                foreach (var scrobble in scrobbles)
                    scrobblesCollection.Insert(scrobble);
            }
        }
    }
}