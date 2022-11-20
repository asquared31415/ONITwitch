using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib;

public static class DelegateUtil
{
	public static T CreateDelegate<T>([NotNull] MethodInfo methodInfo, object arg0)
		where T : MulticastDelegate
	{
		return (T) Delegate.CreateDelegate(typeof(T), arg0, methodInfo);
	}
	
	private static readonly MethodInfo RuntimeDelegateAction = AccessTools.DeclaredMethod(
		typeof(DelegateUtil),
		nameof(RuntimeTypeDelegateActionGeneric)
	);
	
	private static Action<object> RuntimeTypeDelegateActionGeneric<TArg1>(MethodInfo methodInfo, object arg0)
	{
		var del = (Action<TArg1>) Delegate.CreateDelegate(typeof(Action<TArg1>), arg0, methodInfo);

		void Wrapper(object arg1)
		{
			del((TArg1) arg1);
		}

		return Wrapper;
	}

	public static Action<object> CreateRuntimeTypeActionDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type
	)
	{
		var genericMethod = RuntimeDelegateAction.MakeGenericMethod(arg1Type);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object>) erasedDelegate;
	}

	private static Action<object, object> RuntimeTypeDelegateActionGeneric<TArg1, TArg2>(MethodInfo methodInfo, object arg0)
	{
		var del = (Action<TArg1, TArg2>) Delegate.CreateDelegate(typeof(Action<TArg1, TArg2>), arg0, methodInfo);

		void Wrapper(object arg1, object arg2)
		{
			del((TArg1) arg1, (TArg2) arg2);
		}

		return Wrapper;
	}

	public static Action<object, object> CreateRuntimeTypeActionDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type
	)
	{
		var genericMethod = RuntimeDelegateAction.MakeGenericMethod(arg1Type, arg2Type);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object, object>) erasedDelegate;
	}
	
	private static Func<object, object> RuntimeTypeDelegateFuncGeneric<TArg1, TRet>(MethodInfo methodInfo, object arg0)
	{
		var del = (Func<TArg1, TRet>) Delegate.CreateDelegate(typeof(Func<TArg1, TRet>), arg0, methodInfo);

		object Wrapper(object arg1)
		{
			return del((TArg1) arg1);
		}

		return Wrapper;
	}
	
	private static Func<object, object, object> RuntimeTypeDelegateFuncGeneric<TArg1, TArg2, TRet>(MethodInfo methodInfo, object arg0)
	{
		var del = (Func<TArg1, TArg2, TRet>) Delegate.CreateDelegate(typeof(Func<TArg1, TArg2, TRet>), arg0, methodInfo);

		object Wrapper(object arg1, object arg2)
		{
			return del((TArg1) arg1, (TArg2) arg2);
		}

		return Wrapper;
	}

	private static readonly MethodInfo RuntimeDelegateFuncGeneric = AccessTools.DeclaredMethod(
		typeof(DelegateUtil),
		nameof(RuntimeTypeDelegateFuncGeneric)
	);
	
	public static Func<object, object> CreateRuntimeTypeFuncDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type retType
	)
	{
		var genericMethod = RuntimeDelegateFuncGeneric.MakeGenericMethod(arg1Type, retType);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Func<object, object>) erasedDelegate;
	}
	
	public static Func<object, object, object> CreateRuntimeTypeFuncDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type,
		Type retType
	)
	{
		var genericMethod = RuntimeDelegateFuncGeneric.MakeGenericMethod(arg1Type, arg2Type, retType);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Func<object, object, object>) erasedDelegate;
	}
}
