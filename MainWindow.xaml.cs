using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace GeoGebra
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Clase que guarda una lista con las colecciones de datos maneja el programa.
        /// </summary>
        public DataManager dm;

        /// <summary>
        /// DataCollection que está manejando actualmente la ventana principal.
        /// </summary>
        public DataCollection currentDc;

        /// <summary>
        /// Clase auxiliar que contiene las funciones de lectura y escritura en ficheros.
        /// </summary>
        public FileFunctions fc;

        /// <summary>
        /// Se encarga de ejecutar toda la lógica del canvas.
        /// </summary>
        public CanvasController canvasController;

        /// <summary>
        /// Indica si se han modificado o no algunos datos del programa, y sirve 
        /// para decidir si crear un cuadro preguntando por el guardado de datos 
        /// al finalizar el programa.
        /// </summary>
        public bool hasChangeDataContent = false;

        /// <summary>
        /// Variable auxiliar para el cambio de nombre de un DataCollection.
        /// </summary>
        private string previousSelectedName;

        /// <summary>
        /// Lista para el Control de las Ventanas Abiertas.
        /// </summary>
        private static List<HojaDeDatos> windows;

        /// <summary>
        /// Lista para el Control de las Ventanas Abiertas.
        /// </summary>
        private List<String> windows_name;

        /// <summary>
        /// Variable que es verdadera mientras está pulsado el botón izquierdo del ratón.
        /// </summary>
        bool mouseDown = false;

        /// <summary>
        /// Punto donde se pulsó el botón izquierdo del ratón.
        /// </summary>
        Point mouseDownPos; 


        /*
         *      ===================================================
         *               COMPORTAMIENTO VENTANA PRINCIPAL
         *      ===================================================
         */

        /// <summary>
        /// Contructor de la Clase.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Initialize_Data_Collections();
        }

        /// <summary>
        /// Función de inicialización del programa.
        /// </summary>
        private void Initialize_Data_Collections()
        {
            dm = new DataManager();
            windows = new List<HojaDeDatos>();
            windows_name = new List<String>();
            fc = new FileFunctions();
            canvasController = new CanvasController(canGraph);
            canGraph = canvasController.Window_Loaded();
            x_coordinate.Text = "0";
            y_coordinate.Text = "0";

            currentDataCollections.ItemsSource = dm.dataCollection;
            currentDc = dm.dataCollection[0];
            currentCollectionName.Text = currentDc.name.ToString();
        }

        /// <summary>
        /// Función de cierre del programa.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (hasChangeDataContent)
            {
                MessageBoxResult valor = System.Windows.MessageBox.Show("¿Desea guardar antes de salir?", "Advertencia", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (valor)
                {
                    case MessageBoxResult.Yes:
                        fc.SaveAllCollectionsOnCSV(dm);
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }

        }

        /// <summary>
        /// Indica si una ventana está abierta o cerrada
        /// </summary>
        /// <param name="name">Nombre de la Ventana.</param>
        /// <returns>
        /// Devuelve true si está abierta, false en caso contrario.
        /// </returns>
        public bool IsWindowOpen(string name = "")
        {
            if (windows.Find(s => s.Name == name) != null) return true;
            else return false;
        }


        /// <summary>
        /// Acción que es llamada por una instancia de HojaDeDatos cuando cierra su ventana.
        /// Elimina sus datos de las listas de ventanas.
        /// </summary>
        /// <param name="name">Nombre de la Ventana a Eliminar.</param>
        public void CloseMyWindow(string name)
        {
            windows.Remove(windows.Find(s => s.Name == name));
            windows_name.Remove(windows_name.Find(s => s == name));
        }

        /*
         *      ===================================================
         *                     MOVIMIENTO ZOOM
         *      ===================================================
         */

        /// <summary>
        /// Llama al CanvasController para que aumente el Zoom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoreZoom(object sender, EventArgs e)
        {
            canGraph = canvasController.MoreZoom();
        }

        /// <summary>
        /// Llama al CanvasController para que disminuya el Zoom.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LessZoom(object sender, EventArgs e)
        {
            canGraph = canvasController.LessZoom();
        }

        /// <summary>
        /// Llama al CanvasController para que restablezca los valores
        /// de Zoom, x_value e y_value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetViewValues(object sender, EventArgs e)
        {
            canGraph = canvasController.ResetViewValues();
        }

        /*
         *      ===================================================
         *                        MOVIMIENTO EJES
         *      ===================================================
         */

        /// <summary>
        /// Llama al CanvasController para que desplace
        /// hacia arriba la vista del Canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpGraph(object sender, EventArgs e)
        {
            canGraph = canvasController.UpGraph();
        }

        /// <summary>
        /// Llama al CanvasController para que desplace
        /// hacia abajo la vista del Canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownGraph(object sender, EventArgs e)
        {
            canGraph = canvasController.DownGraph();
        }

        /// <summary>
        /// Llama al CanvasController para que desplace
        /// hacia la izquierda la vista del Canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftGraph(object sender, EventArgs e)
        {
            canGraph = canvasController.LeftGraph();
        }

        /// <summary>
        /// Llama al CanvasController para que desplace
        /// hacia la derecha la vista del Canvas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightGraph(object sender, EventArgs e)
        {
            canGraph = canvasController.RightGraph();
        }

        /*
         *      ===================================================
         *                    CAMBIA EL TIPO DE GRÁFICA
         *      ===================================================
         */


        /// <summary>
        /// Cambia en el CanvasController tipo de gráfica a Histograma.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeToPointsBarGraph(object sender, EventArgs e)
        {
            if (currentDc.data.Count > 2){
                canvasController.ChangeToPointsBarGraph();
                if (canvasController.IsInPointsBarGraph())
                {
                    PolylineControlPannel.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("No hay datos suficientes para una gráfica de barras de puntos.", "No hay datos");
            }
        }

        /// <summary>
        /// Cambia en el CanvasController tipo de gráfica a Gráfica Polilínea.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeToLineGraph(object sender, EventArgs e)
        {
            canvasController.ChangeToLineGraph();
            if (!canvasController.IsInPointsBarGraph()){
                PolylineControlPannel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Cambia en el CanvasController tipo de gráfica a Gráfica de Puntos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeToScatterGraph(object sender, EventArgs e)
        {
            canvasController.ChangeToScatterGraph();
            if (!canvasController.IsInPointsBarGraph())
            {
                PolylineControlPannel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Cambia en el CanvasController el tipo de gráfica a Gráfica de Barras.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeToBarGraph(object sender, EventArgs e)
        {
            canvasController.ChangeToBarGraph();
            if (!canvasController.IsInPointsBarGraph())
            {
                PolylineControlPannel.Visibility = Visibility.Visible;
            }
        }

        /*
         *      ===================================================
         *                        FUNCIONES DE MENU
         *      ===================================================
         */

        /// <summary>
        /// Llama al FileFunctions para carga un fichero de datos en una nueva colección.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadFileWithData(object sender, RoutedEventArgs e)
        {
            DataCollection tmp = fc.ReadCollectionFromCSVFile();
            if (tmp != null)
            {
                int counter = 1;
                string tmp_name = tmp.name;

                while (dm.AddNewCollection(tmp) != 0)
                {
                    tmp.name = tmp_name + " (" + counter + ")";
                    counter++;
                }
                currentDataCollections.ItemsSource = null;
                currentDataCollections.ItemsSource = dm.dataCollection;
            }
        }

        /// <summary>
        /// Llama al FileFunctions para guardar la colección de datos en un fichero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSpreadsheetFile(object sender, RoutedEventArgs e)
        {
            fc.SaveCollectionOnCSVFile(currentDc);
        }

        /// <summary>
        /// Llama al FileFunctions para guardar una imagen del canvas en formato PNG.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveGraphFile(object sender, RoutedEventArgs e)
        {
            fc.SaveImage();
        }

        /// <summary>
        /// Guarda todas las colecciones en un CSV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAllDataCollections(object sender, RoutedEventArgs e)
        {
            fc.SaveAllCollectionsOnCSV(dm);
        }

        /// <summary>
        /// Cierre del programa.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenHelp(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"Manual.pdf");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                System.Windows.MessageBox.Show("No se ha encontrado el manual", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*
         *      ===================================================
         *                         LIST BOX
         *      ===================================================
         */

        /// <summary>
        /// Abre una nueva ventana de Hoja de Datos para editar la colección seleccionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDataEditorWindow(object sender, RoutedEventArgs e)
        {
            DataCollection dc = (DataCollection)currentDataCollections.SelectedItem;
            if (dc != null)
            {
                if (!windows_name.Exists(w => w == dc.name))
                {
                    HojaDeDatos w = new HojaDeDatos(dc.name, dc.data, dc.id_counter);
                    windows.Add(w);
                    windows_name.Add(dc.name);
                    w.Show();
                }
                else
                {
                    windows.Find(s => s._id == dc.id_counter).WindowState = (WindowState)FormWindowState.Normal;
                }
            }
        }

        /// <summary>
        /// Cambia el color de representación de la colección seleccionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            DataCollection dc = (DataCollection)currentDataCollections.SelectedItem;
            if (dc != null)
            {
                ColorDialog dlg = new ColorDialog();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dm.dataCollection.Find(s => s.name == dc.name).color = new SolidColorBrush(Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B));
                }
            }
            currentDataCollections.ItemsSource = null;
            currentDataCollections.ItemsSource = dm.dataCollection;
            DrawGraph(currentDc.name);
            hasChangeDataContent = true;
        }

        /// <summary>
        /// Cambia el nombre de la colección de datos seleccionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeName(object sender, EventArgs e)
        {
            try
            {
                if (currentDc != null)
                {
                    DataCollection dc = dm.dataCollection.Find(s => s.name == previousSelectedName);
                    DataCollection coincidence = dm.dataCollection.Find(s => s.name == currentCollectionName.Text);
                    if (!IsWindowOpen(dc.name) && coincidence == null)
                    {

                        dm.dataCollection.Find(s => s.name == previousSelectedName).name = currentCollectionName.Text;
                        dc = (DataCollection)currentDataCollections.SelectedItem;
                        if (dc != null)
                        {
                            currentCollectionName.Text = dc.name;
                            previousSelectedName = dc.name;
                            currentDataCollections.ItemsSource = null;
                            currentDataCollections.ItemsSource = dm.dataCollection;
                            hasChangeDataContent = true;
                        }
                    }
                    else
                    {
                        if (IsWindowOpen(dc.name)) System.Windows.MessageBox.Show("Cierra la ventana del editor antes de cambiar el nombre.");
                        else if (coincidence != null) System.Windows.MessageBox.Show("Ya existe una colección con ese nombre.");
                    }
                }
            }
            catch (Exception)
            {
                  System.Windows.MessageBox.Show("Seleccione antes una colección.");
            }
        }

        /// <summary>
        /// Crea una nueva colección.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void CreateNewCollection(object sender, RoutedEventArgs e)
        {
            dm.NewDataCollection();
            currentDataCollections.ItemsSource = null;
            currentDataCollections.ItemsSource = dm.dataCollection;
        }
        
        /// <summary>
        /// Borra la colección seleccionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSelectedCollection(object sender, RoutedEventArgs e)
        {
            DataCollection dc = (DataCollection)currentDataCollections.SelectedItem;
            if (dc != null)
            {
                MessageBoxResult valor = System.Windows.MessageBox.Show("¿Estás seguro de que deseas borrar " + dc.name + " ?", "Advertencia", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (valor)
                {
                    case MessageBoxResult.Yes:
                        dm.DeleteDataCollection(dc);
                        break;
                    case MessageBoxResult.Cancel:
                    case MessageBoxResult.No:
                    default:
                        return;
                }
            }
            currentDataCollections.ItemsSource = null;
            currentDataCollections.ItemsSource = dm.dataCollection;
        }

        /// <summary>
        /// Muestra el nombre de la colección seleccionada y lo representa por pantalla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mlb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataCollection dc = (DataCollection)currentDataCollections.SelectedItem;
            if (dc != null)
            {
                currentCollectionName.Text = dc.name;
                previousSelectedName = dc.name;
                this.currentDc = dc;
                canGraph = canvasController.DrawGraph(currentDc);
            }
        }

        /// <summary>
        /// Función pública que es llamada cuando se necesita volver a representar una función.
        /// </summary>
        /// <param name="name">Nombre de la DataCollection a Representar.</param>
        public void DrawGraph(string name)
        {
            canGraph = canvasController.DrawGraph(dm.dataCollection.Find(s => s.name == name));
        }

        /*
         *      ===================================================
         *                FUNCIONES DE SELECCIÓN DE DATOS
         *      ===================================================
         */
        
        /// <summary>
        /// Acción cuando se presiona el Mouse sobre el Canvas (Iniciar Selección)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (canvasController != null)
            {
                if (!canvasController.IsInPointsBarGraph())
                {
                    // Captura la posición del Ratón.
                    mouseDown = true;
                    mouseDownPos = e.GetPosition(canGraph);
                    canGraph.CaptureMouse();

                    // Establece la posición inicial del rectángulo.       

                    Canvas.SetLeft(selectionBox, mouseDownPos.X);
                    Canvas.SetTop(selectionBox, mouseDownPos.Y);
                    selectionBox.Width = 0;
                    selectionBox.Height = 0;

                    // Hace visible el rectángulo de selección.
                    selectionBox.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Acción cuando se suelta el botón del Mouse del Canvas (Finalizar Selección).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (canvasController != null)
            {
                if (!canvasController.IsInPointsBarGraph())
                {
                    // Suelta el mouse y para el seguimiento.
                    mouseDown = false;
                    canGraph.ReleaseMouseCapture();

                    // Oculta el rectángulo
                    selectionBox.Visibility = Visibility.Collapsed;
                    Point mouseUpPos = e.GetPosition(canGraph);
                    try
                    {
                        if (currentDc.data.Count > 0)
                        {
                            // Elimina los puntos que se encuentran entre las dos esquinas del Rectángulo
                            currentDc = DeletePointsOutRange(currentDc, canvasController.ConvertedToMatrix(mouseDownPos), canvasController.ConvertedToMatrix(mouseUpPos));
                            int index = dm.dataCollection.IndexOf(dm.dataCollection.Find(s => s.name == currentDc.name));
                            if (index >= 0) dm.dataCollection[index] = currentDc;
                            // Actualiza los datos de la Hoja de Datos de la colección si está abierta
                            HojaDeDatos hdd = (HojaDeDatos)windows.Find(s => s._id == currentDc.id_counter);
                            if (hdd != null)
                            {
                                hdd.ReplaceCurrentDataCollection(currentDc);
                            }
                            canGraph.Children.Clear();

                            // Actualiza el Canvas con los nuevos datos
                            canvasController.UpdateScreen();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Borra los datos fuera de dos puntos determinados en un DataCollection.
        /// </summary>
        /// <param name="dc">DataCollection donde eliminar los puntos.</param>
        /// <param name="p1">Punto 1.</param>
        /// <param name="p2">Punto 2.</param>
        /// <returns>
        /// Devuelve la DataCollection con los puntos ya eliminados.
        /// </returns>
        public DataCollection DeletePointsOutRange(DataCollection dc, Point p1, Point p2)
        {
            double min_x, max_x, min_y, max_y;

            min_x = (p1.X < p2.X) ? p1.X : p2.X;
            max_x = (p1.X > p2.X) ? p1.X : p2.X;
            min_y = (p1.Y < p2.Y) ? p1.Y : p2.Y;
            max_y = (p1.Y > p2.Y) ? p1.Y : p2.Y;

            dc.data.RemoveAll(s => s.x < min_x || s.x > max_x || s.y < min_y || s.y > max_y);

            return dc;
        }

        /// <summary>
        /// Acción cuando se mueve el Mouse sobre el Canvas (Rastrear Selección).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (canvasController != null)
            {
                if (!canvasController.IsInPointsBarGraph())
                {
                    if (mouseDown)
                    {
                        // When the mouse is held down, reposition the drag selection box.

                        Point mousePos = e.GetPosition(canGraph);

                        if (mouseDownPos.X < mousePos.X)
                        {
                            Canvas.SetLeft(selectionBox, mouseDownPos.X);
                            selectionBox.Width = mousePos.X - mouseDownPos.X;
                        }
                        else
                        {
                            Canvas.SetLeft(selectionBox, mousePos.X);
                            selectionBox.Width = mouseDownPos.X - mousePos.X;
                        }

                        if (mouseDownPos.Y < mousePos.Y)
                        {
                            Canvas.SetTop(selectionBox, mouseDownPos.Y);
                            selectionBox.Height = mousePos.Y - mouseDownPos.Y;
                        }
                        else
                        {
                            Canvas.SetTop(selectionBox, mousePos.Y);
                            selectionBox.Height = mouseDownPos.Y - mousePos.Y;
                        }
                    }
                }
            }

        }


        /*
         *      ===================================================
         *                    FUNCIONES DE CHECKBOX
         *      ===================================================
         */

        /// <summary>
        /// Indica al Canvas que se tienen que renderizar los ejes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRenderAxisTrue(object sender, RoutedEventArgs e)
        {
            if (canvasController != null) canvasController.SetRenderAxis(true);
        }

        /// <summary>
        /// Indica al Canvas que no se tienen que renderizar los ejes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRenderAxisFalse(object sender, RoutedEventArgs e)
        {
            if (canvasController != null) canvasController.SetRenderAxis(false);
        }

        /// <summary>
        /// Indica al Canvas que se tiene que renderizar la cuadrícula.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRenderGridTrue(object sender, RoutedEventArgs e)
        {
            if (canvasController != null) canvasController.SetRenderGrid(true);
        }

        /// <summary>
        /// Indica al Canvas que no se tiene que renderizar la cuadrícula.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRenderGridFalse(object sender, RoutedEventArgs e)
        {
            if (canvasController != null) canvasController.SetRenderGrid(false);
        }



        /*
         *      ===================================================
         *           CUADROS DE TEXTO DE LAS COORDENADAS
         *      ===================================================
         */

        /// <summary>
        /// Llama al CanvasController para que cambie el valo de x_values 
        /// en el canvasModel cuando se actualiza el cuadro de texto de la
        /// coordenada X en la ventana principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetNewXCoordinate(object sender, System.EventArgs e)
        {
            int x_number;
            if (Int32.TryParse(x_coordinate.Text, out x_number))
            {
                canGraph = canvasController.ChangeXCoord(x_number);
            }
            x_coordinate.Text = x_number.ToString();
        }

        /// <summary>
        /// Llama al CanvasController para que cambie el valo de y_values 
        /// en el canvasModel cuando se actualiza el cuadro de texto de la
        /// coordenada Y en la ventana principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetNewYCoordinate(object sender, System.EventArgs e)
        {
            int y_number;
            if (Int32.TryParse(y_coordinate.Text, out y_number))
            {
                canGraph = canvasController.ChangeYCoord(y_number);
            }
            y_coordinate.Text = y_number.ToString();
        }

        /// <summary>
        /// Comprueba que la información introducida en el cuadro de texto X sea 
        /// de tipo entero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9.-]+").IsMatch(e.Text);

        }

        /// <summary>
        /// Comprueba que la información introducida en el cuadro de texto X sea 
        /// de tipo entero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9.-]+").IsMatch(e.Text);

        }

        /// <summary>
        /// Actualiza dato X de coordenadas de la pantalla.
        /// </summary>
        /// <param name="x">Valor x.</param>
        public void UpdateXCoordValue(double x)
        {
            x_coordinate.Text = x.ToString();
        }

        /// <summary>
        /// Actualiza dato Y de coordenadas de la pantalla.
        /// </summary>
        /// <param name="y">Valor y.</param>
        public void UpdateYCoordValue(double y)
        {
            y_coordinate.Text = y.ToString();
        }

        /// <summary>
        /// Llama a las diferentes Hojas de Datos que siguen abiertas para que
        /// actualizen sus datos en el DataManager.
        /// </summary>
        public static void AskForUpdateAllCollections()
        {
            foreach(HojaDeDatos w in windows)
            {
                w.AskForUpdateDConDM();
            }
        }
    
    }

}
