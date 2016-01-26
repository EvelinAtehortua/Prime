using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prime_Web.Models
{
    public class Usuario
    {
        public string idUsuario { set; get; }
        public string clave { set; get; }
        public string nomUsuario { set; get; }
        public string nomOrganizacion { set; get; }
        public string nitOrganizacion { set; get; }
    }
}