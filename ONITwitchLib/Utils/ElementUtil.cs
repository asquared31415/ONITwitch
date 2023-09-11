using JetBrains.Annotations;

namespace ONITwitchLib.Utils;

/// <summary>
///     Utilities for finding and working with elements.
/// </summary>
[PublicAPI]
public static class ElementUtil
{
	/// <summary>
	///     Finds an element by its string ID without going through Enum.Parse.
	/// </summary>
	/// <param name="name">The ID of the element to find</param>
	/// <returns>The <see cref="Element" /> if it exists, or <c>null</c> otherwise.</returns>
	[PublicAPI]
	[CanBeNull]
	public static Element FindElementByNameFast(string name)
	{
		var element = ElementLoader.FindElementByHash((SimHashes) Hash.SDBMLower(name));
		return element;
	}

	/// <summary>
	///     Determines whether an element exists and is enabled for the current DLC, if applicable.
	/// </summary>
	/// <param name="name">The ID of the element to find</param>
	/// <returns><c>true</c> if this element does exist and is currently enabled, <c>false</c> otherwise.</returns>
	[PublicAPI]
	public static bool ElementExistsAndEnabled(string name)
	{
		var element = FindElementByNameFast(name);
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}

	/// <summary>
	///     Determines whether an element exists and is enabled for the current DLC, if applicable.
	/// </summary>
	/// <param name="hash">The ID of the element to find</param>
	/// <returns><c>true</c> if this element does exist and is currently enabled, <c>false</c> otherwise.</returns>
	[PublicAPI]
	public static bool ElementExistsAndEnabled(SimHashes hash)
	{
		var element = ElementLoader.FindElementByHash(hash);
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}

	/// <summary>
	///     Determines whether an element exists and is enabled for the current DLC, if applicable.
	/// </summary>
	/// <param name="element">The element to find</param>
	/// <returns><c>true</c> if this element does exist and is currently enabled, <c>false</c> otherwise.</returns>
	[PublicAPI]
	[ContractAnnotation("element:null => false")]
	public static bool ElementExistsAndEnabled([CanBeNull] Element element)
	{
		return (element != null) && DlcManager.IsContentActive(element.dlcId) && !element.disabled;
	}
}

/// <summary>
///     Utilities for inspecting the values of <see cref="Element.State" />.
/// </summary>
[PublicAPI]
public static class ElementStateExt
{
	/// <summary>
	///     Determines if this state is solid.
	/// </summary>
	/// <param name="state">The state to check.</param>
	/// <returns>Whether the state is considered solid.</returns>
	[PublicAPI]
	public static bool IsSolid(this Element.State state)
	{
		// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
		return (state & Element.State.Solid) == Element.State.Solid;
	}

	/// <summary>
	///     Determines if this state is liquid.
	/// </summary>
	/// <param name="state">The state to check.</param>
	/// <returns>Whether the state is considered liquid.</returns>
	[PublicAPI]
	public static bool IsLiquid(this Element.State state)
	{
		// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
		return (state & Element.State.Solid) == Element.State.Liquid;
	}

	/// <summary>
	///     Determines if this state is gas.
	/// </summary>
	/// <param name="state">The state to check.</param>
	/// <returns>Whether the state is considered gas.</returns>
	[PublicAPI]
	public static bool IsGas(this Element.State state)
	{
		// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
		return (state & Element.State.Solid) == Element.State.Gas;
	}

	/// <summary>
	///     Determines if this state is vacuum.
	/// </summary>
	/// <param name="state">The state to check.</param>
	/// <returns>Whether the state is considered vacuum.</returns>
	[PublicAPI]
	public static bool IsVacuum(this Element.State state)
	{
		// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
		return (state & Element.State.Solid) == Element.State.Vacuum;
	}
}
