using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{
    //Test

    /// <summary>
    /// Represents any entity that exists in game
    /// </summary>
    struct Character
    {
        public string name;
        public float health;
        public float attackPower;
        public float defensePower;
    }

    class Game
    {
        bool gameOver;
        int currentScene;
        Character player;
        Character[] enemies;
        private int currentEnemyIndex = 0;
        private Character currentEnemy;

        /// <summary>
        /// Function that starts the main game loop
        /// </summary>
        public void Run()
        {
            Start();

            while (!gameOver)
            {
                Update();
            }

            End();
            
        }

        /// <summary>
        /// Function used to initialize any starting values by default
        /// </summary>
        public void Start()
        {
            Character slime = new Character { name = "Slime", health = 10, attackPower = 1, defensePower = 0 }; 

            Character zomb = new Character { name = "Zom-B", health = 15, attackPower = 5, defensePower = 2 }; 

            Character kris = new Character { name = "guy named Kris", health = 25, attackPower = 15, defensePower = 5 };

            enemies = new Character[] { slime, zomb, kris };

            ResetCurrentEnemy();
        }

        /// <summary>
        /// This function is called every time the game loops.
        /// </summary>
        public void Update()
        {
            UpdateCurrentScene();
            Console.Clear();
        }

        /// <summary>
        /// This function is called before the applications closes
        /// </summary>
        public void End()
        {
            Console.WriteLine("Farewell Adventurer!");
            Console.ReadKey(true);
        }

        void ResetCurrentEnemy()
        {
            currentEnemyIndex = 0;

            currentEnemy = enemies[currentEnemyIndex];
        }

        void UpdateCurrentScene()
        {
            switch (currentScene)
            {
                case 0:
                    DisplayMainMenu();
                    break;

                case 1:
                    DisplayCurrentScene();
                    UpdateCurrentEnemy();
                    break;

                case 2:
                    DisplayRestartMenu();
                    break;
                case 3:
                    Console.WriteLine("You have slain all the enemies.");
                    Console.ReadKey(true);
                    DisplayRestartMenu();
                    break;

                default:
                    Console.WriteLine("Invalid scene index");
                    break;
            }
        }

        void UpdateCurrentEnemy()
        {
            if (currentEnemyIndex >= enemies.Length)
            {
                currentScene = 3;
            }
            if (currentEnemy.health <= 0)
            {
                Console.WriteLine("You have slayed the " + currentEnemy.name + "!");
                Console.ReadKey(true);
                currentEnemyIndex++;

                if (TryEndSimulation())
                {
                    return;
                }

                currentEnemy = enemies[currentEnemyIndex];
            }
            if (player.health <= 0)
            {
                Console.WriteLine("You have died.");
                Console.ReadKey(true);
                currentScene = 2;
            }
        }
        /// <summary>
        /// Gets an input from the player based on some given decision
        /// </summary>
        /// <param name="description">The context for the input</param>
        /// <param name="option1">The first option the player can choose</param>
        /// <param name="option2">The second option the player can choose</param>
        /// <returns></returns>
        int GetInput(string description, string option1, string option2)
        {
            string input = "";
            int inputReceived = 0;

            while (inputReceived != 1 && inputReceived != 2)
            {//Print options
                Console.WriteLine(description);
                Console.WriteLine("1. " + option1);
                Console.WriteLine("2. " + option2);
                Console.Write("> ");

                //Get input from player
                input = Console.ReadLine();

                //If player selected the first option...
                if (input == "1" || input == option1)
                {
                    //Set input received to be the first option
                    inputReceived = 1;
                }
                //Otherwise if the player selected the second option...
                else if (input == "2" || input == option2)
                {
                    //Set input received to be the second option
                    inputReceived = 2;
                }
                //If neither are true...
                else
                {
                    //...display error message
                    Console.WriteLine("Invalid Input");
                    Console.ReadKey();
                }

                Console.Clear();
            }
            return inputReceived;
        }

        /// <summary>
        /// Calls the appropriate function(s) based on the current scene index
        /// </summary>
        void DisplayCurrentScene()
        {
            DisplayStats(player);
            DisplayStats(currentEnemy);
            Battle();
        }

        /// <summary>
        /// Displays the menu that allows the player to start or quit the game
        /// </summary>
        void DisplayMainMenu()
        {
            GetPlayerName();
            CharacterSelection();
            currentScene = 1;
            Console.Clear();

        }

        void DisplayRestartMenu()
        {
            int choice = GetInput("Play Again?", "Yes", "No");

            if (choice == 1)
            {
                ResetCurrentEnemy();
                currentScene = 0;
            }
            else if (choice == 2)
            {
                gameOver = true;
            }
        }

        /// <summary>
        /// Displays text asking for the players name. Doesn't transition to the next section
        /// until the player decides to keep the name.
        /// </summary>
        void GetPlayerName()
        {
            bool validInputRecieved = true;
            while (validInputRecieved == true)
            {
                Console.Write("Welcome! Please enter your name.\n> ");
                player.name = Console.ReadLine();
                Console.Clear();

                int choice = GetInput("You've entered " + player.name + ", are you sure you want to keep this name?", 
                    "Yes", "No");
                if (choice == 1)
                {
                    validInputRecieved = false;
                }
                else
                {
                    validInputRecieved = true;
                }
            }
            
        }

        /// <summary>
        /// Gets the players choice of character. Updates player stats based on
        /// the character chosen.
        /// </summary>
        public void CharacterSelection()
        {
            int choice = GetInput("Nice to meet you " + player.name + ". Please select a character.", "Wizard", "Knight");

            if (choice == 1)
            {
                player.health = 50;
                player.attackPower = 25;
                player.defensePower = 0;
            }
            else if (choice == 2)
            {
                player.health = 75;
                player.attackPower = 15;
                player.defensePower = 10;
            }
        }

        /// <summary>
        /// Prints a characters stats to the console
        /// </summary>
        /// <param name="character">The character that will have its stats shown</param>
        void DisplayStats(Character character)
        {
            Console.WriteLine("Name: " + character.name);
            Console.WriteLine("Health: " + character.health);
            Console.WriteLine("Attack Power: " + character.attackPower);
            Console.WriteLine("Defense Power: " + character.defensePower);
            Console.WriteLine("");
        }

        /// <summary>
        /// Calculates the amount of damage that will be done to a character
        /// </summary>
        /// <param name="attackPower">The attacking character's attack power</param>
        /// <param name="defensePower">The defending character's defense power</param>
        /// <returns>The amount of damage done to the defender</returns>
        float CalculateDamage(float attackPower, float defensePower)
        {
            float damage = attackPower - defensePower;

            if (damage <= 0)
            {
                damage = 0;
            }

            return damage;
        }

        /// <summary>
        /// Deals damage to a character based on an attacker's attack power
        /// </summary>
        /// <param name="attacker">The character that initiated the attack</param>
        /// <param name="defender">The character that is being attacked</param>
        /// <returns>The amount of damage done to the defender</returns>
        public float Attack(ref Character attacker, ref Character defender)
        {
            float damageTaken = CalculateDamage(attacker.attackPower, defender.defensePower);
            defender.health -= damageTaken;

            if (defender.health < 0)
            {
                defender.health = 0;
            }

            return damageTaken;
        }

        /// <summary>
        /// Simulates one turn in the current monster fight
        /// </summary>
        public void Battle()
        {
            int choice = GetInput("A " + currentEnemy.name + " stands in front of you! What will you do?",
                "Attack", "Dodge");

            if (choice == 1)
            {
                float damageTaken = Attack(ref player, ref currentEnemy);
                Console.WriteLine("You dealt " + damageTaken + " damage!");

                damageTaken = Attack(ref currentEnemy, ref player);
                Console.WriteLine("The " + currentEnemy.name + " has dealt " + damageTaken);

                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.WriteLine("You dodged the enemies attack!");

                Console.ReadKey(true);
                Console.Clear();
            }
            
        }

        bool TryEndSimulation()
        {
            bool simulationOver = currentEnemyIndex >= enemies.Length;

            if (simulationOver)
            {
                currentScene = 3;
            }

            return simulationOver;
        }

    }
}
