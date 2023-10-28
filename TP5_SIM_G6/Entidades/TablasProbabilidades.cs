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

        public TablasProbabilidades getTablasProbabilidades(List<double> probTipoAuto, List<string> tipoAuto, List<double> preciosTipoAuto, List<double> probPermanencia, List<double> tiempPerm)
        {
            DataTable tablaTiempoPermanencia = setTablaTipoAuto(probTipoAuto, tipoAuto, preciosTipoAuto);
            DataTable tablaTipoAutomovil = setTablaPermanencia(probPermanencia, tiempPerm);

            if ((this.tablaTiempoPermanencia == null && tablaTiempoPermanencia != null) ||
                (this.tablaTipoAutomovil == null && tablaTipoAutomovil != null))
            {
                _instancia = new TablasProbabilidades(tablaTiempoPermanencia, tablaTipoAutomovil);
            }
            return _instancia;
        }

        public DataTable setTablaTipoAuto(List<double> prob, List<string> tipoAutos, List<double> precios)
        {
            DataTable valores = new DataTable();

            valores.Columns.Add("TIPOAUTO", typeof(int));
            valores.Columns.Add("PROBABILIDAD", typeof(double));
            valores.Columns.Add("LI", typeof(double));
            valores.Columns.Add("LS", typeof(double));
            valores.Columns.Add("PRECIO", typeof(double));

            int cantTipos = tipoAutos.Count;

            //CAMBIAR A FOR
            foreach (string tipo in tipoAutos)
            {
                if (cantTipos == (i - 1)) //es ultima demanda
                {
                    valores.Rows.Add(demanda, probabilidades[i], li, 0.99999, precios[i]);
                }
                else
                {
                    ls += probabilidades[i];
                    valores.Rows.Add(demanda, probabilidades[i], li, ls, precios[i]);
                    li = ls;
                }
                i++;
            }

            return valores;
        }

        public DataTable setTablaPermanencia(List<double> prob, List<double> tiempo)
        {

        }

        internal static double buscarTiempoPermanencia(Random r)
        {
            throw new NotImplementedException();
        }
    }
}
