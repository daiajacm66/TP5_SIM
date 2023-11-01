using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TP5_SIM_G6.Entidades;
using static System.Collections.Specialized.BitVector32;

namespace TP5_SIM_G6.Logica
{
    public class Simulador
    {
        //bool DEBUG = true;
        public GeneradorAleatorio boxMullerGenerator { get; set; }
        public double clock { get; set; }
        public GeneradorAleatorio generator { get; set; }
        public double tiempoCobro { get; set; }

        public Simulador(double mediaExp, double constCobro)
        {
            this.generator = new GeneradorAleatorio(mediaExp);
            this.tiempoCobro = constCobro;
        }


        public IList<VectorEstadoMostrar> Simular(int cantidad, double tiempoMaximo, int desde, VectorEstadoMostrar anterior, TablasProbabilidades prob)
        {
            Dictionary<string, double> tiempos = new Dictionary<string, double>();
            IList<VectorEstadoMostrar> vectoresEstado = new List<VectorEstadoMostrar>();


            for (int i = 0; i < cantidad; i++)
            {
                if (this.clock <= anterior.reloj)
                {
                    VectorEstadoMostrar actual = new VectorEstadoMostrar();

                    // Se arma diccionario con todos los tiempos del vector para determinar el menor, es decir, el siguiente evento
                    tiempos.Clear();
                    if (anterior.tiempoProximaLlegada != 0) 
                        tiempos.Add("tiempoProximaLlegada", anterior.tiempoProximaLlegada);
                    for (int s = 0; s < anterior.sectores.Count(); s++)
                    {
                        if (anterior.sectores[s].finOcupacion != 0)
                            tiempos.Add("tiempoFinOcupacion_" + (s + 1).ToString(), anterior.sectores[s].finOcupacion);
                    }
                    if (anterior.caja.tiempoFinCobro != 0) { 
                        string autoEnCobro = "";
                        foreach (var auto in anterior.clientes)
                        {
                            if (auto.estado == "EnCaja") autoEnCobro = auto.id.ToString();
                        }
                        
                        tiempos.Add("tiempoFinCobro_" + autoEnCobro, anterior.caja.tiempoFinCobro);
                    }

                    // Obtiene diccionario de tiempos ordenado segun menor tiempo
                    var tiemposOrdenados = tiempos.OrderBy(obj => obj.Value).ToDictionary(obj => obj.Key, obj => obj.Value);
                    KeyValuePair<string, double> menorTiempo = tiemposOrdenados.First();

                    // Controlamos que los aviones en tierra sean menores a 30, si lo son, pasamos al siguiente menor tiempo, es decir, el siguiente evento
                    if (menorTiempo.Key == "tiempoProximaLlegada" && (GetCantidadSectoresOcupados(anterior) == 8))
                    {
                        tiemposOrdenados.Remove(tiemposOrdenados.First().Key);
                        menorTiempo = tiemposOrdenados.First();
                    }

                    // Se crea nuevo vectorEstado segun el evento siguiente determinado
                    switch (menorTiempo.Key)
                    {
                        case "tiempoProximaLlegada":
                            actual = CrearEventoLlegadaAuto(anterior, menorTiempo.Value, prob);
                            break;
                        case var val when new Regex(@"tiempoFinOcupacion_*").IsMatch(val):
                            int autoFO = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                            actual = CrearEventoFinEstacionamiento(anterior, menorTiempo.Value, autoFO, prob);
                            break;
                        case var someVal when new Regex(@"tiempoFinCobro_*").IsMatch(someVal):
                            int autoEC = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                            actual = CrearEventoFinCobro(anterior, menorTiempo.Value, autoEC);
                            break;
                    }

                    actual.nroSimulacion = i + 1;

                    if (i >= desde - 1 && i <= desde + 99 || i == (cantidad - 1))
                    {
                        if (desde == 0 && i == 0)
                            vectoresEstado.Add(anterior);
                        vectoresEstado.Add(actual);
                    }

                    anterior = actual;
                }
                else
                {
                    break;
                }

            }
            return vectoresEstado;

        }

        private int GetCantidadSectoresOcupados(VectorEstadoMostrar vector)
        {
            int contador = 0;
            for (int i = 0; i < vector.sectores.Count; i++)
            {
                if (vector.sectores[i].estado == "Ocupado")
                    contador += 1;
            }
            return contador;
        }

        private VectorEstadoMostrar CrearEventoLlegadaAuto(VectorEstadoMostrar anterior, double tiempoProximoEvento, TablasProbabilidades probabilidades)
        {
            Automovil a = new Automovil();

            //Automovil autoNuevo = new Automovil();
            VectorEstadoMostrar nuevo = new VectorEstadoMostrar();
            VectorEstadoMostrar _anterior = anterior;
            nuevo = arrastrarVariablesEst(_anterior);
            nuevo.evento = "Llegada Automovil (" + a.cantidad.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;


            // Se arrastran variables estadísticas.

            // Calcular siguiente tiempo de llegada de prox automovil
            Random r = new Random();
            nuevo.rndLlegadaAutomovil = r.NextDouble();
            nuevo.tiempoLlegadaAutomovil = this.generator.Generate(nuevo.rndLlegadaAutomovil);
            nuevo.tiempoProximaLlegada = nuevo.tiempoLlegadaAutomovil + nuevo.reloj;

            nuevo.contadorSectoresOcupados += 1;
            if (_anterior.nroSimulacion == 1) 
                nuevo.tiempoOcupacion = 0;
            else 
                nuevo.tiempoOcupacion = nuevo.reloj - _anterior.reloj;
            
            nuevo.tiempoOcupacionAC = _anterior.tiempoOcupacionAC + nuevo.tiempoOcupacion;

            nuevo.rndTiempoPermanencia = r.NextDouble();
            nuevo.tiempoPermanencia = probabilidades.buscarTiempoPermanencia(nuevo.rndTiempoPermanencia);
            nuevo.sectores = _anterior.sectores;

            for (int i = 0; i < _anterior.sectores.Count; i++)
            {
                if (_anterior.sectores[i].estado == "Libre")
                {
                    nuevo.sectores[i].estado = "Ocupado"; 
                    nuevo.sectores[i].finOcupacion = nuevo.tiempoPermanencia + nuevo.reloj;
                    break;
                }
            }

            nuevo.rndTipoAuto = r.NextDouble();
            double montoACobrar;
            (nuevo.tipoAuto, montoACobrar) = probabilidades.buscarTipoAuto(nuevo.rndTipoAuto);
            montoACobrar = (montoACobrar * (nuevo.tiempoPermanencia / 60)); //minutos
            nuevo.caja.montoAC = _anterior.caja.montoAC + nuevo.caja.montoCobrar;
            nuevo.caja.estado = "Libre";
            nuevo.caja.tiempoCobro = probabilidades.getTiempoCobro();
            nuevo.caja.tiempoFinCobro = nuevo.caja.tiempoCobro + nuevo.reloj;
            nuevo.caja.cola = _anterior.caja.cola;
            nuevo.caja.conteoCola = _anterior.caja.conteoCola;

            //Creamos instancia de cliente
            nuevo.clientes = new List<Automovil>();
            foreach (Automovil autoAnterior in _anterior.clientes)
            {
                Automovil aux = new Automovil()
                {
                    estado = autoAnterior.estado,
                    id = autoAnterior.id,
                    tipoAuto = autoAnterior.tipoAuto,
                    tiempoPermanencia = autoAnterior.tiempoPermanencia,
                    tiempoLlegada = autoAnterior.tiempoLlegada,
                    tiempoFinPermanencia = autoAnterior.tiempoFinPermanencia,
                    tiempoFinCobro = autoAnterior.tiempoFinCobro,
                    disabled = autoAnterior.disabled,
                    montoACobrar = montoACobrar
                };
                aux.incrementarCantidad();
                nuevo.clientes.Add(aux);
            }
            nuevo.clientes.Add(a);

            return nuevo;
        }


        private VectorEstadoMostrar CrearEventoFinEstacionamiento(VectorEstadoMostrar anterior, double tiempoProximoEvento, int auto, TablasProbabilidades probabilidades)
        {
            VectorEstadoMostrar nuevo = new VectorEstadoMostrar();
            VectorEstadoMostrar _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin Estacionamiento (" + auto.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;
            nuevo.clientes = _anterior.clientes;
            //nuevo.clientes = new List<Automovil>();
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;
            // Se arrastran variables estadísticas

            foreach (Automovil autoAnterior in _anterior.clientes)
            {
                Automovil aux = new Automovil()
                {
                    estado = autoAnterior.estado,
                    id = autoAnterior.id,
                    tipoAuto = autoAnterior.tipoAuto,
                    tiempoPermanencia = autoAnterior.tiempoPermanencia,
                    tiempoLlegada = autoAnterior.tiempoLlegada,
                    tiempoFinPermanencia = autoAnterior.tiempoFinPermanencia,
                    tiempoFinCobro = autoAnterior.tiempoFinCobro,
                    montoACobrar = autoAnterior.montoACobrar
                };
                nuevo.clientes.Add(aux);
            }

            Automovil autoFO = new Automovil();
            autoFO = nuevo.clientes[auto - 1];
            //nuevo.clientes[auto - 1].tiempoFinPermanencia = 0;
            //nuevo.clientes[auto - 1].estado = "EnCaja";
            //nuevo.clientes[auto - 1].disabled = false;


            if (anterior.caja.estado == "Libre")
            {
                foreach (Sector sector in anterior.sectores)
                {
                    if(sector.estado == "Ocupado" && sector.finOcupacion == tiempoProximoEvento)
                    {
                        int idSector = sector.id;
                        nuevo.sectores = anterior.sectores;
                        nuevo.sectores[idSector - 1].estado = "Libre";
                        nuevo.contadorSectoresOcupados = anterior.contadorSectoresOcupados - 1;
                        nuevo.tiempoOcupacion = nuevo.reloj - anterior.reloj;
                        nuevo.tiempoOcupacionAC = anterior.tiempoOcupacionAC + nuevo.tiempoOcupacion;

                        nuevo.caja = anterior.caja;
                        nuevo.caja.estado = "Ocupado";
                        nuevo.caja.tiempoCobro = probabilidades.getTiempoCobro();
                        nuevo.caja.tiempoFinCobro = nuevo.caja.tiempoCobro + nuevo.reloj;
                        nuevo.caja.montoCobrar = autoFO.montoACobrar;
                        nuevo.caja.montoAC += nuevo.caja.montoCobrar;

                        nuevo.clientes = anterior.clientes;
                        nuevo.clientes[auto - 1].estado = "En Caja";
                    }
                }
            }
            else
            {
                foreach (Sector sector in anterior.sectores)
                {
                    if (sector.estado == "Ocupado" && sector.finOcupacion == tiempoProximoEvento)
                    {
                        int idSector = sector.id;
                        nuevo.sectores = anterior.sectores;
                        
                        //nuevo.contadorSectoresOcupados = anterior.contadorSectoresOcupados - 1;
                        nuevo.tiempoOcupacion = nuevo.reloj - anterior.reloj;
                        nuevo.tiempoOcupacionAC = anterior.tiempoOcupacionAC + nuevo.tiempoOcupacion;

                        nuevo.caja = anterior.caja;
                        nuevo.caja.estado = "Ocupado";
                        nuevo.caja.tiempoCobro = 0;
                        nuevo.caja.montoCobrar = 0;
                        nuevo.caja.cola.Add(autoFO);
                        nuevo.caja.conteoCola += 1;

                        nuevo.sectores[idSector - 1].finOcupacion = anterior.sectores[idSector - 1].finOcupacion + (nuevo.caja.tiempoFinCobro - nuevo.reloj) + anterior.caja.conteoCola * probabilidades.getTiempoCobro();

                        nuevo.clientes = anterior.clientes;
                        nuevo.clientes[auto - 1].estado = "Espera";
                    }
                }
            }

            return nuevo;
        }

        private VectorEstadoMostrar CrearEventoFinCobro(VectorEstadoMostrar anterior, double tiempoProximoEvento, int auto)
        {
            VectorEstadoMostrar nuevo = new VectorEstadoMostrar();
            VectorEstadoMostrar _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin cobro (" + auto.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;
            // Se arrastran variables estadÃ­sticas
            foreach (Automovil autoAnterior in _anterior.clientes)
            {
                Automovil aux = new Automovil()
                {
                    // ESTE SE TIENE QUE DESHABILITAR EL OBJETO AUTO
                    estado = "Destruccion",
                    id = autoAnterior.id,
                    tipoAuto = autoAnterior.tipoAuto,
                    tiempoPermanencia = autoAnterior.tiempoPermanencia,
                    tiempoLlegada = autoAnterior.tiempoLlegada,
                    tiempoFinPermanencia = autoAnterior.tiempoFinPermanencia,
                    tiempoFinCobro = autoAnterior.tiempoFinCobro,
                    montoACobrar = autoAnterior.montoACobrar
                };
                nuevo.clientes.Add(aux);
            }

            Automovil autoFO = new Automovil();
            autoFO = nuevo.clientes[auto - 1];
            nuevo.clientes[auto - 1].disabled = true;


            // si hay alguien en la cola
            if (anterior.caja.conteoCola != 0)
            {
                nuevo.caja.conteoCola -= 1;
                nuevo.clientes = anterior.clientes;
                nuevo.clientes[auto - 1].estado = "En Caja";

                foreach (Sector sector in anterior.sectores)
                {
                    // hay que liberar el sector del auto que estaba en la cola
                    if (sector.estado == "Ocupado" && sector.finOcupacion == tiempoProximoEvento)
                    {
                        int idSector = sector.id;
                        nuevo.sectores = anterior.sectores;
                        nuevo.sectores[idSector - 1].estado = "Libre";
                        nuevo.contadorSectoresOcupados = anterior.contadorSectoresOcupados - 1;
                        nuevo.tiempoOcupacion = nuevo.reloj - anterior.reloj;
                        nuevo.tiempoOcupacionAC = anterior.tiempoOcupacionAC + nuevo.tiempoOcupacion;
                    }

                }
            }
            else
            {
                // si no hay cola se libera la caja
                nuevo.caja = anterior.caja;
                nuevo.caja.estado = "Libre";
                nuevo.caja.cola = anterior.caja.cola;
                nuevo.caja.montoAC = anterior.caja.montoCobrar;

            }

            return nuevo;
        }


        private VectorEstadoMostrar arrastrarVariablesEst(VectorEstadoMostrar _anterior)
        {
            VectorEstadoMostrar nuevo = new VectorEstadoMostrar();

            nuevo.tiempoOcupacionAC = _anterior.tiempoOcupacionAC;
            nuevo.caja = _anterior.caja;


            //nuevo.maxEEVTime = _anterior.maxEEVTime;
            //nuevo.maxEETTime = _anterior.maxEETTime;
            //nuevo.porcAvionesAyDInst = _anterior.porcAvionesAyDInst;
            //nuevo.cantAvionesAyDInst = _anterior.cantAvionesAyDInst;
            //nuevo.acumEETTime = _anterior.acumEETTime;
            //nuevo.acumEEVTime = _anterior.acumEEVTime;
            //nuevo.avgEETTime = _anterior.avgEETTime;
            //nuevo.avgEEVTime = _anterior.avgEEVTime;

            return nuevo;
        }

    }
}
