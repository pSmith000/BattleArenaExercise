using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BattleArena
{

    public enum ItemType
    {
        DEFENSE, 
        ATTACK,
        NONE
    }

    public enum Scene
    {
        STARTMENU,
        MAINMENU,
        BATTLE,
        RESTARTMENU,
        KILLALLENEMIES
    }

    public struct Item
    {
        public string Name;
        public float StatBoost;
        public ItemType Type;
    }

    class Game
    {
        private bool _gameOver;
        private Scene _currentScene;
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
            Item bigWand = new Item { Name = "Big Wand", StatBoost = 5, Type = ItemType.ATTACK};
            Item bigShield = new Item { Name = "Big Shield", StatBoost = 15, Type = ItemType.DEFENSE };

            //Knight Items
            Item wand = new Item { Name = "Wand", StatBoost = 1025, Type = ItemType.ATTACK };
            Item shoes = new Item { Name = "Yeezies", StatBoost = 9000.05f, Type = ItemType.DEFENSE };

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

        public void Save()
        {
            //Create a new stream writer
            StreamWriter writer = new StreamWriter("SaveData.txt");

            //Save current enemy index
            writer.WriteLine(_currentEnemyIndex);

            //Save player and enemy stats
            _player.Save(writer);
            _currentEnemy.Save(writer);

            //Close the writer when done saving
            writer.Close();
        }

        public bool Load()
        {
            bool loadSuccessful = true;

            //If the file doesn't exist...
            if (!File.Exists("SaveData.txt"))
            {
                //...return false
                loadSuccessful = false;
            }

            //Create a new reader to read from the text file
            StreamReader reader = new StreamReader("SaveData.txt");

            //If the first line can't be converted into an integer...
            if (!int.TryParse(reader.ReadLine(), out _currentEnemyIndex))
            {
                //...return false
                loadSuccessful = false;
            }

            //Load player job
            string job = reader.ReadLine();

            //Assign items based on player job
            if(job == "Wizard")
            {
                _player = new Player(_wizardItems);
            }
            else if (job == "Knight")
            {
                _player = new Player(_knightItems);
            }
            else
            {
                loadSuccessful = false;
            }

            _player.Job = job;

            if (!_player.Load(reader))
            {
                loadSuccessful = false;
            }

            //Create a new instance and try to reaload the enemy
            _currentEnemy = new Entity();
            if (!_currentEnemy.Load(reader))
            {
                loadSuccessful = false;
            }

            //Update the array to match the current enemy stats
            _enemies[_currentEnemyIndex] = _currentEnemy;

            //Close the reader
            reader.Close();

            return loadSuccessful;
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
                case Scene.STARTMENU:
                    DisplayStartMenu();
                    break;

                case Scene.MAINMENU:
                    DisplayMainMenu();
                    break;

                case Scene.BATTLE:
                    DisplayCurrentScene();
                    UpdateCurrentEnemy();
                    break;

                case Scene.RESTARTMENU:
                    DisplayRestartMenu();
                    break;
                case Scene.KILLALLENEMIES:
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
                _currentScene = Scene.KILLALLENEMIES;
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
                _currentScene = Scene.RESTARTMENU;
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

                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine((i + 1) + "." + options[i]);
                }
                Console.Write("> ");

                //Get input from player
                input = Console.ReadLine();

                //If the player typed an int...
                if (int.TryParse(input, out inputRecieved))
                {
                    //...decrement the input and check if it's within the bounds of the array
                    inputRecieved--;
                    if (inputRecieved < 0 || inputRecieved >= options.Length)
                    {
                        //Set input recieved to be the default value
                        inputRecieved = -1;
                        //Display error message
                        Console.WriteLine("Invalid Input");
                        Console.ReadKey(true);
                    }
                }
                //If the player didn't type an int
                else
                {
                    //Set input recieved to be the default value
                    inputRecieved = -1;
                    Console.WriteLine("Invalid Input");
                    Console.ReadKey(true);
                }

                Console.Clear();
            }

            return inputRecieved;
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
            _currentScene = Scene.BATTLE;
            Console.Clear();

        }

        void DisplayRestartMenu()
        {
            int choice = GetInput("Play Again?", "Yes", "No");

            if (choice == 0)
            {
                ResetCurrentEnemy();
                _currentScene = Scene.STARTMENU;
            }
            else if (choice == 1)
            {
                _gameOver = true;
            }
        }

        public void DisplayStartMenu()
        {
            int choice = GetInput("Welcome to Fight Club!", "Start New Game", "Load Game");

            if (choice == 0)
            {
                _currentScene = Scene.MAINMENU;
            }
            else if (choice == 1)
            {
                if (Load())
                {
                    Console.WriteLine("Load Successful!");
                    Console.ReadKey(true);
                    Console.Clear();
                    _currentScene = Scene.BATTLE;
                }
                else
                {
                    Console.WriteLine("Load Failed.");
                    Console.ReadKey(true);
                    Console.Clear();
                }
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
                if (choice == 0)
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

            if (choice == 0)
            {
                _player = new Player(_playerName, 50, 25, 0, _wizardItems, "Wizard");
            }
            else if (choice == 1)
            {
                _player = new Player(_playerName, 75, 15, 10, _knightItems, "Knight");
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

        public void DisplayEquipItemMenu()
        {
            //Get item index
            int choice = GetInput("Select an item to equip.", _player.GetItemNames());

            //Equip item at given index
            if (!_player.TryEquipItem(choice))
            {
                Console.WriteLine("You couldn't find that item in your bag.");
            }

            //Print feedback
            Console.WriteLine("You equipped " + _player.CurrentItem.Name + "!");
        }

        /// <summary>
        /// Simulates one turn in the current monster fight
        /// </summary>
        public void Battle()
        {
            int choice = GetInput("A " + _currentEnemy.Name + " stands in front of you! What will you do?",
                "Attack", "Equip Item", "Remove current item", "Save");

            if (choice == 0)
            {
                float damageTaken = _player.Attack(_currentEnemy);
                Console.WriteLine("You dealt " + damageTaken + " damage!");

                damageTaken = _currentEnemy.Attack(_player);
                Console.WriteLine("The " + _currentEnemy.Name + " has dealt " + damageTaken);

                Console.ReadKey(true);
                Console.Clear();
            }
            else if (choice == 1)
            {
                DisplayEquipItemMenu();
                Console.ReadKey(true);
                Console.Clear();
            }
            else if (choice == 2)
            {
                if (!_player.TryRemoveCurrentItem())
                {
                    Console.WriteLine("You don't have anything equipped.");
                }
                else
                {
                    Console.WriteLine("You placed the item in your bag");
                }
                Console.ReadKey(true);
                Console.Clear();
            }
            else if (choice == 3)
            {
                Save();
                Console.WriteLine("Saved Game");
                Console.ReadKey(true);
                Console.Clear();
            }
            
        }

        bool TryEndSimulation()
        {
            bool simulationOver = _currentEnemyIndex >= _enemies.Length;

            if (simulationOver)
            {
                _currentScene = Scene.KILLALLENEMIES;
            }

            return simulationOver;
        }

    }
}
