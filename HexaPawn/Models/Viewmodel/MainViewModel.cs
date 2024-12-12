using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HexaPawn.Models.Entities;

namespace HexaPawn.Models.Viewmodel
{
    public partial class MainViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string casilla00;
        [ObservableProperty]
        private string casilla01;
        [ObservableProperty]
        private string casilla02;
        [ObservableProperty]
        private string casilla10;
        [ObservableProperty]
        private string casilla11;
        [ObservableProperty]
        private string casilla12;
        [ObservableProperty]
        private string casilla20;
        [ObservableProperty]
        private string casilla21;
        [ObservableProperty]
        private string casilla22;

        [ObservableProperty]
        private Tablero table;

        private bool primerClick = true; 
        private int filaSeleccionada;
        private int columnaSeleccionada;
        public MainViewModel()
        {
            Table = new Tablero();
            Update();
        }
        [RelayCommand]
        public void Boton(string numero)
        {
            int numeroBoton = int.Parse(numero);

            int fila = (numeroBoton - 1) / 3;
            int columna = (numeroBoton - 1) % 3;

            if (primerClick)
            {
                if (Table.Casillas[fila, columna] == 'X')
                {
                    filaSeleccionada = fila;
                    columnaSeleccionada = columna;
                    primerClick = false;
                }
            }
            else
            {
                if (MovimientoValido(filaSeleccionada, columnaSeleccionada, fila, columna, 'X'))
                {
                    Table.Casillas[filaSeleccionada, columnaSeleccionada] = ' ';
                    Table.Casillas[fila, columna] = 'X';
                    Update();

                    if (JuegoTerminado(Table, out string ganador))
                    {
                        FinalMessage($"¡Felicidades! {ganador} ha ganado.");
                        return;
                    }

                    var mejorMovimiento = ObtenerMoves();
                    if (mejorMovimiento != null)
                    {
                        Table.Casillas[mejorMovimiento.DesdeFila, mejorMovimiento.DesdeColumna] = ' ';
                        Table.Casillas[mejorMovimiento.HastaFila, mejorMovimiento.HastaColumna] = 'O';
                        Update();

                        if (JuegoTerminado(Table, out ganador))
                        {
                            FinalMessage($"{ganador} ha ganado.");
                            return;
                        }
                    }
                }

                primerClick = true;
            }
        }


        private void Update()
        {
            Casilla00 = Table.Casillas[0, 0].ToString();
            Casilla01 = Table.Casillas[0, 1].ToString();
            Casilla02 = Table.Casillas[0, 2].ToString();
            Casilla10 = Table.Casillas[1, 0].ToString();
            Casilla11 = Table.Casillas[1, 1].ToString();
            Casilla12 = Table.Casillas[1, 2].ToString();
            Casilla20 = Table.Casillas[2, 0].ToString();
            Casilla21 = Table.Casillas[2, 1].ToString();
            Casilla22 = Table.Casillas[2, 2].ToString();
        }

        private bool MovimientoValido(int filaInicial, int columnaInicial, int filaDestino, int columnaDestino, char jugador)
        {
            char piezaDestino = Table.Casillas[filaDestino, columnaDestino];
            if (piezaDestino != ' ' && piezaDestino != (jugador == 'X' ? 'O' : 'X'))
            {
                return false;
            }

            if (jugador == 'X')
            {
                //adelante
                if (filaDestino == filaInicial - 1 && columnaDestino == columnaInicial && piezaDestino == ' ')
                {
                    return true;
                }
                //diagonal
                if (filaDestino == filaInicial - 1 && (columnaDestino == columnaInicial - 1 || columnaDestino == columnaInicial + 1) && piezaDestino == 'O')
                {
                    return true;
                }
            }
            else if (jugador == 'O')
            {
                //adelante
                if (filaDestino == filaInicial + 1 && columnaDestino == columnaInicial && piezaDestino == ' ')
                {
                    return true;
                }
                //diagonal
                if (filaDestino == filaInicial + 1 && (columnaDestino == columnaInicial - 1 || columnaDestino == columnaInicial + 1) && piezaDestino == 'X')
                {
                    return true;
                }
            }

            return false;
        }



        private bool VerificarMovimientos(char jugador)
        {
            List<Movimiento> movimientos = ObtenerTodosMovimientos(Table, jugador);
            return movimientos.Count == 0;
        }


        private Movimiento ObtenerMoves()
        {
            List<Movimiento> movimientos = ObtenerTodosMovimientos(Table, 'O');
            Movimiento mejorMovimiento = null;
            int mejorValor = int.MinValue;

            var random = new Random();
            foreach (var movimiento in movimientos)
            {
                Tablero nuevoTablero = HacerMovimiento(Table, movimiento);
                int valor = Minimax(nuevoTablero, 3, false);
                if (valor > mejorValor)
                {
                    mejorValor = valor;
                    mejorMovimiento = movimiento;
                }
                else if (valor == mejorValor && random.Next(2) == 0)
                {
                    mejorMovimiento = movimiento;
                }
            }

            return mejorMovimiento;
        }



        private int Minimax(Tablero tablero, int profundidad, bool jugadorMaximizador)
        {
            char jugador = jugadorMaximizador ? 'X' : 'O';

            if (profundidad == 0 || JuegoTerminado(tablero, out _))
            {
                return Heuristica(tablero, jugador);
            }

            if (jugadorMaximizador)
            {
                int maxEval = int.MinValue;
                foreach (var movimiento in ObtenerTodosMovimientos(tablero, 'X'))
                {
                    Tablero nuevoTablero = HacerMovimiento(tablero, movimiento);
                    int eval = Minimax(nuevoTablero, profundidad - 1, false);
                    maxEval = Math.Max(maxEval, eval);
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var movimiento in ObtenerTodosMovimientos(tablero, 'O'))
                {
                    Tablero nuevoTablero = HacerMovimiento(tablero, movimiento);
                    int eval = Minimax(nuevoTablero, profundidad - 1, true);
                    minEval = Math.Min(minEval, eval);
                }
                return minEval;
            }
        }


        private int Heuristica(Tablero tablero, char jugador)
        {
            int puntaje = 0;

            for (int j = 0; j < 3; j++)
            {
                if (jugador == 'X' && tablero.Casillas[0, j] == 'X')
                {
                }
                if (jugador == 'O' && tablero.Casillas[2, j] == 'O')
                {
                }
            }

            int piezasJugador = 0;
            int piezasOponente = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tablero.Casillas[i, j] == 'X')
                    {
                        piezasJugador++;
                    }
                    if (tablero.Casillas[i, j] == 'O')
                    {
                        piezasOponente++;
                    }
                }
            }

            puntaje += piezasJugador - piezasOponente;

            if (ObtenerTodosMovimientos(tablero, jugador == 'X' ? 'O' : 'X').Count == 0)
            {
                return int.MaxValue;
            }

            return puntaje;
        }

        private bool JuegoTerminado(Tablero tablero, out string ganador)
        {
            ganador = null;
            for (int j = 0; j < 3; j++)
            {
                if (tablero.Casillas[0, j] == 'X')
                {
                    ganador = "X";
                    return true;
                }
                if (tablero.Casillas[2, j] == 'O')
                {
                    ganador = "O";
                    return true;
                }
            }

            if(VerificarMovimientos('O'))
            {
                ganador = "X";
                return true;
            }
            if (VerificarMovimientos('X'))
            {
                ganador = "O";
                return true;
            }
            return false;
        }



        private List<Movimiento> ObtenerTodosMovimientos(Tablero tablero, char jugador)
        {
            var movimientos = new List<Movimiento>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tablero.Casillas[i, j] == jugador)
                    {
                        if (jugador == 'X' && i > 0 && tablero.Casillas[i - 1, j] == ' ')
                        {
                            movimientos.Add(new Movimiento { DesdeFila = i, DesdeColumna = j, HastaFila = i - 1, HastaColumna = j });
                        }
                        if (jugador == 'O' && i < 2 && tablero.Casillas[i + 1, j] == ' ')
                        {
                            movimientos.Add(new Movimiento { DesdeFila = i, DesdeColumna = j, HastaFila = i + 1, HastaColumna = j });
                        }
                        if (jugador == 'X' && i > 0 && j > 0 && tablero.Casillas[i - 1, j - 1] == 'O')
                        {
                            movimientos.Add(new Movimiento { DesdeFila = i, DesdeColumna = j, HastaFila = i - 1, HastaColumna = j - 1 });
                        }
                        if (jugador == 'X' && i > 0 && j < 2 && tablero.Casillas[i - 1, j + 1] == 'O')
                        {
                            movimientos.Add(new Movimiento { DesdeFila = i, DesdeColumna = j, HastaFila = i - 1, HastaColumna = j + 1 });
                        }
                        if (jugador == 'O' && i < 2 && j > 0 && tablero.Casillas[i + 1, j - 1] == 'X')
                        {
                            movimientos.Add(new Movimiento { DesdeFila = i, DesdeColumna = j, HastaFila = i + 1, HastaColumna = j - 1 });
                        }
                        if (jugador == 'O' && i < 2 && j < 2 && tablero.Casillas[i + 1, j + 1] == 'X')
                        {
                            movimientos.Add(new Movimiento { DesdeFila = i, DesdeColumna = j, HastaFila = i + 1, HastaColumna = j + 1 });
                        }
                    }
                }
            }

            return movimientos;
        }

        private Tablero HacerMovimiento(Tablero tablero, Movimiento movimiento)
        {
            Tablero nuevoTablero = new Tablero();
            Array.Copy(tablero.Casillas, nuevoTablero.Casillas, tablero.Casillas.Length);
            nuevoTablero.Casillas[movimiento.HastaFila, movimiento.HastaColumna] = nuevoTablero.Casillas[movimiento.DesdeFila, movimiento.DesdeColumna];
            nuevoTablero.Casillas[movimiento.DesdeFila, movimiento.DesdeColumna] = ' ';
            return nuevoTablero;
        }
        private async void FinalMessage(string mensaje)
        {
            bool DoRebnoot = await Shell.Current.DisplayAlert("Fin del juego", mensaje, accept:"Reiniciar", cancel: "Cerrar");
            if (DoRebnoot)
            {
                Reiniciar();
            }
        }
        public void Reiniciar()
        {
            Table = new Tablero();
            primerClick = true;
            filaSeleccionada = -1;
            columnaSeleccionada = -1;
            Update();
        }

    }



}
