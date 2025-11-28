using chaski_tours_desk.Componentes.Admin.FormsAgregar;
using chaski_tours_desk.Componentes.Admin.FormsInfo;
using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Net.Http.Json;

namespace chaski_tours_desk.Componentes.Admin
{
    /// <summary>
    /// Lógica de interacción para Salidas.xaml
    /// </summary>
    public partial class Salidas : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/calendario/";

        List<Salida> lstSalidas = new List<Salida>();
        public Salidas()
        {
            InitializeComponent();
        }

        private async Task obtenerSalidas()
        {
            lstSalidas = await cliente.GetFromJsonAsync<List<Salida>>(URL);
            tbl_Salidas.ItemsSource = lstSalidas;
            tbl_Salidas.SelectedValuePath = "id_salida";
        }

        public async void verSalidas()
        {
            await obtenerSalidas();
        }

        private void verDatos()
        {
            verSalidas();
        }

        private void Salida_Loaded(object sender, RoutedEventArgs e)
        {
            verDatos();
        }

        private void txbBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = txbBusqueda.Text.ToLower();
            tbl_Salidas.Items.Filter = (item) =>
            {
                var salida = item as Salida;
                if (salida == null) return false;
                return salida.id_salida.ToString().Contains(txt);
            };
        }

        private void tbl_Salidas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {

                new InfoSalida(int.Parse(tbl_Salidas.SelectedValue.ToString())).Show();
            }
            catch
            {
                MessageBox.Show("La salida fue eliminada o no existe");
                verSalidas();
            }
        }

        private async void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            new AgregarSalida().Show();
        }
    }
}
