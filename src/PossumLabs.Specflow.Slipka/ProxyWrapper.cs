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
                    Headers = new List<Header>()
                }
            };
            call.Response.Headers.Add(new Header(type, new List<string>() { value }));
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            AdministrationClient.Execute(request);
        }

        private void Execute(RestRequest request)
        {
            var response = AdministrationClient.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new InvalidOperationException(response.StatusCode.ToString());
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
            Execute(request);
        }

        public void RegisterTag(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/tag", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            Execute(request);
        }

        public void RegisterRecording(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/record", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            Execute(request);
        }

        public void RegisterInject(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/inject", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            Execute(request);
        }

        public void RegisterDecoration(Header header)
        {
            var request = new RestRequest($"/api/proxies/{ProxySession.Id}/decorate", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(header);
            Execute(request);
        }

        public Session GetSession()
        {
            var paramaters = $"sessionId: \"{ProxySession.Id}\"";
            var queury = new GraphQLRequest
            {
                Query = @"
{
    session("+ paramaters + @") {
    id
    name
    targetPort
    tags
    calls {
        recorded
        response {
            content
            }
        }
    }
}
"
            };
            var task = GraphQLClient.PostAsync(queury);
            var graphQLResponse = task.Result;
            return graphQLResponse.GetDataFieldAs<Session>("session"); 
        }

        public CallCollection GetCalls(bool? recorded = null, string tag = null)
        {
            var paramaters = $"sessionId: \"{ProxySession.Id}\"";
            if (recorded.HasValue)
                paramaters += $" recorded: {recorded.ToString().ToLower()}";
            if (tag != null)
                paramaters += $" tag: \"{tag}\"";
            var queury = new GraphQLRequest
            {
                Query = @"
{
    calls(" + paramaters + @") {
        duration
        recorded
        injected
        statusCode
        path
        uri
        tags
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
            var calls = graphQLResponse.GetDataFieldAs<CallRecord[]>("calls");
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

        public byte[] DownloadRequest(int number)
            => AdministrationClient.DownloadData(new RestRequest($"/api/sessions/{ProxySession.Id}/request/{number}"));

        public byte[] DownloadResponse(int number)
            => AdministrationClient.DownloadData(new RestRequest($"/api/sessions/{ProxySession.Id}/response/{number}"));
    }
}
