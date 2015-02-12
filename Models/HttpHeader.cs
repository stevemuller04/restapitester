using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows.Input;

namespace RestApiTester
{
    /// <summary>
    /// Represents a HTTP header and value tuple.
    /// </summary>
    public class HttpHeader : IDataErrorInfo
    {
        public HttpHeader()
            : this(string.Empty, string.Empty)
        {
        }

        public HttpHeader(string header, string value)
        {
            this.Name = header;
            this.Value = value;
        }

        /// <summary>
        /// A collection of all protected HTTP headers.
        /// Any header in this collection cannot be explicitly set on a WebHeaderCollection.
        /// </summary>
        public static readonly string[] ProtectedHttpHeaders = { "Accept", "Connection", "Content-Length", "Content-Type", "Date", "Expect", "Host", "If-Modified-Since", "Range", "Referer", "Transfer-Encoding", "User-Agent", "Proxy-Connection" };

        /// <summary>
        /// Gets or sets the name of the HTTP header.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the HTTP header.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Converts the given WebHeaderCollection into a enumerable of HttpHeader instances.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<HttpHeader> FromWebHeaderCollection(WebHeaderCollection collection)
        {
            return collection.AllKeys.Select(x => new HttpHeader(x, collection[x]));
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case "Name":
                        if (Array.Exists(ProtectedHttpHeaders, x => x.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase)))
                            error = "The HTTP header '" + this.Name + "' is protected and cannot be set explicitly.";
                        break;
                }
                return error;
            }
        }
    }
}