namespace ONITwitch.Content;

internal static class DbEx
{
	internal static ExtraStatusItems ExtraStatusItems;

	internal static void Initialize(ResourceSet root)
	{
		ExtraStatusItems = new ExtraStatusItems(root);
	}
}
