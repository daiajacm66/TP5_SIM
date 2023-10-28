﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TP5_SIM_G6.Logica;
using TP5_SIM_G6.Entidades;

namespace TP5_SIM_G6
{
    public partial class SimuladorCola : Form
    {
        public double clock = 0;

        public SimuladorCola()
        {
            InitializeComponent();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            dgvSimulacion.Rows.Clear();
            txtPorcentajeUtilizacion.Text = "";
            txtRecaudacionTotal.Text = "";

        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            List<double> probTipoAuto = new List<double> { double.Parse(nudProbPequeño.Text.ToString()), double.Parse(nudProbGrande.Text.ToString()), double.Parse(nudProbUtilitario.Text.ToString()) };
            List<string> tipoAuto = new List<string> { "Pequeño", "Grande", "Utilitario" };
            List<double> preciosTipoAuto = new List<double> { 300, 500, 1000 };
            List<double> probPermanencia = new List<double> { double.Parse(nudProb1hora.Text.ToString()), double.Parse(nudProb2horas.Text.ToString()), double.Parse(nudProb3horas.Text.ToString()), double.Parse(nudProb4horas.Text.ToString()) };
            List<double> tiempPerm = new List<double> { 1, 2, 3, 4 };

            TablasProbabilidades prob = new TablasProbabilidades(probTipoAuto, tipoAuto, preciosTipoAuto, probPermanencia, tiempPerm);

            //Simulador sim = new Simulador();
            if (!validarInputs())
            {
                MessageBox.Show("Debe completar todos los campos antes de continuar", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.dgvSimulacion.Rows.Clear();
            Automovil.cantidad = 0;

            int tiempoMaximo = Convert.ToInt32(this.nudX.Text);
            int cantMaxima = Convert.ToInt32(this.nudCantFilas.Text);
            int desde = Convert.ToInt32(this.nudDesde.Text);
            int hasta = Convert.ToInt32(this.nudHasta.Text);
            
            if (hasta > cantMaxima)
                hasta = cantMaxima;
            //this.txtTo.Text = to.ToString();
            GeneradorAleatorio generadorAlea = new GeneradorAleatorio(Convert.ToDouble(this.nudIndiceLlegadas.Text));
            Random r = new Random();
            double rnd = r.NextDouble();
            double proxAutomovil = generadorAlea.Generate(rnd);

            VectorEstadoMostrar initialize = new VectorEstadoMostrar()
            {
                evento = "Inicio",
                reloj = clock,
                rndLlegadaAutomovil = rnd,
                tiempoLlegadaAutomovil = proxAutomovil,
                tiempoProximaLlegada = clock + proxAutomovil,
                contadorSectoresOcupados = 0,
                tiempoOcupacionAC = 0,
                sector1 = new Sector(1, "Libre"),
                sector2 = new Sector(2, "Libre"),
                sector3 = new Sector(3, "Libre"),
                sector4 = new Sector(4, "Libre"),
                sector5 = new Sector(5, "Libre"),
                sector6 = new Sector(6, "Libre"),
                sector7 = new Sector(7, "Libre"),
                sector8 = new Sector(8, "Libre"),
                caja = new Caja(1, "Libre", 0, 0, 0, 0, new List<Automovil>()),
                clientes = new List<Automovil>()

            };

            Simulador simulador = new Simulador(Convert.ToDouble(this.nudIndiceLlegadas.Text), Convert.ToDouble(this.nudTiempoCobro.Text));
            IList<VectorEstadoMostrar> filasAMostrar = simulador.simulate(cantMaxima, tiempoMaximo, desde, initialize);

            //this.txtMaxTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEETTime).ToString();
            //this.txtMaxTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEEVTime).ToString();
            //this.txtAvgTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEETTime).ToString();
            //this.txtAvgTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEEVTime).ToString();
            //this.txtPorcAyDInstant.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].porcAvionesAyDInst).ToString();

            if (to != filasAMostrar.Last().iterationNum)
                filasAMostrar.Remove(filasAMostrar.Last());

            int columnaInicial = 0;
            int columnaFinal = 0;
            for (int a = 0; a < filasAMostrar[0].clientes.Count; a++)
            {
                if (!filasAMostrar[0].clientes[a].disabled)
                {
                    columnaInicial = filasAMostrar[0].clientes[a].id;
                    break;
                    //Console.WriteLine(columnaInicial);
                }
            }
            columnaFinal = filasAMostrar.Last().clientes.Last().id;

            for (int i = 0; i < filasAMostrar.Count; i++)
            {
                string estadoPista = filasAMostrar[i].pista.libre ? "Libre" : "Ocupada";

                // Manejo de columnas
                if (i == 0)
                {
                    this.dgvResults.ColumnCount = 23;
                }
            }
        }

        //private List<Automovil> getAutomovilesEstacionados()
        //{
        //    List<Automovil> result = new List<Automovil>();

        //    if (this.cmbParkedPlanes.SelectedIndex == 0)
        //    {
        //        Automovil.cantidad += 1;
        //        result.Add(new Automovil() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP", disabled = false });
        //        return result;
        //    }
        //    else if (this.cmbParkedPlanes.SelectedIndex == 1)
        //    {
        //        Avion.count += 1;
        //        result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP", disabled = false });
        //        Avion.count += 1;
        //        result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime2.Text), estado = "EP", disabled = false });
        //        return result;
        //    }
        //    else if (this.cmbParkedPlanes.SelectedIndex == 2)
        //    {
        //        Avion.count += 1;
        //        result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime1.Text), estado = "EP", disabled = false });
        //        Avion.count += 1;
        //        result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime2.Text), estado = "EP", disabled = false });
        //        Avion.count += 1;
        //        result.Add(new Avion() { tiempoPermanencia = Convert.ToDouble(this.txtParkingTime3.Text), estado = "EP", disabled = false });
        //        return result;
        //    }
        //    return result;
        //}

        private bool validarInputs()
        {
            //validar inputs
            return true;
        }
    }
}