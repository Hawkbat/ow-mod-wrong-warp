using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Objects
{
    [Serializable]
    public struct XYZ
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(XYZ v) => new Vector3(v.x, v.y, v.z);
        public static Vector3 operator +(XYZ v, Vector3 o) => ((Vector3)v) + o;
        public static Vector3 operator -(XYZ v, Vector3 o) => ((Vector3)v) - o;
        public static Vector3 operator *(XYZ v, float f) => ((Vector3)v) * f;
        public static Vector3 operator /(XYZ v, float f) => ((Vector3)v) / f;
    }
}
