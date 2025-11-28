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
using chaski_tours_desk.Modelos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Reflection;

namespace chaski_tours_desk.Componentes
{
    /// <summary>
    /// Lógica de interacción para SignUp.xaml
    /// </summary>
    public partial class SignUp : UserControl
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private HttpClient client = new HttpClient();
        private string URL_Turista = "http://localhost:8000/api/visitantes/turistas/crear";
        private string URL_Instituciones = "http://localhost:8000/api/visitantes/instituciones";

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try { await registrarCliente(); }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado, Intente nuevamente");
                MessageBox.Show(ex.Message, "Error");
            }
            finally { aclarar(); }
        }

        private async Task registrarCliente()
        {
            oscurecer();
            if (cmbTipoUsuario.SelectedIndex == 0)
            {
                await registrarTurista();
            }
            else {
                await registrarInstitucion();
            }
        }


        public event Action volverLogin;

        private async Task registrarInstitucion()
        {
            if (!EsCorreoValido(txtCorreoInstitucion.Text.Trim()))
            {
                MessageBox.Show("Correo inválido, ingrese nuevamente");
                return;
            }

            Institucion nuevaInstitucion = new Institucion
            {
                nombre = txtNombreEmpresa.Text.Trim(),
                correo_electronico = txtCorreoInstitucion.Text.Trim(),
                contrasenia = txtPasswordInstitucion.Password,
                nacionalidad = (cmbNacionalidadInstitucion.SelectedItem as ComboBoxItem).Content.ToString(),
                telefono = txtTelefonoInstitucion.Text.Trim(),
                nombre_represent = txtNomRep.Text.Trim(),
                ap_pat_represent = txtApRep.Text.Trim(),
                correo_electronico_represent = txtEmailRep.Text.Trim(),
                telefono_represent = txtTelRep.Text.Trim(),
            };

            if (!EsEntradaValida(nuevaInstitucion))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var response = await client.PostAsJsonAsync(URL_Instituciones, nuevaInstitucion);
            
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Institucion registrada correctamente");
                volverLogin.Invoke();
            }
            else
            {
                MessageBox.Show("Error al registrar al Institucion, Intente nuevamente\n" + response);
            }
        }

        private async Task registrarTurista()
        {
            if (!EsCorreoValido(txtCorreoTurista.Text.Trim()))
            {
                MessageBox.Show("Correo inválido, ingrese nuevamente");
                return;
            }

            Turista nuevoTurista = new Turista
            {
                documento = txtDocumento.Text.Trim(),
                nombre = txtNombres.Text.Trim(),
                ap_pat = txtApellidoPaterno.Text.Trim(),
                ap_mat = txtApellidoMaterno.Text.Trim(),
                fecha_nac = dpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd"),
                nacionalidad = (cbNacionalidadTurista.SelectedItem as ComboBoxItem).Content.ToString(),
                telefono = txtTelefonoTurista.Text.Trim(),
                correo_electronico = txtCorreoTurista.Text.Trim(),
                contrasenia = txtPasswordTurista.Password
            };

            if (!EsEntradaValida(nuevoTurista))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var response = await client.PostAsJsonAsync(URL_Turista, nuevoTurista);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Turista registrado correctamente");
                volverLogin.Invoke();
            }
            else
            {
                MessageBox.Show("Error al registrar al turista, Intente nuevamente" + response.Content.ToString());
            }
        }


        private void cbTipoUsuario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (panelCamposTurista == null || panelCamposInstitucion == null)
            {
                return;
            }

            if (cmbTipoUsuario.SelectedIndex == 0)
            {
                panelCamposTurista.Visibility = Visibility.Visible;
                panelCamposInstitucion.Visibility = Visibility.Collapsed;
            }
            else
            {
                panelCamposTurista.Visibility = Visibility.Collapsed;
                panelCamposInstitucion.Visibility = Visibility.Visible;
            }
        }

        private bool EsEntradaValida(object visitante)
        {
            if (visitante == null) return false;
            
            var props = visitante.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)  && p.Name != "cod_visitante");
            
            foreach (var prop in props)
            {
                var valor = prop.GetValue(visitante)?.ToString();

                if (valor == null || string.IsNullOrWhiteSpace(valor.ToString())) return false;
            }
            return true;
        }

        private bool EsCorreoValido(string correo)
        {
            string[] cadena = correo.Split('@');
            string[] dominio = cadena[1].Split('.');
            if (cadena[0].Length < 4 || dominio[0].Length < 4 || dominio[1].Length < 3) { 
                return false;
            }
            return true;
        }

        private void oscurecer()
        {
            SolidColorBrush oscuro = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BB635968"));
            if (cmbTipoUsuario.SelectedIndex == 0)
            {
                brdDocumento.Background = oscuro;
                brdNombres.Background = oscuro;
                brdApPat.Background = oscuro;
                brdApMat.Background = oscuro;
                brdTelefono.Background =  oscuro;
                brdCorreo.Background = oscuro;
                brdPassword.Background = oscuro;
            }
            else {
                brdNomEmpresa.Background = oscuro;
                brdTelEmpresa.Background = oscuro;
                brdCorreoEmpresa.Background = oscuro;
                brdPassEmpresa.Background = oscuro;
                brdNomRep.Background = oscuro;  
                brdApRep.Background = oscuro;  
                brdEmailRep.Background = oscuro;  
                brdTelRep.Background = oscuro;    
            }
        }

        private void aclarar()
        {
            SolidColorBrush claro = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
            if (cmbTipoUsuario.SelectedIndex == 0)
            {
                brdDocumento.Background = claro;
                brdNombres.Background = claro;
                brdApPat.Background = claro;
                brdApMat.Background = claro;
                brdTelefono.Background = claro;
                brdCorreo.Background = claro;
                brdPassword.Background = claro;
            }
            else
            {
                brdNomEmpresa.Background = claro;
                brdTelEmpresa.Background = claro;
                brdCorreoEmpresa.Background = claro;
                brdPassEmpresa.Background = claro;
                brdNomRep.Background = claro;
                brdApRep.Background = claro;
                brdEmailRep.Background = claro;
                brdTelRep.Background = claro;
            }
        }
    }
}
