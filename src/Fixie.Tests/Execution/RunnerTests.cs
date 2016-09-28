namespace Fixie.Tests.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fixie.Execution;
    using static System.Environment;
    using static Utility;

    public class RunnerTests
    {
        static readonly string Self = FullName<RunnerTests>();

        public void ShouldExecuteAllCasesInAllDiscoveredTestClasses()
        {
            var listener = new StubListener();
            var convention = SelfTestConvention.Build();
            convention.CaseExecution.Skip(x => x.Method.Has<SkipAttribute>());

            var bus = new Bus(listener);
            new Runner(bus).RunTypes(GetType().Assembly, convention,
                typeof(SampleIrrelevantClass), typeof(PassTestClass), typeof(int),
                typeof(PassFailTestClass), typeof(SkipTestClass));

            listener.Entries.ShouldEqual(
                Self + "+PassTestClass.PassA passed",
                Self + "+PassTestClass.PassB passed",
                Self + "+PassFailTestClass.Fail failed: 'Fail' failed!",
                Self + "+PassFailTestClass.Pass passed",
                Self + "+SkipTestClass.SkipA skipped",
                Self + "+SkipTestClass.SkipB skipped");
        }

        public void ShouldAllowRandomShufflingOfCaseExecutionOrder()
        {
            var listener = new StubListener();
            var convention = SelfTestConvention.Build();
            convention.CaseExecution.Skip(x => x.Method.Has<SkipAttribute>());

            convention.ClassExecution
                .CreateInstancePerClass()
                .ShuffleCases(new Random(1));

            var bus = new Bus(listener);
            new Runner(bus).RunTypes(GetType().Assembly, convention,
                typeof(SampleIrrelevantClass), typeof(PassTestClass), typeof(int),
                typeof(PassFailTestClass), typeof(SkipTestClass));

            listener.Entries.ShouldEqual(
                Self + "+PassTestClass.PassB passed",
                Self + "+PassTestClass.PassA passed",
                Self + "+PassFailTestClass.Fail failed: 'Fail' failed!",
                Self + "+PassFailTestClass.Pass passed",
                Self + "+SkipTestClass.SkipB skipped",
                Self + "+SkipTestClass.SkipA skipped");
        }

        public void ShouldShortCircuitTestExecutionByFailingAllCasesWhenCaseOrderingThrows()
        {
            var listener = new StubListener();
            var convention = SelfTestConvention.Build();
            convention.CaseExecution.Skip(x => x.Method.Has<SkipAttribute>());

            convention.ClassExecution
                .CreateInstancePerClass()
                .SortCases((caseA, caseB) => { throw new Exception("SortCases lambda expression threw!"); });

            convention.Parameters
                .Add<BuggyParameterSource>();

            var bus = new Bus(listener);
            new Runner(bus).RunTypes(GetType().Assembly, convention,
                typeof(SampleIrrelevantClass), typeof(PassTestClass), typeof(int),
                typeof(PassFailTestClass), typeof(SkipTestClass), typeof(BuggyParameterGenerationTestClass));

            //NOTE: Since the ordering of cases is deliberately failing, and since member order via reflection
            //      is undefined, we explicitly sort the listener Entries here to avoid making a brittle assertion.

            var strings = listener.Entries.OrderBy(x => x).ToArray();
            strings.ShouldEqual(

                Self + "+BuggyParameterGenerationTestClass.ParameterizedA failed: Exception thrown while attempting to yield input parameters for method: ParameterizedA" + NewLine +
                "    Secondary Failure: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+BuggyParameterGenerationTestClass.ParameterizedB failed: Exception thrown while attempting to yield input parameters for method: ParameterizedB" + NewLine +
                "    Secondary Failure: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+PassFailTestClass.Fail failed: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+PassFailTestClass.Pass failed: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+PassTestClass.PassA failed: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+PassTestClass.PassB failed: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+SkipTestClass.SkipA failed: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!",

                Self + "+SkipTestClass.SkipB failed: Failed to compare two elements in the array." + NewLine +
                "    Inner Exception: SortCases lambda expression threw!");
        }

        class SampleIrrelevantClass
        {
            public void PassA() { }
            public void PassB() { }
        }

        class PassTestClass
        {
            public void PassA() { }
            public void PassB() { }
        }

        class PassFailTestClass
        {
            public void Pass() { }
            public void Fail() { throw new FailureException(); }
        }

        class SkipTestClass
        {
            [Skip]
            public void SkipA() { throw new ShouldBeUnreachableException(); }
            [Skip]
            public void SkipB() { throw new ShouldBeUnreachableException(); }
        }

        class BuggyParameterGenerationTestClass
        {
            public void ParameterizedA(int i) { }
            public void ParameterizedB(int i) { }
        }

        class BuggyParameterSource : ParameterSource
        {
            public IEnumerable<object[]> GetParameters(Method method)
            {
                if (method.MethodInfo.GetParameters().Length == 0)
                    yield break;

                throw new Exception("Exception thrown while attempting to yield input parameters for method: " + method.Name);
            }
        }
    }
}