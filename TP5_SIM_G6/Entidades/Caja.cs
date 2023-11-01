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
        public int id { get; set; }
        public string estado { get; set; }
        public double montoCobrar { get; set; }
        public double montoAC { get; set; }
        public double tiempoCobro { get; set; }
        public double tiempoFinCobro { get; set; }
        public List<Automovil> cola { get; set; }
        public int conteoCola { get; set; }
        #endregion

        public Caja(int id, string estado, double monto_cobrar, double monto_acumulado, double tiempo_inicio, double tiempo_fin, List<Automovil> cola)
        {
            this.id = id;
            this.estado = estado;
            this.montoCobrar = monto_cobrar;
            this.montoAC = monto_acumulado;
            this.tiempoCobro = tiempo_inicio;
            this.tiempoFinCobro = tiempo_fin;
            this.cola = cola;
            this.conteoCola = cola.Count;
        }

        

        public void agregarCola(Automovil automovil)
        {
            this.cola.Add(automovil);
        }
    }
}
