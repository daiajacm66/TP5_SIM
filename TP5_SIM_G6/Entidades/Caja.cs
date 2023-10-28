using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class Caja
    {
        #region Atributos
        private int id { get; set; }
        private string estado { get; set; }
        private double montoCobrar { get; set; }
        private double montoAC { get; set; }
        private double tiempoInicio { get; set; }
        private double tiempoFin { get; set; }
        private List<Automovil> cola { get; set; }
        private Caja _instancia { get; set; }
        #endregion

        private Caja(int id, string estado, double monto_cobrar, double monto_acumulado, double tiempo_inicio, double tiempo_fin, List<Automovil> cola)
        {
            this.id = id;
            this.estado = estado;
            this.montoCobrar = monto_cobrar;
            this.montoAC = monto_acumulado;
            this.tiempoInicio = tiempo_inicio;
            this.tiempoFin = tiempo_fin;
            this.cola = cola;
        }

        public static Caja getCaja(int id, string estado, double monto_cobrar, double monto_acumulado, double tiempo_inicio, double tiempo_fin, List<Automovil> cola)
        {
            if (this._instancia == null)
            {
                _instancia = new Caja(id, estado, monto_cobrar, monto_acumulado, tiempo_inicio, tiempo_fin, cola);
            }
            return _instancia;
        }

        public void agregarCola(Automovil automovil)
        {
            this.cola.Add(automovil);
        }
    }
}
