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