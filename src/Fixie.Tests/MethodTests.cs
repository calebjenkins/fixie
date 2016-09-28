namespace Fixie.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Assertions;

    public class MethodTests
    {
        public void CanRepresentMethodDeclaredInChildClass()
        {
            var methodInfo = typeof(ChildClass).GetInstanceMethod("MethodDefinedWithinChildClass");

            var actual = new Method(typeof(ChildClass), methodInfo);
            actual.Class.ShouldEqual(typeof(ChildClass));
            actual.MethodInfo.ShouldEqual(methodInfo);
            actual.Name.ShouldEqual("MethodDefinedWithinChildClass");
        }

        public void CanRepresentMethodDeclaredInParentClass()
        {
            var methodInfo = typeof(ParentClass).GetInstanceMethod("MethodDefinedWithinParentClass");

            var actual = new Method(typeof(ParentClass), methodInfo);
            actual.Class.ShouldEqual(typeof(ParentClass));
            actual.MethodInfo.ShouldEqual(methodInfo);
            actual.Name.ShouldEqual("MethodDefinedWithinParentClass");
        }

        public void CanRepresentMethodInheritedByChildClass()
        {
            var methodInfo = typeof(ChildClass).GetInstanceMethod("MethodDefinedWithinParentClass");

            var actual = new Method(typeof(ChildClass), methodInfo);
            actual.Class.ShouldEqual(typeof(ChildClass));
            actual.MethodInfo.ShouldEqual(methodInfo);
            actual.Name.ShouldEqual("MethodDefinedWithinParentClass");
        }

        public void CanDetectWhetherMethodIsDispose()
        {
            Method("ReturnsVoid").IsDispose().ShouldBeFalse();
            Method("ReturnsInt").IsDispose().ShouldBeFalse();
            Method<NonDisposableWithDisposeMethod>("Dispose").IsDispose().ShouldBeFalse();
            MethodBySignature<Disposable>(typeof(void), "Dispose", typeof(bool)).IsDispose().ShouldBeFalse();
            MethodBySignature<Disposable>(typeof(void), "Dispose").IsDispose().ShouldBeTrue();
        }

        void ReturnsVoid() { }
        int ReturnsInt() { return 0; }

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

        class NonDisposableWithDisposeMethod
        {
            public void Dispose() { }
        }

        class Disposable : NonDisposableWithDisposeMethod, IDisposable
        {
            public void Dispose(bool disposing) { }
        }

        static Method Method(string name)
        {
            return Method<MethodTests>(name);
        }

        static Method Method<T>(string name)
        {
            return new Method(typeof(T), typeof(T).GetInstanceMethod(name));
        }

        private static Method MethodBySignature<T>(Type returnType, string name, params Type[] parameterTypes)
        {
            return new Method(
                typeof(T),
                typeof(T)
                    .GetInstanceMethods()
                    .Single(m => m.HasSignature(returnType, name, parameterTypes)));
        }
    }
}