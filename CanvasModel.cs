using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GeoGebra
{
    /// <summary>
    /// Contiene los datos del Canvas de la ventana Principal, y que son
    /// utilizados por las Clases CanvasController y CanvasFunctions.
    /// </summary>
    public class CanvasModel
    {

        /*
         * ============================================
         *     CONSTANTES INICIALES DE LA APLICACIÓN
         * ============================================
         */

        /// <summary>
        /// Valor inicial de wxmin.
        /// </summary>
        public double wxmin_initial { get; }

        /// <summary>
        /// Valor inicial de wxmax. 
        /// </summary>
        public double wxmax_initial { get; }

        /// <summary>
        /// Valor inicial de wymin. 
        /// </summary>
        public double wymin_initial { get; }

        /// <summary>
        /// Valor inicial de wymax.
        /// </summary>
        public double wymax_initial { get; }

        /// <summary>
        /// Valor inicial de xstep.
        /// </summary>
        public double xstep_initial { get; }

        /// <summary>
        /// Valor inicial de ystep.
        /// </summary>
        public double ystep_initial { get; }

        /// <summary>
        /// Valor inicial de margen.
        /// </summary>
        public double dmargin { get; }

        /*
         * ==========================================
         *        VARIABLES DE LA APLICACIÓN
         * ==========================================
         */

        /// <summary>
        /// Desplazamiento en el eje x.
        /// </summary>
        public double x_value  { get; set; }

        /// <summary>
        /// Desplazamiento en el eje y.
        /// </summary>
        public double y_value  { get; set; }

        /// <summary>
        /// Escala de Zoom.
        /// </summary>
        public double Zoom     { get; set; }

        /// <summary>
        /// Coordenada del extremo izquierdo x.
        /// </summary>
        public double dxmin    { get; set; }

        /// <summary>
        /// Coordenada del extremo derecho x.
        /// </summary>
        public double dxmax    { get; set; }

        /// <summary>
        /// Coordenada del extremo inferior y.
        /// </summary>
        public double dymin    { get; set; }

        /// <summary>
        /// Coordenada del extremo superior y.
        /// </summary>
        public double dymax    { get; set; }

        /// <summary>
        /// Indica el tipo de gráfica que es.
        /// 0 -> Gráfica polilínea.
        /// 1 -> Gráfica de barras.
        /// 2 -> Gráfica de puntos.
        /// </summary>

        public int graphOption { get; set; }

        /// <summary>
        /// Matrices de transformación.
        /// </summary>
        public Matrix WtoDMatrix, DtoWMatrix;


        /* ==============================
        *             CONSTRUCTOR
        * ===============================
        */

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        /// <param name="width">Ancho del Canvas.</param>
        /// <param name="height">Altura del Canvas.</param>
        public CanvasModel(double width, double height)
        {

            // Inicializamos las constantes

            wxmin_initial = -10;
            wxmax_initial = 10;
            wymin_initial = -10;
            wymax_initial = 10;
            xstep_initial = 1;
            ystep_initial = 1;
            dmargin = 10;

            // Inicializamos las variables

            x_value = 0;
            y_value = 0;
            Zoom = 0;
            dxmin = dmargin;
            dxmax = width - dmargin;
            dymin = dmargin;
            dymax = height - dmargin;
            graphOption = 0;
        }

        /*
         * =================================
         *        Funciones de la matriz
         * =================================
         */

        /// <summary>
        /// Prepara las matrices de transformaciones.
        /// </summary>
        /// <param name="wxmin"> Valor wxmin del Canvas.</param>
        /// <param name="wxmax"> Valor wxmax del Canvas.</param>
        /// <param name="wymin"> Valor wymin del Canvas.</param>
        /// <param name="wymax"> Valor wymax del Canvas.</param>
        public void PrepareTransformations(
            double wxmin, double wxmax, 
            double wymin, double wymax)
        {
            // Make WtoD.
            WtoDMatrix = Matrix.Identity;
            WtoDMatrix.Translate(-wxmin, -wymin);

            double xscale = (dxmax - dxmin) / (wxmax - wxmin);
            double yscale = (dymax - dymin) / (wymax - wymin);
            WtoDMatrix.Scale(xscale, yscale);

            WtoDMatrix.Translate(dxmin, dymin);

            // Make DtoW.
            DtoWMatrix = WtoDMatrix;
            DtoWMatrix.Invert();
        }

        /// <summary>
        /// Transforma un punto de Coordenadas del Mundo a Coordenadas del Canvas.
        /// </summary>
        /// <returns>
        /// Devuelve un Punto en Coordenadas del Canvas.
        /// </returns>
        public Point WtoD(Point point)
        {
            return WtoDMatrix.Transform(point);
        }

        /// <summary>
        /// Transforma un punto de Coordenadas del Canvas a Coordenadas del Mundo.
        /// </summary>
        /// <returns>
        /// Devuelve un Punto en Coordenadas del Mundo.
        /// </returns>
        public Point DtoW(Point point)
        {
            return DtoWMatrix.Transform(point);
        }

        /// <summary>
        /// Convierte el valor de X de coordenadas del Canvas a coordenadas del Dispositivo.
        /// </summary>
        /// <param name="x">Valor de X a calcular</param>
        /// <param name="wxmin">Valor de wxmin.</param>
        /// <param name="wxmax">Valor de wxmax.</param>
        /// <returns>
        /// Devuelve el valor calculado.
        /// </returns>
        public double ConvertXToCanvas(double x, double wxmin, double wxmax)
        {
            if (wxmax - wxmin != 0)
            {
                return (945 * (x - wxmin) / (wxmax - wxmin)) + 10;
            }
            else return 0;
        }

        /// <summary>
        /// Convierte el valor de X de coordenadas del Canvas a coordenadas del Dispositivo.
        /// </summary>
        /// <param name="y">Valor de Y a calcular.</param>
        /// <param name="wymin">Valor de wymin.</param>
        /// <param name="wymax">Valor de wymax.</param>
        /// <returns>
        /// Devuelve el valor calculado.
        /// </returns>
        public double ConvertYToCanvas(double y, double wymin, double wymax)
        {
            if (wymax - wymin != 0)
            {
                return (547 * (y - wymin) / (wymax - wymin)) + 10;
            }
            else return 0;
        }

    }
}
