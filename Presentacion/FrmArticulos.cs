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
using System.Diagnostics.Eventing.Reader;


namespace Presentacion
{
    public partial class FrmArticulos : Form
    {
        private List<Articulo> listArticulos;
        public FrmArticulos()
        {
            InitializeComponent();
            //creo una carpeta si no existe para guardar los archivos de imagenes
            string rutaCarpeta = @"C:\CarpetaApp";
            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta); 
            }
            txtFiltroAvanzado.KeyPress += txtFiltroAvanzado_KeyPress;
        }
        private void Articulos_Load(object sender, EventArgs e)
        {
            Cargar();
            cbxCampo.Items.Add("Codigo");
            cbxCampo.Items.Add("Nombre");
            cbxCampo.Items.Add("Marca");
            cbxCampo.Items.Add("Categoria");
            cbxCampo.Items.Add("Precio");
            txtFiltroAvanzado.Enabled = false;
            txtFiltroAvanzadoString.Enabled = false;
            btnBuscar.Enabled = false;
        }
        private void Cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            { 
                listArticulos = negocio.listar();
                dgvArticulos.DataSource = listArticulos;
                OcultarColumnas();
                CargarImagen(listArticulos[0].ImagenUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void OcultarColumnas()
        {
            dgvArticulos.Columns["imagenUrl"].Visible = false;
            dgvArticulos.Columns["Id"].Visible = false;
        }
        public void CargarImagen(string imagen)
        {
            try
            {
                ptbArticulos.Load(imagen);
            }
            catch (Exception ex)
            {
                ptbArticulos.Load("https://t3.ftcdn.net/jpg/02/48/42/64/360_F_248426448_NVKLywWqArG2ADUxDq6QprtIzsF82dMF.jpg");
            }
        }
        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvArticulos.CurrentRow !=null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                CargarImagen(seleccionado.ImagenUrl);
            }
        }
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada = null;
            string filtro=txtFiltro.Text;
            if(filtro.Length > 2)
            {
                listaFiltrada = listArticulos.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Marca.Descripcion.ToUpper().Contains(filtro.ToUpper())|| x.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper()));
                if(listaFiltrada.Count == 0)
                {
                    listaFiltrada = listArticulos;
                }
            }
            else
            {
                listaFiltrada = listArticulos;
            }
            dgvArticulos.DataSource = listaFiltrada;
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAgregar frmAgregar = new FrmAgregar();
            frmAgregar.ShowDialog();
            Cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            FrmAgregar frmAgregar = new FrmAgregar(seleccionado);
            frmAgregar.ShowDialog();
            Cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio articuloNegocio = new ArticuloNegocio();
            Articulo seleccionado = new Articulo();
                try
                {
                    DialogResult resultado = MessageBox.Show("¿Esta seguro que quiere eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resultado == DialogResult.Yes)
                    {
                        seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                        string img=archivoRuta(seleccionado.ImagenUrl);

                    //se fija si hay un archivo de imagen y lo elimina de la carpeta
                        if (File.Exists(ConfigurationManager.AppSettings["images-folder"] + img))
                            {
                                File.Delete(ConfigurationManager.AppSettings["images-folder"] + img);
                            }
                        articuloNegocio.Eliminar(seleccionado.Id);   
                    }
                    Cargar();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }
        private string archivoRuta(string  ruta)//funcion para extraer el nombre del archivo de imagen
        {
             string[] rutaDividida = ruta.Split('\\');
             string img = rutaDividida[rutaDividida.Length-1];
            return img;
        }
        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbxCampo.SelectedItem.ToString();
            if (opcion == "Precio")
            {
                OcultarFiltros();
                cbxCriterio.Items.Add("Mayor a");
                cbxCriterio.Items.Add("Menor a");
                cbxCriterio.Items.Add("Igual a");
            }
            else
            {
                OcultarFiltros();
                cbxCriterio.Items.Add("Empieza con");
                cbxCriterio.Items.Add("Termina con");
                cbxCriterio.Items.Add("Contiene");
            }
        }
        private void cbxCriterio_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbxCriterio.SelectedItem.ToString();
            if (opcion == "Mayor a" || opcion == "Menor a" || opcion == "Igual a")
            {
                txtFiltroAvanzadoString.Visible = false;
                txtFiltroAvanzadoString.Enabled = false;
                txtFiltroAvanzado.Visible = true;
                txtFiltroAvanzado.Enabled = true;
            }
            else if(opcion == "Empieza con" || opcion == "Termina con" || opcion == "Contiene")
            {
                txtFiltroAvanzado.Visible = false;
                txtFiltroAvanzado.Enabled = false;
                txtFiltroAvanzadoString.Visible = true;
                txtFiltroAvanzadoString.Enabled = true;
            }
        }
        private void OcultarFiltros()
        {
            txtFiltroAvanzadoString.Enabled = false;
            txtFiltroAvanzado.Enabled = false;
            txtFiltroAvanzado.Text = null;
            txtFiltroAvanzadoString.Text = null;
            cbxCriterio.Items.Clear();
            cbxCriterio.SelectedItem = null;
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                    string filtro;
                    string campo = cbxCampo.SelectedItem.ToString();
                    string criterio = cbxCriterio.SelectedItem.ToString();
                    if (txtFiltroAvanzado.Text.Length>0)
                    {
                     filtro = txtFiltroAvanzado.Text.ToString();
                    }
                    else
                    {
                     filtro = txtFiltroAvanzadoString.Text.ToString();
                    }
                    dgvArticulos.DataSource = negocio.Filtrar(campo, criterio, filtro);
                    txtFiltroAvanzado.Text = null;
                    txtFiltroAvanzadoString.Text = null;
                    btnBuscar.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            Cargar();
            OcultarFiltros();
            txtFiltro.Text = null;
        }
        private void txtFiltroAvanzado_KeyPress(object sender, KeyPressEventArgs e)
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
            if (txtFiltroAvanzado.Text.Length > 0)
            {
                btnBuscar.Enabled = true;
            }
        }

        private void txtFiltroAvanzadoString_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtFiltroAvanzadoString.Text;
            if (filtro.Length > 0)
            {
                btnBuscar.Enabled = true;
            }
        }
    }
}
