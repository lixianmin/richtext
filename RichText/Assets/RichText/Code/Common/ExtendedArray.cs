
/********************************************************************
created:    2017-08-03
author:     lixianmin

*********************************************************************/

namespace Unique
{
    internal static class ExtendedArray
    {
        public static T[] SetCapacityEx<T> (this T[] array, int capacity)
        {
            if (capacity < 0)
            {
                capacity = 0;
            }

            if (null == array || array.Length != capacity)
            {
                array = new T[capacity];
            }

            return array;
        }
    }
}