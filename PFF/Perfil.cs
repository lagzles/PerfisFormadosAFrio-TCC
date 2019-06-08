using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFF
{
    public class Perfil
    {

        public double Elemento1 { get; private set; }
        public double Elemento2 { get; private set; }
        public double Elemento3 { get; private set; }
        public double Elemento4 { get; private set; }
        public double Elemento5 { get; private set; }
        public double Espessura { get; private set; }
        public double Comprimento { get; private set; }
        public double Raio { get; private set; }
        public string Simetria { get; private set; }

        public Perfil(double elemento1, double elemento2, double elemento3, double elemento4, double elemento5,
            double espessura, double raio, double comprimento, string simetria)
        {
            this.Elemento1 = elemento1;
            this.Elemento2 = elemento2;
            this.Elemento3 = elemento3;
            this.Elemento4 = elemento4;
            this.Elemento5 = elemento5;
            this.Espessura = espessura;
            this.Raio = raio;
            this.Comprimento = comprimento;
            this.Simetria = simetria;
        }

        void flambagemLocal() { }
        void tracao() { }
        void flexaoSimples() { }
        void compressao() { }
        void flexaoComposta() { }
        void cortante() { }
        void distorcional() { }
        void flexaoTorcao() { }


        public void Dimensionar()
        {
            flambagemLocal();
            tracao();
            flexaoSimples();
            compressao();
            flexaoTorcao();
            cortante();
            distorcional();
            flexaoComposta();
        }

        public double PegarElemento1() { return Elemento1; }
        public double PegarElemento2() { return Elemento2; }
        public double PegarElemento3() { return Elemento3; }
        public double PegarElemento4() { return Elemento4; }
        public double PegarElemento5() { return Elemento5; }
        public double PegarEspessura() { return Espessura; }
        public double PegarRaio() { return Raio; }
        public double PegarComprimento() { return Comprimento; }
        public string PegarSimetria() { return Simetria; }

        public virtual double PegarAlma() { return 0.0; }
        public virtual double PegarMesa() { return 0.0; }
        public virtual double PegarEnrijecedor() { return 0.0; }

        public virtual double PegarYcg() { return 0.0; }
        public virtual double Pegarb2() { return 0.0; }

        public virtual double PegarArea() { return 0.0; }
        public virtual double PegarIx() { return 0.0; }
        public virtual double PegarIy() { return 0.0; }
        public virtual double PegarXo() { return 0.0; }
        public virtual double PegarRo() { return 0.0; }
        public virtual double PegarCW() { return 0.0; }
        public virtual double PegarJ() { return 0.0; }
        public virtual double PegarJy() { return 0.0; }

        public virtual double PegarRy() { return 0.0; }
        public virtual double PegarRx() { return 0.0; }
        public virtual double PegarWx() { return 0.0; }
        public virtual double PegarWy() { return 0.0; }
        public virtual double PegarAreaEfetiva() { return 0.0; }
        public virtual double PegarAreaEfetivaLocal() { return 0.0; }
        public virtual double PegarAreaEfetivaGlobal() { return 0.0; }

        public virtual double PegarWxEfetivo() { return 0.0; }
        public virtual double PegarWyEfetivo() { return 0.0; }
        public virtual double PegarRsdLocal() { return 0.0; }
        public virtual double PegarRsdCompressao() { return 0.0; }
        public virtual double PegarRsdTracao() { return 0.0; }
        public virtual double PegarMrdFlexaoSimplesX() { return 0.0; }
        public virtual double PegarMrdFlexaoSimplesY() { return 0.0; }

        public virtual double PegarMrdFlexaoTorcaoX() { return 0.0; }
        public virtual double PegarMrdFlexaoTorcaoY() { return 0.0; }

        public virtual string PegarFlexaoComposta() { return ""; }
        public virtual string PegarFlexaoCortanteX() { return ""; }
        public virtual string PegarFlexaoCortanteY() { return ""; }

        public virtual double PegarVrdCortante() { return 0.0; }

        public virtual double PegarFatorReducaoCompressao() { return 0.0; }
        public virtual double PegarFatorReducaoFlexaoX() { return 0.0; }
        public virtual double PegarFatorReducaoFlexaoY() { return 0.0; }

        public virtual string PegarVerificacaoDistorcionalCompressao() { return ""; }
        public virtual string PegarVerificacaoDistorcionalFlexao() { return ""; }
        public virtual string PegarIndiceEsbeltez() { return ""; }
        public virtual string PegarValoresMaximos() { return ""; }

        public virtual string PegarRelatorio() { return ""; }
    }
}
