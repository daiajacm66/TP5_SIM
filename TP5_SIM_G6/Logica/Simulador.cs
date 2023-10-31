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


        public IList<VectorEstadoMostrar> simulate(int cantidad, double tiempoMaximo, int desde, VectorEstadoMostrar anterior)
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
                    for (int s = 0; s < anterior.sectores.Count; s++)
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
                            actual = CrearEventoLlegadaAuto(anterior, menorTiempo.Value);
                            break;
                        case var val when new Regex(@"tiempoFinOcupacion_*").IsMatch(val):
                            int autoFO = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                            actual = CrearEventoFinEstacionamiento(anterior, menorTiempo.Value, autoFO);
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
                            stateRows.Add(anterior);
                        stateRows.Add(actual);
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

        private VectorEstadoMostrar CrearEventoLlegadaAuto(VectorEstadoMostrar anterior, double tiempoProximoEvento)
        {
            Automovil a = new Automovil();
            
            TablasProbabilidades probabilidades = new TablasProbabilidades();
            //Automovil autoNuevo = new Automovil();
            VectorEstadoMostrar nuevo = new VectorEstadoMostrar();
            VectorEstadoMostrar _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Llegada Automovil (" + a.cantidad.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Se arrastran variables estadísticas.

            // Calcular siguiente tiempo de llegada de prox automovil
            Random r = new Random();
            nuevo.rndLlegadaAutomovil = r.NextDouble();
            nuevo.tiempoLlegadaAutomovil = this.generator.Generate(nuevo.rndLlegadaAutomovil);
            nuevo.tiempoProximaLlegada = nuevo.tiempoLlegadaAutomovil + nuevo.reloj;

            nuevo.contadorSectoresOcupados += 1;
            if (anterior.nroSimulacion == 1) 
                nuevo.tiempoOcupacion = 0;
            else 
                nuevo.tiempoOcupacion = nuevo.reloj - anterior.reloj;
            
            nuevo.tiempoOcupacionAC = anterior.tiempoOcupacionAC + nuevo.tiempoOcupacion;

            nuevo.rndTiempoPermanencia = r.NextDouble();
            nuevo.tiempoPermanencia = probabilidades.buscarTiempoPermanencia(nuevo.rndTiempoPermanencia);

            for (int i = 0; i < anterior.sectores.Count; i++)
            {
                if (anterior.sectores[i].estado == "Libre")
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
            nuevo.caja.montoAC = anterior.caja.montoAC + nuevo.caja.montoCobrar;
            nuevo.caja.estado = "Libre";
            nuevo.caja.tiempoCobro = probabilidades.getTiempoCobro();
            nuevo.caja.tiempoFinCobro = nuevo.caja.tiempoCobro + nuevo.reloj;
            nuevo.caja.cola = anterior.caja.cola;
            nuevo.caja.conteoCola = anterior.caja.conteoCola;

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
            //nuevo.clientes.Add(autoNuevo);

            return nuevo;
        }


        private VectorEstadoMostrar CrearEventoFinEstacionamiento(VectorEstadoMostrar anterior, double tiempoProximoEvento, int auto)
        {
            TablasProbabilidades probabilidades = new TablasProbabilidades();

            VectorEstadoMostrar nuevo = new VectorEstadoMostrar();
            VectorEstadoMostrar _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin Estacionamiento (" + auto.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;
            nuevo.clientes = new List<Automovil>();
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


            if(anterior.caja.estado == "Libre")
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
                        //nuevo.sectores[idSector - 1].estado = "Libre";
                        //nuevo.contadorSectoresOcupados = anterior.contadorSectoresOcupados - 1;
                        nuevo.tiempoOcupacion = nuevo.reloj - anterior.reloj;
                        nuevo.tiempoOcupacionAC = anterior.tiempoOcupacionAC + nuevo.tiempoOcupacion;

                        nuevo.caja = anterior.caja;
                        nuevo.caja.estado = "Ocupado";
                        nuevo.caja.tiempoCobro = 0;
                        nuevo.caja.montoCobrar = 0;
                        nuevo.caja.cola.Add(autoFO);
                        nuevo.caja.conteoCola += 1;

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
            nuevo.evento = "Fin Aterrizaje (" + auto.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;
            // Se arrastran variables estadísticas


            nuevo.clientes = new List<Avion>();
            foreach (Avion avionAnterior in _anterior.clientes)
            {
                Avion aux = new Avion()
                {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    disabled = avionAnterior.disabled,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue,
                    tiempoEETin = avionAnterior.tiempoEETin,
                    tiempoEEVin = avionAnterior.tiempoEEVin,
                    instantLanding = avionAnterior.instantLanding
                };
                nuevo.clientes.Add(aux);
            }

            //Calcular variables tiempo permanencia
            double ac = 0;
            nuevo.tiempoDePermanencia = convolutionGenerator.Generate(out ac);
            nuevo.rndPermanencia = ac;
            nuevo.tiempoFinPermanencia = nuevo.reloj + nuevo.tiempoDePermanencia;
            nuevo.clientes[auto - 1].tiempoPermanencia = nuevo.tiempoFinPermanencia;
            nuevo.clientes[auto - 1].tiempoFinAterrizaje = 0;
            nuevo.clientes[auto - 1].estado = "EP";

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.colaEET = new Queue<Avion>();
            nuevo.pista.colaEEV = new Queue<Avion>();
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;

            if (nuevo.pista.colaEEV.Count != 0)
            {
                // Calculos variables aterrizaje
                Avion avionNuevo = nuevo.pista.colaEEV.Dequeue();
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                nuevo.pista.libre = false;
                nuevo.clientes[avionNuevo.id - 1].tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                nuevo.clientes[avionNuevo.id - 1].estado = "EA";

                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                if (nuevo.clientes[avionNuevo.id - 1].tiempoEEVin != 0)
                {
                    double eevTime = nuevo.reloj - nuevo.clientes[avionNuevo.id - 1].tiempoEEVin;
                    if (eevTime > nuevo.maxEEVTime) nuevo.maxEEVTime = eevTime;

                    nuevo.acumEEVTime += eevTime;
                }
            }
            else if (nuevo.pista.colaEET.Count != 0)
            {
                // Calculos variables de despegue
                Avion avionNuevo = nuevo.pista.colaEET.Dequeue();
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.pista.libre = false;
                nuevo.clientes[avionNuevo.id - 1].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.clientes[avionNuevo.id - 1].estado = "ED";

                if (nuevo.clientes[avionNuevo.id - 1].tiempoEETin != 0)
                {
                    double eetTime = nuevo.reloj - nuevo.clientes[avionNuevo.id - 1].tiempoEETin;
                    if (eetTime > nuevo.maxEETTime) nuevo.maxEETTime = eetTime;

                    nuevo.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevo.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(nuevo.clientes.Count)) * 100;
            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(nuevo.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(nuevo.clientes.Count);

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin permanencia (" + avion.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Se arrastran variables estadísticas



            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;

            // Calcular variables de aterrizaje
            nuevo.tiempoFinAterrizaje = _anterior.tiempoFinAterrizaje;

            nuevo.tiempoFinDeDespegue = _anterior.tiempoFinDeDespegue;

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.colaEET = new Queue<Avion>();
            nuevo.pista.colaEEV = new Queue<Avion>();
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;

            nuevo.clientes = new List<Avion>();
            foreach (Avion avionAnterior in _anterior.clientes)
            {
                Avion aux = new Avion()
                {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    disabled = avionAnterior.disabled,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue,
                    tiempoEETin = avionAnterior.tiempoEETin,
                    tiempoEEVin = avionAnterior.tiempoEEVin,
                    instantLanding = avionAnterior.instantLanding
                };
                nuevo.clientes.Add(aux);
            }


            if (!nuevo.pista.libre)
            {
                nuevo.clientes[avion - 1].estado = "EET";
                nuevo.pista.colaEET.Enqueue(nuevo.clientes[avion - 1]);
                nuevo.clientes[avion - 1].tiempoEETin = nuevo.reloj;
            }
            else
            {
                // Calcular variables de despegue
                nuevo.clientes[avion - 1].estado = "ED";
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.clientes[avion - 1].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.pista.libre = false;

                if (nuevo.clientes[avion - 1].instantLanding) nuevo.cantAvionesAyDInst++;
            }
            nuevo.clientes[avion - 1].tiempoPermanencia = 0;

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(nuevo.clientes.Count)) * 100;

            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(nuevo.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(nuevo.clientes.Count);

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

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
