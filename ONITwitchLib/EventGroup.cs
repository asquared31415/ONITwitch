using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Core;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

/// <summary>
///     A group of associated <see cref="EventInfo" />s with relative weights.
///     <see cref="EventInfo" />s in an <see cref="EventGroup" /> will attempt to be spread out for variety.
/// </summary>
[PublicAPI]
public class EventGroup
{
	private static readonly Func<object, object> CreateEventGroupDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.Method(CoreTypes.EventGroupType, "GetOrCreateGroup", new[] { typeof(string) }),
		null,
		typeof(string),
		CoreTypes.EventGroupType
	);

	private static readonly Func<object, object, object, object> DefaultSingleEventGroupDelegate =
		DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(
				CoreTypes.EventGroupType,
				"InternalDefaultSingleEventGroup",
				new[] { typeof(string), typeof(int), typeof(string), typeof((object, object)) }
			),
			null,
			typeof(string),
			typeof(int),
			typeof(string),
			typeof((object, object))
		);

	internal readonly object Obj;
	private Func<object, object, object, object> addEventDelegate;

	private AccessTools.FieldRef<object, string> getNameDelegate;
	private Func<int> getTotalWeightDelegate;
	private Func<object> getWeightsDelegate;
	private Action<object> removeEventDelegate;
	private Action<object, object> setWeightDelegate;

	internal EventGroup(object inst)
	{
		Obj = inst;
		SetupDelegates();

		// add our listener to the group
		AccessTools.Method(CoreTypes.EventGroupType, "AddMergeLibChangedListener", new[] { typeof(System.Action) })
			.Invoke(Obj, new object[] { () => OnGroupChanged(this) });
	}

	/// <summary>
	///     The name of the group.
	/// </summary>
	[PublicAPI]
	[NotNull]
	public string Name => getNameDelegate(Obj);

	/// <summary>
	///     The total weight of the group.
	/// </summary>
	[PublicAPI]
	public int TotalWeight => getTotalWeightDelegate();

	/// <summary>
	///     An event that fires when the group is changed, called with the group that changed.
	/// </summary>
	[PublicAPI]
	public event Action<EventGroup> OnGroupChanged = _ => { };

	/// <summary>
	///     Gets an existing <see cref="EventGroup" /> with a specified name, or creates it if it does not exist.
	/// </summary>
	/// <param name="name">The name of the <see cref="EventGroup" /> to get or create.</param>
	/// <returns>The group that was found or created.</returns>
	[PublicAPI]
	[Pure]
	[NotNull]
	public static EventGroup GetOrCreateGroup([NotNull] string name)
	{
		return new EventGroup(CreateEventGroupDelegate(name));
	}

	/// <summary>
	///     Creates an <see cref="EventInfo" /> with a unique <see cref="EventGroup" /> that has a default name and no other
	///     <see cref="EventInfo" />s.
	/// </summary>
	/// <param name="id">The id of the <see cref="EventInfo" /> to create.</param>
	/// <param name="weight">The weight of the <see cref="EventInfo" /> to create.</param>
	/// <param name="friendlyName">The friendly name of the <see cref="EventInfo" /> to create.</param>
	/// <returns>The newly created <see cref="EventInfo" /> and its unique <see cref="EventGroup" />.</returns>
	[PublicAPI]
	[MustUseReturnValue("The group should be added to the TwitchDeckManager to be used")]
	public static (EventInfo EventInfo, EventGroup Group) DefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var (info, group) = ((object, object)) DefaultSingleEventGroupDelegate(id, weight, friendlyName);
		return (new EventInfo(info), new EventGroup(group));
	}

	/// <summary>
	///     Creates a new <see cref="EventInfo" /> in this <see cref="EventGroup" />.
	/// </summary>
	/// <param name="id">The id of the <see cref="EventInfo" /> to create.</param>
	/// <param name="weight">The weight of the <see cref="EventInfo" /> to create.</param>
	/// <param name="friendlyName">The friendly name of the <see cref="EventInfo" /> to create.</param>
	/// <returns>The newly created <see cref="EventInfo" />.</returns>
	[PublicAPI]
	[NotNull]
	public EventInfo AddEvent([NotNull] string id, int weight, [CanBeNull] string friendlyName = null)
	{
		return new EventInfo(addEventDelegate(id, weight, friendlyName));
	}

	/// <summary>
	///     Sets the weight of a specified <see cref="EventInfo" /> in the group.
	/// </summary>
	/// <param name="eventInfo">The <see cref="EventInfo" /> to change the weight for.</param>
	/// <param name="weight">The new weight.</param>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="weight" /> is less than 0.</exception>
	[PublicAPI]
	public void SetWeight([NotNull] EventInfo eventInfo, int weight)
	{
		setWeightDelegate(eventInfo.EventInfoInstance, weight);
	}

	/// <summary>
	///     Removes the specified <see cref="EventInfo" /> from the group.
	/// </summary>
	/// <param name="item">The <see cref="EventInfo" /> to remove.</param>
	[PublicAPI]
	public void RemoveEvent([NotNull] EventInfo item)
	{
		removeEventDelegate(item.EventInfoInstance);
	}

	/// <summary>
	///     Gets a the weight of each <see cref="EventInfo" /> in the group.
	/// </summary>
	/// <returns>A read-only dictionary of each <see cref="EventInfo" /> and its corresponding weight.</returns>
	[PublicAPI]
	[Pure]
	[NotNull]
	public IReadOnlyDictionary<EventInfo, int> GetWeights()
	{
		var res = (IDictionary) getWeightsDelegate();
		var keys = res.Keys.Cast<object>().Select(o => new EventInfo(o));
		var values = res.Values.Cast<int>();
		var dict = keys.Zip(values, (info, weight) => new { info, weight }).ToDictionary(e => e.info, e => e.weight);
		return dict;
	}

	/// <summary>
	///     Displays a string representation of a group using its name.
	/// </summary>
	/// <returns>A string representation of the object.</returns>
	public override string ToString()
	{
		return Obj.ToString();
	}

	private void SetupDelegates()
	{
		getNameDelegate = AccessTools.FieldRefAccess<string>(CoreTypes.EventGroupType, "Name");
		getTotalWeightDelegate = DelegateUtil.CreateDelegate<Func<int>>(
			AccessTools.PropertyGetter(CoreTypes.EventGroupType, "TotalWeight"),
			Obj
		);
		addEventDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(
				CoreTypes.EventGroupType,
				"AddEvent",
				new[] { typeof(string), typeof(int), typeof(string) }
			),
			Obj,
			typeof(string),
			typeof(int),
			typeof(string),
			CoreTypes.EventInfoType
		);
		setWeightDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(CoreTypes.EventGroupType, "SetWeight", new[] { CoreTypes.EventInfoType, typeof(int) }),
			Obj,
			CoreTypes.EventInfoType,
			typeof(int)
		);
		removeEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(CoreTypes.EventGroupType, "RemoveEvent", new[] { CoreTypes.EventInfoType }),
			Obj,
			CoreTypes.EventInfoType
		);
		getWeightsDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(CoreTypes.EventGroupType, "GetWeights", new Type[] { }),
			Obj,
			typeof(IReadOnlyDictionary<,>).MakeGenericType(CoreTypes.EventInfoType, typeof(int))
		);
	}
}
