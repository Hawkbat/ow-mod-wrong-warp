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
        public string PlanetName;
        public Transform Planet;
        public string CenterBodyName;
        public Transform CenterBody;
        public float Radius;
        public float ScaleFactor;
        public float DistanceFactor;

        Renderer renderer;
        Exhibit exhibit;
        Material mat;

        public void ApplyConfig(HoloPlanetConfig config)
        {
            if (config.planet != null) PlanetName = config.planet;
            if (config.scaleFactor.HasValue) ScaleFactor = config.scaleFactor.Value;
            if (config.distanceFactor.HasValue) DistanceFactor = config.distanceFactor.Value;
            if (config.centerBody != null) CenterBodyName = config.centerBody;
        }

        public override void WireUp()
        {
            var planet = Mod.NewHorizonsApi.GetPlanet(PlanetName);
            if (planet)
            {
                Planet = planet.transform;
                Radius = Mathf.Max(200f, UnityUtils.GetComponentAtPath<SphereShape>(planet.transform, "Volumes/Ruleset")?.radius ?? 0f) * 0.5f;
                exhibit = planet.GetComponentInChildren<Exhibit>();
            }
            CenterBody = Mod.NewHorizonsApi.GetPlanet(CenterBodyName)?.transform;

            renderer = GetComponent<Renderer>();
            if (renderer)
            {
                mat = new Material(renderer.sharedMaterial);
                renderer.sharedMaterial = mat;
            }
        }

        public void LateUpdate()
        {
            if (mat)
            {
                mat.SetVector("_ObjectNormal", transform.parent.up);
                mat.SetVector("_ObjectPosition", transform.parent.parent.position);
                mat.SetFloat("_ObjectHeight", transform.parent.localPosition.y + transform.localPosition.y);
            }
            if (CenterBody && Planet)
            {
                var diff = Planet.position - CenterBody.position;

                transform.localPosition = diff * DistanceFactor * ScaleFactor;
                transform.localScale = Vector3.one * Radius * ScaleFactor;
            }
            if (exhibit)
            {
                renderer.enabled = exhibit.gameObject.activeInHierarchy;
            }
        }
    }
}
