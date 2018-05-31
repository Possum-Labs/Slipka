using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class CallType : ObjectGraphType<Call>
    {
        public CallType(IMessageRepository rep)
        {
            Name = "Call";
            Description = "the record of a call that went trough the proxy.";

            Field(d => d.StatusCode, nullable: true).Description("Http status code.");
            Field(d => d.Duration, nullable: true).Description("Duration in ms.");
            Field(d => d.Intercepted).Description("Wether the call is intercepted.");
            Field(d => d.Recorded).Description("Wether the call is recorded.");
            Field(d => d.Recieved).Description("When the call was recieved.");

            Field<MessageType>(
                "uri",
                description: "Uri.",
                resolve: context => context.Source.Uri.ToString()
            );

            Field<MessageType>(
                "request",
                description: "The request message.",
                resolve: context => rep.GetMessage(context.Source.RequestId)
            );

            Field<MessageType>(
                "response",
                description: "The response message.",
                resolve: context => rep.GetMessage(context.Source.ResponseId)
            );

            Field<MethodEnum>("method", "Http Method of the call.");

            Field<ListGraphType<StringGraphType>>(
                "tags",
                resolve: context => context.Source.Tags
            );
        }
    }
}
