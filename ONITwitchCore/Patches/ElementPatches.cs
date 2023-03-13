using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.CustomElements;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitchCore.Patches;

internal static class ElementPatches
{
	[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
	private static class ElementLoader_Load_Patch
	{
		private static readonly AccessTools.FieldRef<object, Tag> SubstanceNameTag =
			AccessTools.FieldRefAccess<Tag>(typeof(Substance), "nameTag");

		[UsedImplicitly]
		private static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
		{
			var trav = Traverse.Create(substanceTablesByDlc[DlcManager.VANILLA_ID]);
			var substanceList = trav.Field<List<Substance>>("list").Value;

			var animFile = Assets.Anims.Find(a => a.name == "TI_IndestructibleElement_kanim");
			var material = new Material(Assets.instance.substanceTable.solidMaterial);
			var tex = new Texture2D(2, 2);
			var bytes = File.ReadAllBytes(Path.Combine(TwitchModInfo.MainModFolder, "assets", "neutronium.png"));
			tex.LoadImage(bytes);
			material.mainTexture = tex;

			var substance = ModUtil.CreateSubstance(
				nameof(TwitchSimHashes.IndestructibleElement),
				Element.State.Solid,
				animFile,
				material,
				Color.black,
				Color.black,
				Color.black
			);
			substanceList.Add(substance);
		}

		[UsedImplicitly]
		private static void Postfix()
		{
			var unbreakable = ElementLoader.FindElementByHash(TwitchSimHashes.IndestructibleElement);
			SubstanceNameTag(unbreakable.substance) = TagManager.Create(nameof(TwitchSimHashes.IndestructibleElement));
		}
	}
}
