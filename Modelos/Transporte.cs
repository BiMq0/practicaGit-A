using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chaski_tours_desk.Modelos
{
    public class Transporte
    {
        public int id_vehiculo { get; set; }
        public string matricula { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public int capacidad { get; set; }
        public string año { get; set; }
        public string anio => año;
        public int disponible { get; set; }

        //no quiten esto crjo
        [JsonPropertyName("Activo")]
        public int activo { get; set; }



    }
}
