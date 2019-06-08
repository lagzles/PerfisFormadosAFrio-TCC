using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFF
{
    class Funcoes
    {
        public static double B1, B2, B3, B4, B5;
        private static double inerciaPerfilX, inerciaPerfilY, areaTotal;

        // Retorna (areaT, inerciaPerfilX, inerciaPerfilY, ro, xo, CW, J, modulosW), onde modulosW = (moduloWx, moduloWy)        
        public static void propGeometricasPerfilU()
        {
            double  xg, rx, ry, ro;
            
            double t = Form1.espessura; double b1 = Form1.b1; double b2 = Form1.b2;
            double b3 = Form1.b3; double b4 = Form1.b4; double b5 = Form1.b5;
            double yCG = Form1.yCG; double raio = Form1.raio;

            areaTotal = t * (b1 + b2 + b3 + b4 + b5);
            Form1.areaTotal = areaTotal;
            xg = (b2 * t * (b2 + 2 * b1)) / areaTotal;
            Form1.xg = xg;

            //Mesa
            double momentobx2 = (b2 * Math.Pow(t, 3)) / 12;
            double momentoby2 = (t * Math.Pow(b2, 3)) / 12;
            double areaB2 = b2 * t;
            double yB2 = yCG - t / 2; //Distancia do eixo até a linha media da mesa

            //Alma
            double momentobx3 = (t * Math.Pow(b3, 3)) / 12;
            double momentoby3 = (b3 * Math.Pow(t, 3)) / 12;
            double areaB3 = b3 * t;

            //Enrijecedores
            double momentobx1 = (t * Math.Pow(b1, 3)) / 12;
            double momentoby1 = (b1 * Math.Pow(t, 3)) / 12;
            double areaB1 = b1 * t;
            double yB1 = yCG - (t + b1 * 0.5 );

            //Momento em X
            inerciaPerfilX = 2 * (momentobx2 + areaB2 * Math.Pow(yB2,2)) + 2 * (momentobx1 + areaB1 * Math.Pow(yB1,2))
                + momentobx3;
            Form1.iPerfilX = inerciaPerfilX;
            rx = Math.Sqrt(Form1.iPerfilX / areaTotal);
            Form1.rx = rx;

            Form1.moduloWx = Form1.iPerfilX / yCG;

            // Calculo do xo, cw, J

            // Momento em Y 
            inerciaPerfilY  = (momentoby3 + areaB3 * Math.Pow(xg,2)) + 
                2 * (momentoby2 + areaB2 * Math.Pow(b2 * 0.5 - xg,2)) + 
                2 * (momentoby1 + areaB1 * Math.Pow(b2 - xg - t * 0.5,2));

            Form1.iPerfilY = inerciaPerfilY;
            ry = Math.Sqrt(Form1.iPerfilY / areaTotal);
            Form1.ry = ry;

            Form1.moduloWy = Form1.iPerfilY / yCG;
            
            if (b1 == 0)
            {
                Form1.xo = (Math.Pow(b2, 2) / (b3 + 2 * b2)) + (3 * Math.Pow(b2, 2)) / (6 * b2 + b3); //Wei-Wu pg. 385
                Form1.J = (1 / 3 * Math.Pow(t,3) * (b3 + 2 * b2));
                Form1.cw = ((t * Math.Pow(b3, 2)* Math.Pow(b2, 3)) / 12 ) * ((3 * b2 + 2 * b3) / (6 * b2 + b3));
            }
            else
            { // Wei Wu pg. 385
                Form1.xo = xg +  b2 * ((6 * Math.Pow(b3, 2) * b1 + 3 * b2 * Math.Pow(b3, 2)  - 8 * Math.Pow(b1, 3)) /
                    (Math.Pow(b3, 3) + 6 * b2 * Math.Pow(b3, 2) + 6 * b1 * Math.Pow(b3, 2) 
                    - 12 * b3 * Math.Pow(b1, 2) + 8 * Math.Pow(b1, 3)));
                Form1.J = Math.Pow(t, 3) * (b3 + 2 * b2 + 2 * b1) / 3;
                double Ix = t / 12 * (Math.Pow(b3, 3) + 6 * b2 * Math.Pow(b3, 2) + 6 * b1 * Math.Pow(b3, 2)
                    - 12 * b3 * Math.Pow(b1, 2) + 8 * Math.Pow(b1, 3));
                double xBarra = b2 * t * (b2 + 2 * b1) / areaTotal;
                double m = b2 * t / (12 * Ix) * (6 * b1 * Math.Pow(b3, 2) + 3 * b2 * Math.Pow(b3, 2) - 8 * Math.Pow(b1, 3));
                Form1.cw = Math.Pow(t, 2) / areaTotal * (xBarra * areaTotal * Math.Pow(b3, 2) / t * (Math.Pow(b2, 2) / 3 + Math.Pow(m, 2) - m * b2) +
                    areaTotal / (3 * t) * (Math.Pow(m, 2) * Math.Pow(b3, 3) + Math.Pow(b2, 2) * Math.Pow(b1, 2) * (2 * b1 + 3 * b3)) -
                    Ix * Math.Pow(m, 2) / t * (2 * b3 + 4 * b1) +
                    m * Math.Pow(b1, 2) / 3 * (8 * Math.Pow(b2, 2) * b1 + 2 * m * (2 * b1 * (b1 - b3) + b2 * (2 * b1 - 3 * b3))) +
                    Math.Pow(b2, 2) * Math.Pow(b3, 2) / 6 * ((3 * b1 + b2) * (4 * b1 + b3) - 6 * Math.Pow(b1, 2)) -
                    Math.Pow(m, 2) * Math.Pow(b3, 4) / 4);
            }
            ro = Math.Sqrt(Math.Pow(rx, 2) + Math.Pow(ry, 2) + Math.Pow(Form1.xo, 2));
            Form1.ro = ro;
            
        }


        //Retorna a nova coordenada yCG do elemento
        public static double novoCGelementos(double yCG, double espessuraT, double b, double retB)
        {
            double yCGnovo = (yCG - (espessuraT * 0.5 + (b - retB * 0.5)));
            return yCGnovo;
        }

        public static double deslocamentoCG (double area, double areaRetirar, double t, double CG)
        {
            double deslCG = areaRetirar * (CG - 0.5 * t) / area;
            return deslCG;
        }
        
        public static void largurasDeCalculo(List<double> listaElementos)
        {
            //double b1, b2, b3, b4, b5;
            double espessura = listaElementos.ElementAt(0); double raio = listaElementos.ElementAt(7);
            if (listaElementos.ElementAt(1) != 0)
            {
                Funcoes.B1 = listaElementos.ElementAt(1) - raio;
                double elemento1 = Funcoes.B1 + 2 * espessura;
            }
            else { B1 = 0; }            

            if (Funcoes.B1 <= 0) { Funcoes.B2 = listaElementos.ElementAt(2) - 1 * raio; }
            else { Funcoes.B2 = listaElementos.ElementAt(2) - 2 * raio; }

            Funcoes.B3 = listaElementos.ElementAt(3) - 2 * raio;
            if (listaElementos.ElementAt(5) != 0)
            {
                Funcoes.B5 = listaElementos.ElementAt(5) - raio;
                double elemento5 = Funcoes.B5 + 2 * espessura;
            }
            else { B5 = 0; }            

            if (Funcoes.B5 <= 0) { Funcoes.B4 = listaElementos.ElementAt(4) - raio; }
            else { B4 = listaElementos.ElementAt(4) - 2 * raio; }
            
        }

       
    }
}
