using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GeoGebra
{
    /// <summary>
    /// Lógica de interacción para HojaDeDatos.xaml
    /// </summary>
    public partial class HojaDeDatos : Window
    {
        /// <summary>
        ///  Nombre de la colección y de la Ventana.
        /// </summary>
        public string name;

        /// <summary>
        /// ID de la colección y de la Ventana.
        /// </summary>
        public int _id;

        /// <summary>
        /// Cambia el nombre de la Hoja de Datos.
        /// </summary>
        private ViewModel _vm;

        /// <summary>
        /// Indica si se ha modificado algún dato.
        /// </summary>
        private bool hasChangedDataContent = false;

        /// <summary>
        /// Valores de los campos de texto.
        /// </summary>
        public float min_value = 0, max_value = 0;

        /// <summary>
        /// Número de puntos a generar.
        /// </summary>
        public int n_value = 0;

        /// <summary>
        /// Cadenas de los campos de texto.
        /// </summary>
        public string polinomio, min_s, max_s, n_s = null;

        /// <summary>
        /// Número de puntos en la hoja de Datos.
        /// </summary>
        public int currNumberOfPoints = 0;

        /// <summary>
        /// Id para la creación de puntos.
        /// </summary>
        private float id_counter = 0;

        /// <summary>
        /// Referencia a la Ventana Principal.
        /// </summary>
        MainWindow mainWindow;

        /// <summary>
        /// Lista de Puntos con los que trabaja la Hoja de Datos.
        /// </summary>
        private List<Punto> CurrentPoints { get; set; }

        /// <summary>
        /// Lista de Backup en caso de querer reestablecer los datos.
        /// </summary>
        private static Punto[] backup;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="_name">Nombre de la Hoja de datos</param>
        /// <param name="data">Colección de datos que se le asigna. En caso de ser nula, se crea una nueva.</param>
        /// <param name="id">Se le asigna un id en base a la data collection</param>
        public HojaDeDatos(string _name, List<Punto> data, int id)
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            if (data != null)
                CurrentPoints = data;
            else
                CurrentPoints = new List<Punto>();
            backup = new Punto[CurrentPoints.Count()];
            CurrentPoints.CopyTo(backup);
            if (data.Count == 0)
            {
                Punto pt = new Punto() { id = id_counter, check = false, x = 0, y = 0 };
                if (pt != null) CurrentPoints.Add(pt);
                id_counter++;
            }
            Datagrid.ItemsSource = CurrentPoints;
            this.name = _name;
            this._id = id;
            _vm = new ViewModel();
            _vm.MyTitle = name;
            this.DataContext = _vm;
            hasChangedDataContent = false;
            CountNumberOfPoints();
        }

        /// <summary>
        /// Función que se ejecuta cuando se cierra la ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (hasChangedDataContent)
                {
                    MessageBoxResult valor = MessageBox.Show("¿Desea guardar los datos?", "Advertencia", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (valor)
                    {
                        case MessageBoxResult.Yes:
                            UpdateDataCollectionOnManager();
                            mainWindow.hasChangeDataContent = true;
                            break;
                        case MessageBoxResult.No:
                            ResetDataCollectionOnManager();
                            break;
                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            break;
                        default:
                            e.Cancel = true;
                            break;
                    }
                }
                mainWindow.CloseMyWindow(name);
            }
            catch(Exception ec)
            {
                return;
            }

        }

        /*
         *      ===================================================
         *                      FUNCIONES MENÚ 
         *      ===================================================
         */

        /// <summary>
        /// Guarda los datos en el DM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveDataOnDM(object sender, RoutedEventArgs e)
        {
            UpdateDataCollectionOnManager();
        }
        
        /// <summary>
        /// Abre el manual de usuario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHelp(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"Manual.pdf");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("No se ha encontrado el manual", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*
         *      ===================================================
         *            FUNCIONES MENÚ CONTEXTUAL DEL DATAGRID
         *      ===================================================
         */
        /// <summary>
        /// Crea una nueva fila en la Hoja de Datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewRowAtEnd(object sender, RoutedEventArgs e)
        {
            Datagrid.CommitEdit();
            Datagrid.UnselectAll();
            Punto pt = new Punto() { id = id_counter, check = false, x = 0, y = 0 };
            if (pt != null) CurrentPoints.Add(pt);
            Datagrid.ItemsSource = null;
            Datagrid.ItemsSource = CurrentPoints;
            currNumberOfPoints++;
            id_counter++;
            id_counter++;
            CountNumberOfPoints();
            hasChangedDataContent = true;
        }

        /// <summary>
        /// Añade una fila en la posición del elemento seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewRow(object sender, RoutedEventArgs e)
        {
            Datagrid.CommitEdit();
            var cellInfo = Datagrid.SelectedItem;
            try
            {
                float id_1;
                float final_id;
                try
                {
                    id_1 = ((Punto)cellInfo).id;
                    var tmp_list = CurrentPoints.OrderBy(o => o.id).ToList();
                    var index = tmp_list.FindIndex(o => o.id == id_1);
                    if (tmp_list.Count > index)
                    {
                        var id_2 = tmp_list[index + 1].id;
                        final_id = (id_2 + id_1) / 2;
                        Test.Text += id_1.ToString() + " " + id_2.ToString() + " " + final_id.ToString() + "\n";
                    }
                    else
                    {
                        final_id = id_1 + 1;
                    }
                }
                catch (Exception ex)
                {
                    final_id = 0;
                }
                Punto pt = new Punto() { id = final_id, check = false, x = 0, y = 0 };
                if (pt != null) CurrentPoints.Add(pt);
                CurrentPoints = RefactorDCByID(CurrentPoints);
                CurrentPoints.OrderBy(o => o.id).ToList();
                Datagrid.ItemsSource = null;
                Datagrid.ItemsSource = CurrentPoints;
                currNumberOfPoints++;
                id_counter++;
                CountNumberOfPoints();
                hasChangedDataContent = true;

            }
            catch (Exception ex)
            {
                Test.Text += "Se produjo un error.\n";
            }
        }

        /// <summary>
        /// Elimina la última fila en la Hoja de Datos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLastRow(object sender, RoutedEventArgs e)
        {
            if (CurrentPoints.Count > 0)
            {
                Datagrid.CommitEdit();
                Datagrid.UnselectAll();
                CurrentPoints.RemoveAt(CurrentPoints.Count - 1);
                Datagrid.ItemsSource = null;
                Datagrid.ItemsSource = CurrentPoints;
                currNumberOfPoints--;
                CountNumberOfPoints();
                hasChangedDataContent = true;
            }
        }

        /// <summary>
        /// Muestra el número de datos en el cuadro de texto.
        /// </summary>
        public void ShowNumberOfPoints()
        {
            n_of_points.Text = "Numero de Puntos: " + currNumberOfPoints.ToString();
        }

        /// <summary>
        /// Elimina las filas seleccionadas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSelectedRows(object sender, RoutedEventArgs e)
        {
            Datagrid.CommitEdit();
            int return_value;
            return_value = CurrentPoints.RemoveAll(s => s.check == true);
            if (return_value > 0)
            {
                Datagrid.ItemsSource = null;
                Datagrid.ItemsSource = CurrentPoints;
                currNumberOfPoints -= return_value;
                CountNumberOfPoints();
                hasChangedDataContent = true;
            }

        }

        /// <summary>
        /// Elimina todos los puntos de la tabla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanTable(object sender, EventArgs e)
        {
            Datagrid.UnselectAll();
            CurrentPoints = new List<Punto>();
            id_counter = 0;
            Punto pt = new Punto() { id = id_counter, check = false, x = 0, y = 0 };
            if (pt != null) CurrentPoints.Add(pt);
            Datagrid.ItemsSource = CurrentPoints;
            currNumberOfPoints = 0;
            CountNumberOfPoints();
            hasChangedDataContent = true;
            id_counter++;
        }

        /// <summary>
        /// Selecciona todos los puntos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllValues(object sender, EventArgs e)
        {
            Datagrid.CommitEdit();
            CurrentPoints.ForEach(o => o.check = true);
            Datagrid.ItemsSource = null;
            Datagrid.ItemsSource = CurrentPoints;
            Datagrid.SelectAllCells();
        }


        /// <summary>
        /// Ordena los puntos por orden de creación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderByCreation(object sender, EventArgs e)
        {
            Datagrid.CommitEdit();
            CurrentPoints = CurrentPoints.OrderBy(o => o.id).ToList();
            Datagrid.ItemsSource = CurrentPoints;
        }

        /// <summary>
        /// Ordena los puntos de menor a mayor por su valor X.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderByXValues(object sender, EventArgs e)
        {
            Datagrid.CommitEdit();
            CurrentPoints = CurrentPoints.OrderBy(o => o.x).ToList();
            Datagrid.ItemsSource = CurrentPoints;
        }

        /// <summary>
        /// Ordena los puntos de menor a mayor por su valor Y.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderByYValues(object sender, EventArgs e)
        {
            Datagrid.CommitEdit();
            CurrentPoints = CurrentPoints.OrderBy(o => o.y).ToList();
            Datagrid.ItemsSource = CurrentPoints;
        }

        /// <summary>
        /// Función Pública que reemplaza la colección de datos actual.
        /// </summary>
        /// <param name="newDc">
        /// Nueva DataCollection.
        /// </param>
        public void ReplaceCurrentDataCollection(DataCollection newDc)
        {
            CurrentPoints = null;
            CurrentPoints = newDc.data;
            Datagrid.ItemsSource = null;
            Datagrid.ItemsSource = CurrentPoints;
        }

        /// <summary>
        /// Refactoriza los IDs dentro de una lista de datos convirtiéndolos a números naturales.
        /// </summary>
        /// <param name="dc">Lista de Puntos a Refactorizar.</param>
        /// <returns>
        /// Devuelve la Lista de Puntos con los ID refactorizados.
        /// </returns>
        private List<Punto> RefactorDCByID(List<Punto> dc)
        {
            List<Punto> new_dc = dc;
            new_dc = dc.OrderBy(o => o.id).ToList();
            for (int i = 0; i < new_dc.Count; i++) new_dc[i].id = i;
            return new_dc;
        }

        /*
         *      ===================================================
         *               FUNCIONES BOTONES DE LA PANTALLA
         *      ===================================================
         */

        /// <summary>
        /// Limpia la consola
        /// </summary>
        private void CleanConsole(object sender, EventArgs e)
        {
            Test.Text = "";
        }

        /// <summary>
        /// Llama a la Ventana Principal para que represente la gráfica
        /// </summary>
        private void Represent(object sender, EventArgs e)
        {
            Datagrid.UnselectAll();
            if (CurrentPoints.Count() > 0)
            {
                if (UpdateDataCollectionOnManager() == 0)
                {
                    mainWindow.currentDc = mainWindow.dm.dataCollection.Find(s => s.name == name);
                    mainWindow.currentCollectionName.Text = name;
                    mainWindow.DrawGraph(name);
                }
            }
        }

        /// <summary>
        /// Actualiza la colección de datos en el DataManager
        /// </summary>
        /// <returns>
        /// Devuelve 0 en caso de éxito; otro número (Negativo) en caso contrario.
        /// </returns>
        private int UpdateDataCollectionOnManager()
        {
            var dc = mainWindow.dm.dataCollection.Find(s => s.name == name);
            int result = mainWindow.dm.UpdateDataCollection(new DataCollection(name, CurrentPoints, dc.color));
            if (result != 0)
            {
                MessageBox.Show("Los datos no se han guardado correctamente.", "AdOvertencia");
                hasChangedDataContent = false;
                return -1;
            }
            else
            {
                if (mainWindow.currentDc.name == name)
                {
                    mainWindow.currentDataCollections.ItemsSource = null;
                    mainWindow.currentDataCollections.ItemsSource = mainWindow.dm.dataCollection;
                    mainWindow.DrawGraph(name);
                    return 0;
                }
                else return 0;
            }
        }

        /// <summary>
        /// Función pública que acutaliza los Datos del DataManager con los datos
        /// actuales de la hoja.
        /// </summary>
        /// <returns>
        /// Devuelve 0 en caso de éxito; otro número (Negativo) en caso contrario.
        /// </returns>
        public int AskForUpdateDConDM()
        {
            var dc = mainWindow.dm.dataCollection.Find(s => s.name == name);
            int result = mainWindow.dm.UpdateDataCollection(new DataCollection(name, CurrentPoints, dc.color));
            if (result != 0) return -1;
            else return 0;
        }

        /// <summary>
        /// Reestablece los datos del backup en la coleccíón del Data Manager.
        /// </summary>
        private void ResetDataCollectionOnManager()
        {
            var dc = mainWindow.dm.dataCollection.Find(s => s.name == name);
            int result = mainWindow.dm.UpdateDataCollection(new DataCollection(name, backup.ToList(), dc.color));
            if (result == 0)
            {
                mainWindow.currentDataCollections.ItemsSource = null;
                mainWindow.currentDataCollections.ItemsSource = mainWindow.dm.dataCollection;
                if (mainWindow.currentDc.name == name)
                {
                    mainWindow.currentDc = null;
                    mainWindow.currentDc = mainWindow.dm.dataCollection.Find(s => s.name == name);
                    mainWindow.currentCollectionName.Text = name;
                    mainWindow.DrawGraph(name);
                }
            }
            else
                MessageBox.Show("Ha habido un problema.", "Advertencia");
        }

        /// <summary>
        /// Al tocar el datagrid, se actualiza el valor del número de puntos.
        /// </summary>
        private void Datagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //Get the newly selected cells
            IList<DataGridCellInfo> selectedcells = e.AddedCells;
            CountNumberOfPoints();
        }

        /// <summary>
        /// Cuenta el número de puntos.
        /// </summary>
        private void CountNumberOfPoints()
        {
            currNumberOfPoints = CurrentPoints.Count();
            ShowNumberOfPoints();
            hasChangedDataContent = true;
        }

        /// <summary>
        /// Des-selecciona los datos de la tabla al tocar fuera de la tabla.
        /// </summary>
        private void OnDataGridMouseUp(object o, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                ((DataGrid)o).UnselectAll();
            }
        }

        /// <summary>
        /// Llama a la función que genera los puntos y los añade a la tabla.
        /// </summary>
        private void GeneratePoints(object sender, EventArgs e)
        {
            GeneratePoints_Action();
        }

        /*
         *      ===================================================
         *              FUNCIONES PARA GENERACIÓN PUNTOS
         *      ===================================================
         */

        /// <summary>
        /// Genera los puntos en base a los datos de los cuadros de texto de 
        /// valor mínimo, valor máximo, número de puntos y polinomios, y los añade
        /// al DataCollection de la Hoja de Datos
        /// </summary>
        private void GeneratePoints_Action()
        {
            min_s = min.Text;
            max_s = max.Text;
            n_s = n.Text;
            polinomio = Pol_text.Text;
            bool fail = false;
            bool delete = false;
            hasChangedDataContent = true;

            if (Check_values_consistence())
            {
                delete = AskIfDeleteData();
                float last_id = 0;
                if (CurrentPoints.Count() > 0)
                    last_id = CurrentPoints.OrderBy(o => o.id).ToList().Last().id;
                else
                    last_id = 0;
                double step = (max_value - min_value) / (n_value - 1);
                string incognita = "x";
                string copy_pol;
                List<Punto> tmp_values = new List<Punto>();
                Matematica mp = new Matematica();


                for (double val = min_value; val <= (max_value + (float)step / 2); val += step)
                {
                    copy_pol = mp.preprocessString(polinomio, incognita);
                    copy_pol = ReplaceVarByValue(copy_pol, incognita, val);
                    try
                    {
                        double result = (float)mp.ProgrammaticallyParse(copy_pol);
                        if (!delete)
                        {
                            last_id++;
                            tmp_values.Add(new Punto() { check = false, x = (float)val, y = (float)result, id = last_id });
                        }
                        else
                        {
                            tmp_values.Add(new Punto() { check = false, x = (float)val, y = (float)result });
                        }
                    }
                    catch (Exception)
                    {
                        Test.Foreground = Brushes.Red;
                        Test.Text = Test.Text + " >> Error matemático.\n";
                        fail = true;
                        break;
                    }
                }
                if (!fail)
                {
                    Test.Foreground = Brushes.Black;
                    if (!delete)
                    {
                        CurrentPoints = CurrentPoints.Concat(tmp_values).ToList();
                        Datagrid.ItemsSource = CurrentPoints;
                        id_counter = (int)(last_id + 1);
                    }
                    else
                    {
                        CurrentPoints = tmp_values;
                        Datagrid.ItemsSource = tmp_values;
                    }
                    Test.Text = Test.Text + " >> Correcto.\n";
                }
            }
        }

        /// <summary>
        /// Reemplaza en el polinomio la incógnita por el valor indicado.
        /// </summary>
        /// <param name="equation">Cadena con la expresión matemática a utilizar.</param>
        /// <param name="incognita">Cadena con la incógnica que se sustituirá por un valor numérico.</param>
        /// <param name="value">Valor numérico que sustituirá la incógnita.</param>
        /// <returns>
        /// Devuelve el polinomio con la incógnita ya sustituída.
        /// </returns>
        private string ReplaceVarByValue(string equation, string incognita, double value)
        {
            return equation.Replace(incognita, value.ToString()).Replace(",", ".");
        }

        /// <summary>
        /// Comprueba la consistencia de los valores introducidos; si es así, los convierte a valores numéricos.
        /// </summary>
        /// <returns>
        /// Devuelve True si se cumplen unos requisitos iniciales; False en caso contrario.
        /// </returns>
        private bool Check_values_consistence()
        {
            if (min_s == "" || max_s == "" || n_s == "" || polinomio == "")
            {
                Test.Foreground = Brushes.Red;
                Test.Text = Test.Text + " >> Error: Campos vacíos.\n";
                return false;
            }
            if (!float.TryParse(min_s, out min_value))
            {
                Test.Foreground = Brushes.Red;
                Test.Text = Test.Text + " >> Error: El mínimo tienen que ser un entero.\n";
                return false;
            }
            else if (!float.TryParse(max_s, out max_value))
            {
                Test.Foreground = Brushes.Red;
                Test.Text = Test.Text + " >> Error: El máximo tienen que ser un entero.\n";
                return false;
            }
            else if (!Int32.TryParse(n_s, out n_value))
            {
                Test.Foreground = Brushes.Red;
                Test.Text = Test.Text + " >> Error: Los puntos tienen que ser un entero.\n";
                return false;
            }
            else if (min_value >= max_value)
            {
                Test.Foreground = Brushes.Red;
                Test.Text = Test.Text + " >> Error: el mínimo debe ser menor que el máximo.\n";
                return false;
            }
            else if (n_value <= 0)
            {
                Test.Foreground = Brushes.Red;
                Test.Text = Test.Text + " >> Error: el número de puntos debe ser mayor o igual a 1.\n";
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Pregunta si Desea limpiar los datos de la tabla.
        /// </summary>
        /// <returns>
        /// Devuelve True si el usuario quiere borrar los datos; False en caso contrario.
        /// </returns>
        private bool AskIfDeleteData()
        {
            if (CurrentPoints.Count() > 1 || (CurrentPoints.Count == 2 && CurrentPoints[1].x == 0 && CurrentPoints[1].y == 0))
            {
                MessageBoxResult valor = MessageBox.Show("¿Desea limpiar los datos de la tabla?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (valor)
                {
                    case MessageBoxResult.Yes:
                        return true;
                    case MessageBoxResult.No:
                    default:
                        return false;
                }
            }
            else return true;
        }

    }
}
