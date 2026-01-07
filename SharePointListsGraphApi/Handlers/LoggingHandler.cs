using System.Net.Http;

namespace SharePointListsGraphApi.Handlers
{
    internal class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // Log the request
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[DEBUG] HTTP {request.Method}: {request.RequestUri}");
            Console.ResetColor();

            // Optionally log headers
            // Console.WriteLine($"[DEBUG] Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");

            var response = await base.SendAsync(request, cancellationToken);

            // Log the response status
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[DEBUG] Response: {(int)response.StatusCode} {response.StatusCode}");
            Console.ResetColor();

            return response;
        }
    }
}
