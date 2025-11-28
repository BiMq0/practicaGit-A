using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace chaski_tours_desk.Componentes.Admin.FormsAgregar
{
    public partial class FormularioAlojamiento : Window
    {
        //para show
        private readonly HttpClient _httpClient = new HttpClient();
        private int? _idAlojamiento;
        private bool _modoEdicion = false;
        public FormularioAlojamiento()
        {
            InitializeComponent();
            ConfigurarEstadoInicial();
        }
        //para show
        public FormularioAlojamiento(int? idAlojamiento = null)
        {
            InitializeComponent();
            _idAlojamiento = idAlojamiento;
            ConfigurarEstadoInicial();

            if (_idAlojamiento != null)
            {
                MostrarAlojamiento((int)_idAlojamiento);
                    
                groupBoxTiposHabitaciones.Visibility = Visibility.Visible;
            }
        }

        // parte de registro
        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            // Validación del nombre del alojamiento
            if (string.IsNullOrWhiteSpace(txtNombreAlojamiento.Text))
            {
                MessageBox.Show("Debe ingresar el nombre del alojamiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombreAlojamiento.Focus();
                return;
            }

            if (txtNombreAlojamiento.Text.Length > 25)
            {
                MessageBox.Show("El nombre del alojamiento no puede exceder los 25 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombreAlojamiento.Focus();
                return;
            }

            // Validación del número de estrellas
            if (string.IsNullOrWhiteSpace(txtNroEstrellas.Text))
            {
                MessageBox.Show("Debe ingresar el número de estrellas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNroEstrellas.Focus();
                return;
            }

            if (!double.TryParse(txtNroEstrellas.Text, out double estrellas) || estrellas < 0 || estrellas > 5)
            {
                MessageBox.Show("El número de estrellas debe ser un valor numérico entre 0 y 5.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNroEstrellas.Focus();
                return;
            }

            // Validación de habitaciones
            var habitaciones = new List<object>();
            int totalHabitaciones = 0;
            bool alMenosUnaHabitacion = false;

            if (!ValidarHabitacion(chkIndividual, txtCantidadIndividual, "Individual", 1, habitaciones, ref totalHabitaciones, ref alMenosUnaHabitacion) ||
                !ValidarHabitacion(chkDoble, txtCantidadDoble, "Doble", 2, habitaciones, ref totalHabitaciones, ref alMenosUnaHabitacion) ||
                !ValidarHabitacion(chkSuite, txtCantidadSuite, "Suite", 4, habitaciones, ref totalHabitaciones, ref alMenosUnaHabitacion) ||
                !ValidarHabitacion(chkFamiliar, txtCantidadFamiliar, "Familiar", 6, habitaciones, ref totalHabitaciones, ref alMenosUnaHabitacion))
            {
                return; // Si alguna validación falla, ya se mostró el mensaje
            }

            if (!alMenosUnaHabitacion)
            {
                MessageBox.Show("Debe seleccionar al menos un tipo de habitación con cantidad válida.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Preparar datos para la API
            var data = new
            {
                nombre_aloj = txtNombreAlojamiento.Text.Trim(),
                nro_estrellas = estrellas,
                nro_habitaciones = totalHabitaciones,
                Activo = true,
                habitaciones = habitaciones
            };

            try
            {
                // Mostrar mensaje de confirmación
                var confirmacion = MessageBox.Show("¿Está seguro que desea registrar este alojamiento?", "Confirmar registro", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmacion != MessageBoxResult.Yes)
                {
                    return;
                }

                var client = new HttpClient();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:8000/api/alojamiento-con-habitaciones", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Alojamiento y habitaciones registradas correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Devuelve control al UserControl Alojamientos
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Error al registrar: {response.StatusCode} - {responseBody}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error de conexión con la API: {ex.Message}", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarHabitacion(CheckBox check, TextBox cantidadBox, string tipo, int capacidad, List<object> lista, ref int total, ref bool alMenosUna)
        {
            if (check.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(cantidadBox.Text))
                {
                    MessageBox.Show($"Debe ingresar la cantidad para habitaciones {tipo}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cantidadBox.Focus();
                    return false;
                }

                if (!int.TryParse(cantidadBox.Text, out int cantidad) || cantidad <= 0)
                {
                    MessageBox.Show($"La cantidad para habitaciones {tipo} debe ser un número entero positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    cantidadBox.Focus();
                    return false;
                }

                for (int i = 0; i < cantidad; i++)
                {
                    lista.Add(new
                    {
                        tipo_habitacion = tipo,
                        capacidad = capacidad,
                        disponible = true
                    });
                    total++;
                }

                alMenosUna = true;
            }
            return true;
        }

        private void LimpiarFormulario()
        {
            txtNombreAlojamiento.Clear();
            txtNroEstrellas.Clear();
            txtCantidadIndividual.Clear();
            txtCantidadDoble.Clear();
            txtCantidadSuite.Clear();
            txtCantidadFamiliar.Clear();

            chkIndividual.IsChecked = false;
            chkDoble.IsChecked = false;
            chkSuite.IsChecked = false;
            chkFamiliar.IsChecked = false;

            txtNombreAlojamiento.Focus();
        }


        private void btnCancelar_Click_1(object sender, RoutedEventArgs e)
        {
            _modoEdicion = false;
            txtNombreAlojamiento.IsReadOnly = true;
            txtNroEstrellas.IsReadOnly = true;

            dgHabitaciones.IsReadOnly = true;

            btnEditar.Visibility = Visibility.Visible;
            btnGuardar.Visibility = Visibility.Collapsed;
            btnCancelar.Visibility = Visibility.Collapsed;

            panelAgregarHabitacion.Visibility = Visibility.Collapsed;

            // Recarga para descartar cambios
            MostrarAlojamiento((int)_idAlojamiento);
            var confirmacion = MessageBox.Show("¿Está seguro que desea cancelar ?", "Confirmar cancelación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmacion == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        // fin de registro

        // parte show
        private void ConfigurarEstadoInicial()
        {
            // Ocultar todos los botones al inicio
            borderRegistrar.Visibility = Visibility.Visible;
            borderEditar.Visibility = Visibility.Collapsed;
            borderGuardar.Visibility = Visibility.Collapsed;
            borderEliminar.Visibility = Visibility.Collapsed;

            // Ocultar secciones hasta tener datos
            dgHabitaciones.Visibility = Visibility.Collapsed;
            grupoHabitaciones.Visibility = Visibility.Collapsed;

            // Tipos de habitaciones solo visibles para nuevo registro
            groupBoxTiposHabitaciones.Visibility = Visibility.Visible;
        }

        private async void MostrarAlojamiento(int id)
        {
            try
            {
                string url = $"http://localhost:8000/api/habitaciones/alojamiento/{id}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RespuestaHabitaciones>();
                    if (result != null)
                    {
                        txtNombreAlojamiento.Text = result.alojamiento;
                        txtNroEstrellas.Text = result.nro_estrellas.ToString();
                        txtNombreAlojamiento.IsReadOnly = true;
                        txtNroEstrellas.IsReadOnly = true;
                        result.data.ForEach(h =>
                        {
                            h.original_disponible = h.disponible_raw;
                            h.ActualizarTextoDisponibilidad();
                        });

                        dgHabitaciones.ItemsSource = result.data;
                        dgHabitaciones.Visibility = Visibility.Visible;
                        grupoHabitaciones.Visibility = Visibility.Visible;

                        // Ocultar panel de tipos de habitaciones (solo en nuevo registro)
                        groupBoxTiposHabitaciones.Visibility = Visibility.Collapsed;

                        // Mostrar solo los botones adecuados
                        borderRegistrar.Visibility = Visibility.Collapsed;
                        borderGuardar.Visibility = Visibility.Collapsed;

                        borderEditar.Visibility = Visibility.Visible;
                        borderEliminar.Visibility = Visibility.Visible;

                        btnCancelar.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo cargar el alojamiento", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con el servidor: " + ex.Message);
            }
        }

        public class HabitacionData : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool _disponible_raw;
            private string _disponible;

            public int nro { get; set; }
            public string tipo { get; set; }
            public int capacidad { get; set; }

            public bool disponible_raw
            {
                get => _disponible_raw;
                set
                {
                    if (_disponible_raw != value)
                    {
                        _disponible_raw = value;
                        disponible = value ? "Disponible" : "Ocupada";
                        OnPropertyChanged(nameof(disponible_raw));
                        OnPropertyChanged(nameof(disponible));
                    }
                }
            }

            public string disponible
            {
                get => _disponible;
                set
                {
                    if (_disponible != value)
                    {
                        _disponible = value;
                        disponible_raw = value == "Disponible";
                        OnPropertyChanged(nameof(disponible));
                        OnPropertyChanged(nameof(disponible_raw));
                    }
                }
            }

            public bool original_disponible { get; set; }

            public bool HasChanged => disponible_raw != original_disponible;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public void ActualizarTextoDisponibilidad()
            {
                disponible = disponible_raw ? "Disponible" : "Ocupada";
            }

        }


        public class RespuestaHabitaciones
        {
            public string alojamiento { get; set; }
            public double nro_estrellas { get; set; }
            public List<HabitacionData> data { get; set; }
        }

        // para editar
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            _modoEdicion = true;
            txtNombreAlojamiento.IsReadOnly = false;
            txtNroEstrellas.IsReadOnly = false;

            dgHabitaciones.IsReadOnly = false; // Para permitir editar disponibilidad
            dgHabitaciones.Columns.Where(c => c.Header.ToString() != "Disponibilidad").ToList().ForEach(c => c.IsReadOnly = true);

            borderEditar.Visibility = Visibility.Collapsed;
            borderGuardar.Visibility = Visibility.Visible;
            btnCancelar.Visibility = Visibility.Visible;

            panelAgregarHabitacion.Visibility = Visibility.Visible;
        }
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1) Validar campos del alojamiento
                if (string.IsNullOrWhiteSpace(txtNombreAlojamiento.Text))
                {
                    MessageBox.Show("Debe ingresar el nombre del alojamiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(txtNroEstrellas.Text, out double estrellas) || estrellas < 0 || estrellas > 5)
                {
                    MessageBox.Show("El número de estrellas debe ser entre 0 y 5.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 2) Actualizar alojamiento
                var alojPayload = new
                {
                    nombre_aloj = txtNombreAlojamiento.Text,
                    nro_estrellas = estrellas
                };

                var alojResponse = await _httpClient.PutAsJsonAsync($"http://localhost:8000/api/alojamientos/{_idAlojamiento}", alojPayload);

                if (!alojResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Error al actualizar alojamiento", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3) Actualizar habitaciones
                var habitaciones = dgHabitaciones.ItemsSource as List<HabitacionData>;
                if (habitaciones != null)
                {
                    foreach (var item in habitaciones.Where(h => h.HasChanged))
                    {
                        var payload = new
                        {
                            nro_habitacion = item.nro,
                            id_alojamiento = _idAlojamiento,
                            disponible = item.disponible_raw
                        };

                        var response = await _httpClient.PutAsJsonAsync(
                            "http://localhost:8000/api/habitaciones/actualizar",
                            payload
                        );

                        if (!response.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Error al actualizar habitación nro {item.nro}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }

                // 4) Cerrar el formulario y notificar éxito
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //agregar habitacion
        private async void btnAgregarHabitacion_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTipoHabitacion.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tipo de habitación para agregar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tipo = ((ComboBoxItem)cmbTipoHabitacion.SelectedItem).Content.ToString();

            // Asignar capacidad según el tipo
            int capacidad = 1; 

            if (tipo == "Individual")
                capacidad = 1;
            else if (tipo == "Doble")
                capacidad = 2;
            else if (tipo == "Suite")
                capacidad = 4;
            else if (tipo == "Familiar")
                capacidad = 6;


            var payload = new
            {
                id_alojamiento = _idAlojamiento,
                tipo_habitacion = tipo,
                capacidad = capacidad,
                disponible = true
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/api/habitaciones", payload);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Habitación agregada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Actualizar número de habitaciones
                    var totalActual = ((List<HabitacionData>)dgHabitaciones.ItemsSource)?.Count ?? 0;
                    var alojamientoUpdate = new { nro_habitaciones = totalActual + 1 };

                    await _httpClient.PutAsJsonAsync($"http://localhost:8000/api/alojamientos/{_idAlojamiento}", alojamientoUpdate);

                    // Recargar habitaciones sin salir del modo edición
                    CargarHabitacionesSinReset();
                }
                else
                {
                    MessageBox.Show("Error al agregar habitación", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CargarHabitacionesSinReset()
        {
            try
            {
                string url = $"http://localhost:8000/api/habitaciones/alojamiento/{_idAlojamiento}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RespuestaHabitaciones>();
                    if (result != null)
                    {
                        // Actualizar las propiedades para que la UI muestre bien el estado
                        result.data.ForEach(h =>
                        {
                            h.original_disponible = h.disponible_raw;
                            h.ActualizarTextoDisponibilidad(); // para que el string 'disponible' esté sincronizado
                        });
                       

                        dgHabitaciones.ItemsSource = result.data;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al recargar habitaciones: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgHabitaciones_CurrentCellChanged(object sender, EventArgs e)
        {
            if (_modoEdicion)
            {
                // Detecta si al menos una habitación fue modificada
                var habitaciones = dgHabitaciones.ItemsSource as List<HabitacionData>;
                if (habitaciones != null && habitaciones.Any(h => h.HasChanged))
                {
                    borderGuardar.Visibility = Visibility.Visible;
                }
            }
        }
        private void dgHabitaciones_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var habitaciones = dgHabitaciones.ItemsSource as List<HabitacionData>;
                if (habitaciones != null && habitaciones.Any(h => h.HasChanged))
                {
                    borderGuardar.Visibility = Visibility.Visible;
                }
                else
                {
                    borderGuardar.Visibility = Visibility.Collapsed;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        //parte eliminar

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_idAlojamiento == null)
            {
                MessageBox.Show("No se ha cargado ningún alojamiento para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirmacion = MessageBox.Show("¿Está seguro que desea eliminar este alojamiento?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmacion != MessageBoxResult.Yes)
                return;

            try
            {
                var response = await _httpClient.PutAsync($"http://localhost:8000/api/alojamientos/desactivar/{_idAlojamiento}", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Alojamiento eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al eliminar el alojamiento: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






    }
}