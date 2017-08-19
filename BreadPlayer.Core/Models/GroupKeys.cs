using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Models
{
    public class AlbumGroupKey : IGroupKey
    {
        public string Key { get;set; }
        public Mediafile FirstElement { get; set; }

        public int CompareTo(IGroupKey other)
        {
            return this.Key.CompareTo(other.Key);
        }
        public override string ToString()
        {
            return this.Key.ToString();
        }
        public override bool Equals(object obj)
        {
            return this.Key.Equals((obj as IGroupKey).Key);
        }
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }
    public class ArtistGroupKey : IGroupKey
    {
        public string Key { get; set; }

        public int CompareTo(IGroupKey other)
        {
            return this.Key.CompareTo(other.Key);
        }
        public override string ToString()
        {
            return this.Key.ToString();
        }
        public override bool Equals(object obj)
        {
            return this.Key.Equals((obj as IGroupKey).Key);
        }
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }
    public class TitleGroupKey : ObservableObject, IGroupKey
    {
        public string Key { get; set; }
        int _totalArtists;
        int _totalAlbums;
        int _totalPlays;
        public int TotalArtists
        {
            get => _totalArtists;
            set => Set(ref _totalArtists, value);
        }
        public int TotalAlbums
        {
            get => _totalAlbums;
            set => Set(ref _totalAlbums, value);
        }
        public int TotalPlays
        {
            get => _totalPlays;
            set => Set(ref _totalPlays, value);
        }
        public int CompareTo(IGroupKey other)
        {
            return this.Key.CompareTo(other.Key);
        }      
        public override string ToString()
        {
            return this.Key.ToString();
        }       
        public override bool Equals(object obj)
        {
            return this.Key.Equals((obj as IGroupKey).Key);
        }
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }
}
