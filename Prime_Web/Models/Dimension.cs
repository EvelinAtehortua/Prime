using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prime_Web.Models
{
    public class Dimension
    {
        public string nombreDimension { set; get; }
        public int idControl { set; get; }
        public string nombrePK { set; get; }
        public List<string> valores { set; get; }
        public string tipoControl { set; get; }
    }
}