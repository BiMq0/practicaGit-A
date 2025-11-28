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
    /// Lógica de interacción para ReservasAdmin.xaml
    /// </summary>
    public partial class ReservasAdmin : Window
    {
        private HttpClient cliente = new HttpClient();
        private string URLRes = "http://localhost:8000/api/reservas";
        private string URLTuristas = "http://localhost:8000/api/visitantes/turistas/";
        private string URLInsttituciones = "http://localhost:8000/api/visitantes/instituciones/";
        private string URLCalendarios = "http://localhost:8000/api/calendario";
        public Reserva res;
        List<Turista> tur;
        List<Institucion> instituciones;
        List<CalendarioSalida> calendario;
        List<string> estados = new List<string> { "Pendiente", "Confirmada", "Cancelada", "Completada" };
        public ReservasAdmin(Reserva x)
        {
            InitializeComponent();
            res = x;
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

        //obtener todos los datos al cargar la ventana
        private async void datos(object sender, RoutedEventArgs e)
        {
            await cargardatos();
        }
        private async Task cargardatos()
        {
            await obtenerClientes();
            await obtenerCalendarios();
            configurardatos();
        }
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

        //configurar datos de la reserva
        private void configurardatos()
        {


            //obtener el nombre del visitante ya sea turista o institucion por el codigo

            if (res.cod_visitante.Contains("TUR"))
            {

                foreach (var item in tur)
                {
                    if (item.cod_visitante == res.cod_visitante)
                    {
                        cmb_codvisitante.Text = item.nombre;
                    }
                }

            }
            if (res.cod_visitante.Contains("INS"))
            {

                foreach (var item in instituciones)
                {
                    if (item.cod_visitante == res.cod_visitante)
                    {
                        cmb_codvisitante.Text = item.nombre;
                    }
                }
            }


            //obtener la fecha de salida por el id

            foreach (var item in calendario)
            {
                if (item.id_salida == res.id_salida)
                {
                    cmb_idsalida.Text = item.fecha_salida;
                }
            }

            //cargar datos propios de la reserva

            txt_cantidad.Text = res.cantidad_personas.ToString();
            txt_costototal.Text = res.costo_total_reserva.ToString("F2");
            cmb_estados.Text = res.estado.ToString();
        }




        //condigurar la funcionalidad de los botones

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            BorrarReserva();
        }


        private async void BorrarReserva()
        {
            await EliminarReserva();
        }


        public async Task EliminarReserva()
        {
            HttpResponseMessage response = await cliente.DeleteAsync($"{URLRes}/{res.id_reserva}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Reserva borrada correctamente");
                Close();
            }
            else
            {
                MessageBox.Show("Error al borrar la reserva");
            }
        }



        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                mandarReserva();
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
        private async void mandarReserva()
        {
            await ActualizarReserva();
        }
        public async Task ActualizarReserva()
        {

            //mandar el codigo del visitante por el nombre

            int flag1 = 0;
            foreach (var item in tur)
            {
                if (item.nombre == cmb_codvisitante.Text)
                {
                    res.cod_visitante = item.cod_visitante;
                    flag1 = 1;
                }
            }
            if (flag1 == 0)
            {
                foreach (var item in instituciones)
                {
                    if (item.nombre == cmb_codvisitante.Text)
                    {
                        res.cod_visitante = item.cod_visitante;
                        flag1 = 1;
                    }
                }
            }


            //mandar el id de la salida por la fecha

            foreach (var item in calendario)
            {
                if (item.fecha_salida == cmb_idsalida.Text)
                {
                    res.id_salida = item.id_salida;
                }
            }

            res.cantidad_personas = int.Parse(txt_cantidad.Text);
            res.costo_total_reserva = double.Parse(txt_costototal.Text);
            res.estado = cmb_estados.Text;

            string json = JsonSerializer.Serialize(res);
            MessageBox.Show(json);

            HttpResponseMessage response = await cliente.PutAsJsonAsync($"{URLRes}/{res.id_reserva}", res);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Reserva actualizada correctamente");
                Close();
            }
            else
            {
                MessageBox.Show("Error al actualizar la reserva");
            }

        }

    }
}
