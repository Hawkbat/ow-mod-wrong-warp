using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class HeatImageEffect : WrongWarpBehaviour
    {
        Material mat;
        Camera cam;

        public override void WireUp()
        {
            mat = new Material(Mod.SystemAssetBundle.LoadAsset<Material>($"Assets/ModAssets/Shared/Materials/HeatEffect.mat"));
            cam = GetComponent<Camera>();
            enabled = false;
        }

        void OnPreRender()
        {
            if (cam)
            {
                cam.depthTextureMode = DepthTextureMode.DepthNormals;
            }
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture target)
        {
            var heatVolume = Mod.Heat.GetOverheatVolume();
            if (!mat || !heatVolume)
            {
                Graphics.Blit(source, target);
                return;
            }
            var dir = (heatVolume.transform.position - transform.position).normalized;
            var centerPos = transform.InverseTransformPoint(heatVolume.transform.position);
            var centerDir = transform.InverseTransformDirection(dir);

            mat.SetVector("_CenterPosition", centerPos);
            mat.SetVector("_CenterDirection", centerDir);

            Graphics.Blit(source, target, mat);
        }
    }
}
