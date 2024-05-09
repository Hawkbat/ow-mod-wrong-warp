using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WrongWarp.Utils;

namespace WrongWarp.Components
{
    public class ProbePortScreenGame : ProbePortScreen, ISerializationCallbackReceiver
    {
        public GameConfig Config;
        public ProbeGame CurrentGame;

        public string configJson;

        public override void WireUp()
        {
            base.WireUp();
            if (!string.IsNullOrEmpty(configJson))
                Config = JsonUtils.FromJson<GameConfig>(configJson);
        }

        public override string GetText()
        {
            if (CurrentGame.GameOver) return CurrentGame.Win ? "SUCCESS" : "FAILURE";
            return string.Empty;
        }

        public override bool CanNavigate() => CurrentGame.GameOver;

        public override void OnEnter()
        {
            base.OnEnter();
            CurrentGame = new ProbeGame(Port, this);
            CurrentGame.Start();
        }

        public override void OnExit()
        {
            base.OnExit();
            CurrentGame.End(false);
            CurrentGame = null;
        }

        public override bool OnTick(int dx, int dy)
        {
            if (CurrentGame == null || CurrentGame.GameOver) return false;
            CurrentGame.Tick(dx, dy);
            return true;
        }

        public void OnBeforeSerialize()
        {
            if (Config != null && Config.Entities != null && Config.Entities.Length > 0)
                configJson = JsonUtility.ToJson(Config);
        }

        public void OnAfterDeserialize()
        {
            if (Config != null && Config.Entities != null && Config.Entities.Length > 0)
                configJson = JsonUtility.ToJson(Config);
        }

        [Serializable]
        public class GameConfig
        {
            public EntityConfig[] Entities;
        }

        [Serializable]
        public class EntityConfig
        {
            public int x = 0;
            public int y = 0;
            public int w = 1;
            public int h = 1;
            public EntityType type = EntityType.Unknown;
        }

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

        public class ProbeGame
        {
            public const int GRID_WIDTH = 11;
            public const int GRID_HEIGHT = GRID_WIDTH;
            public const int GRID_LENGTH = GRID_WIDTH * GRID_HEIGHT;

            public ProbePort Port;
            public List<Entity> Entities = new List<Entity>();
            public bool GameOver = false;
            public bool Win = false;

            public ProbeGame(ProbePort port, ProbePortScreenGame config)
            {
                Port = port;
                foreach (var entity in config.Config.Entities)
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
                Port.AudioSource.PlayOneShot(AudioType.SingularityOnObjectExit);
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
                        Port.AudioSource.PlayOneShot(AudioType.SingularityOnObjectEnter);
                        End(true);
                        return true;
                    }
                    if (IsHazard(probe.X, probe.Y))
                    {
                        Port.AudioSource.PlayOneShot(AudioType.ToolProbeRetrieve);
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
                        Port.AudioSource.PlayOneShot(AudioType.DialogueAdvance);
                    }
                    else if (IsWall(probe.X, probe.Y))
                    {
                        probe.X -= dx;
                        probe.Y -= dy;
                        Port.AudioSource.PlayOneShot(AudioType.ImpactLowSpeed);
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
                                    Port.AudioSource.PlayOneShot(AudioType.DBAnglerfishDetectTarget);
                                }
                            }
                            break;
                        case EntityType.Stranger:
                            if (probe != null && probe.X == entity.X && probe.Y >= entity.Y && probe.Y < entity.Y + 5)
                            {
                                entity.Active = true;
                                Port.AudioSource.PlayOneShot(AudioType.Ghost_Chase);
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
                                Port.AudioSource.PlayOneShot(AudioType.Ghost_Chase);
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
                                Port.AudioSource.PlayOneShot(AudioType.Splash_GhostMatter, 0.5f);
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
                => Entities.Any(e => e.Type == EntityType.BlackHole && e.Contains(x, y))
                || !Entities.Where(e => e.Type != EntityType.Probe && e.Type != EntityType.Crystal && e.Type != EntityType.GhostMatter).Any(e => e.Contains(x, y));

            public bool IsHazard(int x, int y)
                => Entities.Any(e => e.Contains(x, y) && IsHazard(e.Type));

            public bool IsWall(int x, int y)
                => Entities.Any(e => e.Contains(x, y) && IsWall(e.Type));

            public bool IsBlackHole(int x, int y)
                => Entities.Any(e => e.Contains(x, y) && e.Type == EntityType.BlackHole);
        }
    }
}
