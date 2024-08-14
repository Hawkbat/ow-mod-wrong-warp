using Epic.OnlineServices;
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
    public class ExoCorpse : WrongWarpBehaviour
    {
        AntiTechSensor scrambleSensor;
        NomaiText blackBoxText;
        bool wasScrambled;

        public bool IsScrambled => Sensor.IsActivated(scrambleSensor);

        public override void WireUp()
        {
            Mod.ExoCorpses.All.Add(this);
            scrambleSensor = GetComponentInChildren<AntiTechSensor>();
            blackBoxText = GetComponentInChildren<NomaiText>();
            if (blackBoxText)
            {
                blackBoxText._location = IsScrambled ? NomaiText.Location.A : NomaiText.Location.B;
            }
        }

        void Update()
        {
            if (wasScrambled != IsScrambled)
            {
                wasScrambled = IsScrambled;
                if (blackBoxText)
                {
                    blackBoxText._location = IsScrambled ? NomaiText.Location.A : NomaiText.Location.B;
                    foreach (var id in blackBoxText._dictNomaiTextData.Keys.ToArray())
                    {
                        var node = blackBoxText._dictNomaiTextData[id];
                        node.IsTranslated = false;
                        blackBoxText._dictNomaiTextData[id] = node;
                    }
                }
            }
        }
    }
}
