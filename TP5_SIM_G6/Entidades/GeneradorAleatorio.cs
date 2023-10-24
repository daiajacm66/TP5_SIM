using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP5_SIM_G6.Entidades
{
    public class GeneradorAleatorio
    {
        //private double rnd { get; set; }
        private double nro_generado { get; set; }
        private double lambda { get; set; }

        public double Generate(double rnd)
        {
            this.nro_generado = (-1 / this.lambda) * Math.Log(1 - rnd);
            return this.nro_generado;
        }
    }
}
