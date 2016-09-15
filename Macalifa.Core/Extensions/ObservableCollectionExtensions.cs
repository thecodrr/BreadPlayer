using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Macalifa.Extensions
{
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Environment.CurrentManagedThreadId))); }
        }
    }

    static class ObservableCollectionExtensions
    {
        public static void Shuffle<T>(this ThreadSafeObservableCollection<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
