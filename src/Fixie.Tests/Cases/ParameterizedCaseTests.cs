﻿namespace Fixie.Tests.Cases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Assertions;

    public class ParameterizedCaseTests : CaseTests
    {
        public void ShouldAllowConventionToGeneratePotentiallyManySetsOfInputParametersPerMethod()
        {
            Convention.Parameters.Add<InputAttributeOrDefaultParameterSource>();

            Run<ParameterizedTestClass>();

            Listener.Entries.ShouldEqual(
                For<ParameterizedTestClass>(
                    ".IntArg(0) passed",
                    ".MultipleCasesFromAttributes(1, 1, 2) passed",
                    ".MultipleCasesFromAttributes(1, 2, 3) passed",
                    ".MultipleCasesFromAttributes(5, 5, 11) failed: Expected sum of 11 but was 10.",
                    ".ZeroArgs passed"));
        }

        public void ShouldFailWithClearExplanationWhenInputParameterGenerationHasNotBeenCustomizedYetTestMethodAcceptsParameters()
        {
            Run<ParameterizedTestClass>();

            Listener.Entries.ShouldEqual(
                For<ParameterizedTestClass>(
                    ".IntArg failed: This test case has declared parameters, but no parameter values have been provided to it.",
                    ".MultipleCasesFromAttributes failed: This test case has declared parameters, but no parameter values have been provided to it.",
                    ".ZeroArgs passed"));
        }

        public void ShouldFailWithClearExplanationWhenInputParameterGenerationHasBeenCustomizedYetYieldsZeroSetsOfInputs()
        {
            Convention.Parameters.Add<EmptyParameterSource>();

            Run<ParameterizedTestClass>();

            Listener.Entries.ShouldEqual(
                For<ParameterizedTestClass>(
                    ".IntArg failed: This test case has declared parameters, but no parameter values have been provided to it.",
                    ".MultipleCasesFromAttributes failed: This test case has declared parameters, but no parameter values have been provided to it.",
                    ".ZeroArgs passed"));
        }

        public void ShouldFailWithClearExplanationWhenParameterCountsAreMismatched()
        {
            FixedParameterSource.Parameters = new[]
            {
                new object[] { },
                new object[] { 0 },
                new object[] { 0, 1 },
                new object[] { 0, 1, 2 },
                new object[] { 0, 1, 2, 3 }
            };

            Convention.Parameters.Add<FixedParameterSource>();

            Run<ParameterizedTestClass>();

            Listener.Entries.ShouldEqual(
                For<ParameterizedTestClass>(
                    ".IntArg failed: Parameter count mismatch.",
                    ".IntArg(0) passed",
                    ".IntArg(0, 1) failed: Parameter count mismatch.",
                    ".IntArg(0, 1, 2) failed: Parameter count mismatch.",
                    ".IntArg(0, 1, 2, 3) failed: Parameter count mismatch.",

                    ".MultipleCasesFromAttributes failed: Parameter count mismatch.",
                    ".MultipleCasesFromAttributes(0) failed: Parameter count mismatch.",
                    ".MultipleCasesFromAttributes(0, 1) failed: Parameter count mismatch.",
                    ".MultipleCasesFromAttributes(0, 1, 2) failed: Expected sum of 2 but was 1.",
                    ".MultipleCasesFromAttributes(0, 1, 2, 3) failed: Parameter count mismatch.",

                    ".ZeroArgs passed",
                    ".ZeroArgs(0) failed: Parameter count mismatch.",
                    ".ZeroArgs(0, 1) failed: Parameter count mismatch.",
                    ".ZeroArgs(0, 1, 2) failed: Parameter count mismatch.",
                    ".ZeroArgs(0, 1, 2, 3) failed: Parameter count mismatch."));
        }

        public void ShouldFailWithClearExplanationWhenParameterGenerationThrows()
        {
            Convention.Parameters.Add<BuggyParameterSource>();

            Run<ParameterizedTestClass>();

            Listener.Entries.ShouldEqual(
                For<ParameterizedTestClass>(
                    ".IntArg failed: Exception thrown while attempting to yield input parameters for method: IntArg",
                    ".MultipleCasesFromAttributes failed: Exception thrown while attempting to yield input parameters for method: MultipleCasesFromAttributes",
                    ".ZeroArgs failed: Exception thrown while attempting to yield input parameters for method: ZeroArgs",

                    ".IntArg(0) passed",
                    ".IntArg(1) failed: Expected 0, but was 1",

                    ".MultipleCasesFromAttributes(0) failed: Parameter count mismatch.",
                    ".MultipleCasesFromAttributes(1) failed: Parameter count mismatch.",

                    ".ZeroArgs(0) failed: Parameter count mismatch.",
                    ".ZeroArgs(1) failed: Parameter count mismatch."));
        }

        public void ShouldFailWithClearExplanationWhenParameterGenerationExceptionPreventsGenericTypeParametersFromBeingResolvable()
        {
            Convention.Parameters.Add<BuggyParameterSource>();

            Run<ConstrainedGenericTestClass>();

            Listener.Entries.ShouldEqual(
                For<ConstrainedGenericTestClass>(
                    ".ConstrainedGeneric<T> failed: Exception thrown while attempting to yield input parameters for method: ConstrainedGeneric",
                    ".UnconstrainedGeneric<System.Object> failed: Exception thrown while attempting to yield input parameters for method: UnconstrainedGeneric",
                    ".ConstrainedGeneric<System.Int32>(0) passed",
                    ".ConstrainedGeneric<System.Int32>(1) passed",
                    ".UnconstrainedGeneric<System.Int32>(0) passed",
                    ".UnconstrainedGeneric<System.Int32>(1) passed"));
        }

        public void ShouldResolveGenericTypeParameters()
        {
            Convention.Parameters.Add<InputAttributeParameterSource>();

            Run<GenericTestClass>();

            Listener.Entries.ShouldEqual(
                For<GenericTestClass>(
                    ".ConstrainedGenericMethodWithNoInputsProvided<T> failed: This test case has declared parameters, but no parameter values have been provided to it.",
                    ".GenericMethodWithNoInputsProvided<System.Object> failed: This test case has declared parameters, but no parameter values have been provided to it.",

                    ".ConstrainedGeneric<System.Int32>(1) passed",
                    ".ConstrainedGeneric<T>(\"Oops\") failed: Could not resolve type parameters for generic test case.",
                    ".GenericMethodWithIncorrectParameterCountProvided<System.Object>(123, 123) failed: Parameter count mismatch.",

                    ".MultipleGenericArgumentsMultipleParameters<System.Int32, System.Object>(123, null, 456, System.Int32, System.Object) passed",
                    ".MultipleGenericArgumentsMultipleParameters<System.Int32, System.String>(123, \"stringArg1\", 456, System.Int32, System.String) passed",
                    ".MultipleGenericArgumentsMultipleParameters<System.String, System.Object>(\"stringArg\", null, null, System.String, System.Object) passed",
                    ".MultipleGenericArgumentsMultipleParameters<System.String, System.Object>(\"stringArg1\", null, \"stringArg2\", System.String, System.Object) passed",
                    ".MultipleGenericArgumentsMultipleParameters<System.String, System.String>(null, \"stringArg1\", \"stringArg2\", System.String, System.String) passed",

                    ".SingleGenericArgument<System.Int32>(123, System.Int32) passed",
                    ".SingleGenericArgument<System.Object>(null, System.Object) passed",
                    ".SingleGenericArgument<System.String>(\"stringArg\", System.String) passed",

                    ".SingleGenericArgumentMultipleParameters<System.Int32>(123, 456, System.Int32) passed",
                    ".SingleGenericArgumentMultipleParameters<System.Object>(\"stringArg\", 123, System.Object) passed",
                    ".SingleGenericArgumentMultipleParameters<System.Object>(123, \"stringArg\", System.Object) passed",
                    ".SingleGenericArgumentMultipleParameters<System.Object>(123, null, System.Object) passed",
                    ".SingleGenericArgumentMultipleParameters<System.Object>(null, null, System.Object) passed",
                    ".SingleGenericArgumentMultipleParameters<System.String>(\"stringArg\", null, System.String) passed",
                    ".SingleGenericArgumentMultipleParameters<System.String>(\"stringArg1\", \"stringArg2\", System.String) passed",
                    ".SingleGenericArgumentMultipleParameters<System.String>(null, \"stringArg\", System.String) passed"));
        }

        class InputAttributeParameterSource : ParameterSource
        {
            public IEnumerable<object[]> GetParameters(Method method)
            {
                var inputAttributes = method.MethodInfo.GetCustomAttributes<InputAttribute>(true).ToArray();

                if (inputAttributes.Any())
                    foreach (var input in inputAttributes)
                        yield return input.Parameters;
            }
        }

        class InputAttributeOrDefaultParameterSource : ParameterSource
        {
            public IEnumerable<object[]> GetParameters(Method method)
            {
                var parameters = method.MethodInfo.GetParameters();

                var inputAttributes = method.MethodInfo.GetCustomAttributes<InputAttribute>(true).ToArray();

                if (inputAttributes.Any())
                {
                    foreach (var input in inputAttributes)
                        yield return input.Parameters;
                }
                else
                {
                    yield return parameters.Select(p => Default(p.ParameterType)).ToArray();
                }
            }
        }

        class EmptyParameterSource : ParameterSource
        {
            public IEnumerable<object[]> GetParameters(Method method)
            {
                yield break;
            }
        }

        class BuggyParameterSource : ParameterSource
        {
            public IEnumerable<object[]> GetParameters(Method method)
            {
                yield return new object[] { 0 };
                yield return new object[] { 1 };
                throw new Exception("Exception thrown while attempting to yield input parameters for method: " + method.Name);
            }
        }

        class FixedParameterSource : ParameterSource
        {
            public static object[][] Parameters { get; set; }

            public IEnumerable<object[]> GetParameters(Method method)
            {
                return Parameters;
            }
        }

        static object Default(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        class ParameterizedTestClass
        {
            public void ZeroArgs() { }

            public void IntArg(int i)
            {
                if (i != 0)
                    throw new Exception("Expected 0, but was " + i);
            }

            [Input(1, 1, 2)]
            [Input(1, 2, 3)]
            [Input(5, 5, 11)]
            public void MultipleCasesFromAttributes(int a, int b, int expectedSum)
            {
                if (a + b != expectedSum)
                    throw new Exception($"Expected sum of {expectedSum} but was {a + b}.");
            }
        }

        class GenericTestClass
        {
            [Input(null, "stringArg1", "stringArg2", typeof(string), typeof(string))]
            [Input("stringArg", null, null, typeof(string), typeof(object))]
            [Input(123, null, 456, typeof(int), typeof(object))]
            [Input("stringArg1", null, "stringArg2", typeof(string), typeof(object))]
            [Input(123, "stringArg1", 456, typeof(int), typeof(string))]
            public void MultipleGenericArgumentsMultipleParameters<T1, T2>(T1 genericArgument1A, T2 genericArgument2, T1 genericArgument1B, Type expectedT1, Type expectedT2)
            {
                typeof(T1).ShouldEqual(expectedT1, $"Expected {Format(genericArgument1A)}+{Format(genericArgument1B)} to resolve to type {expectedT1} but found type {typeof(T1)}");
                typeof(T2).ShouldEqual(expectedT2, $"Expected {Format(genericArgument2)} to resolve to type {expectedT2} but found type {typeof(T2)}");
            }

            [Input(123, 456, typeof(int))]
            [Input(123, null, typeof(object))]
            [Input(null, null, typeof(object))]
            [Input("stringArg1", "stringArg2", typeof(string))]
            [Input(123, "stringArg", typeof(object))]
            [Input("stringArg", 123, typeof(object))]
            [Input(null, "stringArg", typeof(string))]
            [Input("stringArg", null, typeof(string))]
            public void SingleGenericArgumentMultipleParameters<T>(T genericArgument1, T genericArgument2, Type expectedT)
            {
                typeof(T).ShouldEqual(expectedT, $"Expected {Format(genericArgument1)}+{Format(genericArgument2)} to resolve to type {expectedT} but found type {typeof(T)}");
            }

            [Input(123, typeof(int))]
            [Input("stringArg", typeof(string))]
            [Input(null, typeof(object))]
            public void SingleGenericArgument<T>(T genericArgument, Type expectedT)
            {
                typeof(T).ShouldEqual(expectedT, $"Expected {Format(genericArgument)} to resolve to type {expectedT} but found type {typeof(T)}");
            }

            [Input(123, 123)]
            public void GenericMethodWithIncorrectParameterCountProvided<T>(T genericArgument)
            {
                throw new ShouldBeUnreachableException();
            }

            public void GenericMethodWithNoInputsProvided<T>(T genericArgument)
            {
                throw new ShouldBeUnreachableException();
            }

            [Input(1)]
            [Input("Oops")]
            public void ConstrainedGeneric<T>(T input) where T : struct
            {
                typeof(T).IsValueType.ShouldBeTrue();
            }

            public void ConstrainedGenericMethodWithNoInputsProvided<T>(T input) where T : struct
            {
                throw new ShouldBeUnreachableException();
            }

            static string Format(object obj)
            {
                return obj?.ToString() ?? "[null]";
            }
        }

        class ConstrainedGenericTestClass
        {
            public void UnconstrainedGeneric<T>(T input)
            {
            }

            public void ConstrainedGeneric<T>(T input) where T : struct
            {
                typeof(T).IsValueType.ShouldBeTrue();
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        class InputAttribute : Attribute
        {
            public InputAttribute(params object[] parameters)
            {
                Parameters = parameters;
            }

            public object[] Parameters { get; }
        }
    }
}