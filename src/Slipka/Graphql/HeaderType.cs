using GraphQL.Types;
using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class HeaderType : ObjectGraphType<Header>
    {
        public HeaderType()
        {
            Name = "Header";
            Description = "An http header.";

            Field(d => d.Key, nullable: true).Description("Key.");

            Field<ListGraphType<StringGraphType>>(
                "values",
                resolve: context => context.Source.Values
            );
        }
    }
}
