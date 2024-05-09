using ModDataTools.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public abstract class ProbePortScreen : WrongWarpBehaviour
    {
        [HideInInspector]
        public ProbePort Port;
        public int X;
        public int Y;
        public List<FactAsset> RevealFacts = new();

        public override void WireUp()
        {
            Port = GetComponentInParent<ProbePort>();
        }

        public virtual string GetText() => string.Empty;

        public virtual bool CanNavigate() => true;

        public virtual void OnEnter()
        {
            foreach (var fact in RevealFacts)
            {
                Locator.GetShipLogManager().RevealFact(fact.FullID);
            }
        }

        public virtual void OnExit() {
        
        }

        public virtual bool OnTick(int dx, int dy) => false;
    }
}
