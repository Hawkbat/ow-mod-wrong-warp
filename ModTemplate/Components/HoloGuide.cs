using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class HoloGuide : WrongWarpBehaviour, IConfigurable<HoloGuideConfig>
    {
        public GameObject Character;
        public string CharacterPath;
        public float CharacterHeight;

        CharacterAnimController characterAnim;
        SolanumAnimController solanumAnim;
        GhostEffects ghostAnim;
        Material mat;

        public void ApplyConfig(HoloGuideConfig config)
        {
            if (config.characterPath != null) CharacterPath = config.characterPath;
            if (config.characterHeight.HasValue) CharacterHeight = config.characterHeight.Value;
        }

        public override void WireUp()
        {
            var t = Character && string.IsNullOrEmpty(CharacterPath) ? Character.transform : GetTransformAtPath(CharacterPath);
            if (t)
            {
                mat = new Material(Mod.Museum.HologramMaterial);
                
                Character = t.gameObject;
                characterAnim = Character.GetComponent<CharacterAnimController>();
                solanumAnim = Character.GetComponent<SolanumAnimController>();
                ghostAnim = Character.GetComponent<GhostEffects>();

                if (ghostAnim && !ghostAnim._animator)
                {
                    ghostAnim._animator = Character.GetComponent<Animator>();
                    ghostAnim.PlayDefaultAnimation();
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
