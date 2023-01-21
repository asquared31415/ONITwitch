// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using STRINGS;

#pragma warning disable CS1591
namespace ONITwitchCore;

public static class TIStrings
{
	public static class STRINGS
	{
		public static class BUILDINGS
		{
			public static class PREFABS
			{
				public static class ONITWITCH
				{
					public static class POCKETDIMENSIONEXTERIORPORTALCONFIG
					{
						public static readonly LocString NAME = "Pocket Dimension Portal";
						public static readonly LocString DESC = "TODO";
						public static readonly LocString EFFECT = "A portal to an unknown dimension.";
					}

					public static class POCKETDIMENSIONINTERIORPORTALCONFIG
					{
						public static readonly LocString NAME = "Return Portal";
						public static readonly LocString DESC = "TODO";

						public static readonly LocString EFFECT
							= "A portal that should hopefully return your duplicants back where they came from.";
					}
				}
			}
		}

		public static class CREATURES
		{
			public static class SPECIES
			{
				public static class GLITTERPUFT
				{
					public static readonly LocString NAME = "Glitter Puft";

					public static readonly LocString DESC = "A extraordinarily colorful and very pretty Puft";
				}
			}
		}

		public static class ELEMENTS
		{
			public static class INDESTRUCTIBLEELEMENT
			{
				public static readonly LocString NAME = UI.FormatAsLink(
					"Neutrollium",
					nameof(global::STRINGS.ELEMENTS.UNOBTANIUM)
				);

				public static readonly LocString DESC = global::STRINGS.ELEMENTS.UNOBTANIUM.DESC;
			}
		}

		public static class ITEMS
		{
			public static class ONITWITCH
			{
				public static class SURPRISEBOXCONFIG
				{
					public static readonly LocString NAME = "Surprise Box";
					public static readonly LocString EFFECT = "Deconstruct for a surprise!";
					public static readonly LocString DESC = "";
				}
			}

			public static class FOOD
			{
				public static class ONITWITCH
				{
					public static class GLITTERMEATCONFIG
					{
						public static readonly LocString NAME = "Glitter Meat";

						public static readonly LocString DESC =
							"A piece of meat that glitters faintly.\n\n...It's best not to think about where it came from...";
					}
				}
			}
		}
	}
}
