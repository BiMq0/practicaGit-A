using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chaski_tours_desk.Componentes.Admin.FormsAgregar
{
    /// <summary>
    /// Lógica de interacción para AgregarTour.xaml
    /// </summary>
    public partial class AgregarTour : Window
    {
        HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/tours/";
        private string URL_crear = "http://localhost:8000/api/tours/crear";
        private string URL_sitio = "http://localhost:8000/api/sitios/";
        private string URL_aloja = "http://localhost:8000/api/alojamientos/";

        public AgregarTour()
        {
            InitializeComponent();
            habilitar(true);
            cargarDatos();
        }
        private void habilitar(bool valor)
        {
            txbNombreTour.IsEnabled = valor;
            txbDescTour.IsEnabled = valor;
            txbCostoTour.IsEnabled = valor;
            txbDias.IsEnabled = valor;
            txbNoches.IsEnabled = valor;
            cmbActivo.IsEnabled = valor;
            cmbSitioInicial.IsEnabled = valor;
            cmbSitioFinal.IsEnabled = valor;
            cmbAlojamiento.IsEnabled = valor;
        }
        private async void cargarDatos()
        {
            await cargarSitios();
            await cargarAlojamientos();
        }
        private async Task cargarSitios()
        {
            try
            {
                var sitios = await cliente.GetFromJsonAsync<List<Sitio>>(URL_sitio);
                if (sitios != null)
                {
                    cmbSitioInicial.ItemsSource = sitios;
                    cmbSitioFinal.ItemsSource = sitios;

                    cmbSitioInicial.DisplayMemberPath = "nombre";
                    cmbSitioFinal.DisplayMemberPath = "nombre";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar sitios: " + ex.Message);
            }
        }
        private async Task cargarAlojamientos()
        {
            try
            {
                var alojamientos = await cliente.GetFromJsonAsync<List<Alojamiento>>(URL_aloja);
                if (alojamientos != null)
                {
                    cmbAlojamiento.ItemsSource = alojamientos;
                    cmbAlojamiento.DisplayMemberPath = "nombre_aloj";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar alojamientos: " + ex.Message);
            }
        }
        private bool EntradasValidas()
        {
            if (string.IsNullOrWhiteSpace(txbNombreTour.Text) ||
                string.IsNullOrWhiteSpace(txbDescTour.Text) ||
                string.IsNullOrWhiteSpace(txbCostoTour.Text) ||
                string.IsNullOrWhiteSpace(txbDias.Text) ||
                string.IsNullOrWhiteSpace(txbNoches.Text))
            {
                MessageBox.Show("Todos los campos de texto son obligatorios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!decimal.TryParse(txbCostoTour.Text, out decimal costo) || costo < 0)
            {
                MessageBox.Show("El costo debe ser un número válido y positivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(txbDias.Text, out int dias) || dias <= 0)
            {
                MessageBox.Show("El número de días debe ser un entero válido y mayor a cero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (((Sitio)cmbSitioInicial.SelectedItem).id_sitio == ((Sitio)cmbSitioFinal.SelectedItem).id_sitio)
            {
                MessageBox.Show("El sitio inicial y final no pueden ser el mismo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!int.TryParse(txbNoches.Text, out int noches) || noches < 0)
            {
                MessageBox.Show("El número de noches debe ser un número válido y no negativo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (noches != dias - 1)
            {
                MessageBox.Show("Ingresar bien la duración (días y noches)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (cmbSitioInicial.SelectedItem == null || cmbSitioFinal.SelectedItem == null || cmbAlojamiento.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un sitio inicial, final y un alojamiento", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
        
        private async void btnGuardarTour_Click(object sender, RoutedEventArgs e)
        {
            if (!EntradasValidas())
            {
                return;
            }
            var nuevoTour = new Tour
            {
                nombre_tour = txbNombreTour.Text,
                descripcion_tour = txbDescTour.Text,
                costo_tour = double.Parse(txbCostoTour.Text),
                duracion_dias = int.Parse(txbDias.Text),
                duracion_noches = int.Parse(txbNoches.Text),
                Activo = cmbActivo.SelectedIndex,
                id_sitio_inicio = ((Sitio)cmbSitioInicial.SelectedItem).id_sitio,
                id_sitio_fin = ((Sitio)cmbSitioFinal.SelectedItem).id_sitio,
                id_alojamiento = ((Alojamiento)cmbAlojamiento.SelectedItem).id_alojamiento
            };
            var respuesta = await cliente.PostAsJsonAsync(URL_crear, nuevoTour);

            if (respuesta.IsSuccessStatusCode)
            {
                MessageBox.Show("Tour creado correctamente", "Éxito", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Error al crear el tour.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
