using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestApiTester
{
    /// <summary>
    /// A view model for an engine which sends HTTP requests. 
    /// </summary>
    public class RequesterViewModel : ViewModelBase
    {
        public RequesterViewModel()
        {
            this.Responses = new ObservableCollection<ResponseViewModel>();
            this.NewRequestCommand = new RelayCommand(CreateNewRequest);
            this.DefaultRequestValues = new Request()
            {
                Accept = "application/json;q=1.0, text/xml;q=0.5, text/*;q=0.1",
                AuthUsername = string.Empty,
                AuthPassword = string.Empty,
                Headers = new List<HttpHeader>(),
                Parameters = new List<HttpParameter>(),
                Method = "GET",
                Url = new Uri("http://localhost"),
                UserAgent = "ReST API Testing Tool"
            };
        }

        /// <summary>
        /// Raised when the requester wishes to display a request.
        /// </summary>
        public event EventHandler<RequestViewModel> WantRequestView;

        /// <summary>
        /// Raised when the requester wishes to display a response.
        /// </summary>
        public event EventHandler<ResponseViewModel> WantResponseView;

        /// <summary>
        /// Gets an (observable) collection of all responses the engine got.
        /// Note that a response is already created if the request is sent, even if no actual
        /// response has been received - in that case, the response is marked an incomplete.
        /// </summary>
        public ObservableCollection<ResponseViewModel> Responses { get; private set; }

        /// <summary>
        /// A command which, when activated, creates a new request.
        /// Raises <see cref="WantRequestView"/>.
        /// </summary>
        public ICommand NewRequestCommand { get; private set; }

        /// <summary>
        /// Gets a Request instance which defines default values to use when creating a new request.
        /// Initially, this is initialized to some hardcoded values. Whenever a request is issued,
        /// the default values are updated to the latter request.
        /// </summary>
        public Request DefaultRequestValues { get; private set; }

        /// <summary>
        /// Sends the given request and adds a new response to <see cref="Responses"/> which holds
        /// the status of the request (and the response itself, once available).
        /// </summary>
        /// <param name="request">The request to issue.</param>
        public async void Send(Request request)
        {
            // Update default values
            this.DefaultRequestValues = request;

            // Create new response view model and add it to the list
            ResponseViewModel responseViewModel = new ResponseViewModel(this, request);
            responseViewModel.WantResponseView += responseViewModel_WantResponseView;
            responseViewModel.WantRequestView += responseViewModel_WantRequestView;
            this.Responses.Add(responseViewModel);

            // Then actually fetch the response.
            // This method keeps updating the response instance created above.
            await responseViewModel.FetchAsync();
        }

        private void CreateNewRequest(object param)
        {
            RequestViewModel requestViewModel = new RequestViewModel(this);
            if (WantRequestView != null)
                WantRequestView(this, requestViewModel);
        }

        private void responseViewModel_WantResponseView(object sender, EventArgs e)
        {
            // Just delegate the events issued by the ResponseViewModel
            var handler = this.WantResponseView;
            if (handler != null)
                handler(this, (ResponseViewModel)sender);
        }

        void responseViewModel_WantRequestView(object sender, EventArgs e)
        {
            // Just delegate the events issued by the RequestViewModel
            RequestViewModel requestViewModel = ((ResponseViewModel)sender).Request;
            var handler = this.WantRequestView;
            if (handler != null)
                handler(this, requestViewModel);
        }
    }
}