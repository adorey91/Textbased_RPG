using System;
using System.IO;
using System.Diagnostics;

namespace Textbased_RPG_AdrianDorey
{
    internal class Program
    {
        static char[,] mapContent;

        static char player = 'P';
        static int playerPositionX;
        static int playerPositionY;
        
        static char enemy = 'E';
        static int enemyPositionX;
        static int enemyPositionY;

        static Random randomMovement = new Random();
        static string[] movements = { "XForward", "XBackwards", "None","YForward","YBackward"};
        
        static bool gameOver = false;

        static void Main(string[] args)
        {
            mapInit("map.txt");

            playerPositionX = 2;
            playerPositionY = 2;
            enemyPositionX = 15;
            enemyPositionY = 10;

            Console.CursorVisible = false;

            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("Textbased RPG - Adrian Dorey");
                Console.WriteLine();

                drawMap();
                enemyPosition();
                

                playerPosition();
            }
        }

        static void mapInit(string filePath)   // initializes map
        {
            string[] lines = File.ReadAllLines(filePath);

            mapContent = new char[lines.Length, lines[0].Length];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    mapContent[i, j] = lines[i][j];
                }
            }
        }

        static void drawMap()
        {
            for (int i = 0; i < mapContent.GetLength(0); i++)
            {
                for (int j = 0; j < mapContent.GetLength(1); j++)
                {
                    if (i == playerPositionY && j == playerPositionX)
                        Console.Write(player); // Draw the character
                    else if(i == enemyPositionY && j == enemyPositionX)
                        Console.Write(enemy);
                    else
                        Console.Write(mapContent[i, j]);
                }
                Console.WriteLine();
            }
        }

        static void playerPosition()
        {
            ConsoleKeyInfo input;
            input = Console.ReadKey();

            if (input.Key == ConsoleKey.W || input.Key == ConsoleKey.UpArrow)
            {
                if (availableMove(playerPositionX, playerPositionY - 1))
                    playerPositionY--;
            }
            else if (input.Key == ConsoleKey.S || input.Key == ConsoleKey.DownArrow)
            {
                if (availableMove(playerPositionX, playerPositionY + 1))
                    playerPositionY++;
            }
            else if (input.Key == ConsoleKey.A || input.Key == ConsoleKey.LeftArrow)
            {
                if (availableMove(playerPositionX - 1, playerPositionY))
                    playerPositionX--;
            }
            else if (input.Key == ConsoleKey.D || input.Key == ConsoleKey.RightArrow)
            {
                if (availableMove(playerPositionX + 1, playerPositionY))
                    playerPositionX++;
            }
            else if (input.Key == ConsoleKey.Escape)
            {
                gameOver = true;
            }
        }

        static void enemyPosition()
        {
            int Movement = randomMovement.Next(0, movements.Length);

            if (movements[Movement] == "XForward")
            {
                if (availableMove(enemyPositionX + 1, enemyPositionY))
                {
                    enemyPositionX++;
                }
            }
            else if (movements[Movement] == "XBackwards")
            {
                if (availableMove(enemyPositionX - 1, enemyPositionY))
                {
                    enemyPositionX--;
                }
            }
            else if (movements[Movement] == "YForward")
            {
                if (availableMove(enemyPositionX, enemyPositionY + 1))
                {
                    enemyPositionY++;
                }
            }
            else if (movements[Movement] == "YBackwards")
            {
                if (availableMove(enemyPositionX, enemyPositionY - 1))
                {
                    enemyPositionY--;
                }
            }
            else
                return;

            Console.WriteLine("Enemy Movement " + movements[Movement]);
        }

        static bool availableMove(int x, int y)
        {
            return x >= 0 && x < mapContent.GetLength(1) && y >= 0 && y < mapContent.GetLength(0) && mapContent[y, x] != '#';
        }
    }
}
