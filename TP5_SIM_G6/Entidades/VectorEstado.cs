using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class VectorEstado
    {
        #region Atributos
        public int nroSimulacion { get; set; }
        public string evento { get; set; }
        public double reloj { get; set; }
        public Sector sector1 { get; set; }
        public Sector sector2 { get; set; }
        public Sector sector3 { get; set; }
        public Sector sector4 { get; set; }
        public Sector sector5 { get; set; }
        public Sector sector6 { get; set; }
        public Sector sector7 { get; set; }
        public Sector sector8 { get; set; }
        public Caja caja { get; set; }
        public Automovil Automovil1 { get; set; }
        public Automovil Automovil2 { get; set; }
        public Automovil Automovil3 { get; set; }
        public Automovil Automovil4 { get; set; }
        public Automovil Automovil5 { get; set; }
        public Automovil Automovil6 { get; set; }
        public Automovil Automovil7 { get; set; }
        public Automovil Automovil8 { get; set; }
        public Automovil Automovil9 { get; set; }
        #endregion
    }
}
