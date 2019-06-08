using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFF
{
    public class PerfilU : Perfil
    {
        public PerfilU(double elemento1,
            double elemento2,
            double elemento3,
            double elemento4,
            double elemento5,
            double espessura, double raio,
            double comprimento,
            string simetria)
            : base(elemento1, elemento2, elemento3, elemento4, elemento5, espessura, raio, comprimento, simetria)
        {

        }

        public static double b1m, b2m, b3m, b4m, b5m; // Larguras para calculo das propriedades Geometricas
        private static double b1, b2, b3, b4, b5; // Largura para calculo de dimensionamento e propriedades efetivas;
        static double b1ef, b3ef, b5ef, b3ef2;
        static List<double> b2ef; static List<double> b4ef;

        static public double area, iPerfilX, iPerfilY;
        static double ycg;
        static double ro, rx, ry, xg, xo, cw, jx, jy, moduloWx, moduloWy;
        static double areaEfetiva, areaEfetivaLocal, areaEfetivaGlobal, moduloWxEfetivo, moduloWyEfetivo;
        static double rsdLocal, rsdCompressao, rsdTracao, mrdFlexaoSimplesx, mrdFlexaoSimplesy, mrdFlexaoTorcaoX, mrdFlexaoTorcaoY, vrdCortante;
        static double fatorReducaoCompressao, fatorReducaoFlexaoX, fatorReducaoFlexaoY;
        static string verificacaoDistorcionalCompressao, verificacaoDistorcionalFlexao, relatorioDeCalculo, condicaoFlexaoComposta;
        static string condicaoFlexaoCortanteX, condicaoFlexaoCortanteY, indiceEsbeltez, valoresMaximos;

        public void largurasDeCalculo()
        {
            if (Elemento1 == 0)
            {
                b1 = 0;
                b1m = 0;
            } // Enrijecedor            
            else
            {
                b1 = Elemento1 - Espessura - Raio;
                b1m = Elemento1 - 0.5 * Espessura;
            }
            if (Elemento1 == 0) // Mesa com enrijecedor
            {
                b2 = Elemento2 - Espessura - Raio;
                b2m = Elemento2 - 0.5 * Espessura;
            }
            else // Sem enrijecedor
            {
                b2 = Elemento2 - 2 * Espessura - 2 * Raio;
                b2m = Elemento2 - Espessura;
            }
            b3 = Elemento3 - 2 * Espessura; // Alma
            b3m = Elemento3 - Espessura; // Alma

            if (Elemento5 == 0) // Mesa com enrijecedor
            {
                b4 = Elemento4 - Espessura - Raio;
                b4m = Elemento4 - 0.5 * Espessura;
            }
            else // Sem enrijecedor
            {
                b4 = Elemento4 - 2 * Espessura - 2 * Raio;
                b4m = Elemento4 - Espessura;
            }
            if (Elemento5 == 0)
            {
                b5 = 0;
                b5m = 0;
            } // Enrijecedor            
            else
            {
                b5 = Elemento5 - Espessura - Raio;
                b5m = Elemento5 - 0.5 * Espessura;
            }
        }

        static void propGeometricasPerfilU(Perfil perfil)
        {
            double t = perfil.PegarEspessura();
            double elemento1 = perfil.PegarElemento1();
            double elemento2 = perfil.PegarElemento2();
            double elemento3 = perfil.PegarElemento3();

            ycg = elemento3 * 0.5;

            area = t * (PerfilU.b1m + PerfilU.b2m + PerfilU.b3m + b4m + b5m);
            xg = (b2m * t * (b2m + 2 * b1m)) / area; // Com relação a linha média da alma

            //Mesa
            double momentobx2 = (b2m * Math.Pow(t, 3)) / 12;
            double momentoby2 = (t * Math.Pow(b2m, 3)) / 12; ;
            double areaB2 = b2m * t;
            double yB2 = ycg - t / 2; //Distancia do eixo até a linha media da mesa
            double xB2 = (0.5 * b2m) - xg;

            //Alma
            double momentobx3 = (t * Math.Pow(b3m, 3)) / 12;
            double momentoby3 = (b3m * Math.Pow(t, 3)) / 12;
            double areaB3 = b3m * t;
            double xb3 = xg;

            //Enrijecedores
            double momentobx1 = (t * Math.Pow(b1m, 3)) / 12;
            double momentoby1 = (b1m * Math.Pow(t, 3)) / 12;
            double areaB1 = b1m * t;
            double yB1 = ycg - (0.5 * t + b1m * 0.5);
            double xb1 = elemento2 - xg - t;

            //Momento em X
            iPerfilX = 2 * (momentobx2 + areaB2 * Math.Pow(yB2, 2)) + 2 * (momentobx1 + areaB1 * Math.Pow(yB1, 2))
                + momentobx3;

            rx = Math.Sqrt(iPerfilX / area);

            moduloWx = iPerfilX / ycg;

            // Calculo do xo, cw, J
            // Momento em Y 
            iPerfilY = (momentoby3 + areaB3 * Math.Pow(xb3, 2)) + 
                2 * (momentoby2 + areaB2 * Math.Pow(xB2, 2)) +
                2 * (momentoby1 + areaB1 * Math.Pow(xb1, 2));

            ry = Math.Sqrt(iPerfilY / area);

            moduloWy = iPerfilY / xg;

            double am = b3m;
            double bm, cm = b1m;

            double bethaL, bethaF, bethaW;
            if (b1m == 0)
            {
                bethaL = 0;
                bm = b2m;
                xo = (Math.Pow(bm, 2) / (am + 2 * bm)) + (3 * Math.Pow(bm, 2)) / (6 * bm + am); //Wei-Wu pg. 385
                jx = (Math.Pow(t, 3) * (am + 2 * bm)) / 3;
                cw = ((t * Math.Pow(am, 2) * Math.Pow(bm, 3)) / 12) * ((3 * bm + 2 * am) / (6 * bm + am));
            }
            else
            {
                bm = b2m;
                // Wei Wu pg. 385
                xo = xg + bm * ((6 * Math.Pow(am, 2) * cm + 3 * bm * Math.Pow(am, 2) - 8 * Math.Pow(cm, 3)) /
                    (Math.Pow(am, 3) + 6 * bm * Math.Pow(am, 2) + 6 * cm * Math.Pow(am, 2)
                    - 12 * am * Math.Pow(cm, 2) + 8 * Math.Pow(cm, 3)));

                jx = Math.Pow(t, 3) * (am + 2 * bm + 2 * cm) / 3;

                double Ix = t / 12 * (Math.Pow(am, 3) + 6 * bm * Math.Pow(am, 2) + 6 * cm * Math.Pow(am, 2)
                    - 12 * am * Math.Pow(cm, 2) + 8 * Math.Pow(cm, 3));

                double m = bm * t / (12 * Ix) * (6 * cm * Math.Pow(am, 2) + 3 * bm * Math.Pow(am, 2) - 8 * Math.Pow(cm, 3));
                cw = Math.Pow(t, 2) / area * (xg * area * Math.Pow(am, 2) / t * (Math.Pow(bm, 2) / 3 + Math.Pow(m, 2) - m * bm) +
                    area / (3 * t) * (Math.Pow(m, 2) * Math.Pow(am, 3) + Math.Pow(bm, 2) * Math.Pow(cm, 2) * (2 * cm + 3 * am)) -
                    Ix * Math.Pow(m, 2) / t * (2 * am + 4 * cm) +
                    m * Math.Pow(cm, 2) / 3 * (8 * Math.Pow(bm, 2) * cm + 2 * m * (2 * cm * (cm - am) + bm * (2 * cm - 3 * am))) +
                    Math.Pow(bm, 2) * Math.Pow(am, 2) / 6 * ((3 * cm + bm) * (4 * cm + am) - 6 * Math.Pow(cm, 2)) -
                    Math.Pow(m, 2) * Math.Pow(am, 4) / 4);

                bethaL = 2 * cm * t * Math.Pow(bm - xg, 3) + 2 / 3 * t * (bm - xg) * ((Math.Pow(am / 2, 3) - Math.Pow(am / 2 - cm, 3)));
            }

            bethaF = (t / 2) * (Math.Pow((bm - xg),4) - Math.Pow(xg, 4)) + (t * Math.Pow(am, 2) / 4 ) * (Math.Pow((bm - xg), 2) - Math.Pow(xg, 2));
            bethaW = -(t * xg * Math.Pow(am, 3) / 12 + t * Math.Pow(xg, 3) * am);

            jy = 1 / (2 * iPerfilY) * (bethaW + bethaL + bethaF) + xo;
            ro = Math.Sqrt(Math.Pow(rx, 2) + Math.Pow(ry, 2) + Math.Pow(xo, 2));
        }

        //Realiza o calculo de das Larguras Efetivas de um Perfil U e retorna uma lista destes elementos
        public static List<double>
            MLEPerfilUCompressao(Perfil perfil, double tensao)
        {

            double t = perfil.PegarEspessura();
            double raio = perfil.PegarRaio();
            double elemento1 = perfil.PegarElemento1();
            double elemento2 = perfil.PegarElemento2();
            double elemento3 = perfil.PegarElemento3();
            double elemento5 = perfil.PegarElemento5();

            string solicitacao = "COMPRESSAO";

            //Obtenção das Larguras efetivas de cada elemento, nas configurações de Perfil U
            // Elementos totalmente comprimidos
            if (b1 <= 0) { b1ef = 0; }
            else { b1ef = NBR.MLECompleta("AL", solicitacao, tensao, b1, t, raio, 0, ycg).ElementAt(0); }

            if (b1 <= 0) { b2ef = NBR.MLECompleta("AL", solicitacao, tensao, b2, t, raio, 0, ycg); }
            else { b2ef = NBR.elementoEnrijecido(b2, elemento1, b1ef, tensao, t); }

            b3ef = NBR.MLECompleta("AA", solicitacao, tensao, b3, t, raio, 0, ycg).ElementAt(0);
            b3ef2 = NBR.MLECompleta("AA", solicitacao, tensao, b3, t, raio, 0, ycg).ElementAt(1);

            if (b5 <= 0) { b5ef = 0; }
            else { b5ef = NBR.MLECompleta("AL", solicitacao, tensao, b5, t, raio, 0, ycg).ElementAt(0); }

            if (b5 <= 0) { b4ef = NBR.MLECompleta("AL", solicitacao, tensao, b4, t, raio, 0, ycg); }
            else { b4ef = NBR.elementoEnrijecido(b4, elemento5, b5ef, tensao, t); }

            List<double> listaEfetivos = new List<double>();

            // (t, b1ef, b2ef, b3ef, b4ef, b5ef, b3ef2)
            listaEfetivos.Add(t); //0
            listaEfetivos.Add(b1ef); //1 
            listaEfetivos.Add(b2ef.ElementAt(0)); //2
            listaEfetivos.Add(b3ef); //3
            listaEfetivos.Add(b4ef.ElementAt(0)); //4
            listaEfetivos.Add(b5ef); //5
            listaEfetivos.Add(ycg); // 6, ycgEfetivo
            listaEfetivos.Add(xg); //7
            listaEfetivos.Add(b2ef.ElementAt(1)); //8, ds
            listaEfetivos.Add(0); //9, b3ef2
            //  listaEfetivos.Add(Form1.xg);

            return listaEfetivos;
            //return new Tuple<double, double, double, double, double, double>
            //   (espessura, b1ef, b2ef, b3ef, b4ef, b5ef, ycg, xg);
        }

        public static List<double>
           MLEPerfilUFlexaoX(Perfil perfil, double tensao)
        {

            double t = perfil.PegarEspessura();
            double raio = perfil.PegarRaio();
            double elemento1 = perfil.PegarElemento1();
            double elemento2 = perfil.PegarElemento2();
            double elemento3 = perfil.PegarElemento3();
            double elemento5 = perfil.PegarElemento5();

            // Obtenção das larguras efetivas dos elementos de Perfil U
            double ycgf = 0, contador = 3;
            double ycgi = ycg, areaRetirar;

            while (contador >= 2)
            {
                contador = 0;
                //Obtenção das Larguras efetivas de cada elemento, nas configurações de Perfil U

                // Enrijecedor 
                if (b1 == 0) { b1ef = 0; }
                else { b1ef = NBR.MLECompleta("AL", "FLEXAOX", tensao, b1, t, raio, b1, ycgi).ElementAt(0); }
                if (b1 == b1ef) { ycgf = ycgi; }
                else
                {
                    areaRetirar = t * ((b1 - b1ef));
                    double areaEfetiva = area - t * ((b1 - b1ef));
                    ycgf = ycgi + ((areaRetirar / areaEfetiva) * (ycg - t * 0.5)); contador += 1;
                }

                // Mesa, totalmente comprimida
                if (b1 == 0) 
                { // Caso não tenha enrijecedor
                    b2ef = NBR.MLECompleta("AL", "COMPRESSAO", tensao, b2, t, raio, b2, ycgf);
                    areaRetirar = t * (b2 - b2ef.ElementAt(0));
                    areaEfetiva -= areaRetirar;
                }
                else
                { // Caso tenha enrijecedor
                    b2ef = NBR.elementoEnrijecido(b2, elemento1, b1ef, tensao, t);
                    areaRetirar = t * (b2 - b2ef.ElementAt(0) + b1 - b2ef.ElementAt(1));
                    areaEfetiva -= areaRetirar;
                }

                if (areaRetirar == 0)
                { }
                else { ycgf += ((areaRetirar / areaEfetiva) * (ycg - t * 0.5)); contador += 1; }

                //Alma, parcialmente comprimida
                b3ef = NBR.MLECompleta("AA", "FLEXAOX", tensao, b3, t, raio, b3, ycgf).ElementAt(0);
                b3ef2 = NBR.MLECompleta("AA", "FLEXAOX", tensao, b3, t, raio, b3, ycgf).ElementAt(1);
                if (b3 == b3ef + b3ef2) { }
                else
                {
                    areaRetirar = t * (b3 - b3ef - b3ef2);
                    areaEfetiva -= areaRetirar;
                    ycgf += Math.Round(((areaRetirar / areaEfetiva) * (ycg - t * 0.5)), 3); contador += 3;
                }

                if (contador >= 3) //Caso enrijecedor, mesa e alma não sejam totalmente efetivos;
                {
                    double b3efi = b3ef; double b3ef2i = b3ef2;

                    b3ef = NBR.MLECompleta("AA", "FLEXAOX", tensao, b3, t, raio, b3, ycgf).ElementAt(0);
                    b3ef2 = NBR.MLECompleta("AA", "FLEXAOX", tensao, b3, t, raio, b3, ycgf).ElementAt(1);

                    areaRetirar = t * (b3 - b3ef - b3ef2);
                    areaEfetiva -= areaRetirar;
                    ycgf += Math.Round(((areaRetirar / areaEfetiva) * (ycg - t * 0.5)), 3);

                    double relacao = (b3efi + b3ef2i) / (b3ef + b3ef2); // Verifica se pode se desprezar a proxima iteração.
                    if (relacao < 0.95) { ycgi = ycgf; continue; }
                    else { break; }
                }
                ycgi = ycgf;
            }

            List<double> lstEfetivos = new List<double>();
            // (t, b1ef, b2ef, b3ef, b4ef, b5ef, b3ef2)
            lstEfetivos.Add(t); //0
            lstEfetivos.Add(b1ef); //1, comprimid0
            lstEfetivos.Add(b2ef.ElementAt(0)); //2, comprimida
            lstEfetivos.Add(b3ef); //3. parcialmente comprimido
            lstEfetivos.Add(b4); //4, tracionado
            lstEfetivos.Add(b5); //5, tracionado
            lstEfetivos.Add(ycgf); //6
            lstEfetivos.Add(xg); //7
            lstEfetivos.Add(b2ef.ElementAt(1)); //8, ds
            lstEfetivos.Add(b3ef2); //9

            return lstEfetivos;
        }

        public static List<double>
            MLEPerfilUFlexaoY(Perfil perfil, double tensao)
        {
            double t = perfil.PegarEspessura();
            double raio = perfil.PegarRaio();
            double elemento1 = perfil.PegarElemento1();
            double elemento2 = perfil.PegarElemento2();
            double elemento4 = perfil.PegarElemento4();
            double elemento3 = perfil.PegarElemento3();
            double elemento5 = perfil.PegarElemento5();
            
            double xgi, xgf = 0;
            double tensaoAt1 = 0, tensaoAt2 = tensao;
            xgi = xg;
            // Primeira Situaçao - Alma comprimida
            //Alma totalmente comprimida = elemento AA
            while (tensaoAt1 != tensaoAt2) // TODO corrigir loop infinito
            {
                tensaoAt1 = tensaoAt2;

                b3ef = NBR.MLECompleta("AA", "COMPRESSAO", tensaoAt1, b3, t, raio, b3, xg).ElementAt(0);
                if (b3ef != b3)
                {
                    double areaRetirar = (b3 - b3ef) * t;
                    xgf = xgi + areaRetirar / area * (xg - 0.5 * t);
                }
                else { xgf = xgi; }

                tensaoAt2 = Math.Round(tensao * xgf / (b2 - xgf), 2);
                xgi = xgf;
                if (xgf > elemento2) { xgf = elemento2;b3ef = 0;break; }
            }

            //Verificação da Mesa, elemento parcialmente comprimido - AA
            double xgMesa;
            if (xgf > 0.5 * elemento2) { xgMesa = xgf; }
            else { xgMesa = elemento2 - xgf; }
            b2ef = NBR.MLECompleta("AA", "FLEXAOY", tensaoAt2, b2, t, raio, 0, xgMesa);

            List<double> lstEfetivos = new List<double>();
            // (t, b1ef, b2ef, b3ef, b4ef, b5ef, b3ef2)
            lstEfetivos.Add(t); //0
            lstEfetivos.Add(b1); //1
            lstEfetivos.Add(b2ef.ElementAt(0)); //2
            lstEfetivos.Add(b3ef); //3
            lstEfetivos.Add(b2ef.ElementAt(0)); //4, b4ef
            lstEfetivos.Add(b5); //5
            lstEfetivos.Add(ycg); //6
            lstEfetivos.Add(xgf); //7
            lstEfetivos.Add(b2ef.ElementAt(1)); //8, ds
            lstEfetivos.Add(b3ef2); //9
            lstEfetivos.Add(b2ef.ElementAt(1)); //10, bef2

            return lstEfetivos;
        }

        public static List<double> // (area efetiva (0), modulo W x efetivo (1), modulo W efetivo Y (2))
           propEfetivasPerfilU(Perfil perfil, List<double> listaEfetivos, string solicitacao)
        {
            double t = perfil.PegarEspessura();
            double raio = perfil.PegarRaio();
            double elemento1 = perfil.PegarElemento1();
            double elemento2 = perfil.PegarElemento2();
            double elemento3 = perfil.PegarElemento3();
            double elemento5 = perfil.PegarElemento5();

            double yCGefetivo, deslYcg, xCGefetivo, deslXcg;
            double Iy = iPerfilY; //Inercia do Perfil em Y

            double b1ef = listaEfetivos.ElementAt(1); double b2ef = listaEfetivos.ElementAt(2); double b3ef = listaEfetivos.ElementAt(3);
            double b4ef = listaEfetivos.ElementAt(4); double b5ef = listaEfetivos.ElementAt(5); double ds = listaEfetivos.ElementAt(8);
            double b2ef2 = listaEfetivos.ElementAt(8); double b3ef2 = listaEfetivos.ElementAt(9);

            // A retirar de cada elemento
            double retB1 = b1 - b1ef;
            double retB2 = b2 - b2ef;
            double retB3 = b3 - b3ef - b3ef2;
            double retB4 = b4 - b4ef;
            double retB5 = b5 - b5ef;
            yCGefetivo = listaEfetivos.ElementAt(6);
            deslYcg = Math.Abs(ycg - yCGefetivo);

            xCGefetivo = listaEfetivos.ElementAt(7);
            deslXcg = xg - xCGefetivo;

            // Aera a retirar por elemento
            double areaRet1 = retB1 * t;
            double areaRet2 = retB2 * t;
            double areaRet3 = retB3 * t;
            double areaRet4 = retB4 * t;
            double areaRet5 = retB5 * t;
            double areaRetds; double retDs;
            if (b1 != 0)
            {
                retDs = (b1 - ds);
                areaRetds = t * retDs;
            }
            else { retDs = 0; areaRetds = 0; }

            double areaRet = (areaRet1 + areaRet2 + areaRet3 + areaRet4 + areaRet5 + areaRetds * 2);
            double areaEfetiva = area - areaRet;

            //==================================================================================
            // Inercia a retirar por elemento, em X
            // Enrijecedor - elemento 1
            double yCGb1 = Funcoes.novoCGelementos(yCGefetivo, t, b1, retB1);
            double inerciaRet1X = (t * Math.Pow(retB1, 3)) / 12 + areaRet1 * Math.Pow(yCGb1, 2);

            // Mesa - elemento 2
            double yCGb2 = (yCGefetivo - (t * 0.5));
            double inerciaRet2X = (retB2 * Math.Pow(t, 3)) / 12 + areaRet2 * Math.Pow(yCGb2, 2);

            // Alma - elemento 3
            double yCGb3;
            if (solicitacao == "COMPRESSAO")
            { yCGb3 = 0; }
            else // Flexao
            { yCGb3 = retB3 * 0.5; }
            double inerciaRet3X = (t * Math.Pow(retB3, 3)) / 12 + areaRet3 * Math.Pow(yCGb3, 2);

            // Mesa - elemento 4
            double yCGb4 = (yCGefetivo - (t * 0.5));
            double inerciaRet4X = (retB4 * Math.Pow(t, 3)) / 12 + areaRet4 * Math.Pow(yCGb4, 2);
            
            // Enrijecedor - elemento 5
            double yCGb5 = Funcoes.novoCGelementos(yCGefetivo, t, b5, retB5);
            double inerciaRet5X = (t * Math.Pow(retB5, 3)) / 12 + areaRet5 * Math.Pow(yCGb5, 2);

            // Enrijecedor - conforme Item 9.2.3.1 (Largura efetiva de elementos comprimidos com enrijecedor)
            double yCGds = Funcoes.novoCGelementos(yCGefetivo, t, b1, retDs);
            double inerciaRetds = (t * Math.Pow(retDs, 3)) / 12 + areaRetds * Math.Pow(yCGds, 2);

            double inerciaRetTotal;
            if (solicitacao == "COMPRESSAO")
            { //retira-se duas vezes a não efetividade do enrijecedor, dado por ds
                inerciaRetTotal = inerciaRet1X + inerciaRet2X + inerciaRet3X + inerciaRet4X + inerciaRet5X + inerciaRetds * 2;
            }
            else // Flexao em X
            {//retira-se uma vez a não efetividade do enrijecedor, dado por ds
                inerciaRetTotal = inerciaRet1X + inerciaRet2X + inerciaRet3X + inerciaRet4X + inerciaRet5X + inerciaRetds;
            }

            double inerciaEfetivaX = iPerfilX - inerciaRetTotal + areaEfetiva * Math.Pow(deslYcg, 2);
            double moduloWXEfetivo = inerciaEfetivaX / yCGefetivo;
            //==================================================================================

            //Inercia a retirar por elemento, em Y
            //Mesa - elemento 2
            double retB2Y = b2 - (b2ef); //TODO ajustar o calculo da Wef, pois esta errado a definição do momento a retirar e Xcg
            double xCGb2;
            if ( b1 == 0) { xCGb2 = b2 - retB2Y - xg; }
            else { xCGb2 = b2 * 0.5 - xg; }
            double areaRetB2y = retB2Y * t;
            double inerciaRetB2Y = (t * Math.Pow(retB2Y, 3) / 12) + areaRetB2y * Math.Pow(xCGb2, 3);

            //Alma - elemento 3
            double retB3Y = b3 - (b3ef + b3ef2);
            double xCGb3 = 0;
            double areaRetB3y = retB3Y * t;
            double inerciaRetB3y = (retB3Y * Math.Pow(t, 3) / 12) + areaRetB3y * Math.Pow(xCGb3, 2);

            areaEfetiva = area - areaRetB2y - areaRetB3y;
            inerciaRetTotal = inerciaRetB3y + inerciaRetB2Y;

            double inerciaEfetivaY = iPerfilY - inerciaRetTotal + areaEfetiva * Math.Pow(deslXcg, 2);
            double moduloWYEfetivo = inerciaEfetivaY / xCGefetivo;

            List<double> retorno = new List<double>();
            retorno.Add(areaEfetiva); retorno.Add(moduloWXEfetivo); retorno.Add(moduloWYEfetivo);
            return retorno;
        }

        void relatorioLargurasEfetivas (List<double> listaDeLarguras)
        {
            string condicao;
            relatorioDeCalculo += "Larguras efetivas dos elementos: (Conforme Item 9.2.2 da NBR 14762:2010) \r\n";
            if (Elemento1 != 0)
            {
                relatorioDeCalculo += "Enrijecedor (Conforme Item 9.2.3.1 da NBR 14762): "
                + listaDeLarguras.ElementAt(1).ToString("#.##") + " cm" + "\r\n";
                condicao = "(AA)";
            }
            else { condicao = "(AL)"; }
            relatorioDeCalculo += "Mesa superior " + condicao + ": " + listaDeLarguras.ElementAt(2).ToString("#.##") + " cm" + "\r\n"
                + "Alma (AA): " + listaDeLarguras.ElementAt(3).ToString("#.##") + " cm" + "\r\n"
                + "Mesa inferior " + condicao + ": " + listaDeLarguras.ElementAt(4).ToString("#.##") + " cm" + "\r\n";
            if (Elemento1 != 5)
            {
                relatorioDeCalculo += "Enrijecedor (Conforme Item 9.2.3.1 da NBR 14762): "
                + listaDeLarguras.ElementAt(5).ToString("#.##") + " cm" + "\r\n";
            }
        }

        void relatorioFlambagemLocal (List<double> listaDeLarguras, double tensao)
        {
            relatorioDeCalculo += "Resistencia a Flambagem Local: \r\n"
                + "Tensão: " + tensao.ToString() + " kN/cm2" + "\r\n\r\n";
            relatorioLargurasEfetivas(listaDeLarguras);
            relatorioDeCalculo += "\r\nArea Efetiva: " + areaEfetivaLocal.ToString("#.##") + " cm2" + "\r\n\r\n"
                + "Resistencia a Flambagem Local: (Conforme Item 9.2 da NBR 14762:2010)\r\n"
                + "Nc,Rsd = " + rsdLocal.ToString("#.##") + " kN" + "\r\n\r\n" 
                + "======================================================================================" + "\r\n\r\n";
        }

        void relatorioTracao()
        {
            relatorioDeCalculo += "Resistencia a Tração da Seção: \r\n"
                + "Nt,Rsd = " + rsdTracao.ToString("#,##") + " kN" + "\r\n\r\n"
                + "======================================================================================" + "\r\n\r\n";
        }

        void relatorioFLexaoSiples(string eixoDaSolicitacao, List<double> listaDeLarguras,double moduloW, double mrd)
        {
            relatorioDeCalculo += "Resistencia a flexão simples com momento fletor em "+ eixoDaSolicitacao + "\r\n"
                + "(Conforme Item 9.8.2.1 da NBR 14762)\r\n\r\n";
            relatorioLargurasEfetivas(listaDeLarguras);
            relatorioDeCalculo += "\r\n" 
                + "Resulta em um modulo de resistencia elastico da seção efetiva (Wef)\r\n "
                + "Wef: " + moduloW.ToString("#.##") + " cm3" + "\r\n\r\n"
                + "Momento Fletor Resistente de cálculo:\r\n"
                + "Mrd = Wef * fy / 1.10 \r\n"
                + "Mrd = " + mrd.ToString("#.##") + " kN.cm" + "\r\n\r\n"
                + "======================================================================================" + "\r\n\r\n";
        }

        void relatorioCompressao(List<double> listaDeLarguras, double tensao)
        {
            relatorioDeCalculo += "Flambagem Global \r\n"
                + "(Conforme Item 9.7.2 NBR 14762:2010)\r\n\r\n"
                + "Fator de redução da força axial de compressão resistente (X) \r\n"
                + "X = " + fatorReducaoCompressao.ToString("#.##") + "\r\n"
                + "Utiliza-se para o calculo das larguras efetivas a tensao obtida pela expressão:\r\n"
                + "tensao = X * fy = " + fatorReducaoCompressao.ToString("#.##") + tensao.ToString() + " = " 
                + (tensao * fatorReducaoCompressao).ToString("#.##") + " kN" + "\r\n\r\n"                
                ;
            relatorioLargurasEfetivas(listaDeLarguras);

            relatorioDeCalculo += "\r\n" + "Resultando em uma area efetiva da seção de: \r\n"
                + "Aef = " + areaEfetivaGlobal.ToString("#.##") + " cm2" + "\r\n\r\n"
                + "Força axial de compressão resistente (Nc,Rd) é dada por:\r\n"
                + "Nc,Rd = X * fy * Aef / 1.20 = " + fatorReducaoCompressao.ToString("#.##") + " * "
                + tensao.ToString() + " * " + areaEfetivaGlobal.ToString("#.##") + " / 1.20\r\n"
                + "Nc,Rd = " + rsdCompressao.ToString("#.##") + " kN" + "\r\n\r\n"
                + "======================================================================================" + "\r\n\r\n";
        }

        void relatorioFlexoTorcao(string eixoDaSolicitacao, double tensao, List<double> listaDeLarguras, double moduloW, double fator, double mrd)
        {
            relatorioDeCalculo += "Resistencia a flambagem lateral com torção com momento fletor em " + eixoDaSolicitacao + "\r\n"
                + "(Conforme Item 9.8.2.2 da NBR 14762)" + "\r\n\r\n"
                + "Considerando Cb = " + Form1.fatorCb.ToString() + "\r\n"
                + "Fator de redução do momento fletor resistente (Xflt) \r\n "
                + "Xflt = " + fator.ToString("#.##") + "\r\n"
                + "Utiliza-se para o calculo das larguras efetivas a tensao obtida pela expressão:\r\n"
                + "tensao = Xflt * fy = " + fator.ToString("#.##") + tensao.ToString() + " = "
                + (tensao*fator).ToString("#.##") + " kN" + "\r\n\r\n"
                ;
            relatorioLargurasEfetivas(listaDeLarguras);
            relatorioDeCalculo += "\r\n" + "Resulta em um modulo de resistencia elastico da seção efetiva (Wc,ef)" + "\r\n"
                + "Wc,ef: " + moduloW.ToString("#.##") + " cm3" + "\r\n\r\n"
                + "Momento Fletor Resistente de cálculo:\r\n"
                + "Mrd = Xflt * Wc,ef * fy / 1.10 \r\n"
                + "Mrd = " + mrd.ToString("#.##") + " kN.cm" + "\r\n\r\n"
                + "======================================================================================" + "\r\n\r\n";
        }

        void relatorioCortante()
        {
            relatorioDeCalculo += "Resistencia ao esforço cortante. \r\n"
                + "(Conforme Item 9.8.3 da NBR 14762:2010)" + "\r\n\r\n"
                + "Considerando kv = 5.0" + "\r\n"
                + "h / t = " + Elemento3.ToString() + " / " + Espessura.ToString() + " \r\n"
                + "h / t = " + (Elemento3 / Espessura).ToString("#.##") + "\r\n"
                + "Vrd = " + vrdCortante.ToString("#.##") + " kN" + "\r\n\r\n"
                + "======================================================================================" + "\r\n\r\n";
        }

        void relatorioDistorcionalComp(double condicao, double almaXt, double mesaXalma, double enrijecedorXalma)
        {

            relatorioDeCalculo += "Verificação se a flambagem distorcional para barras submetidas a compressão pode ser dispensada"
                + "\r\n"
                + "(Conforme Tabela 11 do Item 9.7.3 da NBR 14762:2010)" + "\r\n\r\n"
                + "bw/t = " + almaXt.ToString("#.##") + "\r\n"
                + "bf/bw = " + mesaXalma.ToString("#.##") + "\r\n"
                + "D/bw = " + enrijecedorXalma.ToString("#.##") + "\r\n";
            if (condicao == 0)
            { relatorioDeCalculo += "É necessario realizar a verificação a flambagem distorcional\r\n"; }
            else { relatorioDeCalculo += "A verificação a flambagem distorcional pode ser dispensada\r\n"; }
            relatorioDeCalculo += "======================================================================================" + "\r\n\r\n";
        }

        void relatorioDistorcionalFlex(double condicao, double almaXt, double mesaXalma, double enrijecedorXalma)
        {

            relatorioDeCalculo += "Verificação se a flambagem distorcional para barras submetidas a flexão simples pode ser dispensada"
                + "\r\n"
                + "(Conforme Tabela 14 do Item 9.8.2.3 da NBR 14762:2010)" + "\r\n\r\n"
                + "bw/t = " + almaXt.ToString("#.##") + "\r\n"
                + "bf/bw = " + mesaXalma.ToString("#.##") + "\r\n"
                + "D/bw = " + enrijecedorXalma.ToString("#.##") + "\r\n";
            if (condicao == 1)
            { relatorioDeCalculo += "É necessario realizar a verificação a flambagem distorcional\r\n"; }
            else { relatorioDeCalculo += "A verificação a flambagem distorcional pode ser dispensada\r\n"; }
            relatorioDeCalculo += "======================================================================================" + "\r\n\r\n";
        }


        void flambagemLocal()
        {
            double tensao = Form1.tensaofy;

            List<double> lstLargurasEfetivas = new List<double>();
            List<double> lstPropriedadesEfetivas = new List<double>();

            lstLargurasEfetivas = MLEPerfilUCompressao(this, tensao);
            lstPropriedadesEfetivas = propEfetivasPerfilU(this, lstLargurasEfetivas, "COMPRESSAO");

            areaEfetiva = lstPropriedadesEfetivas.ElementAt(0);
            areaEfetivaLocal = areaEfetiva;

            rsdLocal = NBR.flambagemLocal(areaEfetivaLocal);

            relatorioFlambagemLocal(lstLargurasEfetivas, tensao);
        }

        void tracao()
        {
            rsdTracao = NBR.resistenciaTracao(area);

            relatorioTracao();
        }

        void flexaoSimples()
        {
            double tensao = Form1.tensaofy;

            List<double> lstLargurasEfetivasX = new List<double>();
            List<double> lstPropriedadesEfetivasX = new List<double>();

            lstLargurasEfetivasX = MLEPerfilUFlexaoX(this, tensao);
            lstPropriedadesEfetivasX = propEfetivasPerfilU(this, lstLargurasEfetivasX, "FLEXAOX");
            //areaEfetiva = lstPropriedadesEfetivasX.ElementAt(0);

            if (lstLargurasEfetivasX.ElementAt(6) == Elemento3)
            {
                moduloWxEfetivo = 0;
                mrdFlexaoSimplesx = 0;
            }
            else
            {
                moduloWxEfetivo = lstPropriedadesEfetivasX.ElementAt(1);
                mrdFlexaoSimplesx = NBR.flexaoSimples(moduloWxEfetivo);
            }

            relatorioFLexaoSiples("X", lstLargurasEfetivasX, moduloWxEfetivo, mrdFlexaoSimplesx);

            List<double> lstLargurasEfetivasY = new List<double>();
            List<double> lstPropriedadesEfetivasY = new List<double>();

            lstLargurasEfetivasY = MLEPerfilUFlexaoY(this, tensao);
            lstPropriedadesEfetivasY = propEfetivasPerfilU(this, lstLargurasEfetivasY, "FLEXAOY");
            //areaEfetiva = lstPropriedadesEfetivasY.ElementAt(0);

            moduloWyEfetivo = lstPropriedadesEfetivasY.ElementAt(2);
            mrdFlexaoSimplesy = NBR.flexaoSimples(moduloWyEfetivo);
            relatorioFLexaoSiples("Y", lstLargurasEfetivasY, moduloWyEfetivo, mrdFlexaoSimplesy);
        }

        void compressao()
        {
            string solicitacao = "COMPRESSAO";
            double tensao = Form1.tensaofy;

            fatorReducaoCompressao = NBR.fatorReducaoCompressao(this);
            double tensaoCompressao = tensao * fatorReducaoCompressao;

            List<double> lstLargurasEfetivas = MLEPerfilUCompressao(this, tensaoCompressao);
            List<double> lstPropEfetivas = propEfetivasPerfilU(this, lstLargurasEfetivas, solicitacao);

            areaEfetiva = lstPropEfetivas.ElementAt(0);
            areaEfetivaGlobal = areaEfetiva;

            rsdCompressao = NBR.resistenciaCompressao(fatorReducaoCompressao, this);


            relatorioCompressao(lstLargurasEfetivas, tensao);
        }

        void flexaoTorcao()
        {
            double tensao = Form1.tensaofy;

            Tuple<double, double> fatorReducao = NBR.fatorReducaoFlexao(this);
            //==========================================
            //Calculo da resistencia a Flexao no eixo de simetria (eixo X)
            fatorReducaoFlexaoX = fatorReducao.Item1;
            double tensaoFlexaoX = tensao * fatorReducaoFlexaoX;

            List<double> lstLargurasEfetivasEmX = MLEPerfilUFlexaoX(this, tensaoFlexaoX);
            List<double> lstPropEfetivasEmX = propEfetivasPerfilU(this, lstLargurasEfetivasEmX, "FLEXAOX");

            moduloWxEfetivo = lstPropEfetivasEmX.ElementAt(2);
            mrdFlexaoTorcaoX = NBR.flexaoLateralTorcao(moduloWxEfetivo, fatorReducaoFlexaoX);
            relatorioFlexoTorcao("X", tensao, lstLargurasEfetivasEmX, moduloWxEfetivo, fatorReducaoFlexaoX, mrdFlexaoTorcaoX);
            //==========================================
            //Calculo da resistencia a Flexao no eixo perpendicular ao eixo de simetria (eixo y)
            fatorReducaoFlexaoY = fatorReducao.Item2;
            double tensaoFlexaoY = fatorReducaoFlexaoY * tensao;

            List<double> lstLargurasEfetivasEmY = MLEPerfilUFlexaoY(this, tensaoFlexaoY);
            List<double> lstPropEfetivasEmY = propEfetivasPerfilU(this, lstLargurasEfetivasEmY, "FLEXAOY");

            moduloWyEfetivo = lstPropEfetivasEmY.ElementAt(2);
            mrdFlexaoTorcaoY = NBR.flexaoLateralTorcao(moduloWyEfetivo, fatorReducaoFlexaoY);
            relatorioFlexoTorcao("Y", tensao, lstLargurasEfetivasEmY, moduloWyEfetivo, fatorReducaoFlexaoY, mrdFlexaoTorcaoY);

            NBR.verificacaoDistorcional(Espessura, Elemento1, b2, b3, "FLEXAO");
        }

        void cortante()
        {
            vrdCortante = NBR.cortante(this);
            relatorioCortante();
        }

        void distorcional()
        {
            if (Elemento1 != 0) //Perfil U enrijecido
            {
                List<double> lstDistorcionalCompressao = new List<double>();
                List<double> lstDistorcionalflexao = new List<double>();
                lstDistorcionalCompressao = NBR.verificacaoDistorcional(Espessura, Elemento1, Elemento2, Elemento3, "COMPRESSAO");

                double almaXtComp = lstDistorcionalCompressao.ElementAt(1);
                double mesaXalmaComp = lstDistorcionalCompressao.ElementAt(2);
                double enrijecedorXalmaComp = lstDistorcionalCompressao.ElementAt(3);
                double condicaoComp = lstDistorcionalCompressao.ElementAt(0);
                relatorioDistorcionalComp(condicaoComp, almaXtComp, mesaXalmaComp, enrijecedorXalmaComp);

                if (lstDistorcionalCompressao.ElementAt(0) == 1) { verificacaoDistorcionalCompressao = "Verificar"; }
                else { verificacaoDistorcionalCompressao = "Dispensada"; }

                lstDistorcionalflexao = NBR.verificacaoDistorcional(Espessura, Elemento1, Elemento2, Elemento3, "FLEXAO");

                double almaXtFlex = lstDistorcionalflexao.ElementAt(1);
                double mesaXalmaFlex = lstDistorcionalflexao.ElementAt(2);
                double enrijecedorXalmaFlex = lstDistorcionalflexao.ElementAt(3);
                double condicaoFlex = lstDistorcionalflexao.ElementAt(0);
                relatorioDistorcionalFlex(condicaoFlex, almaXtFlex, mesaXalmaFlex, enrijecedorXalmaFlex);

                if (lstDistorcionalflexao.ElementAt(0) == 1) { verificacaoDistorcionalFlexao = "Verificar"; }
                else { verificacaoDistorcionalFlexao = "Dispensada"; }
            }
            else // Perfil U simples
            {
                verificacaoDistorcionalCompressao = "Dispensada";
                verificacaoDistorcionalFlexao = "Dispensada";
            }
        }

        void flexaoComposta()
        {
            condicaoFlexaoComposta = NBR.flexaoComposta(this);
        }
        
        void flexaoCortante()
        {
            condicaoFlexaoCortanteX = NBR.FlexaoECortanteX(this);
            condicaoFlexaoCortanteY = NBR.FlexaoECortanteY(this);
        }

        public new void Dimensionar()
        {
            relatorioDeCalculo = "";
            largurasDeCalculo();
            propGeometricasPerfilU(this);

            relatorioDeCalculo += "Propriedades Geométricas da seção: \r\n"
                + "Area da seção: " + area.ToString("#.##") + " cm2" + "\r\n"
                + "Momento de Inercia em X: " + iPerfilX.ToString("#.##") + " cm4" + "\r\n"
                + "Momento de Inercia em Y: " + iPerfilY.ToString("#.##") + " cm4" + "\r\n"
                + "Raio de Giração Polar:" + ro.ToString("#.##") + " cm" + "\r\n" + "\r\n" + "\r\n";

            flambagemLocal();
            tracao();
            flexaoSimples();
            compressao();
            flexaoTorcao();
            cortante();
            distorcional();
            flexaoComposta();
            flexaoCortante();
            indiceEsbeltez = NBR.limiteEsbeltez(this);
            valoresMaximos = NBR.larguraEspessura(this);
        }

        public new double PegarYcg() { return ycg; }
        public new double Pegarb2() { return b2; }
        public new string PegarSimetria() { return Simetria; }
        public new double PegarComprimento() { return Comprimento; }

        public override double PegarAlma() { return Elemento3; }
        public override double PegarMesa() { return Elemento2; }
        public override double PegarEnrijecedor() { return Elemento1; }

        public override double PegarArea() { return area; }
        public override double PegarIx() { return iPerfilX; }
        public override double PegarIy() { return iPerfilY; }
        public override double PegarXo() { return xo; }
        public override double PegarRo() { return ro; }
        public override double PegarCW() { return cw; }
        public override double PegarJ() { return jx; }
        public override double PegarJy() { return jy; }
        
        public override double PegarRy() { return ry; }
        public override double PegarRx() { return rx; }
        public override double PegarWx() { return moduloWx; }
        public override double PegarWy() { return moduloWy; }
        public override double PegarAreaEfetiva() { return areaEfetiva; }
        public override double PegarAreaEfetivaLocal() { return areaEfetivaLocal; }
        public override double PegarAreaEfetivaGlobal() { return areaEfetivaGlobal; }


        public override double PegarWxEfetivo() { return moduloWxEfetivo; }
        public override double PegarWyEfetivo() { return moduloWyEfetivo; }
        public override double PegarRsdLocal() { return rsdLocal; }
        public override double PegarRsdCompressao() { return rsdCompressao; }
        public override double PegarRsdTracao() { return rsdTracao; }
        public override double PegarMrdFlexaoSimplesX() { return mrdFlexaoSimplesx; }
        public override double PegarMrdFlexaoSimplesY() { return mrdFlexaoSimplesy; }

        public override double PegarMrdFlexaoTorcaoX() { return mrdFlexaoTorcaoX; }
        public override double PegarMrdFlexaoTorcaoY() { return mrdFlexaoTorcaoY; }

        public override string PegarFlexaoComposta() { return condicaoFlexaoComposta; }
        public override string PegarFlexaoCortanteX() { return condicaoFlexaoCortanteX; }
        public override string PegarFlexaoCortanteY() { return condicaoFlexaoCortanteY; }

        public override double PegarVrdCortante() { return vrdCortante; }

        public override double PegarFatorReducaoCompressao() { return fatorReducaoCompressao; }
        public override double PegarFatorReducaoFlexaoX() { return fatorReducaoFlexaoX; }
        public override double PegarFatorReducaoFlexaoY() { return fatorReducaoFlexaoY; }

        public override string PegarVerificacaoDistorcionalCompressao() { return verificacaoDistorcionalCompressao; }
        public override string PegarVerificacaoDistorcionalFlexao() { return verificacaoDistorcionalFlexao; }
        public override string PegarIndiceEsbeltez() { return indiceEsbeltez; }
        public override string PegarValoresMaximos() { return valoresMaximos; }

        public override string PegarRelatorio() {  return relatorioDeCalculo; }
    }
}
