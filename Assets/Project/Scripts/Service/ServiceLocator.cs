using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<System.Type, object> services = new();

    public static void Provide<T>(T service)
    {
        services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        return (T)services.GetValueOrDefault(typeof(T));
    }
}
