using System;
using System.IO;
using System.Runtime.InteropServices;

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

        static char item2 = '$';
        static char item2HUD;
        static int itemPosition2X;
        static int itemPosition2Y;

        static Random randomMovement = new Random();    // this is a RNG for the enemy movement
        static string[] movements = {"XForward", "XBackwards", "YForward", "YBackward"}; // an array to help define enemy movement

        static bool gameOver = false;   // sets gameplay or quits game depending on logic


        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            mapInit("map.txt");

            //sets positions & health of player, enemy & item 
            playerPositionX = 2;
            playerPositionY = 2;
            playerHealth = 100;
            enemyPositionX = 32;
            enemyPositionY = 5;
            enemyHealth = 100;
            itemPositionX = 30;
            itemPositionY = 4;
            itemPosition2X = 4;
            itemPosition2Y = 13;

            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("Textbased RPG - Adrian Dorey");
                Console.WriteLine();

                playerUpdate(); 
                enemyUpdate();
                ShowHUD();

                drawMap();


             
                Console.WriteLine();
                DisplayLegend();

                if (enemyHealth == 0 && itemHUD == '$' && item2HUD == '$')
                    gameOver = true;
                else if (playerHealth == 0)
                    gameOver = true;
                playerPosition(); 
                itemPickUp();   

                enemyPosition();



            }
            Console.WriteLine();
            Console.WriteLine("Game Over, press any key to continue");
            Console.ReadKey();
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

        static void drawMap() // uses the map content to draw the map includes player, enemy & items
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
                            Console.Write(' ');
                        else
                            Console.Write(enemy);
                    }
                    else if (i == itemPositionY && j == itemPositionX)
                    {
                        writeItem("item");
                    }
                    else if (i == itemPosition2Y && j == itemPosition2X)
                    {
                        writeItem("item2");
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

        static void writeItem(string thisItem) // handles writing item in map
        {
            if (thisItem == "item")
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
            else if (thisItem == "item2")
            {
                if (item2 != ' ')
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(item2);
                    Console.ResetColor();
                }
                else
                    Console.Write(item2);
            }
        }

        static void MapColor(char c)    // handles map color
        {
            switch (c)
            {
                case '#':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                case 'V':
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case '~':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case '$':
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
            }
        }

        static void DisplayLegend() // displays legend on the bottom of the map.
        {
            Console.WriteLine("Map Legend:");

            MapColor('#');
            Console.Write("#");
            Console.ResetColor();
            Console.WriteLine(" = Walls");

            MapColor('V');
            Console.Write("V");
            Console.ResetColor();
            Console.WriteLine(" = Lava");

            MapColor('~');
            Console.Write("~");
            Console.ResetColor();
            Console.WriteLine(" = Deep Water");

            MapColor('$');
            Console.Write("$");
            Console.ResetColor();
            Console.WriteLine(" = Money");
            Console.ResetColor();
        }

        static void playerPosition()    // handles player movement depending on keyboard input
        {
            ConsoleKeyInfo input = Console.ReadKey();

            int dirx = 0, diry = 0;

            if (input.Key == ConsoleKey.W || input.Key == ConsoleKey.UpArrow) diry = -1;
            else if (input.Key == ConsoleKey.S || input.Key == ConsoleKey.DownArrow) diry = 1;
            else if (input.Key == ConsoleKey.A || input.Key == ConsoleKey.LeftArrow) dirx = -1;
            else if (input.Key == ConsoleKey.D || input.Key == ConsoleKey.RightArrow) dirx = 1;
            else if (input.Key == ConsoleKey.Spacebar) return;
            else if (input.Key == ConsoleKey.Escape) gameOver = true;

            if (dirx != 0 || diry != 0)
            {
                int newX = playerPositionX + dirx;
                int newY = playerPositionY + diry;

                if (checkBoundaries(newX, newY))
                {
                    if (newX == enemyPositionX && newY == enemyPositionY)
                    {
                        attackEnemy();
                    }
                    else
                    {
                        playerPositionX = newX;
                        playerPositionY = newY;

                        char landedChar = mapContent[playerPositionY, playerPositionX];
                        if (landedChar == 'V')
                        {
                            playerHealth -= 5;
                        }
                    }
                }
            }
        }

        static void itemPickUp() // handles item pickUp
        {
            if (itemPositionX == playerPositionX && itemPositionY == playerPositionY)
            {
                if (item == ' ')
                    Console.Write(" ");
                else
                {
                    itemHUD = '$';
                    item = ' ';
                }
            }
            else if (itemPosition2X == playerPositionX && itemPosition2Y == playerPositionY)
            {
                if (item2 == ' ')
                    Console.Write(" ");
                else
                {
                    item2HUD = '$';
                    item2 = ' ';
                }
            }
        }

        static void enemyPosition() //handles enemy position based on random number of an array
        {
            if (enemyHealth > 0)
            {
                int Direction = randomMovement.Next(0, movements.Length);

                int dx = 0, dy = 0;

                if (movements[Direction] == "YBackwards") dy = -1;
                else if (movements[Direction] == "YForward") dy = 1;
                else if (movements[Direction] == "XBackwards") dx = -1;
                else if (movements[Direction] == "XForward") dx = 1;

                if (dx != 0 || dy != 0)
                {
                    int newEnemyX = enemyPositionX + dx;
                    int newEnemyY = enemyPositionY + dy;

                    if (checkBoundaries(newEnemyX, newEnemyY))
                    {
                        if (newEnemyX == playerPositionX && newEnemyY == playerPositionY)
                        {
                            attackPlayer();
                        }
                        else
                        {
                            enemyPositionX = newEnemyX;
                            enemyPositionY = newEnemyY;

                            char landedChar = mapContent[enemyPositionY, enemyPositionX];
                            if (landedChar == 'V')
                            {
                                enemyHealth -= 5;
                            }
                        }
                    }
                }
            }
        }   

        static void attackEnemy()   // handles attacking enemy
        {
            if (enemyHealth == 0)
                enemyHealth = 0;
            else
                enemyHealth = enemyHealth - 10;
        }

        static void attackPlayer()  // handles attacking player
        {
            playerHealth = playerHealth - 10;
        }

        static void ShowHUD()   // handles hud output
        {
            Console.WriteLine("Player Health: " + playerHealth + " || Player Status: " + playerStatus);
            Console.WriteLine("Enemy Health: " + enemyHealth + " || Enemy Status: " + enemyStatus);
            Console.Write("Item Picked Up: ");

            if(itemHUD == '$')
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write(itemHUD);
            Console.ResetColor();
            Console.Write(' ');
            if(item2HUD == '$')
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write(item2HUD);
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine();
        }

        static void enemyUpdate()   // handles enemy health update for HUD
        {
            if (enemyHealth <= 0)
            {
                enemyStatus = "Enemy Died";
                enemyHealth = 0;
            }
            else if ((enemyHealth > 0) && (enemyHealth <= 15))
                enemyStatus = "Imminent Danger";
            else if ((enemyHealth > 15) && (enemyHealth <= 50))
                enemyStatus = "Badly Hurt";
            else if ((enemyHealth > 50 && enemyHealth <= 75))
                enemyStatus = "Hurt";
            else if ((enemyHealth > 75 && enemyHealth < 100))
                enemyStatus = "Healthy";
            else if (enemyHealth == 100)
                enemyStatus = "Perfectly Healthy";
        }

        static void playerUpdate()  // handles player Health update for HUD
        {
            if (playerHealth <= 0)
            { 
                playerStatus = "Player Died";
                playerHealth = 0;
            }
            else if ((playerHealth > 0) && (playerHealth <= 15))
                playerStatus = "Imminent Danger";
            else if ((playerHealth > 15) && (playerHealth <= 50))
                playerStatus = "Badly Hurt";
            else if ((playerHealth > 50 && playerHealth <= 75))
                playerStatus = "Hurt";
            else if ((playerHealth > 75 && playerHealth < 100))
                playerStatus = "Healthy";
            else if (playerHealth == 100)
                playerStatus = "Perfectly Healthy";
        }

        static bool checkBoundaries(int x, int y) //handles player avoiding boundaries & water
        {
            return x >= 0 && x < mapContent.GetLength(1) && y >= 0 && y < mapContent.GetLength(0) && mapContent[y, x] != '#' && mapContent[y, x] != '~';
        }
    }
}
