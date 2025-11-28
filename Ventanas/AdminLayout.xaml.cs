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
using chaski_tours_desk.Componentes.Admin;

namespace chaski_tours_desk.Ventanas
{
    /// <summary>
    /// Lógica de interacción para AdminLayout.xaml
    /// </summary>
    public partial class AdminLayout : UserControl
    {
        public AdminLayout()
        {
            InitializeComponent();
        }


        public void cambiarVentanas(int index)
        {
            switch (index)
            {
                case 0:
                    Inicio.Visibility = Visibility.Visible;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Visible;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Visible;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Visible;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Visible;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 5:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Visible;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 6:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Visible;
                    Salidas.Visibility = Visibility.Collapsed;
                    break;
                case 7:
                    Inicio.Visibility = Visibility.Collapsed;
                    Reservas.Visibility = Visibility.Collapsed;
                    Clientes.Visibility = Visibility.Collapsed;
                    Tours.Visibility = Visibility.Collapsed;
                    Sitios.Visibility = Visibility.Collapsed;
                    Transportes.Visibility = Visibility.Collapsed;
                    Alojamientos.Visibility = Visibility.Collapsed;
                    Salidas.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dashboard.cambiarVentana += cambiarVentanas;
        }
    }
}
