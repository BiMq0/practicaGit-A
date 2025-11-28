using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

namespace chaski_tours_desk.Modelos
{
        public class Turista
        {
       
        public string cod_visitante { get; set; }
        public string correo_electronico { get; set; }
        public string contrasenia { get; set; }
        public string documento { get; set; }
        public string nombre { get; set; }
        public string ap_pat { get; set; }
        public string ap_mat { get; set; }
        public string fecha_nac { get; set; } 
        public string nacionalidad { get; set; }
        public string telefono { get; set; }
    }
    }
