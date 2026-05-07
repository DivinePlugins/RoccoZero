using System;
using System.Linq;

using Divine.Core.Entities;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Game;
using Divine.GameConsole;
using Divine.Numerics;
using Divine.Renderer;

namespace Divine.Core.Helpers
{
    public static class HUDInfo
    {
        internal static float CompareWidth { get; set; }

        internal static float PanelHeroSizeX { get; set; }

        internal static float RadiantCompare { get; set; }

        internal static float DireCompare { get; set; }

        internal static float HpBarHeight { get; set; }

        internal static float HpBarWidth { get; set; }

        internal static float HpBarX { get; set; }

        internal static float HpBarY { get; set; }

        internal static Vector2 MinimapSize { get; set; }

        public static float RatioPercentage { get; internal set; }

        public static float HpBarSizeX { get; internal set; }

        public static float HpBarSizeY { get; internal set; }

        public static float TopPanelSizeX { get; internal set; }

        public static float TopPanelSizeY { get; internal set; }

        public static Vector2 ScreenSize { get; } = RendererManager.ScreenSize;

        private static float MapBottom { get; } = -7521;

        internal static float MapLeft { get; } = -8068;

        internal static float MapRight { get; } = 7933;

        internal static float MapTop { get; } = 7679;

        internal static float MapHeight { get; } = Math.Abs(MapBottom - MapTop);

        internal static float MapWidth { get; } = Math.Abs(MapLeft - MapRight);

        internal static float MinimapMapScaleX { get; set; }

        internal static float MinimapMapScaleY { get; set; }

        internal static bool MinimapIsOnRight { get; set; }

        internal static bool IsExtraLarge { get; } = GameConsoleManager.GetInt32("dota_hud_extra_large_minimap") == 1;

        private static Vector2[] UltraHD4x3 { get; } =
        {
            new Vector2(2560, 2048),
            new Vector2(2560, 1920),
            new Vector2(2304, 1728),
            new Vector2(2048, 1536),
            new Vector2(1920, 1440),
            new Vector2(1600, 1200)
        };

        private static Vector2[] UltraHD16x10 { get; } =
        {
            new Vector2(3360, 2100),
            new Vector2(2880, 1800),
            new Vector2(2560, 1600),
            new Vector2(1920, 1200),
        };

        private static Vector2[] UltraHD16x9 { get; } =
        {
            new Vector2(3840, 2160),
            new Vector2(3200, 1800),
            new Vector2(2732, 1536),
            new Vector2(2560, 1440)
        };

        static HUDInfo()
        {
            Action();
        }

        public static void Action()
        {
            var ratio = Math.Floor(ScreenSize.X / ScreenSize.Y * 100);
            if (ScreenSize == new Vector2(1366, 768)) // 177
            {
                CompareWidth = 1649.14f;
                PanelHeroSizeX = 53.9656f;

                RadiantCompare = 3.396777f;
                DireCompare = 2.49442f;

                HpBarHeight = 12.939f;
                HpBarWidth = 88.27f;
                HpBarX = 44.806f;
                HpBarY = 28.369f;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(1600, 900)) // 177
            {
                CompareWidth = 1704.495f;
                PanelHeroSizeX = 55.5374f;

                RadiantCompare = 3.396357f;
                DireCompare = 2.49285f;

                HpBarHeight = 12.066f;
                HpBarWidth = 92.398f;
                HpBarX = 47.13f;
                HpBarY = 30.706f;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(1680, 1050)) // 160
            {
                CompareWidth = 1479.14f;
                PanelHeroSizeX = 53.3262f;

                RadiantCompare = 3.670904f;
                DireCompare = 2.552618f;

                HpBarHeight = 11.203f;
                HpBarWidth = 87.433f;
                HpBarX = 44.331f;
                HpBarY = 28.369f;

                MinimapSize = new Vector2(0.142583f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(1768, 992)) // 178
            {
                CompareWidth = 1672;
                PanelHeroSizeX = 54.83002f;

                RadiantCompare = 3.39914f;
                DireCompare = 2.501999f;

                HpBarHeight = 10.70002f;
                HpBarWidth = 85.8796f;
                HpBarX = 44.06002f;
                HpBarY = 28;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(1920, 1080)) // 177
            {
                CompareWidth = 1639.955f;
                PanelHeroSizeX = 53.5554f;

                RadiantCompare = 3.389952f;
                DireCompare = 2.491662f;

                HpBarHeight = 12.26805f;
                HpBarWidth = 88.675f;
                HpBarX = 45.17f;
                HpBarY = 28.547f;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(3440, 1440)) //New
            {
                CompareWidth = 2178.415f;
                PanelHeroSizeX = 52.7434f;

                RadiantCompare = 2.876922f;
                DireCompare = 2.337606f;

                HpBarHeight = 11.837f;
                HpBarWidth = 85.185f;
                HpBarX = 44.529f;
                HpBarY = 25.84f;

                MinimapSize = new Vector2(0.094999f * ScreenSize.X, 0.224629f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(2560, 1440)) //New
            {
                CompareWidth = 1674.3f;
                PanelHeroSizeX = 54.4996f;

                RadiantCompare = 3.394915f;
                DireCompare = 2.481099f;

                HpBarHeight = 14.878f;
                HpBarWidth = 86.793f;
                HpBarX = 44.459f;
                HpBarY = 29.019f;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ScreenSize == new Vector2(2560, 1080)) //New
            {
                CompareWidth = 2222;
                PanelHeroSizeX = 54.83002f;

                RadiantCompare = 2.904136f;
                DireCompare = 2.351998f;

                HpBarHeight = 8.599968f;
                HpBarWidth = 90.99874f;
                HpBarX = 46.78009f;
                HpBarY = 28;

                MinimapSize = new Vector2(0.092979f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (UltraHD4x3.Any(x => x == ScreenSize)) // 133
            {
                CompareWidth = 1236.22f;
                PanelHeroSizeX = 53.7694f;

                RadiantCompare = 4.42627f;
                DireCompare = 2.711166f;

                HpBarHeight = 12.301f;
                HpBarWidth = 88.65401f;
                HpBarX = 45.078f;
                HpBarY = 28.392f;

                MinimapSize = new Vector2(0.169083f * ScreenSize.X, 0.222574f * ScreenSize.Y);
            }
            else if (UltraHD16x10.Any(x => x == ScreenSize)) // 160
            {
                CompareWidth = 1507.11f;
                PanelHeroSizeX = 54.088f;

                RadiantCompare = 3.672969f;
                DireCompare = 2.537547f;

                HpBarHeight = 10.974f;
                HpBarWidth = 88.264f;
                HpBarX = 45.078f;
                HpBarY = 28.392f;

                MinimapSize = new Vector2(0.142583f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ratio == 156) // 156 1600x1024
            {
                CompareWidth = 1507.11f;
                PanelHeroSizeX = 56.248f;

                RadiantCompare = 3.78097f;
                DireCompare = 2.582547f;

                HpBarHeight = 10.974f;
                HpBarWidth = 88.264f;
                HpBarX = 45.078f;
                HpBarY = 28.392f;

                MinimapSize = new Vector2(0.142583f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (UltraHD16x9.Any(x => x == ScreenSize)) // 177
            {
                CompareWidth = 1600;
                PanelHeroSizeX = 52.35f;

                RadiantCompare = 3.39f;
                DireCompare = 2.493f;

                HpBarHeight = 10;
                HpBarWidth = 86.5f;
                HpBarX = 44;
                HpBarY = 27;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ratio == 213)
            {
                CompareWidth = 1600;
                PanelHeroSizeX = 45.28f;

                RadiantCompare = 3.08f;
                DireCompare = 2.402f;

                HpBarHeight = 7;
                HpBarWidth = 69;
                HpBarX = 36;
                HpBarY = 23;

                MinimapSize = new Vector2(0.107083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ratio == 177)
            {
                CompareWidth = 1600;
                PanelHeroSizeX = 52.35f;

                RadiantCompare = 3.39f;
                DireCompare = 2.493f;

                HpBarHeight = 10;
                HpBarWidth = 86.5f;
                HpBarX = 44;
                HpBarY = 27;

                MinimapSize = new Vector2(0.128083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ratio == 166)
            {
                CompareWidth = 1280;
                PanelHeroSizeX = 47.19f;

                RadiantCompare = 3.64f;
                DireCompare = 2.59f;

                HpBarHeight = 7.4f;
                HpBarWidth = 71;
                HpBarX = 37;
                HpBarY = 22;

                MinimapSize = new Vector2(0.137083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ratio == 160)
            {
                CompareWidth = 1280;
                PanelHeroSizeX = 48.95f;

                RadiantCompare = 3.78f;
                DireCompare = 2.609f;

                HpBarHeight = 9;
                HpBarWidth = 75;
                HpBarX = 38.3f;
                HpBarY = 25;

                MinimapSize = new Vector2(0.142583f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else if (ratio == 150)
            {
                CompareWidth = 1024;
                PanelHeroSizeX = 47.21f;

                RadiantCompare = 4.57f;
                DireCompare = 2.775f;

                HpBarHeight = 8;
                HpBarWidth = 79.2f;
                HpBarX = 40.2f;
                HpBarY = 24;

                MinimapSize = new Vector2(0.150023f * ScreenSize.X, 0.220094f * ScreenSize.Y);
            }
            else if (ratio == 133)
            {
                CompareWidth = 1280;
                PanelHeroSizeX = 58.3f;

                RadiantCompare = 4.65f;
                DireCompare = 2.78f;

                HpBarHeight = 8;
                HpBarWidth = 71;
                HpBarX = 36.6f;
                HpBarY = 23;

                MinimapSize = new Vector2(0.169083f * ScreenSize.X, 0.222574f * ScreenSize.Y);
            }
            else if (ratio == 125)
            {
                CompareWidth = 1280;
                PanelHeroSizeX = 58.3f;

                RadiantCompare = 4.65f;
                DireCompare = 2.78f;

                HpBarHeight = 11;
                HpBarWidth = 96.5f;
                HpBarX = 49;
                HpBarY = 32;

                MinimapSize = new Vector2(0.185083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }
            else
            {
                CompareWidth = 1600;
                PanelHeroSizeX = 65;

                RadiantCompare = 5.985f;
                DireCompare = 2.655f;

                HpBarHeight = 10;
                HpBarWidth = 83.5f;
                HpBarX = 43;
                HpBarY = 28;

                MinimapSize = new Vector2(0.127083f * ScreenSize.X, 0.224074f * ScreenSize.Y);
            }

            RatioPercentage = ScreenSize.X / CompareWidth;

            TopPanelSizeX = PanelHeroSizeX * RatioPercentage;
            TopPanelSizeY = 35 * RatioPercentage;

            HpBarSizeX = HpBarWidth * RatioPercentage;
            HpBarSizeY = HpBarHeight * RatioPercentage;

            if (IsExtraLarge)
            {
                MinimapSize *= 1.147083f;
            }

            MinimapMapScaleX = MinimapSize.X / MapWidth;
            MinimapMapScaleY = MinimapSize.Y / MapHeight;
        }

        public static Vector2 GetHpBarPosition(CUnit unit)
        {
            var pos = unit.Position + new Vector3(0, 0, unit.HealthBarOffset);
            var screenPos = RendererManager.WorldToScreen(pos, true);
            if (screenPos.IsZero)
            {
                return Vector2.Zero;
            }

            if (Equals(unit, UnitManager.Owner))
            {
                if (unit is CHero hero && hero.HeroId == HeroId.npc_dota_hero_meepo)
                {
                    return screenPos + new Vector2(-HpBarX * 1.05f * RatioPercentage, -HpBarY * 1.3f * RatioPercentage);
                }

                return screenPos + new Vector2(-HpBarX * RatioPercentage, -HpBarY * 1.15f * RatioPercentage);
            }

            if (unit.IsAlly())
            {
                return screenPos + new Vector2(-HpBarX * RatioPercentage, -HpBarY * 1.022f * RatioPercentage);
            }

            return screenPos + new Vector2(-HpBarX * RatioPercentage, -HpBarY * RatioPercentage);
        }

        public static Vector2 GetTopPanelPosition(CHero hero)
        {
            var playerid = 0;
            if (hero.Player != null)
            {
                playerid = hero.Player.Id;
            }

            return new Vector2(GetXX(hero.Team) - 20 * RatioPercentage + TopPanelSizeX * playerid, 0);
        }

        public static Vector2 GetCustomTopPanelPosition(int id, Team team)
        {
            return new Vector2(GetXX(team) - 20 * RatioPercentage + TopPanelSizeX * id, 0);
        }

        private static float GetXX(Team team)
        {
            if (team == Team.Radiant)
            {
                return ScreenSize.X / RadiantCompare + 1;
            }

            return ScreenSize.X / DireCompare + 1;
        }

        public static Vector2 MousePositionFromMinimap
        {
            get
            {
                var mouse = GameManager.MouseScreenPosition;

                var scaledX = mouse.X;
                var scaledY = ScreenSize.Y - mouse.Y;

                var x = scaledX / MinimapMapScaleX + MapLeft;
                var y = scaledY / MinimapMapScaleY + MapBottom;

                if (Math.Abs(x) > 7900 || Math.Abs(y) > 7200)
                {
                    return Vector2.Zero;
                }

                return new Vector2(x, y);
            }
        }

        public static Vector2 WorldToMinimap(this Vector3 mapPosition)
        {
            var scaledX = Math.Min(Math.Max((mapPosition.X - MapLeft) * MinimapMapScaleX, 0), MinimapSize.X);
            var scaledY = Math.Min(Math.Max((mapPosition.Y - MapBottom) * MinimapMapScaleY, 0), MinimapSize.Y);

            if (MinimapIsOnRight)
            {
                scaledX = (ScreenSize.X + scaledX) - MinimapSize.X;
            }

            return new Vector2((float)Math.Floor(scaledX), (float)Math.Floor(ScreenSize.Y - scaledY));
        }
    }
}
