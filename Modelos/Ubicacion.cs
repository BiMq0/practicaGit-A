using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaski_tours_desk.Modelos
{
    internal class Ubicacion
    {
        public int id_ubicacion { get; set; }
        public string departamento { get; set; }
        public string municipio { get; set; }
        public string zona { get; set; }
        public string calle { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
