using JetBrains.Annotations;

namespace ONITwitchLib;

public static class ElementUtil
{
	[CanBeNull]
	public static Element FindElementByNameFast(string name)
	{
		var element = ElementLoader.FindElementByHash((SimHashes) Hash.SDBMLower(name));
		return element;
	}

	public static bool ElementExistsAndEnabled(string name)
	{
		var element = FindElementByNameFast(name);
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}

	public static bool ElementExistsAndEnabled(SimHashes hash)
	{
		var element = ElementLoader.FindElementByHash(hash);
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}

	[ContractAnnotation("element:null => false")]
	public static bool ElementExistsAndEnabled(Element element)
	{
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}
}
