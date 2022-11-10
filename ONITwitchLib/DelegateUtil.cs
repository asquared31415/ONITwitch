using System;
using System.Reflection;

namespace ONITwitchLib;

public static class DelegateUtil
{
	public static T CreateDelegate<T>(object arg0, MethodInfo methodInfo)
	where T: MulticastDelegate
	{
		return (T) Delegate.CreateDelegate(typeof(T), arg0, methodInfo);
	}
}
