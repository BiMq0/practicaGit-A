using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace chaski_tours_desk.Modelos.ModelosNoDB
{
    public class Departamento
    {
        public string Nombre { get; set; }
        public BitmapImage Imagen { get; set; }


        public Departamento(string nombre, string imagePath)
        {
            Nombre = nombre;
            Imagen = CargarImagen(imagePath);
        }

        public Departamento(){}

        private BitmapImage CargarImagen(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Images/Images_Departamentos/" + path);
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

        public List<Departamento> ObtenerTodasLasDepartamentos()
        {

            return new List<Departamento>
            {
                new Departamento("La Paz", "LaPaz.jpg"),
                new Departamento("Oruro", "Oruro.jpg"),
                new Departamento("Cochabamba", "Cochabamba.png"),
                new Departamento("Beni", "Beni.jpg"),
                new Departamento("Santa Cruz", "SantaCruz.jpg"),
                new Departamento("Potosi", "Potosi.jpg"),
                new Departamento("Tarija", "Tarija.jpg"),
                new Departamento("Chuquisaca", "Chuquisaca.jpg"),
                new Departamento("Pando", "Pando.jpg"),
            };
        }
    }
}
