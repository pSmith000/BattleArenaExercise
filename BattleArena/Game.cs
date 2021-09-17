using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{

    public struct Item
    {
        public string Name;
        public float StatBoost;
    }

    class Game
    {
        private bool _gameOver;
        private int _currentScene;
        private Player _player;
        private Entity[] _enemies;
        private int _currentEnemyIndex = 0;
        private Entity _currentEnemy;
        private string _playerName;
        private Item[] _wizardItems;
        private Item[] _knightItems;

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
            InitializeItems();
        }

        
        public void InitializeItems()
        {
            //Wizard Items
            Item bigWand = new Item { Name = "Big Wand", StatBoost = 5 };
            Item bigShield = new Item { Name = "Big Shield", StatBoost = 15 };

            //Knight Items
            Item wand = new Item { Name = "Wand", StatBoost = 1025 };
            Item shoes = new Item { Name = "Yeezies", StatBoost = 9000.05f };

            //Initialize arrays
            _wizardItems = new Item[] { bigWand, bigShield };
            _knightItems = new Item[] { wand, shoes };
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
        int GetInput(string description, params string[] options)
        {
            string input = "";
            int inputRecieved = -1;

            while (inputRecieved == -1)
            {
                //Print options
                Console.WriteLine(description);


            }
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
                _player = new Player(_playerName, 50, 25, 0, _wizardItems);
            }
            else if (choice == 2)
            {
                _player = new Player(_playerName, 75, 15, 10, _knightItems);
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
                "Attack", "Equip Item");

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
