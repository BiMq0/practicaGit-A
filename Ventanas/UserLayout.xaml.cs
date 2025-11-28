using chaski_tours_desk.Modelos.ModelosNoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chaski_tours_desk.Ventanas
{
    /// <summary>
    /// Lógica de interacción para UserLayout.xaml
    /// </summary>
    public partial class UserLayout : UserControl
    {
        public UserLayout()
        {
            InitializeComponent(); landing.MostrarListadoSitios += MostrarListadoSitios;
            landing.MostrarListadoTours += MostrarListadoTours;
            listadoSitios.CerrarListadoSitios += CerrarListadoSitios;
            listadoTours.CerrarListadoTours += CerrarListadoTours;


        }

        private void btnReservar_Click(object sender, RoutedEventArgs e)
        {
            abrirAdmin.Invoke();
        }

        public event Action abrirAdmin;

        private void VerCategorias() { 
            landing.Visibility = Visibility.Collapsed;
            vistaDepartamentos.Visibility = Visibility.Collapsed;
            vistaCategorias.Visibility = Visibility.Visible;
        }

        private void VerDepartamentos()
        {
            landing.Visibility = Visibility.Collapsed;
            vistaDepartamentos.Visibility = Visibility.Visible;
            vistaCategorias.Visibility = Visibility.Collapsed;
        }

        private void VerLanding()
        {
            landing.Visibility = Visibility.Visible;
            vistaCategorias.Visibility = Visibility.Collapsed;
            vistaDepartamentos.Visibility = Visibility.Collapsed;
        }

        //aun no implementados

        private void VerSitios(string filtro)
        {
            listadoSitios.FiltrarSitios(filtro);
        }


        private void MostrarListadoSitios(object sender, EventArgs e)
        {
            listadoSitios.Visibility = Visibility.Visible;
            landing.Visibility = Visibility.Collapsed;
        }
        private void MostrarListadoTours(object sender, EventArgs e)
        {
            listadoTours.Visibility = Visibility.Visible;
            landing.Visibility = Visibility.Collapsed;
        }
        private void CerrarListadoSitios(object sender, EventArgs e)
        {
            listadoSitios.Visibility = Visibility.Collapsed;
            landing.Visibility = Visibility.Visible;
        }
        private void CerrarListadoTours(object sender, EventArgs e)
        {
            listadoTours.Visibility = Visibility.Collapsed;
            landing.Visibility = Visibility.Visible;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            landing.AbrirDepartamentos += VerDepartamentos;
            landing.AbrirCategorias += VerCategorias;
            vistaCategorias.volverLanding += VerLanding;
            vistaDepartamentos.volverLanding += VerLanding;

            vistaCategorias.VerSitios += VerSitios;
            vistaDepartamentos.VerSitios += VerSitios;
        }

    }
}
