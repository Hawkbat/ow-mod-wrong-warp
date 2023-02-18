using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Utils
{
    public static class UnityUtils
    {
        public static void DoWhen(WrongWarpMod mod, Func<bool> criteria, Action action)
        {
            mod.StartCoroutine(DoWhenCouroutine(criteria, action));
        }

        static IEnumerator DoWhenCouroutine(Func<bool> criteria, Action action)
        {
            while (!criteria()) yield return null;
            action();
        }

        public static void DoAfterFrames(WrongWarpMod mod, int frameCount, Action action, bool bypassSystemCheck = false)
        {
            mod.StartCoroutine(DoAfterFramesCouroutine(mod, frameCount, action, bypassSystemCheck));
        }

        private static IEnumerator DoAfterFramesCouroutine(WrongWarpMod mod, int frameCount, Action action, bool bypassSystemCheck)
        {
            var currentLoadCount = mod.SystemLoadCounter;
            var isInWrongWarpSystem = mod.IsInWrongWarpSystem;
            for (int i = 0; i < frameCount; i++) yield return null;
            if (bypassSystemCheck || mod.SystemLoadCounter == currentLoadCount && mod.IsInWrongWarpSystem == isInWrongWarpSystem)
            {
                action();
            }
        }

        public static void DoAfterSeconds(WrongWarpMod mod, float secs, Action action)
        {
            mod.StartCoroutine(DoAfterSecondsCoroutine(mod, secs, action));
        }

        private static IEnumerator DoAfterSecondsCoroutine(WrongWarpMod mod, float secs, Action action)
        {
            var currentLoadCount = mod.SystemLoadCounter;
            var isInWrongWarpSystem = mod.IsInWrongWarpSystem;
            yield return new WaitForSeconds(secs);
            if (mod.SystemLoadCounter == currentLoadCount && mod.IsInWrongWarpSystem == isInWrongWarpSystem)
            {
                action();
            }
        }

        public static string GetTransformPath(Transform transform)
        {
            if (transform == null) return null;
            var path = transform.name;
            while (transform.parent != null)
            {
                path = transform.parent.name + "/" + path;
                transform = transform.parent;
            }
            return path;
        }

        public static Transform GetTransformAtPath(Transform transform, string path)
        {
            if (path == null) return null;
            var t = transform;
            var q = new Queue<string>(path.Split('/'));
            while (q.Count > 0 && t != null)
            {
                var s = q.Dequeue();
                if (s == "..") t = t.parent;
                else if (s != ".") t = t.Find(s);
            }
            return t;
        }

        public static TComponent GetComponentAtPath<TComponent>(Transform transform, string path) where TComponent : Component
        {
            if (path == null) return null;
            var t = GetTransformAtPath(transform, path);
            if (!t) return null;
            return t.GetComponent<TComponent>();
        }

        public static List<TComponent> GetComponentsAtPaths<TComponent>(Transform transform, List<string> paths) where TComponent : Component
        {
            return paths?.Select(p => GetComponentAtPath<TComponent>(transform, p)).ToList();
        }

        public static List<Transform> GetChildren(Transform transform)
        {
            var children = new List<Transform>();
            if (transform != null)
            {
                foreach (Transform t in transform) children.Add(t);
            }
            return children;
        }

        public static T FindResource<T>(string name) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(o => o.name == name);
        }
    }
}
