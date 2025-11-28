using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

namespace chaski_tours_desk.Modelos
{
    internal class Alojamiento
    {
        public int id_alojamiento { get; set; }
        public string nombre_aloj { get; set; }
        public double nro_estrellas { get; set; } 
        public int nro_habitaciones { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int Activo { get; set; }
    }
}
