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
using System.Windows.Shapes;

namespace chaski_tours_desk.Componentes.User
{
    /// <summary>
    /// Lógica de interacción para ListadoSitios.xaml
    /// </summary>
    public partial class ListadoSitios : UserControl
    {
        HttpClient client = new HttpClient();
        private string URL_Sitios = "http://localhost:8000/api/sitios/";
        private string URL_Imagenes = "http://localhost:8000/api/imagenes/";
        private string URL_Ubicaciones = "http://localhost:8000/api/ubicaciones/";


        int indice = 0;
        public event EventHandler CerrarListadoSitios;

        List<Sitio> sitios = new List<Sitio>();
        public ListadoSitios()
        {
            InitializeComponent();
            navbar.siteIcon.Fill = Application.Current.Resources["BoliviaLightGreen"] as SolidColorBrush;
            navbar.siteIconText.Foreground = Application.Current.Resources["BoliviaLightGreen"] as SolidColorBrush;

            navbar.homeIcon.Fill = Brushes.White;
            navbar.homeIconText.Foreground = Brushes.White;

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cargarDatosaCarts();
        }
        private async void cargarDatosaCarts()
        {
            sitios = await client.GetFromJsonAsync<List<Sitio>>(URL_Sitios);

            List<Imagen> imagenes = new List<Imagen>();
            imagenes = await client.GetFromJsonAsync<List<Imagen>>(URL_Imagenes);

            foreach (var sitio in sitios)
            {
                var contenidoGrid = new Grid
                {
                    Margin = new Thickness(20),
                    Width = 1200,
                    Height = 300,
                    Background = (Brush)Application.Current.Resources["BoliviaDarkGreen"],
                    Opacity = 0.9,           };

                contenidoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                contenidoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Imagen del sitio
                var imagen = new Image
                {
                    Source = new BitmapImage(new Uri(imagenes[indice].url_img)),
                    Stretch = Stretch.UniformToFill,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(20),
                };
                indice++;
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
                    Text = sitio.id_sitio.ToString(),
                    Visibility = Visibility.Collapsed,
                    FontSize = 32,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)
                });


                textoStack.Children.Add(new TextBlock
                {
                    Text = sitio.nombre,
                    FontSize = 32,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = "DESCRIPCION HISTORICA",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = sitio.desc_historica_sitio,
                    FontSize = 20,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = "DESCRIPCION CONCEPTUAL",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = sitio.desc_conceptual_sitio,
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
                    Text = sitio.costo_sitio.ToString("C"),
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
                    configurarCarasSitio(sitio);
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
            indice = 0;
        }

        private async void cargarDatosaCarts(string filtro)
        {
            stackMain.Children.Clear();
            var lstUbicaciones = (await client.GetFromJsonAsync<List<Ubicacion>>(URL_Ubicaciones)).Where(ubicacion => ubicacion.departamento == filtro).ToList();
            List<int> ubicaciones = new List<int>();
            lstUbicaciones.ForEach(ubicacion => ubicaciones.Add(ubicacion.id_ubicacion));


            sitios = await client.GetFromJsonAsync<List<Sitio>>(URL_Sitios);

            sitios = sitios.Where(sitio => ubicaciones.Contains(sitio.id_ubicacion)).ToList();

            List<Imagen> imagenes = new List<Imagen>();
            imagenes = await client.GetFromJsonAsync<List<Imagen>>(URL_Imagenes);

            foreach (var sitio in sitios)
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

                // Imagen del sitio
                var imagen = new Image
                {
                    Source = new BitmapImage(new Uri(imagenes[indice].url_img)),
                    Stretch = Stretch.UniformToFill,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(20),
                };
                indice++;
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
                    Text = sitio.id_sitio.ToString(),
                    Visibility = Visibility.Collapsed,
                    FontSize = 32,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)
                });


                textoStack.Children.Add(new TextBlock
                {
                    Text = sitio.nombre,
                    FontSize = 32,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = "DESCRIPCION HISTORICA",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = sitio.desc_historica_sitio,
                    FontSize = 20,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = "DESCRIPCION CONCEPTUAL",
                    FontSize = 24,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                });

                textoStack.Children.Add(new TextBlock
                {
                    Text = sitio.desc_conceptual_sitio,
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
                    Text = sitio.costo_sitio.ToString("C"),
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
                    configurarCarasSitio(sitio);
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
            if (indice >= 9) {
                indice = 0;
            };
        }

        private void configurarCarasSitio(Sitio sitio)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.usuario.listadoSitios.Visibility = Visibility.Collapsed;
            mainWindow.usuario.datosSitio.Visibility = Visibility.Visible;
            mainWindow.usuario.datosSitio.hiddenId.Text = sitio.id_sitio.ToString();
            mainWindow.usuario.datosSitio.cargarTodosDatos();
            mainWindow.usuario.btnReservar.Visibility = Visibility.Collapsed;
        }
        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            CerrarListadoSitios?.Invoke(this, EventArgs.Empty);
        }

        public void FiltrarSitios(string filtro) {
            cargarDatosaCarts(filtro);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.usuario.listadoSitios.Visibility = Visibility.Visible;
            mainWindow.usuario.vistaCategorias.Visibility = Visibility.Collapsed;
            mainWindow.usuario.vistaDepartamentos.Visibility = Visibility.Collapsed;
        }
    }
}
