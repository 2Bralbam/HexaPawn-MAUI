using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HexaPawn.Models.Entities
{
    public partial class Tablero : ObservableObject
    {
        [ObservableProperty]
        public char[,] casillas;

        public Tablero()
        {
            Casillas = new char[3, 3];
            InicializarTablero();
        }

        private void InicializarTablero()
        {
            for (int i = 0; i < 3; i++)
            {
                Casillas[0, i] = 'O';
                Casillas[1, i] = ' ';
                Casillas[2, i] = 'X';
            }
        }

        public void ImprimirTablero()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(Casillas[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }

}
