using System;

public static class SafetyAction
{
	public static void SafeInvoke(this Action action)
	{
		if (action != null)
		{
			action.Invoke();
		}
	}

	public static void SafeInvoke<T>(this Action<T> action, T arg1)
	{
		if (action != null)
		{
			action.Invoke(arg1);
		}
	}

	public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		if (action != null)
		{
			action.Invoke(arg1, arg2);
		}
	}

}