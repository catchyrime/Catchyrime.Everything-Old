using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Catchyrime.Everything.Developer;

namespace Catchyrime.Everything
{
    public static class Enumerables
    {
        public static void ForEach<T>(
            [In, NotChecked, NotNull] this IEnumerable<T> collection,
            [In, NotChecked, NotNull] Action<T> action
            )
        {
            foreach (T t in collection) {
                action.Invoke(t);
            }
        }

        public static bool ForEach<T>(
            [In, NotChecked, NotNull] this IEnumerable<T> collection,
            [In, NotChecked, NotNull] Func<T, bool> func
            )
        {
            foreach (T t in collection) {
                if (!func.Invoke(t)) return false;
            }
            return true;
        }
    }
}
