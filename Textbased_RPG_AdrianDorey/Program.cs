using System;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Textbased_RPG_AdrianDorey
{
    internal class Program
    {
        static char[,] mapContent;  // holds the map file

        static char player = 'P';   // represents player
        static string playerStatus; // players health status
        static int playerHealth;    // players health in form of an int
        static int playerPositionX;
        static int playerPositionY;
        
        static char enemy = 'E';    // represents enemy
        static string enemyStatus;  // enemy health status
        static int enemyHealth;     // enemy health in the form of an int
        static int enemyPositionX;
        static int enemyPositionY;

        static char item = '$'; // represents pick up
        static char itemHUD;    // represents pick up in HUD
        static int itemPositionX;
        static int itemPositionY;

        static Random randomMovement = new Random();    // this is a RNG for the enemy movement
        static string[] movements = { "XForward", "XBackwards", "None","YForward","YBackward"}; // an array to help define enemy movement
        
        static bool gameOver = false;   // sets gameplay or quits game depending on logic

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            mapInit("map.txt");

            //sets positions & health of player, enemy & item 
            playerPositionX = 2;
            playerPositionY = 2;
            playerHealth = 100;
            enemyPositionX = 15;
            enemyPositionY = 10;
            enemyHealth = 100;
            itemPositionX = 4;
            itemPositionY = 4;

            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("Textbased RPG - Adrian Dorey");
                Console.WriteLine();
                
                playerUpdate(); // updates HUD health status of player
                enemyUpdate();  // updates HUD health status of enemy
                ShowHUD();
                
                drawMap();
                
                playerPosition();   // sets player position 
                itemPickUp();
                enemyPosition();
            }
        }

        static void mapInit(string filePath)   // initializes map from file to mapContent
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

        static void drawMap() // uses the map content to draw the map includes player, enemy & item
        {
            for (int i = 0; i < mapContent.GetLength(0); i++)
            {
                for (int j = 0; j < mapContent.GetLength(1); j++)
                {
                    if (i == playerPositionY && j == playerPositionX)
                    {
                        Console.Write(player);
                    }
                    else if (i == enemyPositionY && j == enemyPositionX)
                    {
                        if (enemyHealth == 0)
                        {
                            Console.Write(' ');
                        }
                        else
                        {
                            Console.Write(enemy);
                        }
                    }
                    else if (i == itemPositionY && j == itemPositionX)
                    {
                        writeItem();
                    }
                    else
                    {
                        MapColor(mapContent[i, j]);
                        Console.Write(mapContent[i, j]);
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }
        
        static void writeItem() // handles item in map
        {
            if (item != ' ')
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(item);
                Console.ResetColor();
            }
            else
                Console.Write(item);
        }

        static void MapColor(char c)
        {
            if (c == '#')
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.BackgroundColor = ConsoleColor.DarkGray;
            }
            else if (c == 'V')
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Red;
            }
            else if  (c == 'W')
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
            }
        }   // handles map color

        static void playerPosition()
        {
            ConsoleKeyInfo input;
            input = Console.ReadKey();

            if (input.Key == ConsoleKey.W || input.Key == ConsoleKey.UpArrow)
            {
                if (availableMove(playerPositionX, playerPositionY - 1))
                    playerPositionY--;
                attackEnemy();
            }
            else if (input.Key == ConsoleKey.S || input.Key == ConsoleKey.DownArrow)
            {
                if (availableMove(playerPositionX, playerPositionY + 1))
                    playerPositionY++;
                attackEnemy();
            }
            else if (input.Key == ConsoleKey.A || input.Key == ConsoleKey.LeftArrow)
            {
                if (availableMove(playerPositionX - 1, playerPositionY))
                    playerPositionX--;
                attackEnemy();
            }
            else if (input.Key == ConsoleKey.D || input.Key == ConsoleKey.RightArrow)
            {
                if (availableMove(playerPositionX + 1, playerPositionY))
                    playerPositionX++;
                attackEnemy();
            }
            else if (input.Key == ConsoleKey.Spacebar)
                return;
            else if (input.Key == ConsoleKey.Escape)
                gameOver = true;
        }   // handles player movement
        
        static void itemPickUp() // handles item pickUp
        {
            if (itemPositionX == playerPositionX && itemPositionY == playerPositionY)
            {
                if (item == ' ')
                {
                    Console.Write(" ");
                }
                else
                {
                    itemHUD = '$';
                    item = ' ';
                }

            }
        }
        
        static void enemyPosition()
        {
            if (enemyHealth > 0)
            {
                int Movement = randomMovement.Next(0, movements.Length);

                if (movements[Movement] == "None")
                    return;
                else
                {
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
                    attackPlayer();
                }
            }
        }   // handles enemy movement

        static void attackEnemy()   // handles attacking enemy
        {
            if (playerPositionY == enemyPositionY && playerPositionX == enemyPositionX)
            {
                if (enemyHealth == 0)
                    enemyHealth = 0;
                else
                    enemyHealth = enemyHealth - 10;
            }
        }

        static void attackPlayer()  // handles attacking player
        {
            if (enemyPositionY == playerPositionY && enemyPositionX == playerPositionX)
            {
                if (playerHealth == 0)
                {
                    gameOver = true;
                }
                else
                    playerHealth = playerHealth - 10;
            }
        }

        static void ShowHUD()   // handles hud
        {
            Console.WriteLine("Player Health: " + playerHealth + " || Player Status: " + playerStatus);
            Console.WriteLine("Enemy Health: " + enemyHealth + " || Enemy Status: " + enemyStatus);
            Console.Write("Item Picked Up: ");
            if (item == ' ')
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(itemHUD);
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        
        static void enemyUpdate()
        {
            if (enemyHealth <= 0)
            {
                enemyStatus = "Enemy Died";
            }
            else if ((enemyHealth > 0) && (enemyHealth <= 15))
            {
                enemyStatus = "Imminent Danger";
            }
            else if ((enemyHealth > 15) && (enemyHealth <= 50))
            {
                enemyStatus = "Badly Hurt";
            }
            else if ((enemyHealth > 50 && enemyHealth <= 75))
            {
                enemyStatus = "Hurt";
            }
            else if ((enemyHealth > 75 && enemyHealth < 100))
            {
                enemyStatus = "Healthy";
            }
            else if (enemyHealth == 100)
            {
                enemyStatus = "Perfectly Healthy";
            }
        }   // enemy health HUD update

        static void playerUpdate()
        {
            if (playerHealth <= 0)
            {
                playerStatus = "Player Died";
            }
            else if ((playerHealth > 0) && (playerHealth <= 15))
            {
                playerStatus = "Imminent Danger";
            }
            else if ((playerHealth > 15) && (playerHealth <= 50))
            {
                playerStatus = "Badly Hurt";
            }
            else if ((playerHealth > 50 && playerHealth <= 75))
            {
                playerStatus = "Hurt";
            }
            else if ((playerHealth > 75 && playerHealth < 100))
            {
                playerStatus = "Healthy";
            }
            else if (playerHealth == 100)
            {
                playerStatus = "Perfectly Healthy";
            }
        }   // player health HUD update

        static bool availableMove(int x, int y) //handles player avoiding boundaries
        {
            return x >= 0 && x < mapContent.GetLength(1) && y >= 0 && y < mapContent.GetLength(0) && mapContent[y, x] != '#';
        }
    }
}
