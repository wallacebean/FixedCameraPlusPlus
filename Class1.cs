using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using LLHandlers;
using GameplayEntities;
using LLGUI;
using Abilities;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using BepInEx.Configuration;
using LLBML;
using LLBML.States;
using LLBML.Utils;
using LLBML.Math;
using LLBML.Players;
using LLBML.Networking;



namespace FixedCameraPlusPlus
{
    [BepInPlugin("us.wallace.plugins.llb.FixedCameraPlusPlus", "FixedCameraPlusPlus Plug-In", "1.0.1.0")]
    [BepInDependency(LLBML.PluginInfos.PLUGIN_ID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("no.mrgentle.plugins.llb.modmenu", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("LLBlaze.exe")]

    public class Plugin : BaseUnityPlugin

    {
        public static ManualLogSource Log { get; private set; } = null;

        public static ConfigEntry<bool> enableCamera;
        public static ConfigEntry<int> cameraPositionZ;
        public static ConfigEntry<int> cameraPositionY;

        public static ConfigEntry<bool> enableStageOutline;


        //global
        public static ConfigEntry<bool> useGlobalColors;
        public static ConfigEntry<int> globalColorR;
        public static ConfigEntry<int> globalColorG;
        public static ConfigEntry<int> globalColorB;
        //outskirts
        public static ConfigEntry<int> outskirtsColorR;
        public static ConfigEntry<int> outskirtsColorG;
        public static ConfigEntry<int> outskirtsColorB;
        //sewers
        public static ConfigEntry<int> sewersColorR;
        public static ConfigEntry<int> sewersColorG;
        public static ConfigEntry<int> sewersColorB;
        //desert
        public static ConfigEntry<int> desertColorR;
        public static ConfigEntry<int> desertColorG;
        public static ConfigEntry<int> desertColorB;
        //elevator
        public static ConfigEntry<int> elevatorColorR;
        public static ConfigEntry<int> elevatorColorG;
        public static ConfigEntry<int> elevatorColorB;
        //factory
        public static ConfigEntry<int> factoryColorR;
        public static ConfigEntry<int> factoryColorG;
        public static ConfigEntry<int> factoryColorB;
        //subway
        public static ConfigEntry<int> subwayColorR;
        public static ConfigEntry<int> subwayColorG;
        public static ConfigEntry<int> subwayColorB;
        //stadium
        public static ConfigEntry<int> stadiumColorR;
        public static ConfigEntry<int> stadiumColorG;
        public static ConfigEntry<int> stadiumColorB;
        //streets
        public static ConfigEntry<int> streetsColorR;
        public static ConfigEntry<int> streetsColorG;
        public static ConfigEntry<int> streetsColorB;
        //pool
        public static ConfigEntry<int> poolColorR;
        public static ConfigEntry<int> poolColorG;
        public static ConfigEntry<int> poolColorB;
        //room21
        public static ConfigEntry<int> room21ColorR;
        public static ConfigEntry<int> room21ColorG;
        public static ConfigEntry<int> room21ColorB;


        //outskirts 2d
        public static ConfigEntry<int> outskirts2dColorR;
        public static ConfigEntry<int> outskirts2dColorG;
        public static ConfigEntry<int> outskirts2dColorB;
        //sewers 2d
        public static ConfigEntry<int> sewers2dColorR;
        public static ConfigEntry<int> sewers2dColorG;
        public static ConfigEntry<int> sewers2dColorB;
        //factory 2d
        public static ConfigEntry<int> factory2dColorR;
        public static ConfigEntry<int> factory2dColorG;
        public static ConfigEntry<int> factory2dColorB;
        //subway 2d
        public static ConfigEntry<int> subway2dColorR;
        public static ConfigEntry<int> subway2dColorG;
        public static ConfigEntry<int> subway2dColorB;
        //streets 2d
        public static ConfigEntry<int> streets2dColorR;
        public static ConfigEntry<int> streets2dColorG;
        public static ConfigEntry<int> streets2dColorB;
        //pool 2d
        public static ConfigEntry<int> pool2dColorR;
        public static ConfigEntry<int> pool2dColorG;
        public static ConfigEntry<int> pool2dColorB;
        //room21 2d
        public static ConfigEntry<int> room212dColorR;
        public static ConfigEntry<int> room212dColorG;
        public static ConfigEntry<int> room212dColorB;

        void Awake()
        {


            Config.Bind("Camera Settings", "mm_header_qol", "Camera Settings", new ConfigDescription("", null, "modmenu_header"));
            enableCamera = Config.Bind<bool>("CameraToggles", "enableCamera", true);
            cameraPositionZ = Config.Bind("1. FCPPconfig", "Z Position of Camera (Divided by 1000, Vanilla Default: -15625, Mod Default: -17600)", -17600, new ConfigDescription("Controls How Far Away the Camera is from the Stage"));
            cameraPositionY = Config.Bind("1. FCPPconfig", "Y Position of Camera (Divided by 1000, Vanilla Default: 1890, Mod Default: 2330)", 2330, new ConfigDescription("Controls the Hight of the Camera"));

            Config.Bind("gap", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Stage Outline Settings", "mm_header_qol", "Stage Outline Settings", new ConfigDescription("", null, "modmenu_header"));
            enableStageOutline = Config.Bind<bool>("OutlineToggles", "enableStageOutline", true);
            useGlobalColors = Config.Bind<bool>("OutlineToggles", "useGlobalColors", false);

            Config.Bind("gap2", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Global Stage Outline Colors", "mm_header_qol", "Global Stage Outline Colors", new ConfigDescription("", null, "modmenu_header"));
            globalColorR = Config.Bind("3. GlobalColors", "ColorR", 255, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            globalColorG = Config.Bind("3. GlobalColors", "ColorG", 234, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            globalColorB = Config.Bind("3. GlobalColors", "ColorB", 5, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap3", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Outskirts Outline Color", "mm_header_qol", "Outskirts Outline Color", new ConfigDescription("", null, "modmenu_header"));
            outskirtsColorR = Config.Bind("4. OutskirtsColors", "ColorR", 255, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            outskirtsColorG = Config.Bind("4. OutskirtsColors", "ColorG", 204, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            outskirtsColorB = Config.Bind("4. OutskirtsColors", "ColorB", 99, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap4", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Sewers Outline Color", "mm_header_qol", "Sewers Outline Color", new ConfigDescription("", null, "modmenu_header"));
            sewersColorR = Config.Bind("5. SewersColors", "ColorR", 226, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            sewersColorG = Config.Bind("5. SewersColors", "ColorG", 150, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            sewersColorB = Config.Bind("5. SewersColors", "ColorB", 235, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap5", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Desert Outline Color", "mm_header_qol", "Desert Outline Color", new ConfigDescription("", null, "modmenu_header"));
            desertColorR = Config.Bind("6. DesertColors", "ColorR", 110, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            desertColorG = Config.Bind("6. DesertColors", "ColorG", 166, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            desertColorB = Config.Bind("6. DesertColors", "ColorB", 189, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap6", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Elevator Outline Color", "mm_header_qol", "Elevator Outline Color", new ConfigDescription("", null, "modmenu_header"));
            elevatorColorR = Config.Bind("7. ElevatorColors", "ElevatorColorR", 245, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            elevatorColorG = Config.Bind("7. ElevatorColors", "ElevatorColorG", 93, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            elevatorColorB = Config.Bind("7. ElevatorColors", "ElevatorColorB", 54, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap7", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Factory Outline Color", "mm_header_qol", "Factory Outline Color", new ConfigDescription("", null, "modmenu_header"));
            factoryColorR = Config.Bind("8. FactoryColors", "ColorR", 255, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            factoryColorG = Config.Bind("8. FactoryColors", "ColorG", 234, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            factoryColorB = Config.Bind("8. FactoryColors", "ColorB", 5, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap8", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Subway Outline Color", "mm_header_qol", "Subway Outline Color", new ConfigDescription("", null, "modmenu_header"));
            subwayColorR = Config.Bind("9. SubwayColors", "ColorR", 234, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            subwayColorG = Config.Bind("9. SubwayColors", "ColorG", 106, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            subwayColorB = Config.Bind("9. SubwayColors", "ColorB", 55, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap9", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Stadium Outline Color", "mm_header_qol", "Stadium Outline Color", new ConfigDescription("", null, "modmenu_header"));
            stadiumColorR = Config.Bind("10. StadiumColors", "ColorR", 229, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            stadiumColorG = Config.Bind("10. StadiumColors", "ColorG", 81, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            stadiumColorB = Config.Bind("10. StadiumColors", "ColorB", 113, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap10", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Streets Outline Color", "mm_header_qol", "Streets Outline Color", new ConfigDescription("", null, "modmenu_header"));
            streetsColorR = Config.Bind("11. StreetsColors", "ColorR", 228, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            streetsColorG = Config.Bind("11. StreetsColors", "ColorG", 71, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            streetsColorB = Config.Bind("11. StreetsColors", "ColorB", 59, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap11", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Pool Outline Color", "mm_header_qol", "Pool Outline Color", new ConfigDescription("", null, "modmenu_header"));
            poolColorR = Config.Bind("12. PoolColors", "ColorR", 144, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            poolColorG = Config.Bind("12. PoolColors", "ColorG", 221, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            poolColorB = Config.Bind("12. PoolColors", "ColorB", 165, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap12", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Room 21 Outline Color", "mm_header_qol", "Room 21 Outline Color", new ConfigDescription("", null, "modmenu_header"));
            room21ColorR = Config.Bind("13. RoomColors", "ColorR", 50, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            room21ColorG = Config.Bind("13. RoomColors", "ColorG", 50, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            room21ColorB = Config.Bind("13. RoomColors", "ColorB", 50, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap13", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Stages", "mm_header_qol", "Retro Stages", new ConfigDescription("", null, "modmenu_header"));

            Config.Bind("gap14", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Outskirts Outline Color", "mm_header_qol", "Retro Outskirts Outline Color", new ConfigDescription("", null, "modmenu_header"));
            outskirts2dColorR = Config.Bind("14. OutskirtsColors", "ColorR", 255, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            outskirts2dColorG = Config.Bind("14. OutskirtsColors", "ColorG", 204, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            outskirts2dColorB = Config.Bind("14 OutskirtsColors", "ColorB", 99, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap15", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Sewers Outline Color", "mm_header_qol", "Retro Sewers Outline Color", new ConfigDescription("", null, "modmenu_header"));
            sewers2dColorR = Config.Bind("15. SewersColors", "ColorR", 226, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            sewers2dColorG = Config.Bind("15. SewersColors", "ColorG", 150, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            sewers2dColorB = Config.Bind("15. SewersColors", "ColorB", 235, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap16", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Factory Outline Color", "mm_header_qol", "Retro Factory Outline Color", new ConfigDescription("", null, "modmenu_header"));
            factory2dColorR = Config.Bind("16. FactoryColors", "ColorR", 255, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            factory2dColorG = Config.Bind("16. FactoryColors", "ColorG", 234, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            factory2dColorB = Config.Bind("16. FactoryColors", "ColorB", 5, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap17", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Subway Outline Color", "mm_header_qol", "Retro Subway Outline Color", new ConfigDescription("", null, "modmenu_header"));
            subway2dColorR = Config.Bind("17. SubwayColors", "ColorR", 234, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            subway2dColorG = Config.Bind("17. SubwayColors", "ColorG", 106, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            subway2dColorB = Config.Bind("17. SubwayColors", "ColorB", 55, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap18", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Streets Outline Color", "mm_header_qol", "Retro Streets Outline Color", new ConfigDescription("", null, "modmenu_header"));
            streets2dColorR = Config.Bind("18. StreetsColors", "ColorR", 228, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            streets2dColorG = Config.Bind("18. StreetsColors", "ColorG", 71, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            streets2dColorB = Config.Bind("18. StreetsColors", "ColorB", 59, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap19", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Pool Outline Color", "mm_header_qol", "Retro Pool Outline Color", new ConfigDescription("", null, "modmenu_header"));
            pool2dColorR = Config.Bind("19. PoolColors", "ColorR", 144, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            pool2dColorG = Config.Bind("19. PoolColors", "ColorG", 221, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            pool2dColorB = Config.Bind("19. PoolColors", "ColorB", 165, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));

            Config.Bind("gap20", "mm_header_gap", 50, new ConfigDescription("", null, "modmenu_gap"));
            Config.Bind("Retro Room 21 Outline Color", "mm_header_qol", "Retro Room 21 Outline Color", new ConfigDescription("", null, "modmenu_header"));
            room212dColorR = Config.Bind("20. RoomColors", "ColorR", 50, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            room212dColorG = Config.Bind("20. RoomColors", "ColorG", 50, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));
            room212dColorB = Config.Bind("20. RoomColors", "ColorB", 50, new BepInEx.Configuration.ConfigDescription("", new BepInEx.Configuration.AcceptableValueRange<int>(0, 255), new object[0]));


            Log = this.Logger;

            Logger.LogDebug("Patching effects settings...");



            var harmony = new Harmony("us.wallace.plugins.llb.FixedCameraPlusPlus");


            {
                harmony.PatchAll(typeof(GameCameraInitPatch));
            }


            Logger.LogDebug("FixedCameraPlusPlus is loaded");
        }

        void Start()
        {
            ModDependenciesUtils.RegisterToModMenu(this.Info);

        }
    }
    class GameCameraInitPatch
    {
        [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.Init))]
        [HarmonyPrefix]
        public static bool Init_Prefix(GameCamera __instance, World setWorld, CJJDGIPHBCG data)
        {
            int valZ1 = Plugin.cameraPositionZ.Value;
            float valZ2 = (float)valZ1 / 1000;
            int valY1 = Plugin.cameraPositionY.Value;
            float valY2 = (float)valY1 / 1000;

            GameCamera.InitCamera(__instance.gameObject, ICCBAHKDIPO.HBJOBPFMENA | ICCBAHKDIPO.KEMKLFFLBMJ);
            __instance.AddSubCameras();
            __instance.world = setWorld;
            __instance.position = __instance.transform.position;
            __instance.minScroll = 0f;
            __instance.maxScroll = 0f;
            GameCamera.camToLookAt = GameCamera.gameplayCam.transform;
            __instance.cameraData = data;
            UnityEngine.GameObject gameObject = GameCamera.gameplayCam.gameObject;
            if ((bool)Plugin.enableStageOutline.Value)
            {
                StageOutline.CreateStageMesh(World.instance.GetStageCenter(), World.instance.stageSize);
            }
            if ((bool)Plugin.enableCamera.Value)
            {
                StageBackground.BG.instance.maxPosZ = valZ2;
                StageBackground.BG.instance.fixedCamPosY = valY2;
            }
            return false;
        }

    }



    internal class StageOutline : UnityEngine.MonoBehaviour
    {
        public void Start()
        {
            lineMaterial = new UnityEngine.Material(UnityEngine.Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = UnityEngine.HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", 5);
            lineMaterial.SetInt("_DstBlend", 10);
            lineMaterial.SetInt("_Cull", 0);
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.SetInt("_ZTest", 8);
            CreateStageMesh(World.instance.GetStageCenter(), World.instance.stageSize);
            stageColor = new UnityEngine.Color((float)Plugin.globalColorR.Value / 255f, (float)Plugin.globalColorG.Value / 255f, (float)Plugin.globalColorB.Value / 255f);

        }

        public static void CreateStageMesh(IBGCBLLKIHA obCenter, IBGCBLLKIHA obSize)
        {
            lineMaterial = new UnityEngine.Material(UnityEngine.Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = UnityEngine.HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", 5);
            lineMaterial.SetInt("_DstBlend", 10);
            lineMaterial.SetInt("_Cull", 0);
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.SetInt("_ZTest", 8);

            if ((bool)Plugin.useGlobalColors.Value)
            {
                stageColor = new UnityEngine.Color((float)Plugin.globalColorR.Value / 255f, (float)Plugin.globalColorG.Value / 255f, (float)Plugin.globalColorB.Value / 255f);
            }
            else
            {
                if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.OUTSKIRTS)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.outskirtsColorR.Value / 255f, (float)Plugin.outskirtsColorG.Value / 255f, (float)Plugin.outskirtsColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.SEWERS)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.sewersColorR.Value / 255f, (float)Plugin.sewersColorG.Value / 255f, (float)Plugin.sewersColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.JUNKTOWN)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.desertColorR.Value / 255f, (float)Plugin.desertColorG.Value / 255f, (float)Plugin.desertColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.CONSTRUCTION)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.elevatorColorR.Value / 255f, (float)Plugin.elevatorColorG.Value / 255f, (float)Plugin.elevatorColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.FACTORY)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.factoryColorR.Value / 255f, (float)Plugin.factoryColorG.Value / 255f, (float)Plugin.factoryColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.SUBWAY)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.subwayColorR.Value / 255f, (float)Plugin.subwayColorG.Value / 255f, (float)Plugin.subwayColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.STADIUM)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.stadiumColorR.Value / 255f, (float)Plugin.stadiumColorG.Value / 255f, (float)Plugin.stadiumColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.STREETS)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.streetsColorR.Value / 255f, (float)Plugin.streetsColorG.Value / 255f, (float)Plugin.streetsColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.POOL)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.poolColorR.Value / 255f, (float)Plugin.poolColorG.Value / 255f, (float)Plugin.poolColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.ROOM21)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.room21ColorR.Value / 255f, (float)Plugin.room21ColorG.Value / 255f, (float)Plugin.room21ColorB.Value / 255f);
                }

                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.OUTSKIRTS_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.outskirts2dColorR.Value / 255f, (float)Plugin.outskirts2dColorG.Value / 255f, (float)Plugin.outskirts2dColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.SEWERS_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.sewers2dColorR.Value / 255f, (float)Plugin.sewers2dColorG.Value / 255f, (float)Plugin.sewers2dColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.FACTORY_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.factory2dColorR.Value / 255f, (float)Plugin.factory2dColorG.Value / 255f, (float)Plugin.factory2dColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.SUBWAY_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.subway2dColorR.Value / 255f, (float)Plugin.subway2dColorG.Value / 255f, (float)Plugin.subway2dColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.STREETS_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.streets2dColorR.Value / 255f, (float)Plugin.streets2dColorG.Value / 255f, (float)Plugin.streets2dColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.POOL_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.pool2dColorR.Value / 255f, (float)Plugin.pool2dColorG.Value / 255f, (float)Plugin.pool2dColorB.Value / 255f);
                }
                else if (LLHandlers.StageHandler.curStage == LLHandlers.Stage.ROOM21_2D)
                {
                    stageColor = new UnityEngine.Color((float)Plugin.room212dColorR.Value / 255f, (float)Plugin.room212dColorG.Value / 255f, (float)Plugin.room212dColorB.Value / 255f);
                }
                else
                {
                    stageColor = new UnityEngine.Color((float)Plugin.globalColorR.Value / 255f, (float)Plugin.globalColorG.Value / 255f, (float)Plugin.globalColorB.Value / 255f);
                }
            }
            UnityEngine.Vector3[] array = new UnityEngine.Vector3[4];
            UnityEngine.Vector3[] array2 = new UnityEngine.Vector3[4];
            UnityEngine.Vector3[] array3 = new UnityEngine.Vector3[4];
            UnityEngine.Vector3[] array4 = new UnityEngine.Vector3[4];
            UnityEngine.Vector3[] array5 = new UnityEngine.Vector3[4];
            float num = 0.1f;
            UnityEngine.Vector2 vector = Vector2(obCenter);
            UnityEngine.Vector2 vector2 = Vector2(obSize) * new UnityEngine.Vector2(0.5f, 0.5f);
            array[0] = new UnityEngine.Vector3(vector.x - vector2.x, vector.y - vector2.y);
            array[1] = new UnityEngine.Vector3(vector.x - vector2.x, vector.y + vector2.y);
            array[2] = new UnityEngine.Vector3(vector.x + vector2.x, vector.y - vector2.y);
            array[3] = new UnityEngine.Vector3(vector.x + vector2.x, vector.y + vector2.y);
            array2[0] = array[0];
            array2[1] = array2[0] - new UnityEngine.Vector3(num, 0f);
            array2[2] = array[1];
            array2[3] = array2[2] - new UnityEngine.Vector3(num, 0f);
            array4[1] = array[2];
            array4[0] = array4[1] + new UnityEngine.Vector3(num, 0f);
            array4[3] = array[3];
            array4[2] = array4[3] + new UnityEngine.Vector3(num, 0f);
            array3[0] = array2[3];
            array3[1] = array3[0] + new UnityEngine.Vector3(0f, num);
            array3[2] = array4[2];
            array3[3] = array3[2] + new UnityEngine.Vector3(0f, num);
            array5[1] = array2[1];
            array5[0] = array5[1] - new UnityEngine.Vector3(0f, num);
            array5[3] = array4[0];
            array5[2] = array5[3] - new UnityEngine.Vector3(0f, num);
            UnityEngine.Color color = stageColor;
            CreateMeshObject("LeftMesh", array2, color);
            CreateMeshObject("RightMesh", array4, color);
            CreateMeshObject("TopMesh", array3, color);
            CreateMeshObject("BottomMesh", array5, color);
        }

        public static void CreateMeshObject(string meshName, UnityEngine.Vector3[] vertices, UnityEngine.Color color)
        {
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            int[] triangles = new int[]
            {
        0,
        1,
        2,
        2,
        1,
        3
            };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            UnityEngine.GameObject gameObject = new UnityEngine.GameObject(meshName, new System.Type[]
           {
        typeof( UnityEngine.MeshFilter),
        typeof( UnityEngine.MeshRenderer)
           });
            gameObject.transform.localScale = UnityEngine.Vector3.one;
            gameObject.GetComponent<UnityEngine.MeshFilter>().mesh = mesh;
            gameObject.GetComponent<UnityEngine.MeshRenderer>().material = lineMaterial;
            gameObject.GetComponent<UnityEngine.MeshRenderer>().material.color = color;
        }

        public static IBGCBLLKIHA Vector2f(UnityEngine.Vector2 v)
        {
            HHBCPNCDNDH hlbffjjdkam = HHBCPNCDNDH.NKKIFJJEPOL((decimal)v.x);
            HHBCPNCDNDH iidpnbfgcmc = HHBCPNCDNDH.NKKIFJJEPOL((decimal)v.y);
            return new IBGCBLLKIHA(hlbffjjdkam, iidpnbfgcmc);
        }

        public static UnityEngine.Vector2 Vector2(HHBCPNCDNDH fX, HHBCPNCDNDH fY)
        {
            float x = (float)fX.EPOACNMBMMN / 4.2949673E+09f;
            float y = (float)fY.EPOACNMBMMN / 4.2949673E+09f;
            return new UnityEngine.Vector2(x, y);
        }

        public static UnityEngine.Vector2 Vector2(IBGCBLLKIHA v)
        {
            float x = (float)v.GCPKPHMKLBN.EPOACNMBMMN / 4.2949673E+09f;
            float y = (float)v.CGJJEHPPOAN.EPOACNMBMMN / 4.2949673E+09f;
            return new UnityEngine.Vector2(x, y);
        }

        public static UnityEngine.Material lineMaterial;
        public static UnityEngine.Color stageColor;
    }




}



