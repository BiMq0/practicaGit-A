using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para ReservasNuevo.xaml
    /// </summary>
    public partial class ReservasNuevo : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URLRes = "http://localhost:8000/api/reservas/crear";
        private string URLTuristas = "http://localhost:8000/api/visitantes/turistas/";
        private string URLInsttituciones = "http://localhost:8000/api/visitantes/instituciones/";
        private string URLCalendarios = "http://localhost:8000/api/calendario";
        public Reserva res;
        List<Turista> tur;
        List<Institucion> instituciones;
        List<CalendarioSalida> calendario;
        List<string> estados = new List<string> { "Pendiente", "Confirmada", "Cancelada", "Completada" };
        public ReservasNuevo()
        {
            InitializeComponent();
            habilitar(true);
        }
        private void habilitar(bool valor)
        {
            cmb_codvisitante.IsEnabled = valor;
            cmb_idsalida.IsEnabled = valor;
            txt_cantidad.IsEnabled = valor;
            txt_costototal.IsEnabled = valor;
            cmb_estados.IsEnabled = valor;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //obtener todos los datos al cargar la ventana
        private async void datos(object sender, RoutedEventArgs e)
        {
            await cargardatos();
        }
        private async Task cargardatos()
        {
            await obtenerClientes();
            await obtenerCalendarios();
        }
        //optener la lista de los alojamientos


        //obtener lista de los clientes
        private async Task obtenerClientes()
        {
            tur = await cliente.GetFromJsonAsync<List<Turista>>(URLTuristas);
            instituciones = await cliente.GetFromJsonAsync<List<Institucion>>(URLInsttituciones);
            foreach (var item in tur)
            {
                cmb_codvisitante.Items.Add(item.nombre);
            }
            foreach (var item in instituciones)
            {
                cmb_codvisitante.Items.Add(item.nombre);
            }
        }
        //obtener la lista de los calendarios

        private async Task obtenerCalendarios()
        {
            calendario = await cliente.GetFromJsonAsync<List<CalendarioSalida>>(URLCalendarios);

            foreach (var item in calendario)
            {
                cmb_idsalida.Items.Add(item.fecha_salida);
            }
            foreach (var item in estados)
            {
                cmb_estados.Items.Add(item);
            }
        }

        private void Enviar_Click_1(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                mandarReserva();
            }
        }

        private async void mandarReserva()
        {
            await crearReserva();
        }
        public async Task crearReserva()
        {

            //mandar el codigo del visitante por el nombre
            string codigo = "";
            int flag1 = 0;
            foreach (var item in tur)
            {
                if (item.nombre == cmb_codvisitante.Text)
                {
                    codigo = item.cod_visitante;
                    flag1 = 1;
                }
            }
            if (flag1 == 0)
            {
                foreach (var item in instituciones)
                {
                    if (item.nombre == cmb_codvisitante.Text)
                    {
                        codigo = item.cod_visitante;
                        flag1 = 1;
                    }
                }
            }


            //mandar el id de la salida por la fecha
            int fecha = 0;
            foreach (var item in calendario)
            {
                if (item.fecha_salida == cmb_idsalida.Text)
                {
                    fecha = item.id_salida;
                }
            }


            var res = new Reserva
            {
                id_reserva = 0,
                cod_visitante = codigo,
                id_salida = fecha,
                cantidad_personas = int.Parse(txt_cantidad.Text),
                costo_total_reserva = double.Parse(txt_costototal.Text),
                estado = cmb_estados.Text
            };
            string json = JsonSerializer.Serialize(res);
            MessageBox.Show(json);
            HttpResponseMessage response = await cliente.PostAsJsonAsync(URLRes, res);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Reserva creada correctamente");
                Close();
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show("Error al crear la reserva");
            }

        }
        private bool validar()
        {
            if (cmb_codvisitante.Text == "" ||
                cmb_idsalida.Text == "" ||
                txt_cantidad.Text == "" ||
                txt_costototal.Text == "" ||
                cmb_estados.Text == "")
            {
                MessageBox.Show("Por favor, complete todos los campos");
                return false;
            }
            if (!int.TryParse(txt_cantidad.Text, out int cantidad) ||
                 !double.TryParse(txt_costototal.Text, out double costo))
            {
                MessageBox.Show("los campos CANTIDAD y COSTO deben ser números válidos.");
                return false;
            }
            if (cantidad > 250 || cantidad < 1)
            {
                MessageBox.Show("debe ingresar una cantidad valida entre 1 y 250");
                return false;
            }
            if (costo > 1000000 || costo < 0)
            {
                MessageBox.Show("debe ingresar un costo  valido entre 1000000 y 0");
                return false;
            }


            return true;
        }


    }
}