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
using System.Windows.Shapes;

namespace DzinniczekWPF
{
    /// <summary>
    /// Logika interakcji dla klasy AddSchool.xaml
    /// </summary>
    public partial class AddSchool : Window
    {
        public AddSchool()
        {
            InitializeComponent();
        }
        public AddSchool(Array Schools)
        {
            
        }

        private void btn_AddSchool_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            ((MainWindow)Application.Current.MainWindow).addedSchool = txt_AddedSchool.Text;

            this.Close();
        }

        private void AddSchool_Load(object sender, RoutedEventArgs e)
        {

        }
    }
}
