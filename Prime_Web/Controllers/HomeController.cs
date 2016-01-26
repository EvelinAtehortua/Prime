using Prime_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;

namespace Prime_Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult validarInicio(Usuario u, string recordarme)
        {
            Conexion conexion = new Conexion();
            DataRow[] rows = conexion.SelectDataTable("SELECT Id_Usuario, Nombre_Usuario, Clave_Usuario, str_Organización FROM tbl_M_Usuarios WHERE Id_Usuario = '" + u.idUsuario + "' AND Clave_Usuario = '" + u.clave + "'", null).Select();
            if (rows.Count() > 0)
            {
                DataRow dr = rows[0];
                u.nomUsuario = dr.Field<string>("Nombre_Usuario");
                u.nomOrganizacion = dr.Field<string>("str_Organización");
                ViewBag.Error = null;
                System.Web.HttpContext.Current.Session["usuario"] = u;
                System.Web.HttpContext.Current.Session["recordarUsuario"] = 1;
                return RedirectToAction("Menu", "Menu");
            }
            else
            {
                ViewBag.Error = "Usuario y/o clave incorrectos";
                u.clave = "";
                return View("Index", u);
            }
        }
    }
}