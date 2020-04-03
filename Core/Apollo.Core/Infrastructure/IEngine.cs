using Apollo.Core.Configuration;
using Apollo.Core.Infrastructure.DependencyManagement;
using System;

namespace Apollo.Core.Infrastructure
{
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        void Initialize(ApolloConfig config);

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}
