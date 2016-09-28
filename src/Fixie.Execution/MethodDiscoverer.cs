﻿namespace Fixie.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MethodDiscoverer
    {
        readonly Func<MethodInfo, bool>[] testMethodConditions;

        public MethodDiscoverer(Convention convention)
        {
            testMethodConditions = convention.Config.TestMethodConditions.ToArray();
        }

        public IReadOnlyList<Method> TestMethods(Type testClass)
        {
            try
            {
                return testClass
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(IsMatch)
                    .Select(m => new Method(testClass, m))
                    .ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Exception thrown while attempting to run a custom method-discovery predicate. " +
                    "Check the inner exception for more details.", exception);
            }
        }

        bool IsMatch(MethodInfo candidate)
        {
            return testMethodConditions.All(condition => condition(candidate));
        }
    }
}