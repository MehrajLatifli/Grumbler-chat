using Grumbler_chat__Client_.View_Models;
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

namespace Grumbler_chat__Client_.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Service_UC.xaml
    /// </summary>
    public partial class Service_UC : UserControl
    {
        public Service_UC()
        {
            InitializeComponent();

            var vm = new Service_UCViewModel() { Service_UCs = this };
            this.DataContext = vm;
        }
    }
}
