using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace ONITwitchCore;

public class MainThreadScheduler : KMonoBehaviour
{
	private static MainThreadScheduler instance;
	private readonly ConcurrentQueue<System.Action> actions = new();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		instance = this;
	}

	private void Update()
	{
		while (actions.TryDequeue(out var action))
		{
			action();
		}
	}

	public static void Schedule([NotNull] System.Action action)
	{
		instance.actions.Enqueue(action);
	}
}
