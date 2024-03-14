using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Negocio
{
    public class AccesoDatos
    {
        private SqlConnection conn;   //coneccion
        private SqlCommand conexionCommand;  //comando
        private SqlDataReader conexionDataReader; //lector

        public SqlDataReader ConexionDataReader
        {
            get { return conexionDataReader; }
        }

        public AccesoDatos()
        {
            conn = new SqlConnection("server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true");
            conexionCommand = new SqlCommand();
        }
        public void SetearQuery(string query)
        {
            conexionCommand.CommandType = System.Data.CommandType.Text;
            conexionCommand.CommandText = query;
        }

        public void Ejecutar()
        {
            conexionCommand.Connection = conn;
            try
            {
                conn.Open();
                conexionDataReader = conexionCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void EjecutarAccion()
        {
            conexionCommand.Connection = conn;
            try
            {
                conn.Open();
                conexionCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setearParametro(string nombre, object valor)
        {
            conexionCommand.Parameters.AddWithValue(nombre, valor);
        }
        public void SetearPrecio(string nombre, object valor)
        {
            conexionCommand.Parameters.Add(nombre, System.Data.SqlDbType.Money).Value = valor;
        }

        public void CloseConexion()
        {
            if (conexionDataReader != null) //se acabo la lectura
            {
                conexionDataReader.Close();//cierro lector
                conn.Close();
            }
        }
    }
}
