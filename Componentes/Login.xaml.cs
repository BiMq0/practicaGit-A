using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Net.Http.Json;
using chaski_tours_desk.Modelos;
using chaski_tours_desk;
using System.IO;
using chaski_tours_desk.Componentes.User;

namespace chaski_tours_desk.Componentes
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private string URL = "http://localhost:8000/api/visitantes/turistas/";
        private string URL_inst = "http://localhost:8000/api/visitantes/instituciones/";

        private static readonly HttpClient cliente = new HttpClient();
        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try { await verificarUsuario(); }
            catch (Exception ex) { 
                MessageBox.Show("Error al verificar el usuario, Intente nuevamente"); 
                MessageBox.Show(ex.Message, "Error");
            }
            finally { aclarar(); }
        }

        private async Task verificarUsuario()
        {
            oscurecer();

            if (!entradasValidas()) return;

            var turista = await obtenerUsuario();
            if (turista != null && turista.contrasenia == txtPassword.Password)
            {
                MainWindow.codVisitanteActual = turista.cod_visitante;
                redirigirUsuarios("turista");
                return;
            }

            var institucion = await obtenerUsuarioInst();
            if (institucion != null && institucion.contrasenia == txtPassword.Password)
            {
                MainWindow.codVisitanteActual = institucion.cod_visitante;
                redirigirUsuarios("institucion");
                return;
            }

            MessageBox.Show("Correo o contraseña incorrectos.");
        }
        private bool entradasValidas()
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return false;
            }
            return true;
        }
        private async Task<Turista> obtenerUsuario()
        {
            List<Turista> usuario = await cliente.GetFromJsonAsync<List<Turista>>(URL + txtUsuario.Text);
            if (usuario.Count == 0) {
                return null;
            }
            return usuario[0];
        }
        private async Task<Institucion> obtenerUsuarioInst()
        {
            List<Institucion> usuario = await cliente.GetFromJsonAsync<List<Institucion>>(URL_inst + txtUsuario.Text);
            if (usuario.Count == 0)
            {
                return null;
            }
            return usuario[0];
        }

        private void redirigirUsuarios(string tipo) {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            string[] correo = txtUsuario.Text.Split('@');
            if (correo[1] == "chaskitours.com")
            {
                MessageBox.Show("Bienvenido administrador");
                mainWindow.admin.Visibility = Visibility.Visible;
                mainWindow.admin.Inicio.titulo.Content = txtUsuario.Text;
                mainWindow.logSign.Visibility = Visibility.Collapsed;
            }
            else {
                if (tipo == "turista")
                    MessageBox.Show("Bienvenido turista");
                else if (tipo == "institucion")
                    MessageBox.Show("Bienvenido institución");
                mainWindow.usuario.Visibility = Visibility.Visible;
                mainWindow.logSign.Visibility = Visibility.Collapsed;
                // MOSTRAR VENTANA DE RESERVA
                var reservaWindow = new ReservaForm(tipo);
                reservaWindow.ShowDialog(); // Bloquea hasta cerrar
            }
        }
        private void oscurecer()
        {
            brdMail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BB635968"));
            brdPass.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BB635968"));
        }

        private void aclarar() {
            brdMail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
            brdPass.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
        }
    }
}
