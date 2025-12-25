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
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using UMA.Examples;
using UnityEngine;

namespace BitchlandCheatConsoleBepInEx
{
    [BepInPlugin("com.wolfitdm.BitchlandCheatConsoleBepInEx", "BitchlandCheatConsoleBepInEx Plugin", "1.0.0.0")]
    public class BitchlandCheatConsoleBepInEx : BaseUnityPlugin
    {
        private bool showGUI = false;       // Toggle GUI visibility
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

        private void Update()
        {
            Init();
            // Toggle GUI with F1 key
            if (Input.GetKeyUp(KeyCode.F1))
            {
                showGUI = !showGUI;
            }

            if (showGUI)
            {
                onOpenCheatConsole();
            } else
            {
                onCloseCheatConsole();
            }
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

            bool pressSubmitButton = GUILayout.Button("Submit");
            bool pressEnter = Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "TextField";

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

        public static void nude()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: nude");
            Main.Instance.Player.ClothingCondition = e_ClothingCondition.Nude;
            Main.Instance.Player.States[9] = true; // Nude
            Main.Instance.Player.States[10] = false; // Casual
            Main.Instance.Player.States[11] = false; // Sexy

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
        public static void sexy()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: sexy");
            Main.Instance.Player.ClothingCondition = e_ClothingCondition.Sexy;
            Main.Instance.Player.States[9] = false; // Nude
            Main.Instance.Player.States[10] = false; // Casual
            Main.Instance.Player.States[11] = true; // Sexy

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

        public static void casual()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: casual");
            Main.Instance.Player.ClothingCondition = e_ClothingCondition.Casual;
            Main.Instance.Player.States[9] = false; // Nude
            Main.Instance.Player.States[10] = true; // Casual
            Main.Instance.Player.States[11] = false; // Sexy

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

                case "cleanskin":
                    {
                        cleanskin();
                    }
                    break;

                case "casual":
                    {
                        casual();
                    }
                    break;

                case "sexy":
                    {
                        sexy();
                    }
                    break;

                case "nude":
                    {
                        nude();
                    }
                    break;

                case "heal":
                    {
                        heal();
                    }
                    break;

                case "addallitems":
                    {
                        addallitems();
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

                default: {
                        Main.Instance.GameplayMenu.ShowNotification("No command");
                    }
                    break;
            }
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
                string item = Prefabs[i].name.ToLower().Replace(" ", "_");
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
            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                if (Prefabs[i].IsNull())
                {
                    continue;
                }

                if (Prefabs[i].name.ToLower().Replace(" ", "_") == name)
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
        private static UI_Gameplay UI_Gameplay_Instance = null;

        [HarmonyPatch(typeof(UI_Gameplay), "Update")] 
        [HarmonyPrefix]
        public static bool Update(object __instance)
        {
            if (UI_Gameplay_Instance == null && __instance != null)
            {
                UI_Gameplay_Instance = (UI_Gameplay) __instance;
            }
            return true;
        }
    }
}
