using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaPawn.Models.Entities
{
    public class Movimiento
    {
        public int DesdeFila { get; set; }
        public int DesdeColumna { get; set; }
        public int HastaFila { get; set; }
        public int HastaColumna { get; set; }
    }
}
