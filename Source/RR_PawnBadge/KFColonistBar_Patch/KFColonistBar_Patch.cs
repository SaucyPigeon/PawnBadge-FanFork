using ColonistBarKF.Bar;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace RR_PawnBadge
{
    [StaticConstructorOnStartup]
    public class KFColonistBar_Patch
    {
        static KFColonistBar_Patch()
        {
            var harmony = new Harmony("SaucyPigeon.PawnBadge.KFColonistBar_Patch");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ColonistBarColonistDrawer_KF), nameof(ColonistBarColonistDrawer_KF.DrawColonist))]
        private class RimWorld_ColonistBarColonistDrawer_DrawColonist
        {
            private const float ICON_WIDTH = 35f;

            private static void Postfix(Rect outerRect, Pawn colonist, Map pawnMap, bool highlight, bool reordering)
            {
                var cb = colonist.GetComp<CompBadge>();
                if (cb == null)
                {
                    return;
                }

                var iwidth = ICON_WIDTH;
                switch (Settings.badgeSize)
                {
                    case Settings.BadgeSize.Small:
                        iwidth = iwidth - 10f;
                        break;
                    case Settings.BadgeSize.Large:
                        iwidth = iwidth + 10f;
                        break;
                }

                var iwidth_half = iwidth / 2.0f;
                var ibottommargin = iwidth_half;

                // default position is Top, adjust starting from this
                if (cb.badges[0] != string.Empty)
                {
                    var brect = new Rect(outerRect.x - iwidth_half, outerRect.y - iwidth_half, iwidth, iwidth);
                    switch (Settings.badgePosition)
                    {
                        case Settings.BadgePosition.Bottom:
                            brect.y += outerRect.height - ibottommargin;
                            break;
                        case Settings.BadgePosition.Right:
                            brect.x += outerRect.width;
                            break;
                    }

                    if (TryGetBadgeDef(cb, 0, out var badgeDef))
                    {
                        GUI.DrawTexture(brect, badgeDef.Symbol, ScaleMode.ScaleToFit);
                    }
                }

                if (cb.badges[1] != string.Empty)
                {
                    var brect = new Rect(outerRect.xMax - iwidth_half, outerRect.y - iwidth_half, iwidth, iwidth);
                    switch (Settings.badgePosition)
                    {
                        case Settings.BadgePosition.Bottom:
                            brect.y += outerRect.height - ibottommargin;
                            break;
                        case Settings.BadgePosition.Left:
                            brect.x -= outerRect.width;
                            brect.y += outerRect.height - ibottommargin;
                            break;
                        case Settings.BadgePosition.Right:
                            brect.y += outerRect.height - ibottommargin;
                            break;
                    }

                    if (TryGetBadgeDef(cb, 1, out var badgeDef))
                    {
                        GUI.DrawTexture(brect, badgeDef.Symbol, ScaleMode.ScaleToFit);
                    }
                }
            }

            private static bool TryGetBadgeDef(CompBadge compBadge, int index, out BadgeDef badgeDef)
            {
                badgeDef = DefDatabase<BadgeDef>.GetNamedSilentFail(compBadge.badges[index]);
                if (badgeDef == null)
                {
                    Log.Warning($"Pawn Badge failed to find badge def with name \"{compBadge.badges[index]}\". Resetting badge to empty.");
                    compBadge.badges[index] = string.Empty;
                    return false;
                }

                return true;
            }
        }
    }
}