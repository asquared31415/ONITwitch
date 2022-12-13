using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchCore.Elements;
using ONITwitchLib;
using UnityEngine;

namespace ONITwitchCore.Patches;

public static class ElementPatches
{
	[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
	public static class ElementLoader_Load_Patch
	{
		private static readonly AccessTools.FieldRef<object, Tag> SubstanceNameTag =
			AccessTools.FieldRefAccess<Tag>(typeof(Substance), "nameTag");

		[UsedImplicitly]
		public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
		{
			var trav = Traverse.Create(substanceTablesByDlc[DlcManager.VANILLA_ID]);
			var substanceList = trav.Field<List<Substance>>("list").Value;

			var animFile = Assets.Anims.Find(a => a.name == "TI_IndestructibleElement_kanim");
			var material = new Material(Assets.instance.substanceTable.solidMaterial);
			var tex = new Texture2D(2, 2);
			var bytes = File.ReadAllBytes(Path.Combine(TwitchModInfo.MainModFolder, "assets", "neutronium.png"));
			tex.LoadImage(bytes);
			material.mainTexture = tex;

			Debug.Log(Hash.SDBMLower(nameof(TwitchSimHashes.IndestructibleElement)));
			var substance = ModUtil.CreateSubstance(
				nameof(TwitchSimHashes.IndestructibleElement),
				Element.State.Solid,
				animFile,
				material,
				Color.black,
				Color.black,
				Color.black
			);
			Debug.Log(Traverse.Create(substance).Field<Tag>("nameTag").Value);
			substanceList.Add(substance);
		}

		[UsedImplicitly]
		public static void Postfix()
		{
			var unbreakable = ElementLoader.FindElementByHash(TwitchSimHashes.IndestructibleElement);
			SubstanceNameTag(unbreakable.substance) = TagManager.Create(nameof(TwitchSimHashes.IndestructibleElement));
		}
	}
}
