namespace Fixie
{
    using System;
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
}