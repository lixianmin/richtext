
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unique.RichText
{
    [System.Serializable]
    public class SpriteTag
    {
        public SpriteTag ()
        {
            
        }

        public void Reset ()
        {
            _name = null;
        }

        public void SetVertexIndex (int index)
        {
            _vertexIndex = index;
        }

        public int GetVertexIndex ()
        {
            return _vertexIndex;
        }

        public void SetName (string name)
        {
            _name = name;
        }

        public string GetName ()
        {
            return _name;
        }

        public Vector2 Size;
        public float Offset;

        private string _name;
        private int _vertexIndex;
    }
}