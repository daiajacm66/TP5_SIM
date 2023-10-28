using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class Sector
    {
        #region Atributos
        private int id { get; set; }
        private string estado { get; set; }
        private double finOcupacion { get; set; }
        #endregion

        #region Constructores
        public Sector(int id, string estado, double finOcupacion = 0)
        {
            this.id = id;
            this.estado = estado;
            this.finOcupacion = finOcupacion;
        }
        #endregion
    }
}
