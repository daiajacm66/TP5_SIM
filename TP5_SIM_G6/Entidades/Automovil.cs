using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class Automovil
    {
        #region Atributos
        public int id { get; set; }
        public string estado { get; set; }
        public string tipoAuto { get; set; }
        public double tiempoPermanencia { get; set; }

        public double tiempoLlegada { get; set; }
        public double tiempoFinPermanencia { get; set; }
        public double tiempoFinCobro { get; set; }

        public static int cantidad { get; set; }
        #endregion
    }
}
