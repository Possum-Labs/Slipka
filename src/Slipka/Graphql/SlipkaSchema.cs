using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class SlipkaSchema : Schema
    {
        public SlipkaSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<SlipkaQuery>();
            Mutation = resolver.Resolve<SlipkaMutation>();
        }
    }
}
