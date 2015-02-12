using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RestApiTester
{
    /// <summary>
    /// A view model for a request.
    /// </summary>
    public class RequestViewModel : ViewModelBase, IDataErrorInfo
    {
        /// <summary>
        /// Initializes a new reqeust view model.
        /// </summary>
        /// <param name="requesterViewModel">The parent requester view model.</param>
        /// <param name="request">
        /// If specified, the view model will read all properties from it, and will remain in a read-only mode.
        /// Otherwise, the view model loads all properties from the default values (specified by requesterViewModel)
        /// and will remain editable until the request is sent.
        /// </param>
        public RequestViewModel(RequesterViewModel requesterViewModel, Request request = null)
        {
            _requesterViewModel = requesterViewModel;

            if (request == null)
            {
                this.Init(HttpMethods.Default, requesterViewModel.DefaultRequestValues);
                this.IsNew = true;
            }
            else
            {
                this.Init(HttpMethods.Default, request);
                this.IsNew = false;
            }

            this.SendCommand = new RelayCommand(
                _ => this.Send(),
                _ => this.IsNew && this.IsInputValid);
        }

        private RequesterViewModel _requesterViewModel;
        private HttpMethodViewModel _method;
        private string _url, _accept, _authUsername, _authPassword;
        private ObservableCollection<HttpHeader> _headers;

        /// <summary>
        /// Raised when the view model wants the associated window to be closed.
        /// This is used when the request is sent.
        /// </summary>
        public event EventHandler WantClose;

        /// <summary>
        /// Gets or sets the HTTP method (verb) used to request a resource.
        /// </summary>
        public HttpMethodViewModel Method
        {
            get { return _method; }
            set
            {
                _method = value;
                OnPropertyChanged("Method");
            }
        }

        /// <summary>
        /// Gets or sets the location of the resource to request.
        /// The property will be marked as erroneous if this is not a valid http: or https: URI.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged("Url");
            }
        }

        /// <summary>
        /// Gets or sets the value of the HTTP &quot;Accept&quot; header.
        /// Using this the client can specify what content type the server should use to reply.
        /// The format is defined by RFC 2616, section 14.1.
        /// </summary>
        public string Accept
        {
            get { return _accept; }
            set
            {
                _accept = value;
                OnPropertyChanged("Accept");
            }
        }

        /// <summary>
        /// Gets or sets the username for basic authentication via HTTP.
        /// Authentication is disabled if this value is null or empty.
        /// </summary>
        public string AuthUsername
        {
            get { return _authUsername; }
            set
            {
                _authUsername = value;
                OnPropertyChanged("AuthUsername");
            }
        }

        /// <summary>
        /// Gets or sets the password for basic authentication via HTTP.
        /// </summary>
        public string AuthPassword
        {
            get { return _authPassword; }
            set
            {
                _authPassword = value;
                OnPropertyChanged("AuthPassword");
            }
        }

        /// <summary>
        /// Gets or sets a list of all additional HTTP headers the request specifies.
        /// These do not include protected headers, see <see cref="HttpHeader.ProtectedHttpHeaders"/>.
        /// </summary>
        public ObservableCollection<HttpHeader> Headers
        {
            get { return _headers; }
            set
            {
                _headers = value;
                OnPropertyChanged("Headers");
            }
        }

        /// <summary>
        /// Gets the command responsible for sending the request.
        /// </summary>
        public ICommand SendCommand { get; private set; }

        /// <summary>
        /// Indicates whether this request is new.
        /// A request is new iff it has not been sent yet.
        /// </summary>
        public bool IsNew { get; private set; }

        /// <summary>
        /// Indicates whether the view model is in read-only mode.
        /// </summary>
        public bool IsReadOnly
        {
            get { return !this.IsNew; }
        }

        /// <summary>
        /// Checks whether the given input is semantically valid.
        /// </summary>
        public bool IsInputValid
        {
            get
            {
                if (this.Method == null) return false;
                if (!Uri.IsWellFormedUriString(this.Url, UriKind.Absolute)) return false;

                Uri uri = new Uri(this.Url, UriKind.Absolute);
                if (uri.Scheme != "http" && uri.Scheme != "https") return false;

                foreach (var header in this.Headers)
                {
                    if (!string.IsNullOrEmpty(header["Name"]))
                        return false;
                }

                return true;
            }
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
                    case "Method":
                        if (this.Method == null)
                            error = "Method is required";
                        break;
                    case "Url":
                        if (!Uri.IsWellFormedUriString(this.Url, UriKind.Absolute))
                            error = "Invalid URL";
                        Uri uri = new Uri(this.Url, UriKind.Absolute);
                        if (uri.Scheme != "http" && uri.Scheme != "https")
                            error = "Only http: and http: URIs are supported.";
                        break;
                }
                return error;
            }
        }

        /// <summary>
        /// Creates an associated <see cref="Request"/> instance encoding the same value as this view model.
        /// </summary>
        public Request CreateModel()
        {
            return new Request()
            {
                Accept = this.Accept,
                AuthPassword = this.AuthPassword,
                AuthUsername = this.AuthUsername,
                Headers = this.Headers.ToList(),
                Method = this.Method.Name,
                Url = new Uri(this.Url, UriKind.Absolute)
            };
        }

        /// <summary>
        /// Initializes the values of this view model by reading them off a given <see cref="Request"/> instance.
        /// </summary>
        /// <param name="httpMethods">A collection of HTTP Method view models. This is required to get the associated view model - note that the Request instance only knows the verb itself.</param>
        /// <param name="request">The request instance to read all values off.</param>
        private void Init(IEnumerable<HttpMethodViewModel> httpMethods, Request request)
        {
            this.Method = httpMethods.Where(x => x.Name == request.Method).First();
            this.Url = request.Url.ToString();
            this.Accept = request.Accept;
            this.AuthUsername = request.AuthUsername;
            this.AuthPassword = request.AuthPassword;
            this.Headers = new ObservableCollection<HttpHeader>(request.Headers);
        }

        /// <summary>
        /// Sends the request.
        /// This method also raises <see cref="WantClose"/> and unmarks this request from being 'new'.
        /// </summary>
        private void Send()
        {
            var handler = this.WantClose;
            if (handler != null)
                handler(this, new EventArgs());

            this.IsNew = false;
            _requesterViewModel.Send(this.CreateModel());
        }
    }
}