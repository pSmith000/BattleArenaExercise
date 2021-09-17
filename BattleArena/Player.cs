﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{
    class Player : Entity
    {
        private Item[] _items;
        private Item _currentItem;

        public Item CurrentItem
        {
            get
            {
                return _currentItem;
            }
        }

        public Player(string name, float health, float attackPower, float defensePower, Item[] items) : base(name, health, attackPower, defensePower)
        {
            _items = items;
            _currentItem.Name = "Nothing";
        }

        /// <summary>
        /// Sets the item at the given index to be the current item
        /// </summary>
        /// <param name="index">the index of the item in the array</param>
        /// <returns>false if the index is outside the bounds of the array</returns>
        public bool TryEquipItem(int index)
        {
            //If the index is out of bounds...
            if (index >= _items.Length || index < 0)
            {
                //...return false
                return false;
            }

            //Set the current item to be the array at the given index
            _currentItem = _items[index];

            return true;
        }

        /// <summary>
        /// Set the current item to be nothing
        /// </summary>
        /// <returns>false if there is no item equipped</returns>
        public bool TryRemoveCurrentItem()
        {
            //Returns false if there is no item equipped
            if (CurrentItem.Name == "Nothing")
            {
                return false;
            }

            //Set item to be nothing
            _currentItem = new Item();
            _currentItem.Name = "Nothing";
            return true;
        }
    }
}
