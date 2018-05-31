using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Slipka.Graphql
{
    public class MethodEnum : EnumerationGraphType
    {
        public MethodEnum()
        {
            Name = "Methods";
            Description = "HTTP verbs";

            AddValue("Delete", "DELETE", "Delete");
            AddValue("Get", "GET", "Get");
            AddValue("Head", "HEAD", "Head");
            AddValue("Options", "OPTIONS", "Options");
            AddValue("Post", "POST", "Post");
            AddValue("Put", "PUT", "Put");
            AddValue("Trace", "TRACE", "Trace");
        }
    }
}
