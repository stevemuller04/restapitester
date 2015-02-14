/*
Copyright © 2015 Steve Muller <steve.muller@outlook.com>
This file is subject to the license terms in the LICENSE file found in the top-level directory of
this distribution and at http://github.com/stevemuller04/restapitester/blob/master/LICENSE
*/

using System.ComponentModel;

namespace RestApiTester
{
    /// <summary>
    /// Represents a parameter passed to a resource via HTTP.
    /// How the parameter is actally transfered largely depends on the HTTP method chosen.
    /// </summary>
    public class HttpParameter : IDataErrorInfo
    {
        public HttpParameter()
            : this(string.Empty, string.Empty, false)
        {
        }

        public HttpParameter(string name, string value, bool viaRequestBody)
        {
            this.Name = name;
            this.Value = value;
            this.ViaRequestBody = viaRequestBody;
        }

        /// <summary>
        /// Gets or sets the name of this parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of this parameter.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Indicates whether this parameter is sent via the request body as application/x-www-form-urlencoded.
        /// Otherwise it is appended to the URL (in the query part).
        /// </summary>
        public bool ViaRequestBody { get; set; }

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
                        if (string.IsNullOrWhiteSpace(this.Name))
                            error = "Empty parameter names are not allowed.";
                        break;
                }
                return error;
            }
        }
    }
}