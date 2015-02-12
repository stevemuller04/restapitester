using System.Collections.Generic;
using System.Windows.Media;

namespace RestApiTester
{
    public class HttpMethods
    {
        public static readonly IEnumerable<HttpMethodViewModel> Default = new HttpMethodViewModel[]
        {
            new HttpMethodViewModel("GET", Color.FromRgb(0x50, 0xB3, 0x61)),
            new HttpMethodViewModel("POST", Color.FromRgb(0xF9, 0xC2, 0x37)),
            new HttpMethodViewModel("PUT", Color.FromRgb(0x50, 0x83, 0xDE)),
            new HttpMethodViewModel("PATCH", Color.FromRgb(0xBD, 0x42, 0x77)),
            new HttpMethodViewModel("DELETE", Color.FromRgb(0xEC, 0x1F, 0x1D)),
            new HttpMethodViewModel("OPTIONS", Color.FromRgb(0x66, 0x66, 0x66)),
            new HttpMethodViewModel("HEAD", Color.FromRgb(0x66, 0x66, 0x66)),
            new HttpMethodViewModel("TRACE", Color.FromRgb(0x66, 0x66, 0x66)),
        };
    }
}
