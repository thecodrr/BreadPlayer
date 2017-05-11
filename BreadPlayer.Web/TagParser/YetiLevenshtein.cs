using System;

namespace BreadPlayer.Web.TagParser
{
    public class YetiLevenshteinDistance
    {                  
        public static unsafe int YetiLevenshtein(string s1, string s2)
        {
            fixed (char* p1 = s1)
            fixed (char* p2 = s2)
            {
                return YetiLevenshtein(p1, s1.Length, p2, s2.Length, 0); // substitutionCost = 1
            }
        }

        public static unsafe int YetiLevenshtein(string s1, string s2, int substitionCost)
        {
            int xc = substitionCost - 1;
            if (xc < 0 || xc > 1)
            {
                throw new ArgumentException("", "substitionCost");
            }

            fixed (char* p1 = s1)
            fixed (char* p2 = s2)
            {
                return YetiLevenshtein(p1, s1.Length, p2, s2.Length, xc);
            }
        }

        /// <summary>
        /// Cetin Sert, David Necas
        /// http://webcleaner.svn.sourceforge.net/viewvc/webcleaner/trunk/webcleaner2/wc/levenshtein.c?revision=6015&view=markup
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="l1"></param>
        /// <param name="s2"></param>
        /// <param name="l2"></param>
        /// <param name="xcost"></param>
        /// <returns></returns>
        public static unsafe int YetiLevenshtein(char* s1, int l1, char* s2, int l2, int xcost)
        {
            int i;
            //int *row;  /* we only need to keep one row of costs */
            int* end;
            int half;

            /* strip common prefix */
            while (l1 > 0 && l2 > 0 && *s1 == *s2)
            {
                l1--;
                l2--;
                s1++;
                s2++;
            }

            /* strip common suffix */
            while (l1 > 0 && l2 > 0 && s1[l1 - 1] == s2[l2 - 1])
            {
                l1--;
                l2--;
            }

            /* catch trivial cases */
            if (l1 == 0)
            {
                return l2;
            }

            if (l2 == 0)
            {
                return l1;
            }

            /* make the inner cycle (i.e. string2) the longer one */
            if (l1 > l2)
            {
                int nx = l1;
                char* sx = s1;
                l1 = l2;
                l2 = nx;
                s1 = s2;
                s2 = sx;
            }

            //check len1 == 1 separately
            if (l1 == 1)
            {
                //throw new NotImplementedException();
                if (xcost > 0)
                {
                    //return l2 + 1 - 2*(memchr(s2, *s1, l2) != NULL);
                    return l2 + 1 - 2 * MemchrRplc(s2, *s1, l2);
                }

                return l2 - MemchrRplc(s2, *s1, l2);
            }

            l1++;
            l2++;
            half = l1 >> 1;

            /* initalize first row */
            //row = (int*)malloc(l2*sizeof(int));
            int* row = stackalloc int[l2];
            if (l2 < 0)
            {
                //if (!row)
                return -1;
            }

            end = row + l2 - 1;
            for (i = 0; i < l2 - (xcost > 0 ? 0 : half); i++)
            {
                row[i] = i;
            }

            /* go through the matrix and compute the costs.  yes, this is an extremely
             * obfuscated version, but also extremely memory-conservative and
             * relatively fast.
             */
            if (xcost > 0)
            {
                for (i = 1; i < l1; i++)
                {
                    int* p = row + 1;
                    char char1 = s1[i - 1];
                    char* char2P = s2;
                    int d = i;
                    int x = i;
                    while (p <= end)
                    {
                        if (char1 == *(char2P++))
                        {
                            x = --d;
                        }
                        else
                        {
                            x++;
                        }

                        d = *p;
                        d++;
                        if (x > d)
                        {
                            x = d;
                        }

                        *(p++) = x;
                    }
                }
            }
            else
            {
                /* in this case we don't have to scan two corner triangles (of size len1/2)
                 * in the matrix because no best path can go throught them. note this
                 * breaks when len1 == len2 == 2 so the memchr() special case above is
                 * necessary */
                row[0] = l1 - half - 1;
                for (i = 1; i < l1; i++)
                {
                    int* p;
                    char char1 = s1[i - 1];
                    char* char2P;
                    int d, x;
                    /* skip the upper triangle */
                    if (i >= l1 - half)
                    {
                        int offset = i - (l1 - half);
                        int c3;

                        char2P = s2 + offset;
                        p = row + offset;
                        c3 = *(p++) + ((char1 != *(char2P++)) ? 1 : 0);
                        x = *p;
                        x++;
                        d = x;
                        if (x > c3)
                        {
                            x = c3;
                        }

                        *(p++) = x;
                    }
                    else
                    {
                        p = row + 1;
                        char2P = s2;
                        d = x = i;
                    }
                    /* skip the lower triangle */
                    if (i <= half + 1)
                    {
                        end = row + l2 + i - half - 2;
                    }
                    /* main */
                    while (p <= end)
                    {
                        int c3 = --d + ((char1 != *(char2P++)) ? 1 : 0);
                        x++;
                        if (x > c3)
                        {
                            x = c3;
                        }

                        d = *p;
                        d++;
                        if (x > d)
                        {
                            x = d;
                        }

                        *(p++) = x;
                    }
                    /* lower triangle sentinel */
                    if (i <= half)
                    {
                        int c3 = --d + ((char1 != *char2P) ? 1 : 0);
                        x++;
                        if (x > c3)
                        {
                            x = c3;
                        }

                        *p = x;
                    }
                }
            }

            i = *end;
            return i;
        }

        private static unsafe int MemchrRplc(char* buffer, char c, int count)
        {
            char* p = buffer;
            char* e = buffer + count;
            while (p++ < e)
            {
                if (*p == c)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
