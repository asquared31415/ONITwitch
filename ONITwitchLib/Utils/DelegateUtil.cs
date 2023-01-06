using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib.Utils;

public static class DelegateUtil
{
	public static T CreateDelegate<T>([NotNull] MethodInfo methodInfo, object arg0)
		where T : MulticastDelegate
	{
		return (T) Delegate.CreateDelegate(typeof(T), arg0, methodInfo);
	}

	private static Action<object> RuntimeTypeDelegateActionGenericOneArg<TArg1>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Action<TArg1>) Delegate.CreateDelegate(typeof(Action<TArg1>), arg0, methodInfo);

		void Wrapper(object arg1)
		{
			del((TArg1) arg1);
		}

		return Wrapper;
	}

	private static Action<object, object> RuntimeTypeDelegateActionGenericTwoArgs<TArg1, TArg2>(
		[NotNull] MethodInfo methodInfo,
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

	private static Action<object, object, object> RuntimeTypeDelegateActionGenericThreeArgs<TArg1, TArg2, TArg3>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Action<TArg1, TArg2, TArg3>) Delegate.CreateDelegate(
			typeof(Action<TArg1, TArg2, TArg3>),
			arg0,
			methodInfo
		);

		void Wrapper(object arg1, object arg2, object arg3)
		{
			del((TArg1) arg1, (TArg2) arg2, (TArg3) arg3);
		}

		return Wrapper;
	}

	private static Action<object, object, object, object> RuntimeTypeDelegateActionGenericFourArgs<TArg1, TArg2, TArg3,
		TArg4>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Action<TArg1, TArg2, TArg3, TArg4>) Delegate.CreateDelegate(
			typeof(Action<TArg1, TArg2, TArg3, TArg4>),
			arg0,
			methodInfo
		);

		void Wrapper(object arg1, object arg2, object arg3, object arg4)
		{
			del((TArg1) arg1, (TArg2) arg2, (TArg3) arg3, (TArg4) arg4);
		}

		return Wrapper;
	}

	public static Action<object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
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
		[NotNull] MethodInfo methodInfo,
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

	public static Action<object, object, object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type,
		Type arg3Type
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateActionGenericThreeArgs),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type, arg2Type, arg3Type }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object, object, object>) erasedDelegate;
	}

	public static Action<object, object, object, object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type,
		Type arg3Type,
		Type arg4Type
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateActionGenericFourArgs),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type, arg2Type, arg3Type, arg4Type }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Action<object, object, object, object>) erasedDelegate;
	}

	private static Func<object> RuntimeTypeDelegateFuncGenericZeroArgs<TRet>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Func<TRet>) Delegate.CreateDelegate(typeof(Func<TRet>), arg0, methodInfo);

		object Wrapper()
		{
			return del();
		}

		return Wrapper;
	}

	private static Func<object, object> RuntimeTypeDelegateFuncGenericOneArg<TArg1, TRet>(
		[NotNull] MethodInfo methodInfo,
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
		[NotNull] MethodInfo methodInfo,
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

	private static Func<object, object, object, object> RuntimeTypeDelegateFuncGenericThreeArgs<TArg1, TArg2, TArg3,
		TRet>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Func<TArg1, TArg2, TArg3, TRet>) Delegate.CreateDelegate(
			typeof(Func<TArg1, TArg2, TArg3, TRet>),
			arg0,
			methodInfo
		);

		object Wrapper(object arg1, object arg2, object arg3)
		{
			return del((TArg1) arg1, (TArg2) arg2, (TArg3) arg3);
		}

		return Wrapper;
	}

	public static Func<object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
		object arg0,
		Type retType
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateFuncGenericZeroArgs),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { retType }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Func<object>) erasedDelegate;
	}

	public static Func<object, object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
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
		[NotNull] MethodInfo methodInfo,
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

	public static Func<object, object, object, object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
		object arg0,
		Type arg1Type,
		Type arg2Type,
		Type arg3Type,
		Type retType
	)
	{
		var genericMethod = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeDelegateFuncGenericThreeArgs),
			new[] { typeof(MethodInfo), typeof(object) },
			new[] { arg1Type, arg2Type, arg3Type, retType }
		);
		var erasedDelegate = genericMethod.Invoke(null, new[] { methodInfo, arg0 });

		return (Func<object, object, object, object>) erasedDelegate;
	}

	private static Func<object, object> RuntimeTypeConstructorGenericOneArg<TArg, TRet>(
		ConstructorInfo constructorInfo
	)
	{
		var param = System.Linq.Expressions.Expression.Parameter(typeof(TArg));
		var lambda = System.Linq.Expressions.Expression.Lambda<Func<TArg, TRet>>(
			System.Linq.Expressions.Expression.New(constructorInfo, param),
			param
		);
		var compiled = lambda.Compile();

		object Wrapper(object arg1)
		{
			return compiled((TArg) arg1);
		}

		return Wrapper;
	}

	public static Func<object, object> CreateRuntimeTypeConstructorDelegate(
		ConstructorInfo constructorInfo,
		Type arg1Ty,
		Type retTy
	)
	{
		var generic = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeConstructorGenericOneArg),
			new[] { typeof(ConstructorInfo) },
			new[] { arg1Ty, retTy }
		);
		var erased = generic.Invoke(null, new object[] { constructorInfo });

		return (Func<object, object>) erased;
	}
}
