using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class VectorEstadoMostrar
    {
        #region Atributos
        public int nroSimulacion { get; set; }
        public string evento { get; set; }
        public double reloj { get; set; }
        public double rndLlegadaAutomovil { get; set; }
        public double tiempoLlegadaAutomovil { get; set; }
        public double tiempoProximaLlegada { get; set; }
        public int contadorSectoresOcupados { get; set; }
        public double tiempoOcupacion { get; set; }
        public double tiempoOcupacionAC { get; set; }
        public double rndTiempoPermanencia { get; set; }
        public double tiempoPermanencia { get; set; }
        public Sector sector1 { get; set; }
        //public string estadoS1 { get; set; }
        //public double finOcupacionS1 { get; set; }
        public Sector sector2 { get; set; }
        //public string estadoS2 { get; set; }
        //public double finOcupacionS2 { get; set; }
        public Sector sector3 { get; set; }
        //public string estadoS3 { get; set; }
        //public double finOcupacionS3 { get; set; }
        public Sector sector4 { get; set; }
        //public string estadoS4 { get; set; }
        //public double finOcupacionS4 { get; set; }
        public Sector sector5 { get; set; }
        //public string estadoS5 { get; set; }
        //public double finOcupacionS5 { get; set; }
        public Sector sector6 { get; set; }
        //public string estadoS6 { get; set; }
        //public double finOcupacionS6 { get; set; }
        public Sector sector7 { get; set; }
        //public string estadoS7 { get; set; }
        //public double finOcupacionS7 { get; set; }
        public Sector sector8 { get; set; }
        //public string estadoS8 { get; set; }
        //public double finOcupacionS8 { get; set; }
        //public double rndTipoAutomovil { get; set; }
        //public string tipoAutomovil { get; set; }
        public double rndTipoAuto { get;set;}
        public string tipoAuto { get; set; }
        public Caja caja { get; set; }
        //public string estadoCobro { get; set; }
        //public double tiempoCobro { get; set; }
        //public double finCobro { get; set; }
        //public double montoCobrar { get; set; }
        //public double montoCobrarAC { get; set; }
        //public int colaCobro { get; set; }
        public Automovil A1 { get; set; }
        //public string estadoA1 { get; set; }
        //public string tipoA1 { get; set; }
        //public double tiempoA1 { get; set; }
        public Automovil A2 { get; set; }
        //public string estadoA2 { get; set; }
        //public string tipoA2 { get; set; }
        //public double tiempoA2 { get; set; }
        public Automovil A3 { get; set; }
        //public string estadoA3 { get; set; }
        //public string tipoA3 { get; set; }
        //public double tiempoA3 { get; set; }
        public Automovil A4 { get; set; }
        //public string estadoA4 { get; set; }
        //public string tipoA4 { get; set; }
        //public double tiempoA4 { get; set; }
        public Automovil A5 { get; set; }
        //public string estadoA5 { get; set; }
        //public string tipoA5 { get; set; }
        //public double tiempoA5 { get; set; }
        public Automovil A6 { get; set; }
        //public string estadoA6 { get; set; }
        //public string tipoA6 { get; set; }
        //public double tiempoA6 { get; set; }
        public Automovil A7 { get; set; }
        //public string estadoA7 { get; set; }
        //public string tipoA7 { get; set; }
        //public double tiempoA7 { get; set; }
        public Automovil A8 { get; set; }
        //public string estadoA8 { get; set; }
        //public string tipoA8 { get; set; }
        //public double tiempoA8 { get; set; }
        public Automovil A9 { get; set; }
        //public string estadoA9 { get; set; }
        //public string tipoA9 { get; set; }
        //public double tiempoA9 { get; set; }
        public List<Automovil> clientes { get; set; }
        public List<Sector> sectores { get; set; }
        #endregion


        public int getCantidadClientes()
        {
            return this.clientes.Count();
        }
    }
}