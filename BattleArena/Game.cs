using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{
    class Game
    {
        private bool _gameOver;
        private int _currentScene;
        private Entity _player;
        private Entity[] _enemies;
        private int _currentEnemyIndex = 0;
        private Entity _currentEnemy;
        private string _playerName;

        /// <summary>
        /// Function that starts the main game loop
        /// </summary>
        public void Run()
        {
            Start();

            while (!_gameOver)
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
            _currentEnemyIndex = 0;

            Entity slime = new Entity("Slime", 10, 1, 0);

            Entity zomb = new Entity("Zom-B", 15, 5, 2);

            Entity kris = new Entity("guy named Kris", 25, 15, 5);

            _enemies = new Entity[] { slime, zomb, kris };

            _currentEnemy = _enemies[_currentEnemyIndex];
        }

        void UpdateCurrentScene()
        {
            switch (_currentScene)
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
            if (_currentEnemyIndex >= _enemies.Length)
            {
                _currentScene = 3;
            }
            if (_currentEnemy.Health <= 0)
            {
                Console.WriteLine("You have slayed the " + _currentEnemy.Name + "!");
                Console.ReadKey(true);
                _currentEnemyIndex++;

                if (TryEndSimulation())
                {
                    return;
                }

                _currentEnemy = _enemies[_currentEnemyIndex];
            }
            if (_player.Health <= 0)
            {
                Console.WriteLine("You have died.");
                Console.ReadKey(true);
                _currentScene = 2;
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
            DisplayStats(_player);
            DisplayStats(_currentEnemy);
            Battle();
        }

        /// <summary>
        /// Displays the menu that allows the player to start or quit the game
        /// </summary>
        void DisplayMainMenu()
        {
            GetPlayerName();
            CharacterSelection();
            _currentScene = 1;
            Console.Clear();

        }

        void DisplayRestartMenu()
        {
            int choice = GetInput("Play Again?", "Yes", "No");

            if (choice == 1)
            {
                ResetCurrentEnemy();
                _currentScene = 0;
            }
            else if (choice == 2)
            {
                _gameOver = true;
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
                _playerName = Console.ReadLine();
                Console.Clear();

                int choice = GetInput("You've entered " + _playerName + ", are you sure you want to keep this name?", 
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
            int choice = GetInput("Nice to meet you " + _playerName + ". Please select a character.", "Wizard", "Knight");

            if (choice == 1)
            {
                _player = new Entity(_playerName, 50, 25, 0);
            }
            else if (choice == 2)
            {
                _player = new Entity(_playerName, 75, 15, 10);
            }
        }

        /// <summary>
        /// Prints a characters stats to the console
        /// </summary>
        /// <param name="character">The character that will have its stats shown</param>
        void DisplayStats(Entity character)
        {
            Console.WriteLine("Name: " + character.Name);
            Console.WriteLine("Health: " + character.Health);
            Console.WriteLine("Attack Power: " + character.AttackPower);
            Console.WriteLine("Defense Power: " + character.DefensePower);
            Console.WriteLine("");
        }

        /// <summary>
        /// Simulates one turn in the current monster fight
        /// </summary>
        public void Battle()
        {
            int choice = GetInput("A " + _currentEnemy.Name + " stands in front of you! What will you do?",
                "Attack", "Dodge");

            if (choice == 1)
            {
                float damageTaken = _player.Attack(_currentEnemy);
                Console.WriteLine("You dealt " + damageTaken + " damage!");

                damageTaken = _currentEnemy.Attack(_player);
                Console.WriteLine("The " + _currentEnemy.Name + " has dealt " + damageTaken);

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
            bool simulationOver = _currentEnemyIndex >= _enemies.Length;

            if (simulationOver)
            {
                _currentScene = 3;
            }

            return simulationOver;
        }

    }
}
