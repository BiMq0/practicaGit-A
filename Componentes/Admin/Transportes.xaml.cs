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
    /// Lógica de interacción para Transportes.xaml
    /// </summary>
    public partial class Transportes : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/transporte";
        private List<Transporte> todosLosTransportes = new List<Transporte>();
        public Transportes()
        {
            InitializeComponent();
        }

        private async Task obtenerTransportes()
        {
            var transportes = await cliente.GetFromJsonAsync<List<Transporte>>(URL);
            todosLosTransportes = transportes;
            tbl_Transportes.ItemsSource = transportes;

        }

        private async void verTransportes()
        {
            await obtenerTransportes();
        }

        private void verDatos()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verTransportes();
            }
        }

        private void Transporte_Loaded(object sender, RoutedEventArgs e)
        {
            verDatos();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            TransporteNuevo crear = new TransporteNuevo();
            crear.Closed += async (s, args) =>
            {
                await obtenerTransportes();
            };

            crear.Show();
        }

        private void FiltrarTransportes(string texto)
        {
            if (todosLosTransportes == null) return;

            var filtrados = todosLosTransportes
                .Where(t => t.matricula != null &&
                            t.matricula.ToLower().Contains(texto.ToLower()))
                .ToList();

            tbl_Transportes.ItemsSource = filtrados;
        }

        private void txbBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarTransportes(txbBusqueda.Text);
        }

        private void tbl_Transportes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tbl_Transportes.SelectedItem is Transporte transporteSeleccionado)
            {
                TransporteAdmin editar = new TransporteAdmin(transporteSeleccionado);

                editar.Closed += async (s, args) =>
                {
                    await obtenerTransportes();
                };


                editar.Show();
            }
        }
    }
}
