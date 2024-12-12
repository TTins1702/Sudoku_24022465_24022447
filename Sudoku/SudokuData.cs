using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuData : MonoBehaviour
{
    public static SudokuData Instance;

    public struct SudokuBoardData
    {
        public int[] unsolved_data;
        public int[] solved_data;

        public SudokuBoardData(int[] unsolved, int[] solved) : this()
        {
            this.unsolved_data = unsolved;
            this.solved_data = solved;
        }
    }

    public Dictionary<string, List<SudokuBoardData>> sudoku_game;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sudoku_game = new Dictionary<string, List<SudokuBoardData>>();
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        sudoku_game.Add("Easy", GenerateSudokuData(20));
        sudoku_game.Add("Medium", GenerateSudokuData(40));
        sudoku_game.Add("Hard", GenerateSudokuData(50));
        sudoku_game.Add("VeryHard", GenerateSudokuData(60));
    }

    private List<SudokuBoardData> GenerateSudokuData(int emptyCells)
    {
        List<SudokuBoardData> data = new List<SudokuBoardData>();
        for (int i = 0; i < 10; i++)
        {
            int[,] solvedBoard = SudokuGenerator.GenerateSolvedBoard();
            int[,] puzzle = SudokuGenerator.GeneratePuzzle(solvedBoard, emptyCells);

            data.Add(new SudokuBoardData(
                puzzle.Cast<int>().ToArray(),
                solvedBoard.Cast<int>().ToArray()
            ));
        }
        return data;
    }
}

public class SudokuGenerator
{
    public static int[,] GenerateSolvedBoard()
    {
        int[,] board = new int[9, 9];
        List<int> randomNumbers = Enumerable.Range(1, 9).OrderBy(x => Guid.NewGuid()).ToList();

        FillDiagonalBlocks(board, randomNumbers);
        SudokuSolver.Solve(board);
        return board;
    }

    private static void FillDiagonalBlocks(int[,] board, List<int> randomNumbers)
    {
        for (int block = 0; block < 9; block += 3)
        {
            FillBlock(board, block, block, randomNumbers);
        }
    }

    private static void FillBlock(int[,] board, int startRow, int startCol, List<int> randomNumbers)
    {
        System.Random random = new System.Random();
        List<int> shuffledNumbers = randomNumbers.OrderBy(x => random.Next()).ToList();
        int index = 0;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[startRow + i, startCol + j] = shuffledNumbers[index++];
            }
        }
    }

    public static int[,] GeneratePuzzle(int[,] solvedBoard, int emptyCells)
    {
        int[,] puzzle = (int[,])solvedBoard.Clone();
        System.Random random = new System.Random();

        while (emptyCells > 0)
        {
            int row = random.Next(0, 9);
            int col = random.Next(0, 9);

            if (puzzle[row, col] != 0)
            {
                puzzle[row, col] = 0;
                emptyCells--;
            }
        }

        return puzzle;
    }
}

public class SudokuSolver
{
    public static bool Solve(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsValid(board, row, col, num))
                        {
                            board[row, col] = num;

                            if (Solve(board))
                                return true;

                            board[row, col] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    private static bool IsValid(int[,] board, int row, int col, int num)
    {
        for (int x = 0; x < 9; x++)
        {
            if (board[row, x] == num)
                return false;
        }

        for (int x = 0; x < 9; x++)
        {
            if (board[x, col] == num)
                return false;
        }

        int startRow = row - row % 3, startCol = col - col % 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i + startRow, j + startCol] == num)
                    return false;
            }
        }

        return true;
    }
}
