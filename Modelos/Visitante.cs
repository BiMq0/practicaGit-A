using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chaski_tours_desk.Modelos
{
    public class Visitante
    {
        public int id { get; set; }
        public string cod_visitante { get; set; }

        public string tipo_visitante { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int Activo { get; set; }
        [JsonPropertyName("nombre_completo")]
        public string NombreCompleto { get; set; }
    }
}
