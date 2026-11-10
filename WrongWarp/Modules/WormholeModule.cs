using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public class WormholeModule : WrongWarpModule
    {
        GameObject whiteHole;
        GameObject blackHole;
        GameObject blackHoleWarpVolume;
        GameObject heatNova;
        bool hasNovaExploded;

        public WormholeModule(WrongWarpMod mod) : base(mod) { }

        public override void OnSystemLoad()
        {
            var core = Mod.NewHorizonsApi.GetPlanet("WW_CORE");
            blackHole = UnityUtils.GetTransformAtPath(core.transform, "./Sector/WW_C_BLACK_HOLE").gameObject;
            whiteHole = UnityUtils.GetTransformAtPath(core.transform, "./Sector/WW_C_WHITE_HOLE").gameObject;
            blackHoleWarpVolume = UnityUtils.GetTransformAtPath(blackHole.transform, "./DestructionVolume").gameObject;
            var heatNovaBody = Mod.NewHorizonsApi.GetPlanet("WW_HEAT_NOVA");
            heatNova = UnityUtils.GetTransformAtPath(heatNovaBody.transform, "./Sector/Star").gameObject;
            hasNovaExploded = false;
        }

        public override void OnSystemUnload()
        {
            blackHole = null;
            whiteHole = null;
        }

        public override void OnLateUpdate()
        {
            var blackHoleActive = Mod.SaveData[SaveDataFlag.HasPlayedMuseumMelody];
            if (blackHole && !blackHole.activeSelf && blackHoleActive)
            {
                blackHole.SetActive(true);
            }
            if (whiteHole && !whiteHole.activeSelf && !blackHoleActive)
            {
                whiteHole.SetActive(true);
            }
            if (blackHole && blackHole.activeSelf && !blackHoleActive)
            {
                blackHole.SetActive(false);
            }
            if (whiteHole && whiteHole.activeSelf && blackHoleActive)
            {
                whiteHole.SetActive(false);
            }
            if (blackHoleWarpVolume && blackHoleWarpVolume.activeSelf)
            {
                // Handled by WormholeRespawnVolume
                blackHoleWarpVolume.SetActive(false);
            }
            if (!blackHoleActive && TimeLoop.GetMinutesElapsed() > 1f && !hasNovaExploded)
            {
                heatNova.SendMessage("StartSupernova");
                hasNovaExploded = true;
            }
        }

        public float GetHeatNovaRadius()
        {
            if (!hasNovaExploded) return 0f;
            const float SUPERNOVA_EXPLOSION_TIME = 1f;
            const float SUPERNOVA_EXPANSION_RATE = 12f;
            var supernovaRadius = (TimeLoop.GetMinutesElapsed() - SUPERNOVA_EXPLOSION_TIME) * SUPERNOVA_EXPANSION_RATE;
            return supernovaRadius;
        }

        public bool HasHeatNovaOvertakenSpawn()
        {
            if (!hasNovaExploded) return false;
            const float SPAWN_ORBIT_RADIUS = 16000f;
            var supernovaRadius = GetHeatNovaRadius();
            return supernovaRadius >= SPAWN_ORBIT_RADIUS;
        }
    }
}
