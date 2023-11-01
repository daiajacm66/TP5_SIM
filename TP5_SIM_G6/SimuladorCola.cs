using System;
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
            double tiempoCobro = double.Parse(nudTiempoCobro.Text.ToString());

            TablasProbabilidades prob = new TablasProbabilidades(probTipoAuto, tipoAuto, preciosTipoAuto, probPermanencia, tiempPerm, tiempoCobro);
            Automovil automovil = new Automovil();

            //Simulador sim = new Simulador();
            if (!validarInputs())
            {
                MessageBox.Show("Debe completar todos los campos antes de continuar", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.dgvSimulacion.Rows.Clear();
            automovil.cantidad = 0;

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

                sectores = new List<Sector>(),

                caja = new Caja(1, "Libre", 0, 0, 0, 0, new List<Automovil>()),
                clientes = new List<Automovil>()

            };

            initialize.sectores.Add(new Sector(1, "Libre"));
            initialize.sectores.Add(new Sector(2, "Libre"));
            initialize.sectores.Add(new Sector(3, "Libre"));
            initialize.sectores.Add(new Sector(4, "Libre"));
            initialize.sectores.Add(new Sector(5, "Libre"));
            initialize.sectores.Add(new Sector(6, "Libre"));
            initialize.sectores.Add(new Sector(7, "Libre"));
            initialize.sectores.Add(new Sector(8, "Libre"));

            Simulador simulador = new Simulador(Convert.ToDouble(this.nudIndiceLlegadas.Text), Convert.ToDouble(this.nudTiempoCobro.Text));
            IList<VectorEstadoMostrar> filasAMostrar = simulador.Simular(cantMaxima, tiempoMaximo, desde, initialize, prob);

            //this.txtMaxTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEETTime).ToString();
            //this.txtMaxTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].maxEEVTime).ToString();
            //this.txtAvgTimeEET.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEETTime).ToString();
            //this.txtAvgTimeEEV.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].avgEEVTime).ToString();
            //this.txtPorcAyDInstant.Text = truncar(filasAMostrar[filasAMostrar.Count - 1].porcAvionesAyDInst).ToString();

            //for (int i = 0; i < Double.Parse(nudCantFilas.ToString()); i++)
            //{
            //    if (i >= desde)
            //    {
            //        dgvSimulacion.Rows.Add(i, filasAMostrar.cantClientes, filaActual.cantPastelitos, filaActual.stockPastelitos, filaActual.stockAC, filaActual.ingreso, filaActual.ingresoAC, filaActual.utilidad, filaActual.utilidadAC);
            //        //prbSimulacion.Value = d + 1;

            //        //if (d == n)
            //        //{
            //        //    grdSimUltimaFila.Rows.Add(d, filaActual.cantClientes, filaActual.cantPastelitos, filaActual.stockPastelitos, filaActual.stockAC, filaActual.ingreso, filaActual.ingresoAC, filaActual.utilidad, filaActual.utilidadAC);
            //        //}
            //    }
            //}
            /*  filaAnterior = filaActual*/
            ;

            if (desde != filasAMostrar.Last().nroSimulacion)
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
                //string estadoPista = filasAMostrar[i].pista.libre ? "Libre" : "Ocupada";

                // Manejo de columnas
                if (i == 0)
                {
                    this.dgvSimulacion.ColumnCount = 34;

                    this.dgvSimulacion.Columns[0].HeaderText = "N° SIM";
                    this.dgvSimulacion.Columns[1].HeaderText = "Evento";
                    this.dgvSimulacion.Columns[2].HeaderText = "Reloj";
                    this.dgvSimulacion.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[3].HeaderText = "RND";
                    this.dgvSimulacion
                        .Columns[4].HeaderText = "T. entre llegadas";
                    this.dgvSimulacion
                        .Columns[5].HeaderText = "T. prox. llegada";
                    this.dgvSimulacion
                        .Columns[3].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    this.dgvSimulacion
                        .Columns[4].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    this.dgvSimulacion
                        .Columns[5].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    this.dgvSimulacion
                        .Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[6].HeaderText = "Sectores Ocupados";
                    this.dgvSimulacion
                        .Columns[7].HeaderText = "T. ocupacion";
                    this.dgvSimulacion
                        .Columns[8].HeaderText = "AC T. ocupacion";
                    this.dgvSimulacion
                        .Columns[6].DefaultCellStyle.BackColor = Color.LightPink;
                    this.dgvSimulacion
                        .Columns[7].DefaultCellStyle.BackColor = Color.LightPink;
                    this.dgvSimulacion
                        .Columns[8].DefaultCellStyle.BackColor = Color.LightPink;
                    this.dgvSimulacion
                        .Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[9].HeaderText = "RND";
                    this.dgvSimulacion
                        .Columns[10].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[9].DefaultCellStyle.BackColor = Color.Turquoise;
                    this.dgvSimulacion
                        .Columns[10].DefaultCellStyle.BackColor = Color.Turquoise;
                    this.dgvSimulacion
                        .Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[10].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[12].HeaderText = "S1 estado";
                    this.dgvSimulacion
                        .Columns[13].HeaderText = "S1 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[14].HeaderText = "S2 estado";
                    this.dgvSimulacion
                        .Columns[15].HeaderText = "S2 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[16].HeaderText = "S3 estado";
                    this.dgvSimulacion
                        .Columns[17].HeaderText = "S3 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[18].HeaderText = "S4 estado";
                    this.dgvSimulacion
                        .Columns[19].HeaderText = "S4 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[20].HeaderText = "S5 estado";
                    this.dgvSimulacion
                        .Columns[21].HeaderText = "S5 fin ocupacion";
                    this.dgvSimulacion
                    .Columns[22].HeaderText = "S6 estado";
                    this.dgvSimulacion
                        .Columns[23].HeaderText = "S6 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[24].HeaderText = "S7 estado";
                    this.dgvSimulacion
                        .Columns[25].HeaderText = "S7 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[26].HeaderText = "S8 estado";
                    this.dgvSimulacion
                        .Columns[27].HeaderText = "S8 fin ocupacion";
                    this.dgvSimulacion
                        .Columns[12].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[13].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[14].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[15].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[16].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[17].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[18].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[19].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[20].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[21].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[22].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[23].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[24].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[25].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[26].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[27].DefaultCellStyle.BackColor = Color.SandyBrown;
                    this.dgvSimulacion
                        .Columns[12].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[13].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[14].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[15].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[16].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[17].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[18].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[19].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[20].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[21].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[22].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[23].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[24].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[25].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[26].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[27].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[28].HeaderText = "RND";
                    this.dgvSimulacion
                        .Columns[29].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[28].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[29].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[30].HeaderText = "Estado caja";
                    this.dgvSimulacion
                        .Columns[31].HeaderText = "T. cobro";
                    this.dgvSimulacion
                        .Columns[32].HeaderText = "Fin cobro";
                    this.dgvSimulacion
                        .Columns[33].HeaderText = "Monto a cobrar";
                    this.dgvSimulacion
                        .Columns[34].HeaderText = "AC Monto";
                    this.dgvSimulacion
                        .Columns[35].HeaderText = "Cola caja";
                    this.dgvSimulacion
                        .Columns[30].DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    this.dgvSimulacion
                        .Columns[31].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[32].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[33].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[34].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[35].DefaultCellStyle.BackColor = Color.LightSteelBlue;

                    this.dgvSimulacion
                        .Columns[30].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[31].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[32].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[33].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[34].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[35].SortMode = DataGridViewColumnSortMode.NotSortable;

                    this.dgvSimulacion
                        .Columns[36].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[37].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[38].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[39].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[40].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[41].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[42].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[43].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[44].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[45].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[46].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[47].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[48].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[49].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[50].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[51].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[52].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[53].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[54].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[55].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[56].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[57].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[58].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[59].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[60].HeaderText = "Tipo auto";
                    this.dgvSimulacion
                        .Columns[61].HeaderText = "T. permanencia";
                    this.dgvSimulacion
                        .Columns[62].HeaderText = "Estado";
                    this.dgvSimulacion
                        .Columns[36].DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    this.dgvSimulacion
                        .Columns[37].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[38].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[39].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[40].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[41].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[42].DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    this.dgvSimulacion
                        .Columns[43].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[44].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[45].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[46].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[47].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[48].DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    this.dgvSimulacion
                        .Columns[49].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[50].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[51].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[52].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[53].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[54].DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    this.dgvSimulacion
                        .Columns[55].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[56].DefaultCellStyle.BackColor = Color.DarkSalmon;
                    this.dgvSimulacion
                        .Columns[57].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[58].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[59].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[60].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[61].DefaultCellStyle.BackColor = Color.LightSteelBlue;
                    this.dgvSimulacion
                        .Columns[62].DefaultCellStyle.BackColor = Color.LightSteelBlue;

                    this.dgvSimulacion
                        .Columns[36].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[37].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[38].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[39].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[40].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[41].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[42].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[43].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[44].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[45].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[46].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[47].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[48].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[49].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[50].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[51].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[52].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[53].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[54].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[55].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[56].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[57].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[58].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[59].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[60].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[61].SortMode = DataGridViewColumnSortMode.NotSortable;
                    this.dgvSimulacion
                        .Columns[62].SortMode = DataGridViewColumnSortMode.NotSortable;
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