using System;
using System.Collections.Generic;
using System.Net;

namespace RestApiTester
{
    /// <summary>
    /// Represents a HTTP request.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Gets or sets the HTTP method (verb) used to request a resource.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the location of the resource to request.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the value of the HTTP &quot;Accept&quot; header.
        /// Using this the client can specify what content type the server should use to reply.
        /// The format is defined by RFC 2616, section 14.1.
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// Gets or sets the username for basic authentication via HTTP.
        /// Authentication is disabled if this value is null or empty.
        /// </summary>
        public string AuthUsername { get; set; }

        /// <summary>
        /// Gets or sets the password for basic authentication via HTTP.
        /// </summary>
        public string AuthPassword { get; set; }

        /// <summary>
        /// Gets or sets a list of all additional HTTP headers the request specifies.
        /// These do not include protected headers, see <see cref="HttpHeader.ProtectedHttpHeaders"/>.
        /// </summary>
        public List<HttpHeader> Headers { get; set; }

        /// <summary>
        /// Creates a HttpWebRequest instance based on this request.
        /// </summary>
        public HttpWebRequest CreateHttpWebRequest()
        {
            var httpRequest = HttpWebRequest.CreateHttp(this.Url);
            httpRequest.Method = this.Method;
            httpRequest.Accept = this.Accept;
            httpRequest.UserAgent = "ReST API Testing Tool";

            if (!string.IsNullOrEmpty(this.AuthUsername))
                httpRequest.Credentials = new NetworkCredential(this.AuthUsername, this.AuthPassword);

            foreach (var header in this.Headers)
                httpRequest.Headers[header.Name] = header.Value;

            return httpRequest;
        }
    }
}