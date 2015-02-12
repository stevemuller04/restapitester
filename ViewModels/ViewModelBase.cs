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