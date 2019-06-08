using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFF
{
    class NBR
    {
        // Calculo das larguras Efetivas conforme Item 9.2.2.1 NBR 14762:2010
        public static double larguraEfetiva(double b, double k, double tensaoAtuante, double t)
        {
            double bef;
            double lambdaP = (b / t) / (0.95 * Math.Sqrt(k * Form1.E / tensaoAtuante));
            if (lambdaP > 0.673)
            {
                bef = (b * (1 - 0.22 / lambdaP) / lambdaP);
                return bef;
            }
            else
            {
                return bef = b;
            }
        }

        // Retorna Largura Efetiva de Elemento com Enrijecedor
        public static List<double> elementoEnrijecido(double b, double D, double def, double tensaoAtuante, double t)
        {
            // Conforme Item 9.2.3.1
            double bef, k;
            double lambdaPo = (b / t) / (0.623 * Math.Sqrt(Form1.E / tensaoAtuante));
            double n; if ((0.582 - .122 * lambdaPo) > 1/3) { n = (0.582 - .122 * lambdaPo); } else { n = (1.0 / 3.0); }           
            double ds;
            List<double> retorno = new List<double> ();

            if (lambdaPo <= 0.673) // Enrijecedor nao necessario
            {
                ds = def; bef = b; retorno.Add(bef); retorno.Add(ds);
                return retorno;
            }
            else
            {
                double Is = (t * Math.Pow(def, 3)) / 12;// Momento de inercia da seção bruta do enrijecedor
                double Ia = 399 * Math.Pow(t, 4) * Math.Pow(0.487 * lambdaPo - 0.328, 3);// Momento de inercia de referencia do enrijecedor de borda
                double IaCondicao = Math.Pow(t, 4) * (56 * lambdaPo + 5);
                if (Ia <= IaCondicao) { }
                else { Ia = IaCondicao; }

                double Isa = Is / Ia; if (Isa > 1) { Isa = 1; }
                ds = Isa * def;

                if (Is / Ia > 1) { Isa = 1; }

                if (D / b <= 0.25)
                {
                    k = 3.57 * Math.Pow((Isa), n) + 0.43;
                }
                else
                {
                    k = (4.82 - 5 * D / b) * Math.Pow(Isa, n) + 0.43;
                }
                if (k > 4) { k = 4; }

                bef = larguraEfetiva(b, k, tensaoAtuante, t);// Largura Efetiva conforme Item 9.2.2.1
                retorno.Add(bef);retorno.Add(ds);
                return retorno;
            }
        }

        // Retorna Larguras Efetivas conforme Tabelas 5 e 6 da NBR 14762:2010
        public static List<double>
            MLECompleta(string condicao, string solicitacao, double tensaoAtuante, double b, double t, double raio, double aux, double Cg)
        {
            double k, bef, befi, befj, tensao1, tensao2, psi;

            if (condicao == "AA") 
            {
                tensao1 = -(Cg - t - raio) * tensaoAtuante / Cg;
                tensao2 = -tensaoAtuante * ((Cg - raio - t)- aux) / Cg;

                psi = tensao2 / tensao1;
                if (solicitacao == "COMPRESSAO")
                {
                    k = 4.0;
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    befi = bef;
                    befj = 0;
                }
                else if (psi < 1.0 & psi > 0) // Caso b, Tabela 5
                {
                    k = 4 + 2 * (1 - psi) + 2 * Math.Pow((1 - psi), 3);
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    if (bef == b) { befi = bef; befj = 0; }
                    else
                    {
                        befi = bef / (3 - psi);
                        befj = bef - befi;
                    }
                }

                else if (psi > -0.236 & psi < 0) // Caso c, Tabela 5
                {
                    k = 4 + 2 * (1 - psi) + 2 * Math.Pow((1 - psi), 3);
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    if (bef == b) { befi = bef; befj = 0; }
                    else
                    {
                        befi = bef / (3 - psi);
                        befj = bef - befi;
                    }
                }

                else // Caso d, Tabela 5
                {
                    k = 4 + 2 * (1 - psi) + 2 * Math.Pow((1 - psi), 3);
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    if (bef == b) { befi = bef; befj = 0; }
                    else
                    {
                        befi = bef / (3 - psi);
                        befj = bef * 0.5;
                    }
                    
                }
            }
            else //Elemento AL
            {
                tensao1 = -(Cg - t - raio) * tensaoAtuante / Cg;
                tensao2 = -tensaoAtuante * (Cg - aux ) / Cg;

                psi = tensao2 / tensao1;
                if (solicitacao == "COMPRESSAO")
                {
                    k = 0.43;
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    befi = bef;
                    befj = 0;
                }
                else if (psi < 1.0 & psi > 0) // Caso b, Tabela 6
                {
                    k = 0.578 / (psi + 0.34);
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    befi = bef;
                    befj = 0;
                }
                else if (psi > -1.0 & psi < 0) // Caso c, Tabela 6
                {
                    k = 1.7 - 5 * psi + 17.1 * Math.Pow(psi, 2);
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    befi = bef;
                    befj = 0;
                }
                else // Caso d, Tabela 6
                {
                    k = 0.57 - 0.21 * psi + 0.07 * Math.Pow(psi, 2);
                    bef = larguraEfetiva(b, k, tensaoAtuante, t);
                    befi = bef;
                    befj = 0;
                }
            }
            List<double> retorno = new List<double>(); retorno.Add(befi); retorno.Add(befj);
            return retorno;
        }

        // Retorna RsdLocal e AreaEfetiva no ELU
        public static double flambagemLocal(double areaEf)
        {
            double rsd = areaEf * Form1.tensaofy / 1.20;
            return rsd;            
        }

        //Retorna RsdT
        public static double resistenciaTracao(double area)
        {
            double rsdTracao = area * Form1.tensaofy / 1.1;
            return rsdTracao;
        }

        // Retorna (Ne (0), Nex (1), Ney(2), Nez(3), Nexz(4))
        public static List<double> forcaAxialFlambagemGlobal(Perfil perfil, string simetria)
        {
            double pi2 = Math.Pow(Math.PI, 2);
            double Ne = 0;
            double comp = perfil.PegarComprimento();
            double iPerfilX = perfil.PegarIx();
            double iPerfilY = perfil.PegarIy();
            double ro = perfil.PegarRo();
            double xo = perfil.PegarXo();
            double cw = perfil.PegarCW();
            double J = perfil.PegarJ();

            //dupla simetria
            double Nex = pi2 * Form1.E * iPerfilX / (Math.Pow(comp, 2));
            double Ney = pi2 * Form1.E * iPerfilY / (Math.Pow(comp, 2));
            double Nez = 1 / Math.Pow(ro, 2) * ((pi2 * Form1.E * cw) / (Math.Pow(comp, 2)) + (Form1.G * J));

            //monossimetricos
            double _1xoro = 1 - Math.Pow(xo / ro, 2);
            double Nexz = (Nex + Nez) / (2 * _1xoro) * (1 - Math.Sqrt(1 - (4 * Nex * Nez * _1xoro) / (Math.Pow(Nex + Nez, 2))));

            //assimetricos
            // A ser desenvolvido... equação de 3 grau

            // Retorna o menor valor 
            if (simetria == "MONOSSIMETRICO") { Ne = Math.Min(Ney, Nexz); }
            else { Ne = Math.Min(Ney, Math.Min(Nex, Nez)); }

            List<double> retorno = new List<double>();
            retorno.Add(Ne); retorno.Add(Nex); retorno.Add(Ney); retorno.Add(Nez); retorno.Add(Nexz);

            return retorno;
        }

        // Obtem o Fator de Redução à compressao
        public static double fatorReducaoCompressao(Perfil perfil)
        {
            string simetria = perfil.PegarSimetria();
            List<double> lstNe = forcaAxialFlambagemGlobal(perfil, simetria);
            double Ne = lstNe.ElementAt(0);

            //(areaT, inerciaPerfilX, inerciaPerfilY, ro, xo, CW, J, modulosW)
            double areaTotal = perfil.PegarArea();
            double qui; //Fator de Redução da Força Axial de Compressao Resistente
            double lambda0 = Math.Sqrt(areaTotal * Form1.tensaofy / Ne);
            double lambda0quadrado = Math.Pow(lambda0, 2);

            if (lambda0 > 1.5) { qui = 0.877 / lambda0quadrado; }
            else { qui = Math.Pow(0.658, lambda0quadrado); }

            double tensaoCompressao = Form1.tensaofy * qui;

            return qui;
        }

        //Realiza a verificação à compressao do Perfil
        public static double resistenciaCompressao(double qui, Perfil perfil)
        {
            double rsdCompressao = perfil.PegarAreaEfetivaGlobal() * qui * Form1.tensaofy / 1.20;
            return rsdCompressao;
        }

        //Retorna o Momento Fletor Resistente da seção efetiva
        public static double flexaoSimples(double wXEfetivo)
        {
            double mrd = wXEfetivo * Form1.tensaofy / 1.1;
            return mrd;
        }

        public static Tuple<double,double> fatorReducaoFlexao(Perfil perfil)
        {
            double wcx = perfil.PegarWx();
            double wcy = perfil.PegarWy();
            double ro = perfil.PegarRo();
            string simetria = perfil.PegarSimetria();

            double tensao = Form1.tensaofy;

            List<double> lstForcaAxial = forcaAxialFlambagemGlobal(perfil, simetria);
            double ney = lstForcaAxial.ElementAt(2);
            double nez = lstForcaAxial.ElementAt(3);

            double mex = 1.0 * ro * Math.Sqrt(ney * nez);
            double mey = calculoMeFlexaoY(perfil);

            double lambdaZerox = Math.Sqrt(wcx * tensao) / mex;
            double lambdaZeroy = Math.Sqrt(wcy * tensao) / mey;

            double quiFltx = condicaoFLTFlexao(lambdaZerox);
            double quiFlty = condicaoFLTFlexao(lambdaZeroy);

            Tuple<double, double> quiFlt = new Tuple<double, double>(quiFltx, quiFlty);
            return quiFlt;
        }

        public static double flexaoLateralTorcao(double wEfetivo, double qui)
        {
            double tensao = Form1.tensaofy;
            double mrd = qui * wEfetivo * tensao / 1.10;
            return mrd;
        }

        public static double cortante(Perfil perfil) // DImensionamento da alma
        {
            double vrd = 0;
            double t = perfil.Espessura;
            double raio = perfil.Raio;
            double h = perfil.Elemento3 - 2 * (raio + t);

            double E = Form1.E;
            double tensao = Form1.tensaofy;
            double kv = 5;

            double primeiraCondicao = 1.08 * Math.Sqrt(E * kv / tensao);
            double segundaCondicao = 1.40 * Math.Sqrt(E * kv / tensao);

            if(h/t <= primeiraCondicao) { vrd = 0.6 * tensao * t * h / 1.1; }
            else if(h/t > primeiraCondicao & h/t <= segundaCondicao) { vrd = 0.65 * Math.Pow(t, 2) * Math.Sqrt(E * kv * tensao) / 1.1; }
            else { vrd = 0.905 * E * kv * Math.Pow(t, 3) / (h * 1.10); }

            return vrd;
        }

        public static string flexaoComposta(Perfil perfil)
        {
            string condicao;
            double precondicao;
            double nsd = Form1.solicitacaoNormal;
            double msdX = Form1.solicitacaoMomentoX;
            double msdY = Form1.solicitacaoMomentoY;
            double nrd, mrdX, mrdY;

            if (perfil.PegarRsdCompressao() > perfil.PegarRsdLocal()) { nrd = perfil.PegarRsdLocal(); }
            else { nrd = perfil.PegarRsdCompressao(); }

            if (perfil.PegarMrdFlexaoSimplesX() > perfil.PegarMrdFlexaoTorcaoX()) { mrdX = perfil.PegarMrdFlexaoTorcaoX(); }
            else { mrdX = perfil.PegarMrdFlexaoSimplesX(); }

            if (perfil.PegarMrdFlexaoSimplesY() > perfil.PegarMrdFlexaoTorcaoY()) { mrdY = perfil.PegarMrdFlexaoTorcaoY(); }
            else { mrdY = perfil.PegarMrdFlexaoSimplesY(); }

            precondicao = Math.Round(nsd / nrd + msdX / mrdX + msdY / mrdY, 1);

            if(precondicao <= 1) { condicao = "= " + precondicao.ToString() + " < 1, OK"; }
            else { condicao = "= " + precondicao.ToString() + " > 1, Não OK"; }

            return condicao;
        }

        public static string FlexaoECortanteX(Perfil perfil)
        {
            string condicao;
            double precondicao;
            double vsd = Form1.solicitacaoCortante;
            double msdX = Form1.solicitacaoMomentoX;
            double vrd = perfil.PegarVrdCortante(), mrd = perfil.PegarMrdFlexaoSimplesX();

            precondicao = Math.Pow(msdX / mrd, 2) + Math.Pow(vsd / vrd, 2);

            if(precondicao <= 1) { condicao = precondicao.ToString("#.##") + " <= 1.0, Ok"; }
            else { condicao = precondicao.ToString("#.##") + " > 1.0, Não  Ok"; }

            return condicao;

        }

        public static string FlexaoECortanteY(Perfil perfil)
        {
            string condicao;
            double precondicao;
            double vsd = Form1.solicitacaoCortante;
            double msdY = Form1.solicitacaoMomentoX;
            double vrd = perfil.PegarVrdCortante(), mrd = perfil.PegarMrdFlexaoSimplesY();

            precondicao = Math.Pow(msdY / mrd, 2) + Math.Pow(vsd / vrd, 2);

            if (precondicao <= 1) { condicao = precondicao.ToString("#.##") + " <= 1.0, Ok"; }
            else { condicao = precondicao.ToString("#.##") + " > 1.0, Não  Ok"; }

            return condicao;

        }

        public static string limiteEsbeltez(Perfil perfil)
        {
            string retorno = "";
            double comprimento = perfil.PegarComprimento();
            double rx = perfil.PegarRx();
            double ry = perfil.PegarRy();
            double r = Math.Min(rx, ry);

            double limite = comprimento / r;

            if (limite > 300) //Limite de esbeltez para Tração.
            {
                retorno += "Limite do indice de eslbeltez à tração excedido.";
            }
            else if (limite > 200)
            {
                retorno += "Limite do indice de eslbeltez à tração excedido.";
            }
            else
            {
                retorno = "Indice de esbeltez: OK";
            }
            return retorno;
        }

        public static string larguraEspessura(Perfil perfil)// TODO REALIZAR A VERIFICAÇÃO DOS VALORES MAXIMOS
        { // Item 9.1.2
            string retorno = "";
            double contador = 0;

            double t = perfil.PegarEspessura();
            double alma = perfil.PegarAlma();
            double mesa = perfil.PegarMesa();
            double enrijecedor = perfil.PegarEnrijecedor();

            double lambdaPo = (mesa / t) / (0.623 * Math.Sqrt(Form1.E / Form1.tensaofy));

            double Is = (t * Math.Pow(enrijecedor, 3)) / 12;// Momento de inercia da seção bruta do enrijecedor
            double Ia = 399 * Math.Pow(t, 4) * Math.Pow(0.487 * lambdaPo - 0.328, 3);// Momento de inercia de referencia do enrijecedor de borda

            double caso1 = mesa / t;
            double caso2 = alma / t;
            double caso3 = enrijecedor / t;

           if (caso1 > 60) { contador += 1; }
           else if (caso1 > 90 & Is >= Ia) { contador += 1; }
           else if (caso2 > 90) { contador += 1; }
           else if (caso3 > 60 | caso2 > 60 & Is < Ia) { contador += 1; }
           else { contador += 0; }

           if(contador == 0) { retorno = "ok"; }
           else { retorno = "Verificar"; }

           return retorno;
        }

        //Verifica o pefil conforme a Tabela 11 e Tabela 14, Item 9.7.3 NBR 14762:2010
        public static List<double> verificacaoDistorcional(double espessura, double enrijecedor, double elementoB2, double elementoB3, string solicitacao)
        {
            double t = espessura;
            double D = enrijecedor;
            double b2 = elementoB2;
            double b3 = elementoB3;
            double mensagem; // 1 = Não OK, 0 = OK

            double almaXt = b3 / t;
            double mesaXalma = b2 / b3;
            double enrijecedorXmesa = D / b2;
            
            //==========================================
            //Tabela 11, NBR 14762:2010
            List<double> lstAlmaXt = new List<double>();
            lstAlmaXt.Add(250); lstAlmaXt.Add(200); lstAlmaXt.Add(125); lstAlmaXt.Add(100); lstAlmaXt.Add(50);

            List<double> lstMesaXAlma = new List<double>();
            lstMesaXAlma.Add(0.4); lstMesaXAlma.Add(0.6); lstMesaXAlma.Add(0.8); lstMesaXAlma.Add(1.0); lstMesaXAlma.Add(1.2);
            lstMesaXAlma.Add(1.4); lstMesaXAlma.Add(1.6); lstMesaXAlma.Add(1.8); lstMesaXAlma.Add(2.0);

            List<List<double>> lstLimites = new List<List<double>>();
            if (solicitacao == "COMPRESSAO")
            {
                lstLimites.Add(new List<double> { 0.02, 0.03, 0.05, 0.06, 0.06, 0.06, 0.07, 0.07, 0.07 });
                lstLimites.Add(new List<double> { 0.03, 0.04, 0.06, 0.07, 0.07, 0.08, 0.08, 0.08, 0.08 });
                lstLimites.Add(new List<double> { 0.04, 0.06, 0.08, 0.10, 0.12, 0.12, 0.12, 0.12, 0.12 });
                lstLimites.Add(new List<double> { 0.04, 0.06, 0.10, 0.12, 0.15, 0.15, 0.15, 0.15, 0.15 });
                lstLimites.Add(new List<double> { 0.08, 0.15, 0.22, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27 });
            }
            else
            {
                lstLimites.Add(new List<double> { 0.05, 0.05, 0.05, 0.05, 0.05, 0.05, 0.05, 0.05, 0.05 });
                lstLimites.Add(new List<double> { 0.06, 0.06, 0.06, 0.06, 0.06, 0.06, 0.06, 0.06, 0.06 });
                lstLimites.Add(new List<double> { 0.10, 0.10, 0.09, 0.09, 0.09, 0.09, 0.09, 0.09, 0.09 });
                lstLimites.Add(new List<double> { 0.12, 0.12, 0.12, 0.11, 0.11, 0.10, 0.10, 0.10, 0.10 });
                lstLimites.Add(new List<double> { 0.25, 0.25, 0.22, 0.22, 0.20, 0.20, 0.20, 0.19, 0.19 });
            }
            
            //==========================================

            if (lstAlmaXt.Contains(almaXt))// verifica se a relação possui um valor igual aos indices da coluna
            {
                int i = lstAlmaXt.IndexOf(almaXt);  
                if (lstMesaXAlma.Contains(mesaXalma)) // verifica se a a relação possui um valor igual aos indices da linha
                {
                    int j = lstMesaXAlma.IndexOf(mesaXalma);

                    if (lstLimites.ElementAt(i).ElementAt(j) > enrijecedorXmesa) // verifica se a relação do perfil é inferior ao da tabela
                    {
                        mensagem = 1; //caso seja inferior, mensagem será "nao ok"
                    }
                    else { mensagem = 0; } // caso seja superior, mensagem sera "ok"
                }
                else // caso a relação nao seja igual a da linha, sera feita a interpolação
                {
                    int j1 = NBR.interpLstDistMesaAlma(lstMesaXAlma, mesaXalma).Item1;
                    int j2 = NBR.interpLstDistMesaAlma(lstMesaXAlma, mesaXalma).Item2;
                    if (j1 == j2) //caso a interpolação obtenha o mesmo indice, valor menor que o primeiro indice ( valor menor que 0,4)
                    {
                        double referencia = lstLimites.ElementAt(i).ElementAt(j1);
                        if (referencia > enrijecedorXmesa)// verifica se a relação do perfil é inferior ao da tabela
                        { mensagem = 1; } //caso seja inferior, mensagem será "nao ok"
                        else { mensagem = 0; }// caso seja superior, mensagem sera "ok"
                    }
                    else
                    {
                        double deltaSup =  //variação entre indices de linha
                            Math.Abs(lstMesaXAlma.ElementAt(j1) - lstMesaXAlma.ElementAt(j2));
                        double deltaInf =  //variação entre valores minimos
                            Math.Abs(lstLimites.ElementAt(i).ElementAt(j1) - lstLimites.ElementAt(i).ElementAt(j2));
                        double referencia =  // valor interpolado - (valor maior - relação) * (variação por unidade) + valor minimo do maior indice
                             Math.Abs((lstMesaXAlma.ElementAt(j1) - mesaXalma) * (deltaInf / deltaSup) - lstLimites.ElementAt(i).ElementAt(j1));
                        if (referencia > enrijecedorXmesa)// verifica se a relação do perfil é inferior ao da tabela
                        { mensagem = 1; }//caso seja inferior, mensagem será "nao ok"
                        else { mensagem = 0; }// caso seja superior, mensagem sera "ok"
                    }
                }
            }
            else //interpolar indice de coluna
            {
                int i1 = NBR.interpListDistAlmaT(lstAlmaXt, almaXt).Item1;
                int i2 = NBR.interpListDistAlmaT(lstAlmaXt, almaXt).Item2;
                int j;

                if (lstMesaXAlma.Contains(mesaXalma)) //se relação mesa x alma esta contido no indice de linha
                {
                    j = lstMesaXAlma.IndexOf(mesaXalma);
                }
                else // valores de indices de linha interpolados
                {
                    int j1 = NBR.interpLstDistMesaAlma(lstMesaXAlma, mesaXalma).Item1;
                    int j2 = NBR.interpLstDistMesaAlma(lstMesaXAlma, mesaXalma).Item2;
                    if (j1 == j2) { j = j1; }
                    else { j = j2; }
                }
                if (i1 == i2) // caso a realção tenha valor menor ou maior que o indice
                {
                    double referencia = lstLimites.ElementAt(i1).ElementAt(j);
                    if (referencia > enrijecedorXmesa) { mensagem = 1; }
                    else { mensagem = 0; }
                }
                else //interpolação dos valores
                {
                    double deltaSup = Math.Abs(lstAlmaXt.ElementAt(i2) - lstAlmaXt.ElementAt(i1)); //delta coluna
                    double deltaInf = Math.Abs((lstLimites.ElementAt(i2)).ElementAt(j) - (lstLimites.ElementAt(i1)).ElementAt(j));//delta linha
                    double referencia = (lstAlmaXt.ElementAt(i2) - almaXt) * ( deltaInf / deltaSup) + (lstLimites.ElementAt(i2)).ElementAt(j);
                    if (referencia > enrijecedorXmesa) { mensagem = 1; }
                    else { mensagem = 0; }
                }
            }

            List<double> retorno = new List<double>();
            retorno.Add(mensagem); retorno.Add(almaXt); retorno.Add(mesaXalma); retorno.Add(enrijecedorXmesa);

            return retorno;
        }

        public static Tuple<int, int> interpLstDistMesaAlma(List<double> lista, double relacaoIndice)
        {
            int k1 = 0, k2 = 0;
            foreach (double k in lista)
            {
                if (k > relacaoIndice)
                {
                    k1 = lista.IndexOf(k);
                    if (k1 - 1 < 0)
                    {
                        k2 = k1;
                    }
                    else
                    {
                        k2 = k1 - 1;
                    }
                    break;
                }
                else
                {
                    if (lista.IndexOf(k) < lista.Count) { continue; }
                    else { k1 = lista.IndexOf(k); k2 = k1; }
                }
            }

            return new Tuple<int, int>(k1, k2);
        }

        public static Tuple<int, int> interpListDistAlmaT(List<double> lista, double relacaoIndice)
        {
            int k1 = 0, k2 = 0;
            foreach (int k in lista)
            {
                if (k < relacaoIndice) 
                {
                    k1 = lista.IndexOf(k);
                    if (k1 - 1 < 0)
                    {
                        k2 = k1;
                    }
                    else
                    {
                        k2 = k1 - 1;
                    }
                    break;
                }
                else
                {
                    if (lista.IndexOf(k) < lista.Count) { continue; }
                    else { k1 = lista.IndexOf(k); k2 = k1; }
                }
            }
            return new Tuple<int, int>(k1, k2);
        }

        public static double calculoMeFlexaoY(Perfil perfil)
        {
            double ro = perfil.PegarRo();
            double jy = perfil.PegarJy();
            string simetria = perfil.PegarSimetria();

            List<double> lstForcaAxial = forcaAxialFlambagemGlobal(perfil, simetria);

            double nex = lstForcaAxial.ElementAt(1);
            double nez = lstForcaAxial.ElementAt(3);

            // Considerando Cs = + 1.0 e Cm = 1.0
            double me = nex * (jy + Math.Sqrt(Math.Pow(jy, 2) + Math.Pow(ro, 2) * (nez / nex)));
            
            return me;
        }

        public static double condicaoFLTFlexao(double lambda)
        {
            double qui;

            if(lambda <= 0.6)
            {
                qui = 1.0;
            }
            else if (lambda < 1.336)
            {
                qui = 1.11 * (1 - 0.278 * Math.Pow(lambda, 2));
            }
            else
            {
                qui = 1 / Math.Pow(lambda, 2);
            }

            return qui;
        }
    }
}

