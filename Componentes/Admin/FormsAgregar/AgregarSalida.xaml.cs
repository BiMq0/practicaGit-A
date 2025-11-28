using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace chaski_tours_desk.Componentes.Admin.FormsAgregar
{
    /// <summary>
    /// Lógica de interacción para InfoSalida.xaml
    /// </summary>
    public partial class AgregarSalida : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/calendario/";
        private string URL_Tour = "http://localhost:8000/api/tour/";

        public AgregarSalida()
        {
            InitializeComponent();

            cargarTours();
        }

        private bool EntradasValidas()
        {
            
            if (dpFechaSalida.SelectedDate == null || dpFechaRegreso.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar ambas fechas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (dpFechaRegreso.SelectedDate < dpFechaSalida.SelectedDate)
            {
                MessageBox.Show("La fecha de regreso debe ser posterior a la fecha de salida", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (cmbTours.SelectedItem == null) {
                MessageBox.Show("Debe seleccionar un tour", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void cargarTours() {
            var tours = await cliente.GetFromJsonAsync<List<Tour>>(URL_Tour);
            foreach (var tour in tours.OrderBy(tour => tour.id_tour)) {
                var item = new ComboBoxItem
                {
                    Content = tour.nombre_tour,
                };
                cmbTours.Items.Add(item);
            }
        }

        private async void btnGuardarSalida_Click(object sender, RoutedEventArgs e)
        {
            if (!EntradasValidas()) {
                return;
            }

            var salida = new Salida
            {
                id_tour = cmbTours.SelectedIndex + 1,
                fecha_salida = dpFechaSalida.SelectedDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                fecha_regreso = dpFechaRegreso.SelectedDate?.ToString("yyyy-MM-dd HH:mm:ss")
            };

            var serializado = JsonSerializer.Serialize(salida);
            MessageBox.Show(serializado);
            Debug.WriteLine(serializado);
            
            var response = await cliente.PostAsJsonAsync(URL, salida);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Salida creada correctamente");
            }
            else {
                MessageBox.Show("Error al crear la salida");
            }
        }
    }
}