using chaski_tours_desk.Componentes.Admin.FormsAgregar;
using chaski_tours_desk.Componentes.Admin.FormsInfo;
using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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

namespace chaski_tours_desk.Componentes.Admin
{
    /// <summary>
    /// Lógica de interacción para Tours.xaml
    /// </summary>
    public partial class Tours : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/tour/";
        public Tours()
        {
            InitializeComponent();
        }

        private async Task obtenerSitios()
        {
            var sitios = await cliente.GetFromJsonAsync<List<Tour>>(URL);


            tbl_Tours.ItemsSource = sitios;
        }

        private async void verTours()
        {
            await obtenerSitios();
        }

        private void verDatos()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verTours();
            }
        }

        private void Tours_Loaded(object sender, RoutedEventArgs e)
        {
            verDatos();
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tbl_Tours.SelectedItem is Tour tourSeleccionado)
            {
                new InfoTour(tourSeleccionado.id_tour).Show();
            }
            else
            {
                MessageBox.Show("Seleccione un tour válido.");
            }
        }

        private void txbBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = txbBusqueda.Text.ToLower();
            tbl_Tours.Items.Filter = (item) =>
            {
                var tour = item as Tour;
                if (tour == null) return false;
                return tour.nombre_tour.ToLower().Contains(txt);
            };
        }

        private void AddTour_Click(object sender, RoutedEventArgs e)
        {
            new AgregarTour().Show();
        }
    }
}
