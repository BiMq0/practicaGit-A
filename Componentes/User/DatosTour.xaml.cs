using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para DatosTour.xaml
    /// </summary>
    public partial class DatosTour : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/tour/";
        private string URL_sitio = "http://localhost:8000/api/sitios/";
        private string URL_aloja = "http://localhost:8000/api/alojamientos/";

        private Tour tourActual;
        private List<Sitio> sitios;
        private List<Alojamiento> alojamientos;
        public DatosTour()
        {
            InitializeComponent();
        }

        public async void cargarDatos()
        {
            try
            {
                
                int id_tour = int.Parse(hiddenId.Text);
                tourActual = await cliente.GetFromJsonAsync<Tour>(URL + id_tour);
                if (tourActual == null) return;

                hiddenId.Text = tourActual.id_tour.ToString();
                txbNombreTour.Text = tourActual.nombre_tour;
                txbDescTour.Text = tourActual.descripcion_tour;
                txbCostoTour.Text = "Bs. " + tourActual.costo_tour.ToString("F2");
                txbDias.Text = tourActual.duracion_dias.ToString();
                txbNoches.Text = tourActual.duracion_noches.ToString();
                cmbActivo.SelectedIndex = tourActual.Activo == 1 ? 1 : 0;

                var sitioInicio = await cliente.GetFromJsonAsync<Sitio>($"{URL_sitio}{tourActual.id_sitio_inicio}");
                var sitioFin = await cliente.GetFromJsonAsync<Sitio>($"{URL_sitio}{tourActual.id_sitio_fin}");
                var alojamiento = await cliente.GetFromJsonAsync<Alojamiento>($"{URL_aloja}{tourActual.id_alojamiento}");

                txbSitioInicial.Text = sitioInicio?.nombre ?? "No encontrado";
                txbSitioFinal.Text = sitioFin?.nombre ?? "No encontrado";
                txbAlojamiento.Text = alojamiento?.nombre_aloj ?? "No encontrado";

                cmbSitioInicio.Visibility = Visibility.Collapsed;
                cmbSitioFin.Visibility = Visibility.Collapsed;
                cmbAlojamiento.Visibility = Visibility.Collapsed;

                habilitar(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos    singinifica que esto esta mal pipip: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void habilitar(bool valor)
        {
            txbDescTour.IsReadOnly = !valor;
            txbCostoTour.IsReadOnly = !valor;
            txbDias.IsReadOnly = !valor;
            txbNoches.IsReadOnly = !valor;
            cmbActivo.IsEnabled = valor;

            cmbSitioInicio.IsEnabled = valor;
            cmbSitioFin.IsEnabled = valor;
            cmbAlojamiento.IsEnabled = valor;

            txbSitioInicial.Visibility = valor ? Visibility.Collapsed : Visibility.Visible;
            txbSitioFinal.Visibility = valor ? Visibility.Collapsed : Visibility.Visible;
            txbAlojamiento.Visibility = valor ? Visibility.Collapsed : Visibility.Visible;
            cmbSitioInicio.Visibility = valor ? Visibility.Visible : Visibility.Collapsed;
            cmbSitioFin.Visibility = valor ? Visibility.Visible : Visibility.Collapsed;
            cmbAlojamiento.Visibility = valor ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnReservarSitio_Click(object sender, RoutedEventArgs e)
        {
            //meter aqui el envio a formulario con la validacion de si esta registrado o no
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.usuario.datosTour.Visibility = Visibility.Collapsed;
            mainWindow.usuario.listadoTours.Visibility = Visibility.Visible;
            mainWindow.usuario.btnReservar.Visibility = Visibility.Visible;
        }
    }
}
