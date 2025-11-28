using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace chaski_tours_desk.Componentes.Admin.FormsInfo
{
    /// <summary>
    /// Lógica de interacción para InfoSalida.xaml
    /// </summary>
    public partial class InfoSalida : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/calendario/";
        private string URL_Tour = "http://localhost:8000/api/tour/";

        public InfoSalida(int id_salida)
        {
            InitializeComponent();

            try
            {
                var salida = obtenerSalida(id_salida);
                cargarDatos(salida);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}");
            }
        }

        private void cargarDatos(Salida salida)
        {
            hiddenId.Text = salida.id_salida.ToString();
            txbIdSalida.Text = $"ID: {salida.id_salida}";
            dpFechaSalida.SelectedDate = DateTime.Parse(salida.fecha_salida);
            dpFechaRegreso.SelectedDate = DateTime.Parse(salida.fecha_regreso);
            txbIdTour.Text = salida.id_tour.ToString();
        }

        private Salida obtenerSalida(int id_salida)
        {
            var salida = cliente.GetFromJsonAsync<Salida>(URL + id_salida).Result;
            return salida;
        }

        private async void btnVerTour_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbIdTour.Text)) return;

            try
            {
                var tour = await cliente.GetFromJsonAsync<Tour>(URL_Tour + txbIdTour.Text);

                txbNombreTour.Text = tour.nombre_tour;
                txbDescripcionTour.Text = tour.descripcion_tour;
                brdInfoTour.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar información del tour: {ex.Message}");
            }
        }

        private int editUpdate = 0;

        private void habilitar(bool valor)
        {
            dpFechaSalida.IsEnabled = valor;
            dpFechaRegreso.IsEnabled = valor;
            
        }

        private async void btnEditarSalida_Click(object sender, RoutedEventArgs e)
        {
            editUpdate++;
            if (editUpdate == 1)
            {
                // Configurar interfaz para edición
                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesUser"];
                btnEditarSalida.Style = (Style)Application.Current.Resources["UserButtonStyle"];
                btnEditarSalida.Content = "Guardar";
                btnEliminarSalida.Content = "Cancelar";
                habilitar(true);

                btnVolver.Visibility = Visibility.Collapsed;
                brdBtnVolver.Visibility = Visibility.Collapsed;
            }
            else if (editUpdate == 2)
            {
                // Validar y guardar cambios
                if (!EntradasValidas())
                {
                    editUpdate = 1;
                    return;
                }

                var salidaActualizada = new Salida
                {
                    fecha_salida = dpFechaSalida.SelectedDate?.ToString("yyyy-MM-dd"),
                    fecha_regreso = dpFechaRegreso.SelectedDate?.ToString("yyyy-MM-dd"),
                };

                var response = await cliente.PutAsJsonAsync(URL + hiddenId.Text, salidaActualizada);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Salida actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al actualizar la salida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Restaurar interfaz
                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesSecundarios"];
                btnEditarSalida.Style = (Style)Application.Current.Resources["TertiaryButtonStyle"];
                btnEditarSalida.Content = "Editar";
                btnEliminarSalida.Content = "Eliminar";
                habilitar(false);

                btnVolver.Visibility = Visibility.Visible;
                brdBtnVolver.Visibility = Visibility.Visible;

                // Recargar datos
                cargarDatos(obtenerSalida(int.Parse(hiddenId.Text)));

                editUpdate = 0;
            }
        }

        private bool EntradasValidas()
        {
            // Validar fechas
            if (dpFechaSalida.SelectedDate == null || dpFechaRegreso.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar ambas fechas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validar que la fecha de regreso sea posterior a la de salida
            if (dpFechaRegreso.SelectedDate <= dpFechaSalida.SelectedDate)
            {
                MessageBox.Show("La fecha de regreso debe ser posterior a la fecha de salida", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnEliminarSalida_Click(object sender, RoutedEventArgs e)
        {
            if (editUpdate == 1)
            {
                // Cancelar edición
                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesSecundarios"];
                btnEditarSalida.Style = (Style)Application.Current.Resources["TertiaryButtonStyle"];
                btnEditarSalida.Content = "Editar";
                btnEliminarSalida.Content = "Eliminar";
                habilitar(false);

                btnVolver.Visibility = Visibility.Visible;
                brdBtnVolver.Visibility = Visibility.Visible;

                // Recargar datos originales
                cargarDatos(obtenerSalida(int.Parse(hiddenId.Text)));

                editUpdate = 0;
            }
            else if (editUpdate == 0)
            {
                // Eliminar salida
                var result = MessageBox.Show("¿Está seguro que quiere eliminar esta salida?", "Eliminar", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var response = cliente.DeleteAsync(URL + hiddenId.Text).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Salida eliminada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        MainWindow window = (MainWindow)Application.Current.MainWindow;
                        window.admin.Salidas.verSalidas();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar la salida", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}