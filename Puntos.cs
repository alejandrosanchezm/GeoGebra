using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GeoGebra
{
    /// <summary>
    /// Clase adaptada de un par x, y
    /// que se va a utilizar en las hojas de datos
    /// para el Datagrid, y que servirá también 
    /// para almacenar la información.
    /// </summary>
    public class Punto
    {
        /// <summary>
        /// Sirve para su uso en las Hojas de Datos, indica si está o no marcado el Punto.
        /// </summary>
        public bool check { get; set; }

        /// <summary>
        /// Valor X del Punto.
        /// </summary>
        public float x { get; set; }

        /// <summary>
        /// Valor Y del Punto.
        /// </summary>
        public float y { get; set; }

        /// <summary>
        /// ID del Punto. Indica el orden de 'Creación'.
        /// </summary>
        public float id { get; set; }
    }

    /// <summary>
    /// Clase que sirve para almacenar cualquier
    /// colección de Punto que se va a utilizar a lo largo del
    /// programa.
    /// </summary>
    public class DataCollection
    {
        /// <summary>
        /// Nombre de la colección.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Lista de puntos de la colección.
        /// </summary>
        public List<Punto> data { get; set; }

        /// <summary>
        /// Referencia al color de la colección.
        /// </summary>
        public SolidColorBrush color { get; set; }

        /// <summary>
        /// ID que se le asigna en la creación. Sirve para no generar dos Colecciones con el mismo nombre.
        /// </summary>
        public int id_counter = 0;

        /// <summary>
        /// Brush que inicializa los datos en caso de no asignarsele ningún color.
        /// </summary>
        private SolidColorBrush Black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        /*
         *  ==============================
         *          CONSTRUCTORES
         *  ==============================
         */
        public DataCollection(string name)
        {
            this.data = new List<Punto>();
            this.name = name;
            this.color = Black;
        }

        public DataCollection(string name, List<Punto> pts)
        {
            this.name = name;
            this.data = pts;
            this.color = Black;
        }

        public DataCollection(string name, List<Punto> pts, SolidColorBrush color)
        {
            this.data = pts;
            this.name = name;
            this.color = color;
        }
    }

}
