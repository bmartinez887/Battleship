using System;

class Program
{
    static void Main()
    {
        int size = 10;

        char[,] playerBoard = CreateEmptyBoard(size);
        char[,] opponentBoard = GetRandomOpponentBoard(size);
        char[,] shotsBoard = CreateEmptyBoard(size);

        Console.WriteLine("=== Place your ships ===");
        PlacePlayerShips(playerBoard);

        Console.WriteLine("\nYour board:");
        PrintBoard(playerBoard);

        Console.WriteLine("\n=== Game start! Shoot the opponent ===");

        int remainingOpponentCells = CountCells(opponentBoard, 'V');

        while (remainingOpponentCells > 0)
        {
            Console.WriteLine("\nYour shots board (X=hit, O=miss):");
            PrintBoard(shotsBoard);

            int r, c;
            while (true)
            {
                Console.Write("Shoot Row (0-9): ");
                if (!int.TryParse(Console.ReadLine(), out r)) { Console.WriteLine("Invalid number."); continue; }

                Console.Write("Shoot Col (0-9): ");
                if (!int.TryParse(Console.ReadLine(), out c)) { Console.WriteLine("Invalid number."); continue; }

                if (r < 0 || r >= size || c < 0 || c >= size)
                {
                    Console.WriteLine("Out of bounds. Try again.");
                    continue;
                }

                if (shotsBoard[r, c] == 'X' || shotsBoard[r, c] == 'O')
                {
                    Console.WriteLine("You already shot there. Try again.");
                    continue;
                }

                break;
            }

            if (opponentBoard[r, c] == 'V')
            {
                Console.WriteLine("HIT!");
                shotsBoard[r, c] = 'X';
                opponentBoard[r, c] = 'X'; 
                remainingOpponentCells--;
            }
            else
            {
                Console.WriteLine("MISS!");
                shotsBoard[r, c] = 'O';
            }
        }

        Console.WriteLine("\nYOU WIN! You sank all opponent ships!");
        Console.WriteLine("\nFinal shots board:");
        PrintBoard(shotsBoard);
    }

    

    static void PlacePlayerShips(char[,] playerBoard)
    {
        int[] shipSizes = { 2, 3, 4 };

        for (int i = 0; i < shipSizes.Length; i++)
        {
            int shipSize = shipSizes[i];
            Console.WriteLine($"\nPlace ship of size {shipSize}");

            int row, col;
            char direction;

            while (true)
            {
                Console.Write("Row (0-9): ");
                if (!int.TryParse(Console.ReadLine(), out row)) { Console.WriteLine("Invalid number."); continue; }

                Console.Write("Column (0-9): ");
                if (!int.TryParse(Console.ReadLine(), out col)) { Console.WriteLine("Invalid number."); continue; }

                Console.Write("Direction (R=right, D=down): ");
                string dirInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(dirInput)) { Console.WriteLine("Invalid direction."); continue; }
                direction = char.ToUpper(dirInput[0]);

                if (direction != 'R' && direction != 'D')
                {
                    Console.WriteLine("Direction must be R or D.");
                    continue;
                }

                if (CanPlaceShip(playerBoard, row, col, direction, shipSize))
                {
                    PlaceShip(playerBoard, row, col, direction, shipSize, 'V');
                    break;
                }

                Console.WriteLine("Invalid position (out of bounds or overlaps). Try again.");
            }

            Console.WriteLine("\nYour board so far:");
            PrintBoard(playerBoard);
        }
    }

    static bool CanPlaceShip(char[,] board, int row, int col, char dir, int size)
    {
        int dr = (dir == 'D') ? 1 : 0;
        int dc = (dir == 'R') ? 1 : 0;

        for (int i = 0; i < size; i++)
        {
            int r = row + dr * i;
            int c = col + dc * i;

            if (r < 0 || r >= board.GetLength(0) || c < 0 || c >= board.GetLength(1))
                return false;

            if (board[r, c] != '~')
                return false;
        }

        return true;
    }

    static void PlaceShip(char[,] board, int row, int col, char dir, int size, char symbol)
    {
        int dr = (dir == 'D') ? 1 : 0;
        int dc = (dir == 'R') ? 1 : 0;

        for (int i = 0; i < size; i++)
        {
            board[row + dr * i, col + dc * i] = symbol;
        }
    }

    

    static char[,] GetRandomOpponentBoard(int size)
    {
        char[][,] boards = CreateHardcodedOpponentBoards(size);
        Random rng = new Random();
        int pick = rng.Next(boards.Length);
        return boards[pick];
    }

    static char[][,] CreateHardcodedOpponentBoards(int size)
    {
        char[][,] boards = new char[5][,];

        boards[0] = CreateEmptyBoard(size);
        PlaceShip(boards[0], 0, 0, 'R', 2, 'V');
        PlaceShip(boards[0], 2, 2, 'D', 3, 'V');
        PlaceShip(boards[0], 6, 4, 'R', 4, 'V');

        boards[1] = CreateEmptyBoard(size);
        PlaceShip(boards[1], 1, 7, 'D', 2, 'V');
        PlaceShip(boards[1], 4, 1, 'R', 3, 'V');
        PlaceShip(boards[1], 8, 2, 'R', 4, 'V');

        boards[2] = CreateEmptyBoard(size);
        PlaceShip(boards[2], 3, 3, 'R', 2, 'V');
        PlaceShip(boards[2], 0, 9, 'D', 3, 'V');
        PlaceShip(boards[2], 5, 0, 'D', 4, 'V');

        boards[3] = CreateEmptyBoard(size);
        PlaceShip(boards[3], 9, 0, 'R', 2, 'V');
        PlaceShip(boards[3], 1, 4, 'D', 3, 'V');
        PlaceShip(boards[3], 4, 6, 'D', 4, 'V');

        boards[4] = CreateEmptyBoard(size);
        PlaceShip(boards[4], 7, 7, 'R', 2, 'V');
        PlaceShip(boards[4], 2, 0, 'R', 3, 'V');
        PlaceShip(boards[4], 0, 5, 'D', 4, 'V');

         

        return boards;
    }

    

    static char[,] CreateEmptyBoard(int size)
    {
        char[,] board = new char[size, size];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                board[r, c] = '~';
        return board;
    }

    static int CountCells(char[,] board, char target)
    {
        int count = 0;
        for (int r = 0; r < board.GetLength(0); r++)
            for (int c = 0; c < board.GetLength(1); c++)
                if (board[r, c] == target) count++;
        return count;
    }

    static void PrintBoard(char[,] board)
    {
        for (int r = 0; r < board.GetLength(0); r++)
        {
            for (int c = 0; c < board.GetLength(1); c++)
            {
                Console.Write($"[{board[r, c]}]");
            }
            Console.WriteLine();
        }
    }
}
