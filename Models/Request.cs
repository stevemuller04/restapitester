using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;

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
        /// Gets or sets the user agent string identifying the software accessing the API.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a list of all additional HTTP headers the request specifies.
        /// These do not include protected headers, see <see cref="HttpHeader.ProtectedHttpHeaders"/>.
        /// </summary>
        public List<HttpHeader> Headers { get; set; }

        /// <summary>
        /// Gets or sets a list of all parameters passed to the requested resource.
        /// How these are transfered largely depends on the HTTP method chosen.
        /// </summary>
        public List<HttpParameter> Parameters { get; set; }

        /// <summary>
        /// Creates a HttpWebRequest instance based on this request.
        /// </summary>
        public async Task<HttpWebRequest> CreateHttpWebRequestAsync()
        {
            // There are two ways to specify parameters: either via the query part of the URL, or in the request body.
            // First append the respective parameters to the URL.
            UriBuilder urlWithParameters = new UriBuilder(this.Url);

            // The UriBuilder.Query property is a bit weird. According to MSDN,
            // it returns the leading question mark, but the latter should not be
            // specified when setting the property.
            var parametersViaQuery = this.Parameters.Where(x => !x.ViaRequestBody);
            if (parametersViaQuery.Count() > 0)
            {
                string queryToAppend = this.CreateQueryStringFromParameters(parametersViaQuery);
                if (this.Url.Query != null && this.Url.Query.Length > 1)
                    urlWithParameters.Query = this.Url.Query.Substring(1) + "&" + queryToAppend;
                else
                    urlWithParameters.Query = queryToAppend;
            }

            var httpRequest = HttpWebRequest.CreateHttp(urlWithParameters.Uri);
            httpRequest.Method = this.Method;
            httpRequest.Accept = this.Accept;
            httpRequest.UserAgent = this.UserAgent;

            // Add credentials only when a username has been specified
            if (!string.IsNullOrEmpty(this.AuthUsername))
                httpRequest.Credentials = new NetworkCredential(this.AuthUsername, this.AuthPassword);

            // Add additional headers
            foreach (var header in this.Headers)
                httpRequest.Headers[header.Name] = header.Value;

            // Specify remaining parameters in the request body
            var parametersViaRequestBody = this.Parameters.Where(x => x.ViaRequestBody);
            if (parametersViaRequestBody.Count() > 0)
            {
                httpRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                string requestBody = this.CreateQueryStringFromParameters(parametersViaRequestBody);

                using (var httpRequestStream = await httpRequest.GetRequestStreamAsync())
                using (var streamWriter = new StreamWriter(httpRequestStream, System.Text.Encoding.UTF8))
                {
                    await streamWriter.WriteAsync(requestBody);
                }
            }

            return httpRequest;
        }

        /// <summary>
        /// Builds a query string (excluding the leading question mark) based on the given parameters.
        /// </summary>
        private string CreateQueryStringFromParameters(IEnumerable<HttpParameter> parameters)
        {
            StringBuilder query = new StringBuilder();
            foreach (var parameter in parameters)
            {
                // Add separator except for the first parameter
                if (query.Length != 0)
                    query.Append("&");

                // WebUtility.UrlEncode() internally uses UTF-8 to encode, which is fine
                query.Append(WebUtility.UrlEncode(parameter.Name));
                query.Append("=");
                query.Append(WebUtility.UrlEncode(parameter.Value));
            }
            return query.ToString();
        }
    }
}