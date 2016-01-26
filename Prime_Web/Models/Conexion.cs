using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Prime_Web.Models
{
    public class Conexion
    {
        /*public string GetConnectionStrings(string dataSource)
        {
            string ConStr = "";
            //string ConStr = ConfigurationManager.ConnectionStrings["db_Prime_WebEntities"].ConnectionString;
            if(dataSource == null)
            {
                ConStr = "data source=208.91.198.174;initial catalog=db_Prime_Web;persist security info=True;user id=catriana;password=iCx0y2_9";
            }
            else
            {
                ConStr = dataSource;
            }
            //string ConStr = "data source=208.91.198.174;initial catalog=db_Prime_Web;persist security info=True;user id=catriana;password=iCx0y2_9";
            return ConStr;
        }*/

        public DataTable SelectDataTable(String Sql, string dataSource)
        {
            string ConStr = "";
            DataTable dt = new DataTable();
            if (dataSource == null)
            {
                ConStr = "data source=208.91.198.174;initial catalog=db_Prime_Web;persist security info=True;user id=catriana;password=iCx0y2_9";
            }
            else
            {
                ConStr = dataSource;
            }
            SqlConnection oSqlConnection = new SqlConnection(ConStr);
            try
            {
                oSqlConnection.Open();
                SqlDataAdapter sqlda = new SqlDataAdapter(Sql, ConStr);
                sqlda.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                oSqlConnection.Close();
                oSqlConnection.Dispose();
            }
            return dt;
        }

        public void obtenerDataSource(string idMenu)
        {
            Conexion conexion = new Conexion();
            DataRow[] config = conexion.SelectDataTable("select bd.Id_Base_Datos, bd.str_Ruta_Base_Datos, ma.ID_Menu from tbl_M_Menú_Aplicaciones ma "
                + "inner join tbl_M_Bases_Datos bd on bd.Id_Base_Datos = ma.Id_Base_Datos where ma.ID_Menu = " + idMenu, null).Select();
            if (config.Count() > 0)
            {
                DataRow dr = config[0];
                System.Web.HttpContext.Current.Session["dataSource"] = dr.Field<string>("str_Ruta_Base_Datos");
            }
        }
    }
}