using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Objects
{
    [Serializable]
    public struct RGBA
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public static implicit operator Color(RGBA c) => new Color(c.r, c.g, c.b, c.a);
    }
}
