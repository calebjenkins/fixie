namespace Fixie
{
    public class MethodGroup
    {
        public string Class { get; }
        public string Method { get; }
        public string FullName { get; }

        public MethodGroup(Method method)
        {
            Class = method.Class.FullName;
            Method = method.Name;
            FullName = Class + "." + Method;
        }

        public MethodGroup(string fullName)
        {
            var indexOfMemberSeparator = fullName.LastIndexOf(".");
            var className = fullName.Substring(0, indexOfMemberSeparator);
            var methodName = fullName.Substring(indexOfMemberSeparator + 1);

            Class = className;
            Method = methodName;
            FullName = fullName;
        }
    }
}