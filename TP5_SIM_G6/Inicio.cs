using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP5_SIM_G6
{
    public partial class Inicio : Form
    {
        public Inicio()
        {
            InitializeComponent();
        }

        private void btnIniciarSim_Click(object sender, EventArgs e)
        {
            SimuladorCola ventana = new SimuladorCola();
            ventana.Show();
        }
    }
}
