using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace chaski_tours_desk.Componentes.Admin.FormsInfo
{
    /// <summary>
    /// Lógica de interacción para InfoSitio.xaml
    /// </summary>
    public partial class InfoSitio : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/sitios/";
        private string URL_Ubi = "http://localhost:8000/api/ubicaciones/";

        public InfoSitio(int id_sitio)
        {
            InitializeComponent();
            cargarDepartamentos();
            agregarHoraCmb();

            try
            {
                var sitio = obtenerSitio(id_sitio);
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

        private Sitio obtenerSitio(int id_sitio)
        {   
            var sitio = cliente.GetFromJsonAsync<Sitio>(URL + id_sitio).Result;
            
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

        private int editUpdate = 0;

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
        private async void btnEditarSitio_Click(object sender, RoutedEventArgs e)
        {
            editUpdate++;
            if (editUpdate == 1)
            {
                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesUser"];
                btnEditarSitio.Style = (Style)Application.Current.Resources["UserButtonStyle"];
                btnEditarSitio.Content = "Guardar";
                btnEliminarSitio.Content = "Cancelar";
                habilitar(true);

                btnVolver.Visibility = Visibility.Collapsed;
                brdBtnVolver.Visibility = Visibility.Collapsed;
            }
            else if(editUpdate == 2){
                if (!EntradasValidas())
                {
                    editUpdate = 1;
                    return;
                }
                var nuevoSitio = new Sitio{
                    desc_conceptual_sitio = txbDescConceptual.Text,
                    desc_historica_sitio = txbDescHistorica.Text,
                    costo_sitio = txbCostoSitio.Text == "Gratis" ? 0 : double.Parse(txbCostoSitio.Text.Replace("Bs.", "").Trim()),
                    temporada_recomendada = cmbTemporada.SelectedItem.ToString(),
                    recomendacion_climatica = txbRecomendacion.Text,
                    horario_apertura = cmbApertura.SelectedItem.ToString(),
                    horario_cierre = cmbCierre.SelectedItem.ToString(),
                    Activo = cmbActivo.SelectedIndex 
                };
                var nuevaUbicacion = new Ubicacion
                {
                    departamento = cmbDepartamento.SelectedItem.ToString(),
                    municipio = txbMunicipio.Text,
                    zona = txbZona.Text,
                    calle = txbCalle.Text,
                    latitud = txbLatitud.Text,
                    longitud = txbLongitud.Text
                };
                var responseUbi = await cliente.PutAsJsonAsync(URL_Ubi + hiddenIdUbi.Text, nuevaUbicacion);
                var response = await cliente.PutAsJsonAsync(URL + hiddenId.Text, nuevoSitio) ;

                if (response.IsSuccessStatusCode && responseUbi.IsSuccessStatusCode)MessageBox.Show("Sitio actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Error al actualizar el sitio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesSecundarios"];
                btnEditarSitio.Style = (Style)Application.Current.Resources["TertiaryButtonStyle"];
                btnEditarSitio.Content = "Editar";
                btnEliminarSitio.Content = "Eliminar";
                habilitar(false);
                btnEliminarSitio.Visibility = Visibility.Visible;
                btnVolver.Visibility = Visibility.Visible;
                brdBtnVolver.Visibility = Visibility.Visible;
                
                obtenerUbicacion(obtenerSitio(int.Parse(hiddenId.Text)));

                editUpdate = 0;   
            }
        }

        private bool EntradasValidas()
        {
            // Validar campos de texto obligatorios
            if (string.IsNullOrWhiteSpace(txbNombreSitio.Text) ||
                string.IsNullOrWhiteSpace(txbDescConceptual.Text) ||
                string.IsNullOrWhiteSpace(txbDescHistorica.Text) ||
                string.IsNullOrWhiteSpace(txbCostoSitio.Text) ||
                string.IsNullOrWhiteSpace(txbMunicipio.Text) ||
                string.IsNullOrWhiteSpace(txbZona.Text) ||
                string.IsNullOrWhiteSpace(txbCalle.Text) ||
                string.IsNullOrWhiteSpace(txbLatitud.Text) ||
                string.IsNullOrWhiteSpace(txbLongitud.Text))
            {
                MessageBox.Show("Todos los campos de texto son obligatorios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validar campos numéricos
            if (!decimal.TryParse(txbCostoSitio.Text.Replace("Bs.", ""), out decimal costo) || costo < 0)
            {
                MessageBox.Show("El costo debe ser un número válido y positivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(txbLatitud.Text, out double latitud) || latitud < -90 || latitud > 90)
            {
                MessageBox.Show("La latitud debe ser un número válido entre -90 y 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(txbLongitud.Text, out double longitud) || longitud < -180 || longitud > 180)
            {
                MessageBox.Show("La longitud debe ser un número válido entre -180 y 180", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validar comboboxes
            if (cmbTemporada.SelectedItem == null ||
                cmbApertura.SelectedItem == null ||
                cmbCierre.SelectedItem == null ||
                cmbDepartamento.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar valores en todos los desplegables", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validar horario (cierre debe ser después de apertura)
            if (cmbApertura.SelectedIndex >= cmbCierre.SelectedIndex)
            {
                MessageBox.Show("La hora de cierre debe ser posterior a la hora de apertura", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnEliminarSitio_Click(object sender, RoutedEventArgs e)
        {
            if (editUpdate == 1)
            {
                brdEditar.Style = (Style)Application.Current.Resources["BordeBotonesSecundarios"];
                btnEditarSitio.Style = (Style)Application.Current.Resources["TertiaryButtonStyle"];
                btnEditarSitio.Content = "Editar";
                btnEliminarSitio.Content = "Eliminar";
                habilitar(false);

                btnVolver.Visibility = Visibility.Visible;
                brdBtnVolver.Visibility = Visibility.Visible;

                cargarDatos(obtenerSitio(int.Parse(hiddenId.Text)));
                obtenerUbicacion(obtenerSitio(int.Parse(hiddenId.Text)));

                editUpdate = 0;
            }
            else if (editUpdate == 0)
            {
                var result = MessageBox.Show("¿Está seguro que quiere eliminar el sitio?", "Eliminar", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    cliente.DeleteFromJsonAsync<Sitio>(URL + hiddenId.Text);
                }

                MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.admin.Sitios.verSitios();
                Close();
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void agregarHoraCmb() {
            for (int i = 0; i <= 24; i++) { 
                cmbApertura.Items.Add(i.ToString("D2") + ":00:00");
                cmbCierre.Items.Add(i.ToString("D2") + ":00:00");
            }
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
    }
}
