
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/
using System.Collections;
using System.Collections.Generic;

namespace Unique
{
    internal static class ExtendedIList
    {
        public static void EnsureSizeEx<T> (this IList<T> list, int size)
        {
            if (null != list)
            {
                var count = list.Count;
                for (int i= count; i< size; ++i)
                {
                    list.Add(default(T));
                }
            }
        }
    }
}