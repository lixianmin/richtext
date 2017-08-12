
/********************************************************************
created:    2017-08-12
author:     lixianmin

*********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{
    internal class MaterialInfo
    {
        public MaterialInfo (Material mat)
        {
            if (null == mat)
            {
                throw new ArgumentNullException();
            }

            _material = mat;
        }

        public void Attach (Graphic target)
        {
            if (null == target)
            {
                return;
            }

            target.material = _material;
            _fans.Add(target, this);
        }

        public void Detach (Graphic target)
        {
            if (null == target)
            {
                return;
            }

            _fans.Remove(target);
        }

        public Material GetMaterial ()
        {
            return _material;
        }

        public int GetCount ()
        {
            return _fans.Count;
        }

        private Material _material;
        private readonly Hashtable _fans = new Hashtable();
    }
}