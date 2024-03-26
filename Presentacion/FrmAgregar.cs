using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;


namespace Presentacion
{
    public partial class FrmAgregar : Form
    {
        OpenFileDialog archivo = null;
        private Articulo articulo = null;
        public FrmAgregar()
        {
            InitializeComponent();
            txtPrecio.KeyPress += txtPrecio_KeyPress;
        }

        public FrmAgregar(Articulo articulo)
        {
            this.articulo= articulo;
            InitializeComponent();
            Text = "Modificar Articulo";
            txtPrecio.KeyPress += txtPrecio_KeyPress;
        }

        private void FrmAgregar_Load(object sender, EventArgs e)
        {
            
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            try
            {
                cbxMarca.DataSource= marcaNegocio.ListarMarca();
                cbxCategoria.DataSource = categoriaNegocio.ListarCategoria();
                cbxMarca.ValueMember = "id";
                cbxMarca.DisplayMember = "descripcion";
                cbxCategoria.ValueMember = "id";
                cbxCategoria.DisplayMember = "descripcion";
                if (articulo!=null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtUrl.Text = articulo.ImagenUrl;
                    CargarImagen(articulo.ImagenUrl);
                    cbxMarca.SelectedValue = articulo.Marca.Id;
                    cbxCategoria.SelectedValue = articulo.Categoria.Id;
                    txtPrecio.Text=articulo.Precio.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CargarImagen(string imagen)
        {
            try
            {
                pbxAgregar.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxAgregar.Load("https://t3.ftcdn.net/jpg/02/48/42/64/360_F_248426448_NVKLywWqArG2ADUxDq6QprtIzsF82dMF.jpg");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio articuloNegocio = new ArticuloNegocio();

            try
            {
                if (articulo == null)
                {
                    articulo = new Articulo();
                }
                if (txtCodigo.Text == "" || txtNombre.Text == "" || txtDescripcion.Text == "" || txtPrecio.Text == "")
                {
                    MessageBox.Show("Ingrese todos los datos");
                    this.Show();
                }
                else
                {
                    articulo.Codigo = txtCodigo.Text;
                    articulo.Nombre = txtNombre.Text;
                    articulo.Descripcion = txtDescripcion.Text;
                    articulo.ImagenUrl = txtUrl.Text;
                    articulo.Marca = (Marca)cbxMarca.SelectedItem;
                    articulo.Categoria = (Categoria)cbxCategoria.SelectedItem;
                    articulo.Precio = decimal.Parse(txtPrecio.Text);

                    if (articulo.Id != 0)
                    {
                        articuloNegocio.Modificar(articulo);
                        MessageBox.Show("Modificado exitosamente");
                    }
                    else
                    {
                        articuloNegocio.Agregar(articulo);
                        MessageBox.Show("Agregado exitosamente");

                    }

                    if (archivo != null && !File.Exists(ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName))   //&& txtUrl.Text.ToUpper().Contains("HTTP")
                    {
                        File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
                    }

                    this.Close();
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            CargarImagen(txtUrl.Text);
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            ArticuloNegocio articuloNegocio = new ArticuloNegocio();
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg";
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                string img = archivo.FileName;
                if (articuloNegocio.ArchivoImagen(img)==img)
                {
                    MessageBox.Show("La imagen ya esta siendo usada por otro articulo");
                    txtUrl.Text = " ";
                }
                else
                {
                    txtUrl.Text = archivo.FileName;
                    CargarImagen(archivo.FileName);
                }
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            //permite numeros del 0 al 9, un solo punto (.) y solo 4 digitos despues de la coma
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.')
            {
                if ((sender as TextBox).Text.Contains('.') || (sender as TextBox).Text.Length == 0)
                {
                    e.Handled = true;
                    return;
                }
            }
            if ((sender as TextBox).Text.Contains('.'))
            {
                string[] parts = (sender as TextBox).Text.Split('.');
                if (parts.Length > 1 && parts[1].Length >= 4 && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
