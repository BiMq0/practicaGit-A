using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaski_tours_desk.Modelos
{
    internal class CalendarioSalida
    {
        public int id_salida { get; set; }
        public int id_tour { get; set; }
        public string fecha_salida { get; set; }
        public string fecha_regreso { get; set; }
    }
}
