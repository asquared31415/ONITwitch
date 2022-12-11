using JetBrains.Annotations;

namespace ONITwitchLib.Utils;

/// <summary>
/// Utilities for finding and working with elements.
/// </summary>
public static class ElementUtil
{
	/// <summary>
	/// Finds an element by its string ID without going through Enum.Parse.
	/// </summary>
	/// <param name="name">The ID of the element to find</param>
	/// <returns>The <see cref="Element"/> if it exists, or <c>null</c> otherwise.</returns>
	[CanBeNull]
	public static Element FindElementByNameFast(string name)
	{
		var element = ElementLoader.FindElementByHash((SimHashes) Hash.SDBMLower(name));
		return element;
	}

	/// <summary>
	/// Determines whether an element exists and is enabled for the current DLC, if applicable.
	/// </summary>
	/// <param name="name">The ID of the element to find</param>
	/// <returns><c>true</c> if this element does exist and is currently enabled, <c>false</c> otherwise.</returns>
	public static bool ElementExistsAndEnabled(string name)
	{
		var element = FindElementByNameFast(name);
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}

	/// <summary>
	/// Determines whether an element exists and is enabled for the current DLC, if applicable.
	/// </summary>
	/// <param name="hash">The ID of the element to find</param>
	/// <returns><c>true</c> if this element does exist and is currently enabled, <c>false</c> otherwise.</returns>
	public static bool ElementExistsAndEnabled(SimHashes hash)
	{
		var element = ElementLoader.FindElementByHash(hash);
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}

	/// <summary>
	/// Determines whether an element exists and is enabled for the current DLC, if applicable.
	/// </summary>
	/// <param name="element">The element to find</param>
	/// <returns><c>true</c> if this element does exist and is currently enabled, <c>false</c> otherwise.</returns>
	[ContractAnnotation("element:null => false")]
	public static bool ElementExistsAndEnabled(Element element)
	{
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}
}
