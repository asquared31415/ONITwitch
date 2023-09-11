using System.IO;
using JetBrains.Annotations;
using ONITwitchLib;
using ONITwitchLib.Utils;
using UnityEngine;

namespace ONITwitch.Content.Elements;

internal record ElementInfo([NotNull] string Id, [NotNull] string Anim, Color Color, Element.State State)
{
	/// <summary>
	///     The <c>SimHashes</c> for this element, derived from the Id passed.
	/// </summary>
	public readonly SimHashes Hash = (SimHashes) global::Hash.SDBMLower(Id);

	[NotNull]
	public Substance CreateSubstance()
	{
		var material = new Material(
			State.IsSolid()
				? Assets.instance.substanceTable.solidMaterial
				: Assets.instance.substanceTable.liquidMaterial
		);

		var tex = new Texture2D(2, 2);
		var bytes = File.ReadAllBytes(
			Path.Combine(TwitchModInfo.MainModFolder, "assets", "textures", Id.ToLowerInvariant() + ".png")
		);
		tex.LoadImage(bytes);
		material.mainTexture = tex;

		var kanim = Assets.Anims.Find(anim => anim.name == Anim);

		var substance = ModUtil.CreateSubstance(Id, State, kanim, material, Color, Color, Color);
		return substance;
	}
}
