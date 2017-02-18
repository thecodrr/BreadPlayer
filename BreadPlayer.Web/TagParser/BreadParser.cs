using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.TagParser
{
    public class BreadParser
    {
        List<string> TLDList = new List<string>();
        List<string> SeperatorList = new List<string>() {" - "};
        List<char> GarbageCharList = new List<char>() { };
        public int Compare2Strings(string a, string b)
        {
            List<string> list = new List<string>() {"eminem", "justin", "justin bieber", "the way I am eminem", "nothing like us justin bieber" };
            foreach(var item in list)
            {
                int similarity = YetiLevenshteinDistance.YetiLevenshtein("justin bieber", item);
                int similarity3 = YetiLevenshteinDistance.YetiLevenshtein("bieber justin", item);
                string represent = a + ": " + similarity.ToString() + " || " + b + ": " + similarity3;
                //return similarity + similarity3;
            }
            return 0;
        }
    }
}
