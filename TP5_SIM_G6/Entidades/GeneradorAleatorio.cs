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
        private double media { get; set; }

        public GeneradorAleatorio(double media)
        {
            this.media = media;
        }

        public double Generate(double rnd)
        {
            this.nro_generado = (-this.media) * Math.Log(1 - rnd);
            return this.nro_generado;
        }
    }
}
