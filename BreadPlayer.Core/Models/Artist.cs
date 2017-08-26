using BreadPlayer.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Core.Models
{
    public class Artist : IDbRecord
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Bio { get; set; }
        public string DOB { get; set; }
        public string TextSearchKey => GetTextSearchKey();

        public string GetTextSearchKey()
        {
            return Name;
        }
    }
}
