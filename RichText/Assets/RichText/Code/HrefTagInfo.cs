
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Unique.UI
{
    /// <summary>
    /// 超链接信息类
    /// </summary>
    internal class HrefTagInfo
    {
        public int StartIndex;

        public int EndIndex;

        public string Name;

        public void Reset()
        {
            StartIndex = -1;
            EndIndex = -1;
        }

        public bool IsValid ()
        {
            return StartIndex != -1 && EndIndex != -1;
        }

        public List<Rect> Boxes = new List<Rect>();
    }
}