using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Defective.JSON;
using Den.Tools;
using HarmonyLib;
using HarmonyLib.Tools;
using SemanticVersioning;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Json;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UMA.Examples;
using UnityEngine;
using UnityStandardAssets.Water;
using static Mono.Security.X509.X520;
using static UnityEngine.InputSystem.Controls.DiscreteButtonControl;
using static UnityEngine.Rendering.DebugUI.Table;

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
        private static JSONObject warpPointsUser = new JSONObject();
        private static List<string> predefinedUserWarpsNames = new List<string>();
        private static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

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
            spawnpointsNames.Add("slums");
            spawnpointsNames.Add("plaza");
            spawnpointsNames.Add("clinic");
            spawnpointsNames.Add("hs");
            spawnpointsNames.Add("highstreet");
            spawnpointsNames.Add("vs");
            spawnpointsNames.Add("vendingstreet");
            spawnpointsNames.Add("lab");
            spawnpointsNames.Add("garden");
            spawnpointsNames.Add("army");
            spawnpointsNames.Add("tc");
            spawnpointsNames.Add("trainingcenter");
            spawnpointsNames.Add("sc");
            spawnpointsNames.Add("stripclub");
            spawnpointsNames.Add("carol");
            spawnpointsNames.Add("jailyard");
            spawnpointsNames.Add("jail");
            spawnpointsNames.Add("f8");
            spawnpointsNames.Add("airdefenders");
            spawnpoints.Add("safearea1", new Vector3(-2.949118f, 1.192093E-07f, 39.10889f));
            spawnpoints.Add("safearea2", new Vector3(179.8053f, 0.05544382f, -73.4415f));
            spawnpoints.Add("safearea3", new Vector3(-69f, 0.0f, 10f));
            spawnpoints.Add("safearea4", new Vector3(-49.10827f, 3.067196f, 14.40517f));
            spawnpoints.Add("slums", new Vector3(46.70115f, 5.960464E-08f, 34.79505f));
            spawnpoints.Add("plaza", new Vector3(20.92864f, 0.06719887f, 2.584693f));
            spawnpoints.Add("clinic", new Vector3(18.31792f, 0.06720525f, 15.40343f));
            spawnpoints.Add("hs", new Vector3(70.0073f, 0.05544364f, -59.5325f));
            spawnpoints.Add("highstreet", new Vector3(70.0073f, 0.05544364f, -59.5325f));
            spawnpoints.Add("vs", new Vector3(86.47327f, 0.06720108f, -6.379012f));
            spawnpoints.Add("vendingstreet", new Vector3(86.47327f, 0.06720108f, -6.379012f));
            spawnpoints.Add("lab", new Vector3(95.99965f, 5.960464E-08f, -120.2192f));
            spawnpoints.Add("garden", new Vector3(95.99965f, 5.960464E-08f, -120.2192f));
            spawnpoints.Add("army", new Vector3(-26.46828f, 0f, 34.43687f));
            spawnpoints.Add("tc", new Vector3(-48.20982f, 0.07555467f, 3.772348f));
            spawnpoints.Add("trainingcenter", new Vector3(-48.20982f, 0.07555467f, 3.772348f));
            spawnpoints.Add("sc", new Vector3(-2.626162f, 0.06719756f, 6.736708f));
            spawnpoints.Add("stripclub", new Vector3(-2.626162f, 0.06719756f, 6.736708f));
            spawnpoints.Add("carol", new Vector3(-2.626162f, 0.06719756f, 6.736708f));
            spawnpoints.Add("jailyard", new Vector3(-69f, 0.0f, 10f));
            spawnpoints.Add("jail", new Vector3(-49.10827f, 3.067196f, 14.40517f));
            spawnpoints.Add("f8", new Vector3(-69f, 0.0f, 10f));
            spawnpoints.Add("airdefenders", new Vector3(-125.599f, 0.506795f, 312.4564f));
            warpPointsUser = readWarpPointsFromFile();
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
            if (Input.GetKeyUp(KeyCodeF1) || Input.GetKeyUp(KeyCodeF2))
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
            bool pressF1 = Event.current.isKey && Event.current.keyCode == KeyCodeF1 && hasFocus;
            bool pressF2 = Event.current.isKey && Event.current.keyCode == KeyCodeF2 && hasFocus;

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

        public static string vector3ToJsonString(Vector3 point)
        {
            if (point == null)
            {
                point = new Vector3(0, 0, 0);
            }

            string x = point.x.ToString("G", culture);
            string y = point.y.ToString("G", culture);
            string z = point.z.ToString("G", culture);
            string jsonstring = "{\"x\":\"" + x + "\",\"y\":\"" + y + "\",\"z\":\"" + z + "\"}";
            return jsonstring;
        }
        public static string vector3ToString(Vector3 point)
        {
            if (point == null)
            {
                point = new Vector3(0, 0, 0);
            }

            string x = point.x.ToString("G", culture);
            string y = point.y.ToString("G", culture);
            string z = point.z.ToString("G", culture);
            string vstring = $"x: {x}, y: {y}, z: {z}";
            return vstring;
        }

        public static JSONObject addWarpPoint(JSONObject json, string name, Vector3 point)
        {
            if (json == null)
            {
                return null;
            }

            if (name == null || name.Length == 0)
            {
                return json;
            }

            if (spawnpointsNames.Contains(name) || predefinedUserWarpsNames.Contains(name))
            {
                return json;
            }

            if (point == null)
            {
                return json;
            }

            string jsonstring = vector3ToJsonString(point);
            try
            {
                json.AddField(name, new JSONObject(jsonstring));
                spawnpointsNames.Add(name);
                spawnpoints.Add(name, point);
                predefinedUserWarpsNames.Add(name);
            }
            catch (Exception ex)
            {
            }

            return json;
        }

        public static JSONObject removeWarpPoint(JSONObject json, string name)
        {
            if (json == null)
            {
                return null;
            }

            if (name == null || name.Length == 0)
            {
                return json;
            }

            if (!spawnpointsNames.Contains(name) || !predefinedUserWarpsNames.Contains(name))
            {
                return json;
            }

            try
            {
                json.RemoveField(name);
                spawnpointsNames.Remove(name);
                spawnpoints.Remove(name);
                predefinedUserWarpsNames.Remove(name);
            }
            catch (Exception ex)
            {
            }

            return json;
        }


        public static void readWarpPoints(JSONObject json)
        {
            if (json == null)
            {
                return;
            }

            for (int i = 0; i < json.list.Count; i++)
            {
                string key = json.keys[i];
                
                if (key == null || key.Length == 0)
                {
                    continue;
                }

                if (spawnpointsNames.Contains(key) || predefinedUserWarpsNames.Contains(key))
                {
                    continue;
                }

                JSONObject value = json.list[i];

                Vector3 point = new Vector3();

                for (int j = 0; j < value.list.Count; j++)
                {
                    string valuekey = value.keys[j];
                    string valuevalue = value.list[j].stringValue;

                    float v = 0;

                    try
                    {
                        v = float.Parse(valuevalue, culture);
                    } catch (Exception ex)
                    {
                        v = 0;
                    }

                    switch(valuekey)
                    {
                        case "x": point.x = v; break;
                        case "y": point.y = v; break;
                        case "z": point.z = v; break;
                    }
                }

                string pointstring = vector3ToJsonString(point);
                //Logger.LogInfo($"{key}: {pointstring}");
                spawnpointsNames.Add(key);
                predefinedUserWarpsNames.Add(key);
                spawnpoints.Add(key, point);
            }
        }

        public static void safeWarpPoints()
        {
            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            string filename = $"{objectsFolder}/warps.json";

            if (!File.Exists(filename))
            {
                try
                {
                    File.WriteAllText(filename, "{}");
                }
                catch (Exception e)
                {
                }
            }

            try
            {
                File.WriteAllText(filename, warpPointsUser.ToString());
            }
            catch (Exception e)
            {
            }
        }

        public static JSONObject readWarpPointsFromFile()
        {
            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            string filename = $"{objectsFolder}/warps.json";

            if (!File.Exists(filename))
            {
                try
                {
                    File.WriteAllText(filename, "{}");
                }
                catch (Exception e)
                {
                }
            }

            string all = "";

            try
            {

                string[] textFromFile = File.ReadAllLines(filename);

                foreach (string line in textFromFile)
                {
                    all += line;
                }

            }
            catch (Exception ex)
            {
            }

            JSONObject json = null;

            try
            {
                json = new JSONObject(all);
                readWarpPoints(json);
            }
            catch (Exception e)
            {
                json = new JSONObject();
            }

            return json;
        }

        public static void OpenUrl(string url)
        {
            try
            {
                // .NET Core / .NET 5+ safe way
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                // Fallback for older .NET or restricted environments
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    return;
                }
            }
        }
        public static string SaveGameObjectFile(GameObject rootObject, string name)
        {
            if (name == null || name.Length == 0)
            {
                return "";
            }

            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            string filename = $"{objectsFolder}/{name}.obj";

            if (File.Exists(filename))
            {
                Main.Instance.GameplayMenu.ShowNotification(filename + " exists!");
                return "";
            }

            try
            {
                SaveableBehaviour s = rootObject.GetComponent<SaveableBehaviour>();
                s.SaveToFile(filename);
                return filename;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return "";
            }
        }
        public static GameObject LoadGameObjectFile(string name)
        {
            if (name == null || name.Length == 0)
            {
                return null;
            }

            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            string filename = $"{objectsFolder}/{name}.obj";

            if (!File.Exists(filename))
            {
                Main.Instance.GameplayMenu.ShowNotification(filename + " not exists!");
                return null;
            }
            try
            {
                SaveableBehaviour s = new SaveableBehaviour();
                s.LoadFromFile(filename);
                return s.gameObject;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return null;
            }
        }
        public static GameObject getInteract()
        {
            try
            {
                if (Main.Instance.Player == null || Main.Instance.Player.WeaponInv == null || Main.Instance.Player.WeaponInv.IntLookingAt == null)
                {
                    return null;
                }

                Interactible la = Main.Instance.Player.WeaponInv.IntLookingAt;

                if (la != null)
                {
                    GameObject ga = la.gameObject;
                    return ga;
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }
        public static GameObject getPersonInteract()
        {
            GameObject ga = getInteract();

            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            if (la != null)
            {
                if (la is int_Person)
                {
                    int_Person int_thisPerson = (int_Person)la;
                    if (int_thisPerson.ThisPerson != null)
                    {
                        Person thisPerson = int_thisPerson.ThisPerson;
                        return thisPerson.gameObject;
                    }
                }
            }

            return null;
        }
        public static GameObject getHealthPodInteract()
        {
            GameObject ga = getInteract();

            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            if (la != null)
            {
                if (la is int_HealthPod)
                {
                    int_HealthPod int_P = (int_HealthPod)la;
                    return int_P.gameObject;
                }
            }

            return null;
        }
        public static GameObject getLockableInteract()
        {
            GameObject ga = getInteract();

            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            if (la != null)
            {
                if (la is int_Lockable)
                {
                    int_Lockable int_Lo = (int_Lockable)la;
                    return int_Lo.gameObject;
                }
            }

            return null;
        }
        public static GameObject getStorableInteract()
        {
            GameObject ga = getInteract();

            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            if (la is Int_Storage)
            {
                Int_Storage int_St = (Int_Storage)la;
                return int_St.gameObject;
            }

            return null;
        }

        public static GameObject CreatePersonNew(string name, bool save = true, bool spawnFemale = true)
        {
            bool LoadSpecificNPC = true;
            Person PersonGenerated = null;
            if (LoadSpecificNPC)
            {
                PersonGenerated = spawnFemale ? UnityEngine.Object.Instantiate<GameObject>(Main.Instance.PersonPrefab).GetComponent<Person>() : UnityEngine.Object.Instantiate<GameObject>(Main.Instance.PersonGuyPrefab).GetComponent<Person>();
                string femalesDir = $"{Main.AssetsFolder}/wolfitdm/females";
                string malesDir = $"{Main.AssetsFolder}/wolfitdm/males";
                string objectsDir = $"{Main.AssetsFolder}/wolfitdm/objects";
                Directory.CreateDirectory(femalesDir);
                Directory.CreateDirectory(malesDir);
                Directory.CreateDirectory(objectsDir);
                string maleOrFemale = spawnFemale ? "females" : "males";
                string filename = $"{Main.AssetsFolder}/wolfitdm/{maleOrFemale}/{name}.png";
                if (!File.Exists(filename))
                {
                    Main.Instance.GameplayMenu.ShowNotification(filename + " not exists!");
                    return null;
                }
                PersonGenerated._DontLoadClothing = true;
                PersonGenerated._DontLoadInteraction = true;
                PersonGenerated.LoadFromFile(filename);
                PersonGenerated.transform.position = Main.Instance.Player.transform.position;
                PersonGenerated.transform.rotation = Main.Instance.Player.transform.rotation;
            }
            PersonGenerated.WorldSaveID = Main.GenerateRandomString(25);
            PersonGenerated.DontSaveInMain = !save;
            PersonGenerated.CanSaveFlagger = new List<string>();
            PersonGenerated.JobIndex = 0;
            PersonGenerated.SPAWN_noUglyHair = false;
            PersonGenerated.SPAWN_onlyGoodHair = true;
            PersonGenerated.State = Person_State.Free;
            PersonGenerated.Home = Main.Instance.PossibleStreetHomes[UnityEngine.Random.Range(0, Main.Instance.PossibleStreetHomes.Count)];
            //PersonGenerated.CurrentZone = null;
            PersonGenerated.StartingClothes = new List<GameObject>();
            PersonGenerated.StartingWeapons = new List<GameObject>();
            PersonGenerated._StartingClothes = new List<string>();
            PersonGenerated._StartingWeapons = new List<string>();
            PersonGenerated.Inited = false;
            PersonGenerated.PersonType = Main.Instance.PersonTypes[(int)Person_Type.Wild];
            RandomNPCHere inst = new RandomNPCHere();
            inst.SpawnClean = true;
            PersonGenerated.PutFeet();
            PersonGenerated.PersonType.ApplyTo(PersonGenerated, false, false, false, inst);
            if (PersonGenerated.Name == null || PersonGenerated.Name.Length == 0)
            {
                PersonGenerated.Name = Main.Instance.GenerateRandomName();
            }
            if (PersonGenerated.States == null || PersonGenerated.States.Length < 34)
            {
                PersonGenerated.States = new bool[34];
            }
            PersonGenerated.RefreshColors();
            setReverseWildStates(PersonGenerated.gameObject);
            PersonGenerated.TheHealth.canDie = false;
            setPersonaltyToNympho(PersonGenerated.gameObject);
            return PersonGenerated.gameObject;
        }
        public static GameObject CreatePersonMaleOld()
        {
            Person s = Person.GenerateRandom(spawnedPerson: null, female: true, randomgender: false, randompartsizes: true, _DEBUG: false);
            s.transform.position = Main.Instance.Player.transform.position;
            s.transform.rotation = Main.Instance.Player.transform.rotation;
            return s.gameObject;
        }

        public static GameObject CreatePersonFemaleOld()
        {
            Person s = Person.GenerateRandom(spawnedPerson: null, female: true, randomgender: false, randompartsizes: true, _DEBUG: false);
            s.transform.position = Main.Instance.Player.transform.position;
            s.transform.rotation = Main.Instance.Player.transform.rotation;
            return s.gameObject;
        }

        private static List<GameObject> getPrefabsByName2(string prefab)
        {
            List<GameObject> Prefabs2 = new List<GameObject>();

            try
            {
                List<Weapon> Prefabs = null;

                if (prefab == null)
                {
                    Prefabs = Main.Instance.Prefabs_Weapons;
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

                if (Prefabs == null)
                {
                    return Prefabs2;
                }

                int length = Prefabs.Count;
                for (int i = 0; i < length; i++)
                {
                    Prefabs2.Add(Prefabs[i].gameObject);
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }

            return Prefabs2;
        }
        private static GameObject getWeaponByName(string prefab, string name)
        {
            List<GameObject> Prefabs = getPrefabsByName2(prefab);

            if (Prefabs == null || Prefabs.Count == 0)
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
            List<GameObject> Prefabs = getPrefabsByName2(prefab);

            if (Prefabs == null || Prefabs.Count == 0)
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
                GameObject item = Prefabs[i];
                string name = item.name.Replace(" ", "_").ToLower();
                all.Add(name);
            }

            return all;
        }
        private static void showWeaponsInLogByPrefab()
        {
            string prefabName = "Weapons";

            List<string> Prefabs = getAllWeaponsByPrefab(null);

            if (Prefabs == null || Prefabs.Count == 0)
            {
                return;
            }

            string itemi = "-------------------------------------------------------------------" + prefabName + "-------------------------------------------------------------------";
            Logger.LogInfo(itemi);

            int length = Prefabs.Count;
            for (int i = 0; i < length; i++)
            {
                string item = Prefabs[i];
                Logger.LogInfo(item);
            }
        }

        public static void setNudeStates(GameObject personGa, bool realnude = false, bool setallnudestates = false)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();
            person.ClothingCondition = e_ClothingCondition.Nude;
            person.States[9] = true; // Nude Clothing Vipe
            person.States[10] = false; // Casual Clothing Vipe
            person.States[11] = false; // Sexy Clothing Vipe

            if (realnude)
            {
                person.States[0] = false; // Dirty -10 Sexy
                person.States[1] = false; // Horny +10 Sexy
                person.States[2] = false; // Very Dirty -20 Sexy
                person.States[3] = false; // Shitten -20 Sexy

                if (setallnudestates)
                {
                    person.States[4] = false; // Sleepy - speed
                    person.States[5] = false; // Needs toilet
                    person.States[6] = false; // Hungry
                    person.States[7] = false; // Pregnant
                }

                person.States[8] = false; // Bloody -10 Sexy
                person.States[12] = false; // Cum Stains + 1 Sexy 
                person.States[13] = false; // Cum Stains + 2 Sexy
                person.States[14] = false; // Cum Stains + 3 Sexy
                person.States[15] = false; // Cum Stains + 4 Sexy 
                person.States[16] = false; // Cum Stains + 5 Sexy 
                person.States[17] = false; // Body Writting + 1 Sexy 
                person.States[18] = false; // Body Writting + 2 Sexy 
                person.States[19] = false; // Body Writting + 3 Sexy 
                person.States[20] = false; // Bruises - 10 Sexy 
                person.States[21] = false; // Heavy Bruises - 20 Sexy 
                person.States[22] = false; // Basic Makeup + 10 Sexy
                person.States[23] = false; // Runny Makeup + 1 Sexy
                person.States[24] = false; // Runny Makeup + 1 Sexy
                person.States[25] = false; // Runny Makeup + 1 Sexy
                person.States[26] = false; // Cum in mouth + 1 Sexy

                if (setallnudestates)
                {
                    person.States[27] = false; // Beard
                    person.States[28] = false; // Lipstick
                    person.States[29] = false; // Lipstick
                    person.States[30] = false; // Lipstick
                    person.States[31] = false; // Skin color lips
                    person.States[32] = false; // Freckets
                    person.States[33] = false; // Dirty Mouth
                }
            }
        }

        public static void setNudeClothesPoints(GameObject personGa)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            if (person.EquippedClothes == null)
            {
                return;
            }

            for (int i = 0; i < person.EquippedClothes.Count; i++)
            {
                person.EquippedClothes[i].SexyPoints = 0;
                person.EquippedClothes[i].CasualPoints = 0;
            }

            person.GetClothingCondition();
        }
        public static void setSexyStates(GameObject personGa, bool realsexy, bool setallsexystates = false)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            person.ClothingCondition = e_ClothingCondition.Sexy;
            person.States[9] = false; // Nude Clothing Vipe
            person.States[10] = false; // Casual Clothing Vipe
            person.States[11] = true; // Sexy Clothing Vipe

            if (realsexy)
            {
                person.States[0] = false; // Dirty -10 Sexy
                person.States[1] = true; // Horny +10 Sexy
                person.States[2] = false; // Very Dirty -20 Sexy
                person.States[3] = false; // Shitten -20 Sexy

                if (setallsexystates)
                {
                    person.States[4] = false; // Sleepy - speed
                    person.States[5] = false; // Needs toilet
                    person.States[6] = false; // Hungry
                    person.States[7] = false; // Pregnant
                }

                person.States[8] = false; // Bloody -10 Sexy
                person.States[12] = true; // Cum Stains + 1 Sexy 
                person.States[13] = true; // Cum Stains + 2 Sexy
                person.States[14] = true; // Cum Stains + 3 Sexy
                person.States[15] = true; // Cum Stains + 4 Sexy 
                person.States[16] = true; // Cum Stains + 5 Sexy 
                person.States[17] = true; // Body Writting + 1 Sexy 
                person.States[18] = true; // Body Writting + 2 Sexy 
                person.States[19] = true; // Body Writting + 3 Sexy 
                person.States[20] = false; // Bruises - 10 Sexy 
                person.States[21] = false; // Heavy Bruises - 20 Sexy 
                person.States[22] = true; // Basic Makeup + 10 Sexy
                person.States[23] = true; // Runny Makeup + 1 Sexy
                person.States[24] = true; // Runny Makeup + 1 Sexy
                person.States[25] = true; // Runny Makeup + 1 Sexy
                person.States[26] = true; // Cum in mouth + 1 Sexy

                if (setallsexystates)
                {
                    person.States[27] = false; // Beard
                    person.States[28] = false; // Lipstick
                    person.States[29] = false; // Lipstick
                    person.States[30] = false; // Lipstick
                    person.States[31] = false; // Skin color lips
                    person.States[32] = false; // Freckets
                    person.States[33] = false; // Dirty Mouth
                }
            }
        }

        public static void setSexyClothesPoints(GameObject personGa)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            if (person.EquippedClothes == null)
            {
                return;
            }

            int add = 100;

            for (int i = 0; i < person.EquippedClothes.Count; i++)
            {
                person.EquippedClothes[i].CasualPoints = 0;
                person.EquippedClothes[i].SexyPoints = add;
                person.EquippedClothes[i].SexyPoints = person.EquippedClothes[i].CasualPoints + person.EquippedClothes[i].SexyPoints + add;
            }

            person.GetClothingCondition();
        }

        public static void setCasualStates(GameObject personGa, bool realcasual = false, bool setallcasualstates = false)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            person.ClothingCondition = e_ClothingCondition.Casual;
            person.States[9] = false; // Nude Clothing Vipe
            person.States[10] = true; // Casual Clothing Vipe
            person.States[11] = false; // Sexy Clothing Vipe

            if (realcasual)
            {
                person.States[0] = false; // Dirty -10 Sexy
                person.States[1] = false; // Horny +10 Sexy
                person.States[2] = false; // Very Dirty -20 Sexy
                person.States[3] = false; // Shitten -20 Sexy

                if (setallcasualstates)
                {
                    person.States[4] = false; // Sleepy - speed
                    person.States[5] = false; // Needs toilet
                    person.States[6] = false; // Hungry
                    person.States[7] = false; // Pregnant
                }

                person.States[8] = false; // Bloody -10 Sexy
                person.States[12] = false; // Cum Stains + 1 Sexy 
                person.States[13] = false; // Cum Stains + 2 Sexy
                person.States[14] = false; // Cum Stains + 3 Sexy
                person.States[15] = false; // Cum Stains + 4 Sexy 
                person.States[16] = false; // Cum Stains + 5 Sexy 
                person.States[17] = false; // Body Writting + 1 Sexy 
                person.States[18] = false; // Body Writting + 2 Sexy 
                person.States[19] = false; // Body Writting + 3 Sexy 
                person.States[20] = false; // Bruises - 10 Sexy 
                person.States[21] = false; // Heavy Bruises - 20 Sexy 
                person.States[22] = false; // Basic Makeup + 10 Sexy
                person.States[23] = false; // Runny Makeup + 1 Sexy
                person.States[24] = false; // Runny Makeup + 1 Sexy
                person.States[25] = false; // Runny Makeup + 1 Sexy
                person.States[26] = false; // Cum in mouth + 1 Sexy

                if (setallcasualstates)
                {
                    person.States[27] = false; // Beard
                    person.States[28] = false; // Lipstick
                    person.States[29] = false; // Lipstick
                    person.States[30] = false; // Lipstick
                    person.States[31] = false; // Skin color lips
                    person.States[32] = false; // Freckets
                    person.States[33] = false; // Dirty Mouth
                }
            }
        }
        public static void setCasualClothesPoints(GameObject personGa)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            if (person.EquippedClothes == null)
            {
                return;
            }

            int add = 100;

            for (int i = 0; i < person.EquippedClothes.Count; i++)
            {
                person.EquippedClothes[i].SexyPoints = 0;
                person.EquippedClothes[i].CasualPoints = add;
                person.EquippedClothes[i].CasualPoints = person.EquippedClothes[i].CasualPoints + person.EquippedClothes[i].SexyPoints + add;
            }

            person.GetClothingCondition();
        }

        public static void setCleanSkinStates(GameObject _thisGa)
        {
            if (_thisGa == null)
            {
                return;
            }

            Person _this = _thisGa.GetComponent<Person>();

            if (_this == null)
            {
                return;
            }

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

        public static void setPersonaltyToNympho(GameObject personGa)
        {
            if (personGa == null)
            {
                return;
            }

            Person PersonGenerated = personGa.GetComponent<Person>();

            if (PersonGenerated == null)
            {
                return;
            }

            PersonGenerated.Personality = Personality_Type.Nympho;
            if (PersonGenerated.Fetishes == null)
            {
                PersonGenerated.Fetishes = new List<e_Fetish>();
            }
            PersonGenerated.Fetishes.Clear();
            PersonGenerated.Fetishes.Add(e_Fetish.Dildo);
            PersonGenerated.Fetishes.Add(e_Fetish.Pregnant);
            PersonGenerated.Fetishes.Add(e_Fetish.Anal);
            PersonGenerated.Fetishes.Add(e_Fetish.Scat);
            PersonGenerated.Fetishes.Add(e_Fetish.Masochist);
            PersonGenerated.Fetishes.Add(e_Fetish.Clean);
            PersonGenerated.Fetishes.Add(e_Fetish.Futa);
            PersonGenerated.Fetishes.Add(e_Fetish.Machine);
            PersonGenerated.Fetishes.Add(e_Fetish.Sadist);
            PersonGenerated.Fetishes.Add(e_Fetish.Oral);
            PersonGenerated.Fetishes.Add(e_Fetish.Outdoors);
        }
        public static void setReverseWildStates(GameObject personGa)
        {
            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            person.States[17] = false;
            person.States[18] = false;
            person.States[19] = false;
            person.States[12] = false;
            person.States[13] = false;
            person.States[14] = false;
            person.States[15] = false;
            person.States[16 /*0x10*/] = false;
            person.States[20] = false;
            person.DirtySkin = false;
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

            if (Prefabs == null || Prefabs.Count == 0)
            {
                return;
            }

            string itemi = "-------------------------------------------------------------------" + prefabName + "-------------------------------------------------------------------";
            Logger.LogInfo(itemi);

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
                Logger.LogInfo(item);

            }
        }
        private static List<GameObject> getPrefabsByName(string prefab)
        {
            List<GameObject> Prefabs = null;

            try
            {
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
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                Prefabs = new List<GameObject>();
            }

            return Prefabs;
        }
        private static GameObject getItemByName(string prefab, string name)
        {
            List<GameObject> Prefabs = getPrefabsByName(prefab);

            if (Prefabs == null || Prefabs.Count == 0)
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

            if (name == null)
            {
                return null;
            }

            for (int i = 0; i < itemPCount; i++)
            {
                item = getItemByName(itemsP[i], name);
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }
        private static List<GameObject> getAllItems()
        {
            List<GameObject> itemList = new List<GameObject>();

            for (int i = 0; i < itemPCount; i++)
            {
                string prefab = itemsP[i];
                List<GameObject> items = getPrefabsByName(prefab);
                if (items == null || items.Count == 0)
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
                value = value <= 0 ? 1 : value;

                int count = Main.Instance.Player.CurrentBackpack.ThisStorage.StorageItems.Count;

                count += value + 10;

                count = count <= 0 ? int.MaxValue : count;

                int storagemax = Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax;

                if (count >= storagemax)
                {
                    storagemax = count;
                }

                Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = storagemax;

                for (int i = 0; i < value; i++)
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.AddItem(item);
                }
            }
        }
        public static void patreon()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: patreon");
            OpenUrl("https://www.patreon.com/breakfast5");
        }
        public static void kofi()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: kofi");
            OpenUrl("https://www.patreon.com/breakfast5");
        }
        public static void subscribestar()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: subscribestar");
            OpenUrl("https://subscribestar.adult/breakfast5");
        }
        public static void discord()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: discord");
            OpenUrl("https://discord.gg/QjjXAyfwEU");
        }
        public static void getmodslink()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: getmodslink");
            OpenUrl("https://discord.gg/S7hcQvYzmJ");
        }
        public static void getmoremods()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: getmoremods");
            OpenUrl("https://discord.com/channels/1021414197558513664/1228264001893437491");
        }
        public static void supportme()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: supportme");
            patreon();
            kofi();
            subscribestar();
            discord();
            getmodslink();
            getmoremods();
        }
        public static void saveobject(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: saveobject");
            if (copyObj != null)
            {
                string filename = SaveGameObjectFile(copyObj, value);
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"saveobject: loaded or copied object '{copyObj.name}' saved to '{filename}' ");
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static void saveobject2(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: saveobject2");
            if (copyObj2 != null)
            {
                string filename = SaveGameObjectFile(copyObj2, value);
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"saveobject2: loaded or copied object '{copyObj2.name}' saved to '{filename}' ");
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void loadobject(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: loadobject");
            GameObject obj = LoadGameObjectFile(value);

            if (obj != null)
            {
                copyObj = obj;
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"loadobject: object '{copyObj.name}' loaded, now you can use the command 'paste'");
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static void loadobject2(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: loadobject2");
            GameObject obj = LoadGameObjectFile(value);

            if (obj != null)
            {
                copyObj2 = obj;
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"loadobject2: object '{copyObj2.name}' loaded, now you can use the command 'paste2'");
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void iamanympho()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: iamanympho");
            try
            {
                if (Main.Instance.Player == null)
                {
                    return;
                }

                setPersonaltyToNympho(Main.Instance.Player.gameObject);
                Main.Instance.GameplayMenu.ShowNotification("set the player to a nympho!");
            }
            catch (Exception e)
            {
            }
        }
        public static void nympho()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: nympho");
            GameObject personInteract = getPersonInteract();
            
            if (personInteract == null)
            {
                return;
            }

            Person thisPerson = personInteract.GetComponent<Person>();
            setPersonaltyToNympho(thisPerson.gameObject);
            Main.Instance.GameplayMenu.ShowNotification("Set person the you looked at to nympho!");
        }

        public static GameObject copyObj = null;
        public static GameObject copyObj2 = null;

        public static void copy()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: copy");
            try
            {
                RaycastHit hitInfo;
                WeaponSystem _this = Main.Instance.Player.WeaponInv;
                if (Physics.Raycast(_this.transform.position, _this.transform.TransformDirection(Vector3.forward), out hitInfo, _this.RayDistance, (int)_this.PromptLayers))
                {
                    Component obj = hitInfo.transform.GetComponent<Component>();
                    Component obj2 = hitInfo.transform.root.GetComponent<Component>();
                    
                    if (obj != null)
                    {
                        copyObj = obj.gameObject;

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj.name}' copied, now you can use the command 'paste', to spawn the object!");
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj.name}' copied, now you can use the command 'saveobject name', to save the object to file!");
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    if (obj2 != null)
                    {
                        copyObj2 = obj2.gameObject;

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj2.name}' copied, now you can use the command 'paste2', to spawn the object!");
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj2.name}' copied, now you can use the command 'saveobject2 name', to save the object to file!");
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            } 
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public static void paste()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: paste");
            if (copyObj != null)
            {

                GameObject newElement = Main.Spawn(copyObj);
                Person player = Main.Instance.Player;
                newElement.transform.position = player.transform.position;
                newElement.transform.rotation = player.transform.rotation;
                newElement.transform.parent = player.transform.parent;
                newElement.SetActive(true);
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"paste: object '{newElement.name}' pastied!");
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static void paste2()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: paste2");
            if (copyObj2 != null)
            {

                GameObject newElement = Main.Spawn(copyObj2);
                Person player = Main.Instance.Player;
                newElement.transform.position = player.transform.position;
                newElement.transform.rotation = player.transform.rotation;
                newElement.transform.parent = player.transform.parent;
                newElement.SetActive(true);
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"paste2: object '{newElement.name}' pastied!");
                }
                catch (Exception ex)
                {
                }
            }
        }
        public static void help()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: help");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/README.md");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/commandslist.txt");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/commandslist.md");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/itemlist.txt");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/statelist.txt");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/warplist.txt");
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/weaponlist.txt");
            OpenUrl("https://github.com/wolfitdm/BitchlandCheatConsoleBepInEx/releases/tag/v1.0.0");
            Main.Instance.GameplayMenu.ShowNotification("executed command: type all commands and warps without the '* '. it is only for github, to use markdown and list items!");
        }

        public static void spawnbirthintopod()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: spawnbirthintopod");

            if (!(Main.Instance.Player is Girl))
            {
                Main.Instance.GameplayMenu.ShowNotification("you are a male!");
                return;
            }

            GameObject podAvailableFree = getHealthPodInteract();

            if (podAvailableFree == null)
            {
                Main.Instance.GameplayMenu.ShowNotification("You must look at a HealthPod, in the clinic you can found a healthpod, or in the lab you can also found a healthpod");
                return;
            }

            int_HealthPod podAvailable = podAvailableFree.GetComponent<int_HealthPod>();

            Person parent1 = Main.Instance.Player;
            Person parent2 = (Main.Instance.Player as Girl).PregnancyParent;
            Person offspring = Main.Instance.CreateOffspring(parent1, parent2);
            podAvailable.PodUseType = 2;
            podAvailable.Interact(offspring);
            offspring.IsPlayerDescendant = true;
            offspring.CantBeForced = true;
        }
        public static void spawnmale(string value, bool save = false)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: spawnmale");
            Person s = null;
            switch (value)
            {
                default:
                    {
                        s = CreatePersonNew(value, save, false).GetComponent<Person>();
                    }
                    break;
            }
        }

        public static void spawnmalenude(string value, bool save = false)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: spawnmalenude");
            Person s = CreatePersonNew(value, save, false).GetComponent<Person>();
        }
        public static void spawnfemale(string value, bool save = false)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: spawnfemale");
            Person s = null;
            switch (value)
            {
                case "jeanne":
                    {
                        s = CreatePersonNew("jeanne", save).GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[184]);
                            s.DressClothe(Main.Instance.AllPrefabs[43]);
                            s.DressClothe(Main.Instance.AllPrefabs[38]);
                            s.DressClothe(Main.Instance.AllPrefabs[40]);
                            s.DressClothe(Main.Instance.AllPrefabs[36]);
                        }
                    }
                    break;

                case "sarahoffwork":
                    {
                        s = CreatePersonNew("sarahoffwork", save).GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[3]);
                            s.DressClothe(Main.Instance.AllPrefabs[14]);
                        }
                    }
                    break;

                case "uniformedsarah":
                    {
                        s = CreatePersonNew("uniformedsarah", save).GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[3]);
                            s.DressClothe(Main.Instance.AllPrefabs[14]);
                            s.DressClothe(Main.Instance.Prefabs_ProstSuit2[1]);
                            s.DressClothe(Main.Instance.Prefabs_ProstSuit2[2]);
                            s.DressClothe(Main.Instance.Prefabs_ProstSuit2[3]);
                            s.DressClothe(Main.Instance.Prefabs_ProstSuit2[4]);
                            s.DressClothe(Main.Instance.Prefabs_Hair[63]);
                        }
                    }
                    break;

                case "nameless":
                    {
                        s = CreatePersonNew("nameless", save).GetComponent<Person>();
                        if (s != null)
                        {
                            GameObject[] uniform = new GameObject[6];
                            uniform[0] = Main.Spawn(Main.Instance.AllPrefabs[36], Main.Instance.DisabledObjects);
                            uniform[1] = Main.Spawn(Main.Instance.AllPrefabs[144], Main.Instance.DisabledObjects);
                            uniform[2] = Main.Spawn(Main.Instance.AllPrefabs[161], Main.Instance.DisabledObjects);
                            uniform[3] = Main.Spawn(Main.Instance.AllPrefabs[165], Main.Instance.DisabledObjects);
                            uniform[4] = Main.Spawn(Main.Instance.AllPrefabs[197], Main.Instance.DisabledObjects);
                            uniform[5] = Main.Spawn(Main.Instance.AllPrefabs[198], Main.Instance.DisabledObjects);
                            int_PickableClothingPackage componentInChildren = uniform[3].GetComponentInChildren<int_PickableClothingPackage>(true);
                            if (componentInChildren != null)
                            {
                                componentInChildren.ClothingData = ":RGBA(0.000, 0.000, 0.000, 1.000):RGBA(0.000, 0.000, 0.000, 0.000)";
                            }
                            s.ChangeUniform(uniform);
                        }
                    }
                    break;

                case "rit":
                    {
                        s = CreatePersonNew("rit", save).GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[184]);
                            s.DressClothe(Main.Instance.AllPrefabs[193]);
                            s.DressClothe(Main.Instance.AllPrefabs[38]);
                            s.DressClothe(Main.Instance.AllPrefabs[40]);
                        }
                    }
                    break;

                case "carol":
                    {
                        s = CreatePersonNew("carol", save).GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[7]);
                            s.DressClothe(Main.Instance.AllPrefabs[31]);
                            s.DressClothe(Main.Instance.AllPrefabs[3]);
                            s.DressClothe(Main.Instance.AllPrefabs[55]);
                            s.DressClothe(Main.Instance.AllPrefabs[4]);
                        }
                    }
                    break;

                case "beth":
                    {
                        s = CreatePersonNew("beth", save).GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[197]);
                            s.DressClothe(Main.Instance.AllPrefabs[198]);
                            s.DressClothe(Main.Instance.AllPrefabs[33]);
                            s.DressClothe(Main.Instance.AllPrefabs[174]);
                            s.DressClothe(Main.Instance.AllPrefabs[177]);
                        }
                    }
                    break;

                default:
                    {
                        s = CreatePersonNew(value, save).GetComponent<Person>();
                    }
                    break;
            }
        }
        public static void spawnfemalenude(string value, bool save = false)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: spawnfemalenude");
            Person s = CreatePersonNew(value, save).GetComponent<Person>();
        }
        public static void completeallquests()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: completallquests");
            int allMissionsCount = Main.Instance.AllMissions.Count;
            for (int index1 = 0; index1 < allMissionsCount; ++index1)
            {
                if (!Main.Instance.AllMissions[index1].CurrentGoal.isNull())
                {
                    Main.Instance.AllMissions[index1].CurrentGoal.Completed = true;
                    Main.Instance.AllMissions[index1].CurrentGoal.Failed = false;
                    string completeQuestString = "bl_CompleteAllQuestsModDoWork.doWork() Complete quest: " + Main.Instance.AllMissions[index1].CurrentGoal.ToString();
                    Main.Instance.GameplayMenu.ShowNotification(completeQuestString);
                    Logger.LogInfo(completeQuestString);
                }

                int goalsCount = Main.Instance.AllMissions[index1].Goals.Count;
                for (int index2 = 0; index2 < goalsCount; ++index2)
                {
                    if (!Main.Instance.AllMissions[index1].Goals[index2].isNull())
                    {
                        Main.Instance.AllMissions[index1].Goals[index2].Completed = true;
                        Main.Instance.AllMissions[index1].Goals[index2].Failed = false;
                        string completeQuestString = "bl_CompleteAllQuestsModDoWork.doWork() Complete quest: " + Main.Instance.AllMissions[index1].Goals[index2].ToString();
                        Main.Instance.GameplayMenu.ShowNotification(completeQuestString);
                        Logger.LogInfo(completeQuestString);
                    }
                }
            }
            allMissionsCount = Main.Instance.GameplayMenu.CurrentMissions.Count;
            for (int index3 = 0; index3 < allMissionsCount; ++index3)
            {
                if (!Main.Instance.GameplayMenu.CurrentMissions[index3].CurrentGoal.isNull())
                {
                    Main.Instance.GameplayMenu.CurrentMissions[index3].CurrentGoal.Completed = true;
                    Main.Instance.GameplayMenu.CurrentMissions[index3].CurrentGoal.Failed = false;
                    string completeQuestString = "bl_CompleteAllQuestsModDoWork.doWork() Complete quest: " + Main.Instance.GameplayMenu.CurrentMissions[index3].CurrentGoal.ToString();
                    Main.Instance.GameplayMenu.ShowNotification(completeQuestString);
                    Logger.LogInfo(completeQuestString);
                }
                int goalsCount = Main.Instance.GameplayMenu.CurrentMissions[index3].Goals.Count;
                for (int index4 = 0; index4 < goalsCount; ++index4)
                {
                    if (!Main.Instance.GameplayMenu.CurrentMissions[index3].Goals[index4].isNull())
                    {
                        Main.Instance.GameplayMenu.CurrentMissions[index3].Goals[index4].Completed = true;
                        Main.Instance.GameplayMenu.CurrentMissions[index3].Goals[index4].Failed = false;
                        string completeQuestString = "bl_CompleteAllQuestsModDoWork.doWork() Complete quest: " + Main.Instance.GameplayMenu.CurrentMissions[index3].Goals[index4].ToString();
                        Main.Instance.GameplayMenu.ShowNotification(completeQuestString);
                        Logger.LogInfo(completeQuestString);
                    }
                }
            }
            if (!Main.Instance.GameplayMenu.CurrentMission.IsNull())
            {
                if (!Main.Instance.GameplayMenu.CurrentMission.CurrentGoal.isNull())
                {
                    Main.Instance.GameplayMenu.CurrentMission.CurrentGoal.Completed = true;
                    Main.Instance.GameplayMenu.CurrentMission.CurrentGoal.Failed = false;
                    string completeQuestString = "bl_CompleteAllQuestsModDoWork.doWork() Complete quest: " + Main.Instance.GameplayMenu.CurrentMission.CurrentGoal.ToString();
                    Main.Instance.GameplayMenu.ShowNotification(completeQuestString);
                    Logger.LogInfo(completeQuestString);
                }
                int goalsCount = Main.Instance.GameplayMenu.CurrentMission.Goals.Count;
                for (int index = 0; index < goalsCount; ++index)
                {
                    if (!Main.Instance.GameplayMenu.CurrentMission.Goals[index].isNull())
                    {
                        Main.Instance.GameplayMenu.CurrentMission.Goals[index].Completed = true;
                        Main.Instance.GameplayMenu.CurrentMission.Goals[index].Failed = false;
                        string completeQuestString = "bl_CompleteAllQuestsModDoWork.doWork() Complete quest: " + Main.Instance.GameplayMenu.CurrentMission.Goals[index].ToString();
                        Main.Instance.GameplayMenu.ShowNotification(completeQuestString);
                        Logger.LogInfo(completeQuestString);
                    }
                }
            }
            string completeQuestString2 = "bl_CompleteAllQuestsModDoWork.doWork() Complete All Quests";
            Main.Instance.GameplayMenu.ShowNotification(completeQuestString2);
            Logger.LogInfo(completeQuestString2);
        }
        public static void getpersonstate()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: getpersonstate");

            GameObject thisPersonFree = getPersonInteract();

            if (thisPersonFree == null)
            {
                return;
            }
           
            Person thisPerson = thisPersonFree.GetComponent<Person>();
                        
            if (thisPerson.State == Person_State.Free)
            {
                Main.Instance.GameplayMenu.ShowNotification("getpersonstate: person state is free");
                Main.Instance.GameplayMenu.ShowNotification("getpersonstate: that mean, npc is fuckable great!, yeah :)");
            }
            else if (thisPerson.State == Person_State.Work)
            {
                Main.Instance.GameplayMenu.ShowNotification("getpersonstate: person state is work");
                Main.Instance.GameplayMenu.ShowNotification("getpersonstate: that mean, npc is not fuckable, soo bad ): ");
            }
                   
        }
        public static void togglepersonstate()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: togglepersonstate");

            GameObject thisPersonFree = getPersonInteract();

            if (thisPersonFree == null)
            {
                return;
            }

            Person thisPerson = thisPersonFree.GetComponent<Person>();

            if (thisPerson.State == Person_State.Free)
            {
                thisPerson.State = Person_State.Work;
                Main.Instance.GameplayMenu.ShowNotification("togglepersonstate: person state was free, now person state is work");
                Main.Instance.GameplayMenu.ShowNotification("togglepersonstate: that mean, npc is not fuckable now, soo bad ): ");
            }
            else if (thisPerson.State == Person_State.Work)
            {
                 thisPerson.State = Person_State.Free;
                 Main.Instance.GameplayMenu.ShowNotification("togglepersonstate: person state was work, now person state is free");
                 Main.Instance.GameplayMenu.ShowNotification("togglepersonstate: that mean, npc is fuckable now great!, yeah :)");
            }
        }
        public static void setpersonstatetowork()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setpersonstatetowork");

            GameObject thisPersonFree = getPersonInteract();

            if (thisPersonFree == null)
            {
                return;
            }

            Person thisPerson = thisPersonFree.GetComponent<Person>();

            if (thisPerson.State == Person_State.Free)
            {
                thisPerson.State = Person_State.Work;
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetowork: person state was free, now person state is work");
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetowork: that mean, npc is not fuckable, soo bad ): ");
            }
            else if (thisPerson.State == Person_State.Work)
            {
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetowork: person state is already work");
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetowork: that mean, npc is not fuckable, soo bad ): ");
            }
        }
        public static void setpersonstatetofree()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setpersonstatetofree");

            GameObject thisPersonFree = getPersonInteract();

            if (thisPersonFree == null)
            {
                return;
            }

            Person thisPerson = thisPersonFree.GetComponent<Person>();

            if (thisPerson.State == Person_State.Free)
            {
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetofree: person state is already free");
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetofree: that mean, npc is fuckable great!, yeah :)");
            }
            else if (thisPerson.State == Person_State.Work)
            {
                thisPerson.State = Person_State.Free;
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetofree: person state was work, now person state is free");
                Main.Instance.GameplayMenu.ShowNotification("setpersonstatetofree: that mean, npc is fuckable great!, yeah :)");
            }
        }
        public static void lock_int()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: lock");

            GameObject lockable = getLockableInteract();

            if (lockable == null)
            {
                return;
            }

            int_Lockable int_Lo = lockable.GetComponent<int_Lockable>();

            int_Lo.m_Locked = true;
            int_Lo.InteractIcon = 1;
            
            if (int_Lo.InteractText != null)
            {
                int_Lo.InteractText = int_Lo.InteractText.Replace("(Unlocked", "(Locked");
            }
        }
        public static void unlock_int()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: unlock");
            GameObject lockable = getLockableInteract();

            if (lockable == null)
            {
                return;
            }

            int_Lockable int_Lo = lockable.GetComponent<int_Lockable>();

            int_Lo.m_Locked = false;
            int_Lo.InteractIcon = int_Lo.DefaultInteractIcon;

            if (int_Lo.InteractText != null)
            {
                int_Lo.InteractText = int_Lo.InteractText.Replace("(Locked", "(Unlocked");
            }
        }

        public static void storagemax_int()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: storagemax");
            GameObject storable = getStorableInteract();

            if (storable == null)
            {
                return;
            }

            Int_Storage int_storage = storable.GetComponent<Int_Storage>();

            int_storage.StorageMax = int.MaxValue;
        }

        public static void storagemax_int_backpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: storagemaxbackpack");
            try
            {
                if (Main.Instance.Player == null || Main.Instance.Player.CurrentBackpack == null || Main.Instance.Player.CurrentBackpack.ThisStorage == null)
                {
                    return;
                }

                Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
            }
            catch (Exception e)
            {
            }
        }

        public static void warpfollower()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: warpfollower");
            if (Main.Instance.PeopleFollowingPlayer.Count > 0)
            {
                for (int i = 0; i < Main.Instance.PeopleFollowingPlayer.Count; i++)
                {
                    if (Main.Instance.Player.transform != null && Main.Instance.PeopleFollowingPlayer[i].transform != null)
                    {
                        Main.Instance.PeopleFollowingPlayer[i].transform.position = Main.Instance.Player.transform.position;
                    }
                }
            }
        }
        public static void unstuckme()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: unstuckme");
            try
            {
                Main.Instance.Player.RunBlockers.Clear();
            }
            catch
            {

                try
                {
                    Main.Instance.Player.RunBlockers = new List<string>();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                Main.Instance.Player.MoveBlockers.Clear();
            }
            catch
            {

                try
                {
                    Main.Instance.Player.MoveBlockers = new List<string>();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                Main.Instance.Player.ThisPersonInt.InteractBlockers.Clear();
            }
            catch
            {

                try
                {
                    Main.Instance.Player.ThisPersonInt.InteractBlockers = new List<string>();
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                Main.Instance.Player.CanMove = true;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.InteractingWith.CanLeave = true;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.Interacting = false;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.InCombat = false;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.CanSaveFlags.Remove("CantMoveNow");
            }
            catch
            {
            }

            try
            {
                Main.Instance.CanSaveFlags = Main.Instance.CanSaveFlags;
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.SleepMenu.SetActive(false);
            }
            catch (Exception e)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.EscMenu.SetActive(false);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.TextInputMenu.SetActive(false);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.TraderMenu.SetActive(false);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.EnableMove();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.AllowCursor();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.EndChat();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.UnstuckPlayer();
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.m_Character.m_Animator.SetFloat("Forward", 0.5f);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.m_Character.m_Animator.SetFloat("Turn", 0.5f);
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.m_Character.m_Rigidbody.velocity = Vector3.one;
            }
            catch (Exception ex)
            {
            }

            try
            {
                UI_Gameplay _this = (UI_Gameplay)Main.Instance.GameplayMenu;
                try
                {
                    _this.CloseStorage();
                }
                catch (Exception ex)
                {
                }

                try
                {
                    _this.CloseEscMenu();
                }
                catch (Exception ex)
                {
                }

                try
                {
                    _this.CloseJournal();
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (Main.Instance.PeopleFollowingPlayer.Count > 0)
                {
                    Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME 2.0 Following Player ");
                    for (int i = 0; i < Main.Instance.PeopleFollowingPlayer.Count; i++)
                    {
                        if (Main.Instance.Player.transform != null && Main.Instance.PeopleFollowingPlayer[i].transform != null)
                        {
                            Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME MINI F8 2.0 Following Player ");
                            Main.Instance.PeopleFollowingPlayer[i].transform.position = Main.Instance.Player.transform.position;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                Main.Instance.GameplayMenu.ShowNotification("UNSTUCK ME 3.0!");
            }
            catch (Exception ex)
            {
            }

            try
            {
                Logger.LogInfo("UNSTUCK ME REALLLY!");
            }
            catch (Exception ex)
            {
            }
        }
        public static void autosave()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: autosave");
            Main.Instance.SaveGame(true);
        }
        public static void save()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: save");
            Main.Instance.SaveGame(false);
        }
        public static void maxrelationships()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxrelationships");
            if (Main.Instance.GameplayMenu.Relationships != null)
            {
                int length = Main.Instance.GameplayMenu.Relationships.Count;
                for (int i = 0; i < length; i++)
                {
                    Main.Instance.GameplayMenu.Relationships[i].Favor = 100000000;
                }
            }
        }

        public static void pregnancy(bool realpregnancy)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: pregnancy");
            bool isNotAGirl = Main.Instance.Player is Girl;
            isNotAGirl = !isNotAGirl;

            if (isNotAGirl)
            {
                Main.Instance.GameplayMenu.ShowNotification("You are a male!");
                return;
            }

            float fertility = Main.Instance.Player.Fertility;
            float storymodefertility = Main.Instance.Player.StoryModeFertility;

            bool setFertility = fertility > 0;
            bool setStoryModeFertility = storymodefertility > 0;

            bool togglePregnancy = setFertility || setStoryModeFertility;

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

            Main.Instance.Player.Fertility = fertility;
            Main.Instance.Player.StoryModeFertility = storymodefertility;

            if (realpregnancy && togglePregnancy)
            {
                // Main.Instance.Player.States[7] = togglePregnancy; // toggle you are pregnant state
                Main.Instance.Player.Fertility *= 1000;
                (Main.Instance.Player as Girl).BecomePreg();
                Main.Instance.Player.Fertility = fertility;
            }
        }

        public static void infinitehealth()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: infinitehealth");
            Main.Instance.Player.CantBeHit = !Main.Instance.Player.CantBeHit;
            if (Main.Instance.Player.CantBeHit)
            {
                Main.Instance.Player.NoEnergyLoss = true;
                Main.Instance.GameplayMenu.ShowNotification("infinitehealth: on");
            }
            else
            {
                Main.Instance.Player.NoEnergyLoss = false;
                Main.Instance.GameplayMenu.ShowNotification("infinitehealth: off");
            }
        }
        public static void npcinfinitehealth()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcinfinitehealth");

            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            person.CantBeHit = !person.CantBeHit;
            if (person.CantBeHit)
            {
                person.NoEnergyLoss = true;
                Main.Instance.GameplayMenu.ShowNotification("npcinfinitehealth: on");
            }
            else
            {
                person.NoEnergyLoss = false;
                Main.Instance.GameplayMenu.ShowNotification("npcinfinitehealth: off");
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

                if (items == null || items.Count == 0)
                {
                    return;
                }

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
            setCleanSkinStates(_this.gameObject);
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

            setNudeStates(Main.Instance.Player.gameObject, realnude, false);

            setNudeClothesPoints(Main.Instance.Player.gameObject);
        }
        public static void sexy(bool realsexy)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: sexy");

            setSexyStates(Main.Instance.Player.gameObject, realsexy, false);

            setSexyClothesPoints(Main.Instance.Player.gameObject);
        }
        public static void npcsexy()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcsexy");

            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            setSexyStates(person.gameObject, true, false);

            setSexyClothesPoints(person.gameObject);
        }

        public static void casual(bool realcasual)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: casual");

            setCasualStates(Main.Instance.Player.gameObject, realcasual, false);

            setCasualClothesPoints(Main.Instance.Player .gameObject);
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
            Main.Instance.GameplayMenu.ShowNotification(vector3ToString(lastSpawnPoint));            
        }

        public static void handleCommand(string inputText)
        {
            if (inputText == null) { return; }

            Logger.LogInfo($"User entered: {inputText}");

            try
            {
                string pattern3 = @"(?:^(?<command>\S+)\s+(?<key>\S+)\s+(?<value>\S+)$)";
                Regex rg3 = new Regex(pattern3, RegexOptions.IgnoreCase);
                Match rg3Match = rg3.Match(inputText);

                if (rg3Match.Success)
                {
                    handleCommandLength3(rg3Match.Groups["command"].Value.ToLower(), rg3Match.Groups["key"].Value.ToLower(), rg3Match.Groups["value"].Value.ToLower());
                    return;
                }

                string pattern2 = @"(?:^(?<command>\S+)\s+(?<value>\S+)$)";
                Regex rg2 = new Regex(pattern2, RegexOptions.IgnoreCase);
                Match rg2Match = rg2.Match(inputText);

                if (rg2Match.Success)
                {
                    handleCommandLength2(rg2Match.Groups["command"].Value.ToLower(), rg2Match.Groups["value"].Value.ToLower());
                    return;
                }

                string pattern1 = @"(?:^(?<command>\S+)$)";

                Regex rg1 = new Regex(pattern1, RegexOptions.IgnoreCase);
                Match rg1Match = rg1.Match(inputText);

                if (rg1Match.Success)
                {
                    handleCommandLength1(rg1Match.Groups["command"].Value.ToLower());
                    return;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
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

                case "maxrs":
                case "maxrelationships":
                    {
                        maxrelationships();
                    }
                    break;

                case "save":
                    {
                        save();
                    }
                    break;

                case "autosave":
                    {
                        autosave();
                    }
                    break;

                case "unstuckme":
                    {
                        unstuckme();
                    }
                    break;

                case "warpfollower":
                    {
                        warpfollower();
                    }
                    break;

                case "lock":
                    {
                        lock_int();
                    }
                    break;


                case "unlock":
                    {
                        unlock_int();
                    }
                    break;

                case "storagemax":
                    {
                        storagemax_int();
                    }
                    break;

                case "storagemaxbp":
                case "storagemaxbackpack":
                    {
                        storagemax_int_backpack();
                    }
                    break;

                case "isnpcfuckable":
                case "getpersonstate":
                    {
                        getpersonstate();
                    }
                    break;

                case "setnpctonotfuckable":
                case "setpersonstatetowork":
                    {
                        setpersonstatetowork();
                    }
                    break;

                case "setnpctofuckable":
                case "setpersonstatetofree":
                    {
                        setpersonstatetofree();
                    }
                    break;

                case "togglenpcfuckable":
                case "togglepersonstate":
                    {
                        togglepersonstate();
                    }
                    break;

                case "completeallquests":
                    {
                        completeallquests();
                    }
                    break;

                case "spawnmale":
                    {
                        spawnmale("guy1");
                    }
                    break;

                case "spawnfemale":
                    {
                        spawnfemale("nameless");
                    }
                    break;

                case "spawnmalenude":
                    {
                        spawnmalenude("guy1");
                    }
                    break;

                case "spawnfemalenude":
                    {
                        spawnfemalenude("brat");
                    }
                    break;

                case "spawnmalesave":
                    {
                        spawnmale("guy1", true);
                    }
                    break;

                case "spawnfemalesave":
                    {
                        spawnfemale("nameless", true);
                    }
                    break;

                case "spawnmalenudesave":
                    {
                        spawnmalenude("guy1", true);
                    }
                    break;

                case "spawnfemalenudesave":
                    {
                        spawnfemalenude("brat", true);
                    }
                    break;

                case "spawnbirth":
                case "spawnbirthintopod":
                    {
                        spawnbirthintopod();
                    }
                    break;

                case "patreon":
                    {
                        patreon();
                    }
                    break;

                case "kofi":
                    {
                        kofi();
                    }
                    break;

                case "subscribestar":
                    {
                        subscribestar();
                    }
                    break;

                case "discord":
                    {
                        discord();
                    }
                    break;

                case "getmodslink":
                    {
                        getmodslink();
                    }
                    break;

                case "getmoremods":
                    {
                        getmoremods();
                    }
                    break;

                case "supportme":
                    {
                        supportme();
                    }
                    break;

                case "help":
                    {
                        help();
                    }
                    break;

                case "nympho":
                    {
                        nympho();
                    }
                    break;

                case "iamanympho":
                    {
                        iamanympho();
                    }
                    break;

                case "raycastobjcopy":
                case "copyobject":
                case "c":
                case "copy":
                    {
                        copy();
                    }
                    break;

                case "spawnobject":
                case "raycastobjpaste1":
                case "p":
                case "paste":
                    {
                        paste();
                    }
                    break;

                case "spawnobject2":
                case "raycastobjpaste2":
                case "p2":
                case "paste2":
                    {
                       paste2();
                    }
                    break;

                case "npcsexy":
                    {
                        npcsexy();
                    }
                    break;

                case "npcinfinitehealth":
                    {
                        npcinfinitehealth();
                    }
                    break;

                default:
                    {
                        Main.Instance.GameplayMenu.ShowNotification("No command");
                    }
                    break;
            }
        }

        public static void handleCommandLength2(string command, string value)
        {
            switch (command)
            {
                // not for @Neoton begin
                case "additem":
                    {
                        additem(value, "1");
                    }
                    break;
                // not for @Neoton end

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

                case "setstoragesize":
                    {
                        setstoragesize(value);
                    }
                    break;

                case "setstoragesizebp":
                case "setstoragesizebackpack":
                    {
                        setstoragesizebackpack(value);
                    }
                    break;

                case "spawnmale":
                    {
                        spawnmale(value);
                    }
                    break;

                case "spawnfemale":
                    {
                        spawnfemale(value);
                    }
                    break;

                case "spawnmalenude":
                    {
                        spawnmalenude(value);
                    }
                    break;

                case "spawnfemalenude":
                    {
                        spawnfemalenude(value);
                    }
                    break;

                case "spawnmalesave":
                    {
                        spawnmale(value, true);
                    }
                    break;

                case "spawnfemalesave":
                    {
                        spawnfemale(value, true);
                    }
                    break;

                case "spawnmalenudesave":
                    {
                        spawnmalenude(value, true);
                    }
                    break;

                case "spawnfemalenudesave":
                    {
                        spawnfemalenude(value, true);
                    }
                    break;

                case "s":
                case "saveobject":
                    {
                        saveobject(value);
                    }
                    break;

                case "l":
                case "loadobject":
                    {
                        loadobject(value);
                    }
                    break;

                case "s2":
                case "saveobject2":
                    {
                        saveobject2(value);
                    }
                    break;

                case "l2":
                case "loadobject2":
                    {
                        loadobject2(value);
                    }
                    break;

                case "addwarp":
                case "savewarp":
                    {
                        savewarp(value);
                    }
                    break;

                case "removewarp":
                    {
                        removewarp(value);
                    }
                    break;

                default:
                    {
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

                case "setnpcstate":
                    {
                        setnpcstate(key, value);
                    }
                    break;

                default:
                    {
                        Main.Instance.GameplayMenu.ShowNotification("No command");
                    }
                    break;
            }
        }
        public static void savewarp(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: savewarp");

            Vector3 point = Main.Instance.Player.transform.position;

            if (spawnpointsNames.Contains(value) || predefinedUserWarpsNames.Contains(value))
            {
                Main.Instance.GameplayMenu.ShowNotification($"savewarp: warppoint {value} already exists!");
                return;
            }

            try
            {
                warpPointsUser = addWarpPoint(warpPointsUser, value, point);
                safeWarpPoints();
            }
            catch (Exception ex)
            {
            }

            Main.Instance.GameplayMenu.ShowNotification($"savewarp: warppoint {value} saved!");
        }
        public static void removewarp(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: removewarp");

            Vector3 point = Main.Instance.Player.transform.position;

            if (!spawnpointsNames.Contains(value) || !predefinedUserWarpsNames.Contains(value))
            {
                Main.Instance.GameplayMenu.ShowNotification($"removewarp: warppoint {value} not exists!");
                return;
            }

            try
            {
                warpPointsUser = removeWarpPoint(warpPointsUser, value);
                safeWarpPoints();
            }
            catch (Exception ex)
            {
            }

            Main.Instance.GameplayMenu.ShowNotification($"removewarp: warppoint {value} removed!");
        }
        public static void addweapon(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: addweapon");

            GameObject weapon = getWeaponByName(null, value);

            if (weapon != null)
            {
                GameObject weaponx = Main.Spawn(weapon);

                if (weaponx == null)
                {
                    return;
                }

                Main.Instance.Player.WeaponInv.DropAllWeapons();
                Main.Instance.Player.WeaponInv.PickupWeapon(weaponx);
            }
        }
        public static void setstoragesizebackpack(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setstoragesizebackpack");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? int.MaxValue : amount;
            amount = amount >= int.MaxValue ? int.MaxValue : amount;

            int level = amount;

            try
            {
                if (Main.Instance.Player == null || Main.Instance.Player.CurrentBackpack == null || Main.Instance.Player.CurrentBackpack.ThisStorage == null)
                {
                    return;
                }

                int count = Main.Instance.Player.CurrentBackpack.ThisStorage.StorageItems.Count;

                if (level <= count)
                {
                    level = count + 10;
                }

                level = level <= 0 ? int.MaxValue : level;

                Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = level;
                Main.Instance.GameplayMenu.ShowNotification("setstoragesizebackpack: set storage size from player to " + level.ToString());
            }
            catch (Exception e)
            {
            }

            try
            {
                if (Main.Instance.Player == null || Main.Instance.Player == null || Main.Instance.Player.WeaponInv.IntLookingAt == null)
                {
                    return;
                }

                Interactible la = Main.Instance.Player.WeaponInv.IntLookingAt;

                if (la is Int_Storage)
                {
                    Int_Storage int_storage = (Int_Storage)la;
                    int_storage.StorageMax = level;
                    Main.Instance.GameplayMenu.ShowNotification("setstoragesizebackpack: set storage size to " + level.ToString());
                }
            }
            catch (Exception e)
            {
            }
        }

        public static void setstoragesize(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setstoragesize");
            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            amount = amount <= 0 ? int.MaxValue : amount;
            amount = amount >= int.MaxValue ? int.MaxValue : amount;

            int level = amount;
            try
            {
                if (Main.Instance.Player == null || Main.Instance.Player.WeaponInv == null || Main.Instance.Player.WeaponInv.IntLookingAt == null)
                {
                    return;
                }

                Interactible la = Main.Instance.Player.WeaponInv.IntLookingAt;

                if (la is Int_Storage)
                {
                    Int_Storage int_storage = (Int_Storage)la;

                    int count = int_storage.StorageItems.Count;

                    if (level <= count)
                    {
                        level = count + 10;
                    }

                    level = level <= 0 ? int.MaxValue : level;

                    int_storage.StorageMax = level;
                    Main.Instance.GameplayMenu.ShowNotification("setstoragesize: set storage size to " + level.ToString());
                }
            }
            catch (Exception e)
            {
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
            }
            else
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
        public static void setnpcstate(string key, string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: setnpcstate");

            GameObject thisPersonGa = getPersonInteract();

            if (thisPersonGa == null)
            {
                return;
            }

            Person thisPerson = thisPersonGa.GetComponent<Person>();

            if (thisPerson == null)
            {
                return;
            }

            int statesLength = thisPerson.States.Length;

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

            thisPerson.States[index] = state;
            Main.Instance.GameplayMenu.ShowNotification("setnpcstate " + index.ToString() + " to " + (state ? "true" : "false"));
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
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("additem: No item found");
            }
        }

        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> configEnableMe;
        private ConfigEntry<KeyCode> configKeyCodeOpenTheCheatConsole;
        private ConfigEntry<KeyCode> configKeyCodeOpenTheCheatConsoleFallback;


        public BitchlandCheatConsoleBepInEx()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        private static string pluginKey = "General.Toggles";

        private static string pluginKeyControls = "General.KeyControls";

        public static bool enableThisMod = false;

        public static KeyCode KeyCodeF1 = 0;
        public static KeyCode KeyCodeF2 = 0;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableThisMod",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");

            configKeyCodeOpenTheCheatConsole = Config.Bind(pluginKeyControls,
                                                           "KeyCodeOpenTheCheatConsole",
                                                            KeyCode.F1,
                                                           "KeyCode to open the cheat console default F1");

            configKeyCodeOpenTheCheatConsoleFallback = Config.Bind(pluginKeyControls,
                                                                   "KeyCodeOpenTheCheatConsoleFallback",
                                                                   KeyCode.F2,
                                                                   "KeyCode fallback to open the cheat console default F2");

            enableThisMod = configEnableMe.Value;

            KeyCodeF1 = configKeyCodeOpenTheCheatConsole.Value;
            KeyCodeF2 = configKeyCodeOpenTheCheatConsoleFallback.Value;

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

        /*
        [HarmonyPatch(typeof(Main), "SpawnFromCustomBundle")]
        [HarmonyPrefix]
        public static bool SpawnFromCustomBundle(string assetName, ref GameObject __result, object __instance)
        {
            Main _this = (Main)__instance;
            Main.Log("SpawnFromCustomBundle: " + assetName);
            for (int index = 0; index < _this.RegisteredCustomBundles.Count; ++index)
            {
                try
                {
                    Main.RegisteredCustomBundle registeredCustomBundle = _this.RegisteredCustomBundles[index];
                    if (registeredCustomBundle.AssetName == assetName)
                    {
                        if ((UnityEngine.Object)registeredCustomBundle.LoadedAsset == (UnityEngine.Object)null)
                        {
                            if ((UnityEngine.Object)registeredCustomBundle.LoadedBundle == (UnityEngine.Object)null)
                                registeredCustomBundle.LoadedBundle = AssetBundle.LoadFromFile(registeredCustomBundle.BundleFileName);
                            registeredCustomBundle.LoadedAsset = registeredCustomBundle.LoadedBundle.LoadAsset<GameObject>(assetName);
                        }
                        GameObject gameObject = Main.Spawn(registeredCustomBundle.LoadedAsset);
                        registeredCustomBundle.LoadedBundle.Unload(false);
                        __result = gameObject;
                        Main.Log(gameObject.name);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Main.Log($"SpawnFromCustomBundle ERROR: {ex.Message}\n{ex.StackTrace}");
                }
            }
            return false;
        }*/

        /*
        [HarmonyPatch(typeof(BaseObjectPlacer), "SpawnObject")]
        [HarmonyPrefix]
        public static bool SpawnObject(
            GameObject prefab,
            Vector3 spawnLocation,
            Transform objectRoot, object __instance)
        {
            Logger.LogInfo("EXECUTE MME SPAWNOBJECT");
            return true;
        }*/
        /*
        [HarmonyPatch(typeof(BaseObjectPlacer), "Execute")]
        [HarmonyPrefix]
        public static bool Execute(
            ProcGenConfigSO globalConfig,
            Transform objectRoot,
            int mapResolution,
            float[,] heightMap,
            Vector3 heightmapScale,
            float[,] slopeMap,
            float[,,] alphaMaps,
            int alphaMapResolution,
            byte[,] biomeMap,
            int biomeIndex,
            BiomeConfigSO biome, object __instance)
        {
            Logger.LogInfo("EXECUTE MME EXECUTE");
            string lines = "";
            BaseObjectPlacer _this = (BaseObjectPlacer)__instance;
            FieldInfo ObjectsField = __instance.GetType().GetField("Objects", BindingFlags.NonPublic | BindingFlags.Instance);

            if (ObjectsField == null)
            {
                return true;
            }

            object fieldValue = ObjectsField.GetValue(__instance);

            if (fieldValue == null)
            {
                return true;
            }

            List<PlaceableObjectConfig> Objects = (List<PlaceableObjectConfig>)fieldValue;

            List<GameObject> prefabs = new List<GameObject>();

            foreach (PlaceableObjectConfig placeableObjectConfig in Objects)
            {
                int count = placeableObjectConfig.Prefabs.Count;

                for (int i = 0; i < count; i++)
                {
                    GameObject prefab = placeableObjectConfig.Prefabs[i];

                    if (prefab == null)
                    {
                        continue;
                    }

                    string line = "prefab name: " + prefab.name.ToString();

                    Logger.LogInfo(line);

                    lines += line + "\n";

                    prefabs.Add(prefab);
                }
            }

            System.IO.File.WriteAllText("output.txt", lines);

            return true;
        }
        /*
        [HarmonyPatch(typeof(MainMenu), "PutDisplayGirl")]
        [HarmonyPrefix]
        public static bool PutDisplayGirl(object __instance)
        {
            MainMenu _this = (MainMenu)__instance;
            Transform displayGirl = _this.DisplayGirls[_this.CurrentDisplayGirl];
            Girl _girl;
            if (displayGirl.childCount == 0)
            {
                _girl = Main.Spawn(Main.Instance.PersonPrefab, _this.DisplayGirls[_this.CurrentDisplayGirl]).GetComponent<Girl>();
                _girl.LoadFromFile($"{Main.AssetsFolder}/Characters/MainMenu/{displayGirl.name}.png");
                 Logger.LogInfo("GIrl NAME: " + displayGirl.name);
                _girl.transform.localEulerAngles = Vector3.zero;
                _girl.transform.localPosition = Vector3.zero;
                _girl.AddMoveBlocker("mainmenudisplay");
                _girl.LodRen.gameObject.SetActive(false);
                _girl.SetHighLod();
                _girl.Anim.applyRootMotion = false;
                _girl.LookAtPlayer.enabled = true;
                _girl.LookAtPlayer.playerTransform = _this.MainMenuCam;
                _girl.LookAtPlayer.DontBlockSides = true;
                _girl.A_Standing = "boobs1";
                _this.CurrentGirlText.text = displayGirl.name;
                _girl.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                string path = $"{Main.AssetsFolder}/Characters/MainMenu/{displayGirl.name}.txt";
                bool flag = false;
                if (File.Exists(path))
                {
                    foreach (string readAllLine in File.ReadAllLines(path))
                    {
                        string[] strArray1 = readAllLine.Split("=", StringSplitOptions.None);
                        switch (strArray1[0])
                        {
                            case "anim":
                                flag = true;
                                _girl.A_Standing = strArray1[1];
                                break;
                            case "clothes":
                                string[] strArray2 = strArray1[1].Split(";", StringSplitOptions.None);
                                _this._Clothes = new GameObject[strArray2.Length];
                                for (int index1 = 0; index1 < strArray2.Length; ++index1)
                                {
                                    string[] strArray3 = strArray2[index1].Split(":", StringSplitOptions.None);
                                    for (int index2 = 0; index2 < Main.Instance.AllPrefabs.Count; ++index2)
                                    {
                                        if (strArray3[0] == Main.Instance.AllPrefabs[index2].name)
                                        {
                                            _this._Clothes[index1] = Main.Instance.AllPrefabs[index2];
                                            Logger.LogInfo("DISPLAY GIRL NAME " + displayGirl.name + " AllPrefabs[index2]: " + index2.ToString());
                                            if (strArray3.Length > 1 && (UnityEngine.Object)_this._Clothes[index1].GetComponentInChildren<int_PickableClothingPackage>(true) != (UnityEngine.Object)null)
                                            {
                                                GameObject gameObject = Main.Spawn(_this._Clothes[index1], Main.Instance.DisabledObjects);
                                                int_PickableClothingPackage componentInChildren = gameObject.GetComponentInChildren<int_PickableClothingPackage>(true);
                                                if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
                                                {
                                                    componentInChildren.ClothingData = $":{strArray3[1]}:RGBA(0.000, 0.000, 0.000, 0.000)";
                                                    Logger.LogInfo("DISPLAY GIRL NAME " + displayGirl.name + " index2: " + index2.ToString() + " clothingData: " + componentInChildren.ClothingData);
                                                }
                                                    
                                                _this._Clothes[index1] = gameObject;
                                                break;
                                            }
                                            break;
                                        }
                                    }
                                }
                                _girl.ChangeUniform(_this._Clothes);
                                break;
                            case "extraheadrot":
                                _girl.LookAtPlayer.ExtraRot = true;
                                _girl.LookAtPlayer.AddHeadRotation = Main.ParseVector3(strArray1[1]);
                                break;
                            case "eyes":
                                if (bool.Parse(strArray1[1]))
                                {
                                    _girl.LookAtPlayer.OnlyEyes = true;
                                    break;
                                }
                                _girl.LookAtPlayer.OnlyEyes = false;
                                break;
                            case "face":
                                _girl.ApplyFaceBlendShapeData(Main.Instance.BlendShapesDatas[int.Parse(strArray1[1])]);
                                break;
                            case "name":
                                _this.CurrentGirlText.text = strArray1[1];
                                displayGirl.name = strArray1[1];
                                break;
                            case "pos":
                                _girl.transform.localPosition = Main.ParseVector3(strArray1[1]);
                                break;
                            case "rot":
                                _girl.transform.localEulerAngles = Main.ParseVector3(strArray1[1]);
                                break;
                            case "scale":
                                _girl.transform.localScale = Main.ParseVector3(strArray1[1]);
                                break;
                        }
                    }
                }
                if (!flag || flag && _girl.A_Standing == "boobs1")
                {
                    mminit_1 component = _this.MainMenuPos.GetComponent<mminit_1>();
                    component.TheGirl = _girl;
                    component.Init();
                }
                else
                    Main.RunInNextFrame((Action)(() =>
                    {
                        if (!_girl.gameObject.activeSelf)
                            return;
                        _girl.GirlPhysics = true;
                    }));
                _this.DisplayGirls[0].gameObject.SetActive(false);
            }
            else
            {
                _girl = displayGirl.GetComponentInChildren<Girl>(true);
                Logger.LogInfo("name from girl: " + _girl.name);
                for (int i = 0; i < _girl.EquippedClothes.Count; i++)
                {
                    string name = _girl.EquippedClothes[i].name.Replace(" ", "_").ToLower();
                    Logger.LogInfo("name from girl and item :" + _girl.name + " item: " + name);
                    for (int j = 0; j < Main.Instance.AllPrefabs.Count; j++)
                    {
                        string prefabitem = Main.Instance.AllPrefabs[j].name;
                        prefabitem = prefabitem.Replace(" ","_").ToLower();
                        if (name == prefabitem)
                        {
                            Logger.LogInfo("name from girl and item :" + _girl.name + " item: " + name + " index: " + j.ToString());
                        }
                    }
                }
                _girl.gameObject.SetActive(true);
                _this.CurrentGirlText.text = displayGirl.name;
                Main.RunInNextFrame((Action)(() =>
                {
                    if (!(_girl.A_Standing != "boobs1") || !_girl.gameObject.activeSelf)
                        return;
                    _girl.GirlPhysics = true;
                }));
            }
            return false;
        }
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
