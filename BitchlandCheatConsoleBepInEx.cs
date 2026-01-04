using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using Defective.JSON;
using Den.Tools;
using HarmonyLib;
using SemanticVersioning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using static Den.Tools.MatrixAsset;

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

        // flightSettings
        public static float fly_moveSpeed = 10f;       // Base movement speed
        public static float fly_sprintMultiplier = 2f; // Speed multiplier when holding Shift
        public static bool fly_disableGravityWhileFlying = true;
        public static bool is_free_fly_setup = false;
        public static bool default_useGravity = false;
        public static bool fly_on = false;
        public static Rigidbody fly_rb = null;

        public static BitchlandCheatConsoleBepInEx instance = new BitchlandCheatConsoleBepInEx();

        public static AudioSource audioSource = null;

        private static void initAudioSource()
        {
            try
            {
                if (BitchlandCheatConsoleBepInEx.audioSource == null)
                {
                    BitchlandCheatConsoleBepInEx.audioSource =  Main.Instance.Player.gameObject.AddComponent<AudioSource>();
                }
                if (BitchlandCheatConsoleBepInEx.audioSource == null)
                {
                    throw new Exception("audioSource == null");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }
        private void Init()
        {
            if (isInit)
            {
                return;
            }

            initAudioSource();

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

        private void HandleFlight()
        {
            if (fly_on == false || fly_rb == null)
            {
                return;
            }

            Rigidbody rb = fly_rb;
            // Get input axes
            float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
            float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down (forward/back)

            // Vertical movement (Space = up, LeftControl = down)
            float ascend = 0f;
           // Vector3 v = rb.velocity;
            if (Input.GetKey(KeyCode.Space)) {
                ascend = 1f;
               // v.y += 1f;
            }
            else if (Input.GetKey(KeyCode.LeftControl)) {
                ascend = -1f;
               // v.y -= 1f;
            }

           /* if (Input.GetKey(KeyCode.W))
            {
                v.x -= 1;
            } else if(Input.GetKey(KeyCode.S))
            {
                v.x += 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                v.z += 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                v.z -= 1;
            }*/

            // Sprint modifier
            float currentSpeed = fly_moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed *= fly_sprintMultiplier;

            // Calculate movement direction relative to camera
            Vector3 moveDirection = (transform.forward * vertical) +
                                    (transform.right * horizontal) +
                                    (transform.up * ascend);

            // Apply velocity directly for smooth movement
            rb.velocity = moveDirection.normalized * currentSpeed;
            //rb.velocity = v;
        }

        private void Update()
        {
            TriggerUpdate();
            HandleFlight();
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
                spawnpointsCount = spawnpoints.Count;
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
                spawnpointsCount = spawnpoints.Count;
            }
        }

        public static void safeWarpPoints()
        {
            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            Directory.CreateDirectory(audioFolder);

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

            string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            Directory.CreateDirectory(audioFolder);

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

            string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            Directory.CreateDirectory(audioFolder);

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

            string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            Directory.CreateDirectory(audioFolder);

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
        public static GameObject getMultiInteract()
        {
            GameObject interact = getInteract();
            
            if (interact == null)
            {
                return null;
            }

            Interactible interactible = interact.GetComponent<Interactible>();

            if (interactible == null)
            {
                return null;
            }

            MultiInteractible multi = null;
            
            if (interactible is MultiInteractible)
            {
                multi = (MultiInteractible)interactible;
                return multi.gameObject;
            }

            return null;
        }

        public static Dictionary<string,GameObject> getMultiPartsInteract()
        {
            GameObject multi = getMultiInteract();

            if (multi == null)
            {
                return null;
            }

            MultiInteractible multiInteractible = multi.GetComponent<MultiInteractible>();

            if (multiInteractible.Parts == null)
            {
                return null;
            }
            
            Dictionary<string,GameObject> list = new Dictionary<string, GameObject>();

            for (int i = 0; i < multiInteractible.Parts.Length; i++)
            {
                string interactText = multiInteractible.Parts[i].InteractText;
                interactText = interactText != null ? interactText.ToLower().Replace(" ", "_") : "";
                list.Add(interactText, multiInteractible.Parts[i].gameObject);
            }

            return list;
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

        public static GameObject getPickupToHandInteract()
        {
            GameObject ga = getInteract();

            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            int_PickupToHand pi = null;

            if (la != null)
            {
                if (!(la is MultiInteractible))
                {
                    return null;
                }

                GameObject piGa = la.gameObject;
                if (piGa != null)
                {
                    Interactible pis = piGa.GetComponent<Interactible>();
                    if (pis != null)
                    {
                        pi = pis.GetComponentInChildren<int_PickupToHand>(true);
                    }
                }
                
            }

            if (pi != null && pi is int_PickupToHand)
            {
                int_PickupToHand int_Pu = (int_PickupToHand)pi;
                return int_Pu.gameObject;
            }

            return null;
        }

        public static GameObject getPickupToBagInteract(GameObject ga)
        {
            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            int_PickupToBag pi = null;

            if (la != null)
            {
                if (!(la is MultiInteractible))
                {
                    return null;
                }

                GameObject piGa = la.gameObject;
                if (piGa != null)
                {
                    Interactible pis = piGa.GetComponent<Interactible>();
                    if (pis != null)
                    {
                        pi = pis.GetComponentInChildren<int_PickupToBag>(true);
                    }
                }

            }

            if (pi != null && pi is int_PickupToBag)
            {
                int_PickupToBag int_Pu = (int_PickupToBag)pi;
                return int_Pu.gameObject;
            }

            return null;
        }
        public static GameObject[] getChildren(GameObject ga)
        {
            if (ga == null)
            {
                return null;
            }

            Interactible la = ga.GetComponent<Interactible>();

            Interactible[] pi = null;

            if (la != null)
            {
                if (!(la is MultiInteractible))
                {
                    return null;
                }

                GameObject piGa = la.gameObject;
                if (piGa != null)
                {
                    Interactible pis = piGa.GetComponent<Interactible>();
                    if (pis != null)
                    {
                        pi = pis.GetComponentsInChildren<Interactible>(true);
                    }
                }

            }

            if (pi != null)
            {
                GameObject[] gas = new GameObject[pi.Length];
                for (int i = 0; i < pi.Length; i++)
                {
                    gas[i] = pi[i].gameObject;
                }
                return gas;
            }

            return null;
        }

        public static GameObject SafeSpawn(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            Int_Storage storage_hands = Main.Instance.Player.Storage_Hands;
            Transform storage_hands_dropspot = storage_hands.DropSpot;
            if (storage_hands_dropspot != null)
            {
                return SafeSpawnFromStorage(go, storage_hands);
            }

            Int_Storage storage_anal = Main.Instance.Player.Storage_Anal;
            Transform storage_anal_dropspot = storage_anal.DropSpot;
            if (storage_anal_dropspot != null)
            {
                return SafeSpawnFromStorage(go, storage_anal);
            }

            Int_Storage storage_vaginal = Main.Instance.Player.Storage_Vag;
            Transform storage_vaginal_dropspot = storage_vaginal.DropSpot;
            if (storage_vaginal_dropspot != null)
            {
                return SafeSpawnFromStorage(go, storage_vaginal);
            }

            Int_Storage backpack = Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null ? Main.Instance.Player.CurrentBackpack.ThisStorage : null;
            Transform backpack_dropspot = backpack != null ? backpack.DropSpot : null;
            if (backpack_dropspot != null)
            {
                return SafeSpawnFromStorage(go, backpack);
            }

            return SafeSpawnFromStorage(go, null);
        }

        public static GameObject SafeSpawnFromStorage(GameObject go, Int_Storage storage)
        {
            if (go == null)
            {
                return null;
            }

            Transform storageSpot = storage == null ? null : storage.DropSpot;
            GameObject item = Main.Spawn(go);

            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            if (storageSpot != null)
            {
                position = storageSpot.position;
                rotation = storageSpot.rotation;
                item.transform.SetPositionAndRotation(position, rotation);
            } else
            {
                item.transform.SetLocalPositionAndRotation(position, rotation);
            }

            item.transform.SetParent((Transform)null, true);
            Rigidbody componentInChildren1 = item.GetComponentInChildren<Rigidbody>(false);
            Collider componentInChildren2 = item.GetComponentInChildren<Collider>(false);
            if (componentInChildren1 != null)
                componentInChildren1.isKinematic = false;
            if (componentInChildren2 != null)
                componentInChildren2.enabled = true;
            item.SetActive(true);
            return item;
        }

        public static GameObject CreatePersonNew(string name, bool save = true, bool spawnFemale = true)
        {
            bool LoadSpecificNPC = true;
            Person PersonGenerated = null;
            if (LoadSpecificNPC)
            {
                PersonGenerated = spawnFemale ? UnityEngine.Object.Instantiate<GameObject>(Main.Instance.PersonPrefab).GetComponent<Person>() : UnityEngine.Object.Instantiate<GameObject>(Main.Instance.PersonGuyPrefab).GetComponent<Person>();
                string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";
                string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";
                string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";
                string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";
                Directory.CreateDirectory(objectsFolder);
                Directory.CreateDirectory(malesFolder);
                Directory.CreateDirectory(femalesFolder);
                Directory.CreateDirectory(audioFolder);
                string maleOrFemale = spawnFemale ? "females" : "males";
                string filename = $"{Main.AssetsFolder}/wolfitdm/{maleOrFemale}/{name}.png";
                if (!File.Exists(filename))
                {
                    Main.Instance.GameplayMenu.ShowNotification(filename + " not exists!");
                    return null;
                }
                PersonGenerated.StartingClothes = new List<GameObject>();
                PersonGenerated.StartingWeapons = new List<GameObject>();
                PersonGenerated._StartingClothes = new List<string>();
                PersonGenerated._StartingWeapons = new List<string>();
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

     /*   public static GameObject BasicPerson()
        {
            bool spawnFemale = Main.Instance.Player is Girl ? true : false;
            GameObject personPrefab = spawnFemale ? Main.Instance.PersonPrefab : Main.Instance.PersonGuyPrefab;

            if (personPrefab == null)
            {
                return Main.Instance.Player.gameObject;
            }

            Person person = personPrefab.GetComponent<Person>();

            if (person == null)
            {
                return Main.Instance.Player.gameObject;
            }

            Person player = Main.Instance.Player;

            player.DEBUG = person.DEBUG;
            player.Penis = person.Penis;
            player.PenisEnabled = person.PenisEnabled;
            player.HasPenis = person.HasPenis;
            player.HasCondomPut = person.HasCondomPut;
            player.UnparentOnStart = person.UnparentOnStart;
            player.Anim = person.Anim;
            player.navMesh = person.navMesh;
            player._Rigidbody = person._Rigidbody;
            player.Eyes = person.Eyes;
            player.EyesSpot = person.EyesSpot;
            player.LOD = person.LOD;
            player.ProxSeen = person.ProxSeen;
            player.CantBeHit = person.CantBeHit;
            player.VoicePitch = person.VoicePitch;
            player.ViewPoint = person.ViewPoint;
            player.TorsoViewPoint = person.TorsoViewPoint;
            player._DontLoadInteraction = person._DontLoadInteraction;
            player._DontLoadClothing = person._DontLoadClothing;
            player.MainBodyLowPoly = person.MainBodyLowPoly;
            player.MainBody = person.MainBody;
            player.EyesObjects = person.EyesObjects;
            player.RagdollParts = person.RagdollParts;
            player.StartingClothes = person.StartingClothes;
            player._StartingClothes = person._StartingClothes;
            player.StartingWeapons = person.StartingWeapons;
            player._StartingWeapons = person._StartingWeapons;
            player.CurrentShoes = person.CurrentShoes;
            player.CurrentPants = person.CurrentPants;
            player.CurrentTop = person.CurrentTop;
            player.CurrentUnderwearTop = person.CurrentUnderwearTop;
            player.CurrentUnderwearLower = person.CurrentUnderwearLower;
            player.CurrentGarter = person.CurrentGarter;
            player.CurrentSocks = person.CurrentSocks;
            player.CurrentHat = person.CurrentHat;
            player.CurrentHair = person.CurrentHair;
            player.CurrentFeet = person.CurrentFeet;
            player.CurrentBeard = person.CurrentBeard;
            player.CurrentFeetMesh = person.CurrentFeetMesh;
            player.CurrentAnys = person.CurrentAnys;
            player.CurrentBody = person.CurrentBody;
            player.CurrentHead = person.CurrentHead;
            player.NaturalSkinColor = person.NaturalSkinColor;
            player.NaturalHairColor = person.NaturalHairColor;
            player.NaturalEyeColor = person.NaturalEyeColor;
            player.TannedSkinColor = person.TannedSkinColor;
            player.DyedHairColor = person.DyedHairColor;
            player.DyedEyeColor = person.DyedEyeColor;
            player._CustomSkinStates = person._CustomSkinStates;
            player._CustomFaceSkinStates = person._CustomFaceSkinStates;
            player._FaceSkinStates = person._FaceSkinStates;
            player._SkinStates = person._SkinStates;
            player._States = person._States;
            player._PersonalityData = person._PersonalityData;
            player.BoobSize = person.BoobSize;
            player.AssSize = person.AssSize;
            player.FatSize = person.FatSize;
            player.AnalTraining = person.AnalTraining;
            player.VaginalTraining = person.VaginalTraining;
            player.NippleTraining = person.NippleTraining;
            player.ClitTraining = person.ClitTraining;
            player.BodyTraining = person.BodyTraining;
            player.EquippedClothes = person.EquippedClothes;
            player.SPAWN_noUglyHair = person.SPAWN_noUglyHair;
            player.SPAWN_onlyGoodHair = person.SPAWN_onlyGoodHair;
            player.SecondWeapon = person.SecondWeapon;            player.PersonType = person.PersonType;            player.Inited = person.Inited;
            player.Holes = person.Holes;
            player.PenisBones = person.PenisBones;
            player._CharacterVisible = person._CharacterVisible;
            player._StartedMastPP = person._StartedMastPP;
            player.Do_Schedule_GoingToTargetThread = person.Do_Schedule_GoingToTargetThread;
            player._PathRedo = person._PathRedo;
            player._TimeGoingToTarget = person._TimeGoingToTarget;
            player.RandActionTimer = person.RandActionTimer;
            player.WhileDoingAction = person.WhileDoingAction;
            player.RuntimeActions = person.RuntimeActions;
            player.TEMP_HANDLENEEDS_OFF = person.TEMP_HANDLENEEDS_OFF;
            player.TEMP_HANDLEANIMS_OFF = person.TEMP_HANDLEANIMS_OFF;
            player.TEMP_SEXUPDATE_OFF = person.TEMP_SEXUPDATE_OFF;
            player.TEMP_UPDATE_OFF = person.TEMP_UPDATE_OFF;
            player.TEMP_RUNTIME_OFF = person.TEMP_RUNTIME_OFF;
            player.DecideTimer = person.DecideTimer;
            player._ShootBlind = person._ShootBlind;
            player.CombatDistance = person.CombatDistance;
            player.EndingCombat = person.EndingCombat;
            player.HavingSex_Scene = person.HavingSex_Scene;
            player.HavingSexWith = person.HavingSexWith;
            player.Orgasming = person.Orgasming;
            player.NoEnergyLoss = person.NoEnergyLoss;
            player._HiddenHead = person._HiddenHead;
            player._ClothingBatch1Bones = person._ClothingBatch1Bones;
            player.ObjInHand = person.ObjInHand;
            player.HeadStuff = person.HeadStuff;
            player.RightHandStuff = person.RightHandStuff;
            player.LeftHandStuff = person.LeftHandStuff;
            player.RightHandWankCenter = person.RightHandWankCenter;
            player.AllFaceBones = person.AllFaceBones;
            player.Head = person.Head;
            player.MouthBase = person.MouthBase;
            player.MouthLeft = person.MouthLeft;
            player.MouthRight = person.MouthRight;
            player.MouthTop = person.MouthTop;
            player.MouthBottom = person.MouthBottom;
            player.CheekLowLeft = person.CheekLowLeft;
            player.CheekLowRight = person.CheekLowRight;
            player.CheekUpLeft = person.CheekUpLeft;
            player.CheekUpRight = person.CheekUpRight;
            player.Jaw = person.Jaw;
            player.JawLow = person.JawLow;
            player.Chin = person.Chin;
            player.EarLeft = person.EarLeft;
            player.EarLeftLow = person.EarLeftLow;
            player.EarLeftHigh = person.EarLeftHigh;
            player.EarRight = person.EarRight;
            player.EarRightLow = person.EarRightLow;
            player.EarRightHigh = person.EarRightHigh;
            player.Nose = person.Nose;
            player.NoseBridge = person.NoseBridge;
            player.NoseTip = person.NoseTip;
            player.NostrilLeft = person.NostrilLeft;
            player.NostrilRight = person.NostrilRight;
            player.EyeLeft = person.EyeLeft;
            player.EyeRight = person.EyeRight;
            player.EyeBallLeft = person.EyeBallLeft;
            player.EyeBallRight = person.EyeBallRight;
            player.EyeLeftTop = person.EyeLeftTop;
            player.EyeLeftLow = person.EyeLeftLow;
            player.EyeLeftInner = person.EyeLeftInner;
            player.EyeLeftOuter = person.EyeLeftOuter;
            player.EyeRightTop = person.EyeRightTop;
            player.EyeRightLow = person.EyeRightLow;
            player.EyeRightInner = person.EyeRightInner;
            player.EyeRightOuter = person.EyeRightOuter;
            player.AllBodyBones = person.AllBodyBones;
            player.BoobLeft = person.BoobLeft;
            player.BoobRight = person.BoobRight;
            player.NippleLeft = person.NippleLeft;
            player.NippleRight = person.NippleRight;
            player.AssCheekLeft = person.AssCheekLeft;
            player.AssCheekRight = person.AssCheekRight;
            player.LegLeft = person.LegLeft;
            player.LegRight = person.LegRight;
            player.UpperThighLeft = person.UpperThighLeft;
            player.UpperThighRight = person.UpperThighRight;
            player.MidThighLeft = person.MidThighLeft;
            player.MidThighRight = person.MidThighRight;
            player.LowerThighLeft = person.LowerThighLeft;
            player.LowerThighRight = person.LowerThighRight;
            player.KneeLeft = person.KneeLeft;
            player.KneeRight = person.KneeRight;
            player.CalveLeft = person.CalveLeft;
            player.CalveRight = person.CalveRight;
            player.FootLeft = person.FootLeft;
            player.FootRight = person.FootRight;
            player.ActualHips = person.ActualHips;
            player.Hips = person.Hips;
            player.Hips2 = person.Hips2;
            player.Belly = person.Belly;
            player.Waist = person.Waist;
            player.Ribcage = person.Ribcage;
            player.Torso = person.Torso;
            player.Neck = person.Neck;
            player.ShoulderLeft = person.ShoulderLeft;
            player.UpperArmLeft = person.UpperArmLeft;
            player.ForeArmLeft = person.ForeArmLeft;
            player.HandLeft = person.HandLeft;
            player.ShoulderRight = person.ShoulderRight;
            player.UpperArmRight = person.UpperArmRight;
            player.ForeArmRight = person.ForeArmRight;
            player.HandRight = person.HandRight;
            player.Height = person.Height;
            player.CurrentLOD = person.CurrentLOD;
            player.CinematicCharacter = person.CinematicCharacter;
            player._CantBeForced = person._CantBeForced;
            player.CallWhenHighCol = person.CallWhenHighCol;
            player._DirtySkin = person._DirtySkin;
            player.MainBodyTex = person.MainBodyTex;
            player.MainFaceTex = person.MainFaceTex;
            player.CustomMainBodyTex = person.CustomMainBodyTex;
            player.CustomMainFaceTex = person.CustomMainFaceTex;
            player.sBodyTexIndex = person.sBodyTexIndex;
            player.sFaceTexIndex = person.sFaceTexIndex;
            player.sCustomBodyTexIndex = person.sCustomBodyTexIndex;
            player.sCustomFaceTexIndex = person.sCustomFaceTexIndex;
            player.MaterialTypeNPC = person.MaterialTypeNPC;
            player.PunchingAnim = person.PunchingAnim;
            player.PersonThrowingDown = person.PersonThrowingDown;
            player._CurrentPunchPower = person._CurrentPunchPower;
            player.MeleeHitBox = person.MeleeHitBox;
            player.Doing_Punch = person.Doing_Punch;
            player.Doing_ThrowDown = person.Doing_ThrowDown;
            player.Doing_MeleeHit = person.Doing_MeleeHit;
            player.ClothingCondition = person.ClothingCondition;
            player.transform.position = person.transform.position;
            player.transform.rotation = person.transform.rotation;

            return player.gameObject;
        }*/

        public static void UpdatePerson(GameObject DisplayPersonGa, bool spawnFemale, string filename)
        {
            if (DisplayPersonGa == null) {
                return; 
            }

            Person DisplayPerson = DisplayPersonGa.GetComponent<Person>();
            DisplayPerson._SkinStates = new bool[15];
            DisplayPerson._FaceSkinStates = new bool[16];
            DisplayPerson._CustomSkinStates = new bool[Main.Instance._CustomBodySkinsName.Count];
            DisplayPerson._CustomFaceSkinStates = new bool[Main.Instance._CustomFaceSkinsName.Count];

            for (int i = 0; i < DisplayPerson._SkinStates.Length; i++)
            {
                DisplayPerson._SkinStates[i] = false;
            }

            for (int i = 0; i < DisplayPerson._FaceSkinStates.Length; i++)
            {
                DisplayPerson._FaceSkinStates[i] = false;
            }

            for (int i = 0; i <  DisplayPerson._CustomSkinStates.Length; i++)
            {
                DisplayPerson._CustomSkinStates[i] = false;
            }

            for (int i = 0; i < DisplayPerson._CustomFaceSkinStates.Length; i++)
            {
                DisplayPerson._CustomFaceSkinStates[i] = false;
            }

            if (DisplayPerson == null)
            {
                return;
            }

            Person PresetLoaderNPC_F = Main.Spawn(spawnFemale ? Main.Instance.PersonPrefab : Main.Instance.PersonGuyPrefab).gameObject.GetComponent<Person>();
            
            if (PresetLoaderNPC_F == null)
            {
                return;
            }

            PresetLoaderNPC_F.StartingClothes.Clear();
            PresetLoaderNPC_F._StartingClothes.Clear();
            PresetLoaderNPC_F._SkinStates = new bool[0];
            PresetLoaderNPC_F._FaceSkinStates = new bool[0];
            PresetLoaderNPC_F._DontLoadClothing = true;
            PresetLoaderNPC_F.LoadFromFile(filename);
            PresetLoaderNPC_F.Inited = false;
            PresetLoaderNPC_F.Init();

            if (DisplayPerson.CurrentBody != null)
            {
                Dressable hair = DisplayPerson.CurrentBody;
                DisplayPerson.UndressClothe(hair);
                //UnityEngine.Object.Destroy(hair.gameObject);
            }

            if (DisplayPerson.CurrentAnys != null && DisplayPerson.CurrentAnys.Count > 0)
            {
                List<Dressable> anys = DisplayPerson.CurrentAnys;
                for (int i = 0; i < anys.Count; i++)
                {
                    DisplayPerson.UndressClothe(anys[i]);
                    //UnityEngine.Object.Destroy(anys[i].gameObject);
                }
            }

            if (DisplayPerson.CurrentHair != null)
            {
                Dressable hair = DisplayPerson.CurrentHair;
                DisplayPerson.UndressClothe(hair);
                UnityEngine.Object.Destroy(hair.gameObject);
                DisplayPerson.CurrentHair = null;
            }

            if (PresetLoaderNPC_F.CurrentHair != null)
            {
                DisplayPerson.DressClothe(PresetLoaderNPC_F.CurrentHair.gameObject);
            }

            for (int index = 0; index < PresetLoaderNPC_F.AllFaceBones.Length; ++index)
            {
                DisplayPerson.AllFaceBones[index].localPosition = PresetLoaderNPC_F.AllFaceBones[index].localPosition;
                DisplayPerson.AllFaceBones[index].localEulerAngles = PresetLoaderNPC_F.AllFaceBones[index].localEulerAngles;
                DisplayPerson.AllFaceBones[index].localScale = PresetLoaderNPC_F.AllFaceBones[index].localScale;
            }

            for (int index = 0; index < PresetLoaderNPC_F.AllBodyBones.Length; ++index)
            {
                DisplayPerson.AllBodyBones[index].localPosition = PresetLoaderNPC_F.AllBodyBones[index].localPosition;
                DisplayPerson.AllBodyBones[index].localEulerAngles = PresetLoaderNPC_F.AllBodyBones[index].localEulerAngles;
                DisplayPerson.AllBodyBones[index].localScale = PresetLoaderNPC_F.AllBodyBones[index].localScale;
            }

            DisplayPerson.Name = "";
            DisplayPerson.PlayerKnowsName = false;
            UnityEngine.Object.Destroy(PresetLoaderNPC_F.gameObject);
        }
        public static GameObject ChangeSkin(string name)
        {
            bool LoadSpecificNPC = true;
            Person PersonGenerated = null;
            bool spawnFemale = Main.Instance.Player is Girl;
            Vector3 position = Main.Instance.Player.transform.position;
            Quaternion rotation = Main.Instance.Player.transform.rotation;
            if (LoadSpecificNPC)
            {
                //PersonGenerated = BasicPerson().gameObject.GetComponent<Person>();
                PersonGenerated = Main.Instance.Player;
                int money = PersonGenerated.Money;
                int sexlevel = PersonGenerated.SexXpThisLvl;
                int sexlevelmax = PersonGenerated.SexXpThisLvlMax;
                int worklevel = PersonGenerated.WorkXpThisLvl;
                int worklevelmax = PersonGenerated.WorkXpThisLvlMax;
                int armylevel = PersonGenerated.ArmyXpThisLvl;
                int armylevelmax = PersonGenerated.ArmyXpThisLvlMax;
                int_Person personInt = Main.Instance.Player.ThisPersonInt;
                Person thisPersonInt = null;
                if (personInt != null && personInt.ThisPerson != null)
                {
                    thisPersonInt = personInt.ThisPerson;
                }
                string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";
                string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";
                string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";
                string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";
                Directory.CreateDirectory(objectsFolder);
                Directory.CreateDirectory(malesFolder);
                Directory.CreateDirectory(femalesFolder);
                Directory.CreateDirectory(audioFolder);
                string maleOrFemale = spawnFemale ? "females" : "males";
                string filename = $"{Main.AssetsFolder}/wolfitdm/{maleOrFemale}/{name}.png";
                if (!File.Exists(filename))
                {
                    Main.Instance.GameplayMenu.ShowNotification(filename + " not exists!");
                    return PersonGenerated.gameObject;
                }
                UpdatePerson(PersonGenerated.gameObject, spawnFemale, filename);
                PersonGenerated.StartingClothes = new List<GameObject>();
                PersonGenerated.StartingWeapons = new List<GameObject>();
                PersonGenerated._StartingClothes = new List<string>();
                PersonGenerated._StartingWeapons = new List<string>();
                PersonGenerated._DontLoadClothing = true;
                PersonGenerated._DontLoadInteraction = true;
                PersonGenerated.LoadFromFile(filename);
                PersonGenerated.Name = "";
                PersonGenerated.PlayerKnowsName = false;
                PersonGenerated.ThisPersonInt = personInt;
                if (PersonGenerated.ThisPersonInt != null && thisPersonInt != null)
                {
                    PersonGenerated.ThisPersonInt.ThisPerson = thisPersonInt;
                }
                PersonGenerated.Money = money;
                PersonGenerated.SexXpThisLvl = sexlevel;
                PersonGenerated.SexXpThisLvlMax = sexlevelmax;
                PersonGenerated.WorkXpThisLvl = worklevel;
                PersonGenerated.WorkXpThisLvlMax = worklevelmax;
                PersonGenerated.ArmyXpThisLvl = armylevel;
                PersonGenerated.ArmyXpThisLvlMax = armylevelmax;
            }

            if (PersonGenerated.WorldSaveID == null)
            {
                PersonGenerated.WorldSaveID = Main.GenerateRandomString(25);
            }
            if (PersonGenerated.SPAWN_noUglyHair == false && PersonGenerated.SPAWN_onlyGoodHair == false)
            {
                PersonGenerated.SPAWN_noUglyHair = false;
                PersonGenerated.SPAWN_onlyGoodHair = true;
            }
            if (PersonGenerated.Home == null)
            {
                PersonGenerated.Home = Main.Instance.PossibleStreetHomes[UnityEngine.Random.Range(0, Main.Instance.PossibleStreetHomes.Count)];
            }
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
            if (PersonGenerated.States == null || PersonGenerated.States.Length < 34)
            {
                PersonGenerated.States = new bool[34];
            }
            PersonGenerated.RefreshColors();
            PersonGenerated.transform.position = position;
            PersonGenerated.transform.rotation = rotation;
            setReverseWildStates(PersonGenerated.gameObject);
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

            addallperkstoperson(personGa);

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
        private static void addItemReal(GameObject playerGa, GameObject item, int value)
        {
            if (playerGa == null)
            {
                return;
            }

            Person player = playerGa.GetComponent<Person>();

            if (player == null)
            {
                return;
            }

            if (player.CurrentBackpack == null)
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
                player.DressClothe(Main.Spawn(backpack2));
            }

            if (player.CurrentBackpack != null && player.CurrentBackpack.ThisStorage != null)
            {
                value = value <= 0 ? 1 : value;

                int count = player.CurrentBackpack.ThisStorage.StorageItems.Count;

                count += value + 10;

                count = count <= 0 ? int.MaxValue : count;

                int storagemax = player.CurrentBackpack.ThisStorage.StorageMax;

                if (count >= storagemax)
                {
                    storagemax = count;
                }

                player.CurrentBackpack.ThisStorage.StorageMax = storagemax;

                for (int i = 0; i < value; i++)
                {
                    player.CurrentBackpack.ThisStorage.AddItem(item);
                }
            }
        }

        private static void spawnItemReal(GameObject item, int value)
        {
            value = value <= 0 ? 1 : value;

            for (int i = 0; i < value; i++)
            {
                SafeSpawn(item);
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

        public static bool active1 = true;
        public static bool active2 = true;

        public static bool collision1 = true;
        public static bool collision2 = true;

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
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj.name}' copied/selected, now you can use the command 'paste' or the command 'toggleactive' to toggle the active state, paste = to spawn the object!");
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj.name}' copied/selected, now you can use the command 'saveobject name' or the command 'toggleactive' to toggle the active state, saveobject = to save the object to file!");
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj.name}' copied/selected, now you can use the command 'toggleactive/togglecollision', to toggle active/collision of this object!");
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
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj2.name}' copied/selected, now you can use the command 'paste2', to spawn the object!");
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj2.name}' copied/selected, now you can use the command 'saveobject2 name', to save the object to file!");
                        }
                        catch (Exception ex)
                        {
                        }

                        try
                        {
                            Main.Instance.GameplayMenu.ShowNotification($"copy: object '{copyObj2.name}' copied/selected, now you can use the command 'toggleactive2/togglecollision2', to toggle active/collision of this object!");
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
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("No target selected, please use the command 'copy' to select/copy a target");
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
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("No target selected, please use the command 'copy' to select/copy a target");
            }
        }

        public static void toggleactive()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: toggleactive");
            if (copyObj != null)
            {
                active1 = !active1;
                copyObj.SetActive(active1);
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"toggleactive: object '{copyObj.name}' is set to " + (active1 ? "active" : "not active"));
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("No target selected, please use the command 'copy' to select/copy a target");
            }
        }
        public static void toggleactive2()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: toggleactive2");
            if (copyObj2 != null)
            {
                active2 = !active2;
                copyObj2.SetActive(active2);
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"toggleactive2: object '{copyObj2.name}' is set to " + (active2 ? "active" : "not active"));
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("No target selected, please use the command 'copy' to select/copy a target");
            }
        }

        public static void togglecollision()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: togglecollision");
            if (copyObj != null)
            {
                collision1 = !collision1;
                Collider collider = copyObj.GetComponent<Collider>();
                if (collider == null)
                {
                    return;
                }
                collider.enabled = collision1;
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"togglecollision: object collision '{copyObj.name}' is set to " + (collision1 ? "on" : "off"));
                }
                catch (Exception ex)
                {
                }
            } else
            {
                Main.Instance.GameplayMenu.ShowNotification("No target selected, please use the command 'copy' to select/copy a target");
            }
        }
        public static void togglecollision2()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: togglecollision2");
            if (copyObj2 != null)
            {
                collision2 = !collision2;
                Collider collider = copyObj2.GetComponent<Collider>();
                if (collider == null)
                {
                    return;
                }
                collider.enabled = collision2;
                try
                {
                    Main.Instance.GameplayMenu.ShowNotification($"togglecollision2: object collision '{copyObj2.name}' is set to " + (collision2 ? "on" : "off"));
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("No target selected, please use the command 'copy' to select/copy a target");
            }
        }

        public static void maxtraining()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxtraining");
            Person player = Main.Instance.Player;
            player.AnalTraining += 10;
            player.VaginalTraining += 10;
            player.NippleTraining += 10;
            player.ClitTraining += 10;
            player.BodyTraining += 10;
            Main.Instance.GameplayMenu.ShowNotification("Increases clit training, anal training, vaginal training, nipple training and body training by 10!");
        }

        public static void npcmaxtraining()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcmaxtraining");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            player.AnalTraining += 10;
            player.VaginalTraining += 10;
            player.NippleTraining += 10;
            player.ClitTraining += 10;
            player.BodyTraining += 10;
            Main.Instance.GameplayMenu.ShowNotification("Increases clit training, anal training, vaginal training, nipple training and body training from the npc the you are looked at by 10!");
        }
        public static void removeallwarps()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: removeallwarps");
            int length = predefinedUserWarpsNames.Count;

            List<string> predefinedUserWarpsNamesTemp = new List<string>();

            for (int i = 0; i < length; i++)
            {
                predefinedUserWarpsNamesTemp.Add(predefinedUserWarpsNames[i]);
            }

            length = predefinedUserWarpsNamesTemp.Count;

            for (int i = 0; i < length; i++)
            {
                string value = predefinedUserWarpsNamesTemp[i];
                if (!spawnpointsNames.Contains(value) || !predefinedUserWarpsNames.Contains(value))
                {
                    continue;
                }

                try
                {
                    warpPointsUser = removeWarpPoint(warpPointsUser, value);
                    safeWarpPoints();
                }
                catch (Exception ex)
                {
                }
            }
            Main.Instance.GameplayMenu.ShowNotification($"removeallwarps: all predefined user warp points removed!");
        }

        public static void analtraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: analtraining");
            Person player = Main.Instance.Player;

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

            float training = player.AnalTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.AnalTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases anal training by amount: " + amount);
        }
        public static void vaginaltraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: vaginaltraining");
            Person player = Main.Instance.Player;

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

            float training = player.VaginalTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.VaginalTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases vaginal training by amount: " + amount);
        }
        public static void nippletraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: nippletraining");
            Person player = Main.Instance.Player;

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

            float training = player.NippleTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.NippleTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases nipple training by amount: " + amount);
        }

        public static void clittraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: clittraining");
            Person player = Main.Instance.Player;

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

            float training = player.ClitTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.ClitTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases clit training by amount: " + amount);
        }
        public static void bodytraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: bodytraining");
            Person player = Main.Instance.Player;

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

            float training = player.BodyTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.BodyTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases body training by amount: " + amount);
        }


        public static void npcanaltraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcanaltraining");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();

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

            float training = player.AnalTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.AnalTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases anal training from the npc the you are looked at by amount: " + amount);
        }
        public static void npcvaginaltraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcvaginaltraining");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();

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

            float training = player.VaginalTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.VaginalTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases vaginal training from the npc the you are looked at by amount: " + amount);
        }
        public static void npcnippletraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcnippletraining");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();

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

            float training = player.NippleTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.NippleTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases nipple training from the npc the you are looked at by amount: " + amount);
        }

        public static void npcclittraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcclittraining");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();

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

            float training = player.ClitTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.ClitTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases clit training from the npc the you are looked at by amount: " + amount);
        }
        public static void npcbodytraining(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcbodytraining");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();

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

            float training = player.BodyTraining;
            training = amount + training;
            training = training <= 0 ? int.MaxValue : training;
            player.BodyTraining = training;
            Main.Instance.GameplayMenu.ShowNotification("Increases body training from the npc the you are looked at by amount: " + amount);
        }

        public static void npcsleep()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcsleep");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            player.Energy = 0;
        }

        public static void shit()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: shit");
            Person player = Main.Instance.Player;
            player.Toilet = player.ToiletMax;
        }

        public static void sleep()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: sleep");
            Person player = Main.Instance.Player;
            player.Energy = 0;
        }

        public static bool infiniteammovar = false;
        public static void infiniteammo()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: infiniteammo");
            Person player = Main.Instance.Player;
            Weapon weapon = player.WeaponInv.CurrentWeapon;
            if (weapon != null)
            {
                infiniteammovar = !infiniteammovar;
                weapon.infiniteAmmo = infiniteammovar;
                weapon.infiniteBeam = infiniteammovar;
                if (infiniteammovar)
                {
                    Main.Instance.GameplayMenu.ShowNotification("infiniteammo: on");
                } else
                {
                    Main.Instance.GameplayMenu.ShowNotification("infiniteammo: off");
                }
            } else
            {
                Main.Instance.GameplayMenu.ShowNotification("infiniteammo: you need a weapon in your main hand in order to use this command!");
            }
        }

        public static void npcshit()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcshit");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            player.Toilet = player.ToiletMax;
        }

        public static void npcinfiniteammo()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcinfiniteammo");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            Weapon weapon = player.WeaponInv.CurrentWeapon;
            if (weapon != null)
            {
                bool infiniteammovar = weapon.infiniteAmmo || weapon.infiniteBeam;
                infiniteammovar = !infiniteammovar;
                weapon.infiniteAmmo = infiniteammovar;
                weapon.infiniteBeam = infiniteammovar;
                if (infiniteammovar)
                {
                    Main.Instance.GameplayMenu.ShowNotification("npcinfiniteammo: on");
                }
                else
                {
                    Main.Instance.GameplayMenu.ShowNotification("npcinfiniteammo: off");
                }
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("npcinfiniteammo: the npc need a weapon in your main hand in order to use this command!");
            }
        }

        public static void npcaddweapon(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcaddweapon");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            GameObject weapon = getWeaponByName(null, value);

            if (weapon != null)
            {
                GameObject weaponx = Main.Spawn(weapon);

                if (weaponx == null)
                {
                    return;
                }

                player.WeaponInv.DropAllWeapons();
                player.WeaponInv.PickupWeapon(weaponx);
            }
        }

        public static void npcadditem(string key, string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcadditem");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
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
                addItemReal(player.gameObject, item, amount);
                Main.Instance.GameplayMenu.ShowNotification("npcadditem: " + item.name.ToString() + " " + amount.ToString() + " added");
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("npcadditem: No item found");
            }
        }


        public static void npcmaxhunger()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcmaxhunger");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            player.Hunger = player.HungerMax;
        }
        public static void npcnohunger()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcnohunger");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            player.Hunger = 0;
        }

        public static void kill(bool forcekill)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: kill");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person player = personGa.GetComponent<Person>();
            if (player.TheHealth != null)
            {
                bool canDie = player.TheHealth.canDie;
                bool alwaysDie = player.TheHealth.AlwaysDie;
                if (!forcekill && !player.TheHealth.canDie)
                {
                    Main.Instance.GameplayMenu.ShowNotification(player.Name + " can not be killed!");
                    return;
                } else
                {
                    player.TheHealth.AlwaysDie = true;
                    player.TheHealth.canDie = true;
                }
                player.Hunger = player.HungerMax;
                player.Toilet = player.ToiletMax;
                player.Energy = 0;
                player.TheHealth.dead = false;
                player.TheHealth.currentHealth = 0.0f;
                player.TheHealth.ChangeHealth(0, true, null);
                player.TheHealth.Die();
                player.TheHealth.canDie = canDie;
                player.TheHealth.AlwaysDie = alwaysDie;
                Main.Instance.GameplayMenu.ShowNotification(player.Name + " killed!");
            }
        }

        public static void killme(bool forcekill)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: killme");
            Person player = Main.Instance.Player;
            if (player.TheHealth != null)
            {
                bool canDie = player.TheHealth.canDie;
                bool alwaysDie = player.TheHealth.AlwaysDie;
                if (!forcekill && !player.TheHealth.canDie)
                {
                    Main.Instance.GameplayMenu.ShowNotification(player.Name + " can not be killed!");
                    return;
                }
                else
                {
                    player.TheHealth.AlwaysDie = true;
                    player.TheHealth.canDie = true;
                }
                player.Hunger = player.HungerMax;
                player.Toilet = player.ToiletMax;
                player.Energy = 0;
                player.TheHealth.dead = false;
                player.TheHealth.currentHealth = 0.0f;
                player.TheHealth.ChangeHealth(0, true, null);
                player.TheHealth.Die();
                player.TheHealth.canDie = canDie;
                player.TheHealth.AlwaysDie = alwaysDie;
                Main.Instance.GameplayMenu.ShowNotification(player.Name + " killed!");
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
            OpenUrl("https://raw.githubusercontent.com/wolfitdm/BitchlandCheatConsoleBepInEx/refs/heads/main/menulist.txt");
            OpenUrl("https://github.com/wolfitdm/BitchlandCheatConsoleBepInEx/releases/tag/v1.0.0");
            Main.Instance.GameplayMenu.ShowNotification("executed command: type all commands and warps without the '* '. it is only for github, to use markdown and list items!");
        }

        public static void dropallass()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: dropallass");

            if (!Main.Instance.Player.Perks.Contains("Anal Storage"))
            {
                Main.Instance.Player.Perks.Add("Anal Storage");
            }

            Person person = Main.Instance.Player;
            Int_Storage ass = person.Storage_Anal;

            if (ass == null)
            {
                return;
            }

            int length = ass.StorageItems.Count;

            for (int i = 0; i < length; i++)
            {
                GameObject ga = ass.StorageItems[i];
                SafeSpawn(ga);
            }

            ass.StorageItems.Clear();
            ass.RemoveAllItems();
            person.Storage_Anal = ass;
        }

        public static void equiptoass()
        {
            if (!Main.Instance.Player.Perks.Contains("Anal Storage"))
            {
                Main.Instance.Player.Perks.Add("Anal Storage");
            }

            Main.Instance.GameplayMenu.ShowNotification("executed command: equiptoass");
            GameObject pi = getPickupToHandInteract();

            if (pi == null)
            {
                return;
            }

            int_PickupToHand pih = pi.GetComponent<int_PickupToHand>();

            if (pih == null)
            {
                return;
            }

            Person person = Main.Instance.Player;
            if (person.Storage_Anal.Full) {
                dropallass();
            }
            pih.EquipToAss(person);
        }
        public static void dropallvag()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: dropallvag");

            if (!Main.Instance.Player.Perks.Contains("Vaginal Storage"))
            {
                Main.Instance.Player.Perks.Add("Vaginal Storage");
            }

            Person person = Main.Instance.Player;
            Int_Storage vag = person.Storage_Vag;

            if (vag == null)
            {
                return;
            }

            int length = vag.StorageItems.Count;

            for (int i = 0; i < length; i++)
            {
                GameObject ga = vag.StorageItems[i];
                SafeSpawn(ga);
            }

            vag.StorageItems.Clear();
            vag.RemoveAllItems();
            person.Storage_Vag = vag;
        }
        public static void equiptovag()
        {
            if (!Main.Instance.Player.Perks.Contains("Vaginal Storage"))
            {
                Main.Instance.Player.Perks.Add("Vaginal Storage");
            }

            Main.Instance.GameplayMenu.ShowNotification("executed command: equiptovag");
            GameObject pi = getPickupToHandInteract();

            if (pi == null)
            {
                return;
            }

            int_PickupToHand pih = pi.GetComponent<int_PickupToHand>();

            if (pih == null)
            {
                return;
            }

            Person person = Main.Instance.Player;

            if (person.Storage_Vag.Full)
            {
                dropallvag();
            }

            pih.EquipToVag(Main.Instance.Player);
        }
        public static void dropallhand()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: dropallhand");

            Person person = Main.Instance.Player;
            Int_Storage hand = person.Storage_Hands;

            if (hand == null)
            {
                return;
            }

            int length = hand.StorageItems.Count;

            for (int i = 0; i < length; i++)
            {
                GameObject ga = hand.StorageItems[i];
                SafeSpawn(ga);
            }

            hand.StorageItems.Clear();
            hand.RemoveAllItems();
            person.Storage_Hands = hand;
        }

        public static void equiptohand()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: equiptohand");
            GameObject pi = getPickupToHandInteract();

            if (pi == null)
            {
                return;
            }

            int_PickupToHand pih = pi.GetComponent<int_PickupToHand>();

            if (pih == null)
            {
                return;
            }

            Person person = Main.Instance.Player;
            
            if (person.Storage_Hands.Full)
            {
                dropallhand();
            }

            pih.EquipToHand(Main.Instance.Player);
        }
        public static void equip()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: equip");
            GameObject pi = getInteract();

            if (pi == null)
            {
                return;
            }

            Interactible pih = pi.GetComponent<Interactible>();

            if (pih == null)
            {
                return;
            }

            Main.Instance.Player.DressClothe(pih.gameObject);
        }

        public static void dropallbackpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: dropallbackpack");

            Person person = Main.Instance.Player;

            if (person.CurrentBackpack == null)
            {
                Main.Instance.GameplayMenu.ShowNotification("dropallbackpack: No backpack equipped!");
                return;
            }

            BackPack backpack = person.CurrentBackpack;
            Int_Storage bp = backpack.ThisStorage;

            if (bp == null)
            {
                return;
            }

            int length = bp.StorageItems.Count;

            for (int i = 0; i < length; i++)
            {
                GameObject ga = bp.StorageItems[i];
                SafeSpawn(ga);
            }

            bp.StorageItems.Clear();
            bp.RemoveAllItems();
            backpack.ThisStorage = bp;
            person.CurrentBackpack = backpack;
        }

        public static void equiptobackpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: equiptobackpack");

            Person person = Main.Instance.Player;

            if (person.CurrentBackpack == null)
            {
                Main.Instance.GameplayMenu.ShowNotification("equiptobackpack: No backpack equipped!");
                return;
            }

            GameObject ga = getInteract();

            if (ga == null)
            {
                return;
            }

            GameObject pi = getPickupToBagInteract(ga);

            if (pi == null)
            {
                Main.Instance.GameplayMenu.ShowNotification("equiptobackpack: No backpack equipped!");
                return;
            }

            int_PickupToBag pih = pi.GetComponent<int_PickupToBag>();

            if (pih == null)
            {
                Main.Instance.GameplayMenu.ShowNotification("equiptobackpack: No backpack equipped!");
                return;
            }
            pih.Interact(person);
            Main.Instance.GameplayMenu.ShowNotification("equiptobackpack: " + ga.name.ToString() + " 1 added");
        }
        public static void addbackpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: addbackpack");
            additem("jeans", "1");
        }
        public static void use()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: use");

            Person person = Main.Instance.Player;

            GameObject ga = getInteract();

            if (ga == null)
            {
                return;
            }

            GameObject[] gas = getChildren(ga);

            if (gas == null)
            {
                return;
            }

            for (int i = 0; i < gas.Length; i++)
            {
                GameObject gass = gas[i];
                if (gass == null)
                {
                    continue;
                }

                Interactible gasInt = gass.GetComponent<Interactible>();

                if (gasInt == null)
                {
                    continue;
                }

                if (gasInt is int_PickupToBag)
                {
                    continue;
                }
                else if (gasInt is int_PickupToHand)
                {
                    continue;
                }
                else if (gasInt is int_dildo)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is int_money)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is int_food)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is Int_Pickupable)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is int_PickableClothingPackage)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is int_Piss)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is int_basicSit)
                {
                    gasInt.Interact(person);
                }
                else if (gasInt is int_ResourceItem)
                {
                    gasInt.Interact(person);
                }
                else
                {
                    gasInt.Interact(person);
                }
            }
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

        public static void changeskin(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: changeskin");
            Person s = null;
            switch (value)
            {
                case "jeanne":
                    {
                        s = ChangeSkin("jeanne").GetComponent<Person>();
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
                        s = ChangeSkin("sarahoffwork").GetComponent<Person>();
                        if (s != null)
                        {
                            s.DressClothe(Main.Instance.AllPrefabs[3]);
                            s.DressClothe(Main.Instance.AllPrefabs[14]);
                        }
                    }
                    break;

                case "uniformedsarah":
                    {
                        s = ChangeSkin("uniformedsarah").GetComponent<Person>();
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
                        s = ChangeSkin("nameless").GetComponent<Person>();
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
                        s = ChangeSkin("rit").GetComponent<Person>();
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
                        s = ChangeSkin("carol").GetComponent<Person>();
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
                        s = ChangeSkin("beth").GetComponent<Person>();
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
                        s = ChangeSkin(value).GetComponent<Person>();
                    }
                    break;
            }
        }
        public static void changeskinnude(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: changeskinnude");
            Person s = ChangeSkin(value).GetComponent<Person>();
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
            } else
            {
                int psState = 0;
                try
                {
                    psState = (int)thisPerson.State;
                } catch(Exception ex)
                {
                    psState = 0;
                }

                Main.Instance.GameplayMenu.ShowNotification($"getpersonstate: person state is '{psState}'");
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
            } else
            {
                int psState = 0;
                try
                {
                    psState = (int)thisPerson.State;
                }
                catch (Exception ex)
                {
                    psState = 0;
                }
                thisPerson.State = Person_State.Free;
                Main.Instance.GameplayMenu.ShowNotification($"togglepersonstate: person state was '{psState}', now person state is free");
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
            else
            {
                int psState = 0;
                try
                {
                    psState = (int)thisPerson.State;
                }
                catch (Exception ex)
                {
                    psState = 1;
                }
                thisPerson.State = Person_State.Work;
                Main.Instance.GameplayMenu.ShowNotification($"setpersonstatetowork: person state was '{psState}', now person state is work");
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
            } else
            {
                int psState = 0;
                try
                {
                    psState = (int)thisPerson.State;
                }
                catch (Exception ex)
                {
                    psState = 0;
                }
                thisPerson.State = Person_State.Free;
                Main.Instance.GameplayMenu.ShowNotification($"setpersonstatetofree: person state was '{psState}', now person state is free");
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
                GameObject personGa = getPersonInteract();
                if (personGa != null)
                {
                    Main.Instance.GameplayMenu.ShowNotification("unstuck me from the chat");
                    Person person = personGa.GetComponent<Person>();
                    if (person != null)
                    {
                        Main.Instance.GameplayMenu.ShowNotification("unstuck me from the chat really");
                        int_Person personInt = person.ThisPersonInt;
                        if (personInt != null)
                        {
                            Main.Instance.GameplayMenu.ShowNotification("unstuck me from the chat really really");
                            personInt.EndTheChat();
                        }
                    }
                }
            } catch(Exception ex)
            {
            }

            try
            {
                Main.Instance.Player.UserControl.enabled = true;
            }
            catch
            {
            }

            try
            {
                Main.Instance.Player.UserControl.ThirdCamPositionType = Main.Instance.Player.UserControl.ThirdCamPositionTypeOnSettings;
            }
            catch
            {
            }
           
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

        public static void npcpregnancy()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcpregnancy");

            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            bool isNotAGirl = person is Girl;
            isNotAGirl = !isNotAGirl;

            if (isNotAGirl)
            {
                Main.Instance.GameplayMenu.ShowNotification("NPC is a male!");
                return;
            }

            float fertility = person.Fertility;
            float storymodefertility = person.StoryModeFertility;
            person.Fertility *= 1000;
            person.StoryModeFertility *= 1000;
            (person as Girl).BecomePreg();
            person.Fertility = fertility;
            person.StoryModeFertility = storymodefertility;
        }


        public static void infinitehealth()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: infinitehealth");
            Main.Instance.Player.CantBeHit = !Main.Instance.Player.CantBeHit;
            if (Main.Instance.Player.CantBeHit)
            {
                Main.Instance.Player.NoEnergyLoss = true;
                if (Main.Instance.Player.TheHealth != null)
                {
                    Main.Instance.Player.TheHealth.canDie = false;
                }
                Main.Instance.GameplayMenu.ShowNotification("infinitehealth: on");
            }
            else
            {
                Main.Instance.Player.NoEnergyLoss = false;
                if (Main.Instance.Player.TheHealth != null)
                {
                    Main.Instance.Player.TheHealth.canDie = true;
                }
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
        public static void maxhunger()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: maxhunger");
            Main.Instance.Player.Hunger = Main.Instance.Player.HungerMax;
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
        public static void npcfullenergy()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcfullenergy");

            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            person.Energy = person.EnergyMax;

            Main.Instance.GameplayMenu.ShowNotification("npcfullenergy: set the npc to full energy");
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

        public static void addallperkstoperson(GameObject personGa)
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

            if (!person.Perks.Contains("Gaping"))
            {
                person.Perks.Add("Gaping");
            }

            if (!person.Perks.Contains("Smell"))
            {
                person.Perks.Add("Smell");
            }

            if (!person.Perks.Contains("Vaginal Storage"))
            {
                person.Perks.Add("Vaginal Storage");
            }

            if (!person.Perks.Contains("Anal Storage"))
            {
                person.Perks.Add("Anal Storage");
            }

            if (!person.Perks.Contains("Mining Skill lvl 2"))
            {
                person.Perks.Add("Mining Skill lvl 2");
            }

            if (!person.Perks.Contains("Sensetivity"))
            {
                person.Perks.Add("Sensetivity");
            }

            if (!person.Perks.Contains("Longer Orgasm"))
            {
                person.Perks.Add("Longer Orgasm");
            }

            if (!person.Perks.Contains("Prostitution skill lvl 1"))
            {
                person.Perks.Add("Prostitution skill lvl 1");
            }

            if (!person.Perks.Contains("Prostitution skill lvl 2"))
            {
                person.Perks.Add("Prostitution skill lvl 2");
            }

            if (!person.Perks.Contains("Prostitution skill lvl 3"))
            {
                person.Perks.Add("Prostitution skill lvl 3");
            }

            if (!person.Perks.Contains("Prostitution skill lvl 4"))
            {
                person.Perks.Add("Prostitution skill lvl 4");
            }

            if (!person.Perks.Contains("Fluid Gather"))
            {
                person.Perks.Add("Fluid Gather");
            }

            if (!person.Perks.Contains("Trash3"))
            {
                person.Perks.Add("Trash3");
            }
        }

        public static void addallperks()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: addallperks");
            Person person = Main.Instance.Player;
            addallperkstoperson(person.gameObject);
        }
        public static void npcaddallperks()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcaddallperks");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            addallperkstoperson(person.gameObject);
        }

        public static void cleanskin()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: cleanskin");
            Person _this = Main.Instance.Player;
            setCleanSkinStates(_this.gameObject);
        }

        public static void collisionon()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: collisonon");
            Person _this = Main.Instance.Player;
            _this._Rigidbody.detectCollisions = true;
        }
        public static void collisionoff()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: collisonoff");
            Person _this = Main.Instance.Player;
            _this._Rigidbody.detectCollisions = false;
        }

        public static void npccleanskin()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npccleanskin");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            setNudeStates(person.gameObject);

            setNudeClothesPoints(person.gameObject);

            setCleanSkinStates(person.gameObject);

            Main.Instance.GameplayMenu.ShowNotification("npcleanskin: npc cleaned");
        }
        public static void closemenus()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: closemenus");
            Main.Instance.CloseMenus();
        }

        public static void listmenus()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: listmenus");
            string menus = "menus: \n";
            Main.Instance.GameplayMenu.ShowNotification(menus);
            for (int index = 0; index < Main.Instance.Menus.Count; ++index)
            {
                string menuname = Main.Instance.Menus[index].MenuName.ToLower().Replace(" ", "_");
                menus += menuname + "\n";
                Main.Instance.GameplayMenu.ShowNotification(menuname);
            }
            Logger.LogInfo(menus);
        }

        public static void openmenu(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: openmenu");
            for (int index = 0; index < Main.Instance.Menus.Count; ++index)
            {
                string menuname = Main.Instance.Menus[index].MenuName.ToLower().Replace(" ", "_");
                if (menuname == value)
                {
                    Main.Instance.OpenMenu(Main.Instance.Menus[index].MenuName);
                    break;
                }
            }
        }

        public static void closemenu(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: closemenu");
            for (int index = 0; index < Main.Instance.Menus.Count; ++index)
            {
                string menuname = Main.Instance.Menus[index].MenuName.ToLower().Replace(" ", "_");
                if (menuname == value)
                {
                    Main.Instance.Menus[index].Close();
                    break;
                }
            }
        }
        public static void heal()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: heal");
            Main.Instance.Player.TheHealth.currentHealth = Main.Instance.Player.TheHealth.maxHealth;
            Main.Instance.GameplayMenu.UpdateHealth();
        }

        public static void npcfullheal()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcfullheal");
            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            person.TheHealth.currentHealth = person.TheHealth.maxHealth;
            Main.Instance.GameplayMenu.ShowNotification("npcfullheal: npc healed");
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
        public static void clearbackpackold()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: clearbackpack");
            if (Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null)
            {
                Main.Instance.Player.CurrentBackpack.ThisStorage.RemoveAllItems();
            }
        }

        public static void clearbackpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: clearbackpack");
            dropallbackpack();
        }

        public static void clearbackpacknew()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: clearbackpacknew");
            if (Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null)
            {
                Main.Instance.Player.CurrentBackpack.ThisStorage.StorageItems.Clear();
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

                case "clearbackpackold":
                    {
                        clearbackpackold();
                    }
                    break;

                case "clearbackpacknew":
                    {
                        clearbackpacknew();
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

                case "becomepreg":
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

                case "changeskin":
                    {
                        changeskin("nameless");
                    }
                    break;

                case "changeskinnude":
                    {
                        changeskinnude("brat");
                    }
                    break;

                case "toggleactive":
                    {
                        toggleactive();
                    }
                    break;

                case "toggleactive2":
                    {
                        toggleactive2();
                    }
                    break;

                case "npcfullenergy":
                    {
                        npcfullenergy();
                    }
                    break;

                case "maxhunger":
                    {
                        maxhunger();
                    }
                    break;

                case "npccleanskin":
                    {
                        npccleanskin();
                    }
                    break;

                case "npcfullheal":
                    {
                        npcfullheal();
                    }
                    break;

                case "closemenus":
                    {
                        closemenus();
                    }
                    break;

                case "listmenus":
                    {
                        listmenus();
                    }
                    break;

                case "collisionoff":
                    {
                        collisionoff();
                    }
                    break;

                case "collisionon":
                    {
                        collisionon();
                    }
                    break;

                case "togglecollision":
                    {
                        togglecollision();
                    }
                    break;

                case "togglecollision2":
                    {
                        togglecollision2();
                    }
                    break;

                case "maxtraining":
                    {
                        maxtraining();
                    }
                    break;

                case "npcmaxtraining":
                    {
                        maxtraining();
                    }
                    break;

                case "analtraining":
                    {
                        analtraining("10");
                    }
                    break;

                case "npcanaltraining":
                    {
                        npcanaltraining("10");
                    }
                    break;

                case "vaginaltraining":
                    {
                        vaginaltraining("10");
                    }
                    break;

                case "npcvaginaltraining":
                    {
                        npcvaginaltraining("10");
                    }
                    break;

                case "nippletraining":
                    {
                        nippletraining("10");
                    }
                    break;

                case "npcnippletraining":
                    {
                        npcnippletraining("10");
                    }
                    break;

                case "clittraining":
                    {
                        clittraining("10");
                    }
                    break;

                case "npcclittraining":
                    {
                        npcclittraining("10");
                    }
                    break;

                case "bodytraining":
                    {
                        bodytraining("10");
                    }
                    break;

                case "npcbodytraining":
                    {
                        npcbodytraining("10");
                    }
                    break;

                case "removeallwarps":
                    {
                        removeallwarps();
                    }
                    break;

                case "kill":
                    {
                        kill(false);
                    }
                    break;

                case "forcekill":
                case "die":
                    {
                        kill(true);
                    }
                    break;

                case "npcshit":
                    {
                        npcshit();
                    }
                    break;

                case "npcsleep":
                    {
                        npcsleep();
                    }
                    break;

                case "npcnohunger":
                    {
                        npcnohunger();
                    }
                    break;

                case "npcmaxhunger":
                    {
                        npcmaxhunger();
                    }
                    break;

                case "npcinfiniteammo":
                    {
                        npcinfiniteammo();
                    }
                    break;

                case "shit":
                    {
                        shit();
                    }
                    break;

                case "sleep":
                    {
                        sleep();
                    }
                    break;

                case "infiniteammo":
                    {
                        infiniteammo();
                    }
                    break;

                case "npcbecomepreg":
                case "npcrealpregnancy":
                case "npcpregnancy":
                    {
                        npcpregnancy();
                    }
                    break;

                case "npcsetfavor":
                    {
                        npcsetfavor("10000000");
                    }
                    break;

                case "killme":
                    {
                        killme(false);
                    }
                    break;

                case "forcekillme":
                case "dieme":
                    {
                        killme(true);
                    }
                    break;

                case "fly":
                    {
                        fly();
                    }
                    break;

                case "equip":
                    {
                        equip();
                    }
                    break;

                case "equiptohand":
                    {
                        equiptohand();
                    }
                    break;

                case "dropallhand":
                    {
                        dropallhand();
                    }
                    break;

                case "equiptovagina":
                case "equiptovag":
                    {
                        equiptovag();
                    }
                    break;

                case "dropallvagina":
                case "dropallvag":
                    {
                        dropallvag();
                    }
                    break;

                case "equiptoass":
                    {
                        equiptoass();
                    }
                    break;

                case "dropallass":
                    {
                        dropallass();
                    }
                    break;

                case "addbp":
                case "addbackpack":
                    {
                        addbackpack();
                    }
                    break;

                case "dropallbp":
                case "dropallbackpack":
                    {
                        equiptobackpack();
                    }
                    break;

                case "equiptobp":
                case "equiptobackpack":
                    {
                        equiptobackpack();
                    }
                    break;

                case "addallperks":
                    {
                        addallperks();
                    }
                    break;

                case "npcaddallperks":
                    {
                        npcaddallperks();
                    }
                    break;

                case "use":
                    {
                        use();
                    }
                    break;

                case "stuckland":
                    {
                        openmenu("loadgame");
                    }
                    break;

                case "playaudio":
                    {
                        playaudio("slave.mp3");
                    }
                    break;

                case "playaudioloop":
                    {
                        playaudio("slave.mp3", true);
                    }
                    break;

                case "stopaudio":
                    {
                        stopaudio();
                    }
                    break;

                case "listaudio":
                    {
                        listaudio();
                    }
                    break;

                case "pauseaudio":
                    {
                        pauseaudio();
                    }
                    break;

                case "unpauseaudio":
                    {
                        unpauseaudio();
                    }
                    break;

                case "checkupdate":
                case "autoupdate":
                    {
                        help();
                    }
                    break;

                case "helloworld":
                    {
                        Main.Instance.GameplayMenu.ShowMessageBox("hello world");
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

                case "spawnitem":
                    {
                        spawnitem(value, "1");
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

                case "changeskin":
                    {
                        changeskin(value);
                    }
                    break;

                case "changeskinnude":
                    {
                        changeskinnude(value);
                    }
                    break;

                case "openmenu":
                    {
                        openmenu(value);
                    }
                    break;

                case "closemenu":
                    {
                        closemenu(value);
                    }
                    break;

                case "analtraining":
                    {
                        analtraining(value);
                    }
                    break;

                case "npcanaltraining":
                    {
                        npcanaltraining(value);
                    }
                    break;

                case "vaginaltraining":
                    {
                        vaginaltraining(value);
                    }
                    break;

                case "npcvaginaltraining":
                    {
                        npcvaginaltraining(value);
                    }
                    break;

                case "nippletraining":
                    {
                        nippletraining(value);
                    }
                    break;

                case "npcnippletraining":
                    {
                        npcnippletraining(value);
                    }
                    break;

                case "clittraining":
                    {
                        clittraining(value);
                    }
                    break;

                case "npcclittraining":
                    {
                        npcclittraining(value);
                    }
                    break;

                case "bodytraining":
                    {
                        bodytraining(value);
                    }
                    break;

                case "npcbodytraining":
                    {
                        npcbodytraining(value);
                    }
                    break;

                case "npcaddweapon":
                    {
                        npcaddweapon(value);
                    }
                    break;

                case "npcsetfavor":
                    {
                        npcsetfavor(value);
                    }
                    break;

                case "playaudio":
                    {
                        playaudio(value);
                    }
                    break;

                case "playaudioloop":
                    {
                        playaudio(value, true);
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

                case "npcadditem":
                    {
                        npcadditem(key, value);
                    }
                    break;

                case "spawnitem":
                    {
                        spawnitem(key, value);
                    }
                    break;

                default:
                    {
                        Main.Instance.GameplayMenu.ShowNotification("No command");
                    }
                    break;
            }
        }

        public static IEnumerator GetAudioClip(string url, Action<AudioClip> callback, AudioType type)
        {
            Logger.LogInfo("execute GetAudioClip");
            if (callback == null) { yield break; }
            Logger.LogInfo("execute GetAudioClip 1");
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, type))
            {
                Logger.LogInfo("execute GetAudioClip 2");
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogError(uwr.error);
                    yield break;
                }
                Logger.LogInfo("execute GetAudioClip 3");
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                Logger.LogInfo("Loaded clip");
                // use audio clip
                callback(clip);
            }
        }
        public static void unpauseaudio()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: unpauseaudio");

            initAudioSource();

            AudioSource audioSource = BitchlandCheatConsoleBepInEx.audioSource;

            if (audioSource != null)
            {
                audioSource.UnPause();

            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("unpauseaudio: no audio source to unpause audio!");
            }
        }
        public static void pauseaudio()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: pauseaudio");

            initAudioSource();

            AudioSource audioSource = BitchlandCheatConsoleBepInEx.audioSource;

            if (audioSource != null)
            {
                audioSource.Pause();

            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("pauseaudio: no audio source to pause audio!");
            }
        }
        public static void stopaudio()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: stopaudio");

            initAudioSource();

            AudioSource audioSource = BitchlandCheatConsoleBepInEx.audioSource;

            if (audioSource != null)
            {
                audioSource.Stop();

            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("stopaudio: no audio source to stop audio!");
            }
        }

        public static void listaudio()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: playaudio");

            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            Directory.CreateDirectory(audioFolder);

            string[] files = Directory.GetFiles(audioFolder);

            if (files == null || files.Length == 0)
            {
                string fullFile = "listaudio: No files found in Assets/wolfitdm/audio";
                Main.Instance.GameplayMenu.ShowNotification(fullFile);
                Logger.LogInfo(fullFile);
                return;
            }

            int length = files.Length;

            for (int i = 0; i < length; i++)
            {
                string file = Path.GetFileName(files[i]);

                string fullFile = "listaudio: Assets/wolfitdm/audio/" + file;
                Main.Instance.GameplayMenu.ShowNotification(fullFile);
                Logger.LogInfo(fullFile);
            }
        }
        public static void playaudio(string value, bool loop = false)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: playaudio");

            string objectsFolder = $"{Main.AssetsFolder}/wolfitdm/objects";

            string malesFolder = $"{Main.AssetsFolder}/wolfitdm/males";

            string femalesFolder = $"{Main.AssetsFolder}/wolfitdm/females";

            string audioFolder = $"{Main.AssetsFolder}/wolfitdm/audio";

            Directory.CreateDirectory(objectsFolder);

            Directory.CreateDirectory(malesFolder);

            Directory.CreateDirectory(femalesFolder);

            Directory.CreateDirectory(audioFolder);

            string audioFile = $"{audioFolder}/{value}";

            string realAudioFile = audioFile;

            AudioType audioType = AudioType.MPEG;

            if (!File.Exists(realAudioFile))
            {
                realAudioFile = audioFile + ".mp3";
                audioType = AudioType.MPEG;
            }

            if (!File.Exists(realAudioFile))
            {
                realAudioFile = audioFile + ".ogg";
                audioType = AudioType.OGGVORBIS;
            }

            if (!File.Exists(realAudioFile))
            {
                realAudioFile = audioFile + ".wav";
                audioType = AudioType.WAV;
            }

            if (!File.Exists(realAudioFile))
            {
                Main.Instance.GameplayMenu.ShowNotification("playaudio: audio file not found!");
            }

            string fileUri = $"file:///{realAudioFile}";

            Logger.LogInfo("playaudio: path: " +  fileUri);

            initAudioSource();
       
            AudioSource audioSource = BitchlandCheatConsoleBepInEx.audioSource;

            if (audioSource != null)
            {
                Logger.LogInfo("playaudio: execute GetAudioClip");
                Action<AudioClip> onAudioLoad = (loadedClip) => {
                    Logger.LogInfo("Clip geladen:  " + loadedClip.name);
                  // Jetzt kannst du etwas mit dem geladenen Clip machen
                  audioSource.loop = loop;
                  audioSource.playOnAwake = false;
                  audioSource.clip = loadedClip;
                  audioSource.Play();
              };
              Main.Instance.Player.StartCoroutine(GetAudioClip(fileUri, onAudioLoad, audioType));

            }
            else
            {
              Main.Instance.GameplayMenu.ShowNotification("playaudio: no audio source to play audio!");
            }
               
        }

        public static void fly()
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: fly");
            if (fly_rb == null)
            {
                if (!is_free_fly_setup)
                {
                    default_useGravity = Main.Instance.Player._Rigidbody.useGravity;
                    is_free_fly_setup = true;
                }
                fly_rb = Main.Instance.Player._Rigidbody;
            }
            fly_on = !fly_on;
            if (fly_on)
            {
                if (fly_disableGravityWhileFlying)
                {
                    fly_rb.useGravity = false;
                }
                Main.Instance.GameplayMenu.ShowNotification("fly: on");
            } else
            {
                fly_rb.useGravity = default_useGravity;
                Main.Instance.GameplayMenu.ShowNotification("fly: off");
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

            Main.Instance.Player.Favor = amount;

            if (Main.Instance.GameplayMenu.Relationships != null)
            {
                int length = Main.Instance.GameplayMenu.Relationships.Count;
                for (int i = 0; i < length; i++)
                {
                    Main.Instance.GameplayMenu.Relationships[i].Favor = amount;
                }
            }
        }

        public static void npcsetfavor(string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: npcsetfavor");

            GameObject personGa = getPersonInteract();

            if (personGa == null)
            {
                return;
            }

            Person person = personGa.GetComponent<Person>();

            if (person == null)
            {
                return;
            }

            person.CreatePersonRelationship();

            int amount = 0;
            try
            {
                amount = int.Parse(value);
            }
            catch (Exception ex)
            {
                amount = 0;
            }

            person.Favor = amount;
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
                addItemReal(Main.Instance.Player.gameObject, item, amount);
                Main.Instance.GameplayMenu.ShowNotification("additem: " + item.name.ToString() + " " + amount.ToString() + " added");
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("additem: No item found");
            }
        }

        public static void spawnitem(string key, string value)
        {
            Main.Instance.GameplayMenu.ShowNotification("executed command: spawnitem");
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
                spawnItemReal(item, amount);
                Main.Instance.GameplayMenu.ShowNotification("spawnitem: " + item.name.ToString() + " " + amount.ToString() + " spawned");
            }
            else
            {
                Main.Instance.GameplayMenu.ShowNotification("spawnitem: No item found");
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
        */
        /*
        [HarmonyPatch(typeof(WeaponSystem), "ShowPromptFor")]
        [HarmonyPrefix]
        public static bool ShowPromptFor(Interactible interactible, object __instance)
        {
            WeaponSystem _this = (WeaponSystem)__instance;
            _this.IntLookingAt = interactible;
            Main.Instance.GameplayMenu.Crossair.SetActive(value: true);
            Main.Instance.GameplayMenu.PickupText.text = interactible.InteractText + " HALLO ";
            Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[interactible.InteractIcon];
            Main.Instance.GameplayMenu.PromptIcon.enabled = interactible.InteractIcon != 0;
            Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: false);
            if (interactible is MultiInteractible)
            {
                MultiInteractible multiInteractible = (MultiInteractible)interactible;
                if (multiInteractible.NewMulti)
                {
                    Interactible interactible2 = null;
                    for (int i = 1; i < multiInteractible.Parts.Length; i++)
                    {
                        multiInteractible.Parts[i].InteractText = multiInteractible.Parts[i].InteractText + " SUSPS";
                        if (multiInteractible.Parts[i] != null && multiInteractible.Parts[i].CheckCanInteract(Main.Instance.Player))
                        {
                            interactible2 = multiInteractible.Parts[i];
                            break;
                        }
                    }

                    if (interactible2 != null)
                    {
                        Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: true);
                        Main.Instance.GameplayMenu.NewMultiOption_text.text = "More options";
                        if (Input.GetKeyUp(KeyCode.F))
                        {
                            Main.Instance.GameplayMenu.Crossair.SetActive(value: false);
                            Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                            Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                            Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                            Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: false);
                            interactible2.Interact(_this.ThisPerson);
                        }
                    }
                }
                else if (Main.Instance.PeopleFollowingPlayer.Count > 0 && multiInteractible.Parts[0].NPCCanUseInFollow)
                {
                    if (Main.Instance.PeopleFollowingPlayer[0].InteractingWith != null)
                    {
                        Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: true);
                        Main.Instance.GameplayMenu.NewMultiOption_text.text = "Ask to stop using";
                        if (Input.GetKeyUp(KeyCode.F))
                        {
                            Main.Instance.GameplayMenu.Crossair.SetActive(value: false);
                            Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                            Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                            Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                            Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: false);
                            Interactible interactingWith = Main.Instance.PeopleFollowingPlayer[0].InteractingWith;
                            interactingWith.InteractingPerson = Main.Instance.PeopleFollowingPlayer[0];
                            interactingWith.StopInteracting();
                            interactingWith.InteractingPerson = null;
                            Main.Instance.PeopleFollowingPlayer[0].InteractingWith = null;
                        }
                    }
                    else
                    {
                        Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: true);
                        Main.Instance.GameplayMenu.NewMultiOption_text.text = "Ask " + Main.Instance.PeopleFollowingPlayer[0].Name + " to use";
                        if (Input.GetKeyUp(KeyCode.F))
                        {
                            Main.Instance.GameplayMenu.Crossair.SetActive(value: false);
                            Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                            Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                            Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                            Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: false);
                            multiInteractible.Parts[0].Interact(Main.Instance.PeopleFollowingPlayer[0]);
                        }
                    }
                }
            }
            else if (Main.Instance.PeopleFollowingPlayer.Count > 0 && interactible.NPCCanUseInFollow)
            {
                if (Main.Instance.PeopleFollowingPlayer[0].InteractingWith != null)
                {
                    Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: true);
                    Main.Instance.GameplayMenu.NewMultiOption_text.text = "Ask to stop using";
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        Main.Instance.GameplayMenu.Crossair.SetActive(value: false);
                        Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                        Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                        Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                        Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: false);
                        Interactible interactingWith2 = Main.Instance.PeopleFollowingPlayer[0].InteractingWith;
                        interactingWith2.InteractingPerson = Main.Instance.PeopleFollowingPlayer[0];
                        interactingWith2.StopInteracting();
                        interactingWith2.InteractingPerson = null;
                        Main.Instance.PeopleFollowingPlayer[0].InteractingWith = null;
                    }
                }
                else
                {
                    Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: true);
                    Main.Instance.GameplayMenu.NewMultiOption_text.text = "Ask " + Main.Instance.PeopleFollowingPlayer[0].Name + " to use";
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        Main.Instance.GameplayMenu.Crossair.SetActive(value: false);
                        Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                        Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                        Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                        Main.Instance.GameplayMenu.NewMultiOption.SetActive(value: false);
                        Main.Instance.PeopleFollowingPlayer[0].InteractingWith = interactible;
                        interactible.Interact(Main.Instance.PeopleFollowingPlayer[0]);
                    }
                }
            }

            if (Input.GetButtonUp("Interact") || Input.GetMouseButtonUp(UI_Settings.RightMouseButton))
            {
                Main.Instance.GameplayMenu.Crossair.SetActive(value: false);
                Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                Main.Instance.GameplayMenu.PromptIcon.enabled = false;

                if (interactible is MultiInteractible)
                {
                    MultiInteractible multiInteractible = (MultiInteractible)interactible;
                    for (int i = 0; i < multiInteractible.Parts.Length; i++)
                    {
                        string interactText = multiInteractible.Parts[i].InteractText;
                        interactText = interactText != null ? interactText.ToLower().Replace(" ", "_") : "";
                        if (multiInteractible.Parts[i] is int_PickupToBag)
                        {
                            interactText = "int_PickupToBag";
                        } else if (multiInteractible.Parts[i] is int_PickupToHand)
                        {
                            interactText = "int_PickupToHand";
                        }
                        else if (multiInteractible.Parts[i] is int_useWeapon)
                        {
                            interactText = "int_useWeapon";
                        } else if (multiInteractible.Parts[i] is Int_Pickupable)
                        {
                            interactText = "Int_Pickupable";
                        }
                        else if (multiInteractible.Parts[i] is int_dildo)
                        {
                            interactText = "int_dildo";
                        }
                        else if (multiInteractible.Parts[i] is int_food)
                        {
                            interactText = "int_food";
                        }
                        else if (multiInteractible.Parts[i] is int_money)
                        {
                            interactText = "int_money";
                        } else if (multiInteractible.Parts[i] is int_ResourceItem)
                        { 
                           interactText = "int_ResourceItem";
                        }
                        else if (multiInteractible.Parts[i] is int_PickableClothingPackage)
                        {
                            interactText = "int_PickableClothingPackage";
                        }
                    multiInteractible.Parts[i].InteractText = interactText;
                    }
                }
                interactible.Interact(_this.ThisPerson);
            }
            return false;
        }

        [HarmonyPatch(typeof(WeaponSystem), "Update")]
        [HarmonyPrefix]
        public static bool UpdateWeaponSystem(object __instance)
        {
            int x = 1;
            WeaponSystem _this = (WeaponSystem)__instance;
            if (!_this.isPlayer || _this.ThisPerson.Interacting || !_this.ThisPerson.CanMove)
                return false;
            if (Input.GetKeyUp(KeyCode.Alpha1))
                _this.SetActiveWeapon(0);
            if (Input.GetKeyUp(KeyCode.Alpha2) && _this.weapons.Count > 1)
                _this.SetActiveWeapon(1);
            if (Input.GetKeyUp(KeyCode.Alpha3) && _this.weapons.Count > 2)
                _this.SetActiveWeapon(2);
            if (Input.GetButtonUp("Drop"))
            {
                _this.DropWeapon(_this.weaponIndex);
            }
            else
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(_this.transform.position, _this.transform.TransformDirection(Vector3.forward), out hitInfo, _this.RayDistance, (int)_this.PromptLayers))
                {
                    int_Dragable component1 = hitInfo.transform.GetComponent<int_Dragable>();
                    if (component1 != null && component1.CanInteract && Input.GetKeyDown(KeyCode.Z))
                    {
                        component1.Interact(Main.Instance.Player);
                        return false;
                    }
                    Weapon component2 = hitInfo.transform.root.GetComponent<Weapon>();
                    if (component2 == null)
                        component2 = hitInfo.transform.GetComponent<Weapon>();
                    if (component2 != null)
                    {
                        Main.Instance.GameplayMenu.Crossair.SetActive(true);
                        Main.Instance.GameplayMenu.PickupText.text = x.ToString() + " " + "Pickup " + component2.transform.name;
                        x++;
                        Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                        Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                        if (Input.GetButtonUp("Interact") || Input.GetMouseButtonUp(UI_Settings.RightMouseButton))
                        {
                            Main.Instance.GameplayMenu.Crossair.SetActive(false);
                            _this.PickupWeapon(component2.gameObject);
                            Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                            Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                            Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                        }
                        if (!Input.GetKeyDown(KeyCode.Z))
                            return false;
                        component2.int_Drag.Interact(Main.Instance.Player);
                        return false;
                    }
                    Interactible component3 = hitInfo.transform.GetComponent<Interactible>();
                    if (component3 != null && (component3.CanInteract && component3.PlayerCanInteract || Main.Instance.PeopleFollowingPlayer.Count > 0 && component3.InteractingPerson == Main.Instance.PeopleFollowingPlayer[0]))
                    { 
                        component3.InteractText = "(2) " + component3.InteractText;
                        _this.ShowPromptFor(component3);
                        return false;
                    }
                    InteractRedirect component4 = hitInfo.transform.GetComponent<InteractRedirect>();
                    if (component4 != null && !component4.Disabled && (component4.Redirect.CanInteract && component4.Redirect.PlayerCanInteract || Main.Instance.PeopleFollowingPlayer.Count > 0 && component4.Redirect.InteractingPerson == Main.Instance.PeopleFollowingPlayer[0]))
                    {
                        component4.Redirect.InteractText = "(3) " + component4.Redirect.InteractText;
                        _this.ShowPromptFor(component4.Redirect);
                        return false;
                    }
                    Interactible component5 = hitInfo.transform.root.GetComponent<Interactible>();
                    if (component5 != null && (component5.CanInteract && component5.PlayerCanInteract || Main.Instance.PeopleFollowingPlayer.Count > 0 && component5.InteractingPerson == Main.Instance.PeopleFollowingPlayer[0]))
                    {
                        component5.InteractText = "(4) " + component5.InteractText;
                        _this.ShowPromptFor(component5);
                        return false;
                    }
                }
                _this.IntLookingAt = (Interactible)null;
                Main.Instance.GameplayMenu.Crossair.SetActive(false);
                Main.Instance.GameplayMenu.PickupText.text = string.Empty;
                Main.Instance.GameplayMenu.PromptIcon.sprite = Main.Instance.PromptIcons[0];
                Main.Instance.GameplayMenu.PromptIcon.enabled = false;
                Main.Instance.GameplayMenu.NewMultiOption.SetActive(false);
            }
            return false;
        }

       /* [HarmonyPatch(typeof(MultiInteractible), "Interact")]
        [HarmonyPrefix]
        public static bool MultiInteract2(Person person, int part, object __instance)
        {
            return true;
        }

        [HarmonyPatch(typeof(MultiInteractible), "Interact")]
        [HarmonyPrefix]
        public static bool MultiInteract(Person person, object __instance)
        {
            MultiInteractible _this = (MultiInteractible)__instance;
            _this.PersonGoingToUse = (Person)null;
            if (_this.AskWhichOption && person.IsPlayer)
            {
                _this.PersonWantingToInteract = person;
                Main.Instance.GameplayMenu.MultiOptions.SetActive(true);
                Main.Instance.GameplayMenu.PlayerWeaponSystem.PickupText.text = string.Empty;
                Main.Instance.GameplayMenu.PlayerWeaponSystem.PromptIcon.enabled = false;
                for (int index = 0; index < Main.Instance.GameplayMenu.ItemOptions.Length; ++index)
                    Main.Instance.GameplayMenu.ItemOptions[index].SetActive(false);
                _this._PartsToUse.Clear();
                _this._PartsUseIndex.Clear();
                int num = _this.Parts.Length >= 8 ? 8 : _this.Parts.Length;
                List<(Interactible, int, string)> valueTupleList = new List<(Interactible, int, string)>();
                for (int index1 = 0; index1 < num; ++index1)
                {
                    if (_this.Parts[index1].CanInteract && _this.Parts[index1].CheckCanInteract(Main.Instance.Player))
                    {
                        valueTupleList.Add((_this.Parts[index1], 0, _this.Parts[index1].InteractText + " SUSP "));
                        if (_this.Parts[index1]._AvailableUses != null && _this.Parts[index1]._AvailableUses.Length > 1)
                        {
                            for (int index2 = 1; index2 < _this.Parts[index1]._AvailableUses.Length; ++index2)
                            {
                                if (_this.Parts[index1]._AvailableUses[index2])
                                {
                                    if (index1 + index2 < 8)
                                        valueTupleList.Add((_this.Parts[index1], index2, _this.Parts[index1]._InteractTexts[index2] + " ALLLLL "));
                                    else
                                        goto label_16;
                                }
                            }
                        }
                    }
                }
            label_16:
                for (int index = 0; index < valueTupleList.Count; ++index)
                {
                    Main.Instance.GameplayMenu.ItemOptions[index].SetActive(true);
                    Main.Instance.GameplayMenu.ItemOptions_text[index].text = valueTupleList[index].Item3 + " Hello ";
                    _this._PartsToUse.Add(valueTupleList[index].Item1);
                    _this._PartsUseIndex.Add(valueTupleList[index].Item2);
                }
                Main.Instance.Player.AddMoveBlocker("MultiOption");
                _this.CanInteract = false;
                Main.Instance.MainThreads.Add(new Action(_this.WaitForOption));
            }
            else
            {
                for (int part = 0; part < _this.Parts.Length; ++part)
                {
                    if (_this.Parts[part].CanInteract && _this.Parts[part].CheckCanInteract(person))
                    {
                        _this.Interact(person, part);
                        break;
                    }
                }
            }
            return false;
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
