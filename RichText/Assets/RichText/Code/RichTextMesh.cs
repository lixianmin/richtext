
/********************************************************************
created:    2017-08-04
author:     lixianmin

*********************************************************************/

using System;
using UnityEngine;

namespace Unique.UI
{
    internal class RichTextMesh
    {
        public void SetSize (int size)
        {
            if (size < 0 || size == _size)
            {
                return;
            }

            _size = size;

            if (null == _mesh)
            {
                _mesh = new Mesh();
            }
            else
            {
                _mesh.Clear();
            }

            _vertices = new Vector3[size * 4];
            _uv = new Vector2[size * 4];
            _triangles = new int[size * 6];

            _mesh.vertices = _vertices;
            _mesh.uv = _uv;
            _mesh.triangles = _triangles;
        }

        public void UpdateVertices (Vector3[] srcVertices, int index)
        {
            if (null == _mesh)
            {
                return;
            }

            if (null == srcVertices || index < 0)
            {
                return;
            }

            var count = srcVertices.Length;
            if (count == 0)
            {
                return;
            }

            var startIndex = index * 4;
            Array.Copy(srcVertices, 0, _vertices, startIndex, count);
            _mesh.vertices = _vertices;
        }

        public void UpdateUV (Rect rect, int index)
        {
            if (null == _mesh)
            {
                return;
            }

            if (index < 0)
            {
                return;
            }

            var startIndex = index * 4;
            var uv = _uv;

            uv[startIndex].x =  rect.x;
            uv[startIndex].y =  rect.y;

            uv[startIndex + 1].x = rect.x + rect.width;
            uv[startIndex + 1].y = rect.y + rect.height;

            uv[startIndex + 2].x = rect.x + rect.width;
            uv[startIndex + 2].y = rect.y;

            uv[startIndex+3].x = rect.x ;
            uv[startIndex+3].y = rect.y + rect.height;

            _mesh.uv = uv;
        }

        public void UpdateTriangles (int index)
        {
            if (null == _mesh)
            {
                return;
            }

            if (index < 0)
            {
                return;
            }

            int startIndex = index * 6;
            int offset = index * 4;

            _triangles[startIndex + 0] = 0 + offset;
            _triangles[startIndex + 1] = 1 + offset;
            _triangles[startIndex + 2] = 2 + offset;

            _triangles[startIndex + 3] = 1 + offset;
            _triangles[startIndex + 4] = 0 + offset;
            _triangles[startIndex + 5] = 3 + offset;

            _mesh.triangles = _triangles;
        }

        public void Clear ()
        {
            if (null != _mesh)
            {
                _mesh.Clear();
            }
        }

        public Mesh GetMesh ()
        {
            return _mesh;
        }

        private Vector3[] _vertices;
        private Vector2[] _uv;
        private int[] _triangles;

        private int _size;
        private Mesh _mesh;
    }
}