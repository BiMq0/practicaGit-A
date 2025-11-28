using chaski_tours_desk.Componentes;
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
using System.IO;
using chaski_tours_desk.Ventanas;

namespace chaski_tours_desk
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string codVisitanteActual { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usuario.abrirAdmin += abrirLogin;
        }

        private void abrirLogin()
        {
            usuario.Visibility = Visibility.Collapsed;
            logSign.Visibility = Visibility.Visible;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("Images/fondoLogin.jpg", UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            Fondo.Source = bitmap;
        }
    }
}
