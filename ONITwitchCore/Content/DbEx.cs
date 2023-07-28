namespace ONITwitch.Content;

public class DbEx
{
	public static ExtraStatusItems ExtraStatusItems;

	internal static void Initialize(ResourceSet root)
	{
		ExtraStatusItems = new ExtraStatusItems(root);
	}
}
