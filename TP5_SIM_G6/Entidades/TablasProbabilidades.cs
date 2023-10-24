using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class TablasProbabilidades
    {
        #region Atributos
        private DataTable tablaTiempoPermanencia { get; set; } //instancia
        private DataTable tablaTipoAutomovil { get; set; } //instancia
        private TablasProbabilidades _instancia { get; set; }
        #endregion

        private TablasProbabilidades(DataTable tablaTiempoPermanencia, DataTable tablaTipoAutomovil)
        {
            this.tablaTiempoPermanencia = tablaTiempoPermanencia;
            this.tablaTipoAutomovil = tablaTipoAutomovil;
        }

        public TablasProbabilidades getTablasProbabilidades(DataTable tablaTiempoPermanencia, DataTable tablaTipoAutomovil)
        {
            if ((this.tablaTiempoPermanencia == null && tablaTiempoPermanencia != null) ||
                (this.tablaTipoAutomovil == null && tablaTipoAutomovil != null))
            {
                _instancia = new TablasProbabilidades(tablaTiempoPermanencia, tablaTipoAutomovil);
            }
            return _instancia;
        }
    }
}
