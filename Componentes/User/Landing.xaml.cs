using chaski_tours_desk.Componentes.User.ListaDE;
using chaski_tours_desk.Modelos.ModelosNoDB;
using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
using System.Net.Http.Json;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para Landing.xaml
    /// </summary>
    public partial class Landing : UserControl
    {
        HttpClient client = new HttpClient();

        LDE lstDECategorias = new LDE();

        LDE lstDEDepartamentos = new LDE();
        private string URL_Sitios = "http://localhost:8000/api/sitios";
        private string URL_Tours = "http://localhost:8000/api/tour";
        List<Tour> tours = new List<Tour>();
        List<Sitio> sitios = new List<Sitio>();

        public event EventHandler MostrarListadoSitios;
        public event EventHandler MostrarListadoTours;

        public event EventHandler CerrarListadoTours;
        public Landing()
        {
            InitializeComponent();
        }

        private void cargarDatosaCarts()
        {
            tbx_nombresitio1.Text = sitios[1].nombre;
            tbx_descripcionsitio1.Text = sitios[1].desc_conceptual_sitio;
            tbx_nombresitio2.Text = sitios[2].nombre;
            tbx_descripcionsitio2.Text = sitios[2].desc_conceptual_sitio;
            tbx_nombretour1.Text = tours[1].nombre_tour;
            tbx_descripciontour1.Text = tours[1].descripcion_tour;
            tbx_nombretour2.Text = tours[2].nombre_tour;
            tbx_descripciontour2.Text = tours[2].descripcion_tour;
        }
        private async void cargarListas()
        {
            await noborres();


        }

        private async Task noborres()
        {
            tours = await client.GetFromJsonAsync<List<Tour>>(URL_Tours);
            sitios = await client.GetFromJsonAsync<List<Sitio>>(URL_Sitios);
            cargarDatosaCarts();
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cargarListas();
            cargarListasDE();
        }


        //estos nenes se dedican a entregar acciones a las imagenes
        private void Grid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarListadoSitios?.Invoke(this, EventArgs.Empty);
        }
        private void Grid2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarListadoSitios?.Invoke(this, EventArgs.Empty);
        }
        private void Grid3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarListadoTours?.Invoke(this, EventArgs.Empty);
        }
        private void Grid4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarListadoTours?.Invoke(this, EventArgs.Empty);
        }


        private void cargarListasDE()
        {
            cargarListaDECategorias();
            cargarListaDEDepartamentos();
        }

        private void cargarListaDEDepartamentos()
        {
            try
            {
                List<Departamento> departamentos = new Departamento().ObtenerTodasLasDepartamentos();
                lstDEDepartamentos.crearListaDE(departamentos.Cast<object>().ToList());

                if (lstDEDepartamentos.Actual?.Valor is Departamento departamentoActual)
                {
                    ActualizarCuadroDepartamentos(departamentoActual);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cargarListaDECategorias()
        {
            try
            {
                List<Categoria> categorias = new Categoria().ObtenerTodasLasCategorias();
                lstDECategorias.crearListaDE(categorias.Cast<object>().ToList());

                if (lstDECategorias.Actual?.Valor is Categoria categoriaActual)
                {
                    ActualizarCuadroCategorias(categoriaActual);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ActualizarCuadroDepartamentos(Departamento departamentoActual)
        {
            imgDepartamentos.Source = departamentoActual.Imagen;
            nomDepartamento.Text = departamentoActual.Nombre;
        }

        private void ActualizarCuadroCategorias(Categoria categoriaActual)
        {
            imgCategorias.Source = categoriaActual.Imagen;
            nomCategorias.Text = categoriaActual.Nombre;
        }

        private void btnDerechaDep_Click(object sender, RoutedEventArgs e)
        {
            lstDEDepartamentos.pasarSiguiente();
            ActualizarCuadroDepartamentos(lstDEDepartamentos.Actual.Valor as Departamento);
        }

        private void btnIzquierdaDep_Click(object sender, RoutedEventArgs e)
        {
            lstDEDepartamentos.pasarAnterior();
            ActualizarCuadroDepartamentos(lstDEDepartamentos.Actual.Valor as Departamento);
        }

        private void btnIzquierdaCat_Click(object sender, RoutedEventArgs e)
        {
            lstDECategorias.pasarSiguiente();
            ActualizarCuadroCategorias(lstDECategorias.Actual.Valor as Categoria);
        }

        private void btnDerechaCat_Click(object sender, RoutedEventArgs e)
        {
            lstDECategorias.pasarAnterior();
            ActualizarCuadroCategorias(lstDECategorias.Actual.Valor as Categoria);
        }


        private void btnVerCategoria_Click(object sender, RoutedEventArgs e)
        {
            AbrirCategorias.Invoke();
        }

        private void btnVerDepartamento_Click(object sender, RoutedEventArgs e)
        {
            AbrirDepartamentos.Invoke();
        }

        public event Action AbrirCategorias;

        public event Action AbrirDepartamentos;

        private void btnMiPerfil_Click(object sender, RoutedEventArgs e)
        {
            string cod = MainWindow.codVisitanteActual;

            if (string.IsNullOrEmpty(cod))
            {
                MessageBox.Show("No hay visitante logueado.");
                return;
            }

            if (cod.StartsWith("TUR"))
            {
                Cuenta perfilTurista = new Cuenta(cod);
                perfilTurista.Show();
            }
            else if (cod.StartsWith("INS"))
            {
                CuentaIns perfilInstitucion = new CuentaIns(cod);
                perfilInstitucion.Show();
            }
            else
            {
                MessageBox.Show("Tipo de visitante desconocido.");
            }
        }
    }
}
