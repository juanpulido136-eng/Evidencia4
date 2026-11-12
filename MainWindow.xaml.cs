using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Productos
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Marca { get; set; } = "";
        public decimal Precio { get; set; }
        public int Existencias { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Imagen { get; set; } = "";
    }

    public sealed partial class MainWindow : Window
    {
        private readonly List<Producto> _productos = new List<Producto>();
        private int _siguienteId = 1;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void BtnImagen_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");

            StorageFile archivo = await picker.PickSingleFileAsync();
            if (archivo != null)
            {
                txtImagen.Text = archivo.Path;
            }
        }

        private async void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MostrarError("El nombre y la marca son obligatorios.");
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Number,
                                  CultureInfo.InvariantCulture, out decimal precio) || precio < 0)
            {
                MostrarError("El precio no es válido (ejemplo: 12.50).");
                return;
            }

            if (!int.TryParse(txtExistencias.Text, out int existencias) || existencias < 0)
            {
                MostrarError("Las existencias deben ser un número entero.");
                return;
            }

            // Simulación de registro (se guarda solo en memoria)
            var producto = new Producto
            {
                Id = _siguienteId++,
                Nombre = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim(),
                Marca = txtMarca.Text.Trim(),
                Precio = precio,
                Existencias = existencias,
                FechaLanzamiento = dpFecha.Date.DateTime,
                Imagen = txtImagen.Text
            };
            _productos.Add(producto);

            txtMensaje.Text = "";

            // Ventana que confirma el registro
            ContentDialog dialogo = new ContentDialog
            {
                Title = "Registro exitoso",
                Content = $"El producto se registró correctamente.\n\n" +
                          $"Id: {producto.Id}\n" +
                          $"Nombre: {producto.Nombre}\n" +
                          $"Marca: {producto.Marca}\n" +
                          $"Precio: {producto.Precio:C}\n" +
                          $"Existencias: {producto.Existencias}",
                CloseButtonText = "Aceptar",
                XamlRoot = this.Content.XamlRoot
            };

            await dialogo.ShowAsync();

            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtMarca.Text = "";
            txtPrecio.Text = "";
            txtExistencias.Text = "";
            txtImagen.Text = "";
        }

        private void MostrarError(string texto)
        {
            txtMensaje.Text = texto;
            txtMensaje.Foreground = new SolidColorBrush(Colors.Red);
        }
    }
}