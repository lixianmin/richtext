
/********************************************************************
created:    2017-08-07
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Unique.RichText
{
    internal class SpriteTagManger
    {
        public SpriteTag GetTag (int index)
        {
            if (index >= 0)
            {
                _tags.EnsureSizeEx(index + 1);
                SpriteTag tag = _tags[index] ?? (_tags[index] = new SpriteTag());
                return tag;
            }
           
            return null;
        }

        public void Reset (int startIndex)
        {
            var count = _tags.Count;
            for (int i= startIndex; i< count; ++i)
            {
                var tag = _tags[i];
                tag.Reset();
            }
        }

        public IList<SpriteTag> GetTags ()
        {
            return _tags;
        }

        private readonly List<SpriteTag> _tags = new List<SpriteTag>();
        public static readonly SpriteTagManger Instance = new SpriteTagManger();
    }
}