using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WrongWarp.Configs;

namespace WrongWarp.Components
{
    public class ProbePort : WrongWarpBehaviour, IConfigurable<ProbePortConfig>
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

        public List<ProbePortConfig.ScreenConfig> Screens = new List<ProbePortConfig.ScreenConfig>();
        public ProbePortConfig.ScreenConfig CurrentScreen;
        public ProbeGame CurrentGame;

        public ProbePortConfig.ScreenConfig ScreenToLeft => Screens.Find(s =>
            (s.x ?? 0) == (CurrentScreen.x ?? 0) - 1 &&
            (s.y ?? 0) == (CurrentScreen.y ?? 0));

        public ProbePortConfig.ScreenConfig ScreenToRight => Screens.Find(s =>
            (s.x ?? 0) == (CurrentScreen.x ?? 0) + 1 &&
            (s.y ?? 0) == (CurrentScreen.y ?? 0));
        public ProbePortConfig.ScreenConfig ScreenAbove => Screens.Find(s =>
            (s.x ?? 0) == (CurrentScreen.x ?? 0) &&
            (s.y ?? 0) == (CurrentScreen.y ?? 0) + 1);
        public ProbePortConfig.ScreenConfig ScreenBelow => Screens.Find(s =>
            (s.x ?? 0) == (CurrentScreen.x ?? 0) &&
            (s.y ?? 0) == (CurrentScreen.y ?? 0) - 1);

        private Vector2 previousRotation;
        private OWAudioSource audioSource;

        public void ApplyConfig(ProbePortConfig config)
        {
            if (config.screens != null)
            {
                foreach (var screen in config.screens)
                {
                    if (!Screens.Contains(screen)) Screens.Add(screen);
                }
            }
        }

        public override void WireUp()
        {
            audioSource = GetComponent<OWAudioSource>();
            var probe = Locator.GetProbe();
            probe.OnAnchorProbe += ProbePort_OnAnchorProbe;
            probe.OnUnanchorProbe += ProbePort_OnUnanchorProbe;
            GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", cam =>
            {
                if (cam == probe.GetRotatingCamera() && IsProbeInPort())
                {
                    OnProbeSnapshot();
                }
            });
            UpdateScreen();
        }

        private void OnProbeSnapshot()
        {
            var probe = Locator.GetProbe();
            var cam = probe.GetRotatingCamera();

            // Horizontal rotation goes infinitely in both directions
            // -30deg clockwise/'left', +30deg clockwise/'right'
            // Vertical rotation moves between 0 and -90 degrees
            // -30deg up, +30deg down
            // rotation here is post-rotation
            var deltaRotation = cam._cameraRotation - previousRotation;

            var rotIncrement = 30f;
            var deltaX = Mathf.RoundToInt(deltaRotation.x / rotIncrement);
            var deltaY = Mathf.RoundToInt(-deltaRotation.y / rotIncrement);

            if (CurrentGame != null && !CurrentGame.GameOver)
            {
                if (deltaX != 0) cam.RotateHorizontal(-deltaRotation.x);
                if (deltaY != 0) cam.RotateVertical(-deltaRotation.y);
                CurrentGame.Tick(deltaX, -deltaY);
            }
            else
            {
                var newScreen = Screens.Find(s =>
                    (s.x ?? 0) == (CurrentScreen?.x ?? 0) + deltaX &&
                    (s.y ?? 0) == (CurrentScreen?.y ?? 0) + deltaY);

                if (newScreen != null)
                {
                    CurrentScreen = newScreen;
                    Locator.GetPlayerAudioController().PlayEnterLaunchCodes();
                    previousRotation = cam._cameraRotation;
                    if (CurrentScreen.type == ScreenType.ProbeGame && CurrentScreen.probeGame != null)
                    {
                        CurrentGame = new ProbeGame(this, CurrentScreen.probeGame);
                        CurrentGame.Start();
                    }
                    else
                    {
                        CurrentGame = null;
                    }
                }
                else
                {
                    Locator.GetPlayerAudioController().PlayNegativeUISound();
                    if (deltaX != 0) cam.RotateHorizontal(-deltaRotation.x);
                    if (deltaY != 0) cam.RotateVertical(-deltaRotation.y);
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

        private void ProbePort_OnAnchorProbe()
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
                if (InterfaceCanvas)
                {
                    InterfaceCanvas.enabled = true;
                    InterfaceCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    InterfaceCanvas.worldCamera = probe.GetRotatingCamera().GetOWCamera().mainCamera;
                }
                probe.GetRotatingCamera().ResetRotation();
                previousRotation = probe.GetRotatingCamera()._cameraRotation;
                CurrentScreen = Screens.Find(s => (s.x ?? 0) == 0 && (s.y ?? 0) == 0);
                UpdateScreen();
            }
        }

        private void ProbePort_OnUnanchorProbe()
        {
            if (InterfaceCanvas)
            {
                InterfaceCanvas.worldCamera = null;
                InterfaceCanvas.enabled = false;
            }
            CurrentScreen = null;
            CurrentGame = null;
        }

        private void UpdateScreen()
        {
            var probe = Locator.GetProbe();
            var cam = probe?.GetRotatingCamera();
            if (cam) cam.TakeSnapshot();
        }

        protected void LateUpdate()
        {
            if (InterfaceCanvas) InterfaceCanvas.enabled = IsProbeInPort();
            foreach (var sr in ProbeGameGrid) sr.enabled = IsProbeInPort() && CurrentGame != null;
            if (CurrentGame != null && !CurrentGame.GameOver)
            {
                InterfaceText.text = "";
                InterfaceMarkerLeft.enabled = true;
                InterfaceMarkerRight.enabled = true;
                InterfaceMarkerUp.enabled = true;
                InterfaceMarkerDown.enabled = true;
            }
            else if (CurrentScreen != null)
            {
                if (InterfaceText)
                {
                    if (CurrentGame != null && CurrentGame.GameOver)
                    {
                        InterfaceText.text = CurrentGame.Win ? "SUCCESS" : "FAILURE";
                    }
                    else
                    {
                        InterfaceText.text = Mod.ProbePorts.GetScreenText(CurrentScreen.type);
                    }
                }
                if (InterfaceMarkerLeft) InterfaceMarkerLeft.enabled = ScreenToLeft != null;
                if (InterfaceMarkerRight) InterfaceMarkerRight.enabled = ScreenToRight != null;
                if (InterfaceMarkerUp) InterfaceMarkerUp.enabled = ScreenAbove != null;
                if (InterfaceMarkerDown) InterfaceMarkerDown.enabled = ScreenBelow != null;
            }
        }

        public enum ScreenType
        {
            Unknown = 0,
            PlayerStatus = 1,
            SignalTowers = 2,
            Temperature = 3,
            ProbeGame = 4,
        }

        public class ProbeGame
        {
            public const int GRID_WIDTH = 11;
            public const int GRID_HEIGHT = GRID_WIDTH;
            public const int GRID_LENGTH = GRID_WIDTH * GRID_HEIGHT;

            public ProbePort Port;
            public List<Entity> Entities = new List<Entity>();
            public bool GameOver = false;
            public bool Win = false;

            public ProbeGame(ProbePort port, ProbePortConfig.ProbeGameScreenConfig config)
            {
                Port = port;
                foreach (var entity in config.entities)
                {
                    Entities.Add(new Entity()
                    {
                        X = entity.x,
                        Y = entity.y,
                        W = entity.w,
                        H = entity.h,
                        Type = entity.type,
                    });
                }
            }

            public void Start()
            {
                Port.audioSource.PlayOneShot(AudioType.SingularityOnObjectExit);
                Render();
            }

            public void End(bool win)
            {
                Entities.RemoveAll(e => e.Type == EntityType.Probe);                    
                GameOver = true;
                Win = win;
                Render();
            }

            public bool CheckForEnd()
            {
                foreach (var probe in Entities.Where(e => e.Type == EntityType.Probe).ToList())
                {
                    if (IsBlackHole(probe.X, probe.Y))
                    {
                        Port.audioSource.PlayOneShot(AudioType.SingularityOnObjectEnter);
                        End(true);
                        return true;
                    }
                    if (IsHazard(probe.X, probe.Y))
                    {
                        Port.audioSource.PlayOneShot(AudioType.ToolProbeRetrieve);
                        End(false);
                        return true;
                    }
                }
                return false;
            }

            public void Tick(int dx, int dy)
            {
                // Active Phase
                foreach (var entity in Entities.Where(e => e.Type != EntityType.Probe && e.Active).ToList())
                {
                    var probe = ClosestProbe(entity.X, entity.Y);
                    switch (entity.Type)
                    {
                        case EntityType.Angler:
                            if (probe != null)
                            {
                                var ty = probe.Y - entity.Y;
                                var tx = probe.X - entity.X;
                                var moveX = false;
                                if (Mathf.Abs(tx) > Mathf.Abs(ty)) moveX = true;
                                else if (Mathf.Abs(ty) > Mathf.Abs(tx)) moveX = false;
                                else moveX = UnityEngine.Random.value > 0.5f;
                                if (moveX)
                                {
                                    entity.X += (int)Mathf.Sign(tx);
                                }
                                else
                                {
                                    entity.Y += (int)Mathf.Sign(ty);
                                }
                                entity.FlipX = tx > 0;
                            }
                            break;
                        case EntityType.Stranger:
                            if (probe != null)
                            {
                                var ty = probe.Y - entity.Y;
                                var tx = probe.X - entity.X;
                                var moveX = false;
                                if (Mathf.Abs(tx) > Mathf.Abs(ty)) moveX = true;
                                else if (Mathf.Abs(ty) > Mathf.Abs(tx)) moveX = false;
                                else moveX = UnityEngine.Random.value > 0.5f;
                                if (moveX)
                                {
                                    entity.X += (int)Mathf.Sign(tx);
                                }
                                else
                                {
                                    entity.Y += (int)Mathf.Sign(ty);
                                }
                                if (probe.Y < entity.Y)
                                {
                                    entity.Type = EntityType.StrangerBack;
                                }
                            }
                            break;
                        case EntityType.StrangerBack:
                            if (probe != null)
                            {
                                var ty = probe.Y - entity.Y;
                                var tx = probe.X - entity.X;
                                var moveX = false;
                                if (Mathf.Abs(tx) > Mathf.Abs(ty)) moveX = true;
                                else if (Mathf.Abs(ty) > Mathf.Abs(tx)) moveX = false;
                                else moveX = UnityEngine.Random.value > 0.5f;
                                if (moveX)
                                {
                                    entity.X += (int)Mathf.Sign(tx);
                                }
                                else
                                {
                                    entity.Y += (int)Mathf.Sign(ty);
                                }
                                if (probe.Y > entity.Y)
                                {
                                    entity.Type = EntityType.Stranger;
                                }
                            }
                            break;
                    }
                }
                if (CheckForEnd()) return;
                // Probe Phase
                foreach (var probe in Entities.Where(e => e.Type == EntityType.Probe).ToList())
                {
                    probe.X += dx;
                    probe.Y += dy;
                    if (IsWalkable(probe.X, probe.Y))
                    {
                        Port.audioSource.PlayOneShot(AudioType.DialogueAdvance);
                    }
                    else if (IsWall(probe.X, probe.Y))
                    {
                        probe.X -= dx;
                        probe.Y -= dy;
                        Port.audioSource.PlayOneShot(AudioType.ImpactLowSpeed);
                    }
                }
                if (CheckForEnd()) return;
                // Inactive Phase
                foreach (var entity in Entities.Where(e => e.Type != EntityType.Probe && !e.Active).ToList())
                {
                    var probe = ClosestProbe(entity.X, entity.Y);
                    switch (entity.Type)
                    {
                        case EntityType.Angler:
                            if (probe != null)
                            {
                                if (DistanceBetween(entity.X, entity.Y, probe.X, probe.Y) <= 3)
                                {
                                    entity.Active = true;
                                    Port.audioSource.PlayOneShot(AudioType.DBAnglerfishDetectTarget);
                                }
                            }
                            break;
                        case EntityType.Stranger:
                            if (probe != null && probe.X == entity.X && probe.Y >= entity.Y && probe.Y < entity.Y + 5)
                            {
                                entity.Active = true;
                                Port.audioSource.PlayOneShot(AudioType.Ghost_Chase);
                            }
                            else if (IsWalkable(entity.X, entity.Y + 1))
                            {
                                entity.Y++;
                            }
                            else
                            {
                                entity.Type = EntityType.StrangerBack;
                            }
                            break;
                        case EntityType.StrangerBack:
                            if (probe != null && probe.X == entity.X && probe.Y <= entity.Y && probe.Y > entity.Y - 5)
                            {
                                entity.Active = true;
                                Port.audioSource.PlayOneShot(AudioType.Ghost_Chase);
                            }
                            else if (IsWalkable(entity.X, entity.Y - 1))
                            {
                                entity.Y--;
                            }
                            else
                            {
                                entity.Type = EntityType.Stranger;
                            }
                            break;
                        case EntityType.VineX:
                            if (IsWalkable(entity.X - 1, entity.Y))
                            {
                                entity.X--;
                                entity.W++;
                            }
                            if (IsWalkable(entity.X + 1, entity.Y))
                            {
                                entity.W++;
                            }
                            break;
                        case EntityType.VineY:
                            if (IsWalkable(entity.X, entity.Y - 1))
                            {
                                entity.Y--;
                                entity.H++;
                            }
                            if (IsWalkable(entity.X, entity.Y + 1))
                            {
                                entity.H++;
                            }
                            break;
                        case EntityType.Crystal:
                            if (probe != null && DistanceBetween(entity.X, entity.Y, probe.X, probe.Y) <= 1)
                            {
                                entity.Type = EntityType.GhostMatter;
                                Port.audioSource.PlayOneShot(AudioType.Splash_GhostMatter, 0.5f);
                            }
                            break;
                        case EntityType.GhostMatter:
                            if (probe == null || DistanceBetween(entity.X, entity.Y, probe.X, probe.Y) > 1)
                            {
                                entity.Type = EntityType.Crystal;
                            }
                            break;
                    }
                }
                if (CheckForEnd()) return;

                Render();
            }

            public void Render()
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    for (int y = 0; y < GRID_HEIGHT; y++)
                    {
                        int i = x + y * GRID_WIDTH;
                        var ent = Entities.Where(e => e.Contains(x, y)).LastOrDefault();
                        Port.ProbeGameGrid[i].sprite = Port.ProbeGameSprites[(int)(ent?.Type ?? EntityType.Unknown)];
                        Port.ProbeGameGrid[i].flipX = ent?.FlipX ?? false;
                        Port.ProbeGameGrid[i].flipY = ent?.FlipY ?? false;
                    }
                }
                Port.UpdateScreen();
            }

            public Entity ClosestProbe(int x, int y)
                => Entities.Where(e => e.Type == EntityType.Probe).OrderBy(e => DistanceBetween(x, y, e.X, e.Y)).FirstOrDefault();

            public int DistanceBetween(int x0, int y0, int x1, int y1)
                => Mathf.Abs(x1 - x0) + Mathf.Abs(y0 - y1);

            public bool IsHazard(EntityType type)
                => type >= EntityType.Angler && type <= EntityType.GhostMatter;

            public bool IsWall(EntityType type)
                => type >= EntityType.WallFull && type <= EntityType.WallSpiral;

            public bool IsWalkable(int x, int y)
                => !Entities.Where(e => e.Type != EntityType.Probe && e.Type != EntityType.Crystal && e.Type != EntityType.GhostMatter).Any(e => e.Contains(x, y));

            public bool IsHazard(int x, int y)
                => Entities.Any(e => e.Contains(x, y) && IsHazard(e.Type));

            public bool IsWall(int x, int y)
                => Entities.Any(e => e.Contains(x, y) && IsWall(e.Type));

            public bool IsBlackHole(int x, int y)
                => Entities.Any(e => e.Contains(x, y) && e.Type == EntityType.BlackHole);

            public enum EntityType
            {
                Unknown = 0,
                Probe = 1,
                WhiteHole = 2,
                BlackHole = 3,
                Angler = 4,
                Stranger = 5,
                StrangerBack = 6,
                Seed = 7,
                VineX = 8,
                VineY = 9,
                Crystal = 10,
                GhostMatter = 11,
                WallFull = 12,
                WallHollow = 13,
                WallSpiral = 14,
                Nomai = 15,
            }

            public class Entity
            {
                public int X = 0;
                public int Y = 0;
                public int W = 1;
                public int H = 1;
                public EntityType Type = EntityType.Unknown;
                public bool Active = false;
                public bool FlipX = false;
                public bool FlipY = false;

                public bool Overlaps(int x, int y, int w, int h)
                    => !(X + W < x || Y + H < y || X > x + w || Y > y + h);

                public bool Contains(int x, int y)
                    => x >= X && y >= Y && x < X + W && y < Y + H;
            }
        }
    }
}
