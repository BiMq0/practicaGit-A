using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaski_tours_desk.Modelos
{
    public class Sitio
    {
        public int id_sitio { get; set; }
        public string nombre { get; set; }
        public string desc_conceptual_sitio { get; set; }
        public string desc_historica_sitio { get; set; }
        public double costo_sitio { get; set; }
        public int id_ubicacion { get; set; }
        public string temporada_recomendada { get; set; }
        public string recomendacion_climatica { get; set; }
        public string horario_apertura { get; set; }
        public string horario_cierre { get; set; }
        public int Activo { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }


        // Extras para tabla
        public string Estado => Activo == 1 ? "Activo" : "Inactivo";
    }
}
