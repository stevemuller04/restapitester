using System.Windows.Media;

namespace RestApiTester
{
    /// <summary>
    /// A view model for a selectable HTTP method.
    /// </summary>
    public class HttpMethodViewModel
    {
        public HttpMethodViewModel(string name, Color color)
        {
            this.Name = name;
            this.Color = new SolidColorBrush(color);
        }

        /// <summary>
        /// The name of the method. Common examples are GET or POST.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The color to use when displaying this method.
        /// </summary>
        public Brush Color { get; private set; }
    }
}
