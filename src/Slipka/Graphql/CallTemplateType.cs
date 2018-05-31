using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class CallTemplateType : ObjectGraphType<CallTemplate>
    {
        public CallTemplateType()
        {
            Name = "CallTemplate";
            Description = "A template used for configuring traffic manipulation.";

            Field(d => d.StatusCode, nullable: true).Description("Http status code to trigger on.");
            Field(d => d.Uri).Description("Uri to trigger on.");
            Field(d => d.Duration, nullable: true).Description("Minimum duration in ms to trigger on.");

            Field<MessageTemplateType>("request", resolve: context => context.Source.Request);
            Field<MessageTemplateType>("response", resolve: context => context.Source.Response);

            Field<MethodEnum>("method", "Http Method of the call.");

            Field<ListGraphType<StringGraphType>>(
                "tags",
                resolve: context => context.Source.Tags
            );
        }
    }
}
