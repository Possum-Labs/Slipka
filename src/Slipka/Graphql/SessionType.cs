using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class SessionType : ObjectGraphType<Session>
    {
        public SessionType()
        {
            Name = "Session";
            Description = "The record of a Slipka proxy session.";

            Field(d => d.Id).Description("The id of the session.");
            Field(d => d.Name, nullable: true).Description("The name of the droid.");

            Field<ListGraphType<CallType>>(
                "calls",
                resolve: context => context.Source.Calls
            );

            Field(d => d.ProxyPort, nullable: true).Description("The port the proxy is exposed on while active.");
            Field(d => d.TargetHost).Description("The host that we forward to.");

            Field<IntGraphType>(
               "targetPort",
               resolve: context => context.Source.TargetPort.Value, 
               description: "The host port that we forward to."
           );

            Field<ListGraphType<StringGraphType>>(
                "tags",
                resolve: context => context.Source.Tags
            );

            Field<ListGraphType<CallTemplateType>>(
                "recordedCalls",
                resolve: context => context.Source.RecordedCalls
            );

            Field<ListGraphType<CallTemplateType>>(
                "interceptedCalls",
                resolve: context => context.Source.InterceptedCalls
            );

            Field<ListGraphType<CallTemplateType>>(
                "taggedCalls",
                resolve: context => context.Source.InterceptedCalls
            );
        }
    }
}
