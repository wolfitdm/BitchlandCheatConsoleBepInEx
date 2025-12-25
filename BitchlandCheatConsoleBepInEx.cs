using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Den.Tools;
using HarmonyLib;
using HarmonyLib.Tools;
using SemanticVersioning;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Json;
using System.Security.Policy;
using System.Text.RegularExpressions;
using UMA.Examples;
using UnityEngine;

namespace BitchlandCheatConsoleBepInEx
{
    [BepInPlugin("com.wolfitdm.BitchlandCheatConsoleBepInEx", "BitchlandCheatConsoleBepInEx Plugin", "1.0.0.0")]
    public class BitchlandCheatConsoleBepInEx : BaseUnityPlugin
    {
        private static bool showGUI = false;       // Toggle GUI visibility
        private bool pressEnter = false;
        private string inputText = "";      // Stores user input
        private Rect windowRect = new Rect(20, 20, 300, 150); // GUI window position
        private static Dictionary<string, Vector3> spawnpoints = new Dictionary<string, Vector3>();
        private static List<string> itemsP = new List<string>();
        private static List<string> spawnpointsNames = new List<string>();
        private static bool isInit = false;
        private static int itemPCount = 0;
        private static int spawnpointsCount = 0;

        private void Init()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;

            spawnpoints.Clear();
            spawnpointsNames.Add("safearea1");
            spawnpointsNames.Add("safearea2");
            spawnpointsNames.Add("safearea3");
            spawnpointsNames.Add("safearea4");
            spawnpoints.Add("safearea1", new Vector3(-2.949118f, 1.192093E-07f, 39.10889f));
            spawnpoints.Add("safearea2", new Vector3(179.8053f, 0.05544382f, -73.4415f));
            spawnpoints.Add("safearea3", new Vector3(-69f, 0.0f, 10f));
            spawnpoints.Add("safearea4", new Vector3(-49.10827f, 3.067196f, 14.40517f));
            spawnpointsCount = spawnpoints.Count;

            itemsP.Clear();
            itemsP.Add(null);
            itemsP.Add("Any");
            itemsP.Add("Shoes");
            itemsP.Add("Pants");
            itemsP.Add("Top");
            itemsP.Add("UnderwearTop");
            itemsP.Add("UnderwearLower");
            itemsP.Add("Garter");
            itemsP.Add("Socks");
            itemsP.Add("Hat");
            itemsP.Add("Hair");
            itemsP.Add("MaleHair");
            itemsP.Add("Bodies");
            itemsP.Add("Heads");
            itemsP.Add("Beards");
            itemsP.Add("ProstSuit1");
            itemsP.Add("ProstSuit2");
            itemPCount = itemsP.Count;
        }

        private void TriggerUpdate()
        {
            Init();
            // Toggle GUI with F1 key
            if (Input.GetKeyUp(KeyCode.F1) || Input.GetKeyUp(KeyCode.F2))
            {
                showGUI = !showGUI;
            }

            if (showGUI)
            {
                onOpenCheatConsole();
            }
            else
            {
                onCloseCheatConsole();
            }
        }

        private void Update()
        {
           TriggerUpdate();
        } 
        private void OnGUI()
        {
            if (!showGUI) return;

            // Draw a draggable window
            windowRect = GUI.Window(0, windowRect, DrawWindow, "BitchlandCheatConsole");
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.Label("Command:");

            // Input text field

            GUI.SetNextControlName("TextField");

            inputText = GUILayout.TextField(inputText, 100); // Max 50 chars

            bool nothingHasFocus = GUI.GetNameOfFocusedControl() == string.Empty;
            bool hasFocus = GUI.GetNameOfFocusedControl() == "TextField";

            // in case nothing else if focused, focus our input
            if (!hasFocus && nothingHasFocus)
            {
                GUI.FocusControl("TextField");
            }

            bool pressSubmitButton = GUILayout.Button("Submit");
            bool pressEnter = Event.current.isKey && Event.current.keyCode == KeyCode.Return && hasFocus;
            bool pressF1 = Event.current.isKey && Event.current.keyCode == KeyCode.F1 && hasFocus;
            bool pressF2 = Event.current.isKey && Event.current.keyCode == KeyCode.F2 && hasFocus;

            if (pressF1 || pressF2)
            {
                showGUI = !showGUI;
            }

            if (pressEnter)
            {
                pressSubmitButton = true;
                Event.current.Use(); // Prevents newline in TextField
            }

            // Submit button
            if (pressSubmitButton)
            {
                if (string.IsNullOrWhiteSpace(inputText))
                {
                    Logger.LogWarning("Input is empty!");
                }
                else
                {
                    handleCommand(inputText);
                }
                inputText = "";
            }

            // Allow window dragging
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public static void pregnancy(bool realpregnancy)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: pregnancy");
            float fertility = Main.Instance.Player.Fertility;
            float storymodefertility = Main.Instance.Player.StoryModeFertility;

            bool togglePregnancy = fertility > 0 || storymodefertility > 0;
            
            togglePregnancy = !togglePregnancy;

            if (togglePregnancy)
            {
                fertility = 1;
                storymodefertility = 0;
                Main.Instance.GameplayMenu.ShowNotification("pregnancy on");
            } 
            else
            {
                fertility = 0;
                storymodefertility = 0;
                Main.Instance.GameplayMenu.ShowNotification("pregnancy off");
            }

            if (realpregnancy)
            {
                Main.Instance.Player.States[7] = togglePregnancy; // toggle you are pregnant state
            }
        }

        public static void infinitehealth()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: infinitehealth");
            Main.Instance.Player.CantBeHit = !Main.Instance.Player.CantBeHit;
            if (Main.Instance.Player.CantBeHit)
            {
                Main.Instance.GameplayMenu.ShowNotification("infinitehealth: on");
            } else
            {
                Main.Instance.GameplayMenu.ShowNotification("infinitehealth: off");
            }
        }

        public static void nohunger()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: nohunger");
            Main.Instance.Player.Hunger = 0;
        }

        public static void notoilet()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: notoilet");
            Main.Instance.Player.Toilet = 0;
        }

        public static void fullenergy()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: fullenergy");
            Main.Instance.Player.Energy = Main.Instance.Player.EnergyMax;
        }

        public static void maxarousal(int level)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxarousal");
            int add = 300;
            level = level > 0 && level <= add ? level : add;
            add = level;
            Main.Instance.Player.Arousal = add;
        }


        public static void maxsexskills(int level)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxsexskills");
            int add = 300;
            level = level > 0 && level <= add ? level : add;
            add = level;
            Main.Instance.Player.SexSkills = add;
            int sexMax = Main.Instance.Player.SexXpThisLvlMax;
            sexMax = sexMax >= 0 ? sexMax : add;
            Main.Instance.Player.SexXpThisLvl = sexMax;
        }

        public static void maxworkskills(int level)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxworkskills");
            int add = 300;
            level = level > 0 && level <= add ? level : add;
            add = level;
            Main.Instance.Player.WorkSkills = add;
            int workMax = Main.Instance.Player.WorkXpThisLvlMax;
            workMax = workMax >= 0 ? workMax : add;
            Main.Instance.Player.WorkXpThisLvl = workMax;
        }

        public static void maxarmyskills(int level)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxarmyskills");
            int add = 300;
            level = level > 0 && level <= add ? level : add;
            add = level;
            Main.Instance.Player.ArmySkills = add;
            int armyMax = Main.Instance.Player.ArmyXpThisLvlMax;
            armyMax = armyMax >= 0 ? armyMax : add;
            Main.Instance.Player.ArmyXpThisLvl = armyMax;
        }

        public static void maxallskills(int level)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxallskills");
            maxsexskills(level);
            maxworkskills(level);
            maxarmyskills(level);
        }

        public static void addallitems()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: addallitems");

            if (Main.Instance.Player.CurrentBackpack == null)
            {
                GameObject backpack2 = getItemByName(null, "backpack2");
                if (backpack2 == null)
                {
                    backpack2 = getItemByName(null, "backpack");
                }
                if (backpack2 == null)
                {
                    return;
                }
                Main.Instance.Player.DressClothe(Main.Spawn(backpack2));
            }

            if (Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null)
            {
                Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;

                List<GameObject> items = getAllItems();

                int length = items.Count;

                for (int i = 0; i < length; i++)
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.AddItem(items[i]);
                }
            }
        }

        public static void cleanskin()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: cleanskin");
            Person _this = Main.Instance.Player;
            _this.States[0] = false;
            _this.States[2] = false;
            _this.States[3] = false;
            _this.States[8] = false;
            _this.States[12] = false;
            _this.States[13] = false;
            _this.States[14] = false;
            _this.States[15] = false;
            _this.States[16 /*0x10*/] = false;
            _this.States[17] = false;
            _this.States[18] = false;
            _this.States[19] = false;
            _this.States[26] = false;
            _this.States[33] = false;
            _this.States[23] = false;
            _this.States[24] = false;
            _this.States[25] = false;
            _this.DirtySkin = false;
        }

        public static void heal()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: heal");
            Main.Instance.Player.TheHealth.currentHealth = Main.Instance.Player.TheHealth.maxHealth;
            Main.Instance.GameplayMenu.UpdateHealth();
        }

        public static void nude(bool realnude)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: nude");
            Main.Instance.Player.ClothingCondition = e_ClothingCondition.Nude;
            Main.Instance.Player.States[9] = true; // Nude Clothing Vipe
            Main.Instance.Player.States[10] = false; // Casual Clothing Vipe
            Main.Instance.Player.States[11] = false; // Sexy Clothing Vipe

            if (realnude)
            {
                Main.Instance.Player.States[0] = false; // Dirty -10 Sexy
                Main.Instance.Player.States[1] = false; // Horny +10 Sexy
                Main.Instance.Player.States[2] = false; // Very Dirty -20 Sexy
                Main.Instance.Player.States[3] = false; // Shitten -20 Sexy
                                                        //Main.Instance.Player.States[4] = false; // Sleepy - speed
                                                        //Main.Instance.Player.States[5] = false; // Needs toilet
                                                        //Main.Instance.Player.States[6] = false; // Hungry
                                                        //Main.Instance.Player.States[7] = false; // Pregnant
                Main.Instance.Player.States[8] = false; // Bloody -10 Sexy
                                                        //Main.Instance.Player.States[9] = false; // Clothing vibe Nude
                                                        //Main.Instance.Player.States[10] = false; // Clothing vibe Casual
                                                        //Main.Instance.Player.States[12] = true; // Clothing vibe Sexy
                Main.Instance.Player.States[12] = false; // Cum Stains + 1 Sexy 
                Main.Instance.Player.States[13] = false; // Cum Stains + 2 Sexy
                Main.Instance.Player.States[14] = false; // Cum Stains + 3 Sexy
                Main.Instance.Player.States[15] = false; // Cum Stains + 4 Sexy 
                Main.Instance.Player.States[16] = false; // Cum Stains + 5 Sexy 
                Main.Instance.Player.States[17] = false; // Body Writting + 1 Sexy 
                Main.Instance.Player.States[18] = false; // Body Writting + 2 Sexy 
                Main.Instance.Player.States[19] = false; // Body Writting + 3 Sexy 
                Main.Instance.Player.States[20] = false; // Bruises - 10 Sexy 
                Main.Instance.Player.States[21] = false; // Heavy Bruises - 20 Sexy 
                Main.Instance.Player.States[22] = false; // Basic Makeup + 10 Sexy
                Main.Instance.Player.States[23] = false; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[24] = false; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[25] = false; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[26] = false; // Cum in mouth + 1 Sexy
                                                         //Main.Instance.Player.States[27] = true; // Beard
                                                         //Main.Instance.Player.States[28] = true; // Lipstick
                                                         //Main.Instance.Player.States[29] = true; // Lipstick
                                                         //Main.Instance.Player.States[30] = true; // Lipstick
                                                         //Main.Instance.Player.States[31] = true; // Skin color lips
                                                         //Main.Instance.Player.States[32] = true; // Freckets
                                                         //Main.Instance.Player.States[33] = true; // Dirty Mouth
            }

            if (Main.Instance.Player.EquippedClothes == null)
            {
                return;
            }

            int add = 100;

            for (int i = 0; i < Main.Instance.Player.EquippedClothes.Count; i++)
            {
                Main.Instance.Player.EquippedClothes[i].CasualPoints = 0;
                Main.Instance.Player.EquippedClothes[i].SexyPoints = 0;
            }

            Main.Instance.Player.GetClothingCondition();
        }
        public static void sexy(bool realsexy)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: sexy");
            Main.Instance.Player.ClothingCondition = e_ClothingCondition.Sexy;
            Main.Instance.Player.States[9] = false; // Nude Clothing Vipe
            Main.Instance.Player.States[10] = false; // Casual Clothing Vipe
            Main.Instance.Player.States[11] = true; // Sexy Clothing Vipe

            if (realsexy)
            {
                Main.Instance.Player.States[0] = false; // Dirty -10 Sexy
                Main.Instance.Player.States[1] = true; // Horny +10 Sexy
                Main.Instance.Player.States[2] = false; // Very Dirty -20 Sexy
                Main.Instance.Player.States[3] = false; // Shitten -20 Sexy
                                                        //Main.Instance.Player.States[4] = false; // Sleepy - speed
                                                        //Main.Instance.Player.States[5] = false; // Needs toilet
                                                        //Main.Instance.Player.States[6] = false; // Hungry
                                                        //Main.Instance.Player.States[7] = false; // Pregnant
                Main.Instance.Player.States[8] = false; // Bloody -10 Sexy
                                                        //Main.Instance.Player.States[9] = false; // Clothing vibe Nude
                                                        //Main.Instance.Player.States[10] = false; // Clothing vibe Casual
                                                        //Main.Instance.Player.States[12] = true; // Clothing vibe Sexy
                Main.Instance.Player.States[12] = true; // Cum Stains + 1 Sexy 
                Main.Instance.Player.States[13] = true; // Cum Stains + 2 Sexy
                Main.Instance.Player.States[14] = true; // Cum Stains + 3 Sexy
                Main.Instance.Player.States[15] = true; // Cum Stains + 4 Sexy 
                Main.Instance.Player.States[16] = true; // Cum Stains + 5 Sexy 
                Main.Instance.Player.States[17] = true; // Body Writting + 1 Sexy 
                Main.Instance.Player.States[18] = true; // Body Writting + 2 Sexy 
                Main.Instance.Player.States[19] = true; // Body Writting + 3 Sexy 
                Main.Instance.Player.States[20] = false; // Bruises - 10 Sexy 
                Main.Instance.Player.States[21] = false; // Heavy Bruises - 20 Sexy 
                Main.Instance.Player.States[22] = true; // Basic Makeup + 10 Sexy
                Main.Instance.Player.States[23] = true; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[24] = true; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[25] = true; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[26] = true; // Cum in mouth + 1 Sexy
                                                        //Main.Instance.Player.States[27] = true; // Beard
                                                        //Main.Instance.Player.States[28] = true; // Lipstick
                                                        //Main.Instance.Player.States[29] = true; // Lipstick
                                                        //Main.Instance.Player.States[30] = true; // Lipstick
                                                        //Main.Instance.Player.States[31] = true; // Skin color lips
                                                        //Main.Instance.Player.States[32] = true; // Freckets
                                                        //Main.Instance.Player.States[33] = true; // Dirty Mouth
            }

            if (Main.Instance.Player.EquippedClothes == null)
            {
                return;
            }

            int add = 100;

            for (int i = 0; i < Main.Instance.Player.EquippedClothes.Count; i++)
            {
                Main.Instance.Player.EquippedClothes[i].CasualPoints = 0;
                Main.Instance.Player.EquippedClothes[i].SexyPoints = add;
                Main.Instance.Player.EquippedClothes[i].SexyPoints = Main.Instance.Player.EquippedClothes[i].CasualPoints + Main.Instance.Player.EquippedClothes[i].SexyPoints + add;
            }

            Main.Instance.Player.GetClothingCondition();
        }

        public static void casual(bool realcasual)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: casual");
            Main.Instance.Player.ClothingCondition = e_ClothingCondition.Casual;
            Main.Instance.Player.States[9] = false; // Nude
            Main.Instance.Player.States[10] = true; // Casual
            Main.Instance.Player.States[11] = false; // Sexy

            if (realcasual)
            {
                Main.Instance.Player.States[0] = false; // Dirty -10 Sexy
                Main.Instance.Player.States[1] = false; // Horny +10 Sexy
                Main.Instance.Player.States[2] = false; // Very Dirty -20 Sexy
                Main.Instance.Player.States[3] = false; // Shitten -20 Sexy
                                                        //Main.Instance.Player.States[4] = false; // Sleepy - speed
                                                        //Main.Instance.Player.States[5] = false; // Needs toilet
                                                        //Main.Instance.Player.States[6] = false; // Hungry
                                                        //Main.Instance.Player.States[7] = false; // Pregnant
                Main.Instance.Player.States[8] = false; // Bloody -10 Sexy
                                                        //Main.Instance.Player.States[9] = false; // Clothing vibe Nude
                                                        //Main.Instance.Player.States[10] = false; // Clothing vibe Casual
                                                        //Main.Instance.Player.States[12] = true; // Clothing vibe Sexy
                Main.Instance.Player.States[12] = false; // Cum Stains + 1 Sexy 
                Main.Instance.Player.States[13] = false; // Cum Stains + 2 Sexy
                Main.Instance.Player.States[14] = false; // Cum Stains + 3 Sexy
                Main.Instance.Player.States[15] = false; // Cum Stains + 4 Sexy 
                Main.Instance.Player.States[16] = false; // Cum Stains + 5 Sexy 
                Main.Instance.Player.States[17] = false; // Body Writting + 1 Sexy 
                Main.Instance.Player.States[18] = false; // Body Writting + 2 Sexy 
                Main.Instance.Player.States[19] = false; // Body Writting + 3 Sexy 
                Main.Instance.Player.States[20] = false; // Bruises - 10 Sexy 
                Main.Instance.Player.States[21] = false; // Heavy Bruises - 20 Sexy 
                Main.Instance.Player.States[22] = false; // Basic Makeup + 10 Sexy
                Main.Instance.Player.States[23] = false; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[24] = false; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[25] = false; // Runny Makeup + 1 Sexy
                Main.Instance.Player.States[26] = false; // Cum in mouth + 1 Sexy
                                                         //Main.Instance.Player.States[27] = true; // Beard
                                                         //Main.Instance.Player.States[28] = true; // Lipstick
                                                         //Main.Instance.Player.States[29] = true; // Lipstick
                                                         //Main.Instance.Player.States[30] = true; // Lipstick
                                                         //Main.Instance.Player.States[31] = true; // Skin color lips
                                                         //Main.Instance.Player.States[32] = true; // Freckets
                                                         //Main.Instance.Player.States[33] = true; // Dirty Mouth
            }

            if (Main.Instance.Player.EquippedClothes == null)
            {
                return;
            }

            int add = 100;

            for (int i = 0; i < Main.Instance.Player.EquippedClothes.Count; i++)
            {
                Main.Instance.Player.EquippedClothes[i].CasualPoints = add;
                Main.Instance.Player.EquippedClothes[i].SexyPoints = 0;
                Main.Instance.Player.EquippedClothes[i].CasualPoints = Main.Instance.Player.EquippedClothes[i].CasualPoints + Main.Instance.Player.EquippedClothes[i].SexyPoints + add;
            }

            Main.Instance.Player.GetClothingCondition();
        }
        public static void clearbackpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: clearbackpack");
            if (Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null)
            {

                Main.Instance.Player.CurrentBackpack.ThisStorage.RemoveAllItems();
            }
        }

        public static void showpos()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: showpos");
            Vector3 lastSpawnPoint = Main.Instance.Player.transform.position;
            Main.Instance.GameplayMenu.ShowNotification(lastSpawnPoint.x.ToString() + " " + lastSpawnPoint.y.ToString() + " " + lastSpawnPoint.z.ToString());
            Main.Instance.GameplayMenu.ShowNotification(lastSpawnPoint.x.ToString() + " " + lastSpawnPoint.y.ToString() + " " + lastSpawnPoint.z.ToString());

        }

        public static void handleCommand(string inputText)
        {
            if (inputText == null) { return; }

            Logger.LogInfo($"User entered: {inputText}");


            string pattern3 = @"(?:^(?<command>\w+)\s+(?<key>\w+)\s+(?<value>\w+)$)";
            Regex rg3 = new Regex(pattern3, RegexOptions.IgnoreCase);
            Match rg3Match = rg3.Match(inputText);

            if (rg3Match.Success)
            {
                handleCommandLength3(rg3Match.Groups["command"].Value.ToLower(), rg3Match.Groups["key"].Value.ToLower(), rg3Match.Groups["value"].Value.ToLower());
                return;
            }

            string pattern2 = @"(?:^(?<command>\w+)\s+(?<value>\w+)$)";
            Regex rg2 = new Regex(pattern2, RegexOptions.IgnoreCase);
            Match rg2Match = rg2.Match(inputText);

            if (rg2Match.Success)
            {
                handleCommandLength2(rg2Match.Groups["command"].Value.ToLower(), rg2Match.Groups["value"].Value.ToLower());
                return;
            }

            string pattern1 = @"(?:^(?<command>\w+)$)";

            Regex rg1 = new Regex(pattern1, RegexOptions.IgnoreCase);
            Match rg1Match = rg1.Match(inputText);

            if (rg1Match.Success)
            {
                handleCommandLength1(rg1Match.Groups["command"].Value.ToLower());
                return;
            }

        }
        public static void handleCommandLength1(string command)
        {
            switch (command)
            {
                case "clearbackpack":
                    {
                        clearbackpack();
                    }
                    break;

                case "showpos":
                    {
                        showpos();
                    }
                    break;

                case "warplist":
                    {
                        warplist();
                    }
                    break;

                case "itemlist":
                    {
                        itemlist();
                    }
                    break;

                case "weaponlist":
                    {
                        weaponlist();
                    }
                    break;

                case "cleanskin":
                    {
                        cleanskin();
                    }
                    break;

                case "nude":
                    {
                        nude(false);
                    }
                    break;

                case "casual":
                    {
                        casual(false);
                    }
                    break;

                case "sexy":
                    {
                        sexy(false);
                    }
                    break;

                case "realnude":
                    {
                        nude(true);
                    }
                    break;

                case "realcasual":
                    {
                        casual(true);
                    }
                    break;

                case "realsexy":
                    {
                        sexy(true);
                    }
                    break;

                case "heal":
                    {
                        heal();
                    }
                    break;

                case "nohunger":
                    {
                        nohunger();
                    }
                    break;

                case "fullenergy":
                    {
                        fullenergy();
                    }
                    break;

                case "notoilet":
                    {
                        notoilet();
                    }
                    break;

                case "maxsexskills":
                    {
                        maxallskills(300);
                    }
                    break;

                case "maxarmyskills":
                    {
                        maxarmyskills(300);
                    }
                    break;

                case "maxworkskills":
                    {
                        maxworkskills(300);
                    }
                    break;

                case "maxallskills":
                    {
                        maxallskills(300);
                    }
                    break;

                case "maxarousal":
                    {
                        maxarousal(300);
                    }
                    break;

                case "addallitems":
                    {
                        addallitems();
                    }
                    break;

                case "pregnancy":
                    {
                        pregnancy(false);
                    }
                    break;

                case "realpregnancy":
                    {
                        pregnancy(true);
                    }
                    break;

                case "invincible":
                case "infinitehealth":
                    {
                        infinitehealth();
                    }
                    break;

                default: {
                        Main.Instance.GameplayMenu.ShowNotification("No command");
                    }
                    break;
            }
        }

        public static void handleCommandLength2(string command, string value)
        {
            switch (command)
            {
                case "warp":
                    {
                        warp(value);
                    }
                    break;

                case "addmoney":
                    {
                        addmoney(value);
                    }
                    break;

                case "addweapon":
                    {
                        addweapon(value);
                    }
                    break;

                case "getstate":
                    {
                        getstate(value);
                    }
                    break;

                case "setfavor":
                    {
                        setfavor(value);
                    }
                    break;

                case "setsexmultiplier":
                    {
                        setsexmultiplier(value);
                    }
                    break;

                case "setsexmaddictionmultiplier":
                    {
                        setsexmaddictionmultiplier(value);
                    }
                    break;

                case "setsexskills":
                    {
                        setsexskills(value);
                    }
                    break;

                case "setarmyskills":
                    {
                        setarmyskills(value);
                    }
                    break;

                case "setworkskills":
                    {
                        setworkskills(value);
                    }
                    break;

                case "setallskills":
                    {
                        setallskills(value);
                    }
                    break;

                case "setarousal":
                    {
                        setarousal(value);
                    }
                    break;

                default: {
                        Main.Instance.GameplayMenu.ShowNotification("No command ");
                    }
                    break;
            }
        }

        public static void handleCommandLength3(string command, string key, string value)
        {
            switch (command)
            {
                case "additem":
                    {
                        additem(key, value);
                    }
                    break;

                case "setstate":
                    {
                        setstate(key, value);
                    }
                    break;

                default: {
                        Main.Instance.GameplayMenu.ShowNotification("No command");
                    }
                    break;
            }
        }
        private static List<Weapon> getPrefabsByName2(string prefab)
        {
            List<Weapon> Prefabs = null;
            if (prefab == null)
            {
                Prefabs = Main.Instance.Prefabs_Weapons;
                return Prefabs;
            }

            switch (prefab)
            {
                case "Weapons":
                    {
                        Prefabs = Main.Instance.Prefabs_Weapons;
                    }
                    break;

                default:
                    {
                        Prefabs = Main.Instance.Prefabs_Weapons;
                    }
                    break;
            }

            return Prefabs;
        }
        private static Weapon getWeaponByName(string prefab, string name)
        {
            List<Weapon> Prefabs = getPrefabsByName2(prefab);
            if (Prefabs == null)
            {
                return null;
            }

            if (name == null || name.Length == 0)
            {
                return null;
            }

            name = name.ToLower();

            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                if (Prefabs[i].IsNull())
                {
                    continue;
                }

                string wname = Prefabs[i].name;
                wname = wname.ToLower().Replace(" ", "_");
                if (wname == name)
                {
                    return Prefabs[i];
                }
            }
            return null;
        }

        private static List<string> getAllWeaponsByPrefab(string prefab)
        {
            List<Weapon> Prefabs = getPrefabsByName2(prefab);

            if (Prefabs == null)
            {
                return null;
            }
            
            List<string> all = new List<string>();
            
            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                if (Prefabs[i].IsNull())
                {
                    continue;
                }

                //bag.PickupWeapon(Main.Spawn(weapon));
                Weapon item = Prefabs[i];
                string name = item.name.Replace(" ", "_").ToLower();
                all.Add(name);
            }

            return all;
        }

        private static void showWeaponsInLogByPrefab()
        {
            string prefabName = "Weapons";

            List<string> Prefabs = getAllWeaponsByPrefab(null);

            if (Prefabs == null)
            {
                return;
            }

            string itemi = "-------------------------------------------------------------------" + prefabName + "-------------------------------------------------------------------";
            Debug.Log((object)itemi);

            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                string item = Prefabs[i];
                Debug.Log((object)item);
            }
        }


        public static void addweapon(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: addweapon");

            Weapon weapon = getWeaponByName(null, value);

            if (weapon != null)
            {
                GameObject weaponx = Main.Spawn(weapon.gameObject);
                
                if (weaponx == null)
                {
                    return;
                }

                Main.Instance.Player.WeaponInv.DropAllWeapons();
                Main.Instance.Player.WeaponInv.PickupWeapon(weaponx);
            }
        }

        public static void setarousal(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setarousal");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= 300 ? 300 : amount;

            int level = amount;
            maxarousal(level);
        }
        public static void setsexskills(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setsexskills");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= 300 ? 300 : amount;

            int level = amount;
            maxsexskills(level);
        }

        public static void setworkskills(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setworkskills");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= 300 ? 300 : amount;

            int level = amount;
            maxworkskills(level);
        }

        public static void setarmyskills(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setarmyskills");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= 300 ? 300 : amount;

            int level = amount;
            maxarmyskills(level);
        }

        public static void setallskills(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setallskills");

            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= 300 ? 300 : amount;

            int level = amount;

            maxsexskills(level);
            maxworkskills(level);
            maxarmyskills(level);
        }

        public static void setsexmaddictionmultiplier(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setsexmaddictionmultiplier");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= int.MaxValue ? int.MaxValue : amount;

            Main.Instance.Player.SexMAddictionultiplier = (float)amount;
        }
        public static void setsexmultiplier(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setsexmultiplier");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= int.MaxValue ? int.MaxValue : amount;

            Main.Instance.Player.SexMultiplier = (float)amount;
        }

        public static void setfavor(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setfavor");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= int.MaxValue ? int.MaxValue : amount;

            Main.Instance.Player.Favor = amount;
        }

        public static void getstate(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: getstate");
            int statesLength = Main.Instance.Player.States.Length;

            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= statesLength ? statesLength - 1 : amount;
            int index = amount;

            string state_string = index.ToString() + ": " + Main.Instance.States_Data[index].Name + (Main.Instance.States_Data[index].Effect.Length == 0 ? string.Empty : $" ({Main.Instance.States_Data[index].Effect})");
            Main.Instance.GameplayMenu.ShowNotification(state_string);
        }

        public static void addmoney(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: addmoney");

            int amount = 1;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 1;
            }

            amount = amount <= 0 ? 1 : amount;
            int money = Main.Instance.Player.Money;

            money += amount;

            if (money < 0)
            {
                money = int.MaxValue;
            }

            Main.Instance.Player.Money = money;
        }

        public static void warp(string town)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: warp");
            if (spawnpoints.ContainsKey(town))
            {
                Main.Instance.Player.transform.position = spawnpoints[town];
            } else
            {
                Main.Instance.GameplayMenu.ShowNotification("No telepoint point found");
            }
        }

        public static void warplist()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: warplist");
            for (int i = 0; i < spawnpointsCount; i++)
            {
                Main.Instance.GameplayMenu.ShowNotification(spawnpointsNames[i]);
            }
        }

        private static void showItemsInLogByPrefab(string prefab)
        {
            string prefabName = null;

            if (prefab == null)
            {
                prefabName = "AllPrefabs";
            }
            else
            {
                prefabName = prefab;
            }

            List<GameObject> Prefabs = getPrefabsByName(prefab);

            if (Prefabs == null)
            {
                return;
            }

            string itemi = "-------------------------------------------------------------------" + prefabName + "-------------------------------------------------------------------";
            Debug.Log((object)itemi);

            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                if (Prefabs[i].IsNull())
                {
                    continue;
                }

                string iname = Prefabs[i].name;
                iname = iname.ToLower().Replace(" ", "_");
                string item = iname;
                Debug.Log((object)item);

            }
        }
        public static void itemlist()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: itemlist");

            for (int i = 0; i < itemPCount; i++)
            {
                showItemsInLogByPrefab(itemsP[i]);
            }

            return;
        }
        public static void weaponlist()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: weaponlist");
            showWeaponsInLogByPrefab();
            return;
        }

        private static List<GameObject> getPrefabsByName(string prefab)
        {
            List<GameObject> Prefabs = null;

            if (prefab == null)
            {
                Prefabs = Main.Instance.AllPrefabs;
                return Prefabs;
            }

            switch (prefab)
            {
                case "Any":
                    {
                        Prefabs = Main.Instance.Prefabs_Any;
                    }
                    break;

                case "Shoes":
                    {
                        Prefabs = Main.Instance.Prefabs_Shoes;
                    }
                    break;

                case "Pants":
                    {
                        Prefabs = Main.Instance.Prefabs_Pants;
                    }
                    break;

                case "Top":
                    {
                        Prefabs = Main.Instance.Prefabs_Top;
                    }
                    break;

                case "UnderwearTop":
                    {
                        Prefabs = Main.Instance.Prefabs_UnderwearTop;
                    }
                    break;

                case "UnderwearLower":
                    {
                        Prefabs = Main.Instance.Prefabs_UnderwearLower;
                    }
                    break;

                case "Garter":
                    {
                        Prefabs = Main.Instance.Prefabs_Garter;
                    }
                    break;

                case "Socks":
                    {
                        Prefabs = Main.Instance.Prefabs_Socks;
                    }
                    break;

                case "Hat":
                    {
                        Prefabs = Main.Instance.Prefabs_Hat;
                    }
                    break;

                case "Hair":
                    {
                        Prefabs = Main.Instance.Prefabs_Hair;
                    }
                    break;

                case "MaleHair":
                    {
                        Prefabs = Main.Instance.Prefabs_MaleHair;
                    }
                    break;

                case "Bodies":
                    {
                        Prefabs = Main.Instance.Prefabs_Bodies;
                    }
                    break;

                case "Heads":
                    {
                        Prefabs = Main.Instance.Prefabs_Heads;
                    }
                    break;

                case "Beards":
                    {
                        Prefabs = Main.Instance.Prefabs_Beards;
                    }
                    break;

                case "ProstSuit1":
                    {
                        Prefabs = Main.Instance.Prefabs_ProstSuit1;
                    }
                    break;

                case "ProstSuit2":
                    {
                        Prefabs = Main.Instance.Prefabs_ProstSuit2;
                    }
                    break;

                case "Weapons":
                    {
                        Prefabs = null;
                    }
                    break;

                default:
                    {
                        Prefabs = Main.Instance.AllPrefabs;
                    }
                    break;
            }

            return Prefabs;
        }
        private static GameObject getItemByName(string prefab, string name)
        {
            List<GameObject> Prefabs = getPrefabsByName(prefab);
            if (Prefabs == null)
            {
                return null;
            }

            if (name == null || name.Length == 0)
            {
                return null;
            }

            name = name.ToLower();

            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                if (Prefabs[i].IsNull())
                {
                    continue;
                }

                string iname = Prefabs[i].name;
                iname = iname.ToLower().Replace(" ", "_");
                
                if (iname == name)
                {
                    return Prefabs[i];
                }
            }
            return null;
        }

        private static GameObject getAllItemByName(string name)
        {
            GameObject item = null;

            if (name == null) { 
                return null; 
            }

            for (int i = 0; i < itemPCount; i++)
            {
                item = getItemByName(itemsP[i], name);
                if (item != null) { 
                    return item; 
                }
            }

            return null;
        }

        private static List<GameObject> getAllItems()
        {
            List<GameObject> itemList = new List<GameObject> ();

            for (int i = 0; i < itemPCount; i++)
            {
                string prefab = itemsP[i];
                List<GameObject> items = getPrefabsByName(prefab);
                if (items == null)
                {
                    continue;
                }
                for (int j = 0; j < items.Count; j++)
                {
                    if (items[j] != null)
                        itemList.Add(items[j]);
                }
            }

            return itemList;
        }
        private static void addItemReal(GameObject item, int value)
        {
            if (Main.Instance.Player.CurrentBackpack == null)
            {
                GameObject backpack2 = getItemByName(null, "backpack2");
                if (backpack2 == null)
                {
                    backpack2 = getItemByName(null, "backpack");
                }
                if (backpack2 == null)
                {
                    return;
                }
                Main.Instance.Player.DressClothe(Main.Spawn(backpack2));
            }

            if (Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null)
            {
                Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;

                value = value <= 0 ? 1 : value;

                for (int i = 0; i < value; i++)
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.AddItem(item);
                }
            }
        }
        public static void setstate(string key, string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setstate");
            int statesLength = Main.Instance.Player.States.Length;

            int amount = 0;
            try
            {
                amount = int.Parse(key);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? 0 : amount;
            amount = amount >= statesLength ? statesLength - 1 : amount;

            int index = amount;

            try
            {
                amount = int.Parse(value);
                amount = amount >= 1 ? 1 : 0;
            }
            catch (Exception ex)
            {
                amount = 0;
                bool amount2 = false;
                try
                {
                   amount2 = bool.Parse(value);
                   amount = amount2 ? 1 : 0;
                }
                catch (Exception ex2)
                {
                    amount = 0;
                }
            }

            bool state = amount >= 1 ? true : false;
            
            Main.Instance.Player.States[index] = state;
            Main.Instance.GameplayMenu.ShowNotification("setstate " + index.ToString() + " to " + (state ? "true" : "false"));
        }
        public static void additem(string key, string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: additem");
            int amount = 1;
            GameObject item = getAllItemByName(key);

            if (item != null)
            {
                try
                {
                    amount = int.Parse(value);
                    amount = amount > 0 ? amount : 1;
                }
                catch (Exception e)
                {
                    amount = 1;
                }
                addItemReal(item, amount);
                Main.Instance.GameplayMenu.ShowNotification("additem: " + item.name.ToString() + " " + amount.ToString() + " added");
            } else
            {
                Main.Instance.GameplayMenu.ShowNotification("additem: No item found");
            }
        }

        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> configEnableMe;

        public BitchlandCheatConsoleBepInEx()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        private static string pluginKey = "General.Toggles";

        public static bool enableThisMod = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableThisMod",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");


            enableThisMod = configEnableMe.Value;

            Harmony.CreateAndPatchAll(typeof(BitchlandCheatConsoleBepInEx));

            Logger.LogInfo($"Plugin BitchlandCheatConsoleBepInEx BepInEx is loaded!");
        }

        public void onOpenCheatConsole()
        {
            if (onOpenCheat)
            {
                return;
            }
            onOpenCheat = true;
            Main.Instance.GameplayMenu.CloseEscMenu();
            Main.Instance.GameplayMenu.CloseJournal();
            Main.Instance.GameplayMenu.AllowCursor();
            Main.Instance.GameplayMenu.UpdateAmmo();
            Main.Instance.GameplayMenu.UpdateNeeds();
            Main.Instance.GameplayMenu.UpdateArousal();
        }

        public void onCloseCheatConsole()
        {
            if (!onOpenCheat)
            {
                return;
            }
            onOpenCheat = false;
            Main.Instance.GameplayMenu.CloseEscMenu();
            Main.Instance.GameplayMenu.CloseJournal();
            Main.Instance.GameplayMenu.DisallowCursor();
            Main.Instance.GameplayMenu.UpdateAmmo();
            Main.Instance.GameplayMenu.UpdateNeeds();
            Main.Instance.GameplayMenu.UpdateArousal();
        }

        private static bool onOpenCheat = false;

        /*[HarmonyPatch(typeof(UI_Gameplay), "SelectInventory")]
        [HarmonyPrefix]
        public static bool SelectInventory(object __instance)
        {
            UI_Gameplay _this = (UI_Gameplay)__instance;
            _this.CloseAllJournalMenus();
            _this.Journal_Inventory.SetActive(true);
            for (int index = 0; index < _this.MenuButtons.Length; ++index)
                _this.MenuButtons[index].sprite = _this.UnselectedButton;
            _this.MenuButtons[1].sprite = _this.SelectedButton;
            for (int index = 0; index < _this.InvEntries.Count; ++index)
                UnityEngine.Object.Destroy((UnityEngine.Object)_this.InvEntries[index]);
            _this.InvEntries.Clear();
            _this.BitchNotesText.text = Main.Instance.Player.Money.ToString();
            for (int index = 0; index < Main.Instance.Player.WeaponInv.weapons.Count; ++index)
            {
                if ((UnityEngine.Object)Main.Instance.Player.WeaponInv.weapons[index] != (UnityEngine.Object)null)
                {
                    misc_invItem component = UnityEngine.Object.Instantiate<GameObject>(_this.InvEntry, _this.InvEntry.transform.parent).GetComponent<misc_invItem>();
                    component.Title.text = Main.Instance.Player.WeaponInv.weapons[index].name;
                    component.ThisWeapomn = Main.Instance.Player.WeaponInv.weapons[index];
                    component.gameObject.SetActive(true);
                    _this.InvEntries.Add(component.gameObject);
                }
            }
            for (int index = 0; index < Main.Instance.Player.EquippedClothes.Count; ++index)
            {
                switch (Main.Instance.Player.EquippedClothes[index].BodyPart)
                {
                    case DressableType.Hair:
                    case DressableType.Head:
                    case DressableType.Body:
                        continue;
                    default:
                        if (!Main.Instance.Player.EquippedClothes[index].HideFromInv)
                        {
                            misc_invItem component = UnityEngine.Object.Instantiate<GameObject>(_this.InvEntry, _this.InvEntry.transform.parent).GetComponent<misc_invItem>();
                            component.Title.text = Main.Instance.Player.EquippedClothes[index].name;
                            component.ThisDressable = Main.Instance.Player.EquippedClothes[index];
                            if (Main.Instance.Player.EquippedClothes[index].BodyPart == DressableType.BackPack)
                                component.OpenBtn.SetActive(true);
                            else if ((UnityEngine.Object)Main.Instance.Player.CurrentBackpack != (UnityEngine.Object)null && !component.ThisDressable.CantBeDroppedByPlayer)
                                component.SendBtn.SetActive(true);
                            if (component.ThisDressable.CantBeDroppedByPlayer)
                                component.DropBtn.interactable = false;
                            component.gameObject.SetActive(true);
                            _this.InvEntries.Add(component.gameObject);
                            continue;
                        }
                        continue;
                }
            }
            for (int index = 0; index < Main.Instance.Player.Storage_Hands.StorageItems.Count; ++index)
            {
                misc_invItem component = UnityEngine.Object.Instantiate<GameObject>(_this.InvEntry, _this.InvEntry.transform.parent).GetComponent<misc_invItem>();
                component.Title.text = "(Hands) " + Main.Instance.Player.Storage_Hands.StorageItems[index].name;
                component.ThisDressable = (Dressable)null;
                component.ThisItem = Main.Instance.Player.Storage_Hands.StorageItems[index];
                component.ThisStorage = Main.Instance.Player.Storage_Hands;
                component.DropBtn.interactable = true;
                component.gameObject.SetActive(true);
                _this.InvEntries.Add(component.gameObject);
            }
            for (int index = 0; index < Main.Instance.Player.Storage_Vag.StorageItems.Count; ++index)
            {
                misc_invItem component = UnityEngine.Object.Instantiate<GameObject>(_this.InvEntry, _this.InvEntry.transform.parent).GetComponent<misc_invItem>();
                component.Title.text = "(Vagina) " + Main.Instance.Player.Storage_Vag.StorageItems[index].name;
                component.ThisDressable = (Dressable)null;
                component.ThisItem = Main.Instance.Player.Storage_Vag.StorageItems[index];
                component.ThisStorage = Main.Instance.Player.Storage_Vag;
                component.DropBtn.interactable = true;
                component.gameObject.SetActive(true);
                _this.InvEntries.Add(component.gameObject);
            }
            for (int index = 0; index < Main.Instance.Player.Storage_Anal.StorageItems.Count; ++index)
            {
                misc_invItem component = UnityEngine.Object.Instantiate<GameObject>(_this.InvEntry, _this.InvEntry.transform.parent).GetComponent<misc_invItem>();
                component.Title.text = "(Anal) " + Main.Instance.Player.Storage_Anal.StorageItems[index].name;
                component.ThisDressable = (Dressable)null;
                component.ThisItem = Main.Instance.Player.Storage_Anal.StorageItems[index];
                component.ThisStorage = Main.Instance.Player.Storage_Anal;
                component.DropBtn.interactable = true;
                component.gameObject.SetActive(true);
                _this.InvEntries.Add(component.gameObject);
            }
            _this.InvContent.sizeDelta = new Vector2(0.0f, (float)(_this.InvEntries.Count * 40 + 50));
            for (int index = 0; index < _this.StatesEntries.Count; ++index)
                UnityEngine.Object.Destroy((UnityEngine.Object)_this.StatesEntries[index]);
            for (int index = 0; index < Main.Instance.Player.States.Length; ++index)
            {
                //Main.Instance.Player.States[index] = index < 20 ? false : true;
                if (Main.Instance.Player.States[index])
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(_this.StatesEntry, _this.StatesEntry.transform.parent);
                    gameObject.transform.Find("text").GetComponent<UnityEngine.UI.Text>().text = index.ToString() + ":::" + Main.Instance.States_Data[index].Name + (Main.Instance.States_Data[index].Effect.Length == 0 ? string.Empty : $" ({Main.Instance.States_Data[index].Effect})");
                    gameObject.SetActive(true);
                    _this.StatesEntries.Add(gameObject);
                }
            }
            Main.Instance.MusicPlayer.PlayOneShot(_this.JournalSound1);
            return false;
        }*/
    }
}
