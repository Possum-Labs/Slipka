using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class SlipkaQuery : ObjectGraphType<object>
    {
        public SlipkaQuery(ISessionRepository data)
        {
            Name = "Query";

            Field<SessionType>(
                "session",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "sessionId", Description = "id of the session" }
                ),
                resolve: context => data.GetSession(context.GetArgument<string>("sessionId"))
            );

            Field<ListGraphType<SessionType>>(
                "sessions",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "tag", DefaultValue=null, Description = "tag of the sessions" }
                ),
                resolve: context => {
                    var tag = context.GetArgument<string>("tag");
                    return data.GetAllSessions()
                        .ContinueWith(sessions => sessions.Result
                            .Where(session => tag == null || session.Tags.Contains(tag)));
                    }
            );

            Field<ListGraphType<CallType>>(
                "calls",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "sessionId", DefaultValue = null, Description = "session of the calls" },
                    new QueryArgument<StringGraphType> { Name = "tag", DefaultValue = null, Description = "tag of the call" },
                    new QueryArgument<BooleanGraphType> { Name = "recorded", DefaultValue = null, Description = "wether to only return recorded calls" },
                    new QueryArgument<IntGraphType> { Name = "minimumDuration", DefaultValue = null, Description = "only return calls with atleast the minimum duration in milliseconds" }
                ),
                resolve: context =>
                {
                    var tag = context.GetArgument<string>("tag");
                    var recorded = context.GetArgument<bool?>("recorded");
                    var minimumDuration = context.GetArgument<int?>("minimumDuration");
                    return data.GetSession(context.GetArgument<string>("sessionId"))
                        .ContinueWith(session => session.Result.Calls
                            .Where(call => tag == null || call.Tags.Contains(tag))
                            .Where(call => recorded == null || call.Recorded == recorded)
                            .Where(call => minimumDuration == null || (call.Duration.HasValue && call.Duration > minimumDuration)));
                }
            );
        }
    }
}
