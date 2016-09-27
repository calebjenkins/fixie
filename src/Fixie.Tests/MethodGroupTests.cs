namespace Fixie.Tests
{
    using Assertions;

    public class MethodGroupTests
    {
        public void CanRepresentMethodDeclaredInChildClass()
        {
            var methodInfo = typeof(ChildClass).GetInstanceMethod("MethodDefinedWithinChildClass");
            var method = new Method(typeof(ChildClass), methodInfo);

            var actual = new MethodGroup(method);
            actual.Class.ShouldEqual("Fixie.Tests.MethodGroupTests+ChildClass");
            actual.Method.ShouldEqual("MethodDefinedWithinChildClass");
            actual.FullName.ShouldEqual("Fixie.Tests.MethodGroupTests+ChildClass.MethodDefinedWithinChildClass");
        }

        public void CanRepresentMethodDeclaredInParentClass()
        {
            var methodInfo = typeof(ParentClass).GetInstanceMethod("MethodDefinedWithinParentClass");
            var method = new Method(typeof(ParentClass), methodInfo);

            var actual = new MethodGroup(method);
            actual.Class.ShouldEqual("Fixie.Tests.MethodGroupTests+ParentClass");
            actual.Method.ShouldEqual("MethodDefinedWithinParentClass");
            actual.FullName.ShouldEqual("Fixie.Tests.MethodGroupTests+ParentClass.MethodDefinedWithinParentClass");
        }

        public void CanRepresentMethodInheritedByChildClass()
        {
            var methodInfo = typeof(ChildClass).GetInstanceMethod("MethodDefinedWithinParentClass");
            var method = new Method(typeof(ChildClass), methodInfo);

            var actual = new MethodGroup(method);
            actual.Class.ShouldEqual("Fixie.Tests.MethodGroupTests+ChildClass");
            actual.Method.ShouldEqual("MethodDefinedWithinParentClass");
            actual.FullName.ShouldEqual("Fixie.Tests.MethodGroupTests+ChildClass.MethodDefinedWithinParentClass");
        }

        public void CanParseFromFullNameStrings()
        {
            AssertMethodGroup(
                new MethodGroup("Fixie.Tests.MethodGroupTests+ChildClass.MethodDefinedWithinChildClass"),
                "Fixie.Tests.MethodGroupTests+ChildClass",
                "MethodDefinedWithinChildClass",
                "Fixie.Tests.MethodGroupTests+ChildClass.MethodDefinedWithinChildClass");

            AssertMethodGroup(
                new MethodGroup("Fixie.Tests.MethodGroupTests+ParentClass.MethodDefinedWithinParentClass"),
                "Fixie.Tests.MethodGroupTests+ParentClass",
                "MethodDefinedWithinParentClass",
                "Fixie.Tests.MethodGroupTests+ParentClass.MethodDefinedWithinParentClass");

            AssertMethodGroup(
                new MethodGroup("Fixie.Tests.MethodGroupTests+ChildClass.MethodDefinedWithinParentClass"),
                "Fixie.Tests.MethodGroupTests+ChildClass",
                "MethodDefinedWithinParentClass",
                "Fixie.Tests.MethodGroupTests+ChildClass.MethodDefinedWithinParentClass");
        }

        static void AssertMethodGroup(MethodGroup actual, string expectedClass, string expectedMethod, string expectedFullName)
        {
            actual.Class.ShouldEqual(expectedClass);
            actual.Method.ShouldEqual(expectedMethod);
            actual.FullName.ShouldEqual(expectedFullName);
        }

        class ParentClass
        {
            public void MethodDefinedWithinParentClass()
            {
            }
        }

        class ChildClass : ParentClass
        {
            public void MethodDefinedWithinChildClass()
            {
            }
        }
    }
}