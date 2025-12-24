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

        private void Update()
        {
            // Toggle GUI with F1 key
            if (Input.GetKeyUp(KeyCode.F1))
            {
                showGUI = !showGUI;
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
                    Logger.LogInfo($"User entered: {inputText}");
                    switch (inputText)
                    {
                        case "clearbackpack": {
                                clearbackpack();
                        } break;

                        case "showpos":
                        {
                                showpos();
                        }
                        break;
                    }
                }
                inputText = "";
            }

            // Allow window dragging
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public static void clearbackpack()
        {
            Main.Instance.GameplayMenu.ShowNotification("clearbackpack");
            if (Main.Instance.Player.CurrentBackpack != null && Main.Instance.Player.CurrentBackpack.ThisStorage != null)
            {

                Main.Instance.Player.CurrentBackpack.ThisStorage.RemoveAllItems();
            }
        }

        public static void showpos()
        {
            Main.Instance.GameplayMenu.ShowNotification("showpos");
            Vector3 lastSpawnPoint = Main.Instance.Player.transform.position;
            Main.Instance.GameplayMenu.ShowNotification(lastSpawnPoint.x.ToString() + " " + lastSpawnPoint.y.ToString() + " " + lastSpawnPoint.z.ToString());
            Main.Instance.GameplayMenu.ShowNotification(lastSpawnPoint.x.ToString() + " " + lastSpawnPoint.y.ToString() + " " + lastSpawnPoint.z.ToString());

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

        private static Vector3 explorerSpawnPoint = new Vector3(-2.949118f, 1.192093E-07f, 39.10889f);
        private static Vector3 explorerSpawnPoint2 = new Vector3(179.8053f, 0.05544382f, -73.4415f);
        private static Vector3 hardcoreSpawnPoint = new Vector3(-69f, 0.0f, 10f);
        private static Vector3 hardcoreSpawnPoint2 = new Vector3(-49.10827f, 3.067196f, 14.40517f);
    }
}
