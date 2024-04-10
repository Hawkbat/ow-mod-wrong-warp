using ModDataTools.Utilities;
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
        static Material invalidMaterial;
        static Material quantumMaterial;
        static Material uvTestMaterial;
        static Dictionary<string, Material> vanillaMaterials = new();

        public MaterialType Type;
        public int MaterialSlot;
        [ConditionalField(nameof(Type), MaterialType.Vanilla)]
        public string MaterialName;

        public enum MaterialType
        {
            Vanilla = 0,
            QuantumRock = 1,
            UVTest = 2,
        }

        public Material GetMaterial(MaterialType type, string name)
        {
            switch (type)
            {
                case MaterialType.Vanilla:
                    if (vanillaMaterials.TryGetValue(name, out Material mat) && mat)
                    {
                        return mat;
                    } else
                    {
                        var found = UnityUtils.FindResource<Material>(name);
                        if (found)
                        {
                            vanillaMaterials[name] = found;
                            return found;
                        }
                    }
                    break;
                case MaterialType.QuantumRock:
                    if (!quantumMaterial)
                        quantumMaterial = Mod.SystemAssetBundle.LoadAsset<Material>("Assets/ModAssets/Shared/Materials/Quantum.mat");
                    //quantumMaterial = UnityUtils.FindResource<Material>("Rock_QM_EyeRock_mat");
                    return quantumMaterial;
                case MaterialType.UVTest:
                    if (!uvTestMaterial)
                        uvTestMaterial = Mod.SystemAssetBundle.LoadAsset<Material>("Assets/ModAssets/Shared/Materials/UVTest.mat");
                    return uvTestMaterial;
            }
            if (!invalidMaterial)
            {
                invalidMaterial = new Material(Shader.Find("Unlit/Color"));
                invalidMaterial.color = Color.magenta;
            }
            return invalidMaterial;
        }

        public override void WireUp()
        {
            Material mat = GetMaterial(Type, MaterialName);
            foreach (var r in GetComponents<Renderer>())
            {
                var arr = r.sharedMaterials;
                arr[MaterialSlot] = mat;
                r.sharedMaterials = arr;
            }
        }
    }
}
