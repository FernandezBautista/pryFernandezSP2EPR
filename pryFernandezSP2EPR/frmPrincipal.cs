using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace pryFernandezSP2EPR
{
    public partial class frmPrincipal : Form
    {
        private List<string> archivosSeleccionados = new List<string>();

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
        }

        private void btnAñadir_Click(object sender, EventArgs e)
        {
            {
                OpenFileDialog buscador = new OpenFileDialog();
                buscador.Multiselect = true;
                buscador.Filter = "Archivos de texto|*.txt";

                if (buscador.ShowDialog() == DialogResult.OK)
                {
                    foreach (string ruta in buscador.FileNames)
                    {
                        if (!archivosSeleccionados.Contains(ruta))
                        {
                            archivosSeleccionados.Add(ruta);
                            lstInfo.Items.Add("Archivo cargado: " + Path.GetFileName(ruta));
                        }
                    }
                }
            }
        }

        private void btnMigrar_Click(object sender, EventArgs e)
        {
            if (archivosSeleccionados.Count == 0)
            {
                lstInfo.Items.Add("Seleccioná los archivos primero.");
                return;
            }

            CConexion objBase = new CConexion();
            string rutaMdb = "Distribuidora.mdb";

            if (objBase.CrearBase(rutaMdb))
            {
                lstInfo.Items.Clear();
                foreach (string ruta in archivosSeleccionados)
                {
                    string nombreArchivo = Path.GetFileNameWithoutExtension(ruta).ToUpper();
                    string sql = "";
                    string tablaIdentificada = "";

                    if (nombreArchivo.Contains("CATEGORIA"))
                    {
                        tablaIdentificada = "Categorias";
                        sql = "CREATE TABLE Categorias (IdCategoria INT, Nombre TEXT(50))";
                    }
                    else if (nombreArchivo.Contains("ARTICULO"))
                    {
                        tablaIdentificada = "Articulos";
                        sql = "CREATE TABLE Articulos (IdArticulo INT, Nombre TEXT(50), IdCategoria INT, Precio DOUBLE)";
                    }

                    if (tablaIdentificada != "")
                    {
                        lstInfo.Items.Add("Migrando " + tablaIdentificada + "...");
                        int r = objBase.MigrarDatos(ruta, sql, tablaIdentificada);

                        if (r != -1) lstInfo.Items.Add("Archivo migrado Filas: " + r);
                        else lstInfo.Items.Add("Error: " + objBase.ObtenerError());
                    }
                }
                lstInfo.Items.Add("Proceso terminado.");
            }
        }
   
    }
}
        
    
