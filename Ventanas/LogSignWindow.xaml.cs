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
using System.IO;

namespace chaski_tours_desk.Ventanas
{
    /// <summary>
    /// Lógica de interacción para LogSignWindow.xaml
    /// </summary>
    public partial class LogSignWindow : UserControl
    {
        public LogSignWindow()
        {
            InitializeComponent();
            
        }
        private void verLogSignUp()
        {
            Login.btnSignup.Click += BtnSignup_Click;
            Signup.btnLogin.Click += BtnLogin_Click;

            void BtnSignup_Click(object sender, RoutedEventArgs e)
            {
                Signup.Visibility = Visibility.Visible;
                Login.Visibility = Visibility.Collapsed;
            }

            void BtnLogin_Click(object sender, RoutedEventArgs e)
            {
                Signup.Visibility = Visibility.Collapsed;
                Login.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            verLogSignUp();
            Signup.volverLogin += MostrarLogin;
        }

        private void MostrarLogin()
        {
            Signup.Visibility = Visibility.Collapsed;
            Login.Visibility = Visibility.Visible;
        }
    }
}
