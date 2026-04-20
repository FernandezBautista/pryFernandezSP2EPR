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
            {
                if (archivosSeleccionados.Count == 0)
                {
                    lstInfo.Items.Add("Seleccione archivos antes de empezar.");
                    return;
                }

                CConexion objBase = new CConexion();
                string rutaMdb = "Distribuidora.mdb";

                if (objBase.CrearBase(rutaMdb))
                {
                    lstInfo.Items.Clear();

                    foreach (string ruta in archivosSeleccionados)
                    {
                        string tabla = Path.GetFileNameWithoutExtension(ruta);
                        lstInfo.Items.Add("Migrando datos de " + tabla + "...");

                        string sql = "";
                        if (tabla == "Categorias")
                        {
                            sql = "CREATE TABLE Categorias (IdCategoria INT, Nombre TEXT(50))";
                        }
                        else if (tabla == "Articulos")
                        {
                            sql = "CREATE TABLE Articulos (IdArticulo INT, Nombre TEXT(50), IdCategoria INT, Precio DOUBLE)";
                        }

                        int registros = objBase.MigrarDatos(ruta, sql, tabla);

                        if (registros != -1)
                        {
                            lstInfo.Items.Add("Se incorporaron: " + registros + " registros nuevos.");
                        }
                        else
                        {
                            lstInfo.Items.Add("Error: " + objBase.ObtenerError());
                        }
                        lstInfo.Items.Add("");
                    }
                    lstInfo.Items.Add("Migración finalizada.");
                }
            }
        }
    }
}