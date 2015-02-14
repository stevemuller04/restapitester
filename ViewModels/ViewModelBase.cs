/*
Copyright © 2015 Steve Muller <steve.muller@outlook.com>
This file is subject to the license terms in the LICENSE file found in the top-level directory of
this distribution and at http://github.com/stevemuller04/restapitester/blob/master/LICENSE
*/

using System.ComponentModel;

namespace RestApiTester
{
    /// <summary>
    /// A base class for a view model.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property which has changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var e = this.PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}