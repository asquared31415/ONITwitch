using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using ONITwitchLib.Utils;

namespace ONITwitchLib;

public class TwitchDeckManager
{
	private readonly object deckManagerInstance;

	private static readonly Action<object, object> AddSingleItemDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
		AccessTools.DeclaredMethod(
			EventInterface.TwitchDeckManagerType,
			"AddToDeck",
			new[] { EventInterface.EventInfoType }
		),
		null,
		EventInterface.TwitchDeckManagerType,
		EventInterface.EventInfoType
	);

	private static readonly Action<object, object, object> AddMultipleItemsDelegate =
		DelegateUtil.CreateRuntimeTypeActionDelegate(
			AccessTools.DeclaredMethod(
				EventInterface.TwitchDeckManagerType,
				"AddToDeck",
				new[] { EventInterface.EventInfoType, typeof(int) }
			),
			null,
			EventInterface.TwitchDeckManagerType,
			EventInterface.EventInfoType,
			typeof(int)
		);

	private static readonly Type EventInfoEnumerableType = typeof(IEnumerable<>).MakeGenericType(
		EventInterface.EventInfoType
	);

	private static readonly Action<object, object> AddListDelegate = DelegateUtil.CreateRuntimeTypeActionDelegate(
		AccessTools.DeclaredMethod(
			EventInterface.TwitchDeckManagerType,
			"AddToDeck",
			new[] { EventInfoEnumerableType }
		),
		null,
		EventInterface.TwitchDeckManagerType,
		EventInfoEnumerableType
	);

	private static readonly Func<object, object> DrawDelegate = DelegateUtil.CreateRuntimeTypeFuncDelegate(
		AccessTools.DeclaredMethod(EventInterface.TwitchDeckManagerType, "Draw"),
		null,
		EventInterface.TwitchDeckManagerType,
		EventInterface.EventInfoType
	);


	internal TwitchDeckManager(object inst)
	{
		deckManagerInstance = inst;
	}

	public void AddToDeck([NotNull] EventInfo eventInfo)
	{
		AddSingleItemDelegate(deckManagerInstance, eventInfo.EventInfoInstance);
	}

	public void AddToDeck([NotNull] EventInfo eventInfo, int count)
	{
		AddMultipleItemsDelegate(deckManagerInstance, eventInfo.EventInfoInstance, count);
	}

	private static readonly Func<IEnumerable<object>, object> CastEventInfo =
		DelegateUtil.CreateRuntimeTypeFuncDelegate(
			AccessTools.Method(
				typeof(Enumerable),
				"Cast",
				new[] { typeof(IEnumerable) },
				new[] { EventInterface.EventInfoType }
			),
			null,
			typeof(IEnumerable),
			typeof(IEnumerable<>).MakeGenericType(EventInterface.EventInfoType)
		);

	public void AddToDeck([NotNull] IEnumerable<EventInfo> eventInfos)
	{
		var instances = eventInfos.Select(info => info.EventInfoInstance);
		var castInstances = CastEventInfo(instances);
		AddListDelegate(deckManagerInstance, castInstances);
	}

	[CanBeNull]
	public EventInfo Draw()
	{
		var result = DrawDelegate(deckManagerInstance);
		return new EventInfo(result);
	}
}
