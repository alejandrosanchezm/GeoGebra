using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGebra
{
    /// <summary>
    /// Clase simplificada de un punto hecha
    /// a medida para la barChart Calculator
    /// </summary>
    public class Pair
    {
        public Pair(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float x { get; set; }
        public float y { get; set; }

    }

    /// <summary>
    /// Clase que contiene dos datos:
    /// El punto que se quiere representar y el número de ocurrencias
    /// en una lista de ese punto
    /// </summary>
    public class PointOcurrences
    {
        /// <summary>
        /// Número de veces que se repite un dato tipo Pair
        /// </summary>
        public int ocurrences { get; set; }

        /// <summary>
        /// El valor contabilizado.
        /// </summary>
        public Pair pt { get; set; }

        public PointOcurrences(int v, (float x, float y) key)
        {
            this.ocurrences = v;
            this.pt = new Pair(key.x, key.y);
        }
    }

    /// <summary>
    /// Tiene la función de, 
    /// dado un conjunto de puntos de un DataCollection, 
    /// contar el número de ocurrencias de cada punto
    /// creando con ellos una lista, y extrayendo los datos
    /// principales que van a ser necesarios para representarlos
    /// </summary>
    public class BarChartCalculator
    {
        /// <summary>
        /// Lista de la Clase PointOcurrences.
        /// </summary>
        public List<PointOcurrences> lst { get; set; }

        /// <summary>
        /// Valor máximo de número de ocurrencias dentro de la lista. 
        /// </summary>
        public int maxNumberOfValues { get; set; }

        /// <summary>
        /// Número de barras dentro de la lista.
        /// </summary>
        public int numberOfBars { get; set; }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="dc">DataCollection a analizar.</param>
        public BarChartCalculator(DataCollection dc)
        {
            lst = GetPointOcurrence(dc);
            maxNumberOfValues = GetMaxNumOcurrences(lst);
            numberOfBars = lst.Count();
        }

        /// <summary>
        /// Devuelve una lista de Point ocurrences, que es un tipo de Par de valores en 
        /// en el que se indica el Punto (x,y) que se encuentra en el DataCollection dc, 
        /// y el número de ocurrencias de ese punto dentro de la DataCollection.
        /// </summary>
        /// <param name="dc">DataCollection a analizar.</param>
        /// <returns>
        /// Una Lista de PointOcurrences.
        /// </returns>
        private List<PointOcurrences> GetPointOcurrence(DataCollection dc)
        {
            var l1 = dc.data.GroupBy(i => (i.x, i.y));
            List<PointOcurrences> lst = new List<PointOcurrences>();
            foreach (var grp in l1)
            {
                lst.Add(new PointOcurrences(grp.Count(), grp.Key));
            }
            return lst;
        }

        /// <summary>
        /// Devuelve el número máximo de ocurrencias que tiene alguno de los
        /// elementos dentro de la lista generada a partir de la función 
        /// GetPointOcurrence(DataCollection dc), que se ejecuta automáticamente
        /// antes al crear el constructor de la clase.
        /// </summary>
        /// <param name="lst">Lista de PointOcurrences</param>
        /// <returns>
        /// Entero positivo indicando el número máximo de ocurrencias.
        /// </returns>
        private int GetMaxNumOcurrences(List<PointOcurrences> lst)
        {
            return lst.Max(r => r.ocurrences);
        }
    }

}
