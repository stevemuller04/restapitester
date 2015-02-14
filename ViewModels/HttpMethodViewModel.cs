/*
Copyright © 2015 Steve Muller <steve.muller@outlook.com>
This file is subject to the license terms in the LICENSE file found in the top-level directory of
this distribution and at http://github.com/stevemuller04/restapitester/blob/master/LICENSE
*/

using System.Windows.Media;

namespace RestApiTester
{
    /// <summary>
    /// A view model for a selectable HTTP method.
    /// </summary>
    public class HttpMethodViewModel
    {
        public HttpMethodViewModel(string name, Color color, string description)
        {
            this.Name = name;
            this.Color = new SolidColorBrush(color);
            this.Description = description;
        }

        /// <summary>
        /// The name of the method. Common examples are GET or POST.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The color to use when displaying this method.
        /// </summary>
        public Brush Color { get; private set; }

        /// <summary>
        /// A description of the method which explains why it is used for.
        /// </summary>
        public string Description { get; private set; }
    }
}
