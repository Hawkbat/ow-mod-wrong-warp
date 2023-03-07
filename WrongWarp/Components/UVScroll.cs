using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class UVScroll : WrongWarpBehaviour
    {
        public Vector2 Speed;
        public string TextureName = "_MainTex";

        MeshRenderer mr;
        Material mat;

        public override void WireUp()
        {
            mr = GetComponent<MeshRenderer>();
            mat = mr.sharedMaterial;
        }

        void Update()
        {
            if (mr && mat)
            {
                var offset = mat.GetTextureOffset(TextureName);
                offset += Speed * Time.deltaTime;
                mat.SetTextureOffset(TextureName, offset);
                mr.sharedMaterial = mat;
            }
        }
    }
}
