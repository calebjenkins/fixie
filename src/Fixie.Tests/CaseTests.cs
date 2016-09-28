﻿namespace Fixie.Tests
{
    using System;
    using Assertions;

    public class CaseTests
    {
        public void ShouldBeNamedAfterTheUnderlyingMethod()
        {
            var @case = Case("Returns");

            @case.Name.ShouldEqual("Fixie.Tests.CaseTests.Returns");
        }

        public void ShouldIncludeParameterValuesInNameWhenTheUnderlyingMethodHasParameters()
        {
            var @case = Case("Parameterized", 123, true, 'a', "with \"quotes\"", "long \"string\" gets truncated", null, this);

            @case.Name.ShouldEqual("Fixie.Tests.CaseTests.Parameterized(123, True, 'a', \"with \\\"quotes\\\"\", \"long \\\"string\\\" g...\", null, Fixie.Tests.CaseTests)");
        }

        public void ShouldIncludeEscapeSequencesInNameWhenTheUnderlyingMethodHasCharParameters()
        {
            Case("Char", '\"').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\"')");
            Case("Char", '"').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\"')");
            Case("Char", '\'').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\'')");

            Case("Char", '\\').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\\\')");
            Case("Char", '\0').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\a').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\a')");
            Case("Char", '\b').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\b')");
            Case("Char", '\f').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\f')");
            Case("Char", '\n').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\n')");
            Case("Char", '\r').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\r')");
            Case("Char", '\t').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\t')");
            Case("Char", '\v').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\v')");

            // Unicode characters 0085, 2028, and 2029 represent line endings Next Line, Line Separator, and Paragraph Separator, respectively.
            // Just like \r and \n, we escape these in order to present a readable string literal.  All other unicode sequences pass through
            // with no additional special treatment.

            // \uxxxx - Unicode escape sequence for character with hex value xxxx.
            Case("Char", '\u0000').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\u0085').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u0085')");
            Case("Char", '\u2028').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u2028')");
            Case("Char", '\u2029').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u2029')");
            Case("Char", '\u263A').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('☺')");

            // \xn[n][n][n] - Unicode escape sequence for character with hex value nnnn (variable length version of \uxxxx).
            Case("Char", '\x0000').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\x000').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\x00').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\x0').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\x0085').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u0085')");
            Case("Char", '\x085').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u0085')");
            Case("Char", '\x85').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u0085')");
            Case("Char", '\x2028').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u2028')");
            Case("Char", '\x2029').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u2029')");
            Case("Char", '\x263A').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('☺')");

            //\Uxxxxxxxx - Unicode escape sequence for character with hex value xxxxxxxx (for generating surrogates).
            Case("Char", '\U00000000').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\0')");
            Case("Char", '\U00000085').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u0085')");
            Case("Char", '\U00002028').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u2028')");
            Case("Char", '\U00002029').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('\\u2029')");
            Case("Char", '\U0000263A').Name.ShouldEqual("Fixie.Tests.CaseTests.Char('☺')");
        }

        public void ShouldIncludeEscapeSequencesInNameWhenTheUnderlyingMethodHasStringParameters()
        {
            Case("String", "\'").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"'\")");
            Case("String", "'").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"'\")");
            Case("String", "\"").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\\"\")");

            Case("String", "\\").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\\\\")");
            Case("String", "\0").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\a").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\a\")");
            Case("String", "\b").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\b\")");
            Case("String", "\f").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\f\")");
            Case("String", "\n").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\n\")");
            Case("String", "\r").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\r\")");
            Case("String", "\t").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\t\")");
            Case("String", "\v").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\v\")");

            // Unicode characters 0085, 2028, and 2029 represent line endings Next Line, Line Separator, and Paragraph Separator, respectively.
            // Just like \r and \n, we escape these in order to present a readable string literal.  All other unicode sequences pass through
            // with no additional special treatment.

            // \uxxxx - Unicode escape sequence for character with hex value xxxx.
            Case("String", "\u0000").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\u0085").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u0085\")");
            Case("String", "\u2028").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u2028\")");
            Case("String", "\u2029").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u2029\")");
            Case("String", "\u263A").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"☺\")");

            // \xn[n][n][n] - Unicode escape sequence for character with hex value nnnn (variable length version of \uxxxx).
            Case("String", "\x0000").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\x000").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\x00").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\x0").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\x0085").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u0085\")");
            Case("String", "\x085").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u0085\")");
            Case("String", "\x85").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u0085\")");
            Case("String", "\x2028").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u2028\")");
            Case("String", "\x2029").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u2029\")");
            Case("String", "\x263A").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"☺\")");

            //\Uxxxxxxxx - Unicode escape sequence for character with hex value xxxxxxxx (for generating surrogates).
            Case("String", "\U00000000").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\0\")");
            Case("String", "\U00000085").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u0085\")");
            Case("String", "\U00002028").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u2028\")");
            Case("String", "\U00002029").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"\\u2029\")");
            Case("String", "\U0000263A").Name.ShouldEqual("Fixie.Tests.CaseTests.String(\"☺\")");
        }

        public void ShouldIncludeResolvedGenericArgumentsInNameWhenTheUnderlyingMethodIsGeneric()
        {
            Case("Generic", 123, true, "a", "b")
                .Name.ShouldEqual("Fixie.Tests.CaseTests.Generic<System.Boolean, System.String>(123, True, \"a\", \"b\")");

            Case("Generic", 123, true, 1, null)
                .Name.ShouldEqual("Fixie.Tests.CaseTests.Generic<System.Boolean, System.Object>(123, True, 1, null)");

            Case("Generic", 123, 1.23m, "a", null)
                .Name.ShouldEqual("Fixie.Tests.CaseTests.Generic<System.Decimal, System.String>(123, 1.23, \"a\", null)");

            Case("ConstrainedGeneric", 1)
                .Name.ShouldEqual("Fixie.Tests.CaseTests.ConstrainedGeneric<System.Int32>(1)");

            Case("ConstrainedGeneric", true)
                .Name.ShouldEqual("Fixie.Tests.CaseTests.ConstrainedGeneric<System.Boolean>(True)");
        }

        public void ShouldUseGenericTypeParametersInNameWhenGenericTypeParametersCannotBeResolved()
        {
            Case("ConstrainedGeneric", "Incompatable")
                .Name.ShouldEqual("Fixie.Tests.CaseTests.ConstrainedGeneric<T>(\"Incompatable\")");
        }

        public void ShouldHaveMethodGroupComposedOfClassNameAndMethodNameWithNoSignature()
        {
            Case("Returns").MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.Returns");
            Case("Parameterized", 123, true, 'a', "s", null, this).MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.Parameterized");
            Case("Generic", 123, true, "a", "b").MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.Generic");
            Case("Generic", 123, true, 1, null).MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.Generic");
            Case("Generic", 123, 1.23m, "a", null).MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.Generic");
            Case("ConstrainedGeneric", 1).MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.ConstrainedGeneric");
            Case("ConstrainedGeneric", true).MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.ConstrainedGeneric");
            Case("ConstrainedGeneric", "Incompatable").MethodGroup.FullName.ShouldEqual("Fixie.Tests.CaseTests.ConstrainedGeneric");
        }

        public void ShouldTrackConcreteTestClass()
        {
            var methodDeclaredInChildClass =
                Case<SampleChildTestClass>("TestMethodDefinedWithinChildClass");
            methodDeclaredInChildClass.Class.ShouldEqual(typeof(SampleChildTestClass));

            var methodDeclaredInParentClass =
                Case<SampleParentTestClass>("TestMethodDefinedWithinParentClass");
            methodDeclaredInParentClass.Class.ShouldEqual(typeof(SampleParentTestClass));

            var parentMethodInheritedByChildClass =
                Case<SampleChildTestClass>("TestMethodDefinedWithinParentClass");
            parentMethodInheritedByChildClass.Class.ShouldEqual(typeof(SampleChildTestClass));
        }

        public void ShouldTrackExceptionsAsFailureReasons()
        {
            var exceptionA = new InvalidOperationException();
            var exceptionB = new DivideByZeroException();

            var @case = Case("Returns");

            @case.Exceptions.ShouldBeEmpty();
            @case.Fail(exceptionA);
            @case.Fail(exceptionB);
            @case.Exceptions.ShouldEqual(exceptionA, exceptionB);
        }

        public void CanSuppressFailuresByClearingExceptionLog()
        {
            var exceptionA = new InvalidOperationException();
            var exceptionB = new DivideByZeroException();

            var @case = Case("Returns");

            @case.Exceptions.ShouldBeEmpty();
            @case.Fail(exceptionA);
            @case.Fail(exceptionB);
            @case.ClearExceptions();
            @case.Exceptions.ShouldBeEmpty();
        }

        class SampleParentTestClass
        {
            public void TestMethodDefinedWithinParentClass()
            {
            }
        }

        class SampleChildTestClass : SampleParentTestClass
        {
            public void TestMethodDefinedWithinChildClass()
            {
            }
        }

        static Case Case(string methodName, params object[] parameters)
        {
            return Case<CaseTests>(methodName, parameters);
        }

        static Case Case<TTestClass>(string methodName, params object[] parameters)
        {
            return new Case(
                new Method(
                    typeof(TTestClass),
                    typeof(TTestClass)
                        .GetInstanceMethod(methodName)), parameters);
        }

        void Returns()
        {
        }

        void Throws()
        {
            throw new FailureException();
        }

        void Parameterized(int i, bool b, char ch, string s1, string s2, object obj, CaseTests complex)
        {
        }

        void Char(char ch)
        {
        }

        void String(string s)
        {
        }

        void Generic<T1, T2>(int i, T1 t1, T2 t2a, T2 t2b)
        {
        }

        void ConstrainedGeneric<T>(T t) where T : struct
        {
        }
    }
}
