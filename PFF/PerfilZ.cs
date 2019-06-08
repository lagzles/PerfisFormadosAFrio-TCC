using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFF
{
    class PerfilZ : Perfil
    {
        public PerfilZ(
            double el1, 
            double el2, 
            double el3, 
            double el4, 
            double el5, 
            double espessura, 
            double raioi, 
            double comprimento, 
            string simetria) : base(el1, el2, el3, el4, el5, espessura, raioi, comprimento, simetria)
        {
        }
    }
}
