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
    /// Logika interakcji dla klasy EditStudent.xaml
    /// </summary>
    public partial class EditStudent : Window
    {
        string name;
        string surname;
        int age;
        bool sex;
        bool canSend1 = true;
        bool canSend2 = true;
        bool canSend3 = true;
        int ignoreme;
        public string[] studentInfo = new string[4];
        public EditStudent()
        {
            InitializeComponent();
        }

        private void btn_EditStudent_Click(object sender, RoutedEventArgs e)
        {
            string x = txt_Name.Text.Trim();

            string[] parts = x.Split(' ');
            if (parts.Length == 2)
            {
                name = parts[0];
                surname = parts[1];

                canSend1 = true;
            }
            else
            {
                canSend1 = false;
            }

            if (int.TryParse(txt_Age.Text, out ignoreme))
            {
                age = int.Parse(txt_Age.Text);
                canSend2 = true;
            }
            else
            {
                canSend2 = false;
            }

            if (canSend1 == true && canSend2 == true && canSend3 == true)
            {
                MainWindow mainWindow = new MainWindow();
                ((MainWindow)Application.Current.MainWindow).editedStudent[0] = name;
                ((MainWindow)Application.Current.MainWindow).editedStudent[1] = surname;
                ((MainWindow)Application.Current.MainWindow).editedStudent[2] = age.ToString();
                ((MainWindow)Application.Current.MainWindow).editedStudent[3] = sex.ToString();

                this.Close();
            }
            else
            {
                lb_error.Content = "Źle wypełnione pola!";
            }
        }

        private void rad_Boy_Checked(object sender, RoutedEventArgs e)
        {
            if (rad_Boy.IsChecked == false && rad_Girl.IsChecked == false)
            {
                canSend3 = false;
            }
            else
            {
                sex = true;
                canSend3 = true;
            }
        }

        private void rad_Girl_Checked(object sender, RoutedEventArgs e)
        {
            if (rad_Boy.IsChecked == false && rad_Girl.IsChecked == false)
            {
                canSend3 = false;
            }
            else
            {
                sex = false;
                canSend3 = true;
            }
        }

        private void editStudent_OnLoad(object sender, RoutedEventArgs e)
        {
            txt_Name.Text = studentInfo[0] + ' ' + studentInfo[1];
            txt_Age.Text = studentInfo[2];
            rad_Boy.IsChecked = bool.Parse(studentInfo[3]) == true ? true : false; 
            rad_Girl.IsChecked = bool.Parse(studentInfo[3]) == false ? true : false; 
        }
    }
}
