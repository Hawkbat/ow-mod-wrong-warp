using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class ExoCorpse : WrongWarpBehaviour, IConfigurable<ExoCorpseConfig>
    {
        AntiTechSensor scrambleSensor;

        public bool IsScrambled => Sensor.IsActivated(scrambleSensor);

        public void ApplyConfig(ExoCorpseConfig config)
        {

        }

        public override void WireUp()
        {
            Mod.ExoCorpses.All.Add(this);
            scrambleSensor = GetComponentInChildren<AntiTechSensor>();
        }
    }
}
