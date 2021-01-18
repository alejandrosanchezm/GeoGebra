using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeoGebra
{
    /// <summary>
    /// CanvasController es la clase encargada de realizar las operaciones
    /// de creación y actualización de elementos en el Canvas de la ventana
    /// principal, desde la cuál es llamado.
    /// </summary>
    public class CanvasController
    {
        /// <summary>
        /// Referencia hacia la Ventana Principal.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Referencia a un objeto de tipo Canvas. Sirve para crear el Canvas que
        /// posteriormente se enviará a la ventana principal.
        /// </summary>
        private Canvas canvasObject { get; set; }

        /// <summary>
        /// Referencia hacia un Modelo del Canvas, donde se guardarán los datos de control.
        /// </summary>
        private CanvasModel canvasModel { get; set; }

        /// <summary>
        /// Referencia a la Clase que contiene las funciones para la representación de elementos en el Canvas.
        /// </summary>
        private CanvasFunctions canvasFunctions { get; set; }

        /// <summary>
        /// Sirve para guardar temporalmente el cuadro de selección que se encuentras en el Canvas cuando se 
        /// realiza la limpieza de éste.
        /// </summary>
        Rectangle selection;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="args">Canvas con el que se va a trabajar.</param>
        public CanvasController(Canvas args)
        {
            canvasObject = args;
            canvasModel = new CanvasModel(canvasObject.Width, canvasObject.Height);
            canvasFunctions = new CanvasFunctions();
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }


        /// <summary>
        /// Función que se carga al inicio. Carga en el canvas
        /// los ejes y la cuadrícula de fondo.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas pasado por argumento con los ejes
        /// y la cuadrícula de fondo.
        /// </return>
        public Canvas Window_Loaded()
        {
            return canvasFunctions.GridAndAxisDrawing(
                canvasModel, canvasObject,
                canvasModel.wxmin_initial, canvasModel.wxmax_initial,
                canvasModel.wymin_initial, canvasModel.wymax_initial,
                canvasModel.xstep_initial, canvasModel.ystep_initial);
        }

        /*
         *      ===================================================
         *                  CAMBIO DEL TIPO DE GRÁFICA
         *      ===================================================
         */
        /// <summary>
        /// Establece el tipo de Gráfica a Gráfica de Barras.
        /// </summary>
        public void ChangeToBarGraph()
        {
            canvasModel.graphOption = 3;
            mainWindow.canGraph = UpdateCanvas();

        }

        /// <summary>
        /// Establece el tipo de Gráfica a Gráfica Polilínea.
        /// </summary>
        public void ChangeToLineGraph()
        {
            canvasModel.graphOption = 0;
            mainWindow.canGraph = UpdateCanvas();
        }

        /// <summary>
        /// Establece el tipo de Gráfica a Gráfica de puntos.
        /// </summary>
        public void ChangeToScatterGraph()
        {
            canvasModel.graphOption = 2;
            mainWindow.canGraph = UpdateCanvas();
        }

        /// <summary>
        /// Establece el tipo de Gráfica a histograma.
        /// </summary>
        public void ChangeToPointsBarGraph()
        {
            canvasModel.graphOption = 1;
            mainWindow.canGraph = UpdateCanvas();
        }

        /// <summary>
        /// Función pública que devuelve si
        /// está o no en gráfico de barras.
        /// </summary>
        /// <return>
        /// Devuelve True si es gráfico de Barras.
        /// False en caso contrario.
        /// </return>
        public bool IsInPointsBarGraph()
        {
            return canvasModel.graphOption == 1 ? true : false;
        }

        /// <summary>
        /// Función que convierte un punto del dispotivo a 
        /// Coordenadas del Mundo con la matriz.
        /// </summary>
        /// <return>
        /// Devuelve el Punto convertido a coordenadas del Mundo.
        /// </return>
        public Point ConvertedToMatrix(Point pt)
        {
            Point pt2 = canvasModel.DtoW(pt);
            pt2.X = Math.Round(pt2.X, 5);
            pt2.Y = Math.Round(pt2.Y, 5);
            return pt2;
        }

        /*
         *      ===================================================
         *                             ZOOM
         *      ===================================================
         */

        // FUNCIONES PÚBLICAS 

        /// <summary>
        /// Función que Aumenta el Zoom.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con el Zoom Aumentado.
        /// </return>
        public Canvas MoreZoom()
        {
            if (canvasModel.graphOption != 1)
            {
                MoreZoomValues();
                return UpdateCanvas(true);
            }
            return canvasObject;
        }

        /// <summary>
        /// Función que Disminuye el Zoom.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con el Zoom Disminuído.
        /// </return>
        public Canvas LessZoom()
        {
            if (canvasModel.graphOption != 1)
            {
                LessZoomValues();
                return UpdateCanvas(true);
            }
            return canvasObject;
        }

        /// <summary>
        /// Devuelve los valores de Zoom, X, Y a
        /// los valores iniciales.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con los valores
        /// iniciales restituídos.
        /// </return>
        public Canvas ResetViewValues()
        {
            if (canvasModel.graphOption != 1)
            {
                canvasModel.x_value = 0;
                canvasModel.y_value = 0;
                canvasModel.Zoom = 0;
                return UpdateCanvas(true);
            }
            return null;
        }

        // FUNCIONES PRIVADAS

        /// <summary>
        /// Disminuye el valor de la variable de Zoom.
        /// </summary>
        private void LessZoomValues()
        {
            if (canvasModel.Zoom > -100)
            {
                if (canvasModel.Zoom <= -6 && canvasModel.Zoom > -30) canvasModel.Zoom -= 2;
                else if (canvasModel.Zoom <= -30) canvasModel.Zoom -= 10;
                else canvasModel.Zoom -= 1;
            }
        }

        /// <summary>
        /// Aumenta el valor de la variable de Zoom.
        /// </summary>
        private void MoreZoomValues()
        {
            if (canvasModel.Zoom < 8)
            {
                if (canvasModel.Zoom <= -6 && canvasModel.Zoom > -30) canvasModel.Zoom += 2;
                else if (canvasModel.Zoom <= -30) canvasModel.Zoom += 10;
                else canvasModel.Zoom += 1;
            }
        }

        /// <summary>
        /// Función que cambia directamente el valor de x_value por parámetro.
        /// </summary>
        /// <param name="x">Valor que se va a asignar a x_value</param>
        /// <returns>Devuelve el Canvas con el cambio de Coordenada realizado.</returns>
        public Canvas ChangeXCoord(double x)
        {
            if (canvasModel.graphOption != 1)
            {
                canvasModel.x_value = x;
                return UpdateCanvas();
            }
            return canvasObject;
        }

        /// <summary>
        /// Función que cambia directamente el valor de y_value por parámetro.
        /// </summary>
        /// <param name="y">Valor que se va a asignar a y_value.</param>
        /// <returns>Devuelve el Canvas con el cambio de Coordenada realizado.</returns>
        public Canvas ChangeYCoord(double y)
        {
            if (canvasModel.graphOption != 1)
            {
                canvasModel.y_value = y;
                return UpdateCanvas();
            }
            return canvasObject;
        }

        /*
         *      ===================================================
         *                        MOVIMIENTO EJES
         *      ===================================================
         */

        // FUNCIONES PÚBLICAS 

        /// <summary>
        /// Función que Sube la posición de la vista sobre el canvas.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con el vista desplazada hacia arriba.
        /// </return>
        public Canvas UpGraph()
        {
            if (canvasModel.graphOption != 1)
            {
                UpValues();
                mainWindow.UpdateYCoordValue(canvasModel.y_value);
                return UpdateCanvas();
            }
            return canvasObject;
        }

        /// <summary>
        /// Función que baja la posición de la vista sobre el canvas.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con el vista desplazada hacia abajo.
        /// </return>
        public Canvas DownGraph()
        {
            if (canvasModel.graphOption != 1)
            {
                DownValues();
                mainWindow.UpdateYCoordValue(canvasModel.y_value);
                return UpdateCanvas();
            }
            return canvasObject;
        }

        /// <summary>
        /// Función que desplaza a la izquierda la posición de la vista sobre el canvas.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con el vista desplazada hacia la izquierda.
        /// </return>
        public Canvas LeftGraph()
        {
            if (canvasModel.graphOption != 1)
            {
                LeftValues();
                mainWindow.UpdateXCoordValue(canvasModel.x_value);
                return UpdateCanvas();
            }
            return canvasObject;
        }

        /// <summary>
        /// Función que desplaza a la derecha la posición de la vista sobre el canvas.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas con el vista desplazada hacia la derecha.
        /// </return>
        public Canvas RightGraph()
        {
            if (canvasModel.graphOption != 1)
            {
                RightValues();
                mainWindow.UpdateXCoordValue(canvasModel.x_value);
                return UpdateCanvas();
            }
            return canvasObject;

        }


        // FUNCIONES PRIVADAS

        /// <summary>
        /// Función que aumenta el valor de y_value.
        /// </summary>
        private void UpValues()
        {
            if (canvasModel.Zoom <= -6 && canvasModel.Zoom > -30) canvasModel.y_value += 2;
            else if (canvasModel.Zoom <= -30) canvasModel.y_value += 10;
            else canvasModel.y_value += 1;
        }

        /// <summary>
        /// Función que disminuye el valor de y_value.
        /// </summary>
        private void DownValues()
        {
            if (canvasModel.Zoom <= -6 && canvasModel.Zoom > -30) canvasModel.y_value -= 2;
            else if (canvasModel.Zoom <= -30) canvasModel.y_value -= 10;
            else canvasModel.y_value -= 1;
        }

        /// <summary>
        /// Función que disminuye el valor de x_value.
        /// </summary>
        private void LeftValues()
        {
            if (canvasModel.Zoom <= -6 && canvasModel.Zoom > -30) canvasModel.x_value -= 2;
            else if (canvasModel.Zoom <= -30) canvasModel.x_value -= 10;
            else canvasModel.x_value -= 1;
        }

        /// <summary>
        /// Función que aumenta el valor de x_value.
        /// </summary>
        private void RightValues()
        {
            if (canvasModel.Zoom <= -6 && canvasModel.Zoom > -30) canvasModel.x_value += 2;
            else if (canvasModel.Zoom <= -30) canvasModel.x_value += 10;
            else canvasModel.x_value += 1;
        }

        /// <summary>
        /// Establece si se tienen o no que representar los ejes de coordenadas.
        /// </summary>
        /// <param name="set">True si se tienen que mostrar los ejes; False en caso contrario.</param>
        public void SetRenderAxis(bool set)
        {
            canvasFunctions.renderAxis = set;
            UpdateCanvas();
        }

        /// <summary>
        /// Establece si se tiene que representar o no la cuadrícula.
        /// </summary>
        /// <param name="set">True si se tienen que mostrar la cuadrícula; False en caso contrario.</param>
        public void SetRenderGrid(bool set)
        {
            canvasFunctions.renderGrid = set;
            UpdateCanvas();
        }

        /*
         *      ===================================================
         *                      ACTUALIZAR CANVAS
         *      ===================================================
         */

        /// <summary>
        /// Función pública que actualiza el canvas.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas actualizado.
        /// </return>
        public Canvas UpdateScreen()
        {
            return UpdateCanvas();
        }

        /// <summary>
        /// Función interna que actualiza el canvas.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas actualizado.
        /// </return>
        private Canvas UpdateCanvas(bool hasToChangeGrid = false)
        {
            if (canvasModel.graphOption != 1)
            {
                CleanCanvas(); 

                canvasObject = canvasFunctions.GridAndAxisDrawing(
                    canvasModel, canvasObject,
                    canvasModel.wxmin_initial + canvasModel.x_value + canvasModel.Zoom,
                    canvasModel.wxmax_initial + canvasModel.x_value - canvasModel.Zoom,
                    canvasModel.wymin_initial + canvasModel.y_value + canvasModel.Zoom,
                    canvasModel.wymax_initial + canvasModel.y_value - canvasModel.Zoom,
                    canvasModel.xstep_initial,
                    canvasModel.ystep_initial,
                    hasToChangeGrid);
                if (mainWindow.currentDc != null)
                {
                    switch (canvasModel.graphOption)
                    {
                        case 0:
                            return DrawPolyline(mainWindow.currentDc);
                        case 2:
                            return DrawScatter(mainWindow.currentDc);
                        case 3:
                            return DrawBarChart(mainWindow.currentDc);
                        default:
                            return canvasObject;
                    }
                }
                else
                    return canvasObject;
            }
            else
            {
                canvasObject.Children.Clear();
                if (mainWindow.currentDc != null)
                    return DrawPointsBarChart(mainWindow.currentDc);
                else
                    return canvasObject;
            }

        }



        /*
         *      ===================================================
         *                       DIBUJAR GRÁFICAS
         *      ===================================================
         */

        /// <summary>
        /// función pública que es llamada cuando se quiere dibujar un gráfico concreto.
        /// </summary>
        /// <return>
        /// Devuelve el Canvas actualizado.
        /// </return>
        public Canvas DrawGraph(DataCollection dc)
        {
            switch (canvasModel.graphOption)
            {
                case 0:
                    return DrawPolyline(dc);
                case 1:
                    return DrawPointsBarChart(dc);
                case 2:
                    return DrawScatter(dc);
                case 3:
                    return DrawBarChart(dc);
                default:
                    return canvasObject;
            }
        }
        /// <summary>
        /// Función que se encarga de dibujar una polilínea.
        /// </summary>
        /// <param name="dc">DataCollection a Representar.</param>
        /// <return>
        /// Devuelve el Canvas con la polilínea dibujada.
        /// </return>
        private Canvas DrawPolyline(DataCollection dc)
        {
            canvasObject = canvasFunctions.Polyline(canvasObject, canvasModel, dc);
            return canvasObject;
        }

        /// <summary>
        /// Función que se encarga de dibujar un gráfico de barras.
        /// </summary>
        /// <param name="dc">DataCollection a Representar.</param>
        /// <returns>
        /// Devuelve el Canvas con la polilínea dibujada.
        /// </returns>
        private Canvas DrawBarChart(DataCollection dc)
        {
            canvasObject = canvasFunctions.Bars(canvasObject, canvasModel, dc);
            return canvasObject;
        }

        /// <summary>
        /// -función que se encarga de dibujar un gráfico de barras del recuento de puntos
        /// </summary>
        /// <param name="dc">DataCollection a Representar.</param>
        /// <return>
        /// Devuelve el Canvas con la gráfica de barras dibujada
        /// </return>

        private Canvas DrawPointsBarChart(DataCollection dc)
        {
            if (dc.data.Count > 2)
            {
                CleanCanvas();
                canvasObject = canvasFunctions.BarChart(canvasObject, canvasModel, dc);

            }
            else
            {
                MessageBox.Show("No hay datos suficientes para una gráfica de barras.", "Advertencia");
                CleanCanvas();
                canvasModel.graphOption = 0;
                UpdateCanvas();
            }
            return canvasObject;
        }

        /// <summary>
        /// Función que dibuja un gráfico de puntos.
        /// </summary>
        /// <param name="dc">DataCollection a Representar.</param>
        /// <return>
        /// Devuelve el Canvas con la gráfica de barras dibujada
        /// </return>
        private Canvas DrawScatter(DataCollection dc)
        {
            canvasObject = canvasFunctions.Scatter(canvasObject, canvasModel, dc);
            return canvasObject;
        }

        /// <summary>
        /// Limpia el canvas manteniendo el cuadro de selección.
        /// </summary>
        private void CleanCanvas()
        {
            selection = mainWindow.selectionBox;
            canvasObject.Children.Clear();
            canvasObject.Children.Add(selection);
        }

    }
}
