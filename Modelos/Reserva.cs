using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

namespace chaski_tours_desk.Modelos
{
    public class Reserva
    {
        public int id_reserva { get; set; }
        public string cod_visitante { get; set; }
        public int id_salida { get; set; }
        public int cantidad_personas { get; set; }
        public double costo_total_reserva { get; set; }
        public string estado { get; set; }

    }
}
