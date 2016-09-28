namespace Fixie
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class Method
    {
        public Type Class { get; }
        public MethodInfo MethodInfo { get; }
        public string Name => MethodInfo.Name;

        public Method(Type @class, MethodInfo method)
        {
            Class = @class;
            MethodInfo = method;
        }
    }

    public static class MethodExtensions
    {
        public static bool IsDispose(this Method method)
        {
            var methodInfo = method.MethodInfo;

            var hasDisposeSignature =
                methodInfo.Name == "Dispose" &&
                methodInfo.IsVoid() &&
                methodInfo.GetParameters().Length == 0;

            if (!hasDisposeSignature)
                return false;

            return method.Class.GetInterfaces().Any(type => type == typeof(IDisposable));
        }
    }
}