using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

/// <summary>
/// The main interface to the Twitch mod.
/// </summary>
public static class EventInterface
{
	private const string EventManagerTypeName = "EventLib.EventManager, ONITwitch";
	private static Type eventManagerType;

	private const string EventInfoTypeName = "EventLib.EventInfo, ONITwitch";
	private static Type eventInfoType;
	
	private const string DataManagerTypeName = "EventLib.DataManager, ONITwitch";
	private static Type dataManagerType;
	
	private const string TwitchDeckManagerTypeName = "ONITwitchCore.TwitchDeckManager, ONITwitch";
	private static Type twitchDeckManagerType;
	
	private const string ConditionsManagerTypeName = "ONITwitchCore.ConditionsManager, ONITwitch";
	private static Type conditionsManagerType;
	
	private const string DangerManagerTypeName = "ONITwitchCore.DangerManager, ONITwitch";
	private static Type dangerManagerType;
	
	private const string CoreDangerTypeName = "ONITwitchLib.Danger, ONITwitch";
	private static Type coreDangerType;
	
	private const string ToastManagerTypeName = "ONITwitchCore.Toasts.ToastManager, ONITwitch";
	private static Type toastManagerType;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventManagerType => (eventManagerType ??= Type.GetType(EventManagerTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type EventInfoType => (eventInfoType ??= Type.GetType(EventInfoTypeName))!;

	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type DataManagerType => (dataManagerType ??= Type.GetType(DataManagerTypeName))!;
	
	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type TwitchDeckManagerType => (twitchDeckManagerType ??= Type.GetType(TwitchDeckManagerTypeName))!;
	
	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type ConditionsManagerType => (conditionsManagerType ??= Type.GetType(ConditionsManagerTypeName))!;
	
	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type DangerManagerType => (dangerManagerType ??= Type.GetType(DangerManagerTypeName))!;
	
	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type CoreDangerType => (coreDangerType ??= Type.GetType(CoreDangerTypeName))!;
	
	/// <summary>
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	[NotNull]
	public static Type ToastManagerType => (toastManagerType ??= Type.GetType(ToastManagerTypeName))!;

	private static Func<object> eventManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the event manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static EventManager GetEventManagerInstance()
	{
		if (eventManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(EventManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != EventManagerType)
			{
				throw new Exception(
					$"The Instance property on {EventManagerType.AssemblyQualifiedName} does not return an instance of {EventManagerType}"
				);
			}

			// no argument because it's static property
			eventManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var instance = eventManagerInstanceDelegate();
		return new EventManager(instance);
	}
	
	private static Func<object> dataManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the data manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static DataManager GetDataManagerInstance()
	{
		if (dataManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(DataManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != DataManagerType)
			{
				throw new Exception(
					$"The Instance property on {DataManagerType.AssemblyQualifiedName} does not return an instance of {DataManagerType}"
				);
			}

			// no argument because it's static property
			dataManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var instance = dataManagerInstanceDelegate();
		return new DataManager(instance);
	}
	
	private static Func<object> twitchDeckManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the deck manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static TwitchDeckManager GetDeckManager()
	{
		if (twitchDeckManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(TwitchDeckManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != TwitchDeckManagerType)
			{
				throw new Exception(
					$"The Instance property on {TwitchDeckManagerType.AssemblyQualifiedName} does not return an instance of {TwitchDeckManagerType}"
				);
			}

			// no argument because it's static property
			twitchDeckManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var instance = twitchDeckManagerInstanceDelegate();
		return new TwitchDeckManager(instance);
	}
	
	private static Func<object> conditionsManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the conditions manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static ConditionsManager GetConditionsManager()
	{
		if (conditionsManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(ConditionsManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != ConditionsManagerType)
			{
				throw new Exception(
					$"The Instance property on {ConditionsManagerType.AssemblyQualifiedName} does not return an instance of {ConditionsManagerType}"
				);
			}

			// no argument because it's static property
			conditionsManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var instance = conditionsManagerInstanceDelegate();
		return new ConditionsManager(instance);
	}
	
	private static Func<object> dangerManagerInstanceDelegate;

	/// <summary>
	/// Gets the instance of the danger manager from the twitch mod.
	/// Only safe to access if the Twitch mod is active.
	/// </summary>
	public static DangerManager GetDangerManager()
	{
		if (dangerManagerInstanceDelegate == null)
		{
			var prop = AccessTools.Property(DangerManagerType, "Instance");
			var propInfo = prop.GetGetMethod();

			var retType = propInfo.ReturnType;
			if (retType != DangerManagerType)
			{
				throw new Exception(
					$"The Instance property on {DangerManagerType.AssemblyQualifiedName} does not return an instance of {DangerManagerType}"
				);
			}

			// no argument because it's static property
			dangerManagerInstanceDelegate = DelegateUtil.CreateDelegate<Func<object>>(propInfo, null);
		}

		var instance = dangerManagerInstanceDelegate();
		return new DangerManager(instance);
	}
}
