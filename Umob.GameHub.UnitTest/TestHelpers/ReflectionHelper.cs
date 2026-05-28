using System.Reflection;

namespace Umob.GameHub.Application.UnitTests.TestHelpers;

internal static class ReflectionHelper
{
	public static void SetPrivateProperty<T>(
		T instance,
		string propertyName,
		object? value)
	{
		var property = typeof(T).GetProperty(
			propertyName,
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		if (property is null)
		{
			throw new InvalidOperationException(
				$"Property '{propertyName}' was not found on type '{typeof(T).Name}'.");
		}

		property.SetValue(instance, value);
	}

	public static T CreateInstance<T>()
	{
		var instance = Activator.CreateInstance(
			typeof(T),
			nonPublic: true);

		if (instance is null)
		{
			throw new InvalidOperationException(
				$"Could not create instance of type '{typeof(T).Name}'.");
		}

		return (T)instance;
	}
}