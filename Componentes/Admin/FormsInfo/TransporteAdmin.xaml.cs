using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

namespace chaski_tours_desk.Componentes.Admin.FormsInfo
{
    /// <summary>
    /// Lógica de interacción para TransporteAdmin.xaml
    /// </summary>
    public partial class TransporteAdmin : Window
    {

        private HttpClient cliente = new HttpClient();
        private string URL = "http://localhost:8000/api/transporte";
        public Transporte trans;
        public TransporteAdmin(Transporte x)
        {
            InitializeComponent();
            trans = x;
            configurardatos();
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

        private void configurardatos()
        {
            txt_matricula.Text = trans.matricula.ToString();
            txt_marca.Text = trans.marca.ToString();
            txt_modelo.Text = trans.modelo.ToString();
            txt_capacidad.Text = trans.capacidad.ToString();
            txt_anio.Text = trans.anio.ToString();


            if (trans.disponible == 1)
            {
                cmbDisponible.Text = "Disponible";
            }
            else
            {
                cmbDisponible.Text = "No Disponible";
            }

            if (trans.activo == 1)
            {
                cmbActivo.Text = "Activo";
            }
            else
            {
                cmbActivo.Text = "Inactivo";
            }
        }


        private async void BorrarTransporte()
        {
            await EliminarTransporteAsync();
        }
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            BorrarTransporte();
        }

        public async Task EliminarTransporteAsync()
        {

            HttpResponseMessage response = await cliente.DeleteAsync($"{URL}/{trans.id_vehiculo}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Transporte borrado correctamente");
                Close();
            }
            else
            {
                MessageBox.Show("Error al borrar el transporte");
            }
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Los campos Capacidad y Año deben ser números.");
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
            await ActualizarTransporteAsync();
        }

        public async Task ActualizarTransporteAsync()
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

            trans.matricula = txt_matricula.Text;
            trans.marca = txt_marca.Text;
            trans.modelo = txt_modelo.Text;
            trans.capacidad = int.Parse(txt_capacidad.Text);
            trans.año = txt_anio.Text;
            trans.disponible = dispo;
            trans.activo = act;

            string json = JsonSerializer.Serialize(trans);

            MessageBox.Show(json);

            HttpResponseMessage response = await cliente.PutAsJsonAsync($"{URL}/{trans.id_vehiculo}", trans);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Transporte actualizado correctamente");
                Close();
            }
            else
            {
                MessageBox.Show("Error al actualizar el transporte");
            }

        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
