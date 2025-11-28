using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chaski_tours_desk.Componentes.Admin.FormsAgregar
{
    /// <summary>
    /// Lógica de interacción para AgregarSitio.xaml
    /// </summary>
    public partial class AgregarSitio : Window
    {
        HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/sitios/";
        private string URL_Ubi = "http://localhost:8000/api/ubicaciones/";
        private string URL_Imagenes = "http://localhost:8000/api/imagenes/crear";
        public AgregarSitio()
        {
            InitializeComponent();
            habilitar(true);
            agregarHoraCmb();
            cargarDepartamentos();
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
            txburl.IsEnabled = valor;
        }

        private async void btnGuardarSitio_Click(object sender, RoutedEventArgs e)
        {
            if (!EntradasValidas()) {
                return;
            }
            var nuevaUbicacion = new Ubicacion
            {
                departamento = cmbDepartamento.SelectedItem.ToString(),
                municipio = txbMunicipio.Text,
                zona = txbZona.Text,
                calle = txbCalle.Text,
                latitud = txbLatitud.Text,
                longitud = txbLongitud.Text
            };
            var responseUbi = await cliente.PostAsJsonAsync(URL_Ubi, nuevaUbicacion);
            var idNuevaUbicacion = responseUbi.Content.ReadFromJsonAsync<Ubicacion>().Result.id_ubicacion;
            if (responseUbi.IsSuccessStatusCode)
            {
                MessageBox.Show("Ubicacion creada correctamente, id de nueva ubicacion: " + idNuevaUbicacion, "Éxito", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Error al crear la ubicacion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var nuevoSitio = new Sitio
            {
                nombre = txbNombreSitio.Text,
                desc_conceptual_sitio = txbDescConceptual.Text,
                desc_historica_sitio = txbDescHistorica.Text,
                costo_sitio = txbCostoSitio.Text == "Gratis" ? 0 : double.Parse(txbCostoSitio.Text.Replace("Bs.", "").Trim()),
                temporada_recomendada = cmbTemporada.Text,
                recomendacion_climatica = txbRecomendacion.Text,
                id_ubicacion = idNuevaUbicacion,
                horario_apertura = cmbApertura.SelectedItem.ToString(),
                horario_cierre = cmbCierre.SelectedItem.ToString(),
                Activo = cmbActivo.SelectedIndex
            };
            MessageBox.Show(JsonSerializer.Serialize(nuevoSitio));
            var response = await cliente.PostAsJsonAsync(URL, nuevoSitio);

            if (response.IsSuccessStatusCode) {
                MessageBox.Show("Sitio creado correctamente.", "Éxito", MessageBoxButton.OK);
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.admin.Sitios.verSitios();
                Close();
            }
            else {
                MessageBox.Show("Error al crear el sitio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                await cliente.DeleteFromJsonAsync<Ubicacion>(URL_Ubi + idNuevaUbicacion);
            };

            var nuevaImagen = new Imagen
            {
                id_img = idNuevaUbicacion,
                id_sitio = idNuevaUbicacion,
                url_img = txburl.Text
            };
            MessageBox.Show(JsonSerializer.Serialize(nuevaImagen));

            var reponseImg = await cliente.PostAsJsonAsync(URL_Imagenes, nuevaImagen);
            string resultado = await reponseImg.Content.ReadAsStringAsync();
            if (reponseImg.IsSuccessStatusCode)
            {
                MessageBox.Show("Imagen creada correctamente");
            }
            else
            {
                MessageBox.Show("Error al crear la imagen." + resultado, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
                string.IsNullOrWhiteSpace(txburl.Text) ||
                string.IsNullOrWhiteSpace(txbLatitud.Text) ||
                string.IsNullOrWhiteSpace(txbLongitud.Text))
            {
                MessageBox.Show("Todos los campos de texto son obligatorios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Validar campos numéricos
            if (!decimal.TryParse(txbCostoSitio.Text, out decimal costo) || costo < 0)
            {
                MessageBox.Show("El costo debe ser un número válido y positivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(txbLatitud.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double latitud) || latitud < -90 || latitud > 90)
            {
                MessageBox.Show("La latitud debe ser un número válido entre -90 y 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(txbLongitud.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double longitud) || longitud < -180 || longitud > 180)
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

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void agregarHoraCmb()
        {
            for (int i = 0; i <= 24; i++)
            {
                cmbApertura.Items.Add(i.ToString("D2") + ":00:00");
                cmbCierre.Items.Add(i.ToString("D2") + ":00:00");
            }
        }

        private void cargarDepartamentos() {
            List<string> departamentos = new List<string>
            {
                "La Paz", "Oruro", "Potosí", "Cochabamba", "Chuquisaca",
                "Tarija", "Santa Cruz", "Beni", "Pando", "El Alto"
            };

            departamentos.ForEach(departamento => cmbDepartamento.Items.Add(departamento));
        }
    }
}
