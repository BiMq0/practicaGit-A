using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using chaski_tours_desk.Modelos;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;


namespace chaski_tours_desk.Componentes.Admin.FormsAgregar
{
    public partial class FormularioCliente : Window
    {
        private readonly HttpClient httpClient = new HttpClient();
        private string tipoSeleccionado = "";

        // parte para show
        public bool ModoVisualizacion { get; set; } = false;
        // parte put
        private string codigoClienteActual;
        private string tipoClienteActual;
        private bool modoEdicion = false;


        public FormularioCliente()
        {
            InitializeComponent();
            // Configuración inicial
            panelTurista.Visibility = Visibility.Collapsed;
            panelInstitucion.Visibility = Visibility.Collapsed;

            btnRegistrar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Collapsed;
        }

        private async void cbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTipo.SelectedItem == null) return;

            tipoSeleccionado = (cbTipo.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Ocultar todo inicialmente
            panelTurista.Visibility = Visibility.Collapsed;
            panelInstitucion.Visibility = Visibility.Collapsed;
            btnRegistrar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Collapsed;

            if (tipoSeleccionado == "Turista")
            {
                panelTurista.Visibility = Visibility.Visible;
                await CargarNacionalidades();
            }
            else if (tipoSeleccionado == "Institución")
            {
                panelInstitucion.Visibility = Visibility.Visible;
                await CargarNacionalidades();
            }

            // Mostrar registrar solo si NO estás en modo visualización
            if (!ModoVisualizacion)
                btnRegistrar.Visibility = Visibility.Visible;
        }


        private async Task CargarNacionalidades()
        {
            ComboBox comboBox = tipoSeleccionado == "Turista" ? cbNacionalidadTurista : cbNacionalidadInstitucion;

            try
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("Cargando...");

                // 1. Hacer la petición GET a la API
                HttpResponseMessage response = await httpClient.GetAsync("http://localhost:8000/api/nacionalidades");

                // 2. Verificar si la respuesta fue exitosa
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error en la API: {response.StatusCode} - {response.ReasonPhrase}");
                }

                // 3. Leer el contenido como string
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // 4. Deserializar a una lista de Nacionalidad
                List<Nacionalidad> nacionalidades = JsonSerializer.Deserialize<List<Nacionalidad>>(jsonResponse);

                // 5. Limpiar y llenar el ComboBox
                comboBox.Items.Clear();

                if (nacionalidades != null && nacionalidades.Count > 0)
                {
                    foreach (Nacionalidad nacionalidad in nacionalidades)
                    {
                        // Mostrar solo el nombre de la nacionalidad en el ComboBox
                        comboBox.Items.Add(nacionalidad.nacionalidad);
                    }
                    comboBox.SelectedIndex = 0;
                }
                else
                {
                    comboBox.Items.Add("No hay nacionalidades disponibles");
                }
            }
            catch (JsonException jsonEx)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("Error en formato JSON");
                MessageBox.Show($"Error al procesar la respuesta JSON:\n{jsonEx.Message}",
                              "Error de Formato", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (HttpRequestException httpEx)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("Error de conexión");
                MessageBox.Show($"Error al conectar con el servidor:\n{httpEx.Message}",
                              "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("Error inesperado");
                MessageBox.Show($"Error inesperado:\n{ex.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            if (tipoSeleccionado == "Turista")
            {
                await RegistrarTurista();
            }
            else if (tipoSeleccionado == "Institución")
            {
                await RegistrarInstitucion();
            }
            // En caso de mantener la ventana abierta después:
            // btnRegistrar.Visibility = Visibility.Collapsed;
            // btnEditar.Visibility = Visibility.Visible;
            // cbTipo.IsEnabled = false;

        }
        // Método para validar contraseña
        private bool ValidarContrasenia(string contrasenia)
        {
            if (string.IsNullOrWhiteSpace(contrasenia)) return false;

            if (contrasenia.Length < 8) return false;

            // Verificar mayúscula, minúscula y carácter especial
            bool tieneMayuscula = false;
            bool tieneMinuscula = false;
            bool tieneEspecial = false;

            foreach (char c in contrasenia)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                if (char.IsLower(c)) tieneMinuscula = true;
                if (!char.IsLetterOrDigit(c)) tieneEspecial = true;
            }

            return tieneMayuscula && tieneMinuscula && tieneEspecial;
        }
        private bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = email.Trim(); // IMPORTANTE

                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool ValidarTelefono(string telefono)
        {
            
            return !string.IsNullOrWhiteSpace(telefono) && telefono.Length >= 7;
        }
        private async Task RegistrarTurista()
        {
            // Validar contraseña
            if (!ValidarContrasenia(txtContrasenia.Password))
            {
                MessageBox.Show("La contraseña debe tener:\n- Mínimo 8 caracteres\n- 1 mayúscula\n- 1 minúscula\n- 1 carácter especial",
                              "Contraseña inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar documento
            if (txtDocumento.Text.Length < 7)
            {
                MessageBox.Show("El documento debe tener al menos 7 caracteres",
                              "Documento inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar email
            if (!ValidarEmail(txtCorreo.Text))
            {
                MessageBox.Show("Por favor ingrese un correo electrónico válido",
                              "Correo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar teléfono
            if (!ValidarTelefono(txtTelefono.Text))
            {
                MessageBox.Show("Por favor ingrese un número de teléfono válido",
                              "Teléfono inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar fecha de nacimiento
            if (dpFechaNac.SelectedDate == null)
            {
                MessageBox.Show("Por favor seleccione una fecha de nacimiento",
                              "Fecha requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var turistaData = new
                {
                    correo_electronico = txtCorreo.Text,
                    contrasenia = txtContrasenia.Password,
                    documento = txtDocumento.Text,
                    nombre = txtNombre.Text,
                    ap_pat = txtApPat.Text,
                    ap_mat = txtApMat.Text,
                    fecha_nac = dpFechaNac.SelectedDate?.ToString("yyyy-MM-dd"),
                    nacionalidad = cbNacionalidadTurista.SelectedItem?.ToString(),
                    telefono = txtTelefono.Text
                };

                var response = await httpClient.PostAsJsonAsync(
                    "http://localhost:8000/api/visitantes/turistas/crear",
                    turistaData
                );

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Turista registrado con éxito!", "Éxito",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    // Cierra el formulario
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al registrar turista: {errorContent}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar turista: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task RegistrarInstitucion()
        {
            // Validar contraseña
            if (!ValidarContrasenia(txtContraseniaInst.Password))
            {
                MessageBox.Show("La contraseña debe tener:\n- Mínimo 8 caracteres\n- 1 mayúscula\n- 1 minúscula\n- 1 carácter especial",
                              "Contraseña inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Validar email
            if (!ValidarEmail(txtCorreoInst.Text) && !ValidarEmail(txtCorreoRepresent.Text))
            {
                MessageBox.Show("Por favor ingrese un correo electrónico válido",
                              "Correo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar teléfono
            if (!ValidarTelefono(txtTelefonoInst.Text) && !ValidarTelefono(txtTelefonoRepresent.Text))
            {
                MessageBox.Show("Por favor ingrese un número de teléfono válido",
                              "Teléfono inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var institucionData = new
                {
                    nombre = txtNombreInst.Text,
                    correo_electronico = txtCorreoInst.Text,
                    contrasenia = txtContraseniaInst.Password,
                    nacionalidad = cbNacionalidadInstitucion.SelectedItem?.ToString(),
                    telefono = txtTelefonoInst.Text,
                    nombre_represent = txtNombreRepresent.Text,
                    ap_pat_represent = txtApPatRepresent.Text,
                    correo_electronico_represent = txtCorreoRepresent.Text,
                    telefono_represent = txtTelefonoRepresent.Text
                };

                var response = await httpClient.PostAsJsonAsync(
                    "http://localhost:8000/api/visitantes/instituciones",
                    institucionData
                );

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Institución registrada con éxito!", "Éxito",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    // Cierra el formulario
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al registrar institución: {errorContent}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar institución: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //parte contrasenas
        private void chkVerContrasenia_Checked(object sender, RoutedEventArgs e)
        {
            txtContraseniaVisible.Text = txtContrasenia.Password;
            txtContrasenia.Visibility = Visibility.Collapsed;
            txtContraseniaVisible.Visibility = Visibility.Visible;
        }

        private void chkVerContrasenia_Unchecked(object sender, RoutedEventArgs e)
        {
            txtContrasenia.Password = txtContraseniaVisible.Text;
            txtContraseniaVisible.Visibility = Visibility.Collapsed;
            txtContrasenia.Visibility = Visibility.Visible;
        }
        private void chkVerContrasenia_CheckedInst(object sender, RoutedEventArgs e)
        {
            txtContraseniaInstVisible.Text = txtContraseniaInst.Password;
            txtContraseniaInst.Visibility = Visibility.Collapsed;
            txtContraseniaInstVisible.Visibility = Visibility.Visible;
        }

        private void chkVerContrasenia_UncheckedInst(object sender, RoutedEventArgs e)
        {
            txtContraseniaInst.Password = txtContraseniaInstVisible.Text;
            txtContraseniaInstVisible.Visibility = Visibility.Collapsed;
            txtContraseniaInst.Visibility = Visibility.Visible;
        }


        // ---------------------------registro fin de tipo de cliente -----------------------------------
        // Método para cargar datos de turista
        public async void CargarDatosTurista(Turista turista)
        {
            //parte de put
            // Guardar datos originales
            codigoClienteActual = turista.cod_visitante;
            tipoClienteActual = "Turista";
            // Configurar modo visualización
            ModoVisualizacion = true;
            cbTipo.SelectedIndex = 0; // Turista
            // Mostrar el panel Turista
            panelTurista.Visibility = Visibility.Visible;
            // çragar de nuevo nacionalidades
            await CargarNacionalidades();
            cbNacionalidadTurista.SelectedItem = cbNacionalidadTurista.Items
            .Cast<string>()
            .FirstOrDefault(item => item.Equals(turista.nacionalidad, StringComparison.OrdinalIgnoreCase));

            // Llenar campos
            txtDocumento.Text = turista.documento;
            txtNombre.Text = turista.nombre;
            txtApPat.Text = turista.ap_pat;
            txtApMat.Text = turista.ap_mat;
            dpFechaNac.SelectedDate = DateTime.Parse(turista.fecha_nac);
            txtTelefono.Text = turista.telefono;
            txtCorreo.Text = turista.correo_electronico;
            txtContrasenia.Password = turista.contrasenia;
            txtContraseniaVisible.Text = turista.contrasenia;

            // Deshabilitar controles
            HabilitarControles(false);

            //para editicion
            btnRegistrar.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Visible;
        }

        // Método para cargar datos de institución
        public async void CargarDatosInstitucion(Institucion institucion)
        {
            codigoClienteActual = institucion.cod_visitante;
            tipoClienteActual = "Institución";
            ModoVisualizacion = true;
            cbTipo.SelectedIndex = 1;

            panelTurista.Visibility = Visibility.Collapsed;
            panelInstitucion.Visibility = Visibility.Visible;
            await CargarNacionalidades();

            // Seleccionar nacionalidad 
            cbNacionalidadInstitucion.SelectedItem = cbNacionalidadInstitucion.Items
                .Cast<string>()
                .FirstOrDefault(item => item.Equals(institucion.nacionalidad, StringComparison.OrdinalIgnoreCase));

            txtNombreInst.Text = institucion.nombre;
            txtCorreoInst.Text = institucion.correo_electronico;
            txtTelefonoInst.Text = institucion.telefono;
            txtNombreRepresent.Text = institucion.nombre_represent;
            txtApPatRepresent.Text = institucion.ap_pat_represent;
            txtCorreoRepresent.Text = institucion.correo_electronico_represent;
            txtTelefonoRepresent.Text = institucion.telefono_represent;

            txtContraseniaInst.Password = institucion.contrasenia;
            txtContraseniaInstVisible.Text = institucion.contrasenia;

            HabilitarControles(false);
            btnRegistrar.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Visible;
        }


        /*txtContrasenia.Password = institucion.contrasenia;
            txtContraseniaVisible.Text = institucion.contrasenia;*/
        private void HabilitarControles(bool habilitar)
        {
            // Habilitar/Deshabilitar todos los campos
            txtDocumento.IsEnabled = habilitar;
            txtNombre.IsEnabled = habilitar;
            txtApPat.IsEnabled = habilitar;
            txtApMat.IsEnabled = habilitar;
            dpFechaNac.IsEnabled = habilitar;
            cbNacionalidadTurista.IsEnabled = habilitar;
            txtTelefono.IsEnabled = habilitar;
            txtCorreo.IsEnabled = habilitar;
            txtContrasenia.IsEnabled = habilitar;

            txtNombreInst.IsEnabled = habilitar;
            cbNacionalidadInstitucion.IsEnabled = habilitar;
            txtTelefonoInst.IsEnabled = habilitar;
            txtCorreoInst.IsEnabled = habilitar;
            txtContraseniaInst.IsEnabled = habilitar;
            txtNombreRepresent.IsEnabled = habilitar;
            txtApPatRepresent.IsEnabled = habilitar;
            txtCorreoRepresent.IsEnabled = habilitar;
            txtTelefonoRepresent.IsEnabled = habilitar;

            // ComboBox tipo solo deshabilitado si se están viendo datos ya cargados
            cbTipo.IsEnabled = !ModoVisualizacion;

            //botones 
            btnRegistrar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Collapsed;
            btnEliminar.Visibility = Visibility.Collapsed;

            if (ModoVisualizacion && !habilitar)
            {
                // Mostrando datos, pero no en edición
                btnEditar.Visibility = Visibility.Visible;
                btnEliminar.Visibility = Visibility.Visible; 
            }
            else if (ModoVisualizacion && habilitar)
            {
                // Está en modo edición
                btnGuardar.Visibility = Visibility.Visible;
            }
            else if (!ModoVisualizacion && habilitar)
            {
                // Registro nuevo
                btnRegistrar.Visibility = Visibility.Visible;
            }


            // Cambiar estilo visual de campos
            foreach (var control in FindVisualChildren<TextBox>(this))
            {
                control.Background = habilitar ? Brushes.White : Brushes.Transparent;
                control.BorderThickness = habilitar ? new Thickness(1) : new Thickness(0);
            }
        }


        // Método auxiliar para encontrar controles
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    
        // boton de editicion
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            modoEdicion = true;
            HabilitarControles(true);
            btnEditar.Visibility = Visibility.Collapsed;
            btnGuardar.Visibility = Visibility.Visible;
            btnRegistrar.Visibility = Visibility.Collapsed;
            btnEliminar.Visibility = Visibility.Collapsed;


            // Cambiar título según el tipo
            Title = tipoClienteActual == "Turista"
                ? "Editando Turista"
                : "Editando Institución";
        }
        // boton de guardar
        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool actualizado = false;

                if (tipoClienteActual == "Turista")
                {
                    actualizado = await ActualizarTurista();
                }
                else
                {
                    actualizado = await ActualizarInstitucion();
                }

                if (!actualizado)
                {
                    return;
                }

                MessageBox.Show("Datos actualizados correctamente", "Éxito",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                modoEdicion = false;
                HabilitarControles(false);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<bool> ActualizarTurista()
        {
            string contrasenia = chkVerContrasenia.IsChecked == true
                ? txtContraseniaVisible.Text
                : txtContrasenia.Password;

            if (!ValidarContrasenia(contrasenia))
            {
                MessageBox.Show("La contraseña debe tener:\n- Mínimo 8 caracteres\n- 1 mayúscula\n- 1 minúscula\n- 1 carácter especial",
                              "Contraseña inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtDocumento.Text.Length < 7)
            {
                MessageBox.Show("El documento debe tener al menos 7 caracteres",
                              "Documento inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!ValidarEmail(txtCorreo.Text))
            {
                MessageBox.Show("Por favor ingrese un correo electrónico válido",
                              "Correo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!ValidarTelefono(txtTelefono.Text))
            {
                MessageBox.Show("Por favor ingrese un número de teléfono válido",
                              "Teléfono inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpFechaNac.SelectedDate == null)
            {
                MessageBox.Show("Por favor seleccione una fecha de nacimiento",
                              "Fecha requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var turistaData = new
            {
                correo_electronico = txtCorreo.Text,
                documento = txtDocumento.Text,
                nombre = txtNombre.Text,
                ap_pat = txtApPat.Text,
                ap_mat = txtApMat.Text,
                fecha_nac = dpFechaNac.SelectedDate?.ToString("yyyy-MM-dd"),
                nacionalidad = cbNacionalidadTurista.SelectedItem?.ToString(),
                telefono = txtTelefono.Text,
                contrasenia = contrasenia
            };

            var response = await httpClient.PutAsJsonAsync(
                $"http://localhost:8000/api/visitantes/turistas/{codigoClienteActual}",
                turistaData
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error del servidor: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private async Task<bool> ActualizarInstitucion()
        {
            // Validar contraseña
             string contrasenia = chkVerContrasenia.IsChecked == true
            ? txtContraseniaInstVisible.Text
            : txtContraseniaInst.Password;

            // Validar contraseña
            if (!ValidarContrasenia(txtContraseniaInst.Password))
            {
                MessageBox.Show("La contraseña debe tener:\n- Mínimo 8 caracteres\n- 1 mayúscula\n- 1 minúscula\n- 1 carácter especial",
                              "Contraseña inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            // Validar email
            if (!ValidarEmail(txtCorreoInst.Text) && !ValidarEmail(txtCorreoRepresent.Text))
            {
                MessageBox.Show("Por favor ingrese un correo electrónico válido",
                              "Correo inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validar teléfono
            if (!ValidarTelefono(txtTelefonoInst.Text) && !ValidarTelefono(txtTelefonoRepresent.Text))
            {
                MessageBox.Show("Por favor ingrese un número de teléfono válido",
                              "Teléfono inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            var institucionData = new
            {
                nombre = txtNombreInst.Text,
                correo_electronico = txtCorreoInst.Text,
                nacionalidad = cbNacionalidadInstitucion.SelectedItem?.ToString(),
                telefono = txtTelefonoInst.Text,
                nombre_represent = txtNombreRepresent.Text,
                ap_pat_represent = txtApPatRepresent.Text,
                correo_electronico_represent = txtCorreoRepresent.Text,
                telefono_represent = txtTelefonoRepresent.Text,
                contrasenia = txtContraseniaInst.Password
            };

            var response = await httpClient.PutAsJsonAsync(
                $"http://localhost:8000/api/visitantes/instituciones/{codigoClienteActual}",
                institucionData
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent);
            }
            return true;
        }
        // ----------------- borrado logico ----------------------
        private async Task<bool> DesactivarVisitante()
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"),
                    $"http://localhost:8000/api/visitante/desactivar{codigoClienteActual}");

                request.Content = null; 

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Visitante eliminado (desactivado) correctamente.", "Éxito",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al eliminar visitante: {error}", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excepción al eliminar: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro que desea eliminar este visitante?", "Confirmar eliminación",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                bool exito = await DesactivarVisitante();
                if (exito)
                {
                    this.DialogResult = true;
                    this.Close(); // Cierra el formulario para que se recargue la tabla principal
                }
            }
        }


    }
}