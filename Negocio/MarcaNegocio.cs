using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class MarcaNegocio
    {
        public List<Marca> ListarMarca()
        {
            AccesoDatos datos = new AccesoDatos();
            List<Marca> list = new List<Marca>();
            try
            {
                datos.SetearQuery("select id,descripcion from Marcas");
                datos.Ejecutar();
                while(datos.ConexionDataReader.Read())
                {
                    Marca aux = new Marca();
                    aux.Id = (int)datos.ConexionDataReader["id"];
                    aux.Descripcion = (string)datos.ConexionDataReader["descripcion"];
                    list.Add(aux);
                }
                datos.CloseConexion();
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
