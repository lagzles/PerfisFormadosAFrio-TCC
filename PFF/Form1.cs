using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PFF
{
    public partial class Form1 : Form
    {
        double elemento1, elemento2, elemento3, elemento4, elemento5;
        public static double espessura, b1, b2, b3, b4, b5, yCG, raio, comprimento;
        public static double areaTotal;
        public static double iPerfilX;
        public static double iPerfilY;
        public static double ro, rx, ry, xg, xo, cw, J, moduloWx, moduloWy;

        public static double areaEfetiva, inerciaELUX, inerciaELUY;
        public static double areaEfetivaLocal, inerciaEfetivaLocal, rsdLocal; //Flambagem Local
        public static double rsdTracao; //Tração
        public static double areaEfetivaCompressao, inerciaEfetivaCompressao, fatorReducaoCompr, rsdGlobal; //Flambagem Global
        public static double mrdFlexaoSimplesX, mrdFlexaoSimplesY, moduloWEfetivoFlexaoSimplesX, moduloWEfetivoFlexaoSimplesY; //Flexão Simples
        public static string verificacaoDistorcional, verificacaoDistorcionalComp, verificacaoDistorcionalFlex;
        public static double moduloWFlexaoX, fatorReducaoFlexaoX, fatorReducaoFlexaoY, mrdFlexaoTorcaoX, mrdFLexaoTorcaoY; //Flexao Composta
        public static double vrdCortante;

        public static double solicitacaoNormal, solicitacaoMomentoX, solicitacaoMomentoY, solicitacaoCortante;

        public string relatorio, valoresMaximosPerfil;
        public static double tensaofy { get; set; }
        public static double fatorCb { get; set; } = 1.0;
        
        public static double E = 20000;
        public static double G = 7700;
        public static List<double> lstElementos;

        public Form1()
        {
            InitializeComponent();
        }

        public double PegarTensao() { return tensaofy; }
        public double PegarE() { return E; }
        public double PegarG() { return G; }

        private void cmbTensao_SelectedIndex(object sender, EventArgs e)
        {
            switch (cmbTensao.SelectedIndex)
            {
                case 0: //250 Mpa
                    tensaofy = 25;
                    break;

                case 1: //300 Mpa
                    tensaofy = 30;
                    break;

                case 2: //350 Mpa
                    tensaofy = 35;
                    break;

                case 3: //400 Mpa
                    tensaofy = 40;
                    break;

                case 4: //450 Mpa
                    tensaofy = 45;
                    break;
            }
        }

        private void cmbPerfil_SelectedIndex(object sender, EventArgs e)
        {
            switch (cmbPerfil.SelectedIndex)
            {
                case 0:
                    txtElemento1.Text = txtElemento5.Text = "0";
                    txtElemento1.Visible = false;
                    txtElemento5.Visible = false;
                    txtEspessura.Location = new Point(133, 222);
                    txtElemento5.Location = new Point(80, 220);
                    pictureBox1.Image = PFF.Properties.Resources.Perfil_u_simples_meu;
                    break;

                case 1:
                    txtElemento1.Text = txtElemento5.Text = "0";
                    txtElemento1.Visible = true;
                    txtElemento5.Visible = true;
                    txtEspessura.Location = new Point(110, 155);
                    txtElemento5.Location = new Point(133, 222);
                    pictureBox1.Image = PFF.Properties.Resources.perfil_u_enrijecido_meu;
                    break;

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbTensao.Items.AddRange(new object[] { "250 MPa", "300 MPa", "350 MPa", "400 MPa", "450 MPa" });
            cmbTensao.SelectedIndex = 0;

            cmbPerfil.Items.AddRange(new object[] { "Perfil U Simples", "Perfil U Enrijecido" });
            cmbPerfil.SelectedIndex = 1;
        }

        private void alterarTexto (object sender, EventArgs e)
        {
            txtElemento4.Text = txtElemento2.Text;
            txtElemento5.Text = txtElemento1.Text;
        }

        private void alterarTexto2 (object sender, EventArgs e)
        {
            txtRaioInterno.Text = txtEspessura.Text;
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            //define o titulo
            saveFileDialog1.Title = "Salvar Arquivo Texto";
            //Define as extensões permitidas
            saveFileDialog1.Filter = "Text File|.txt  ";
            //define o indice do filtro
            saveFileDialog1.FilterIndex = 0;
            //Atribui um valor vazio ao nome do arquivo
            saveFileDialog1.FileName = "MemoriadeCalculo";
            //Define a extensão padrão como .txt
            saveFileDialog1.DefaultExt = ".txt";
            //define o diretório padrão
            saveFileDialog1.InitialDirectory = @"c:\dados";
            //restaura o diretorio atual antes de fechar a janela
            saveFileDialog1.RestoreDirectory = true;

            //Abre a caixa de dialogo e determina qual botão foi pressionado
            DialogResult resultado = saveFileDialog1.ShowDialog();

            string nomeArquivo = saveFileDialog1.FileName;
            //Se o ousuário pressionar o botão Salvar
            if (resultado == DialogResult.OK)
            {
                //Cria um stream usando o nome do arquivo
                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);

                //Cria um escrito que irá escrever no stream
                StreamWriter writer = new StreamWriter(fs);
                //escreve o conteúdo da caixa de texto no stream
                writer.Write(relatorio);
                
                //fecha o escrito e o stream
                writer.Close();
            }
            else
            {
                //exibe mensagem informando que a operação foi cancelada
                MessageBox.Show("Operação cancelada");
            }
        }

        private void bntMostrarReferencias_Tabela11(object sender, EventArgs e)
        {
            string texto = @"Valores para dispensar a verificação a Flambagem Distorcional na compressão.
Item 9.7.3, Tabela 11, NBR 14762:2010. Flambagem Distorcional";
            Referencias frm = new Referencias(texto, 11);
            frm.Show();
        }

        private void bntMostrarReferencias_Tabela14(object sender, EventArgs e)
        {
            string texto = @"Valores para dispensar a verificação a Flambagem Distorcional na flexão.
Item 9.8.2.3, Tabela 14, NBR 14762:2010.";
            Referencias frm = new Referencias(texto, 14);
            frm.Show();
        }

        private void bntMostrarReferencias_Tabela4(object sender, EventArgs e)
        {
            string texto = @"Valores máximos da relação largura-espessura.
Item 9.1.2, Tabela 4, NBR 14762:2010.";
            Referencias frm = new Referencias(texto, 4);
            frm.Show();
        }

        private void bntMostrarReferencias_Item9221(object sender, EventArgs e)
        {
            string texto = @"Procedimento de calculo das larguras efetivas dos elementos AA e AL.
Item 9.2.2.1, NBR 14762:2010";
            Referencias frm = new Referencias(texto, 9221);
            frm.Show();
        }

        private void bntMostrarReferencias_tabela5(object sender, EventArgs e)
        {
            string texto = @"Largura efetiva e coeficientes de flambagem local para Elementos AA.
Item 9.2.2.1, NBR 14762:2010";
            Referencias frm = new Referencias(texto, 5);
            frm.Show();
        }

        private void bntMostrarReferencias_tabela6(object sender, EventArgs e)
        {
            string texto = @"Largura efetiva e coeficientes de flambagem local para elementos AL.
Item 9.2.2.1, NBR 14762:2010.";
            Referencias frm = new Referencias(texto, 6);
            frm.Show();
        }

        private void bntMostrarReferencias_Item9231 (object sender, EventArgs e)
        {
            string texto = @"Calculo da largura efetiva de elementos enrijecidos.
Item 9.2.3.1, NBR 14762:2010.";
            Referencias frm = new Referencias(texto, 9231);
            frm.Show();
        }

        // Ao clicar no botao 'Verificar'
        private void calculoPerfilU (object sender, EventArgs e)
        {            
            elemento1 = double.Parse(txtElemento1.Text) / 10;
            elemento2 = double.Parse(txtElemento2.Text) / 10;
            elemento3 = double.Parse(txtElemento3.Text) / 10;
            elemento4 = elemento2;
            elemento5 = elemento1;
            raio = double.Parse(txtRaioInterno.Text) / 10;            
            espessura = (double.Parse(txtEspessura.Text)) * 0.1;
            comprimento = double.Parse(txtComprimento.Text);

            solicitacaoNormal = double.Parse(txtbxNsd.Text);
            solicitacaoMomentoX = double.Parse(txtbxMsdX.Text);
            solicitacaoMomentoY = double.Parse(txtbxMsdY.Text);
            solicitacaoCortante = double.Parse(txtbxVsd.Text);

            var novoperfilU = new PerfilU(elemento1, elemento2, elemento3, elemento4, elemento5, espessura, raio, comprimento,"MONOSSIMETRICO");
            //=================================================
            // Calculo das Resistencias e Propriedades Geométricas e Efetivas
            novoperfilU.Dimensionar();

            areaTotal = novoperfilU.PegarArea();
            iPerfilX = novoperfilU.PegarIx();
            iPerfilY = novoperfilU.PegarIy();
            rx = novoperfilU.PegarRx();
            ry = novoperfilU.PegarRy();
            ro = novoperfilU.PegarRo();

            areaEfetivaLocal = novoperfilU.PegarAreaEfetivaLocal();
            moduloWEfetivoFlexaoSimplesX = novoperfilU.PegarWxEfetivo();
            moduloWEfetivoFlexaoSimplesY = novoperfilU.PegarWyEfetivo();
            cw = novoperfilU.PegarCW();
            J = novoperfilU.PegarJ();
            relatorio = novoperfilU.PegarRelatorio();

            label35.Text = novoperfilU.PegarIndiceEsbeltez();
            valoresMaximosPerfil = novoperfilU.PegarValoresMaximos();

            // Resistencia a Flambagem Local
            rsdLocal = novoperfilU.PegarRsdLocal();
            // Resistencia a Tração
            rsdTracao = novoperfilU.PegarRsdTracao();
            // Resistencias a Flexão Simples
            mrdFlexaoSimplesX = novoperfilU.PegarMrdFlexaoSimplesX();// Em X
            mrdFlexaoSimplesY = novoperfilU.PegarMrdFlexaoSimplesY();// Em Y
            //Compressão
            rsdGlobal = novoperfilU.PegarRsdCompressao();
            areaEfetivaCompressao = novoperfilU.PegarAreaEfetivaGlobal();
            fatorReducaoCompr = novoperfilU.PegarFatorReducaoCompressao();
            //Flexão Lateral com Torção
            mrdFlexaoTorcaoX = novoperfilU.PegarMrdFlexaoTorcaoX(); // Em X
            fatorReducaoFlexaoX = novoperfilU.PegarFatorReducaoFlexaoX(); // Em X
            mrdFLexaoTorcaoY = novoperfilU.PegarMrdFlexaoTorcaoY(); // Em Y
            fatorReducaoFlexaoY = novoperfilU.PegarFatorReducaoFlexaoY();// Em Y
            //Cortante
            vrdCortante = novoperfilU.PegarVrdCortante();
            //Verificação Distorcional
            verificacaoDistorcionalComp = novoperfilU.PegarVerificacaoDistorcionalCompressao();
            verificacaoDistorcionalFlex = novoperfilU.PegarVerificacaoDistorcionalFlexao();
            // Flexao Composta, FLexao e Cortante
            lblFLexaoComposta.Text = novoperfilU.PegarFlexaoComposta();
            lblFLexaoCortanteX.Text = novoperfilU.PegarFlexaoCortanteX();
            lblFlexaoCortanteY.Text = novoperfilU.PegarFlexaoCortanteY();
            // Verificação dos valores limites das dimensões
            if (valoresMaximosPerfil != "ok")
            {
                string texto = @"Verificar os valores máximos da relação largura-espessura conforme
Item 9.1.2, Tabela 4, NBR 14762:2010.";
                Referencias frm = new Referencias(texto, 4);
                frm.Show();
            }
            //================================================================
            //Propriedades Geométricas da seção
            lblAreaTotal.Text = "A = " + areaTotal.ToString("#.##") + "cm2";
            lblInerciaPerfilx.Text = "Ix = " + iPerfilX.ToString("#.##") + " cm4";
            lblinerciaPerfilY.Text = "Iy = " + iPerfilY.ToString("#.##") + " cm4";
            lblRaioGiracaoX.Text = "rx = " + rx.ToString("#.##") + " cm";
            lblRaioGiracaoY.Text = "ry = " + ry.ToString("#.##") + " cm";
            lblRzero.Text = "ro = " + ro.ToString("#.##") + " cm";
            lblConstanteEmpenamento.Text = "Cw = " + cw.ToString("#.##") + " cm3";
            lblConstanteTorcao.Text = "J = " + J.ToString("#.##") + " cm3";

            // Dados da Flambagem Local
            lblAreaEfetiva.Text = "Aef = " + areaEfetivaLocal.ToString("#.##") + " cm2";
            lblRsdLocal.Text = "Rsd = " + rsdLocal.ToString("#.##") + " kN";

            // Dados da Resistencia a Tracao
            lblRsdTracao.Text = "Rsd = " + rsdTracao.ToString("#.##") + " kN";

            // Dados da Flexão Simples
            lblRsdFlexaoSimplesX.Text = "Mrd,x = " + mrdFlexaoSimplesX.ToString("#.##") + " kN.cm";
            lblFlexaoSimplesWefetivoX.Text = "Wcx,ef = " + moduloWEfetivoFlexaoSimplesX.ToString("#.##") + " cm3";

            lblMrdFlexaoSimplesY.Text = "Mrd,y = " +  mrdFlexaoSimplesY.ToString("#.##") + " kN.cm";
            lblWEfetivoFlexaoSimplesY.Text = "Wcy,ef = " + moduloWEfetivoFlexaoSimplesY.ToString("#.##") + " cm3";
            
            //Dados da Resistencia da Flambagem Global
            lblRsdGlobal.Text = "Rsd = " + rsdGlobal.ToString("#.##") + " kN";
            lblAreaEfetivaCompressao.Text = "Aef = " + areaEfetivaCompressao.ToString("#.##") + " cm2";
            lblFatorReducaoComp.Text = "X = " + fatorReducaoCompr.ToString("#.######");
            //lblDistorcionalComp.Text = verificacaoDistorcional;

            //Verificação a flambagem Distorcional
            lblVerificacaoDistCompressao.Text = verificacaoDistorcionalComp;
            lblVerificacaoDistFLexao.Text = verificacaoDistorcionalFlex;

            if (verificacaoDistorcionalComp == "Verificar")
            {
                string texto = @"É necessário verificar a seção para flambagem distorcional,
pois o perfil não atendeu ao Item 9.7.3, Tabela 11, NBR 14762:2010";
                Referencias frm = new Referencias(texto, 11);
                frm.Show();
            }
            if (verificacaoDistorcionalFlex == "Verificar")
            {
                string texto = @"É necessário verificar a seção para flambagem distorcional,
pois o perfil não atendeu ao Item 9.8.2.3, Tabela 14, NBR 14762:2010";
                Referencias frm = new Referencias(texto, 14);
                frm.Show();
            }

            //Dados da Resistencia a Flexao com Torção
            lblMrdFlexaoTorcaoX.Text = "Mrd,x = " + mrdFlexaoTorcaoX.ToString("#.##") + " kN.cm";
            lblFatorReducaoFlexaoTorcaoX.Text = "Xflt,x = " + fatorReducaoFlexaoX.ToString("#.####");

            lblMrdFlexaoTorcaoY.Text = "Mrd,y = " + mrdFLexaoTorcaoY.ToString("#.##") + " kN.cm";
            lblFatorReducaoFlexaoTorcaoY.Text = "Xflt,y = " + fatorReducaoFlexaoY.ToString("#.####");

            //Dados Cortante
            lblVrdCortante.Text = "Vrd = " + vrdCortante.ToString("#.##") + "kN";
            //================================================

        }
    }
}
