using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class MessageType : ObjectGraphType<Message>
    {
        public MessageType(IFileRepository repository)
        {
            Name = "Message";
            Description = "The request or response part of a call.";

            Field<StringGraphType>(
                "content",
                resolve: context => 
                    repository.Download(context.Source.Content)
                        .ContinueWith(task => System.Text.Encoding.UTF8.GetString(task.Result))
            );

            Field<ListGraphType<HeaderType>>(
                "headers",
                resolve: context => context.Source.Headers
            );
        }
    }
}
