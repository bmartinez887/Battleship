using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int size = 10; // Tamano del tablero 10x10

        // Creo mi setup (tablero + lista de barcos) hard-coded
        PlayerSetup player = CreatePlayerSetup(size);

        // Selecciono aleatoriamente 1 de los 5 setups del oponente (tablero + barcos)
        int opponentBoardNumber;
        Setup opponent = GetRandomOpponentSetup(size, out opponentBoardNumber);

        // Tablero donde registro MIS disparos  (X=hit, O=miss)
        char[,] myShotsBoard = CreateEmptyBoard(size);

       
        char[,] opponentShotsBoard = CreateEmptyBoard(size);

        Console.WriteLine(" My board (hard-coded) ");
        PrintBoard(player.Board);

        Console.WriteLine($"\nOpponent board selected: Board #{opponentBoardNumber} ");
        Console.WriteLine("Game start ");

        
        int remainingOpponentCells = CountCells(opponent.Board, 'V');
        int remainingPlayerCells = CountCells(player.Board, 'V');

        
        Random rng = new Random();

        
        
        bool playerTurn = true;

        // El juego termina cuando uno de los dos llegue a 0 partes restantes
        while (remainingOpponentCells > 0 && remainingPlayerCells > 0)
        {
            if (playerTurn)
            {
                Console.WriteLine("\n--- YOUR TURN ---");

                // Muestro tablero de mis disparos para que no dispare repetido
                Console.WriteLine("My shots board (X=hit, O=miss):");
                PrintBoard(myShotsBoard);

                int r, c;

                // Pido coordenadas y valido que sean correctas y no repetidas
                while (true)
                {
                    Console.Write("Row (0-9): ");
                    if (!int.TryParse(Console.ReadLine(), out r))
                    {
                        Console.WriteLine("Invalid number.");
                        continue;
                    }

                    Console.Write("Col (0-9): ");
                    if (!int.TryParse(Console.ReadLine(), out c))
                    {
                        Console.WriteLine("Invalid number.");
                        continue;
                    }

                    if (r < 0 || r >= size || c < 0 || c >= size)
                    {
                        Console.WriteLine("Out of bounds.");
                        continue;
                    }

                    if (myShotsBoard[r, c] == 'X' || myShotsBoard[r, c] == 'O')
                    {
                        Console.WriteLine("Already shot there.");
                        continue;
                    }

                    break;
                }

                // Si habia barco del oponente en esa posicion, es HIT
                if (opponent.Board[r, c] == 'V')
                {
                    Console.WriteLine("HIT!");
                    myShotsBoard[r, c] = 'X';     // Marco el acierto en mi tablero de disparos
                    opponent.Board[r, c] = 'X';   // Marco el golpe en el tablero oculto del oponente
                    remainingOpponentCells--;     // Disminuyo las partes restantes del oponente

                    // Reviso si este golpe hundio un barco completo del oponente
                    foreach (Ship ship in opponent.Ships)
                    {
                        if (!ship.SunkAnnounced && ship.IsSunk(opponent.Board))
                        {
                            ship.SunkAnnounced = true; // Evita anunciarlo dos veces
                            Console.WriteLine("You sunk a ship!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("MISS!");
                    myShotsBoard[r, c] = 'O'; // Marco fallo
                }

                // Si ya gane, corto antes de que dispare el oponente
                if (remainingOpponentCells == 0)
                    break;

                // Cambio turno al oponente
                playerTurn = false;
            }
            else
            {
                Console.WriteLine("\n OPPONENT TURN ");

               
                
                int r, c;
                while (true)
                {
                    r = rng.Next(size);
                    c = rng.Next(size);

                   
                    if (opponentShotsBoard[r, c] == 'X' || opponentShotsBoard[r, c] == 'O')
                        continue;

                    break;
                }

                Console.WriteLine($"Opponent shoots: ({r}, {c})");

             
                if (player.Board[r, c] == 'V')
                {
                    Console.WriteLine("Opponent HIT!");
                    opponentShotsBoard[r, c] = 'X'; 
                    player.Board[r, c] = 'X';        
                    remainingPlayerCells--;          

                    
                    foreach (Ship ship in player.Ships)
                    {
                        if (!ship.SunkAnnounced && ship.IsSunk(player.Board))
                        {
                            ship.SunkAnnounced = true;
                            Console.WriteLine("Opponent sunk one of your ships!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Opponent MISS!");
                    opponentShotsBoard[r, c] = 'O'; 
                }

               
                Console.WriteLine("\nMy board after opponent shot:");
                PrintBoard(player.Board);

                
                playerTurn = true;
            }
        }

      
        if (remainingOpponentCells == 0)
        {
            Console.WriteLine("\nYOU WIN! All opponent ships sunk.");
        }
        else
        {
            Console.WriteLine("\nOPPONENT WINS! All your ships were sunk.");
        }
    }

    

    // Representa una celda (fila, columna)
    struct Cell
    {
        public int R;
        public int C;

        public Cell(int r, int c)
        {
            R = r;
            C = c;
        }
    }

    // Un barco es una lista de celdas; esta hundido cuando TODAS esas celdas son 'X'
    class Ship
    {
        public Cell[] Cells;        // Coordenadas del barco
        public bool SunkAnnounced;  // Para no imprimir "sunk" dos veces

        public Ship(Cell[] cells)
        {
            Cells = cells;
            SunkAnnounced = false;
        }

        public bool IsSunk(char[,] board)
        {
            foreach (var cell in Cells)
            {
                if (board[cell.R, cell.C] != 'X')
                    return false;
            }
            return true;
        }
    }

    // Setup generico (tablero + barcos)
    class Setup
    {
        public char[,] Board;
        public Ship[] Ships;

        public Setup(char[,] board, Ship[] ships)
        {
            Board = board;
            Ships = ships;
        }
    }

    // Para el jugador, uso el mismo concepto (solo nombre diferente por claridad)
    class PlayerSetup : Setup
    {
        public PlayerSetup(char[,] board, Ship[] ships) : base(board, ships) { }
    }

    

    static PlayerSetup CreatePlayerSetup(int size)
    {
        char[,] b = CreateEmptyBoard(size);
        var ships = new List<Ship>();

        // 2 barcos de 2
        ships.Add(AddHardcodedShip(b, 0, 0, 'R', 2));
        ships.Add(AddHardcodedShip(b, 2, 7, 'D', 2));

        // 2 barcos de 3
        ships.Add(AddHardcodedShip(b, 5, 1, 'R', 3));
        ships.Add(AddHardcodedShip(b, 7, 6, 'D', 3));

        // 1 barco de 4
        ships.Add(AddHardcodedShip(b, 9, 1, 'R', 4));

        return new PlayerSetup(b, ships.ToArray());
    }

 

    static Setup GetRandomOpponentSetup(int size, out int boardNumber)
    {
        Setup[] setups = CreateOpponentSetups(size);

        Random rng = new Random();
        int pick = rng.Next(setups.Length);

        boardNumber = pick + 1; // Para mostrar 1-5
        return setups[pick];
    }

    static Setup[] CreateOpponentSetups(int size)
    {
        Setup[] setups = new Setup[5];

        // Setup 1
        {
            char[,] b = CreateEmptyBoard(size);
            var ships = new List<Ship>();

            ships.Add(AddHardcodedShip(b, 0, 8, 'D', 2));
            ships.Add(AddHardcodedShip(b, 3, 0, 'R', 2));
            ships.Add(AddHardcodedShip(b, 1, 2, 'D', 3));
            ships.Add(AddHardcodedShip(b, 6, 5, 'R', 3));
            ships.Add(AddHardcodedShip(b, 8, 1, 'R', 4));

            setups[0] = new Setup(b, ships.ToArray());
        }

        // Setup 2
        {
            char[,] b = CreateEmptyBoard(size);
            var ships = new List<Ship>();

            ships.Add(AddHardcodedShip(b, 0, 0, 'R', 2));
            ships.Add(AddHardcodedShip(b, 4, 9, 'D', 2));
            ships.Add(AddHardcodedShip(b, 2, 4, 'R', 3));
            ships.Add(AddHardcodedShip(b, 7, 2, 'D', 3));
            ships.Add(AddHardcodedShip(b, 9, 5, 'R', 4));

            setups[1] = new Setup(b, ships.ToArray());
        }

        // Setup 3
        {
            char[,] b = CreateEmptyBoard(size);
            var ships = new List<Ship>();

            ships.Add(AddHardcodedShip(b, 1, 6, 'D', 2));
            ships.Add(AddHardcodedShip(b, 9, 0, 'R', 2));
            ships.Add(AddHardcodedShip(b, 0, 2, 'R', 3));
            ships.Add(AddHardcodedShip(b, 4, 4, 'D', 3));
            ships.Add(AddHardcodedShip(b, 6, 8, 'D', 4));

            setups[2] = new Setup(b, ships.ToArray());
        }

        // Setup 4
        {
            char[,] b = CreateEmptyBoard(size);
            var ships = new List<Ship>();

            ships.Add(AddHardcodedShip(b, 2, 2, 'R', 2));
            ships.Add(AddHardcodedShip(b, 5, 9, 'D', 2));
            ships.Add(AddHardcodedShip(b, 0, 7, 'D', 3));
            ships.Add(AddHardcodedShip(b, 6, 1, 'R', 3));
            ships.Add(AddHardcodedShip(b, 8, 4, 'R', 4));

            setups[3] = new Setup(b, ships.ToArray());
        }

        // Setup 5
        {
            char[,] b = CreateEmptyBoard(size);
            var ships = new List<Ship>();

            ships.Add(AddHardcodedShip(b, 3, 3, 'D', 2));
            ships.Add(AddHardcodedShip(b, 0, 9, 'D', 2));
            ships.Add(AddHardcodedShip(b, 1, 0, 'R', 3));
            ships.Add(AddHardcodedShip(b, 7, 6, 'R', 3));
            ships.Add(AddHardcodedShip(b, 4, 5, 'D', 4));

            setups[4] = new Setup(b, ships.ToArray());
        }

        return setups;
    }


    static Ship AddHardcodedShip(char[,] board, int row, int col, char dir, int length)
    {
        dir = char.ToUpper(dir);

        // dr/dc define si crece hacia abajo o a la derecha
        int dr = (dir == 'D') ? 1 : 0;
        int dc = (dir == 'R') ? 1 : 0;

        Cell[] cells = new Cell[length];

        // Coloco el barco con 'V' y guardo cada coordenada en el arreglo cells
        for (int i = 0; i < length; i++)
        {
            int r = row + dr * i;
            int c = col + dc * i;

            board[r, c] = 'V';
            cells[i] = new Cell(r, c);
        }

        return new Ship(cells);
    }

    

    static char[,] CreateEmptyBoard(int size)
    {
        char[,] board = new char[size, size];

        // Lleno el tablero con agua '~'
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                board[r, c] = '~';

        return board;
    }

    static int CountCells(char[,] board, char target)
    {
        int count = 0;

        // Cuento cuantas celdas tienen el caracter target
        for (int r = 0; r < board.GetLength(0); r++)
            for (int c = 0; c < board.GetLength(1); c++)
                if (board[r, c] == target)
                    count++;

        return count;
    }

    static void PrintBoard(char[,] board)
    {
        // Imprime el tablero con formato [ ]
        for (int r = 0; r < board.GetLength(0); r++)
        {
            for (int c = 0; c < board.GetLength(1); c++)
                Console.Write($"[{board[r, c]}]");
            Console.WriteLine();
        }
    }
}