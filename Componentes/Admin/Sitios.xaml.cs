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
using chaski_tours_desk.Componentes.Admin.FormsInfo;
using chaski_tours_desk.Componentes.Admin.FormsAgregar;

namespace chaski_tours_desk.Componentes.Admin
{
    /// <summary>
    /// Lógica de interacción para Sitios.xaml
    /// </summary>
    public partial class Sitios : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/sitios/";
        public Sitios()
        {
            InitializeComponent();
        }
        private async Task obtenerSitios()
        {
            var sitios = await cliente.GetFromJsonAsync<List<Sitio>>(URL);

            tbl_Sitios.ItemsSource = sitios;
            tbl_Sitios.SelectedValuePath = "id_sitio";
        }

        public async void verSitios()
        {
            await obtenerSitios();
        }

        public void verDatos()
        {
            verSitios();
        }

        private void Sitios_Loaded(object sender, RoutedEventArgs e)
        {
            verDatos();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                new InfoSitio(int.Parse(tbl_Sitios.SelectedValue.ToString())).Show();
            }
            catch{
                MessageBox.Show("El sitio fue eliminado o no existe");
                verSitios();
            }
        }

        private void txbBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = txbBusqueda.Text.ToLower();
            tbl_Sitios.Items.Filter = (item) =>
            {
                var sitio = item as Sitio;
                if (sitio == null) return false;
                return sitio.nombre.ToLower().Contains(txt);
            };
        }

        private void AddSitio_Click(object sender, RoutedEventArgs e)
        {
            new AgregarSitio().Show();
        }
    }
}
