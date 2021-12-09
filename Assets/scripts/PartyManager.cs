using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public static class PartyManager
    {
        private static List<PartyMemberData> partyMembers = new List<PartyMemberData>();

        // add a party member to the list
        public static void AddPartyMember(PartyMemberData partyMember)
        {
            partyMembers.Add(partyMember);
        }

        // remove a party member from the list by an index position
        public static void RemovePartyMember(int index)
        {
            partyMembers.RemoveAt(index);
        }

        // remove a party member from the list by its name
        public static void RemovePartyMember(string name)
        {
            foreach (PartyMemberData p in partyMembers)
            {
                if (p.name == name)
                {
                    partyMembers.Remove(p);
                    return;
                }
            }
        }

        // remove a party member from the list by a reference
        public static void RemovePartyMember(PartyMemberData partyMember)
        {
            foreach (PartyMemberData p in partyMembers)
            {
                if (p.Equals(partyMember))
                {
                    partyMembers.Remove(p);
                    return;
                }
            }
        }

        // get a party member by an index position
        public static PartyMemberData GetPartyMember(int index)
        {
            return partyMembers[index];
        }

        // get a party member by its name
        public static PartyMemberData GetPartyMember(string name)
        {
            foreach (PartyMemberData p in partyMembers)
            {
                if (p.name == name)
                {
                    return p;
                }
            }
            return null;
        }

        public static List<PartyMemberData> GetPartyMembers()
        {
            return partyMembers;
        } 
    }
}
