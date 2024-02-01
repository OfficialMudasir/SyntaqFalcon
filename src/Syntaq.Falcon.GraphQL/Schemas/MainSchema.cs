using Abp.Dependency;
using GraphQL.Types;
using GraphQL.Utilities;
using Syntaq.Falcon.Queries.Container;
using System;

namespace Syntaq.Falcon.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IServiceProvider provider) :
            base(provider)
        {
            Query = provider.GetRequiredService<QueryContainer>();
        }
    }
}