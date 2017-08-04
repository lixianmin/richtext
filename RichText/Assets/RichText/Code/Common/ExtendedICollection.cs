
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/
using System.Collections;
using System.Collections.Generic;

namespace Unique
{
    internal static class ExtendedICollection
    {
        public static bool IsNullOrEmptyEx<T> (this ICollection<T> collection)
        {
            return null == collection || collection.Count == 0;
        }
    }
}