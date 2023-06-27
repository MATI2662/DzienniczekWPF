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
    /// Logika interakcji dla klasy AddClass.xaml
    /// </summary>
    public partial class AddClass : Window
    {
        public AddClass()
        {
            InitializeComponent();
        }

        private void btn_AddClass_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            ((MainWindow)Application.Current.MainWindow).addedClass = txt_AddedClass.Text;

            this.Close();
        }
    }
}
