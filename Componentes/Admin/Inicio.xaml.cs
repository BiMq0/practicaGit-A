using chaski_tours_desk.Modelos;
using chaski_tours_desk.Ventanas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
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


namespace chaski_tours_desk.Componentes.Admin
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio : UserControl
    {
        private HttpClient cliente = new HttpClient();
        private string URL1 = "http://localhost:8000/api/alojamientos";
        private string URL2 = "http://localhost:8000/api/visitantes";
        private string URL3 = "http://localhost:8000/api/reservas";
        private string URL4 = "http://localhost:8000/api/sitios";
        private string URL5 = "http://localhost:8000/api/tour";
        private string URL6 = "http://localhost:8000/api/transporte";
        private string URL7 = "http://localhost:8000/api/visitantes/turistas/";

        public Inicio()
        {
            InitializeComponent();
        }
        //obtener lista de los turistas
        private async void obtenernombre()
        {

            List<Turista> usuarios = await cliente.GetFromJsonAsync<List<Turista>>(URL7 + titulo.Content);
            titulo.Content = "Bievenid@ " + usuarios[0].nombre;


        }

        //obtener lista de los clientes
        private async Task obtenerClientes()
        {
            int contturistas = 0;
            int continstituciones = 0;
            var usuarios = await cliente.GetFromJsonAsync<List<Visitante>>(URL2);
            lbl_clientes.Content = "Total de Clientes Registrados: " + usuarios.Count;

            foreach (var item in usuarios)
            {
                if (item.cod_visitante.Contains("TUR"))
                {
                    contturistas++;
                }
                if (item.cod_visitante.Contains("INS"))
                {
                    continstituciones++;
                }
            }

            lbl_cantinstituciones.Content = "Total de Instituciones Registradas: " + continstituciones;
            lbl_cantturistas.Content = "Total de Turistas Registrados: " + contturistas;

        }

        private async void verClientes()
        {
            await obtenerClientes();
        }

        private void verDatosClientes()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verClientes();
            }
        }

        //obtener lista de los alojamientos

        private async Task obtenerAlojamiento()
        {
            var alojas = await cliente.GetFromJsonAsync<List<Alojamiento>>(URL1);

            lbl_alojamientos.Content = "Total de Alojamientos Registrados: " + alojas.Count;
        }

        private async void verAlojamientos()
        {
            await obtenerAlojamiento();
        }

        private void verDatosAlojamientos()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verAlojamientos();
            }
        }

        //obtener lista de las reservas
        private async Task obtenerReserva()
        {
            var reserva = await cliente.GetFromJsonAsync<List<Reserva>>(URL3);

            lbl_reservas.Content = "Total de Reservas Registrados: " + reserva.Count;
        }

        private async void verReserva()
        {
            await obtenerReserva();
        }

        private void verDatosReserva()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verReserva();
            }
        }

        //obtener lista de los sitios
        private async Task obtenerSitios()
        {
            var sitios = await cliente.GetFromJsonAsync<List<Sitio>>(URL4);
            lbl_sitios.Content = "Total de Sitios Registrados: " + sitios.Count;
        }

        private async void verSitios()
        {
            await obtenerSitios();
        }

        private void verDatosSitios()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verSitios();
            }
        }

        //obtener lista de los tours
        private async Task obtenerTours()
        {
            var tours = await cliente.GetFromJsonAsync<List<Tour>>(URL5);
            lbl_tours.Content = "Total de Tours Registrados: " + tours.Count;
        }

        private async void verTours()
        {
            await obtenerTours();
        }

        private void verDatosTours()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verTours();
            }
        }
        //obtener lista de los transportes
        private async Task obtenerTransportes()
        {
            var transportes = await cliente.GetFromJsonAsync<List<Transporte>>(URL6);

            lbl_transporte.Content = "Total de Transportes Registrados: " + transportes.Count;
        }

        private async void verTransportes()
        {
            await obtenerTransportes();
        }

        private void verDatosTransporte()
        {
            if (Window.GetWindow(this).Visibility == Visibility.Visible)
            {
                verTransportes();
            }
        }


        //mostrar todos los datos al cargar la ventana
        private void DatosLoaded(object sender, RoutedEventArgs e)
        {

            obtenernombre();
            verDatosAlojamientos();
            verDatosClientes();
            verDatosReserva();
            verDatosSitios();
            verDatosTours();
            verDatosTransporte();
        }


    }
}