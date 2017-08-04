
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Unique.UI
{
    ///已知问题：
    /// 1.下划线解析和超链接解析都是基于字符位置对应实际字符顶点位置，同时存在时位置计算会有偏差
    /// 2.字符串使用正则表达式，会有少量GC (1)减少不必要的表情顶点数据更新; (2)优化更新流程

    /// <summary>
    /// 表情动画数据组
    /// </summary>
    public class SpriteAnimInfo
    {
        public string Key;
        public Vector3[] Vertices;
        public Rect[]  Uvs;

        public int Current = 0;
        public float RuningTime = 0;
        public string[] Names ;

        public SpriteAnimInfo ()
        {
            Key = string.Empty;
            Vertices = new Vector3[4];
            Uvs = new Rect[8];
            Names = null;
        }

        public void Reset ()
        {
            Key = string.Empty;
        }

        public bool IsValid ()
        {
            return !string.IsNullOrEmpty(Key);
        }
    }
}