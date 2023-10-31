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
        private DataTable _tablaTiempoPermanencia { get; set; } 
        private DataTable _tablaTipoAutomovil { get; set; } 
        private double _tiempoCombro { get; set; }
        //private TablasProbabilidades _instancia { get; set; }
        #endregion

        public TablasProbabilidades(List<double> probTipoAuto, List<string> tipoAuto, List<double> preciosTipoAuto, List<double> probPermanencia, List<double> tiempPerm, double tiempoCobro)
        {
            DataTable tablaTiempoPermanencia = setTablaTipoAuto(probTipoAuto, tipoAuto, preciosTipoAuto);
            DataTable tablaTipoAutomovil = setTablaPermanencia(probPermanencia, tiempPerm);

            if (this._tablaTiempoPermanencia == null || this._tablaTipoAutomovil == null)
            {
                this._tablaTiempoPermanencia = tablaTiempoPermanencia;
                this._tablaTipoAutomovil = tablaTipoAutomovil;
                this._tiempoCombro = tiempoCobro;
            }
        }

        public TablasProbabilidades()
        {
        }

        public double getTiempoCobro()
        {
            return this._tiempoCombro;
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
            double li = 0;
            double ls = 0;

            for (int i = 0; i < cantTipos; i++)
            {
                if (cantTipos == (i - 1)) //es ultima demanda
                {
                    valores.Rows.Add(tipoAutos[i], prob[i], li, 0.99999, precios[i]);
                }
                else
                {
                    ls += prob[i];
                    valores.Rows.Add(tipoAutos[i], prob[i], li, ls, precios[i]);
                    li = ls;
                }
                i++;
            }

            return valores;
        }

        public DataTable setTablaPermanencia(List<double> prob, List<double> tiempo)
        {
            DataTable valores = new DataTable();

            valores.Columns.Add("TIEMPO", typeof(int));
            valores.Columns.Add("PROBABILIDAD", typeof(double));
            valores.Columns.Add("LI", typeof(double));
            valores.Columns.Add("LS", typeof(double));

            int cantTiempos = tiempo.Count;
            double li = 0;
            double ls = 0;

            for (int i = 0; i < cantTiempos; i++)
            {
                if (cantTiempos == (i - 1)) //es ultima demanda
                {
                    valores.Rows.Add(tiempo[i], prob[i], li, 0.99999);
                }
                else
                {
                    ls += prob[i];
                    valores.Rows.Add(tiempo[i], prob[i], li, ls);
                    li = ls;
                }
                i++;
            }

            return valores;
        }

        public double buscarTiempoPermanencia(double rndTiempoPermanencia)
        {
            double tiempoPermanencia = 0;
            foreach (DataRow prob in this._tablaTiempoPermanencia.Rows)
            {
                if (rndTiempoPermanencia >= Convert.ToDouble(prob["LI"]) && rndTiempoPermanencia < Convert.ToDouble(prob["LS"]))
                {
                    tiempoPermanencia = Convert.ToInt32(prob["TIEMPO"]);
                    break;
                }
            }
            return tiempoPermanencia;
        }

        public (string,double) buscarTipoAuto(double rndTipoAuto)
        {
            string tipoAuto = "";
            double precio = 0;
            foreach (DataRow prob in this._tablaTiempoPermanencia.Rows)
            {
                if (rndTipoAuto >= Convert.ToDouble(prob["LI"]) && rndTipoAuto < Convert.ToDouble(prob["LS"]))
                {
                    tipoAuto = prob["TIPOAUTO"].ToString();
                    precio = Convert.ToInt32(prob["PRECIO"]);
                    break;
                }
            }
            return (tipoAuto,precio);
        }
    }
}
