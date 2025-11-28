using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaski_tours_desk.Modelos
{
    public class Tour
    {
        public int id_tour { get; set; }
        public string nombre_tour { get; set; }
        public string descripcion_tour { get; set; }
        public double costo_tour { get; set; }
        public int id_sitio_inicio { get; set; }
        public int id_sitio_fin { get; set; }
        public int duracion_dias { get; set; }
        public int duracion_noches { get; set; }
        public int id_alojamiento { get; set; }
        public int Activo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
