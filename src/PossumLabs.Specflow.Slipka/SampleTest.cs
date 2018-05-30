using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

namespace PossumLabs.Specflow.Slipka
{
    //https://fluentassertions.com/examples/
    class SampleTest
    {
        public SampleTest()
        {
            var t = new ProxyWrapper(new Uri(""));
            t.Open(new Uri("http:\\google.com"));
            t.RegisterIntercept(new CallTemplate
            {
                Method ="GET",
                Request = new Message {  },
                Uri = "/test",
                Response = new Message { },
                StatusCode = "200"
            });
            var ret = t.Call("/test", RestSharp.Method.GET);
            ret.StatusCode.Should().Equals(System.Net.HttpStatusCode.OK);

            t.Close();

            var calls = t.GetCalls();
            calls.Should().HaveCount(1);
            var call = calls.First();
            call.Overridden.Should().BeTrue();


            //Action action = () => recipe.AddIngredient("Milk", 100, Unit.Spoon);
            //action
            //                    .Should().Throw<RuleViolationException>()
            //                    .WithMessage("change the unit of an existing ingredient", ComparisonMode.Substring)
            //                    .And.Violations.Should().Contain(BusinessRule.CannotChangeIngredientQuanity);

            // x-slipka This request went trough Slipka proxy, the request is possibly logged and likely tampered with
        }
        ///Todo
        /// Single proxy override
        /// 2 proxy passtrough to override
        /// Record based on path
        /// Record based on headers
        /// Closing a proxy
        /// Decoration of requests
        /// Request filtering

        ///Variations
        /// Methods
        /// Response Codes
        /// Headers
        /// Body
        /// Tags
        /// Reporting


        /// Given the Proxy
        /// |var|
        /// |P1|
        /// And the Proxy 'P1' has Intercepts
        /// |var|path|Method|Status Code |
        /// |I1 |\test|GET| 204|
        /// And the Requests
        /// |var|proxy|path|Method|
        /// |R1|P1|\test|GET|
        /// When executing Request 'R1'
        /// Then the Request 'R1' has response
        /// |Reponse Code|
        /// |204|
        /// And the Proxy 'P1' Recieved calls
        /// |path  | method | Status Code| Is Override |
        /// |\test | GET    |         204|         true|


        /// Given the Proxy 'P1' records
        /// | path  |
        /// | \test |

        /// And the Proxy 'P1' Recieved calls
        /// | Is Recorded |
        /// |        true |
        
        /// Given the Intercept 'I1' is removed

        /// Given the Headers 'H1'
        /// | Key        | Value |
        /// |content-type| json  |

        /// Given the Proxy 'P1' decorates requests with Headers 'H1'
       
        /// Given the Proxy 'P1' has all decorations removed

        /// Given the Proxy 'P1' decorates requests with Headers 'H1' for
        /// |Path  | Method | Headers |
        /// |\test | GET    | H1      |

        /// Given the Proxy
        /// |var|
        /// |Destination|
        /// And the Proxy
        /// |var|Host  | Port        |
        /// |P2|Destination.Host| Destination.ProxyPort|
        /// And the Proxy 'P1' has Intercepts
        /// |path|Method|Status Code |
        /// |\test|GET| 204|
        /// And the Requests
        /// |var|Proxy|Path|Method|
        /// |R1|P2|\test|GET|
        /// When executing Request 'R1'
        /// Then the Request 'R1' has response
        /// |Reponse Code|
        /// |204|
        /// And the Proxy 'P2' Recieved calls
        /// |Path  | Method | Status Code| Is Override |
        /// |\test | GET    |         204|        false| 

    }
}
