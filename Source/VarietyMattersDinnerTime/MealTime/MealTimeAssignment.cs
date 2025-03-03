﻿using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using HarmonyLib;

namespace VarietyMattersDT
{
	[HarmonyPatch]
    public class MealTimeAssignment
    {
        [HarmonyPatch(typeof(TimeAssignmentSelector), "DrawTimeAssignmentSelectorGrid")]
        public static void Postfix(Rect rect)
        {
			rect.yMax -= 2f;
			Rect rect2 = rect;
			rect2.xMax = rect2.center.x;
			rect2.yMax = rect2.center.y;
			rect2.x += (2 + ModSettings_VMDT.assignmentPos) * rect2.width;
			if (ModsConfig.RoyaltyActive)
				rect2.x += rect2.width;
            DrawTimeAssignmentSelectorFor(rect2, DefOf_VMDT.VMDT_Food);
        }

        public static void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
        {
			rect = rect.ContractedBy(2f);
			GUI.DrawTexture(rect, ta.ColorTexture);
			if (Widgets.ButtonInvisible(rect, true))
			{
				TimeAssignmentSelector.selectedAssignment = ta;
				SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
			}
			GUI.color = Color.white;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleCenter;
			GUI.color = Color.white;
			Widgets.Label(rect, ta.LabelCap);
			Text.Anchor = TextAnchor.UpperLeft;
			if (TimeAssignmentSelector.selectedAssignment == ta)
			{
				Widgets.DrawBox(rect, 2);
				return;
			}
			UIHighlighter.HighlightOpportunity(rect, ta.cachedHighlightNotSelectedTag);
		}
    }
}
