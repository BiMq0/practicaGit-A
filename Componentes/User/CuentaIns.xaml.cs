using chaski_tours_desk.Modelos;
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
using System.Windows.Shapes;
using System.Net.Http.Json;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para CuentaIns.xaml
    /// </summary>
    public partial class CuentaIns : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URL_Institucion = "http://localhost:8000/api/visitantes/instituciones/cod/";
        private string URL_Reserva = "http://localhost:8000/api/reservas/cod/";

        public CuentaIns(string codVisitante)
        {
            InitializeComponent();
            cargarPerfil(codVisitante);
            cargarReservas(codVisitante);
        }
        private void cargarPerfil(string cod)
        {
            try
            {
                var institucion = cliente.GetFromJsonAsync<Institucion>(URL_Institucion + cod).Result;

                if (institucion != null)
                {
                    txtNombre.Text = institucion.nombre;
                    txtCorreo.Text = institucion.correo_electronico;
                    txtNacionalidad.Text = institucion.nacionalidad;
                    txtTelefono.Text = institucion.telefono;
                    txtRepresentante.Text = institucion.nombre_represent + " " + institucion.ap_pat_represent;
                    txtCorreoRep.Text = institucion.correo_electronico_represent;
                    txtTelefonoRep.Text = institucion.telefono_represent;
                }
                else
                {
                    MessageBox.Show("No se pudo cargar el perfil de la institución.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el perfil: " + ex.Message);
            }
        }

        private void cargarReservas(string cod)
        {
            try
            {
                var reservas = cliente.GetFromJsonAsync<List<Reserva>>(URL_Reserva + cod).Result;

                if (reservas != null && reservas.Any())
                {
                    listaReservas.ItemsSource = reservas;
                    listaReservas.Visibility = Visibility.Visible;
                    txtNoReservas.Visibility = Visibility.Collapsed;
                }
                else
                {
                    listaReservas.ItemsSource = null;
                    listaReservas.Visibility = Visibility.Collapsed;
                    txtNoReservas.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("JSON"))
                {
                    MessageBox.Show("Aún no ha realizado reservas.");
                    listaReservas.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("Error al cargar el historial de reservas: " + ex.Message);
                }
            }
        }
    }
}
