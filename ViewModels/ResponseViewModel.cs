﻿/*
Copyright © 2015 Steve Muller <steve.muller@outlook.com>
This file is subject to the license terms in the LICENSE file found in the top-level directory of
this distribution and at http://github.com/stevemuller04/restapitester/blob/master/LICENSE
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestApiTester
{
    /// <summary>
    /// A view model for a HTTP response.
    /// </summary>
    public class ResponseViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new response view model.
        /// </summary>
        /// <param name="requesterViewModel">The parent requester view model.</param>
        /// <param name="request">The request which precedes the response.</param>
        public ResponseViewModel(RequesterViewModel requesterViewModel, Request request)
        {
            _requesterViewModel = requesterViewModel;
            _request = request;
            _requestViewModel = new RequestViewModel(requesterViewModel, request);
            this.ShowRequestCommand = new RelayCommand(_ => this.ShowRequest());
            this.ShowResponseCommand = new RelayCommand(_ => this.ShowResponse(), _ => this.IsCompleted);
        }

        private RequesterViewModel _requesterViewModel;
        private Request _request;
        private RequestViewModel _requestViewModel;
        private bool _isCompleted = false;
        private string _statusCode = "...", _statusString = string.Empty, _statusIcon = string.Empty;
        private string _contentType = string.Empty, _content = string.Empty, _contentLength = "n/a", _summary = string.Empty;
        private List<HttpHeader> _headers = new List<HttpHeader>();

        /// <summary>
        /// Raised when the view model wishes to display a response.
        /// </summary>
        public event EventHandler WantResponseView;

        /// <summary>
        /// Raised when the view model wishes to display a request.
        /// </summary>
        public event EventHandler WantRequestView;

        /// <summary>
        /// Gets a view model of the underlying request.
        /// </summary>
        public RequestViewModel Request { get { return _requestViewModel; } }

        /// <summary>
        /// A command which, when activated, shows the underlying request.
        /// </summary>
        public ICommand ShowRequestCommand { get; private set; }

        /// <summary>
        /// A command which, when activated, shows the response details.
        /// </summary>
        public ICommand ShowResponseCommand { get; private set; }

        /// <summary>
        /// Indicates whether this response is complete.
        /// A response is considered as complete when either the server replied something,
        /// or the connection has been closed due to some error (including timeout).
        /// </summary>
        public bool IsCompleted
        {
            get { return _isCompleted; }
            protected set
            {
                _isCompleted = value;
                OnPropertyChanged("IsCompleted");
            }
        }

        /// <summary>
        /// Gets the URI to an icon visualizing the response status.
        /// Possible states are 'success', 'error', and 'incomplete'.
        /// </summary>
        public string StatusIcon
        {
            get { return _statusIcon; }
            protected set
            {
                _statusIcon = "pack://application:,,,/Resources/" + value + ".png";
                OnPropertyChanged("StatusIcon");
            }
        }

        /// <summary>
        /// Gets a status code describing the result of the request.
        /// This correlates with the HTTP status code, unless a connection error occurred
        /// (in which case the value is undefined).
        /// </summary>
        public string StatusCode
        {
            get { return _statusCode; }
            protected set
            {
                _statusCode = value;
                OnPropertyChanged("StatusCode");
            }
        }

        /// <summary>
        /// Gets a status string describing the result of the request.
        /// If an error occurred, this string contains further information.
        /// Otherwise, this string correlates with a description of the HTTP status code.
        /// </summary>
        public string StatusString
        {
            get { return _statusString; }
            protected set
            {
                _statusString = value;
                OnPropertyChanged("StatusString");
            }
        }

        /// <summary>
        /// Gets a collection of all HTTP headers returned along with the response.
        /// </summary>
        public IEnumerable<HttpHeader> Headers
        {
            get { return _headers; }
            protected set
            {
                _headers = value.ToList();
                OnPropertyChanged("Headers");
            }
        }

        /// <summary>
        /// Gets the content type of the response, as specified by the &quot;Content-Type&quot; header.
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            protected set
            {
                _contentType = value;
                OnPropertyChanged("ContentType");
            }
        }

        /// <summary>
        /// Gets the response body as a string, which was interpreted using the encoding
        /// specified by the server (if the server did not specify one, ASCII is used).
        /// </summary>
        public string Content
        {
            get { return _content; }
            protected set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        /// <summary>
        /// Gets the content length of the response in a human-readable format.
        /// Note that the server might not have specified the related header, in which case this value is not determined.
        /// </summary>
        /// <param name="bytes">The number of bytes.</param>
        public string ContentLengthString
        {
            get { return _contentLength; }
            protected set
            {
                _contentLength = value;
                OnPropertyChanged("ContentLengthString");
            }
        }

        /// <summary>
        /// Gets a short summary of the response body.
        /// Includes content-type and content length, if specified.
        /// </summary>
        public string Summary
        {
            get { return _summary; }
            protected set
            {
                _summary = value;
                OnPropertyChanged("Summary");
            }
        }

        /// <summary>
        /// Fetches the response and updates any properties of this instance on the way.
        /// </summary>
        public async Task FetchAsync()
        {
            this.StatusCode = "...";
            this.StatusString = "Connecting to server ...";
            this.StatusIcon = "working";

            var httpRequest = await _request.CreateHttpWebRequestAsync();
            HttpWebResponse httpResponse;

            try
            {
                httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync();
                this.StatusIcon = "success";
            }
            catch (WebException ex)
            {
                this.StatusIcon = "error";
                if (ex.Response is HttpWebResponse)
                {
                    httpResponse = (HttpWebResponse)ex.Response;
                }
                else
                {
                    httpResponse = null;
                    this.StatusCode = "Err";
                    this.StatusString = ex.Message;
                }
            }

            if (httpResponse != null)
                await this.UpdateFromResponseAsync(httpResponse);

            this.IsCompleted = true;
        }

        /// <summary>
        /// Updates the properties of this instance based on the HTTP response returned.
        /// </summary>
        private async Task UpdateFromResponseAsync(HttpWebResponse httpResponse)
        {
            this.StatusCode = ((int)httpResponse.StatusCode).ToString();
            this.StatusString = httpResponse.StatusDescription;
            this.ContentType = httpResponse.ContentType;
            this.Summary = httpResponse.ContentType + ", fetching response body...";
            this.Headers = HttpHeader.FromWebHeaderCollection(httpResponse.Headers);

            // Extract character encoding from "Content-Type" header, if specified.
            // Otherwise use ASCII.
            Encoding encoding;
            try
            {
                Match m = Regex.Match(httpResponse.ContentType, ";charset=([^()<>@,;:\"\\\\/\\[\\]?={} \\t]+)");
                if (m.Success)
                    encoding = Encoding.GetEncoding(m.Groups[1].Value);
                else
                    encoding = Encoding.ASCII;
            }
            catch (ArgumentException)
            {
                encoding = Encoding.ASCII;
            }

            // Read the response body
            using (var httpResponseStream = httpResponse.GetResponseStream())
            using (var streamReader = new StreamReader(httpResponseStream, encoding))
            {
                this.Content = await streamReader.ReadToEndAsync();
            }

            // Make a short summary string of the response body
            if (httpResponse.ContentLength < 0)
            {
                this.ContentLengthString = "n/a";
                this.Summary = httpResponse.ContentType;
            }
            else
            {
                var binarySize = this.GetHumanReableBinarySize(httpResponse.ContentLength);
                this.ContentLengthString = string.Format("{0:0,000} bytes ({1})", httpResponse.ContentLength, binarySize);
                this.Summary = string.Format("{0}  ({1})", httpResponse.ContentType, binarySize);
            }
        }

        /// <summary>
        /// Raises the <see cref="WantRequestView"/> event.
        /// </summary>
        private void ShowRequest()
        {
            var handler = this.WantRequestView;
            if (handler != null)
                handler(this, new EventArgs());
        }

        /// <summary>
        /// Raises the <see cref="WantResponseView"/> event.
        /// </summary>
        private void ShowResponse()
        {
            var handler = this.WantResponseView;
            if (handler != null)
                handler(this, new EventArgs());
        }

        /// <summary>
        /// Gets a string which represents the given number of bytes in a human-readable format.
        /// </summary>
        /// <param name="bytes">The number of bytes.</param>
        private string GetHumanReableBinarySize(long bytes)
        {
            double val = bytes;
            Queue<string> units = new Queue<string>(new string[] { "bytes", "KiB", "MiB", "GiB" });
            while (units.Count >= 2 && val >= 1000)
            {
                units.Dequeue();
                val /= 1024;
            }
            return string.Format("{0:0.##} {1}", val, units.Dequeue());
        }
    }
}