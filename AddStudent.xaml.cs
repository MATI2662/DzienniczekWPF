using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logika interakcji dla klasy AddStudent.xaml
    /// </summary>
    public partial class AddStudent : Window
    {
        string name;
        string surname;
        int age;
        bool sex;
        bool canSend = true;
        int ignoreme;

        public AddStudent()
        {
            InitializeComponent();
        }

        private void rad_Boy_Checked(object sender, RoutedEventArgs e)
        {
            if (rad_Boy.IsChecked == false && rad_Girl.IsChecked == false)
            {
                canSend = false;
            }
            else { 
                sex = true;
                canSend = true;
            }
        }

        private void rad_Girl_Checked(object sender, RoutedEventArgs e)
        {
            if (rad_Boy.IsChecked == false && rad_Girl.IsChecked == false)
            {
                canSend = false;
            }
            else
            {
                sex = false;
                canSend = true;
            }
        }

        private void btn_AddStudent_Click(object sender, RoutedEventArgs e)
        {
            string x = txt_Name.Text.Trim();

            string[] parts = x.Split(' ');
            if (parts.Length == 2)
            {
                name = parts[0];
                surname = parts[1];
            }
            else 
            {
                canSend = false;
            }

            if (int.TryParse(txt_Age.Text, out ignoreme))
            {
                age = int.Parse(txt_Age.Text);
            }
            else
            {
                canSend = false;
            }

            if (canSend == true) {
                MainWindow mainWindow = new MainWindow();
                ((MainWindow)Application.Current.MainWindow).addedStudent[0] = name;
                ((MainWindow)Application.Current.MainWindow).addedStudent[1] = surname;
                ((MainWindow)Application.Current.MainWindow).addedStudent[2] = age.ToString();
                ((MainWindow)Application.Current.MainWindow).addedStudent[3] = sex.ToString();

                this.Close();
            }
            else
            {
                lb_error.Content = "Źle wypełnione pola!";
            }
        }
    }
}
