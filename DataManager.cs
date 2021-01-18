using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGebra
{
    /// <summary>
    /// Clase que almacena un conjunto de DataCollection.
    /// </summary>
    public class DataManager
    {
        /// <summary>
        /// Lista de las DataCollection que maneja el programa.
        /// </summary>
        public List<DataCollection> dataCollection { get; set; }

        /// <summary>
        /// ID que se utiliza cuando se añade un nuevo DataCollection.
        /// </summary>
        int id_counter = 1;

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        public DataManager()
        {
            this.dataCollection = new List<DataCollection>();
            this.dataCollection.Add(new DataCollection("Coleccion_" + id_counter.ToString()));
            this.id_counter++;
        }

        /// <summary>
        /// Añade una nueva DataCollection
        /// </summary>
        public void NewDataCollection()
        {
            this.dataCollection.Add(new DataCollection("Coleccion_" + id_counter.ToString()));
            this.id_counter++;
        }

        /// <summary>
        /// Borra un DataCollection Por su Nombre.
        /// </summary>
        /// <param name="name">Nombre de la DataCollection a eliminar.</param>
        public void DeleteDataCollectionByName(string name)
        {
            var itemToRemove = this.dataCollection.Single(r => r.name == name);
            this.dataCollection.Remove(itemToRemove);
        }

        /// <summary>
        /// Borra un DataCollection concreto.
        /// </summary>
        /// <param name="dc"> DataCollection a eliminar.</param>
        public void DeleteDataCollection(DataCollection dc)
        {
            this.dataCollection.Remove(dc);
        }

        /// <summary>
        /// Actualiza un DataCollection por su nombre
        /// </summary>
        /// <param name="dc">DataCollection con el que se va a actualizar.</param>
        /// <returns>
        /// Devuelve 0 en caso de éxito.
        /// </returns>
        public int UpdateDataCollection(DataCollection dc)
        {
            int index;
            try
            {
                var value = this.dataCollection.First(d => d.name == dc.name);
                if (value != null)
                {
                    index = this.dataCollection.IndexOf(value);
                    this.dataCollection[index] = dc;
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }

        }

        /// <summary>
        /// Añade una nueva colección al DataManager
        /// </summary>
        /// <param name="dc">DataCollection a añadir.</param>
        /// <returns>
        /// Devuelve 0 en caso de éxito.
        /// </returns>
        public int AddNewCollection(DataCollection dc)
        {
            if (dataCollection.Exists(dc_i => dc_i.name == dc.name)) return -1;
            else
            {
                dc.id_counter = id_counter;
                id_counter++;
                dataCollection.Add(dc);
                return 0;
            }
        }

    }
}
