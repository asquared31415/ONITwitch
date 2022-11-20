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

	private static Action<object> RuntimeTypeDelegateActionGenericOneArg<TArg1>(MethodInfo methodInfo, object arg0)
	{
		var del = (Action<TArg1>) Delegate.CreateDelegate(typeof(Action<TArg1>), arg0, methodInfo);

		void Wrapper(object arg1)
		{
			del((TArg1) arg1);
		}

		return Wrapper;
	}

	private static Action<object, object> RuntimeTypeDelegateActionGenericTwoArgs<TArg1, TArg2>(
		MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Action<TArg1, TArg2>) Delegate.CreateDelegate(typeof(Action<TArg1, TArg2>), arg0, methodInfo);

		void Wrapper(object arg1, object arg2)
		{
			del((TArg1) arg1, (TArg2) arg2);
		}

		return Wrapper;
	}

	public static Action<object> CreateRuntimeTypeActionDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateActionGenericOneArg),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object>) erasedDelegate;
	}

	public static Action<object, object> CreateRuntimeTypeActionDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateActionGenericTwoArgs),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type, arg2Type }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object, object>) erasedDelegate;
	}

	private static Func<object, object> RuntimeTypeDelegateFuncGenericOneArg<TArg1, TRet>(
		MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Func<TArg1, TRet>) Delegate.CreateDelegate(typeof(Func<TArg1, TRet>), arg0, methodInfo);

		object Wrapper(object arg1)
		{
			return del((TArg1) arg1);
		}

		return Wrapper;
	}

	private static Func<object, object, object> RuntimeTypeDelegateFuncGenericTwoArgs<TArg1, TArg2, TRet>(
		MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Func<TArg1, TArg2, TRet>) Delegate.CreateDelegate(
			typeof(Func<TArg1, TArg2, TRet>),
			arg0,
			methodInfo
		);

		object Wrapper(object arg1, object arg2)
		{
			return del((TArg1) arg1, (TArg2) arg2);
		}

		return Wrapper;
	}

	public static Func<object, object> CreateRuntimeTypeFuncDelegate(
		MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type retType
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateFuncGenericOneArg),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type, retType }
		);
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
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateFuncGenericTwoArgs),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type, arg2Type, retType }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Func<object, object, object>) erasedDelegate;
	}
}
