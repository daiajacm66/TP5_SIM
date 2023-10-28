namespace TP5_SIM_G6
{
    partial class Inicio
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Inicio));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnIniciarSim = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(403, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "Trabajo práctico N°4 - Sistema de colas";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat Medium", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(65, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(432, 65);
            this.label2.TabIndex = 2;
            this.label2.Text = "Estacionamiento";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(206, 304);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 27);
            this.label3.TabIndex = 3;
            this.label3.Text = "Grupo 6 - 4K4";
            // 
            // btnIniciarSim
            // 
            this.btnIniciarSim.BackColor = System.Drawing.Color.Black;
            this.btnIniciarSim.FlatAppearance.BorderSize = 0;
            this.btnIniciarSim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciarSim.ForeColor = System.Drawing.Color.White;
            this.btnIniciarSim.Location = new System.Drawing.Point(160, 490);
            this.btnIniciarSim.Name = "btnIniciarSim";
            this.btnIniciarSim.Size = new System.Drawing.Size(241, 52);
            this.btnIniciarSim.TabIndex = 4;
            this.btnIniciarSim.Text = "Iniciar simulación";
            this.btnIniciarSim.UseVisualStyleBackColor = false;
            this.btnIniciarSim.Click += new System.EventHandler(this.btnIniciarSim_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TP5_SIM_G6.Properties.Resources.pexels_scott_platt_1294238;
            this.pictureBox1.Location = new System.Drawing.Point(592, -19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(575, 673);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Inicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1165, 644);
            this.Controls.Add(this.btnIniciarSim);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Inicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnIniciarSim;
    }
}

