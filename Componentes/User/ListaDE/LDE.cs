using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace chaski_tours_desk.Componentes.User.ListaDE
{
    public class LDE
    {
        public Nodo Inicio { get; set; }
        public Nodo Fin { get; set; }
        public Nodo Actual { get; set; }
        public LDE()
        {
            Inicio = null;
            Fin = null;
        }

        public void crearListaDE(List<object> listaObj) { 
            Nodo nodoInicio = new Nodo(listaObj[0]);
            Nodo nodoFin = new Nodo(listaObj[listaObj.Count-1]);
            
            listaObj.RemoveAt(0);
            listaObj.RemoveAt(listaObj.Count - 1);

            Inicio = nodoInicio;
            Fin = nodoFin;

            Inicio.Siguiente = Fin;
            Inicio.Anterior = Fin;

            Fin.Siguiente = Inicio;
            Fin.Anterior = Inicio;


            agregarNodosRestantes(listaObj);

            Actual = Inicio;
        }

        private void agregarNodosRestantes(List<object> lstNodosRestantes) { 
            foreach (var item in lstNodosRestantes)
            {
                Nodo nuevoNodo = new Nodo(item);

                nuevoNodo.Anterior = Fin;
                nuevoNodo.Siguiente = Inicio;

                Fin.Siguiente = nuevoNodo;
                Inicio.Anterior = nuevoNodo;

                Fin = nuevoNodo;
            }
        }

        public void pasarSiguiente()
        {
            if (Actual.Siguiente != null)
            {
                Actual = Actual.Siguiente;
            }
            else
            {
                Actual = Inicio;
            }
        }

        public void pasarAnterior() {
            if (Actual.Anterior != null)
            {
                Actual = Actual.Anterior;
            }
            else
            {
                Actual = Fin; 
            }
        }
    }
}
