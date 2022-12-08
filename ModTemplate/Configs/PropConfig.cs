using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrongWarp.Objects;

namespace WrongWarp.Configs
{
    [Serializable]
    public class PropConfig : ComponentConfig
    {
        public string propPath;
        public List<string> streamingAssetBundlesToLoad;
        public bool? generateMeshColliders;
        public List<BoxColliderConfig> boxColliders;
        public List<SphereColliderConfig> sphereColliders;
        public List<CapsuleColliderConfig> capsuleColliders;

        [Serializable]
        public class BoxColliderConfig
        {
            public XYZ? center;
            public XYZ? rotation;
            public XYZ? size;
        }

        [Serializable]
        public class SphereColliderConfig
        {
            public XYZ? center;
            public float? radius;
        }

        [Serializable]
        public class CapsuleColliderConfig
        {
            public XYZ? center;
            public XYZ? rotation;
            public float? height;
            public float? radius;
        }
    }
}
