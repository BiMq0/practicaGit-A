using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para TransporteNuevo.xaml
    /// </summary>
    public partial class TransporteNuevo : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/transporte/crear";
        public TransporteNuevo()
        {
            InitializeComponent();
            habilitar(true);
        }
        private void habilitar(bool valor)
        {
            txt_matricula.IsEnabled = valor;
            txt_marca.IsEnabled = valor;
            txt_modelo.IsEnabled = valor;
            txt_capacidad.IsEnabled = valor;
            txt_anio.IsEnabled = valor;
            cmbDisponible.IsEnabled = valor;
            cmbActivo.IsEnabled = valor;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Enviar_Click(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                mandarTransporte();
            }

        }
        private bool validar()
        {
            if (txt_matricula.Text == "" ||
            txt_marca.Text == "" ||
            txt_modelo.Text == "" ||
            txt_capacidad.Text == "" ||
            txt_anio.Text == "" ||
            cmbDisponible.Text == "" ||
            cmbActivo.Text == "")
            {
                MessageBox.Show("llene todos los campos");
                return false;
            }

            if (!int.TryParse(txt_capacidad.Text, out int capacidad) ||
                !int.TryParse(txt_anio.Text, out int anio))
            {
                MessageBox.Show("Los campos Capacidad y Año  deben ser números.");
                return false;
            }

            int anioActual = DateTime.Now.Year;

            if (anio < 1950 || anio > anioActual + 1)
            {
                MessageBox.Show($"El año debe estar entre 1950 y {anioActual + 1}.");
                return false;
            }
            if (txt_matricula.Text.Length > 8)
            {
                MessageBox.Show("La matricula debe de ser de 8 caracteres");
                return false;
            }
            if (capacidad > 80 && capacidad < 0)
            {
                MessageBox.Show("El campo capacidad no debe ser mayor a 80 y menor a 0");
                return false;
            }

            return true;
        }
        private async void mandarTransporte()
        {
            await CrearTransporteAsync();
        }
        public async Task CrearTransporteAsync()
        {
            int dispo = 0;
            if (cmbDisponible.Text == "Disponible")
            {
                dispo = 1;
            }
            int act = 0;
            if (cmbActivo.Text == "Activo")
            {
                act = 1;
            }

            var transporte = new Transporte
            {
                id_vehiculo = 0,
                matricula = txt_matricula.Text,
                marca = txt_marca.Text,
                modelo = txt_modelo.Text,
                capacidad = int.Parse(txt_capacidad.Text),
                año = txt_anio.Text,
                disponible = dispo,
                activo = act
            };
            string json = JsonSerializer.Serialize(transporte);
            MessageBox.Show(json);
            HttpResponseMessage response = await cliente.PostAsJsonAsync(URL, transporte);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Transporte creado correctamente");
                Close();
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show("Error al crear el transporte");
            }

        }


    }
}