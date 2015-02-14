/*
Copyright © 2015 Steve Muller <steve.muller@outlook.com>
This file is subject to the license terms in the LICENSE file found in the top-level directory of
this distribution and at http://github.com/stevemuller04/restapitester/blob/master/LICENSE
*/

using System.Windows;
using System.Windows.Controls;

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
