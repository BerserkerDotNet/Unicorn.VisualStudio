using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicorn.Emulator
{
    internal class UnicornResponse
    {
        public UnicornResponse()
        {
            Headers = new Dictionary<string, string>();
            Body = new List<string>();
        }

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<string> Body { get; set; }
        public bool IsComplete => Body.Count == 0;
        public bool HasSentHeaders { get; set; }

        public string RenderResponse()
        {
            if (IsComplete && HasSentHeaders)
                return string.Empty;

            var sb = new StringBuilder();
            if (!HasSentHeaders)
                WriteHeaders(sb);
            sb.Append(GetBodyChunk());

            return sb.ToString();
        }

        private string GetBodyChunk()
        {
            if (Body.Count ==0)
                return string.Empty;

            var result = Body.ElementAt(0);
            Body.RemoveAt(0);

            return result;
        }

        private void WriteHeaders(StringBuilder sb)
        {
            sb.AppendLine($"HTTP/1.1 {StatusCode} {StatusMessage}");
            foreach (var header in Headers)
                sb.AppendLine($"{header.Key}: {header.Value}");
            sb.AppendLine($"Date: {DateTime.Now.ToString("R")}");
            sb.AppendLine("Connection: keep-alive");
            sb.AppendLine("Content-Type: text/plain; charset=UTF-8");
            sb.AppendLine($"Content-Length: {Body.Sum(x => x.Length)}");
            sb.AppendLine();
            HasSentHeaders = true;
        }

        public static UnicornResponse CreateUnauthorized()
        {
            return new UnicornResponse {StatusCode = 401, StatusMessage = "Unauthorized"};
        }

        public static UnicornResponse CreateOK(string body)
        {
            var unicornResponse = new UnicornResponse {StatusCode = 200, StatusMessage = "OK"};
            unicornResponse.Body.Add(body);
            return unicornResponse;
        }

        public static UnicornResponse CreateNotFound()
        {
            return new UnicornResponse {StatusCode = 404, StatusMessage = "Not Found"};
        }

        public static UnicornResponse CreateInternalServerError()
        {
            return new UnicornResponse {StatusCode = 500, StatusMessage ="Internal Server Error"};
        }

        public static UnicornResponse CreateChunked()
        {
            return new UnicornResponse { StatusCode = 206, StatusMessage = "Partial Content" };
        }
    }
}