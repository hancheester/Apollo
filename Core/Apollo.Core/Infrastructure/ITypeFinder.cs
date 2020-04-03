using System;
using System.Collections.Generic;
using System.Reflection;

namespace Apollo.Core.Infrastructure
{
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreateClasses = true);
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreateClasses = true);
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }
}
