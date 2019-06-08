using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PFF
{
    public partial class Referencias : Form
    {
        public Referencias()
        {
            InitializeComponent();
        }

        public Referencias(string texto, int x)
        {
            InitializeComponent();
            txtReferencia.Text = texto;

            if(x == 11)
            {
                picboxImagemReferencia.Image = PFF.Properties.Resources.Tabela_11;
            }
            else if (x == 14)
            {
                picboxImagemReferencia.Image = PFF.Properties.Resources.Tabela_14;
            }
            else if (x == 4)
            {
                picboxImagemReferencia.Image = PFF.Properties.Resources.tabela_4;
            }
            else if (x == 5)
            {
                picboxImagemReferencia.Size = new System.Drawing.Size(540, 600);
                picboxImagemReferencia.Image = PFF.Properties.Resources.Tabela5;
                this.Size = new Size(590, 700);
                txtReferencia.Location = new Point(20, 610);
            }
            else if (x == 6)
            {
                picboxImagemReferencia.Size = new System.Drawing.Size(540, 600);
                picboxImagemReferencia.Image = PFF.Properties.Resources.tabela6;
                this.Size = new Size(590, 700);
                txtReferencia.Location = new Point(20, 610);
            }
            else if (x == 9221)
            {
                picboxImagemReferencia.Image = PFF.Properties.Resources.Item_9221;
            }
            else if (x == 9231)
            {
                picboxImagemReferencia.Size = new System.Drawing.Size(540, 600);
                picboxImagemReferencia.Image = PFF.Properties.Resources.enrijecedores;
                this.Size = new Size(590, 700);
                txtReferencia.Location = new Point(20, 610);
            }
        }
    }
}
