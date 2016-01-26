using Prime_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Prime_Web.Controllers
{
    public class ReporteEncuestaController : Controller
    {
        int[,] permisos = { { 0, 0, 0, 0, 0, 0, 0}, { 0, 2, 4, 5, 6, 8, 10 } };
        List<Dimension> dim = new List<Dimension>();
        
        // GET: ReporteGeneralEncuestas
        [HttpGet]
        public ActionResult ReporteGeneral(string idMenu, string idRegistro)
        {
            Usuario usuario = Session["usuario"] as Usuario;
            MenuController reporte = new MenuController();
            string parametros = reporte.GenerateParameters(usuario, idMenu, idRegistro, dim);
            ViewBag.Parametros = parametros;
            ValidateRangoRespuestas(idRegistro);
            ViewBag.Advertencia = null;
            System.Web.HttpContext.Current.Session["dimensiones"] = dim;
            return View("ReporteGeneral");
        }

        [HttpGet]
        public ActionResult ReporteDetalle(string idMenu, string idRegistro)
        {
            Usuario usuario = Session["usuario"] as Usuario;
            //string dataSource = Session["dataSource"].ToString();
            //string idMenu = Session["menu"].ToString();
            MenuController reporte = new MenuController();
            string parametros = reporte.GenerateParameters(usuario, idMenu, idRegistro, dim);
            ViewBag.Parametros = parametros;
            /*Session["dimensiones"] = dimensiones;*/
            return View("ReporteDetalle");
        }

        public void cargar(){}

        public void ValidateRangoRespuestas(string idRegistro)
        {
            int r = 0;
            Conexion conexion = new Conexion();
            string datasource = System.Web.HttpContext.Current.Session["dataSource"].ToString();
            string[] aux = null;
            DataRow infoTabla = (conexion.SelectDataTable("SELECT MAX(resp.Valor) FROM tbl_M_Preguntas preg "
                +"INNER JOIN tbl_M_Cantidad_Respuestas resp ON preg.Cantidad_Prosibles_Respuestas = resp.Cantidad_Id "
                +"and preg.Encuesta = "+idRegistro, datasource).Select())[0];
            //IEnumerable<DataRow> infoTabla = rango.AsEnumerable().Where(x => x["Encuesta"].ToString() == idRegistro);
            try
            {
                r = Int32.Parse(infoTabla[0].ToString());
                DataRow cantidadCaras = (conexion.SelectDataTable("SELECT Cantidad_Caras FROM tbl_M_Cantidad_Respuestas WHERE Valor = " + r, datasource).Select())[0];
                aux = (cantidadCaras[0].ToString()).Split(',');

                //for (int i = 0; i < aux.Length; i++)
                //{
                //    for(int j = 0; j < permisos.Length; j++)
                //    {
                //        if (aux[i].Equals(permisos[j, 1]))
                //        {
                //            permisos[j, 0] = 1;
                //        }
                //    }
                //}
            }
            catch (Exception e){}
            /*r = Int32.Parse(infoTabla[0].ToString());
            if (!infoTabla[0].Equals(null))
            {
                r = Int32.Parse(infoTabla[0].ToString());
            }*/
            //r = Int32.Parse(dr[0].ToString());
            System.Web.HttpContext.Current.Session["rango"] = r;
            System.Web.HttpContext.Current.Session["permisos"] = aux;
        }
    }
}