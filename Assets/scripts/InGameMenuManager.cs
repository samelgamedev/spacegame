using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public static class InGameMenuManager 
    {
        private static bool open;
        private static UINavigateable ui;

        public static void Open()
        {
            // don't open the menu if it's already open
            if (open) return;
            open = !open;

            // stop the player's horizontal movement animation
            Controller.instance.StopHorizontalAnimation();

            // create ui
            ui = UIManager.instance.NewNavigateable(new Vector2(-328, -4), new Vector2(292, 100),
                customPrintTextOptions: UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.Instant);
            // set options
            ui.SetOptions(new string[]
            {
                "back",
                "inventory",
                "party",
                "settings",
                "save",
                "title"
            }, new Action(() => ProcessMenuButton(ui.selectedOption)));
        }

        public static void Close()
        {
            open = false;

            // destroy ui
            ui.DestroyGameObject();
            ui.readyToDequeue = true;
            ui = null;

            // allow movement
            Controller.instance.canMove = true;
        }

        public static void ProcessMenuButton(string button)
        {
            switch (button)
            {
                case "back":
                    Close();
                    break;
                case "inventory":
                    InventoryManager.OpenUI();
                    break;
                case "party":
                    // TODO: tuck this into a method in PartyManager
                    ui = UIManager.instance.NewNavigateable(new Vector2(-18, 193), new Vector2(300, 100),
                        UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.Instant);

                    // concatenate an array containing only "back" and a ling selection array with the party member names 
                    ui.SetOptions(new string[] { "back" }.Concat(PartyManager.GetPartyMembers()
                        .Select((PartyMemberData p ) => p.name)).ToArray(),
                        // callback
                        () => 
                        { 
                            switch (ui.selectedOption)
                            {
                                case "back":
                                    ui.DestroyGameObject();
                                    break;
                                default:
                                    // info text
                                    PartyMemberData p = PartyManager.GetPartyMember(ui.index - 1);

                                    string infoText = $"{p.name}\n\n" +
                                    $"level: {p.level}\n" +
                                    $"hp: {p.baseHp}\n" +
                                    $"attack: {p.fullAttack}\n" +
                                    $"defense: {p.fullDefense}\n\n" +
                                    $"weapon: {p.weapon.name ?? "none"}\n" +
                                    $"armour: {p.armour.name ?? "none"}";

                                    UI info = UIManager.instance.New(new Vector2(-122, 48.5f), new Vector2(620, 430));
                                    info.StartCoroutine(info.PrintText(
                                        infoText,
                                        options: UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.DestroyUIAfterCallback));

                                    break;
                            }
                        });
                    break;
                case "inventory":
                    InventoryManager.OpenUI();
                    break;
                case "settings":
                    break;
                case "save":
                    break;
                // return to title screen
                case "title":
                    UINavigateable options = null;
                    UI textbox = null;

                    // create callback to printing the confirmation text as creating the ui navigateable
                    Action createOptions = new Action(() =>
                    {
                        // create navigateable
                        options = UIManager.instance.NewNavigateable(new Vector2(92, -134), new Vector2(485, 50), 
                            UI.PrintTextOptions.Instant | UI.PrintTextOptions.DestroyUIAfterCallback | UI.PrintTextOptions.CallbackAfterInput);

                        // also destroy the textbox after the callback
                        options.alsoDestroy.Add(textbox);

                        // set the options
                        options.SetOptions(new string[]
                        {
                            "no",
                            "yes",
                        },
                        // callback
                        new Action(() => 
                        {
                            switch (options.selectedOption)
                            {
                                case "no": // do nothing
                                    // destroy ui after callback isn't working here for some reason so i'll just do this
                                    options.DestroyGameObject();
                                    break;
                                case "yes":
                                    Close();
                                    MapManager.ChangeMap("title");
                                    break;
                            }
                        }));
                    });

                    textbox = UIManager.instance.New(new Vector2(92, 94), new Vector2(484, 257));
                    
                    textbox.StartCoroutine(textbox.PrintText(
                        $"are you sure you want to return to the title screen? (last saved format this seconds ago)",
                        createOptions, UI.PrintTextOptions.CallbackAfterPrinting | UI.PrintTextOptions.DontPushToInputQueue));

                    break;
            }
        }
    }
}
