using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Forms;

namespace GeoGebra
{
    /// <summary>
    /// Clase auxiliar para el manejo de ficheros. 
    /// Lee y escribe los diferentes ficheros del programa.
    /// </summary>
    public class FileFunctions
    {
        /// <summary>
        /// Referencia a la ventana Principal.
        /// </summary>
        private MainWindow mw;

        /// <summary>
        /// Constructor de la Clase.
        /// </summary>
        public FileFunctions()
        {
            this.mw = (MainWindow)System.Windows.Application.Current.MainWindow;
        }


        // FUNCIONES PÚBLICAS

        /// <summary>
        /// Función Pública que crea el cuadro de diálogo para guardar el Canvas
        /// en un archivo PNG.
        /// </summary>
        public void SaveImage()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "Image"; // Default file name
            saveFileDialog.DefaultExt = ".png"; // Default file extension
            saveFileDialog.Filter = "PNG File (.png)|*.png"; // Filter files by extension
            if (saveFileDialog.ShowDialog() == true)
                SaveCanvasToFile(mw.canGraph, saveFileDialog.FileName);
        }

        /// <summary>
        /// Guarda una colección en un archivo CSV.
        /// </summary>
        /// <param name="dc">DataCollection a guardar.</param>
        public void SaveCollectionOnCSVFile(DataCollection dc)
        {
            if (dc != null)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.FileName = dc.name; // Default file name
                saveFileDialog.DefaultExt = ".csv"; // Default file extension
                saveFileDialog.Filter = "CSV|*.csv"; // Filter files by extension
                if (saveFileDialog.ShowDialog() == true)
                    SaveDCOnCSVFile(dc, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Guarda todas las colecciones del programa en CSV.
        /// </summary>
        /// <param name="dm">DataManager con todas las colecciones a guardar.</param>
        public void SaveAllCollectionsOnCSV(DataManager dm)
        {
            MessageBoxResult valor = System.Windows.MessageBox.Show("¿Desea que las colecciones se actualicen con los datos de las Hojas de Edición sin guardar?", "Advertencia", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            switch (valor)
            {
                case MessageBoxResult.Yes:
                    MainWindow.AskForUpdateAllCollections();
                    break;
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.No:
                    break;
                default:
                    return;
            }

            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            string path = dialog.SelectedPath;
            foreach (DataCollection dc in dm.dataCollection)
            {
                string name = path + "/" + dc.name + ".csv";
                SaveDCOnCSVFile(dc, name);
            }

        }

        /// <summary>
        /// Lee una colección desde un fichero CSV.
        /// </summary>
        /// <returns>
        /// Devuelve la DataCollection leída.
        /// </returns>
        public DataCollection ReadCollectionFromCSVFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".csv"; // Default file extension
            openFileDialog.Filter = "CSV|*.csv"; // Filter files by extension
            if (openFileDialog.ShowDialog() == true)
                return ReadCSVAndSaveOnCollection(openFileDialog);
            else
                return null;
        }

        // FUNCIONES PRIVADAS

        /// <summary>
        /// Guarda una colección en un csv con la ruta 'name'
        /// </summary>
        /// <param name="dc">DataCollection a guardar.</param>
        /// <param name="name">Path del fichero a guardar.</param>
        private void SaveDCOnCSVFile(DataCollection dc, string name)
        {
            string strFilePath = name;
            string separator = ";";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@strFilePath))
            {
                file.WriteLine(dc.name);
                foreach (Punto punto in dc.data)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    string to_write = punto.x + separator + punto.y;
                    file.WriteLine(to_write);
                }
            }
        }

        /// <summary>
        /// Lee una colección de un csv con la ruta guardada en sfd.
        /// </summary>
        /// <param name="sfd">Cuadro de diálogo del cuál se extraerá el path del archivo.</param>
        /// <returns>
        /// Devuelve la DataCollection leída.
        /// </returns>
        private DataCollection ReadCSVAndSaveOnCollection(Microsoft.Win32.OpenFileDialog sfd)
        {
            string strFilePath = sfd.FileName;
            string[] lines = System.IO.File.ReadAllLines(@strFilePath);
            DataCollection dc = new DataCollection(lines[0]);
            List<Punto> puntos = new List<Punto>();
            foreach (string line in lines)
            {
                string[] values = line.Split(';');
                if (values.Length > 1)
                    puntos.Add(new Punto() { x = float.Parse(values[0]), y = float.Parse(values[1]) });
            }
            dc.data = puntos;
            return dc;
        }

        /// <summary>
        /// Guarda el canvas en un archivo PNG.
        /// </summary>
        /// <param name="canvas">Canvas a guardar.</param>
        /// <param name="filename">Nombre del fichero.</param>
        /// <param name="dpi">Resolución.</param>
        private static void SaveCanvasToFile(Canvas canvas, string filename, int dpi = 96)
        {
            MainWindow window = (MainWindow)System.Windows.Application.Current.MainWindow;
            Size size = new Size(window.Width, window.Height);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width
                (int)window.Height, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(canvas);

            SaveRTBAsPNGBMP(rtb, filename);
        }

        ///<summary>
        /// Guarda la pantalla en un archivo PNG
        ///</summary>
        /// <param name="window">Ventana a guardar.</param>
        /// <param name="dpi">Resolución.</param>
        /// <param name="filename">Nombre del fichero.</param>
        private static void SaveWindowToFile(Window window, int dpi, string filename)
        {
            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width
                (int)window.Width, //height
                dpi, //dpi x
                dpi, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(window);

            SaveRTBAsPNGBMP(rtb, filename);
        }

        /// <summary>
        /// Función interna para guardar el archivo.
        /// </summary>
        /// <param name="bmp">Bitmap.</param>
        /// <param name="filename">Nombre del fichero.</param>
        private static void SaveRTBAsPNGBMP(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

    }
}
