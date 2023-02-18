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
        public Sensor ScrambleSensor;
        public GameObject ScrambledObj;
        public GameObject UnscrambledObj;

        public void ApplyConfig(ExoCorpseConfig config)
        {

        }

        public override void WireUp()
        {
            Mod.ExoCorpses.All.Add(this);
        }

        public void Update()
        {
            var scrambled = Sensor.IsActivated(ScrambleSensor);
            ScrambledObj.SetActive(scrambled);
            UnscrambledObj.SetActive(!scrambled);
        }
    }
}
