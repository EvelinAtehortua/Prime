using Prime_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Prime_Web.Controllers
{
    public class MenuController : Controller
    {
        List<int> dates = new List<int>();
        List<int> selects = new List<int>();
        Usuario usuario = System.Web.HttpContext.Current.Session["usuario"] as Usuario;
        // GET: Menu
        public ActionResult Menu()
        {
            string menu = GenerateMenuSistemas();
            ViewBag.Menu = menu;
            ViewBag.User = usuario.nomUsuario;
            System.Web.HttpContext.Current.Session["permisos"] = 0;
            return View();
        }

        public ActionResult CerrarSesion()
        {
            return View("../Home/Index", usuario);
        }

        // Generación de menú de sistemas
        public string GenerateMenuSistemas()
        {
            string menu = "";
            //menu += "<ul class='nav-sub'>";
            Conexion conexion = new Conexion();
            DataRow[] sistemas = conexion.SelectDataTable("select ubd.Id_Usuario, bd.Id_Base_Datos, bd.str_Descripción_Base_Datos from tbl_M_Bases_Datos bd "
                + "inner join tbl_M_Usuarios_X_Base_Datos ubd on ubd.Id_Base_Datos = bd.Id_Base_Datos "
                + "inner join tbl_M_Usuarios u on u.Id_Usuario = ubd.Id_Usuario where ubd.Id_Usuario = '" + usuario.idUsuario + "'", null).Select();
            //IEnumerable<DataRow> sistemas = sist.AsEnumerable().Where(x => x["Id_Usuario"].ToString() == idUsuario);
            if (sistemas.Count() > 0)
            {
                foreach (DataRow dr in sistemas)
                {
                    string MenuName = dr["str_Descripción_Base_Datos"].ToString();
                    int MenuID = Int32.Parse(dr["Id_Base_Datos"].ToString());
                    DataTable menus = conexion.SelectDataTable("SELECT u.Id_Usuario, ma.ID_Menu, ma.Descripcion, ma.Id_Papa, ma.Icono_Bootstrap, ma.Id_Base_Datos, ma.Url, ma.Tipo, ma.Id_Registro FROM   tbl_M_Bases_Datos bd "
                        +"INNER JOIN tbl_M_Menú_Aplicaciones ma ON bd.Id_Base_Datos = ma.Id_Base_Datos INNER JOIN tbl_M_Roles r ON bd.Id_Base_Datos = r.Id_Base_Datos "
                        +"INNER JOIN tbl_M_Roles_X_Autorizacion ra ON ma.ID_Menu = ra.Id_Menu AND r.Id_Rol = ra.ID_Rol INNER JOIN tbl_M_Usuarios_X_Base_Datos ubd ON bd.Id_Base_Datos = ubd.Id_Base_Datos "
                        +"INNER JOIN tbl_M_Usuarios u ON ubd.Id_Usuario = u.Id_Usuario INNER JOIN tbl_M_Usuarios_X_Roles ur ON r.Id_Rol = ur.Id_Rol AND u.Id_Usuario = ur.Id_Usuario where u.Id_Usuario = '" + usuario.idUsuario + "' AND ma.Id_Base_Datos = " + MenuID, null);
                    IEnumerable<DataRow> rows = menus.AsEnumerable().Where(x => Int32.Parse(x["Id_Papa"].ToString()) == 0);
                    if (rows.Count() > 0)
                    {
                        //String submenu = "#submenu" + MenuID;
                        menu += "<li class='nav-dropdown'><a href='' id='sistema' title='" + MenuName + "'><i class='fa  fa-fw fa-moon-o'></i>" + MenuName+"</a>";
                        string subMenu = "";
                        menu += GenerateMenuTotal(rows, subMenu, MenuID, menus);
                    }
                    /*else
                    {
                        menu += "<li><a href='' id='"+MenuID+"' title='sistema'><i class='fa  fa-fw fa-moon-o'></i>" + MenuName + "</a>";
                    }*/
                    menu += "</li>";
                }
            }
            //menu += "</ul>";
            return menu;
        }

        // Generación de menú de cada sistema
        public string GenerateMenuTotal(IEnumerable<DataRow> rows, string menu, int IDbd, DataTable menus)
        {
            menu += "<ul class='nav-sub'>";
            foreach (DataRow dr in rows)
            {
                int MenuID = dr.Field<int>("ID_Menu");
                string MenuName = dr.Field<string>("Descripcion");
                string IconoMenu = dr.Field<string>("Icono_Bootstrap");
                string Url = dr.Field<string>("Url");
                string Tipo = dr.Field<string>("Tipo").ToLower();
                int Registro = dr.Field<int>("Id_Registro");
                
                if (Tipo != null)
                {
                    if (Tipo.Equals("reporte"))
                    {
                        Url = "../" + Url + "?idMenu=" + MenuID + "&idRegistro=" + Registro;
                    }
                    else
                    {
                        Url = "../" + Url + "/" + Url + "?idMenu=" + MenuID;
                    }
                }
                else
                {
                    Url = "#";
                }
                //DataTable submenus = conexion.SelectDataTable("SELECT ID_Menu, Descripcion, Id_Papa, Icono_Bootstrap, Id_Base_Datos FROM tbl_M_Menú_Aplicaciones");
                IEnumerable<DataRow> r = menus.AsEnumerable().Where(x => Int32.Parse(x["Id_Papa"].ToString()) == MenuID);
                if (r.Count() > 0)
                {
                    //string submenut = "0" + submenu;
                    menu += "<li class='nav-dropdown'><a href='' id='menu' title='" + MenuName + "'>" + MenuName + "</a>";
                    string subMenu = "";
                    menu += GenerateMenuTotal(r, subMenu, IDbd, menus);
                }
                else
                {
                    menu += "<li><a href='"+Url+"' id='menu' title='" + MenuName + "'><span class='" + IconoMenu + "'></span>" + MenuName + "</a>";
                    //menu += "<li><a href='../Prueba/Prueba' id='" + IDbd + "' title='" + MenuName + "'><span class='" + IconoMenu + "'></span>" + MenuName + "</a>";
                }
                menu += "</li>";
            }
            menu += "</ul>";
            return menu;
        }


        // Generación de parametros para reportes

        public string GenerateParameters(string idMenu, string idRegistro, List<Dimension>dim)
        {
            string parametros = "";
            Conexion conexion = new Conexion();
            //string condicion = "";
            //Boolean b = false;
            conexion.obtenerDataSource(idMenu);
            DataRow[] param = conexion.SelectDataTable("select Id_Parametro, Dimension, Tabla_Carga, Tipo_Control, Nombre_Campo, Query_Dinamico from tbl_M_Parametros_Consulta_Tablero_Control where Id_Menu = " + idMenu, null).Select();

            if (param.Count() > 0)
            {
                foreach (DataRow dr in param)
                {
                    int Id = Int32.Parse(dr["Id_Parametro"].ToString());
                    string Dimension = dr["Dimension"].ToString();
                    string NomMenu = Dimension.Replace(" ", "");
                    string TablaCarga = dr["Tabla_Carga"].ToString();
                    string TipoControl = dr["Tipo_Control"].ToString().ToLower();
                    string NombreCampo = dr["Nombre_Campo"].ToString();
                    string Query = dr["Query_Dinamico"].ToString();

                    parametros += "<li class='list-group-item'><a href='#menu" + NomMenu + "' class='name' data-toggle='collapse' aria-expanded='true'>" + Dimension
                        + " <i class='fa fa-caret-down'></i></a><div class='collapse in' id='menu" + NomMenu + "' aria-expanded='true'>";
                    string tmp = "";

                    if (!Query.Equals(""))
                    {
                        Query = Query.Replace("_User_", usuario.idUsuario);
                        Query = Query.Replace("_Encuesta_", idRegistro);
                    }

                    if (TipoControl.Equals("textbox"))
                    {
                        parametros += "<input type='text' id='param"+Id+"' name='" + NombreCampo + "' value=''></div>";
                        List<string> aux = new List<string>();
                        dim.Add(new Dimension { nombreDimension = Dimension, idControl = Id, valores = aux, tipoControl = "textbox"});
                    }
                    else if (TipoControl.Equals("date"))
                    {
                        parametros += "<input type='text' value='' class='form-control' id='param" + Id + "' name='" + NombreCampo + "'></div>";
                        dates.Add(Id);
                        List<string> aux = new List<string>();
                        dim.Add(new Dimension { nombreDimension = Dimension, idControl = Id, valores = aux, tipoControl = "date" });
                    }
                    else if (TipoControl.Equals("textarea"))
                    {
                        parametros += "<textarea class='form-control' id='param"+Id+"' name='" + NombreCampo + "' rows='3'></textarea></div>";
                        List<string> aux = new List<string>();
                        dim.Add(new Dimension { nombreDimension = Dimension, idControl = Id, valores = aux, tipoControl = "textarea" });
                    }
                    else {
                        parametros += LoadParameters(tmp, TablaCarga, TipoControl, NombreCampo, Query, dim, Dimension, Id);
                        parametros += "</div>";
                    }
                    parametros += "</li>";
                }
            }
            System.Web.HttpContext.Current.Session["paramDates"] = dates;
            System.Web.HttpContext.Current.Session["paramSelects"] = selects;
            return parametros;
        }

        public string LoadParameters(string parametros, string tablaCarga, string tipoControl, string nombreCampo, string Query, List<Dimension> dim, string dimension, int idParam)
        {
            //string parametros = "";
            Conexion conexion = new Conexion();
            IEnumerable<DataRow> datos;
            string datasource = System.Web.HttpContext.Current.Session["dataSource"].ToString();

            //DataRow[] datos = conexion.SelectDataTable("select * from "+tablaCarga, Session["dataSource"].ToString()).Select();
            if (!Query.Equals(""))
            {
                datos = conexion.SelectDataTable(Query, datasource).Select();
                //IEnumerable<DataRow> row = datos.AsEnumerable().Where(x => condicion);
            }
            else
            {
                datos = conexion.SelectDataTable("select * from " + tablaCarga, datasource).Select();
            }
            //IEnumerable<DataRow> row = param.AsEnumerable().Where(x => x["Id_Menu"].ToString() == idMenu);
            if (datos.Count() > 0)
            {
                string nomPK = ValidateKey(tablaCarga, datasource);

                if (tipoControl.Contains("check"))
                {
                    if (datos.Count() > 1)
                    {
                        parametros += "<input type='checkbox' class='param"+idParam+"' name='" + nombreCampo + "' value='0' checked> Todas<br>";
                        List<string> aux = new List<string>();
                        aux.Add("Todas");
                        dim.Add(new Dimension { nombreDimension = dimension, idControl = idParam, nombrePK = nomPK, valores = aux, tipoControl = "checkbox" });
                    }
                    foreach (DataRow dr in datos)
                    {
                        var id = dr[nomPK];
                        string nombre = dr[nombreCampo].ToString();

                        if (datos.Count() == 1)
                        {
                            parametros += "<input type='checkbox' class='param"+idParam+"' name='" + nombreCampo + "' value='" + id + "' checked> " + nombre + "<br>";
                            List<string> aux = new List<string>();
                            aux.Add(id.ToString());
                            dim.Add(new Dimension { nombreDimension = dimension, idControl = idParam, nombrePK = nomPK, valores = aux, tipoControl = "checkbox" });
                        }
                        else
                        {
                            parametros += "<input type='checkbox' class='param"+idParam+"' name='" + nombreCampo + "' value='" + id + "'> " + nombre + "<br>";
                        }
                        //parametros += "<input type='checkbox' id='" + id + "'  value='" + id + "'>" + nombre;
                        //parametros += "<option value='" + id + "'>" + nombre + "</option>";
                        //dimensiones.Add(new Dimension { nombreTabla = tablaCarga, nombreColumna = nomPK, valorColumna = id.ToString(), tipoControl = tipoControl });
                    }
                }
                else if (tipoControl.Contains("drop"))
                {
                    parametros += "<select name='" + nombreCampo + "' class='form-control' id='param"+idParam+"'>";
                    selects.Add(idParam);
                    if (datos.Count() > 1)
                    {
                        parametros += "<option value='0' selected>Todas</option>";
                        List<string> aux = new List<string>();
                        aux.Add("Todas");
                        dim.Add(new Dimension { nombreDimension = dimension, idControl = idParam, nombrePK = nomPK, valores = aux, tipoControl = "select" });
                    }
                    foreach (DataRow dr in datos)
                    {
                        //string pk = ValidateKey(tablaCarga);
                        var id = dr[nomPK];
                        string nombre = dr[nombreCampo].ToString();

                        if (datos.Count() == 1)
                        {
                            parametros += "<option value='" + id + "' selected>" + nombre + "</option>";
                            List<string> aux = new List<string>();
                            aux.Add(id.ToString());
                            dim.Add(new Dimension { nombreDimension = dimension, idControl = idParam, nombrePK = nomPK, valores = aux, tipoControl = "select" });
                        }
                        else
                        {
                            parametros += "<option value='" + id + "'>" + nombre + "</option>";
                        }
                        //parametros += "<input type='checkbox' id='" + id + "'  value='" + id + "'>" + nombre; 
                        //dimensiones.Add(new Dimension { nombreTabla = tablaCarga, nombreColumna = nomPK, valorColumna = id.ToString(), tipoControl = tipoControl });
                    }
                    parametros += "</select>";
                }else if (tipoControl.Contains("radio"))
                {
                    //parametros += "<select name='" + nombreCampo + "' class='form-control' id='select2'>";
                    parametros += "<div class='radio'>";

                    if (datos.Count() > 1)
                    {
                        parametros += "<label><input type='radio' id='param"+idParam+"' name='"+nombreCampo+"' value='0' checked> Todas</label><br/>";
                        List<string> aux = new List<string>();
                        aux.Add("Todas");
                        dim.Add(new Dimension { nombreDimension = dimension, idControl = idParam, nombrePK = nomPK, valores = aux, tipoControl = "radiobutton" });
                    }

                    foreach (DataRow dr in datos)
                    {
                        //string pk = ValidateKey(tablaCarga);
                        var id = dr[nomPK];
                        string nombre = dr[nombreCampo].ToString();

                        if (datos.Count() == 1)
                        {
                            //parametros += "<option value='" + id + "'selected>" + nombre + "</option>";
                            parametros += "<label><input type='radio' id='param"+idParam+"' name='" + nombreCampo + "' value='" + id + "' checked> "+nombre+"</label>";
                            List<string> aux = new List<string>();
                            aux.Add(id.ToString());
                            dim.Add(new Dimension { nombreDimension = dimension, idControl = idParam, nombrePK = nomPK, valores = aux, tipoControl = "radiobutton" });
                        }
                        else
                        {
                            parametros += "<label><input type='radio' id='param"+idParam+"' name='" + nombreCampo + "' value='" + id + "'> " + nombre + "</label><br/>";
                        }
                    }
                    parametros += "</div>";
                }
            }
            return parametros;
        }

        //public List<DataRow> ValidateForeignKey(string tablaCarga)
        //{
        //    Conexion conexion = new Conexion();
        //    DataRow[] empr = conexion.SelectDataTable("SELECT CTU.TABLE_NAME, KCU.COLUMN_NAME[COLUMN], CTU2.TABLE_NAME[REFERENCED_TABLE], KCU2.COLUMN_NAME[REFERENCED_COLUMN], "
        //        +"CTU.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE CTU "
        //        +"JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU ON KCU.CONSTRAINT_NAME = CTU.CONSTRAINT_NAME AND KCU.CONSTRAINT_SCHEMA = CTU.CONSTRAINT_SCHEMA "
        //        +"JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON RC.CONSTRAINT_NAME = CTU.CONSTRAINT_NAME AND RC.CONSTRAINT_SCHEMA = CTU.CONSTRAINT_SCHEMA "
        //        +"JOIN INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE CTU2 ON CTU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME AND CTU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA "
        //        +"JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA", Session["dataSource"].ToString()).Select();
        //    List<DataRow> infoTabla = empr.AsEnumerable().Where(x => x["TABLE_NAME"].ToString() == tablaCarga && x["CONSTRAINT_NAME"].ToString().StartsWith("FK_")).ToList();

        //    return infoTabla;
        //}

        public string ValidateKey(string tablaCarga, string datasource)
        {
            string pk = "";
            Conexion conexion = new Conexion();
            DataRow[] infoTabla = conexion.SelectDataTable("select t.table_name, c.column_name, t.constraint_type from information_schema.table_constraints t "
            + "inner join information_schema.key_column_usage c on t.constraint_name = c.constraint_name where t.table_name = '" + tablaCarga + "' and t.constraint_type = 'PRIMARY KEY'", datasource).Select();
            //List<DataRow> infoTabla = empr.AsEnumerable().Where(x => x["TABLE_NAME"].ToString() == tablaCarga && x["CONSTRAINT_TYPE"].ToString() == "PRIMARY KEY").ToList();
            //foreach (DataRow dr in infoTabla)
            //{
                DataRow dr = infoTabla[0];
                pk = dr["COLUMN_NAME"].ToString();
            //}
            return pk;
        }
    }
}