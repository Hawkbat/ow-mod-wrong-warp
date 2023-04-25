using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class VanillaProp : WrongWarpBehaviour, IConfigurable<VanillaPropConfig>
    {
        public string PropPath;
        public List<string> StreamingAssetBundlesToLoad = new List<string>();
        public bool GenerateMeshColliders;
        public List<VanillaPropConfig.BoxColliderConfig> BoxColliders = new List<VanillaPropConfig.BoxColliderConfig>();
        public List<VanillaPropConfig.SphereColliderConfig> SphereColliders = new List<VanillaPropConfig.SphereColliderConfig>();
        public List<VanillaPropConfig.CapsuleColliderConfig> CapsuleColliders = new List<VanillaPropConfig.CapsuleColliderConfig>();

        private GameObject prop;
        private readonly List<Component> generatedMeshColliders = new List<Component>();
        private readonly List<GameObject> primitiveColliderObjects = new List<GameObject>();

        public void ApplyConfig(VanillaPropConfig config)
        {
            if (!string.IsNullOrEmpty(config.propPath)) PropPath = config.propPath;
            if (config.streamingAssetBundlesToLoad != null) StreamingAssetBundlesToLoad = config.streamingAssetBundlesToLoad;
            if (config.generateMeshColliders.HasValue) GenerateMeshColliders = config.generateMeshColliders.Value;
            if (config.boxColliders != null)
            {
                foreach (var box in config.boxColliders)
                {
                    if (!BoxColliders.Contains(box)) BoxColliders.Add(box);
                }
            }
            if (config.sphereColliders != null)
            {
                foreach (var sphere in config.sphereColliders)
                {
                    if (!SphereColliders.Contains(sphere)) SphereColliders.Add(sphere);
                }
            }
            if (config.capsuleColliders != null)
            {
                foreach (var capsule in config.capsuleColliders)
                {
                    if (!CapsuleColliders.Contains(capsule)) CapsuleColliders.Add(capsule);
                }
            }
        }

        public override void WireUp()
        {
            if (!string.IsNullOrEmpty(PropPath))
            {
                prop = Mod.NewHorizonsApi.SpawnObject(transform.root.gameObject, GetComponentInParent<Sector>(), PropPath, Vector3.zero, Vector3.zero, 1f, false);

                foreach (StreamingMeshHandle handle in prop.GetComponentsInChildren<StreamingMeshHandle>())
                {
                    StreamingManager.LoadStreamingAssets(handle.assetBundle);
                }

                foreach (string bundle in StreamingAssetBundlesToLoad)
                {
                    StreamingManager.LoadStreamingAssets(bundle);
                }

                prop.transform.SetParent(transform, false);
                prop.transform.localPosition = Vector3.zero;
                prop.transform.localEulerAngles = Vector3.zero;
                prop.transform.localScale = Vector3.one;


                var pictureFrameInterface = prop.GetComponentInChildren<PictureFrameDoorInterface>();
                if (pictureFrameInterface != null)
                {
                    UnityUtils.DoAfterFrames(Mod, 1, () => pictureFrameInterface.ToggleOpenState());
                    UnityUtils.DoAfterFrames(Mod, 2, () => pictureFrameInterface.ToggleOpenState());
                    UnityUtils.DoAfterFrames(Mod, 3, () => pictureFrameInterface.ToggleOpenState());
                }
            }

            if (generatedMeshColliders.Count > 0 && !GenerateMeshColliders)
            {
                foreach (var c in generatedMeshColliders) Destroy(c);
                generatedMeshColliders.Clear();
            }
            if (generatedMeshColliders.Count == 0 && GenerateMeshColliders)
            {
                foreach (var mf in gameObject.GetComponentsInChildren<MeshFilter>())
                {
                    var mc = mf.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    generatedMeshColliders.Add(mc);
                    var oc = mf.gameObject.AddComponent<OWCollider>();
                    generatedMeshColliders.Add(oc);
                }
            }

            var generatePrimitiveColliders = BoxColliders.Count > 0 || SphereColliders.Count > 0 || CapsuleColliders.Count > 0;
            if (primitiveColliderObjects.Count > 0 && !generatePrimitiveColliders)
            {
                foreach (var g in primitiveColliderObjects) Destroy(g);
                primitiveColliderObjects.Clear();
            }
            if (primitiveColliderObjects.Count == 0 && generatePrimitiveColliders)
            {
                foreach (var box in BoxColliders)
                {
                    var go = new GameObject("Box Collider");
                    go.transform.SetParent(gameObject.transform, false);
                    go.transform.localPosition = box.center ?? Vector3.zero;
                    go.transform.localEulerAngles = box.rotation ?? Vector3.zero;
                    var c = go.AddComponent<BoxCollider>();
                    c.size = box.size ?? Vector3.one;
                    primitiveColliderObjects.Add(go);
                }
                foreach (var sphere in SphereColliders)
                {
                    var go = new GameObject("Sphere Collider");
                    go.transform.SetParent(gameObject.transform, false);
                    go.transform.localPosition = sphere.center ?? Vector3.zero;
                    var c = go.AddComponent<SphereCollider>();
                    c.radius = sphere.radius ?? 1f;
                    primitiveColliderObjects.Add(go);
                }
                foreach (var capsule in CapsuleColliders)
                {
                    var go = new GameObject("Capsule Collider");
                    go.transform.SetParent(gameObject.transform, false);
                    go.transform.localPosition = capsule.center ?? Vector3.zero;
                    go.transform.localEulerAngles = capsule.rotation ?? Vector3.zero;
                    var c = go.AddComponent<CapsuleCollider>();
                    c.height = capsule.height ?? 1f;
                    c.radius = capsule.radius ?? 1f;
                    primitiveColliderObjects.Add(go);
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
