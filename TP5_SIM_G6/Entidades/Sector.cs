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
        private string tipoAutomovil { get; set; }
        private double tiempoPermanencia { get; set; }
        #endregion

        #region Constructores
        public Sector(int id, string estado, string tipoAutomovil, double tiempoPermanencia)
        {
            this.id = id;
            this.estado = estado;
            this.tipoAutomovil = tipoAutomovil;
            this.tiempoPermanencia = tiempoPermanencia;
        }
        #endregion
    }
}
