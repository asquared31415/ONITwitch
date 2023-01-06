using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

public class TwitchDeckManager
{
	public static TwitchDeckManager Instance => instance ??= GetDeckManager();

	public void AddGroup([NotNull] EventGroup group)
	{
		addGroupDelegate(group.Obj);
	}

	[MustUseReturnValue]
	[CanBeNull]
	public EventGroup GetGroup([NotNull] string name)
	{
		return new EventGroup(getGroupDelegate(name));
	}

	[MustUseReturnValue]
	[NotNull]
	public IEnumerable<EventGroup> GetGroups()
	{
		return getGroupsDelegate().Select(o => new EventGroup(o));
	}

	[CanBeNull]
	public EventInfo Draw()
	{
		var result = drawDelegate();
		return new EventInfo(result);
	}

	private static Func<object> twitchDeckManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the deck manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	private static TwitchDeckManager GetDeckManager()
	{
		if (twitchDeckManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(EventInterface.TwitchDeckManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != EventInterface.TwitchDeckManagerType)
			{
				throw new Exception(
					$"The Instance property on {EventInterface.TwitchDeckManagerType.AssemblyQualifiedName} does not return an instance of {EventInterface.TwitchDeckManagerType}"
				);
			}

			// no argument because it's static property
			twitchDeckManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		return new TwitchDeckManager(twitchDeckManagerInstanceDelegate());
	}

	private static TwitchDeckManager instance;

	private readonly Func<object> drawDelegate;
	private readonly Action<object> addGroupDelegate;
	private readonly Func<object, object> getGroupDelegate;
	private readonly Func<IEnumerable<object>> getGroupsDelegate;

	private TwitchDeckManager(object inst)
	{
		drawDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.DeclaredMethod(EventInterface.TwitchDeckManagerType, "Draw"),
			inst,
			EventInterface.EventInfoType
		);
		addGroupDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.Method(EventInterface.TwitchDeckManagerType, "AddGroup"),
			inst,
			EventInterface.EventGroupType
		);
		getGroupDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(EventInterface.TwitchDeckManagerType, "GetGroup"),
			inst,
			typeof(string),
			EventInterface.EventGroupType
		);
		getGroupsDelegate = DelegateUtil.CreateDelegate<Func<IEnumerable<object>>>(
			AccessTools.Method(EventInterface.TwitchDeckManagerType, "InternalGetGroups"),
			inst
		);
	}
}
