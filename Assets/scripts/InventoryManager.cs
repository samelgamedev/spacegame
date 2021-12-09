using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace spacegame
{
    // TODO: this needs some more comments outlining the purpose of things
    public static class InventoryManager 
    {
        public static List<ItemData> inventory = new List<ItemData>();

        private static List<ItemData> itemData = new List<ItemData>();
        private static UINavigateable ui;
        private static bool uiOpen;

        /////////////////////////////////////////////////////////////////////////
        // Public Methods
        /////////////////////////////////////////////////////////////////////////

        // initialization
        public static void InitItems()
        {
            // iterate through the item directories and deserialize the json file back into items
            foreach (string file in Directory.GetFiles(Application.streamingAssetsPath + "\\items\\", "*.json", SearchOption.AllDirectories))
            {
                ItemData i = JsonConvert.DeserializeObject<ItemData>(File.ReadAllText(file));
                itemData.Add(i);
            }
        }

        public static void GiveItem(ItemData item)
        {
            inventory.Add(item);
        }

        public static void GiveItem(string item)
            => GiveItem(GetItemByName(item).Value);

        public static void GiveItem(int index)
            => GiveItem(GetItemByIndex(index));

        public static ItemData? GetItemByName(string name)
        {
            foreach (ItemData i in itemData)
                if (i.name == name)
                    return i;
            Debug.Log($"failed to get item by name: {name}");
            return null;
        }

        public static ItemData GetItemByIndex(int index)
        {
            return itemData[index];
        }

        public static ItemData? GetItemFromInventoryByName(string name)
        {
            foreach (ItemData i in inventory)
                if (i.name == name)
                    return i;
            Debug.Log($"failed to get item from inventory by name: {name}");
            return null;
        }

        public static ItemData GetItemFromInventoryByIndex(int index)
        {
            return inventory[index - 1]; // this is really weird
        }

        // returns true if the item was found, otherwise returns false
        public static bool TryGetItemByName(string name, out ItemData item)
        {
            foreach (ItemData i in itemData)
            {
                if (i.name == name)
                {
                    item = i;
                    return true;
                }
            }
            Debug.Log($"failed to try get item by name: {name}");

            // set the out item to a sorta temp error handling item that we just create here
            item = new ItemData(
                "failed to try get item by name",
                "this is a bug, please send your player log file to the developer! (Documents/My Games/space!!!!/log.txt)",
                false, false, false, 0, 0, 0, null, string.Empty, null, 0, false, false);

            return false;
        }

        public static bool TryGetItemByIndex(int index, out ItemData item)
        {
            // if the item data has an item at the index, we can return true
            if (itemData.Count - 1 >= index)
            {
                item = itemData[index];
                return true;
            }
            // otherwise, return false
            Debug.Log($"failed to try get item by index: {index}");

            item = new ItemData(
                "failed to try get item by index",
                "this is a bug, please send your player log file to the developer! (Documents/My Games/space!!!!/log.txt)",
                false, false, false, 0, 0, 0, null, string.Empty, null, 0, false, false);

            return false;
        }

        public static bool TryGetItemFromInventoryByName(string name, out ItemData item)
        {
            foreach (ItemData i in inventory)
            {
                if (i.name == name)
                {
                    item = i;
                    return true;
                }
            }
            Debug.Log($"failed to try get item by name: {name}");

            // set the out item to a sorta temp error handling item that we just create here
            item = new ItemData(
                "failed to try get item by name",
                "this is a bug, please send your player log file to the developer! (Documents/My Games/space!!!!/log.txt)",
                false, false, false, 0, 0, 0, null, string.Empty, null, 0, false, false);

            return false;
        }

        public static bool TryGetItemFromInventoryByIndex(int index, out ItemData item)
        {
            // if the item data has an item at the index, we can return true
            if (inventory.Count - 1 >= index)
            {
                item = inventory[index];
                return true;
            }
            // otherwise, return false
            Debug.Log($"failed to try get item by index: {index}");

            item = new ItemData(
                "failed to try get item by index",
                "this is a bug, please send your player log file to the developer! (Documents/My Games/space!!!!/log.txt)",
                false, false, false, 0, 0, 0, null, string.Empty, null, 0, false, false);

            return false;
        }

        public static void OpenUI()
        {
            if (uiOpen) return;
            uiOpen = !uiOpen;

            ui = UIManager.instance.NewNavigateable(new Vector2(-18, 193), new Vector2(300, 100),
                UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.Instant);

            // concatenate an array containing only "back" and a ling selection array with the item data names 
            ui.SetOptions(new string[] { "back" }.Concat(inventory.Select((ItemData i) => i.name ?? NullItem())).ToArray(), 
                // callback
                () => ProcessInput(ui.selectedOption));
        }

        public static void CloseUI()
        {
            if (!uiOpen) return;
            uiOpen = false;

            ui.DestroyGameObject();
            ui = null;
        }

        // don't use this, just use the python script in dev scripts instead
        public static void GenerateItemDataSkeleton(string output)
        {
            string whatsit = JsonConvert.SerializeObject(new ItemData());
            File.WriteAllText(output, whatsit);
        }

        /////////////////////////////////////////////////////////////////////////
        // Private Methods
        /////////////////////////////////////////////////////////////////////////
        
        // if an item is null when trying to create the inventory ui, this method is called to log the contents of the list
        // and return a string indicating that an item was null
        private static string NullItem()
        {
            Debug.Log($"null item found in item list when trying to open ui, full list:\n * {string.Join("\n * ", itemData)}");
            return "(null item in list)";
        }

        private static void ProcessInput(string option)
        {
            if (option == "back")
            {
                CloseUI();
            }
            else
            {
                DisplayItem(GetItemFromInventoryByIndex(ui.index));
            }
        }

        private static void DisplayItem(ItemData? item)
        {
            // just in case the item that gets passed as a parameter is null
            if (item is null)
                item = new ItemData(
                "null",
                "this is a bug, please send your player log file to the developer! (Documents/My Games/space!!!!/log.txt)",
                false, false, false, 0, 0, 0, null, string.Empty, null, 0, false, false);

            // display the options (back, info, use)
            List<string> options = new List<string>
            {
                "back",
                "info",
            };
            if (item.Value.canBeUsedInMenu) options.Add("use");
            if (item.Value.weapon || item.Value.armour) options.Add("equip");

            UINavigateable ui = UIManager.instance.NewNavigateable(new Vector2(302, -4), new Vector2(300, 50),
                UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.Instant);
            ui.SetOptions(options.ToArray(),
                // callback
                () => 
                { 
                    switch (ui.selectedOption)
                    {
                        case "back":
                            ui.DestroyGameObject();
                            break;
                        case "info":
                            // show info
                            string infoText = $"{item.Value.name}\n\n\"{item.Value.description}\"\n";
                            if (item.Value.weapon) infoText += $"\nattack damage: {item.Value.attackDamage}";
                            if (item.Value.armour) infoText += $"\ndefense: {item.Value.defense}";
                            if (item.Value.consumeAfterUse) infoText += "\nconsumed after use";

                            UI info = UIManager.instance.New(new Vector2(-122, 48.5f), new Vector2(620, 380));
                            info.StartCoroutine(info.PrintText(
                                infoText,
                                options: UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.DestroyUIAfterCallback));
                            break;
                        case "use":
                            alisonscript.Interpreter.Run(item.Value.script ?? "item", item.Value.scriptArgs);
                            break;
                        case "equip":
                            UI thing = null;
                            UINavigateable options2 = null;

                            Action showOptions = new Action(() =>
                            {
                                options2 = UIManager.instance.NewNavigateable(new Vector2(-122, -144), new Vector2(620, 50));
                                options2.alsoDestroy.Add(thing);

                                // then set the options
                                options2.SetOptions(new string[] { "nevermind" }.Concat(PartyManager.GetPartyMembers()
                                    .Select((PartyMemberData p) => p.name)).ToArray(),
                                    // callback
                                    () =>
                                    {
                                        switch (options2.selectedOption)
                                        {
                                            case "nevermind":
                                                // just break, the game object will destroy
                                                break;
                                            default:
                                                if (item.Value.weapon)
                                                    PartyManager.GetPartyMember(options2.index - 1).weapon = item.Value;
                                                else if (item.Value.armour)
                                                    PartyManager.GetPartyMember(options2.index - 1).armour = item.Value;
                                                break;
                                        }
                                    });
                            });

                            thing = UIManager.instance.New(new Vector2(-122, 48.5f), new Vector2(620, 200));
                            thing.StartCoroutine(thing.PrintText("equip on who?", showOptions, UI.PrintTextOptions.CallbackAfterPrinting));

                            break;
                    }
                });
        }
    }
}
