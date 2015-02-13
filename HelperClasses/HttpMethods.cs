using System.Collections.Generic;
using System.Windows.Media;

namespace RestApiTester
{
    public class HttpMethods
    {
        public static readonly IEnumerable<HttpMethodViewModel> Default = new HttpMethodViewModel[]
        {
            new HttpMethodViewModel("GET", Color.FromRgb(0x50, 0xB3, 0x61), "Retrieves a resource without modifying anything on the server."),
            new HttpMethodViewModel("POST", Color.FromRgb(0xF9, 0xC2, 0x37), "Creates a new resource somewhere on the server (generally not at the given URL). Usually, the server responds with the URL of the created resource in the \"Location\" header.  POST requests are also used to generally modify something."),
            new HttpMethodViewModel("PUT", Color.FromRgb(0x50, 0x83, 0xDE), "Creates a resource at the specified URL (in contrast to POST). This may or may not have side effects at different locations: for instance, a parent resource might not now contain a reference to the former."),
            new HttpMethodViewModel("PATCH", Color.FromRgb(0xBD, 0x42, 0x77), "Updates the resource at the specified URL. This may or may not have side effects at different locations: for instance, a log entry recording the modification might have been created."),
            new HttpMethodViewModel("DELETE", Color.FromRgb(0xEC, 0x1F, 0x1D), "Deletes the resource at the specified URL. This may or may not have side effects at different location: for instance, any child resources might have been deleted as well."),
            new HttpMethodViewModel("OPTIONS", Color.FromRgb(0x66, 0x66, 0x66), "Checks which HTTP methods (verbs) are supported at the given URL. The server replies with an \"Allow\" header containing the latter."),
            new HttpMethodViewModel("HEAD", Color.FromRgb(0x66, 0x66, 0x66), "Equivalent to a GET request, but the server does not send the response body."),
            new HttpMethodViewModel("TRACE", Color.FromRgb(0x66, 0x66, 0x66), "Used for debugging purposes. Tells the server to send the incoming HTTP request back to the client."),
        };
    }
}
