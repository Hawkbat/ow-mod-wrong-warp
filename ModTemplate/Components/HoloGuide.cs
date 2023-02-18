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
    public class HoloGuide : WrongWarpBehaviour, IConfigurable<HoloGuideConfig>
    {
        public GameObject Character;
        public string CharacterPath;
        public float CharacterHeight;
        public string DialoguePath;

        CharacterDialogueTree dialogueTree;
        RemoteDialogueTrigger remoteTrigger;
        CharacterAnimController characterAnim;
        SolanumAnimController solanumAnim;
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
                Character = Mod.NewHorizonsApi.SpawnObject(transform.root.gameObject, gameObject.GetComponentInParent<Sector>(), CharacterPath, transform.position, transform.eulerAngles, 1f, false);
            }
            var t = Character ? Character.transform : null;
            if (t)
            {
                mat = new Material(Mod.Museum.HologramMaterial);
                
                Character = t.gameObject;
                characterAnim = Character.GetComponent<CharacterAnimController>();
                solanumAnim = Character.GetComponent<SolanumAnimController>();

                if (t.name == "Ghostbird_IP_ANIM")
                {
                    Character.GetComponent<Animator>().SetTrigger(Animator.StringToHash("Default"));
                }

                /*transform.position = t.position;
                transform.rotation = t.rotation;
                transform.parent = t.parent;*/
                t.parent = transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                foreach (var renderer in t.GetComponentsInChildren<Renderer>())
                {
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(_ => mat).ToArray();
                }
            }
            if (!string.IsNullOrEmpty(DialoguePath))
            {
                var dialogueObj = new GameObject("Dialogue");
                dialogueObj.transform.parent = transform;
                dialogueObj.transform.localPosition = Vector3.up * CharacterHeight * 1.2f;
                dialogueObj.transform.localRotation = Quaternion.identity;

                (dialogueTree, remoteTrigger) = Mod.NewHorizonsApi.SpawnDialogue(Mod, dialogueObj, DialoguePath, 2f);
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
