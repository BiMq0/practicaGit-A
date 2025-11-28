using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace chaski_tours_desk.Componentes.Admin.FormsInfo
{
    /// <summary>
    /// Lógica de interacción para InfoTour.xaml
    /// </summary>
    public partial class InfoTour : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/tour/";
        private string URL_sitio = "http://localhost:8000/api/sitios/";
        private string URL_aloja = "http://localhost:8000/api/alojamientos/";

        private Tour tourActual;
        private List<Sitio> sitios;
        private List<Alojamiento> alojamientos;
        public InfoTour(int id_tour)
        {
            InitializeComponent();
            cargarDatos(id_tour);
        }
        private async void cargarDatos(int id_tour)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private int editUpdate = 0;
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
        private async void btnEditarSitio_Click(object sender, RoutedEventArgs e)
        {
            editUpdate++;

            if (editUpdate == 1) 
            {
                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesUser"];
                btnEditarSitio.Style = (Style)Application.Current.Resources["UserButtonStyle"];
                btnEditarSitio.Content = "Guardar";
                btnEliminarSitio.Content = "Cancelar";

                if (sitios == null || alojamientos == null)
                {
                    sitios = await cliente.GetFromJsonAsync<List<Sitio>>(URL_sitio);
                    alojamientos = await cliente.GetFromJsonAsync<List<Alojamiento>>(URL_aloja);

                    cmbSitioInicio.ItemsSource = sitios;
                    cmbSitioFin.ItemsSource = sitios;
                    cmbAlojamiento.ItemsSource = alojamientos;

                    cmbSitioInicio.SelectedValue = tourActual.id_sitio_inicio;
                    cmbSitioFin.SelectedValue = tourActual.id_sitio_fin;
                    cmbAlojamiento.SelectedValue = tourActual.id_alojamiento;
                }

                habilitar(true);
                btnVolver.Visibility = Visibility.Collapsed;
            }
            else if (editUpdate == 2) 
            {
                if (!EntradasValidas())
                {
                    editUpdate = 1;
                    return;
                }

                try
                {
                    var tourActualizado = new Tour
                    {
                        id_tour = tourActual.id_tour,
                        nombre_tour = txbNombreTour.Text,
                        descripcion_tour = txbDescTour.Text,
                        costo_tour = double.Parse(txbCostoTour.Text.Replace("Bs.", "").Trim()),
                        duracion_dias = int.Parse(txbDias.Text),
                        duracion_noches = int.Parse(txbNoches.Text),
                        Activo = cmbActivo.SelectedIndex == 1 ? 1 : 0,
                        id_sitio_inicio = (int)cmbSitioInicio.SelectedValue,
                        id_sitio_fin = (int)cmbSitioFin.SelectedValue,
                        id_alojamiento = (int)cmbAlojamiento.SelectedValue
                    };
                    var json = JsonSerializer.Serialize(tourActualizado);
                    Debug.WriteLine(json);
                    var response = await cliente.PutAsJsonAsync($"{URL}{tourActualizado.id_tour}", tourActualizado);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Tour actualizado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error: {response.StatusCode}\n{errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesSecundarios"];
                    btnEditarSitio.Style = (Style)Application.Current.Resources["TertiaryButtonStyle"];
                    btnEditarSitio.Content = "Editar";
                    btnEliminarSitio.Content = "Eliminar";
                    habilitar(false);
                    btnVolver.Visibility = Visibility.Visible;
                    editUpdate = 0;
                }
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

            if (!decimal.TryParse(txbCostoTour.Text.Replace("Bs.", "").Trim(), out decimal costo) || costo < 0)
            {
                MessageBox.Show("El costo debe ser un número válido y positivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(txbDias.Text, out int dias) || dias <= 0)
            {
                MessageBox.Show("El número de días debe ser un entero válido y mayor a cero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(txbNoches.Text, out int noches) || noches < 0)
            {
                MessageBox.Show("El número de noches debe ser un número válido y no negativo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (noches != dias - 1)
            {
                MessageBox.Show("Ingresar bien la duración (días y noches). Ej: 3 días = 2 noches.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (cmbSitioInicio.SelectedItem == null || cmbSitioFin.SelectedItem == null || cmbAlojamiento.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un sitio inicial, final y un alojamiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private async void btnEliminarSitio_Click(object sender, RoutedEventArgs e)
        {
            if (editUpdate==0)
            {
                var confirm = MessageBox.Show("¿Está seguro que desea eliminar este tour?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    var response = await cliente.DeleteAsync(URL + hiddenId.Text);
                    if (response.IsSuccessStatusCode)
                        MessageBox.Show("Tour eliminado correctamente.", "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("No se pudo eliminar el tour.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
