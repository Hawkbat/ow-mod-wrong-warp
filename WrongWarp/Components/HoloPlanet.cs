using ModDataTools.Assets;
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
    public class HoloPlanet : WrongWarpBehaviour, IConfigurable<HoloPlanetConfig>
    {
        public PlanetAsset Planet;
        public PlanetAsset CenterBody;
        public float ScaleFactor;
        public float DistanceFactor;
        public Color ErrorColor;

        [SerializeField]
        Transform planetTransform;
        [SerializeField]
        Transform centerBodyTransform;
        [SerializeField]
        float radius;

        Renderer renderer;
        Material mat;
        Color originalColor;

        MissingExhibit missingExhibit;
        EyeExhibitState quantumState;

        public void ApplyConfig(HoloPlanetConfig config)
        {
            if (config.scaleFactor.HasValue) ScaleFactor = config.scaleFactor.Value;
            if (config.distanceFactor.HasValue) DistanceFactor = config.distanceFactor.Value;
        }

        public override void WireUp()
        {
            var planet = Mod.NewHorizonsApi.GetPlanet(Planet.FullID);
            if (planet)
            {
                planetTransform = planet.transform;
                radius = Mathf.Max(200f, UnityUtils.GetComponentAtPath<SphereShape>(planet.transform, "Volumes/Ruleset")?.radius ?? 0f) * 0.5f;
                missingExhibit = planet.GetComponentInChildren<MissingExhibit>(true);
                quantumState = planet.GetComponentInChildren<EyeExhibitState>(true);
            }
            centerBodyTransform = Mod.NewHorizonsApi.GetPlanet(CenterBody.FullID)?.transform;

            renderer = GetComponent<Renderer>();
            if (renderer)
            {
                mat = new Material(renderer.sharedMaterial);
                renderer.sharedMaterial = mat;
                originalColor = mat.color;
            }

        }

        public void LateUpdate()
        {
            if (centerBodyTransform && planetTransform)
            {
                var diff = planetTransform.position - centerBodyTransform.position;

                transform.localPosition = diff * DistanceFactor * ScaleFactor;
                transform.localScale = Vector3.one * radius * ScaleFactor;
            }

            if (renderer && mat)
            {
                renderer.enabled = true;
                mat.color = originalColor;

                mat.SetVector("_ObjectNormal", transform.parent.up);
                mat.SetVector("_ObjectPosition", transform.parent.parent.position);
                mat.SetFloat("_ObjectHeight", transform.parent.localPosition.y + transform.localPosition.y);

                if (missingExhibit && !missingExhibit.IsRestored())
                {
                    mat.color = ErrorColor;
                    if (((Time.time + 0.25f) % 1f) < 0.5f)
                    {
                        renderer.enabled = false;
                    }
                }

                if (quantumState && !quantumState.IsStateActive())
                {
                    mat.color = ErrorColor;
                    if ((Time.time % 1f) < 0.5f)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }
    }
}
