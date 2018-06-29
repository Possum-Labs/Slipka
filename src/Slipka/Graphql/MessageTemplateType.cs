using GraphQL.Types;
using Slipka.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class MessageTemplateType : ObjectGraphType<MessageTemplate>
    {
        public MessageTemplateType()
        {
            Name = "MessageTemplate";
            Description = "A message template used for modifying traffic.";

            Field(d => d.Content).Description("Content or Body of the message.");
            
            Field<ListGraphType<HeaderType>>(
                "headers",
                resolve: context => context.Source.Headers
            );
        }
    }
}
