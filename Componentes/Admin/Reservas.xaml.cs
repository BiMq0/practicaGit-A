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
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using chaski_tours_desk.Modelos;
using chaski_tours_desk.Componentes.Admin.FormsAgregar;
using chaski_tours_desk.Componentes.Admin.FormsInfo;

namespace chaski_tours_desk.Componentes.Admin
{
    /// <summary>
    /// Lógica de interacción para Reservas.xaml
    /// </summary>
    public partial class Reservas : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/reservas";
        private List<Reserva> todaslasreservas = new List<Reserva>();
        public Reservas()
        {
            InitializeComponent();
        }
        private async Task obtenerReserva()
        {
            var reservas = await cliente.GetFromJsonAsync<List<Reserva>>(URL);
            todaslasreservas = reservas;
            tbl_Reserva.ItemsSource = reservas;
        }

        private async void verReserva()
        {
            await obtenerReserva();
        }

        private void verDatos()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verReserva();
            }
        }

        private void Reserva_Loaded(object sender, RoutedEventArgs e)
        {
            verDatos();
        }

        private void txbBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarReservas(txbBusqueda.Text);
        }
        private void FiltrarReservas(string texto)
        {
            if (todaslasreservas == null) return;

            var filtrados = todaslasreservas;

            tbl_Reserva.ItemsSource = filtrados;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReservasNuevo crear = new ReservasNuevo();
            crear.Closed += async (s, args) =>
            {
                await obtenerReserva();
            };

            crear.Show();
        }

        private void tbl_Reserva_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbl_Reserva.SelectedItem is Reserva reservaseleccionada)
            {
                ReservasAdmin editar = new ReservasAdmin(reservaseleccionada);

                editar.Closed += async (s, args) =>
                {
                    await obtenerReserva();
                };


                editar.Show();
            }
        }
    }
}