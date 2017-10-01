using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BreadPlayer.Parsers.TagParser
{
    public class TagParser
    {
        private static string[] seperators = new string[] {
            "ft.", "FT.", "Ft.", "ft", "Ft", "FT", "feat","Feat", "FEAT",
            "featuring","Featuring", "FEATURING", "feat.", "Feat.", "FEAT.", "/", "," , "&"};

        public static string ParseAlbum(string text)
        {
            throw new NotImplementedException();
        }

        public static List<string> ParseArtistsFromTitle(string title)
        {
            //clean the garbage of bracket garbage
            title = Regex.Replace(title, @"\[|\]|\(|\)|\{|\}", "");
            string[] words = title.Split(' ');

            //linq expression to get the first seperator in the title words.
            //we do this because we want to match only whole words and not any words.
            string sep = seperators.FirstOrDefault(t => words.Any(a => t == a));

            if (string.IsNullOrEmpty(sep))
                return null;

            //get the artist part from the title string
            var artistPart = title.Substring(title.IndexOf(sep) + sep.Length).TrimStart();
            return ParseArtists(artistPart);
        }

        public static List<string> ParseArtists(string text)
        {
            //seperate the artist string into words but keep the delimiters too.
            var artists = text.SplitAndKeepDelimiters(' ', ',', '/');
            List<string> artistArray = new List<string>();

            if (artists != null)
            {
                for (int i = 0; i < artists.Count; i++)
                {
                    foreach (var seperator in seperators)
                    {
                        //check if the artist word at current index
                        //matches the current seperator
                        //we are looking for seperators in the artists' words list
                        if (artists[i] == seperator)
                        {
                            //Take all the words before the seperator index and
                            //string.Join them. Afterwards, add it to our artist list.
                            artistArray.Add(string.Join(" ", artists.Take(i)));

                            //remove the artist words added including the seperator
                            artists.RemoveRange(0, i + 1);

                            //set the index to 0 so IndexOutOfRange can be avoided.
                            i = 0;
                        }
                    }
                }
                //we will have the last artist left. Always. So add it.
                if (artists.Count > 0)
                {
                    artistArray.Add(string.Join(" ", artists));
                }
                return artistArray;
            }
            return null;
        }

        public static string ParseTitle(string text)
        {
            string regex = @"^\d+(?:\.)?(?:\s\-)?\s|[\(\[:].*|(\s&\s).*|(?i)(f(ea)?t).*(?-i)|(?i)((http|https)\:\/\/|(www\.))?[a-zA-Z0-9\.\/\?\:@\-_=#]{2,}\.[a-zA-Z0-9\.\/\?\:@\-_=#]+(?i)";
            return Regex.Replace(text, regex, "").Trim();
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Splits the given string into a list of substrings, while outputting the splitting
        /// delimiters (each in its own string) as well. It's just like String.Split() except
        /// the delimiters are preserved. No empty strings are output.</summary>
        /// <param name="s">String to parse. Can be null or empty.</param>
        /// <param name="delimiters">The delimiting characters. Can be an empty array.</param>
        /// <returns></returns>
        public static List<string> SplitAndKeepDelimiters(this string s, params char[] delimiters)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(s))
            {
                int iFirst = 0;
                do
                {
                    int iLast = s.IndexOfAny(delimiters, iFirst);
                    if (iLast >= 0)
                    {
                        if (iLast > iFirst)
                            parts.Add(s.Substring(iFirst, iLast - iFirst)); //part before the delimiter
                        if (s[iLast] != ' ')
                            parts.Add(new string(s[iLast], 1));//the delimiter
                        iFirst = iLast + 1;
                        continue;
                    }

                    //No delimiters were found, but at least one character remains. Add the rest and stop.
                    parts.Add(s.Substring(iFirst, s.Length - iFirst));
                    break;
                } while (iFirst < s.Length);
            }

            return parts;
        }
    }
}