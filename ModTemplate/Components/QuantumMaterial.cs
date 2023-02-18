using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class VanillaMaterial : WrongWarpBehaviour
    {
        public static Material invalidMaterial;
        public static Material quantumMaterial;
        public static Material uvTestMaterial;

        public MaterialType Type;

        public enum MaterialType
        {
            Unknown = 0,
            QuantumRock = 1,
            UVTest = 2,
        }

        public Material GetMaterial(MaterialType type)
        {
            switch (type)
            {
                case MaterialType.QuantumRock:
                    if (!quantumMaterial)
                        quantumMaterial = Mod.SystemAssetBundle.LoadAsset<Material>("Assets/ModAssets/Shared/Materials/Quantum.mat");
                    //quantumMaterial = UnityUtils.FindResource<Material>("Rock_QM_EyeRock_mat");
                    return quantumMaterial;
                case MaterialType.UVTest:
                    if (!uvTestMaterial)
                        uvTestMaterial = Mod.SystemAssetBundle.LoadAsset<Material>("Assets/ModAssets/Shared/Materials/UVTest.mat");
                    return uvTestMaterial;
                default:
                    if (!invalidMaterial)
                    {
                        invalidMaterial = new Material(Shader.Find("Unlit/Color"));
                        invalidMaterial.color = Color.magenta;
                    }
                    return invalidMaterial;
                    
            }
        }

        public override void WireUp()
        {
            Material mat = GetMaterial(Type);
            foreach (var r in GetComponents<Renderer>())
            {
                r.sharedMaterial = mat;
            }
        }
    }
}
