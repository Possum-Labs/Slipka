using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Slipka.ValueObjects;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Slipka
{
    public class ProxyWrapper:IDomainObject
    {
        public ProxyWrapper(Uri host, Uri destination):this(host)
        {
            Open(destination);
        }

        public ProxyWrapper(Uri host)
        {
            _administrationUri = host;
            _administrationClient = new RestClient(host);
        }

        private readonly Uri _administrationUri;
        private  Uri _proxyUri;
        private readonly RestClient _administrationClient;
        private RestClient _proxyClient;
        private SessionSummary _proxySession;

        public Uri ProxyUri { get => _proxyUri; }

        public void Open(Uri destination)
        {
            _proxySession = new SessionSummary();
            _proxySession.TargetHost = destination.Host;
            _proxySession.TargetPort = destination.Port;

            var request = new RestRequest("/api/proxies", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(_proxySession);
            var response = _administrationClient.Execute<SessionSummary>(request);
            if (!response.IsSuccessful)
                throw new Exception($"Was unable to open the proxy, error was {response.StatusCode} {response.StatusDescription}");
            _proxySession = response.Data;
            _proxyUri = new Uri($"http://{_administrationUri.Host}:{_proxySession.ProxyPort}");
            _proxyClient = new RestClient(_proxyUri);
        }

        public void LogsResponsesOfType(string type, string value)
        {
            var request = new RestRequest($"/api/proxies/{_proxySession.Id}/record", Method.PUT);
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
            _administrationClient.Execute(request);
        }

        public void LogsCallsTo(Uri uri)
        {
            var request = new RestRequest($"/api/proxies/{_proxySession.Id}/record", Method.PUT);
            var call = new Call
            {
                Uri = uri
            };
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            _administrationClient.Execute(request);
        }

        public void RegisterTag(CallTemplate call, string tag)
        {
            var request = new RestRequest($"/api/proxies/{_proxySession.Id}/tag/{tag}", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            _administrationClient.Execute(request);
        }

        public void RegisterRecording(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{_proxySession.Id}/record", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            _administrationClient.Execute(request);
        }

        public void RegisterIntercept(CallTemplate call)
        {
            var request = new RestRequest($"/api/proxies/{_proxySession.Id}/intercept", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(call);
            _administrationClient.Execute(request);
        }

        public CallCollection GetRecordedCalls()
        {
            var ret = new CallCollection();

            var request = new RestRequest($"/api/proxies/{_proxySession.Id}", Method.GET);
            request.RequestFormat = DataFormat.Json;
            var results = _administrationClient.Execute<SessionSummary>(request);

            ret.AddRange(results.Data.Calls.Where(c=>c.Recorded));

            return ret;
        }

        public void Close()
        {
            if (_proxySession == null)
                return;
            _administrationClient.Execute(new RestRequest(
                $"/api/proxies/{_proxySession.Id}",
                Method.DELETE));
        }

        public IRestResponse Call(string path, Method method)
        {
            var request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(_proxySession);
            return _administrationClient.Execute(request);
        }

        public IRestResponse<T> Call<T>(string path, Method method) where T : new()
        {
            var request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(_proxySession);
            return _administrationClient.Execute<T>(request);
        }

        public List<Call> GetCalls()
        {
            var response = _administrationClient.Execute<SessionSummary>(new RestRequest($"/api/sessions/{_proxySession.Id}"));
            var session = response.Data;
            return session.Calls;
        }
    }
}
