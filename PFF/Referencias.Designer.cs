namespace PFF
{
    partial class Referencias
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtReferencia = new System.Windows.Forms.TextBox();
            this.picboxImagemReferencia = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picboxImagemReferencia)).BeginInit();
            this.SuspendLayout();
            // 
            // txtReferencia
            // 
            this.txtReferencia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReferencia.Location = new System.Drawing.Point(16, 449);
            this.txtReferencia.Multiline = true;
            this.txtReferencia.Name = "txtReferencia";
            this.txtReferencia.ReadOnly = true;
            this.txtReferencia.Size = new System.Drawing.Size(524, 42);
            this.txtReferencia.TabIndex = 2;
            this.txtReferencia.TabStop = false;
            this.txtReferencia.WordWrap = false;
            // 
            // picboxImagemReferencia
            // 
            this.picboxImagemReferencia.Image = global::PFF.Properties.Resources.enrijecedores;
            this.picboxImagemReferencia.Location = new System.Drawing.Point(16, 14);
            this.picboxImagemReferencia.Name = "picboxImagemReferencia";
            this.picboxImagemReferencia.Size = new System.Drawing.Size(548, 429);
            this.picboxImagemReferencia.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picboxImagemReferencia.TabIndex = 0;
            this.picboxImagemReferencia.TabStop = false;
            // 
            // Referencias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 517);
            this.Controls.Add(this.txtReferencia);
            this.Controls.Add(this.picboxImagemReferencia);
            this.Name = "Referencias";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Referencias";
            ((System.ComponentModel.ISupportInitialize)(this.picboxImagemReferencia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picboxImagemReferencia;
        private System.Windows.Forms.TextBox txtReferencia;
    }
}