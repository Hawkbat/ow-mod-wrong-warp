using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    public class BitCrushAudioFilter : MonoBehaviour
    {
        const int SAMPLE_RATE = 48000;

        public int BitDepth = 4;
        public int BitRate = 2048;

        public void OnAudioFilterRead(float[] data, int channels)
        {
            int max = (int)Mathf.Pow(2, BitDepth) - 1;
            int step = SAMPLE_RATE / BitRate;

            int i = 0;
            while (i < data.Length)
            {
                float leftFirstSample = Mathf.Round((data[i] + 1.0f) * max) / max - 1.0f;
                float rightFirstSample = Mathf.Round((data[i + 1] + 1.0f) * max) / max - 1.0f;

                // this loop causes us to simulate a down-sample to a lower sample rate
                for (int j = 0; j < step * 2 && i < data.Length; j += 2)
                {
                    data[i] = leftFirstSample;
                    data[i + 1] = rightFirstSample;

                    // move on
                    i += 2;
                }
            }
        }
    }
}
