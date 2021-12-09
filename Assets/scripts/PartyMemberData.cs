using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public sealed class PartyMemberData
    {
        // the name of the party member in menus
        public string name;

        // base attack damage
        public int baseAttack;
        // base defense
        public int baseDefense;
        // base hp
        public int baseHp;
        // full attack damage
        public int fullAttack
        {
            get
            {
                return baseAttack + weapon.attackDamage;
            }
        }
        public int fullDefense
        {
            get
            {
                return baseAttack + armour.defense;
            }
        }

        // the weapon equipped by the party member
        public ItemData weapon;
        // the armour equipped by the party member
        public ItemData armour;

        // stuff that's used in battle
        public int currentHp;
        public int level = 1;

        public void ResetHp() => currentHp = baseHp;

        // this method is called when a battle starts
        public void OnEnterBattle()
        {
            // in case i need to add more stuff here
            ResetHp();
        }

        public void DealDamage(int damage) => currentHp -= damage;
    }
}
