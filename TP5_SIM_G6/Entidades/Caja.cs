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
        private double monto_cobrar { get; set; }
        private double monto_acumulado { get; set; }
        private DateTime tiempo_inicio { get; set; }
        private DateTime tiempo_fin { get; set; }
        private List<Automovil> cola { get; set; }
        private Caja _instancia { get; set; }
        #endregion

        private Caja(int id, string estado, double monto_cobrar, double monto_acumulado, DateTime tiempo_inicio, DateTime tiempo_fin, List<Automovil> cola)
        {
            this.id = id;
            this.estado = estado;
            this.monto_cobrar = monto_cobrar;
            this.monto_acumulado = monto_acumulado;
            this.tiempo_inicio = tiempo_inicio;
            this.tiempo_fin = tiempo_fin;
            this.cola = cola;
        }

        public Caja getCaja(int id, string estado, double monto_cobrar, double monto_acumulado, DateTime tiempo_inicio, DateTime tiempo_fin, List<Automovil> cola)
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
