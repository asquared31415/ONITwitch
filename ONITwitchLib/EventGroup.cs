using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

public class EventGroup
{
	internal readonly object Obj;

	public EventGroup([NotNull] string name)
	{
		Obj = EventGroupCtorByName(name);
		SetupDelegates();
	}

	[MustUseReturnValue("The group should be added to the TwitchDeckManager to actually be useful")]
	public static (EventInfo EventInfo, EventGroup Group) DefaultSingleEventGroup(
		[NotNull] string id,
		int weight,
		[CanBeNull] string friendlyName = null
	)
	{
		var (info, group) = ((object, object)) DefaultSingleEventGroupDelegate(id, weight, friendlyName);
		return (new EventInfo(info), new EventGroup(group));
	}

	[NotNull]
	public EventInfo AddEvent([NotNull] string id, int weight, [CanBeNull] string friendlyName = null)
	{
		return new EventInfo(addEventDelegate(id, weight, friendlyName));
	}

	public void SetWeight([NotNull] EventInfo eventInfo, int weight)
	{
		setWeightDelegate(eventInfo.EventInfoInstance, weight);
	}

	public void RemoveEvent([NotNull] EventInfo item)
	{
		removeEventDelegate(item.EventInfoInstance);
	}

	[NotNull]
	public IReadOnlyDictionary<EventInfo, int> GetWeights()
	{
		var res = (IDictionary) getWeightsDelegate();
		var keys = res.Keys.Cast<object>().Select(o => new EventInfo(o));
		var values = res.Values.Cast<int>();
		var dict = keys.Zip(values, (info, weight) => new { info, weight }).ToDictionary(e => e.info, e => e.weight);
		return dict;
	}

	public override string ToString()
	{
		return Obj.ToString();
	}

	private static Func<object, object> EventGroupCtorByName =
		DelegateUtil.CreateRuntimeTypeConstructorDelegate(
			AccessTools.Constructor(EventInterface.EventGroupType, new[] { typeof(string) }),
			typeof(string),
			EventInterface.EventGroupType
		);

	private static Func<object, object, object, object> DefaultSingleEventGroupDelegate =
		DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(EventInterface.EventGroupType, "InternalDefaultSingleEventGroup"),
			null,
			typeof(string),
			typeof(int),
			typeof(string),
			typeof((object, object))
		);

	internal EventGroup(object inst)
	{
		Obj = inst;
		SetupDelegates();
	}

	private Func<object, object, object, object> addEventDelegate;
	private Action<object, object> setWeightDelegate;
	private Action<object> removeEventDelegate;
	private Func<object> getWeightsDelegate;

	private void SetupDelegates()
	{
		addEventDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(EventInterface.EventGroupType, "AddEvent"),
			Obj,
			typeof(string),
			typeof(int),
			typeof(string),
			EventInterface.EventInfoType
		);
		setWeightDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(EventInterface.EventGroupType, "SetWeight"),
			Obj,
			EventInterface.EventInfoType,
			typeof(int)
		);
		removeEventDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(EventInterface.EventGroupType, "RemoveEvent"),
			Obj,
			EventInterface.EventInfoType
		);
		getWeightsDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(EventInterface.EventGroupType, "GetWeights"),
			Obj,
			typeof(IReadOnlyDictionary<,>).MakeGenericType(EventInterface.EventInfoType, typeof(int))
		);
	}
}
