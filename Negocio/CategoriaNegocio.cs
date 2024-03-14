using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class CategoriaNegocio
    {
        public List<Categoria> ListarCategoria()
        {
            AccesoDatos datos = new AccesoDatos();
            List<Categoria> list = new List<Categoria>();
            try
            {
                datos.SetearQuery("select id,descripcion from Categorias");
                datos.Ejecutar();
                while(datos.ConexionDataReader.Read())
                {
                    Categoria aux = new Categoria();
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
