using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeoGebra
{
    /// <summary>
    /// Contiene funciones utilizadas para la creación de los elementos visuales del Canvas.
    /// </summary>
    class CanvasFunctions
    {
        /*
         *  Variables que sirven para no recalcular, en caso de que no varíen
         *  las líneas de los ejes o la polilínea a representar.
         */

        /// <summary>
        /// Guarda el Path de las línea Horizontales de la cuadrícula para no recalcularla.
        /// </summary>
        private Path x_grid_path_const;

        /// <summary>
        /// Guarda el Path de las línea verticales de la cuadrícula para no recalcularla.
        /// </summary>
        private Path y_grid_path_const;

        /// <summary>
        /// Guarda la Polylínea actual para no recalcularla.
        /// </summary>
        private Polyline currentGraphLine;


        /// <summary>
        /// Guarda los rectángulos dibujados en la gráfica de barras para borrarlos.
        /// </summary>
        private List<Rectangle> rectangles;

        /// <summary>
        /// Guarda los círculos de la gráfica de barras para borrarlos.
        /// </summary>
        private List<Ellipse> ellipses;


        /// <summary>
        /// Indican si se tienen o no que representar los ejes.
        /// </summary>
        public bool renderAxis = true;

        /// <summary>
        /// Indican si se tiene o no que representar la cuadrícula.
        /// </summary>
        public bool renderGrid = true;
        /*
         * ================================================================
         *               Funciones de creación de cuadrícula
         * ================================================================
         */

        /// <summary>
        /// Crea una línea vertical en el punto x indicado desde wymin a wymax.
        /// </summary>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="wymin">Variable wymin. (Coordenada Y mínima del Canvas).</param>
        /// <param name="wymax">Variable wymax. (Coordenada Y máxima del Canvas).</param>
        /// <param name="x">Valor de X donde se va a representar la línea vertical.</param>
        /// <returns>
        /// Devuelve una LineGeometry.
        /// </returns>
        private LineGeometry YLine(CanvasModel cm, double wymin, double wymax, double x)
        {
            return new LineGeometry(cm.WtoD(new Point(x, wymin)), cm.WtoD(new Point(x, wymax)));
        }

        /// <summary>
        /// Crea una línea horizontal en el punto y indicado desde wymin a wymax.
        /// </summary>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="wxmin">Variable wxmin. (Coordenada X mínima del Canvas).</param>
        /// <param name="wxmax">Variable wxmax. (Coordenada X máxima del Canvas).</param>
        /// <param name="y">Valor de Y donde se va a representar la línea horizontal.</param>
        /// <returns>
        /// Devuelve una LineGeometry.
        /// </returns>
        private LineGeometry XLine(CanvasModel cm, double wxmin, double wxmax, double y)
        {
            return new LineGeometry(cm.WtoD(new Point(wxmin, y)), cm.WtoD(new Point(wxmax, y)));
        }


        /// <summary>
        /// Crea un conjunto de líneas verticales desde wxmin hasta wxmax, con altura desde wymin a wymax.
        /// </summary>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="wymin">Variable wymin. (Coordenada Y mínima del Canvas).</param>
        /// <param name="wymax">Variable wymax. (Coordenada Y máxima del Canvas).</param>
        /// <param name="wxmin">Variable wxmin. (Coordenada X mínima del Canvas).</param>
        /// <param name="wxmax">Variable wxmax. (Coordenada X máxima del Canvas).</param>
        /// <param name="xstep">Distancia entre líneas (Calculada en la función DrawGridAndAxis).</param>
        /// <returns>
        /// Devuelve un GeometryGroup del conjunto de líneas.
        /// </returns>
        private GeometryGroup YGrid(CanvasModel cm, double wymin, double wymax, double wxmin, double wxmax, double xstep)
        {
            GeometryGroup x_grid = new GeometryGroup();
            for (double x = wxmin; x <= wxmax - xstep + 1; x += xstep)
            {
                // Draw the y Grid values
                x_grid.Children.Add(YLine(cm, wymin, wymax, x));

            }
            return x_grid;
        }

        /// <summary>
        /// Crea un conjunto de líneas horizontales desde wymin hasta wymax, con longitud desde wxmin a wxmax.
        /// </summary>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="wymin">Variable wymin. (Coordenada Y mínima del Canvas).</param>
        /// <param name="wymax">Variable wymax. (Coordenada Y máxima del Canvas).</param>
        /// <param name="wxmin">Variable wxmin. (Coordenada X mínima del Canvas).</param>
        /// <param name="wxmax">Variable wxmax. (Coordenada X máxima del Canvas).</param>
        /// <param name="ystep">Distancia entre líneas (Calculada en la función DrawGridAndAxis).</param>
        /// <returns>
        /// Devuelve un GeometryGroup del conjunto de líneas.
        /// </returns>
        private GeometryGroup XGrid(CanvasModel cm, double wymin, double wymax, double wxmin, double wxmax, double ystep)
        {
            GeometryGroup y_grid = new GeometryGroup();
            for (double y = wymin; y <= wymax - ystep + 1; y += ystep)
            {
                // Draw the y Grid values
                y_grid.Children.Add(XLine(cm, wxmin, wxmax, y));
            }
            return y_grid;
        }


        /*
         * ================================================================
         *                 Funciones de creación de barras
         * ================================================================
         */

        /// <summary>
        /// Dibuja un rectángulo en el canvas indicado por argumento de unas dimensiones width x height, 
        /// con la esquina inferior izquierda en x, y, y del color indicado.
        /// </summary>
        /// <param name="cv">Canvas.</param>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="width">Ancho del rectángulo a representar.</param>
        /// <param name="height">Altura del rectángulo a representar.</param>
        /// <param name="color">Cadena con el color del rectángulo.</param>
        /// <param name="x">Coordenada X de la esquina izquierda del rectángulo.</param>
        /// <param name="y">Coordenada Y de la esquina del rectángulo.</param>
        /// <param name="bottom">Indica si la coordenada Y indica la esquina inferior, o la esquina superior.</param>
        /// <returns>
        /// Devuelve el Canvas con el rectángulo añadido.
        /// </returns>
        private Canvas DrawRectangle(Canvas cv, CanvasModel cm, double width, double height, string color, double x, double y, bool bottom = true)
        {
            Canvas cv_c = cv; 

            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            Rectangle rectangle = new Rectangle
            {
                Width = width,
                Height = Math.Abs(height),
                Stroke = brush,
                Fill = brush,
                StrokeThickness = 2
            };
            Point pt = cm.WtoD(new Point(x, y));
            Canvas.SetLeft(rectangle, pt.X);
            if (bottom)
                Canvas.SetBottom(rectangle, pt.Y);
            else
                Canvas.SetTop(rectangle, pt.Y);
            cv_c.Children.Add(rectangle);
            if (rectangles != null) rectangles.Add(rectangle);
            return cv_c;
        }

        /// <summary>
        /// Devuelve el nombre de un color indicándole un número por parámetro.
        /// Se usa para asignar un color distinto a cada rectángulo.
        /// </summary>
        /// <param name="i">Entero positivo para indicar un color.</param>
        /// <returns>
        /// Devuelve una cadena con el color asignado al número i.
        /// </returns>
        private string ColorQuery(int i)
        {
            List<string> colorList = new List<string> { "Red", "Yellow", "Orange", "Green", "Blue", "SkyBlue", "Pink", "Wheat", "Indigo", "Crimson" };
            return colorList[i];
        }

        /*
         * ================================================================
         *                 Funciones para representar texto
         * ================================================================
         */

        /// <summary>
        /// Escribe texto en el Canvas.
        /// </summary>
        /// <param name="cv">Canvas donde se escribirá el texto.</param>
        /// <param name="text">Texto a escribir.</param>
        /// <param name="location">Localización donde se va a escribir el texto.</param>
        /// <param name="font_size">Tamaño de la fuente.</param>
        /// <param name="halign">Alineamiento horizontal.</param>
        /// <param name="valign">Alineamiento vertical.</param>
        /// <returns>
        /// Devuelve el Canvas pasado por argumento con el texto representado.
        /// </returns>
        private Canvas DrawText(Canvas cv, string text, Point location, 
            double font_size, HorizontalAlignment halign, VerticalAlignment valign)
        {
            // Crea la etiqueta.
            Label label = new Label();
            label.Content = text;
            label.FontSize = font_size;
            cv.Children.Add(label);

            // Posición de la etiqueta.
            label.Measure(new Size(double.MaxValue, double.MaxValue));

            double x = location.X;
            if (halign == HorizontalAlignment.Center)
                x -= label.DesiredSize.Width / 2;
            else if (halign == HorizontalAlignment.Right)
                x -= label.DesiredSize.Width;
            Canvas.SetLeft(label, x);

            double y = location.Y;
            if (valign == VerticalAlignment.Center)
                y -= label.DesiredSize.Height / 2;
            else if (valign == VerticalAlignment.Bottom)
                y -= label.DesiredSize.Height;
            Canvas.SetTop(label, y);
            return cv;
        }

        /*
         * ================================================================
         *             Función para crear una gráfica de barras
         * ================================================================
         */

        /// <summary>
        /// Dibuja un gráfico de barras del número de veces que se repiten los puntos de la tabla.
        /// </summary>
        /// <param name="cv">Canvas a utilizar.</param>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="dc">DataCollection a representar.</param>
        /// <returns>
        /// Devuelve el Canvas pasado por argumento con la gráfica de barras representada.
        /// </returns>
        public Canvas BarChart(Canvas cv, CanvasModel cm, DataCollection dc)
        {

            double wxmin = cm.wxmin_initial;
            double wxmax = cm.wxmax_initial;
            double wymin = cm.wymin_initial;
            double wymax = cm.wymax_initial;
            double xstep = cm.wxmin_initial + 2;
            double ystep = cm.wymin_initial + 4;

            BarChartCalculator bd = new BarChartCalculator(dc);
            float maxNumberOfValues = bd.maxNumberOfValues;
            int numberOfBars = bd.numberOfBars;
            if (numberOfBars > 9)
            {
                MessageBoxResult result = MessageBox.Show("Solo se pueden representar 9 barras.", "Confirmation");
                return cv;
            }

            cm.PrepareTransformations(
                wxmin, wxmax, wymax, wymin);

            // Si no hay puntos que dibujar, se dejará en blanco
            // En caso contrario

            // Se dibujarán los ejes
            Point p0 = cm.DtoW(new Point(-2, -2));
            Point p1 = cm.DtoW(new Point(2, 2));
            double xtic = p1.X - p0.X;
            double ytic = p1.Y - p0.Y;

            /*
             *  Parte encargada de dibujar la barra lateral izquierda de la gráfica de barras
             */
            GeometryGroup yaxis_geom = new GeometryGroup();
            p0 = new Point(wxmin + 2, wymin + 2);
            p1 = new Point(wxmin + 2, wymax - 1);
            yaxis_geom.Children.Add(new LineGeometry(cm.WtoD(p0), cm.WtoD(p1)));

            for (float i = 0; i <= maxNumberOfValues + 0.001; i += maxNumberOfValues / 10)
            {
                Point tic0 = cm.WtoD(new Point(wxmin + 2 - xtic, xstep));
                Point tic1 = cm.WtoD(new Point(wxmin + 2 + xtic, xstep));
                yaxis_geom.Children.Add(new LineGeometry(tic0, tic1));
                p0 = new Point(wxmin + 2, xstep);
                p1 = new Point(wxmax - 1, xstep);
                yaxis_geom.Children.Add(new LineGeometry(cm.WtoD(p0), cm.WtoD(p1)));

                // Etiqueta de la coordenada Y de la marca.
                cv = DrawText(cv, Math.Round(i, 2).ToString(),
                    new Point(tic0.X - 10, tic0.Y), 12,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);

                xstep += ((wymax - 1) - (wymin + 4)) / 10;
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;
            cv.Children.Add(yaxis_path);

            /*
             *  Parte encargada de dibujar la barra inferior de la gráfica de barras
             */
            GeometryGroup xaxis_geom = new GeometryGroup();
            p0 = new Point(wxmin + 2, wymin + 2);
            p1 = new Point(wxmax - 1, wymin + 2);
            xaxis_geom.Children.Add(new LineGeometry(cm.WtoD(p0), cm.WtoD(p1)));

            double val1 = ((wxmax - 4) - (wxmin + 4)) / (numberOfBars - 1);
            //double width = ConvertXToCanvasForBarGraph(val1, wxmin, wxmax) - ConvertXToCanvasForBarGraph(4, wxmin, wxmax);
            //double height = ConvertYToCanvasForBarGraph(1, wymin, wymax) - ConvertYToCanvasForBarGraph(0, wymin, wymax);

            for (int i = 0; i < numberOfBars; i += 1)
            {
                Point tic0 = cm.WtoD(new Point(ystep, wymin + 1 - ytic));

                // Etiqueta de la coordenada Y de la marca.
                cv = DrawText(cv, "(" + bd.lst[i].pt.x.ToString() + "," + bd.lst[i].pt.y.ToString() + ")",
                    new Point(tic0.X, tic0.Y - 10), 12,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);


                cv = DrawRectangle(cv, cm, cm.ConvertXToCanvas(val1,wxmin, wxmax) / 10, 41.025 * (bd.lst[i].ocurrences / (maxNumberOfValues / 10)), ColorQuery(i), ystep - val1 / 10, wymax - 2.005);
                //cv = DrawRectangle(cv, cm, width, height * bd.lst[i].ocurrences / (maxNumberOfValues / 10), ColorQuery(i), ystep - val1 / 10, wymax - 2.005);
                ystep += ((wxmax - 4) - (wxmin + 4)) / (numberOfBars - 1);
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;
            cv.Children.Add(xaxis_path);
            return cv;
        }

        /*
         * ================================================================
         *     Función para crear una gráfica polilínea y de puntos
         * ================================================================
         */

        /// <summary>
        /// Dibuja los ejes de coordenadas y la cuadrícula del canvas.
        /// </summary>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="cv">Canvas a utilizar.</param>
        /// <param name="wxmin_args">Valor para wxmin de la función. (Mínima Coordenada x del Canvas). </param>
        /// <param name="wxmax_args">Valor para wxmax de la función. (Máxima Coordenada x del Canvas). </param>
        /// <param name="wymin_args">Valor para wymin de la función. (Mínima Coordenada y del Canvas). </param>
        /// <param name="wymax_args">Valor para wymax de la función. (Máxima Coordenada y del Canvas). </param>
        /// <param name="xstep_args">Valor para xstep de la función. (Distancia entre las líneas del eje x). </param>
        /// <param name="ystep_args">Valor para ystep de la función. (Distancia entre las líneas del eje y).</param>
        /// <param name="hasToChangeGrid">Indica si se tiene o no que actualizar el Grid. </param>
        /// <returns>
        /// Devuelve el Canvas pasado por argumento con los ejes y la cuadrícula dibujadas.
        /// </returns>
        public Canvas GridAndAxisDrawing(CanvasModel cm, Canvas cv, 
            double wxmin_args, double wxmax_args, 
            double wymin_args, double wymax_args, 
            double xstep_args, double ystep_args, 
            bool hasToChangeGrid = false)
        {

            double _wxmin = wxmin_args;
            double _wxmax = wxmax_args;
            double _wymin = wymin_args;
            double _wymax = wymax_args;
            double _xstep = xstep_args;
            double _ystep = ystep_args;

            // Prepara las matrices de transformación para el Canvas.

            cm.PrepareTransformations(_wxmin, _wxmax, _wymax, _wymin);

            // Factores de escalado de x_step e y_step en función del tamaño del Zoom

            double _xsize = (_wxmax - _wxmin) / 2;
            double _ysize = (_wymax - _wymin) / 2;

            if (_xsize <= 6.0) { _xstep /= 2; }
            if (_ysize <= 6.0) { _ystep /= 2; }
            if (_xsize >= 16.0) { _xstep *= 2; }
            if (_ysize >= 16.0) { _ystep *= 2; }
            if (_xsize >= 40.0) { _xstep *= 5; }
            if (_ysize >= 40.0) { _ystep *= 5; }

            // Marcas de los ejes.
            Point p0 = cm.DtoW(new Point(-2, -2));
            Point p1 = cm.DtoW(new Point(2, 2));
            double xtic = p1.X - p0.X;
            double ytic = p1.Y - p0.Y;

            // Greometry Groups para el eje X
            GeometryGroup xaxis_geom = new GeometryGroup();
            GeometryGroup x_grid = new GeometryGroup();

            // Greometry Groups para el eje Y
            GeometryGroup yaxis_geom = new GeometryGroup();
            GeometryGroup y_grid = new GeometryGroup();

            // Si no tiene que actualizar ni los ejes ni la cuadrícula, devolvemos el canvas
            if (!renderAxis && !renderGrid) return cv;

            // En caso de tener que actualizar solo la cuadrícula.
            else if (!renderAxis && renderGrid)
            {
                // Si no está creado todavía las líneas horizontales de la cuadrícula || se tiene que actualizar el Grid
                if (x_grid_path_const == null || hasToChangeGrid)
                {
                    x_grid = YGrid(cm, _wymin, _wymax, _wxmin, _wxmax, _xstep);
                    Path x_grid_path = new Path();
                    x_grid_path.StrokeThickness = 1;
                    x_grid_path.Stroke = Brushes.LightGray;
                    x_grid_path.Data = x_grid;
                    x_grid_path_const = x_grid_path;
                    cv.Children.Add(x_grid_path);
                }
                // En caso contrario, añadimos directamente las líneas horizontales al Canvas
                else
                {
                    cv.Children.Add(x_grid_path_const);
                }

                // Si no está creado todavía las líneas verticales de la cuadrícula || se tiene que actualizar el Grid
                if (y_grid_path_const == null || hasToChangeGrid)
                {
                    y_grid = XGrid(cm, _wymin, _wymax, _wxmin, _wxmax, _ystep);
                    Path y_grid_path = new Path();
                    y_grid_path.StrokeThickness = 1;
                    y_grid_path.Stroke = Brushes.LightGray;
                    y_grid_path.Data = y_grid;
                    y_grid_path_const = y_grid_path;
                    cv.Children.Add(y_grid_path);
                }
                // En caso contrario, añadimos directamente las líneas verticales al Canvas
                else
                {
                    cv.Children.Add(y_grid_path_const);
                }
                return cv;
            }

            // En caso de tener que actualizar los Ejes
            else
            {
                //
                //                           EJE X
                // 
                // Si el eje x está en dentro del campo de visión del canvas.
                if (_wymin < 0.0 && _wymax > 0.0)
                {
                    // Crea la línea del eje.
                    p0 = new Point(_wxmin, 0);
                    p1 = new Point(_wxmax, 0);
                    xaxis_geom.Children.Add(new LineGeometry(cm.WtoD(p0), cm.WtoD(p1)));

                    for (double x = _wxmin; x <= _wxmax - _xstep + 1; x += _xstep)
                    {
                        // Añade las marcas del eje X.
                        Point tic0 = cm.WtoD(new Point(x, -ytic));
                        Point tic1 = cm.WtoD(new Point(x, ytic));
                        xaxis_geom.Children.Add(new LineGeometry(tic0, tic1));

                        if ((x_grid_path_const == null || hasToChangeGrid) && renderGrid) x_grid.Children.Add(YLine(cm, _wymin, _wymax, x));

                        // Escribe los números de las marcas del eje X.
                        DrawText(cv, x.ToString(),
                            new Point(tic0.X, tic0.Y + 5), 12,
                            HorizontalAlignment.Center,
                            VerticalAlignment.Top);
                    }
                }
                // En caso contrario, solo añadimos las líneas de la cuadrícula (Si se tienen que crear).
                else
                {
                    if ((x_grid_path_const == null || hasToChangeGrid) && renderGrid) x_grid = YGrid(cm, _wymin, _wymax, _wxmin, _wxmax, _xstep);
                }

                // Añade al Canvas el eje X .
                Path xaxis_path = new Path();
                xaxis_path.StrokeThickness = 1;
                xaxis_path.Stroke = Brushes.Black;
                xaxis_path.Data = xaxis_geom;
                cv.Children.Add(xaxis_path);

                // Si se tiene que añadir la cuadrícula.
                if (renderGrid)
                {
                    // Si no se ha creado la cuadrícula || tiene que cambiar.
                    if (x_grid_path_const == null || hasToChangeGrid)
                    {
                        Path x_grid_path = new Path();
                        x_grid_path.StrokeThickness = 1;
                        x_grid_path.Stroke = Brushes.LightGray;
                        x_grid_path.Data = x_grid;
                        x_grid_path_const = x_grid_path;
                        cv.Children.Add(x_grid_path);
                    }
                    // En caso contrario, se añade la copia guardada.
                    else
                    {
                        cv.Children.Add(x_grid_path_const);
                    }
                }

                //
                //                          EJE Y
                // 
                // Si el eje y está en dentro del campo de visión del canvas.
                if (_wxmin < 0.0 && _wxmax > 0.0)
                {
                    // Crea la línea del eje.
                    p0 = new Point(0, _wymin);
                    p1 = new Point(0, _wymax);
                    yaxis_geom.Children.Add(new LineGeometry(cm.WtoD(p0), cm.WtoD(p1)));

                    for (double y = _wymin; y <= _wymax - _ystep + 1; y += _ystep)
                    {
                        // Añade las marcas del eje
                        Point tic0 = cm.WtoD(new Point(-xtic, y));
                        Point tic1 = cm.WtoD(new Point(xtic, y));
                        yaxis_geom.Children.Add(new LineGeometry(tic0, tic1));

                        // Si se tiene que renderizar el Grid y, o cambia, o no ha sido generada anteriormente.
                        if ((y_grid_path_const == null || hasToChangeGrid) && renderGrid) y_grid.Children.Add(XLine(cm, _wxmin, _wxmax, y));

                        // Escribe el número en las marcas del eje.
                        DrawText(cv, y.ToString(),
                            new Point(tic0.X - 10, tic0.Y), 12,
                            HorizontalAlignment.Center,
                            VerticalAlignment.Center);
                    }
                }
                else
                {
                    // En caso contrario, solo se encarga del Grid.
                    if ((y_grid_path_const == null || hasToChangeGrid) && renderGrid) y_grid = XGrid(cm, _wymin, _wymax, _wxmin, _wxmax, _ystep);
                }

                Path yaxis_path = new Path();
                yaxis_path.StrokeThickness = 1;
                yaxis_path.Stroke = Brushes.Black;
                yaxis_path.Data = yaxis_geom;
                cv.Children.Add(yaxis_path);

                // Si se tiene que representar la cuadrícula
                if (renderGrid)
                {
                    // si no se ha generado la cuadrícula anteriormente, o tiene que cambiar la cuadrícula.
                    if (y_grid_path_const == null || hasToChangeGrid)
                    {
                        Path y_grid_path = new Path();
                        y_grid_path.StrokeThickness = 1;
                        y_grid_path.Stroke = Brushes.LightGray;
                        y_grid_path.Data = y_grid;
                        y_grid_path_const = y_grid_path;
                        cv.Children.Add(y_grid_path);
                    }
                    // En caso contrario, añade la copia guardada anteriormente.
                    else
                    {
                        cv.Children.Add(y_grid_path_const);
                    }
                }

                return cv;
            }
        }

        /// <summary>
        /// Dibuja una polilínea con los puntos de la DataCollection
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="cm"></param>
        /// <param name="dc"></param>
        /// <returns>
        /// Devuelve el Canvas pasado por argumento con la polilínea ya dibujada
        /// </returns>
        public Canvas Polyline(Canvas cv, CanvasModel cm, DataCollection dc)
        {
            // Elimina la Polilínea anterior.
            cv.Children.Remove(currentGraphLine);

            // Cuenta el número de puntos a representar.
            int numpuntos = dc.data.Count();

            // Prepara las wx.
            double wxmin = cm.wxmin_initial + cm.x_value + cm.Zoom;
            double wxmax = cm.wxmax_initial + cm.x_value - cm.Zoom;
            double wymin = cm.wymin_initial + cm.y_value + cm.Zoom;
            double wymax = cm.wymax_initial + cm.y_value - cm.Zoom;

            // Prepara la matriz de transformaciones.
            cm.PrepareTransformations(wxmin, wxmax, wymax, wymin);

            // Si hay puntos
            if (numpuntos > 0)
            {
                // Crea la polillínea
                Polyline polilinea = new Polyline();
                polilinea.Stroke = dc.color;
                Point[] puntos = new Point[numpuntos];
                float value = 0;
                for (int i = 0; i < numpuntos; i++)
                {
                    if (dc.data[i].y != double.NaN)
                    {
                        if (dc.data[i].y.ToString().Equals("-∞")) dc.data[i].y = -10000000;
                        else if (dc.data[i].y.ToString().Equals("∞")) dc.data[i].y = 10000000;
                        puntos[i] = cm.WtoD(new Point(dc.data[i].x, dc.data[i].y));
                    }
                }
                polilinea.Points = new PointCollection(puntos);

                polilinea.Stroke = dc.color;
                cv.Children.Add(polilinea);
                currentGraphLine = polilinea;
            }
            return cv;
        }

        /// <summary>
        /// Dibuja un gráfica de puntos, con los puntos de la DataCollection
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="cm"></param>
        /// <param name="dc"></param>
        /// <returns>
        /// Devuelve el Canvas actualizado.
        /// </returns>
        public Canvas Scatter(Canvas cv, CanvasModel cm, DataCollection dc)
        {
            // Elimina la gráfica anterior.
            cv.Children.Remove(currentGraphLine);

            // Cuenta el número de puntos 
            int numpuntos = dc.data.Count();

            // Prepara las wx.
            double wxmin = cm.wxmin_initial + cm.x_value + cm.Zoom;
            double wxmax = cm.wxmax_initial + cm.x_value - cm.Zoom;
            double wymin = cm.wymin_initial + cm.y_value + cm.Zoom;
            double wymax = cm.wymax_initial + cm.y_value - cm.Zoom;

            // Prepara la matriz de transformaciones.
            cm.PrepareTransformations(wxmin, wxmax, wymax, wymin);

            // Si hay puntos que representar
            if (numpuntos > 0)
            {
                for (int i = 0; i < numpuntos; i++)
                {
                    // Si el punto no es NaN
                    if (dc.data[i].y != double.NaN)
                    {
                        if (dc.data[i].y.ToString().Equals("-∞")) dc.data[i].y = -10000000;
                        else if (dc.data[i].y.ToString().Equals("∞")) dc.data[i].y = 10000000;
                        // Creamos un círculo
                        Ellipse ellipse = new Ellipse();
                        ellipse.Stroke = dc.color;
                        ellipse.Fill = dc.color;
                        ellipse.Width = 7;
                        ellipse.Height = 7;

                        ellipse.Margin = new Thickness(
                            cm.WtoD(new Point(dc.data[i].x, dc.data[i].y)).X, 
                            cm.WtoD(new Point(dc.data[i].x, dc.data[i].y)).Y, 
                            0, 0);
                        cv.Children.Add(ellipse);
                    }

                }
            }
            return cv;
        }
    
        /// <summary>
        /// Dibuja una gráfica de barras con los valores de los puntos.
        /// </summary>
        /// <param name="cv">Canvas a utilizar.</param>
        /// <param name="cm">CanvasModel del Canvas.</param>
        /// <param name="dc">DataCollection a representar.</param>
        /// <returns>
        /// Devuelve el Canvas actualizado.
        /// </returns>
        public Canvas Bars(Canvas cv, CanvasModel cm, DataCollection dc)
        {
            // Elimina la gráfica anterior
            cv.Children.Remove(currentGraphLine);
            // Elimina los rectángulos anteriores
            if (rectangles != null && rectangles.Count() > 0)
            {
                foreach (Rectangle r in rectangles)
                {
                    try
                    {
                        cv.Children.Remove(r);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            rectangles = new List<Rectangle>();
            // Elimina los círculos anteriores
            if (ellipses != null && ellipses.Count() > 0)
            {
                foreach (Ellipse e in ellipses)
                {
                    try
                    {
                        cv.Children.Remove(e);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            ellipses = new List<Ellipse>();

            // Elimina los duplicados
            List<Punto> pts_ = dc.data.Distinct().ToList();

            // Cuenta el número de puntos
            int numpuntos = pts_.Count();

            // Prepara las wx
            double wxmin = cm.wxmin_initial + cm.x_value + cm.Zoom;
            double wxmax = cm.wxmax_initial + cm.x_value - cm.Zoom;
            double wymin = cm.wymin_initial + cm.y_value + cm.Zoom;
            double wymax = cm.wymax_initial + cm.y_value - cm.Zoom;

            // Si hay puntos
            if (numpuntos > 0)
            {
                /*
                 * Con esto calculamos el ancho mínimo que tendrán que tener
                 * las barras para poder ser representadas
                 */
                List<Punto> pts = pts_.OrderBy(o => o.x).ToList();
                float minimum_distance;
                if (numpuntos <= 1) minimum_distance = 1;
                else if (numpuntos == 2) minimum_distance = pts[1].x - pts[0].x;
                else 
                {
                    minimum_distance = pts[1].x - pts[0].x;
                    for (int i = 2; i < numpuntos; i++)
                    {
                        if ((pts[i].x - pts[i - 1].x) < minimum_distance && pts[i].x - pts[i - 1].x != 0)
                        {
                            minimum_distance = pts[i].x - pts[i - 1].x;
                        }
                    }
                }

                // Calculamos la anchura que tendrá cada barra, y la unidad de altura (1) que se multiplicará por el valor del punto
                // para generar la altura del rectángulo
                double width = cm.ConvertXToCanvas(minimum_distance,wxmin, wxmax) - cm.ConvertXToCanvas(0, wxmin, wxmax);
                double height = cm.ConvertYToCanvas(1, wymin, wymax) - cm.ConvertYToCanvas(0, wymin, wymax);
                for (int i = 0; i < numpuntos; i++)
                {
                    // Si el punto no es NaN
                    if (pts_[i].y != double.NaN)
                    {
                        if (dc.data[i].y.ToString().Equals("-∞")) dc.data[i].y = -10000000;
                        else if (dc.data[i].y.ToString().Equals("∞")) dc.data[i].y = 10000000;
                        // Si el valor del punto es mayor a 0 la barra se representa hacia arriba.
                        if (pts_[i].y > 0)
                        {
                            cv = DrawRectangle(cv, cm, width, height * pts_[i].y, dc.color.Color.ToString(), pts_[i].x - minimum_distance/2, 0 + 2*cm.y_value, true);
                        }
                        // Si el valor del punto es meno a 0 la barra se representa hacia abajo.
                        else if (pts_[i].y <= 0)
                        {
                            cv = DrawRectangle(cv, cm, width, height * pts_[i].y, dc.color.Color.ToString(), pts_[i].x - minimum_distance / 2, 0.0, false);
                        }
                        // Se crea un círculo para indicar el valor del punto en el eje x
                        Ellipse ellipse = new Ellipse();
                        ellipse.Stroke = dc.color;
                        ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        ellipse.Width = 7;
                        ellipse.Height = 7;
                        ellipse.Margin = new Thickness(
                                cm.WtoD(new Point(pts_[i].x, pts_[i].y)).X,
                                cm.WtoD(new Point(0, 0)).Y,
                                0, 0);
                        ellipses.Add(ellipse);
                        cv.Children.Add(ellipse);
                    }
                }
                return cv;
            }
            else return cv;
        }
    }
}
