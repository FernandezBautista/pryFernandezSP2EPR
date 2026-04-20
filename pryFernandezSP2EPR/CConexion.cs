using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ADOX;

namespace pryFernandezSP2EPR
{
    public class CConexion
    {
        private string cadena;
        private string error;

        public string ObtenerError()
        {
            return error;
        }

        public bool CrearBase(string ruta)
        {
            try
            {
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }

                Catalog cat = new Catalog();
                cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ruta);
                cat = null;

                cadena = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ruta;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public int MigrarDatos(string rutaArchivo, string sqlCreate, string tabla)
        {
            int filas = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(cadena))
                {
                    con.Open();

                    OleDbCommand cmdTab = new OleDbCommand(sqlCreate, con);
                    cmdTab.ExecuteNonQuery();

                    string[] lineas = File.ReadAllLines(rutaArchivo);

                    for (int i = 1; i < lineas.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lineas[i])) continue;

                        string[] datos = lineas[i].Split(';');
                        string sqlIns = "";

                        if (tabla == "Categorias")
                        {
                            sqlIns = "INSERT INTO Categorias VALUES (" + datos[0] + ", '" + datos[1] + "')";
                        }
                        else if (tabla == "Articulos")
                        {
                            string precio = datos[3].Replace(",", ".");
                            sqlIns = "INSERT INTO Articulos VALUES (" + datos[0] + ", '" + datos[1] + "', " + datos[2] + ", " + precio + ")";
                        }

                        OleDbCommand cmdIns = new OleDbCommand(sqlIns, con);
                        cmdIns.ExecuteNonQuery();
                        filas++;
                    }
                }
                return filas;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return -1;
            }
        }
    }
}