using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace chaski_tours_desk.Modelos.ModelosNoDB
{
    public class Categoria
    {
        public string Nombre { get; set; }
        public BitmapImage Imagen { get; set; }

        public Categoria(string nombre, string imagePath = null)
        {
            Nombre = nombre;
            Imagen = CargarImagen(imagePath) ;
        }
        public Categoria()
        {
            
        }

        private BitmapImage CargarImagen(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Images/Images_Categorias/" + path);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public List<Categoria> ObtenerTodasLasCategorias()
        {

            return new List<Categoria>
            {
                new Categoria("Histórico", "historico.jpg"),
                new Categoria("Arqueológico", "arqueologico.jpg"),
                new Categoria("Colonial", "colonial.jpg"),
                new Categoria("Religioso", "religioso.jpg"),
                new Categoria("Cultural", "cultural.jpg"),
                new Categoria("Patrimonial", "patrimonial.jpg"),
                new Categoria("Museo", "museo.jpg"),
                new Categoria("Batalla", "batalla.png"),
                new Categoria("Hito", "hito.jpeg"),
                new Categoria("Arquitectónico", "arquitectonico.jpg"),
                new Categoria("Industrial", "industrial.png"),
                new Categoria("Natural Histórico", "historico_natural.jpg"),
            };
        }
    }
} 
