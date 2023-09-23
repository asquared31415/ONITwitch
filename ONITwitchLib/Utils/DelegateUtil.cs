using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace ONITwitchLib.Utils;

/// <summary>
///     Utilities for creating delegates.
///     Incorrect types are not enforced at compile time, and will turn into a cast exception at runtime.
/// </summary>
[PublicAPI]
public static class DelegateUtil
{
	/// <summary>
	///     Creates a delegate from a <see cref="MethodInfo" /> and an optional <see langword="this" /> object.
	/// </summary>
	/// <param name="methodInfo">The method to create a delegate for.</param>
	/// <param name="arg0">
	///     The <see langword="this" /> object for the delegate, or <see langword="null" /> if
	///     it is a <see langword="static" /> method.
	/// </param>
	/// <typeparam name="T">The type of the returned delegate.</typeparam>
	/// <returns>A delegate that will call the specified method.</returns>
	[PublicAPI]
	public static T CreateDelegate<T>([NotNull] MethodInfo methodInfo, [CanBeNull] object arg0)
		where T : MulticastDelegate
	{
		return (T) Delegate.CreateDelegate(typeof(T), arg0, methodInfo);
	}

	/// <summary>
	///     Creates a <see cref="Action{T}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Action<object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type
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

	/// <summary>
	///     Creates a <see cref="Action{T,U}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <param name="arg2Type">The <see cref="Type" /> of the second argument to the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Action<object, object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type,
		[NotNull] Type arg2Type
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

	/// <summary>
	///     Creates a <see cref="Action{T,U,V}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <param name="arg2Type">The <see cref="Type" /> of the second argument to the method.</param>
	/// <param name="arg3Type">The <see cref="Type" /> of the third argument to the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Action<object, object, object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type,
		[NotNull] Type arg2Type,
		[NotNull] Type arg3Type
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

	/// <summary>
	///     Creates a <see cref="Action{T,U,V,W}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <param name="arg2Type">The <see cref="Type" /> of the second argument to the method.</param>
	/// <param name="arg3Type">The <see cref="Type" /> of the third argument to the method.</param>
	/// <param name="arg4Type">The <see cref="Type" /> of the fourth argument to the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Action<object, object, object, object> CreateRuntimeTypeActionDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type,
		[NotNull] Type arg2Type,
		[NotNull] Type arg3Type,
		[NotNull] Type arg4Type
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

	/// <summary>
	///     Creates a <see cref="Func{T}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="retType">The return <see cref="Type" /> of the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Func<object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type retType
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

	/// <summary>
	///     Creates a <see cref="Func{T,U}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <param name="retType">The return <see cref="Type" /> of the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Func<object, object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type,
		[NotNull] Type retType
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

	/// <summary>
	///     Creates a <see cref="Func{T,U,V}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <param name="arg2Type">The <see cref="Type" /> of the second argument to the method.</param>
	/// <param name="retType">The return <see cref="Type" /> of the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Func<object, object, object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type,
		[NotNull] Type arg2Type,
		[NotNull] Type retType
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

	/// <summary>
	///     Creates a <see cref="Func{T,U,V,W}" /> delegate for a method with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="methodInfo">The method to call.</param>
	/// <param name="arg0">The optional <see langword="this" /> object for the delegate.</param>
	/// <param name="arg1Type">The <see cref="Type" /> of the first argument to the method.</param>
	/// <param name="arg2Type">The <see cref="Type" /> of the second argument to the method.</param>
	/// <param name="arg3Type">The <see cref="Type" /> of the third argument to the method.</param>
	/// <param name="retType">The return <see cref="Type" /> of the method.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Func<object, object, object, object> CreateRuntimeTypeFuncDelegate(
		[NotNull] MethodInfo methodInfo,
		[CanBeNull] object arg0,
		[NotNull] Type arg1Type,
		[NotNull] Type arg2Type,
		[NotNull] Type arg3Type,
		[NotNull] Type retType
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

	/// <summary>
	///     Creates a <see cref="Func{T,U}" /> delegate for a constructor with its generic types replaced
	///     with <see langword="object" />, with the real type determined at runtime.
	/// </summary>
	/// <param name="constructorInfo">The constructor to call.</param>
	/// <param name="arg1Ty">The <see cref="Type" /> of the first argument to the constructor.</param>
	/// <returns>A delegate that calls the specified method, but accepts any type at compile time.</returns>
	/// <remarks>
	///     This is relatively expensive as a one-time cost, but still significantly faster than
	///     <see cref="MethodBase.Invoke(System.Object, System.Object[])" /> if used several times.
	/// </remarks>
	/// <remarks>
	///     This method can create unbound delegates so that the instance passed can vary, by using no
	///     <see langword="this" /> object and using it as the first argument instead.
	/// </remarks>
	[PublicAPI]
	public static Func<object, object> CreateRuntimeTypeConstructorDelegate(
		[NotNull] ConstructorInfo constructorInfo,
		[NotNull] Type arg1Ty
	)
	{
		var generic = AccessTools.DeclaredMethod(
			typeof(DelegateUtil),
			nameof(RuntimeTypeConstructorGenericOneArg),
			new[] { typeof(ConstructorInfo) },
			new[] { arg1Ty, constructorInfo.ReflectedType }
		);
		var erased = generic.Invoke(null, new object[] { constructorInfo });

		return (Func<object, object>) erased;
	}

	private static Action<object> RuntimeTypeDelegateActionGenericOneArg<TArg1>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Action<TArg1>) Delegate.CreateDelegate(typeof(Action<TArg1>), arg0, methodInfo);

		return Wrapper;

		void Wrapper(object arg1)
		{
			del((TArg1) arg1);
		}
	}

	private static Action<object, object> RuntimeTypeDelegateActionGenericTwoArgs<TArg1, TArg2>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Action<TArg1, TArg2>) Delegate.CreateDelegate(typeof(Action<TArg1, TArg2>), arg0, methodInfo);

		return Wrapper;

		void Wrapper(object arg1, object arg2)
		{
			del((TArg1) arg1, (TArg2) arg2);
		}
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

		return Wrapper;

		void Wrapper(object arg1, object arg2, object arg3)
		{
			del((TArg1) arg1, (TArg2) arg2, (TArg3) arg3);
		}
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

		return Wrapper;

		void Wrapper(object arg1, object arg2, object arg3, object arg4)
		{
			del((TArg1) arg1, (TArg2) arg2, (TArg3) arg3, (TArg4) arg4);
		}
	}

	private static Func<object> RuntimeTypeDelegateFuncGenericZeroArgs<TRet>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Func<TRet>) Delegate.CreateDelegate(typeof(Func<TRet>), arg0, methodInfo);

		return Wrapper;

		object Wrapper()
		{
			return del();
		}
	}

	private static Func<object, object> RuntimeTypeDelegateFuncGenericOneArg<TArg1, TRet>(
		[NotNull] MethodInfo methodInfo,
		object arg0
	)
	{
		var del = (Func<TArg1, TRet>) Delegate.CreateDelegate(typeof(Func<TArg1, TRet>), arg0, methodInfo);

		return Wrapper;

		object Wrapper(object arg1)
		{
			return del((TArg1) arg1);
		}
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

		return Wrapper;

		object Wrapper(object arg1, object arg2)
		{
			return del((TArg1) arg1, (TArg2) arg2);
		}
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

		return Wrapper;

		object Wrapper(object arg1, object arg2, object arg3)
		{
			return del((TArg1) arg1, (TArg2) arg2, (TArg3) arg3);
		}
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

		return Wrapper;

		object Wrapper(object arg1)
		{
			return compiled((TArg) arg1);
		}
	}
}
