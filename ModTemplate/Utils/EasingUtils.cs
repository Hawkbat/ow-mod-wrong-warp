using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Utils
{
    public static class EasingUtils
    {
        private const float BACK_CONSTANT = 1.70158f;

        public static EasingFunc Lookup(EasingType type)
        {
            return Lookup(EnumUtils.GetName(type));
        }

        public static EasingFunc Lookup(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            name = name.Substring(0, 1).ToUpper() + name.Substring(1);
            return name switch
            {
                nameof(Linear) => Linear,
                nameof(Quadratic) => Quadratic,
                nameof(Cubic) => Cubic,
                nameof(Quartic) => Quartic,
                nameof(Quintic) => Quintic,
                nameof(Back) => Back,
                nameof(Bounce) => Bounce,
                nameof(Circle) => Circle,
                nameof(Elastic) => Elastic,
                nameof(Exponent2) => Exponent2,
                nameof(ExponentE) => ExponentE,
                nameof(Log10) => Log10,
                nameof(Sine) => Sine,
                nameof(SquareRoot) => SquareRoot,
                _ => null,
            };
        }

        // https://raw.githubusercontent.com/Michaelangel007/easing/master/pics/easing_cheat_sheet.png
        public static EasingFunc Linear = (float t) => t;
        public static EasingFunc Quadratic = (float t) => InPoly(t, 2);
        public static EasingFunc Cubic = (float t) => InPoly(t, 3);
        public static EasingFunc Quartic = (float t) => InPoly(t, 4);
        public static EasingFunc Quintic = (float t) => InPoly(t, 5);
        public static EasingFunc Sextic = (float t) => InPoly(t, 6);
        public static EasingFunc Septic = (float t) => InPoly(t, 7);
        public static EasingFunc Octic = (float t) => InPoly(t, 8);
        public static EasingFunc Back = (float t) => Mathf.Pow(t, 2f) * (t * (BACK_CONSTANT + 1) - BACK_CONSTANT);
        public static EasingFunc Bounce = (float t) => 1f - OutBounce(1f - t);
        public static EasingFunc Circle = (float t) => 1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
        public static EasingFunc Elastic = (float t) => -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((40f * t - 43f) * Mathf.PI / 6);
        public static EasingFunc Exponent2 = (float t) => t <= 0f ? 0f : Mathf.Pow(2f, 10f * (t - 1f));
        public static EasingFunc ExponentE = (float t) => t <= 0f ? 0f : Mathf.Pow(Mathf.Exp(1f), 10f * (t - 1f));
        public static EasingFunc Log10 = (float t) => 1f - OutLog10(1f - t);
        public static EasingFunc Sine = (float t) => 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        public static EasingFunc SquareRoot = (float t) => 1f - OutSquareRoot(1f - t);

        public static float Ease(float t, EasingFunc inFunc, EasingFunc outFunc)
        {
            if (inFunc != null && outFunc != null) return EaseInOut(t, inFunc, outFunc);
            if (inFunc != null) return EaseIn(t, inFunc);
            if (outFunc != null) return EaseOut(t, outFunc);
            return t;
        }

        public static float EaseIn(float t, EasingFunc inFunc) => inFunc(t);
        public static float EaseOut(float t, EasingFunc outFunc) => 1f - outFunc(1f - t);
        public static float EaseInOut(float t, EasingFunc inOutFunc) => EaseInOut(t, inOutFunc, inOutFunc);
        public static float EaseInOut(float t, EasingFunc inFunc, EasingFunc outFunc)
        {
            if (t < 0.5f) return 0.5f * EaseIn(2f * t, inFunc);
            return 0.5f + 0.5f * EaseOut(2f * (t - 0.5f), outFunc);
        }

        private static float InPoly(float t, int p) => Mathf.Pow(t, p);
        private static float OutBounce(float t)
        {
            float r = 1f / 2.75f;
            float k1 = r;
            float k2 = 2f * r;
            float k3 = 1.5f * r;
            float k4 = 2.5f * r;
            float k5 = 2.25f * r;
            float k6 = 2.625f * r;
            float k0 = 7.5625f;
            if (t < k1) return k0 * Mathf.Pow(t, 2f);
            else if (t < k2) return k0 * Mathf.Pow(t - k3, 2f) + 0.75f;
            else if (t < k4) return k0 * Mathf.Pow(t - k5, 2f) + 0.9375f;
            else return k0 * Mathf.Pow(t - k6, 2f) + 0.984375f;
        }
        private static float OutLog10(float t) => Mathf.Log10(t * 9f + 1f);
        private static float OutSquareRoot(float t) => Mathf.Sqrt(t);
    }
}
