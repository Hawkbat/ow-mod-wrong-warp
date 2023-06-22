using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrongWarp.Components
{
    public class ProbePortScreenPlayerStatus : ProbePortScreen
    {
        public override string GetText()
        {
            var oxygen = Locator.GetPlayerController()._playerResources.GetOxygenInSeconds();
            var health = Locator.GetPlayerController()._playerResources.GetHealthFraction();
            return $"[Visitor Vitals]\nOxygen Remaining: {oxygen}\nBiological Integrity: {Math.Round(health * 100)}%";
        }
    }
}
