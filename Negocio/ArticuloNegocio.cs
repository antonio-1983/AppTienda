using Dominio;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Negocio
{
    public class ArticuloNegocio
    {
       public List<Articulo> listar()
        {
			AccesoDatos datos = new AccesoDatos();
			List<Articulo> list = new List<Articulo>();
			try
			{
				datos.SetearQuery("select a.Id,a.codigo,a.nombre,a.descripcion,m.descripcion marca, a.IdMarca,c.descripcion categoria,a.IdCategoria,a.imagenUrl,a.precio from ARTICULOS a left join Marcas m on idMarca=m.id left join CATEGORIAS c on idCategoria=c.Id");
				datos.Ejecutar();
				while (datos.ConexionDataReader.Read())
				{
					Articulo aux = new Articulo();
					aux.Id = (int)datos.ConexionDataReader["Id"];
					aux.Codigo = (string)datos.ConexionDataReader["codigo"];
					aux.Nombre = (string)datos.ConexionDataReader["nombre"];
					aux.Descripcion = (string)datos.ConexionDataReader["descripcion"];
					aux.Marca = new Marca();
					aux.Marca.Id = (int)datos.ConexionDataReader["IdMarca"];
					aux.Marca.Descripcion= (string)datos.ConexionDataReader["marca"];
					aux.Categoria = new Categoria();
					aux.Categoria.Id = (int)datos.ConexionDataReader["IdCategoria"];
					aux.Categoria.Descripcion = (string)datos.ConexionDataReader["Categoria"];
                    if (!(datos.ConexionDataReader["imagenUrl"] is DBNull))
                    aux.ImagenUrl = (string)datos.ConexionDataReader["imagenUrl"];
					aux.Precio = (decimal)datos.ConexionDataReader["precio"];
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

        public string ArchivoImagen(string imagen)
        {
            string imagenDB = null; 
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearQuery("select ImagenUrl from articulos where ImagenUrl=@Imagen");
                datos.setearParametro("@Imagen", imagen);
                datos.Ejecutar();
                while (datos.ConexionDataReader.Read())
                {
                    if (!(datos.ConexionDataReader["ImagenUrl"] is DBNull))
                        imagenDB = (string)datos.ConexionDataReader["ImagenUrl"];
                }
                        return imagenDB;
            }
            catch (Exception)
            {
                throw;
            }        
        }
		public void Eliminar(int id)
		{
			AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearQuery("delete from articulos where id=@id");
                datos.setearParametro("@id", id);
                datos.Ejecutar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Agregar(Articulo articulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearQuery("insert into articulos (Codigo,Nombre,Descripcion,IdMarca,IdCategoria,ImagenUrl,Precio) values(@Codigo,@Nombre,@Descripcion,@IdMarca,@IdCategoria,@ImagenUrl,@Precio)");
                datos.setearParametro("@Codigo", articulo.Codigo);
                datos.setearParametro("@Nombre", articulo.Nombre);
                datos.setearParametro("@Descripcion", articulo.Descripcion);
                datos.setearParametro("@IdMarca", articulo.Marca.Id);
                datos.setearParametro("@IdCategoria", articulo.Categoria.Id);
                datos.setearParametro("@ImagenUrl", articulo.ImagenUrl);
                datos.setearParametro("@Precio", articulo.Precio);
                datos.EjecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.CloseConexion();
            }           
        }
        public void Modificar(Articulo articulo)//se puede agregar un atributo privado de articulo para no estar setando el parametro
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearQuery("update articulos set codigo = @Codigo, Nombre = @Nombre, Descripcion=@Descripcion, IdMarca = @IdMarca, IdCategoria = @IdCategoria,ImagenUrl=@ImagenUrl,precio=@Precio where id=@Id");

                datos.setearParametro("@Id",articulo.Id);
                datos.setearParametro("@Codigo", articulo.Codigo);
                datos.setearParametro("@Nombre", articulo.Nombre);
                datos.setearParametro("@Descripcion", articulo.Descripcion);
                datos.setearParametro("@IdMarca", articulo.Marca.Id);
                datos.setearParametro("@IdCategoria", articulo.Categoria.Id);
                datos.setearParametro("@ImagenUrl", articulo.ImagenUrl);
                datos.setearParametro("@Precio", articulo.Precio);
                datos.EjecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }
        public List<Articulo> Filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> list = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            string consulta = "select a.Id,a.codigo,a.nombre,a.descripcion,m.descripcion marca, a.IdMarca,c.descripcion categoria,a.IdCategoria,a.imagenUrl,a.precio from ARTICULOS a left join Marcas m on idMarca=m.id left join CATEGORIAS c on idCategoria=c.Id where ";
            try
            {
                if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += " Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += " Precio < " + filtro;
                            break;
                        default:
                            consulta += " Precio = " + filtro;
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Empieza con":
                            consulta += " Nombre like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else if(campo == "Categoria")
                {
                    switch (criterio)
                    {
                        case "Empieza con":
                            consulta += " c.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " c.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " c.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    switch (criterio)
                    {
                        case "Empieza con":
                            consulta += " m.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " m.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " m.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Codigo")
                {
                    switch (criterio)
                    {
                        case "Empieza con":
                            consulta += " a.Codigo like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " a.Codigo like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " a.Codigo like '%" + filtro + "%'";
                            break;
                    }
                }
                datos.SetearQuery(consulta);
                datos.Ejecutar();
                while (datos.ConexionDataReader.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.ConexionDataReader["Id"];
                    aux.Codigo = (string)datos.ConexionDataReader["codigo"];
                    aux.Nombre = (string)datos.ConexionDataReader["nombre"];
                    aux.Descripcion = (string)datos.ConexionDataReader["descripcion"];
                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.ConexionDataReader["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.ConexionDataReader["marca"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.ConexionDataReader["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.ConexionDataReader["Categoria"];
                    if (!(datos.ConexionDataReader["imagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)datos.ConexionDataReader["imagenUrl"];
                    aux.Precio = (decimal)datos.ConexionDataReader["precio"];
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
