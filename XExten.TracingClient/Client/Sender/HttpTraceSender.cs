using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExten.TracingClient.Client.DataContract;
using XExten.XCore;

namespace XExten.TracingClient.Client.Sender
{
    public class HttpTraceSender : IHttpTraceSender
    {
        protected const string spanUrl = "/api/span";
        protected readonly HttpClient _httpClient;
        public HttpTraceSender(string collectorUrl) : this(new HttpClient(new HttpClientHandler() { UseProxy = false }), collectorUrl)
        {

        }
        public HttpTraceSender(HttpClient httpClient, string collectorUrl)
        {
            if (collectorUrl == null)
            {
                throw new ArgumentNullException(nameof(collectorUrl));
            }

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(collectorUrl);
        }
        public Task SendSpanAsync(Span[] spans, CancellationToken cancellationToken = default)
        {
            if (spans == null)
            {
                throw new ArgumentNullException(nameof(spans));
            }

            var content = new ByteArrayContent(spans.ToMsgByte());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-msgpack");
            return _httpClient.PostAsync(spanUrl, content, cancellationToken);
        }
    }
}
