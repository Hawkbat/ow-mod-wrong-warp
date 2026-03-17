using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ProbePortGameSensor : Sensor
    {
        ProbePortScreenGame gameScreen;

        public override void WireUp()
        {
            gameScreen = GetComponent<ProbePortScreenGame>();
        }

        public override float ComputeStrength() => gameScreen && gameScreen.CurrentGame != null && gameScreen.CurrentGame.Win ? 1f : 0f;
    }
}
