using chaski_tours_desk.Modelos.ModelosNoDB;
using chaski_tours_desk.Ventanas;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para VistaDepartamentos.xaml
    /// </summary>
    public partial class VistaDepartamentos : UserControl
    {
        public VistaDepartamentos()
        {
            InitializeComponent();

            navbar.cityIcon.Fill = Application.Current.Resources["BoliviaLightGreen"] as SolidColorBrush;
            navbar.cityIconText.Foreground = Application.Current.Resources["BoliviaLightGreen"] as SolidColorBrush;

            navbar.homeIcon.Fill = Brushes.White;
            navbar.homeIconText.Foreground = Brushes.White;
        }

        public void MostrarDepartamentos()
        {
            List<Departamento> departamentos = new Departamento().ObtenerTodasLasDepartamentos();

            foreach (Departamento departamento in departamentos)
            {
                var nuevoBorde = new Border {
                    CornerRadius = new CornerRadius(20),
                    Margin = new Thickness(0, 60, 0, 40),
                    Height = 400,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Transparent,
                };
                var nuevoGrid = new Grid
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                          new Image
                          {
                              Source = departamento.Imagen,
                              Stretch = Stretch.UniformToFill,
                              VerticalAlignment = VerticalAlignment.Center,
                          },
                          new Rectangle{
                              Stretch = Stretch.Fill,
                              Fill = Brushes.Black,
                              Opacity = 0.4
                          },
                          new TextBlock
                          {
                              Text = departamento.Nombre,
                              VerticalAlignment = VerticalAlignment.Center,
                              HorizontalAlignment = HorizontalAlignment.Center,
                              Foreground = Brushes.White,
                              FontSize = 40,
                              Style = (Style)Application.Current.Resources["TituloTXB"]
                          }
                    },
                    Cursor = Cursors.Hand
                };

                nuevoGrid.MouseLeftButtonDown += (s, e) =>
                {
                    VerSitios.Invoke(departamento.Nombre);
                };

                nuevoBorde.Child = nuevoGrid;
                stackMain.Children.Add(nuevoBorde);
            }
        }

        public event Action<string> VerSitios;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MostrarDepartamentos();
        }

        public event Action volverLanding;

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            volverLanding.Invoke();
        }
    }
}
