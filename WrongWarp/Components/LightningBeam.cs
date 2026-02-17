using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WrongWarp.Components
{
    [ExecuteInEditMode]
    public class LightningBeam : MonoBehaviour
    {
        [SerializeField] Transform startTarget;
        [SerializeField] Transform endTarget;
        [SerializeField] bool reverse;
        [SerializeField] LineRenderer line;
        [SerializeField] LineRenderer splitLine;
        [SerializeField] float updateRate = 0.1f;
        [SerializeField] float pointDensity = 2f;
        [SerializeField] float splitChance = 0.1f;
        [SerializeField] float splitLength = 0.5f;
        [SerializeField] float splitAngle = 30f;
        [SerializeField] float noiseAmount = 0.5f;
        [SerializeField] float noiseScale = 0.3f;
        [SerializeField] float noiseSpeed = 5f;

        float updateTimer;
        List<LineRenderer> splitLines = [];
        List<LineRenderer> splitLinePool = [];

        public void SetPoints(Transform startTarget, Transform endTarget)
        {
            this.startTarget = startTarget;
            this.endTarget = endTarget;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        protected void OnEnable()
        {
            updateTimer = 0f;
            ResetLine();
            line.enabled = true;
        }

        protected void OnDisable()
        {
            ResetLine();
        }

        protected void Awake()
        {
            if (!enabled)
            {
                ResetLine();
            }
        }

        protected void LateUpdate()
        {
            if (!line) return;

            updateTimer -= Time.deltaTime;
            if (updateTimer <= 0f)
            {
                updateTimer += Mathf.Max(updateRate, Time.deltaTime);
                UpdateBeam();
            }
        }

        void ResetLine()
        {
            line.positionCount = 2;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
            line.enabled = false;

            foreach (var split in splitLines)
            {
                if (split) DestroyImmediate(split.gameObject);
            }
            splitLines.Clear();
            foreach (var split in splitLinePool)
            {
                if (split) DestroyImmediate(split.gameObject);
            }
            splitLinePool.Clear();
        }

        void UpdateBeam()
        {
            foreach (var split in splitLines)
            {
                if (!split) continue;
                split.gameObject.SetActive(false);
                splitLinePool.Add(split);
            }
            splitLines.Clear();

            var start = startTarget ? startTarget.position : transform.position;
            var end = endTarget ? endTarget.position : transform.position + transform.forward * 20f;

            if (reverse)
            {
                (end, start) = (start, end);
            }

            var dir = (end - start).normalized;

            var right = Vector3.Cross(dir, Vector3.up);
            if (right.sqrMagnitude < 0.001f)
                right = Vector3.Cross(dir, Vector3.forward);
            right = right.normalized;

            var up = Vector3.Cross(right, dir).normalized;

            var segmentCount = Mathf.CeilToInt(Vector3.Distance(start, end) * pointDensity);
            line.positionCount = segmentCount + 1;

            line.SetPosition(0, start);
            line.SetPosition(segmentCount, end);

            for (int i = 1; i < segmentCount; i++)
            {
                float t = (float)i / segmentCount;
                var point = Vector3.Lerp(start, end, t);
                var noiseX = (Mathf.PerlinNoise(i * noiseScale, Time.time * noiseSpeed) - 0.5f) * noiseAmount;
                var noiseY = (Mathf.PerlinNoise(i * noiseScale + 100f, Time.time * noiseSpeed + 100f) - 0.5f) * noiseAmount;
                point += right * noiseX + up * noiseY;
                line.SetPosition(i, point);

                var shouldSplit = false;
                //shouldSplit = UnityEngine.Random.value < splitChance;
                shouldSplit = Math.Abs(noiseX) + Math.Abs(noiseY) > splitChance;

                if (shouldSplit)
                {
                    var splitDir = Quaternion.AngleAxis(UnityEngine.Random.Range(-splitAngle, splitAngle), dir) * Quaternion.AngleAxis(UnityEngine.Random.Range(-splitAngle, splitAngle), right) * dir;

                    var actualSplitLength = splitLength * (0.5f + UnityEngine.Random.value);

                    var splitEnd = point + splitDir * actualSplitLength;
                    LineRenderer splitLineInstance;
                    if (splitLinePool.Count > 0)
                    {
                        splitLineInstance = splitLinePool[splitLinePool.Count - 1];
                        splitLinePool.RemoveAt(splitLinePool.Count - 1);
                        splitLineInstance.gameObject.SetActive(true);
                    }
                    else
                    {
                        splitLineInstance = Instantiate(splitLine.gameObject, transform).GetComponent<LineRenderer>();
                        splitLineInstance.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    }

                    var splitSegmentCount = Mathf.CeilToInt(actualSplitLength * pointDensity);
                    splitLineInstance.positionCount = splitSegmentCount + 1;

                    splitLineInstance.SetPosition(0, point);
                    splitLineInstance.SetPosition(splitSegmentCount, splitEnd);

                    for (int j = 1; j < splitSegmentCount; j++)
                    {
                        float splitT = (float)j / splitSegmentCount;
                        var splitPoint = Vector3.Lerp(point, splitEnd, splitT);
                        var splitNoiseX = (Mathf.PerlinNoise(j * noiseScale + 200f, Time.time * noiseSpeed + 200f) - 0.5f) * noiseAmount;
                        var splitNoiseY = (Mathf.PerlinNoise(j * noiseScale + 300f, Time.time * noiseSpeed + 300f) - 0.5f) * noiseAmount;
                        splitPoint += right * splitNoiseX + up * splitNoiseY;
                        splitLineInstance.SetPosition(j, splitPoint);
                    }

                    splitLines.Add(splitLineInstance);
                }
            }
        }
    }
}
