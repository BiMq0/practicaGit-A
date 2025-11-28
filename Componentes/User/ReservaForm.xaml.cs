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
using System.Windows.Shapes;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para ReservaForm.xaml
    /// </summary>
    public partial class ReservaForm : Window
    {
        private List<Tour> toursDisponibles;
        private List<CalendarioSalida> salidasDisponibles;
        private Tour tourSeleccionado;
        private string tipoUsuario;
        private HttpClient cliente = new HttpClient();
        public ReservaForm(string tipo)
        {
            InitializeComponent();
            tipoUsuario = tipo;
            cargarTours();
        }
        private async void cargarTours()
        {
            try
            {
                var response = await cliente.GetFromJsonAsync<List<Tour>>("http://localhost:8000/api/tour");
                if (response != null)
                {
                    toursDisponibles = response;
                    cmbTours.ItemsSource = toursDisponibles;
                    cmbTours.DisplayMemberPath = "nombre_tour";
                    cmbTours.SelectedValuePath = "id_tour";
                }
            }
            catch
            {
                MessageBox.Show("Error al cargar los tours.");
            }
        }
        private void txtCantidadPersonas_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Va
        }
        private async void cmbTours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tourSeleccionado = cmbTours.SelectedItem as Tour;
            if (tourSeleccionado != null)
            {
                txtCostoTour.Text = tourSeleccionado.costo_tour.ToString("F2");
                txtDuracion.Text = $"{tourSeleccionado.duracion_dias} días / {tourSeleccionado.duracion_noches} noches";

                double minimo = tourSeleccionado.costo_tour * 0.3;
                txtMontoMinimo.Text = minimo.ToString("F2");

                var response = await cliente.GetFromJsonAsync<List<CalendarioSalida>>("http://localhost:8000/api/calendario");
                salidasDisponibles = response?.FindAll(s => s.id_tour == tourSeleccionado.id_tour);
                cmbFechas.ItemsSource = salidasDisponibles;
                cmbFechas.DisplayMemberPath = "fecha_salida";
                cmbFechas.SelectedValuePath = "id_salida";
            }
        }
        private async void btnConfirmarReserva_Click(object sender, RoutedEventArgs e)
        {
            if (tourSeleccionado == null || cmbFechas.SelectedItem == null)
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            int cantidad = tipoUsuario == "turista" ? 1 : int.Parse(txtCantidadPersonas.Text);
            double monto = double.Parse(txtMontoAPagar.Text);
            double minimo = double.Parse(txtMontoMinimo.Text);

            if (monto < minimo)
            {
                MessageBox.Show("El monto debe ser al menos el 30%.");
                return;
            }

            Reserva reserva = new Reserva
            {
                cod_visitante = MainWindow.codVisitanteActual,
                id_salida = (int)cmbFechas.SelectedValue,
                cantidad_personas = cantidad,
                costo_total_reserva = monto,
                estado = "Pendiente"
            };

            var result = await cliente.PostAsJsonAsync("http://localhost:8000/api/reservas/crear", reserva);
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show("Reserva realizada con éxito.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al registrar reserva.");
            }

        }
    }
}
