/*
Copyright © 2015 Steve Muller <steve.muller@outlook.com>
This file is subject to the license terms in the LICENSE file found in the top-level directory of
this distribution and at http://github.com/stevemuller04/restapitester/blob/master/LICENSE
*/

using System.Windows;

namespace RestApiTester
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var rvm = new RequesterViewModel();
            rvm.WantRequestView += rvm_WantRequestView;
            rvm.WantResponseView += rvm_WantResponseView;
            this.DataContext = rvm;
        }

        private void rvm_WantRequestView(object sender, RequestViewModel requestViewModel)
        {
            RequestWindow w = new RequestWindow();
            w.DataContext = requestViewModel;
            w.Owner = this;
            w.Show();

            requestViewModel.WantClose += delegate { w.Close(); };
        }

        private void rvm_WantResponseView(object sender, ResponseViewModel responseViewModel)
        {
            ResponseWindow w = new ResponseWindow();
            w.DataContext = responseViewModel;
            w.Owner = this;
            w.Show();
        }
    }
}