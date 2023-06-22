using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class TeleporterEffect : WrongWarpBehaviour
    {
        public float BoxSize;
        public float BoxSpacing;
        public int Width;
        public int Height;
        public int Depth;
        public float Duration;
        public Vector3 Offset;

        private ParticleSystem ps;

        public override void WireUp()
        {
            ps = GetComponentInChildren<ParticleSystem>();
            StartCoroutine(DoEffect());
        }

        public IEnumerator DoEffect()
        {
            float expectedFrameTime = 0f;
            float actualFrameTime = 0f;
            var offset = -0.5f * BoxSize * new Vector3(Width, 0f, Depth) + Offset;
            float frameRate = Duration / (Width * Depth * Height);

            var main = ps.main;
            main.maxParticles = Width * Height * Depth;
            if (main.maxParticles > ushort.MaxValue)
            {
                LogUtils.Error($"WAY TOO MANY PARTICLES for this teleporter effect! {UnityUtils.GetTransformPath(transform)}");
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        var emitParams = new ParticleSystem.EmitParams();
                        emitParams.startSize = BoxSize - BoxSpacing;
                        emitParams.position = offset + new Vector3(0.5f + x, 0.5f + y, 0.5f + z) * BoxSize;
                        emitParams.startLifetime = Duration * 2f;
                        ps.Emit(emitParams, 1);

                        expectedFrameTime += frameRate;
                        while (expectedFrameTime > actualFrameTime)
                        {
                            yield return null;
                            actualFrameTime += Time.deltaTime;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(Duration * 2f);
            Destroy(gameObject);
        }
    }
}
