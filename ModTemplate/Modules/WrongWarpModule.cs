using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Modules
{
    public abstract class WrongWarpModule
    {
        public WrongWarpMod Mod;

        public WrongWarpModule(WrongWarpMod mod)
        {
            Mod = mod;
            Mod.Modules.Add(this);
        }

        public virtual void OnSystemLoad() { }
        public virtual void OnSystemUnload() { }
        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnFixedUpdate() { }

        public virtual bool Active => LoadManager.GetCurrentScene() == OWScene.SolarSystem && Mod.IsInWrongWarpSystem;

        protected void DoAfterFrames(int frameCount, Action action)
            => UnityUtils.DoAfterFrames(Mod, frameCount, action);

        protected void DoAfterSeconds(float secs, Action action)
            => UnityUtils.DoAfterSeconds(Mod, secs, action);
    }
}
