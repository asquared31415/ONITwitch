using System;
using System.Reflection;
using HarmonyLib;

namespace ONITwitchLib;

public static class DelegateUtil
{
	public static T CreateDelegate<T>(MethodInfo methodInfo, object arg0)
		where T : MulticastDelegate
	{
		return (T) Delegate.CreateDelegate(typeof(T), arg0, methodInfo);
	}

	private static Action<object, object> RuntimeTypeDelegateGeneric<TArg1, TArg2>(MethodInfo methodInfo, object arg0)
	{
		var del = (Action<TArg1, TArg2>) Delegate.CreateDelegate(typeof(Action<TArg1, TArg2>), arg0, methodInfo);

		void Wrapper(object arg1, object arg2)
		{
			del((TArg1) arg1, (TArg2) arg2);
		}

		return Wrapper;
	}

	private static readonly MethodInfo RuntimeDelegateGenericTwoArgs = AccessTools.DeclaredMethod(
		typeof(DelegateUtil),
		nameof(RuntimeTypeDelegateGeneric)
	);

	public static Action<object, object> CreateRuntimeTypeDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type
	)
	{
		var genericMethod = RuntimeDelegateGenericTwoArgs.MakeGenericMethod(arg1Type, arg2Type);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object, object>) erasedDelegate;
	}
}
