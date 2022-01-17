using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame
{
    // item metadata
    public struct ItemData
    {
        // name of the item in the ui
        public string name;
        // description of the item in the ui
        public string description;

        // whether the item should be shown as food in the ui
        public bool consumeAfterUse;
        // whether item should be shown as armour in the ui
        public bool armour;
        // whether the item should be shown as a weapon in the ui
        public bool weapon;

        // amount healed when the item is used
        public int healAmount;
        // defense given when the item is equipped
        public int defense;
        // attack damage when the item is used as a weapon
        public int attackDamage;

        // text displayed in battle
        public string[] battleText;
        // script that's ran if the item is used 
        public string script;
        // script args
        public string[] scriptArgs;

        // the price that the item is bought/sold for in shops
        public int shopPrice;

        // whether the item can be used in the menu
        public bool canBeUsedInMenu;
        // whether the item can be used in battle
        public bool canBeUsedInBattle;

        // constructor to initialize all of the fields manually, if needed
        public ItemData(
            string name,
            string description,
            bool consumeAfterUse, 
            bool armour, 
            bool weapon, 
            int healAmount, 
            int defense, 
            int attackDamage, 
            string[] battleText,
            string script,
            string[] scriptArgs,
            int shopPrice,
            bool canBeUsedInMenu,
            bool canBeUsedInBattle)
        {
            this.name = name;
            this.description = description;
            this.consumeAfterUse = consumeAfterUse;
            this.armour = armour;
            this.weapon = weapon;
            this.healAmount = healAmount;
            this.defense = defense;
            this.attackDamage = attackDamage;
            this.battleText = battleText;
            this.script = script;
            this.scriptArgs = scriptArgs;
            this.shopPrice = shopPrice;
            this.canBeUsedInMenu = canBeUsedInMenu;
            this.canBeUsedInBattle = canBeUsedInBattle;
        }
    }
}
