using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestApiTester
{
    /// <summary>
    /// Interaktionslogik für HttpMethodComboBox.xaml
    /// </summary>
    public partial class HttpMethodComboBox : UserControl
    {
        public HttpMethodComboBox()
        {
            InitializeComponent();
        }

        // Also create a .SelectedItem property for this user control.
        // In XAML, we will bind it to the .SelectedItem property of the combobox.

        public static readonly DependencyProperty SelectedItemProperty =
            ComboBox.SelectedItemProperty.AddOwner(typeof(HttpMethodComboBox), new FrameworkPropertyMetadata()
            {
                // Make sure the property is Mode=TwoWay by default, as it is the case for ComboBox.SelectedItem
                BindsTwoWayByDefault = true
            });

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

    }
}
