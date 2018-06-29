using GraphQL.Types;
using Slipka.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class SlipkaMutation : ObjectGraphType<object>
    {
        public SlipkaMutation(ISessionRepository sessionRepository)
        {
            Name = "Mutation";
            SessionRepository = sessionRepository;

            Field<BooleanGraphType>(
                "deleteSession",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                ),
                resolve: context =>
                {
                    return SessionRepository.RemoveSession(context.GetArgument<string>(name : "Id"));
                });

            Field<BooleanGraphType>(
                "deleteAll",
                arguments: new QueryArguments(
                ),
                resolve: context =>
                {
                    return SessionRepository.RemoveSessions();
                });

        }

        private ISessionRepository SessionRepository { get; }
    }
}
