using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;
using WrongWarp.Utils;
using ModDataTools.Assets;

namespace WrongWarp.Components
{
    public class HoloGuide : WrongWarpBehaviour, IConfigurable<HoloGuideConfig>
    {
        public GameObject Character;
        public string CharacterPath;
        public float CharacterHeight;
        public DialogueAsset Dialogue;

        Material mat;

        public void ApplyConfig(HoloGuideConfig config)
        {
            if (config.characterPath != null) CharacterPath = config.characterPath;
            if (config.characterHeight.HasValue) CharacterHeight = config.characterHeight.Value;
        }

        public override void WireUp()
        {
            if (!Character && !string.IsNullOrEmpty(CharacterPath))
            {
                Character = Mod.NewHorizonsApi.SpawnObject(Mod, transform.root.gameObject, gameObject.GetComponentInParent<Sector>(), CharacterPath, transform.position, transform.eulerAngles, 1f, false);
            }
            var t = Character ? Character.transform : null;
            if (t)
            {
                mat = new Material(Mod.Museum.HologramMaterial);
                
                Character = t.gameObject;
                foreach (StreamingMeshHandle handle in Character.GetComponentsInChildren<StreamingMeshHandle>())
                {
                    StreamingManager.LoadStreamingAssets(handle.assetBundle);
                }

                if (t.name == "Ghostbird_IP_ANIM")
                {
                    Character.GetComponent<Animator>().SetTrigger(Animator.StringToHash("Default"));
                }

                t.parent = transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                foreach (var renderer in t.GetComponentsInChildren<Renderer>())
                {
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(_ => mat).ToArray();
                }
            }
            var dialoguePath = Dialogue ? Dialogue.GetXmlOutputPath() : null;
            if (!string.IsNullOrEmpty(dialoguePath))
            {
                var dialogueObj = new GameObject("Dialogue");
                dialogueObj.transform.parent = transform;
                dialogueObj.transform.localPosition = Vector3.up * CharacterHeight * 1.2f;
                dialogueObj.transform.localRotation = Quaternion.identity;

                var pathToAnimController = UnityUtils.GetTransformPath(t, true);

                var (dialogueTree, remoteTrigger) = Mod.NewHorizonsApi.SpawnDialogue(Mod, dialogueObj.transform.root.gameObject, dialoguePath, 2f, 1f, null, 5f, pathToAnimController, 0f);
                dialogueTree.transform.parent = dialogueObj.transform;
                dialogueTree.transform.localPosition = Vector3.zero;
                dialogueTree.transform.localRotation = Quaternion.identity;
                if (remoteTrigger) remoteTrigger.transform.SetParent(dialogueObj.transform, true);
            }
        }

        public void LateUpdate()
        {
            if (mat)
            {
                mat.SetVector("_ObjectNormal", transform.up);
                mat.SetVector("_ObjectPosition", transform.position);
                mat.SetFloat("_ObjectHeight", CharacterHeight);
            }
        }
    }
}
