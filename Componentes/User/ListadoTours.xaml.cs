using chaski_tours_desk.Componentes.Admin;
using chaski_tours_desk.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para ListadoTours.xaml
    /// </summary>
    public partial class ListadoTours : UserControl
    {
        HttpClient client = new HttpClient();
        private string URLTours = "http://localhost:8000/api/tour/";
        public event EventHandler CerrarListadoTours;
        List<string> imgTour = new List<string>()

        {
            "pack://application:,,,/Images/Images_Departamentos/Beni.jpg",
            "pack://application:,,,/Images/Images_Departamentos/SantaCruz.jpg",
            "pack://application:,,,/Images/conejo.jpg",
            "pack://application:,,,/Images/Images_Departamentos/LaPaz.jpg",
            "pack://application:,,,/Images/Images_Departamentos/Tarija.jpg"
        };
        Random random = new Random();
        public ListadoTours()
        {
            InitializeComponent();
            navbar.toursIcon.Fill = Application.Current.Resources["BoliviaLightGreen"] as SolidColorBrush;
            navbar.toursIconText.Foreground = Application.Current.Resources["BoliviaLightGreen"] as SolidColorBrush;

            navbar.homeIcon.Fill = Brushes.White;
            navbar.homeIconText.Foreground = Brushes.White;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatosaCarts();
        }
        private async void cargarDatosaCarts()
        {

            List<Tour> tours = new List<Tour>();

            tours = await client.GetFromJsonAsync<List<Tour>>(URLTours);

            foreach (var tour in tours)
            {
                var contenidoGrid = new Grid
                {
                    Margin = new Thickness(20),
                    Width = 1200,
                    Height = 300,
                    Background = (Brush)Application.Current.Resources["BoliviaDarkGreen"],
                    Opacity = 0.9,
                };

                contenidoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                contenidoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Imagen del tour
                var imagen = new Image
                {
                    Source = new BitmapImage(new Uri(imgTour[random.Next(0, 4)])),
                    Stretch = Stretch.UniformToFill,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(20),
                };
                Grid.SetColumn(imagen, 1);

                // Columna de texto
                var textoStack = new StackPanel
                {
                    Margin = new Thickness(30),
                    VerticalAlignment = VerticalAlignment.Top
                };
                Grid.SetColumn(textoStack, 0);

                textoStack.Children.Add(new TextBlock
                {
                    Text = tour.nombre_tour,
                    FontSize = 32,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = "DESCRIPCION",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = tour.descripcion_tour,
                    FontSize = 20,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                

                // Stack horizontal para el precio
                var stackPrecio = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(30),
                    HorizontalAlignment = HorizontalAlignment.Left

                };
                Grid.SetColumnSpan(stackPrecio, 2);

                stackPrecio.Children.Add(new TextBlock
                {
                    Text = "Precio del Sitio: ",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White
                });

                stackPrecio.Children.Add(new TextBlock
                {
                    Text = tour.costo_tour.ToString("C"),
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White
                });

                contenidoGrid.Children.Add(imagen);
                contenidoGrid.Children.Add(textoStack);
                contenidoGrid.Children.Add(stackPrecio);

                contenidoGrid.Cursor = Cursors.Hand;



                //evento del card
                contenidoGrid.MouseLeftButtonDown += (s, e) =>
                {



                    configurarCarasTour(tour);


                };

                var borde = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(20),
                    Height = 400,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Transparent,
                    Child = contenidoGrid,
                    Padding = new Thickness(20),
                };

                stackMain.Children.Add(borde);
            }



        }

        private void configurarCarasTour(Tour tour)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.usuario.listadoTours.Visibility = Visibility.Collapsed;
            mainWindow.usuario.datosTour.hiddenId.Text = tour.id_tour.ToString();
            mainWindow.usuario.datosTour.cargarDatos();
            mainWindow.usuario.btnReservar.Visibility = Visibility.Collapsed;
            mainWindow.usuario.datosTour.Visibility = Visibility.Visible;
        }
        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            CerrarListadoTours?.Invoke(this, EventArgs.Empty);
        }
    }
}
