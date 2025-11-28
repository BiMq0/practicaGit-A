using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chaski_tours_desk.Componentes.User.ListaDE
{
    public class Nodo
    {
        public Nodo Siguiente { get; set; }
        public Nodo Anterior { get; set; }
        public object Valor { get; set; }
        public Nodo(object valor)
        {
            Valor = valor;
            Siguiente = null;
            Anterior = null;
        }
    }
}
