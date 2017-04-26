using System;
using System.Collections.Generic;
using System.Text;

namespace BreadPlayer.Database
{
    public interface IDBRecord
    {
        long Id { get; set; }
        string GetTextSearchKey();
    }
}
