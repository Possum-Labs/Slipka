using GraphQL.Client;
using GraphQL.Common.Request;
using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka.ValueObjects;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Slipka
{
    public class ProxyWrapper : IDomainObject
    {
        public ProxyWrapper(Uri host, Uri destination) : this(host)
        {
            Open(destination);
        }

        public ProxyWrapper(Uri host)
        {
            AdministrationUri = host;
            AdministrationClient = new RestClient(host);
            GraphQLClient = new GraphQLClient($"{host}graphql");
        }

        private Uri AdministrationUri { get; }
        private RestClient AdministrationClient { get; }
        private RestClient ProxyClient { get; set; }
        private SessionSummary ProxySession { get; set; }
        private GraphQLClient GraphQLClient { get;}

        private Uri _proxyUri;

        public Uri ProxyUri { get => _proxyUri; }

        public void Open(Uri destination)
        {
            ProxySession = new SessionSummary();
            ProxySession.TargetHost = destination.Host;
            ProxySession.TargetPort = destination.Port;

            var request = new RestRequest("/api/proxies", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(ProxySession);
            var response = AdministrationClient.Execute<SessionSummary>(request);
            if (!response.IsSuccessful)
                throw new Exception($"Was unable to open the proxy, error was {response.StatusCode} {response.StatusDescription}");
            ProxySession = response.Data;
            _proxyUri = new Uri($"http://{AdministrationUri.Host}:{ProxySession.ProxyPort}");
            ProxyClient = new RestClient(_proxyUri);
        }

        public void LogsResponsesOfType(string type, string value)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/record", Method.PUT);
            var call = new Call
            {
                Response = new Message
                {
                    Headers = new List<KeyValuePair<string, List<string>>>()
                }
            };
            call.Response.Headers.Add(new KeyValuePair<string, List<string>>(type, new List<string>() { value }));
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            AdministrationClient.Execute(request);
        }

        public void LogsCallsTo(Uri uri)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/record", Method.PUT);
            var call = new Call
            {
                Uri = uri
            };
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            AdministrationClient.Execute(request);
        }

        public void RegisterTag(CallTemplate call, string tag)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/tag/{tag}", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            AdministrationClient.Execute(request);
        }

        public void RegisterRecording(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/record", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            AdministrationClient.Execute(request);
        }

        public void RegisterIntercept(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/intercept", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            AdministrationClient.Execute(request);
        }

        public CallCollection GetRecordedCalls()
        {
            var queury = new GraphQLRequest
            {
                Query = @"
{
    calls(sessionId: """ + ProxySession.Id + @""" recorded: true) {
        duration
        recorded
        intercepted
        statusCode
        request {
            content
            headers {
                key
            }
        }
        response {
            content
            headers {
                key
            }
        }
    }
}
"
            };
            var task = GraphQLClient.PostAsync(queury);
            var graphQLResponse = task.Result;
            var calls = graphQLResponse.GetDataFieldAs<Call[]>("calls"); //data->hero is casted as Person
            return new CallCollection(calls);
        }

        public void Close()
        {
            if (ProxySession == null)
                return;
            AdministrationClient.Execute(new RestRequest(
                $"/api/proxies/{ProxySession.Id}",
                Method.DELETE));
        }

        public IRestResponse Call(string path, Method method)
        {
            var request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(ProxySession);
            return AdministrationClient.Execute(request);
        }

        public IRestResponse<T> Call<T>(string path, Method method) where T : new()
        {
            var request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(ProxySession);
            return AdministrationClient.Execute<T>(request);
        }

        public List<Call> GetCalls()
        {
            var queury = new GraphQLRequest
            {
                Query = @"
{
    calls(sessionId: """ + ProxySession.Id + @""") {
        duration
        recorded
        intercepted
        statusCode
        request {
            content
            headers {
                key
            }
        }
        response {
            content
            headers {
                key
            }
        }
    }
}
"
            };
            var graphQLResponse = GraphQLClient.PostAsync(queury).Result;
            var calls = graphQLResponse.GetDataFieldAs<Call[]>("calls"); //data->hero is casted as Person
            return calls.ToList();
        }
    }
}
