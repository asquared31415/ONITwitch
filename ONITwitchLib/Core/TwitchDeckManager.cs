using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib.Core;

/// <summary>
/// Provides methods to manipulate the deck of events 
/// </summary>
[PublicAPI]
public class TwitchDeckManager
{
	/// <summary>
	/// The instance of the deck manager.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[PublicAPI]
	[NotNull]
	public static TwitchDeckManager Instance => instance ??= GetDeckManager();

	/// <summary>
	/// Adds an <see cref="EventGroup"/> of actions to the deck
	/// </summary>
	/// <param name="group"></param>
	[PublicAPI]
	public void AddGroup([NotNull] EventGroup group)
	{
		addGroupDelegate(group.Obj);
	}

	/// <summary>
	/// Gets the <see cref="EventGroup"/> with the name specified by <paramref name="name"/>, if it exists in the deck.
	/// </summary>
	/// <param name="name">The name of the group to retrieve.</param>
	/// <returns>The group, if it exists, otherwise <see langword="null"/>.</returns>
	[PublicAPI]
	[MustUseReturnValue("This retrieves a group without modifying anything")]
	[CanBeNull]
	public EventGroup GetGroup([NotNull] string name)
	{
		var ret = getGroupDelegate(name);
		return ret != null ? new EventGroup(ret) : null;
	}

	/// <summary>
	/// Gets all <see cref="EventGroup"/>s registered in the deck.
	/// </summary>
	/// <returns>An enumerable containing the groups in the deck.</returns>
	[PublicAPI]
	[System.Diagnostics.Contracts.Pure]
	[NotNull]
	public IEnumerable<EventGroup> GetGroups()
	{
		return getGroupsDelegate().Select(o => new EventGroup(o));
	}

	/// <summary>
	/// Draws an <see cref="EventInfo"/> from the deck, shuffling if necessary.
	/// </summary>
	/// <returns>The drawn event.</returns>
	[PublicAPI]
	[MustUseReturnValue]
	[CanBeNull]
	public EventInfo Draw()
	{
		var result = drawDelegate();
		return new EventInfo(result);
	}

	private static Func<object> twitchDeckManagerInstanceDelegate;

	[MustUseReturnValue]
	[NotNull]
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
