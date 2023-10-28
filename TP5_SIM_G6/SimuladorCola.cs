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

namespace TP5_SIM_G6
{
    public partial class SimuladorCola : Form
    {
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
            Simulador sim = new Simulador();
        }
    }
}
