using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ProbePort : WrongWarpBehaviour
    {
        public Transform Socket;
        public Canvas InterfaceCanvas;
        public Text InterfaceText;
        public Text InterfaceMarkerLeft;
        public Text InterfaceMarkerRight;
        public Text InterfaceMarkerUp;
        public Text InterfaceMarkerDown;
        public SpriteRenderer[] ProbeGameGrid;
        public Sprite[] ProbeGameSprites;

        [HideInInspector]
        public OWAudioSource AudioSource;
        [HideInInspector]
        public List<ProbePortScreen> Screens = new List<ProbePortScreen>();
        [HideInInspector]
        public ProbePortScreen CurrentScreen;

        public ProbePortScreen ScreenToLeft => Screens.Find(s =>
            s.X == CurrentScreen.X - 1 &&
            s.Y == CurrentScreen.Y);
        public ProbePortScreen ScreenToRight => Screens.Find(s =>
            s.X == CurrentScreen.X + 1 &&
            s.Y == CurrentScreen.Y);
        public ProbePortScreen ScreenAbove => Screens.Find(s =>
            s.X == CurrentScreen.X &&
            s.Y == CurrentScreen.Y + 1);
        public ProbePortScreen ScreenBelow => Screens.Find(s =>
            s.X == CurrentScreen.X &&
            s.Y == CurrentScreen.Y - 1);

        private Vector2 previousRotation;
        private Vector3 initialOffset;
        private Vector3 previousOffset;
        private Vector3 currentOffset;

        protected void Awake()
        {
            AudioSource = GetComponent<OWAudioSource>();

            foreach (var screen in GetComponentsInChildren<ProbePortScreen>()) Screens.Add(screen);
            UpdateScreen();
            initialOffset = Socket.localPosition;
            previousOffset = initialOffset;
        }

        public override void WireUp()
        {
            var probe = Locator.GetProbe();
            probe.OnAnchorProbe += SurveyorProbe_OnAnchorProbe;
            probe.OnStartRetrieveProbe += SurveyorProbe_OnStartRetrieveProbe;
            GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", OnProbeSnapshot);
        }

        protected void OnDestroy()
        {
            var probe = Locator.GetProbe();
            if (probe)
            {
                probe.OnAnchorProbe -= SurveyorProbe_OnAnchorProbe;
                probe.OnStartRetrieveProbe -= SurveyorProbe_OnStartRetrieveProbe;
            }
            GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", OnProbeSnapshot);
        }

        protected virtual void Update()
        {
            if (IsProbeInPort())
            {
                var t0 = Mathf.Repeat(Time.time - Time.deltaTime, 1f);
                var t1 = Mathf.Repeat(Time.time, 1f);
                if (t1 < t0)
                {
                    previousOffset = currentOffset;
                    currentOffset = initialOffset + (Vector3)UnityEngine.Random.insideUnitCircle * 0.1f;
                }
                var t = Mathf.SmoothStep(0f, 1f, t1);
                Socket.localPosition = Vector3.Lerp(previousOffset, currentOffset, t);
            }
        }

        private void OnProbeSnapshot(ProbeCamera cam)
        {
            if (cam != Locator.GetProbe().GetRotatingCamera() || !IsProbeInPort()) return;
            // Horizontal rotation goes infinitely in both directions
            // -30deg clockwise/'left', +30deg clockwise/'right'
            // Vertical rotation moves between 0 and -90 degrees
            // -30deg up, +30deg down
            // rotation here is post-rotation
            var deltaRotation = cam._cameraRotation - previousRotation;

            var deltaX = 0;
            var deltaY = 0;

            if (OWInput.IsNewlyPressed(InputLibrary.toolOptionRight)) deltaX += 1;
            if (OWInput.IsNewlyPressed(InputLibrary.toolOptionLeft)) deltaX -= 1;
            if (OWInput.IsNewlyPressed(InputLibrary.toolOptionUp)) deltaY += 1;
            if (OWInput.IsNewlyPressed(InputLibrary.toolOptionDown)) deltaY -= 1;

            var newScreen = Screens.Find(s =>
                s.X == CurrentScreen.X + deltaX &&
                s.Y == CurrentScreen.Y + deltaY);

            if (CurrentScreen.CanNavigate() && newScreen != null)
            {
                CurrentScreen.OnExit();
                CurrentScreen = newScreen;
                Locator.GetPlayerAudioController().PlayEnterLaunchCodes();
                previousRotation = cam._cameraRotation;
                CurrentScreen.OnEnter();
            }
            else
            {
                if (deltaRotation.x != 0) cam.RotateHorizontal(-deltaRotation.x);
                if (deltaRotation.y != 0) cam.RotateVertical(-deltaRotation.y);
                if (!CurrentScreen.OnTick(deltaX, -deltaY))
                {
                    Locator.GetPlayerAudioController().PlayNegativeUISound();
                }
            }

            UpdateScreen();
        }

        public bool IsProbeInPort()
        {
            var probe = Locator.GetProbe();
            var anchor = probe?.GetAnchor();
            return anchor?._probeBody.transform.parent == Socket;
        }

        private void SurveyorProbe_OnAnchorProbe()
        {
            var probe = Locator.GetProbe();
            var anchor = probe?.GetAnchor();
            if (anchor && anchor.transform.IsChildOf(transform))
            {
                if (!IsProbeInPort())
                {
                    anchor.UnanchorFromSurface();
                    anchor.AnchorToSocket(Socket);
                }
                foreach (var light in probe.GetLights())
                {
                    light.SetActivation(false);
                }
                if (InterfaceCanvas)
                {
                    InterfaceCanvas.enabled = true;
                    InterfaceCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    InterfaceCanvas.worldCamera = probe.GetRotatingCamera().GetOWCamera().mainCamera;
                }
                probe.GetRotatingCamera().ResetRotation();
                previousRotation = probe.GetRotatingCamera()._cameraRotation;
                CurrentScreen = Screens.Find(s => s.X == 0 && s.Y == 0);
                CurrentScreen.OnEnter();
                UpdateScreen();
            }
        }

        private void SurveyorProbe_OnStartRetrieveProbe(float retrieveLength)
        {
            var probe = Locator.GetProbe();
            foreach (var light in probe.GetLights())
            {
                light.SetActivation(true);
            }
            if (InterfaceCanvas)
            {
                InterfaceCanvas.worldCamera = null;
                InterfaceCanvas.enabled = false;
            }
            foreach (var sr in ProbeGameGrid)
                sr.enabled = false;
            if (CurrentScreen) CurrentScreen.OnExit();
            CurrentScreen = null;
        }

        private void UpdateScreen()
        {
            if (CurrentScreen != null)
            {
                if (InterfaceText) InterfaceText.text = CurrentScreen.GetText();
                if (InterfaceMarkerLeft) InterfaceMarkerLeft.enabled = CurrentScreen.CanNavigate() && ScreenToLeft != null;
                if (InterfaceMarkerRight) InterfaceMarkerRight.enabled = CurrentScreen.CanNavigate() && ScreenToRight != null;
                if (InterfaceMarkerUp) InterfaceMarkerUp.enabled = CurrentScreen.CanNavigate() && ScreenAbove != null;
                if (InterfaceMarkerDown) InterfaceMarkerDown.enabled = CurrentScreen.CanNavigate() && ScreenBelow != null;
            }
            foreach (var sr in ProbeGameGrid)
                sr.enabled = CurrentScreen != null && CurrentScreen is ProbePortScreenGame;

            var probe = Locator.GetProbe();
            var cam = probe?.GetRotatingCamera();
            if (cam) cam.TakeSnapshot();
        }
    }
}
