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
using chaski_tours_desk.Modelos;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para DatosSitio.xaml
    /// </summary>
    public partial class DatosSitio : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/sitios/";
        private string URL_Ubi = "http://localhost:8000/api/ubicaciones/";
        public DatosSitio()
        {
            InitializeComponent();
        }

        public void cargarTodosDatos()
        {
            cargarDepartamentos();
            agregarHoraCmb();

            try
            {
                var sitio = obtenerSitio();
                cargarDatos(sitio);
                obtenerUbicacion(sitio);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}");
            }
        }



        private void cargarDatos(Sitio sitio)
        {
            hiddenId.Text = sitio.id_sitio.ToString();
            txbNombreSitio.Text = sitio.nombre;
            txbDescConceptual.Text = sitio.desc_conceptual_sitio;
            txbDescHistorica.Text = sitio.desc_historica_sitio;
            txbCostoSitio.Text = sitio.costo_sitio == 0 ? "Gratis" : "Bs." + sitio.costo_sitio.ToString();
            cmbTemporada.Text = sitio.temporada_recomendada;
            txbRecomendacion.Text = sitio.recomendacion_climatica;
            cmbApertura.SelectedIndex = int.Parse(sitio.horario_apertura.Split(':')[0]);
            cmbCierre.SelectedIndex = int.Parse(sitio.horario_cierre.Split(':')[0]);
            cmbActivo.SelectedIndex = sitio.Activo == 1 ? 1 : 0;
        }

        private Sitio obtenerSitio()
        {
            int id = int.Parse(hiddenId.Text);
            var sitio = cliente.GetFromJsonAsync<Sitio>(URL + id).Result;

            return sitio;
        }

        private void obtenerUbicacion(Sitio sitio)
        {
            var ubicacion = cliente.GetFromJsonAsync<Ubicacion>(URL_Ubi + sitio.id_ubicacion).Result;
            cargarMapa(ubicacion);
            cargarUbicacion(ubicacion);
        }

        private void cargarUbicacion(Ubicacion ubicacion)
        {
            hiddenIdUbi.Text = ubicacion.id_ubicacion.ToString();
            cmbDepartamento.SelectedValue = ubicacion.departamento;
            txbMunicipio.Text = ubicacion.municipio;
            txbZona.Text = ubicacion.zona;
            txbCalle.Text = ubicacion.calle;
            txbLatitud.Text = ubicacion.latitud;
            txbLongitud.Text = ubicacion.longitud;
            habilitar(false);
        }

        private void cargarMapa(Ubicacion ubi)
        {
            try
            {
                var browser = new WebBrowser();

                if (double.TryParse(ubi.latitud, out double lat) &&
                    double.TryParse(ubi.longitud, out double lng))
                {
                    string html = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.7.1/dist/leaflet.css"" />
                        <script src=""https://unpkg.com/leaflet@1.7.1/dist/leaflet.js""></script>
                        <style>
                            #map {{ height: 500px; width: 100%; }}
                            body {{ margin: 0; padding: 0; }}
                        </style>
                    </head>
                    <body>
                    <div id=""map""></div>
                    <script>
                        var map = L.map('map').setView([{lat}, {lng}], 15);
                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png').addTo(map);
                        L.marker([{lat}, {lng}]).addTo(map);
                    </script>
                    </body>
                    </html>";

                    browser.NavigateToString(html);
                }
                else
                {
                    browser.NavigateToString("<html><body><h3>Error en coordenadas</h3></body></html>");
                }

                browser.Height = 500;
                mapa.Child = browser;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void habilitar(bool valor)
        {
            txbNombreSitio.IsEnabled = valor;
            txbDescConceptual.IsEnabled = valor;
            txbDescHistorica.IsEnabled = valor;
            txbCostoSitio.IsEnabled = valor;
            cmbTemporada.IsEnabled = valor;
            txbRecomendacion.IsEnabled = valor;
            cmbApertura.IsEnabled = valor;
            cmbCierre.IsEnabled = valor;
            cmbActivo.IsEnabled = valor;
            cmbDepartamento.IsEnabled = valor;
            txbMunicipio.IsEnabled = valor;
            txbZona.IsEnabled = valor;
            txbCalle.IsEnabled = valor;
            txbLatitud.IsEnabled = valor;
            txbLongitud.IsEnabled = valor;
        }
        private void cargarDepartamentos()
        {
            List<string> departamentos = new List<string>
            {
                "La Paz", "Oruro", "Potosí", "Cochabamba", "Chuquisaca",
                "Tarija", "Santa Cruz", "Beni", "Pando", "El Alto"
            };

            departamentos.ForEach(departamento => cmbDepartamento.Items.Add(departamento));
        }
        private void agregarHoraCmb()
        {
            for (int i = 0; i <= 24; i++)
            {
                cmbApertura.Items.Add(i.ToString("D2") + ":00:00");
                cmbCierre.Items.Add(i.ToString("D2") + ":00:00");
            }
        }
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.usuario.datosSitio.Visibility = Visibility.Collapsed;
            mainWindow.usuario.listadoSitios.Visibility = Visibility.Visible;
            mainWindow.usuario.btnReservar.Visibility = Visibility.Visible;
        }

    }
}
