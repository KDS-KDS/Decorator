using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using FullSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Decorator.DecoratorManager;

namespace Decorator
{
    public class DecoratorModLoader : MonoBehaviour
    {
        public static Mod Mod { get; private set; }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            Mod = initParams.Mod;

            GameObject go = new GameObject("Decorator");
            go.AddComponent<DecoratorManager>();
            go.AddComponent<DaggerfallAudioSource>();

            Mod.SaveDataInterface = SaveInstance;
            Mod.IsReady = true;
        }
    }

    [fsObject("v1")]
    public class DecoratorSaveData : IHasModSaveData
    {
        public Dictionary<int, PlacedObjectData_v2[]> playerHome;
        public PlacedObjectData_v2[] playerShip;
        public PlacedObjectData_v2[] playerShipExterior;

        public DecoratorData[] decoratorData;

        public Type SaveDataType
        {
            get { return typeof(DecoratorSaveData); }
        }

        public object NewSaveData()
        {
            return new DecoratorSaveData
            {
                decoratorData = null,
            };
        }

        public object GetSaveData()
        {
            return Instance.GetSaveData();
        }

        public void RestoreSaveData(object saveData)
        {
            Instance.RestoreSaveData(saveData);
        }
    }

    [fsObject("v1")]
    public class PlacedObjectData_v1
    {
        public object lootData;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string name;
        public uint modelID;
        public int archive;
        public int record;
        public bool isContainer;
        public bool isLight;
        public bool isPotionMaker;
        public bool isSpellMaker;
        public LightType lightType;
        public Color lightColor;
        public float lightIntensity;
        public float lightSpotAngle;
        public float lightHorizontalRotation;
        public float lightVerticalRotation;
    }

    [fsObject("v2", previousModels: typeof(PlacedObjectData_v1))]
    public class PlacedObjectData_v2
    {
        public PlacedObjectData_v2()
        { }

        public PlacedObjectData_v2(PlacedObjectData_v1 oldData)
        {
            Debug.LogWarning("Updating old Decorator data...");

            lootData = oldData.lootData;
            localRotation = Quaternion.Euler(oldData.rotation);
            localPosition = oldData.position;
            localScale = oldData.scale;
            name = oldData.name;
            modelID = oldData.modelID;
            archive = oldData.archive;
            record = oldData.record;
            isContainer = oldData.isContainer;
            isLight = oldData.isLight;
            isPotionMaker = oldData.isPotionMaker;
            isSpellMaker = oldData.isSpellMaker;
            lightType = oldData.lightType;
            lightColor = oldData.lightColor;
            lightIntensity = oldData.lightIntensity;
            lightSpotAngle = oldData.lightSpotAngle;
            lightHorizontalRotation = oldData.lightHorizontalRotation;
            lightVerticalRotation = oldData.lightVerticalRotation;
        }

        public object lootData;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public string name;
        public uint modelID;
        public int archive;
        public int record;
        public bool isContainer;
        public bool isLight;
        public bool isPotionMaker;
        public bool isSpellMaker;
        public bool isItemMaker;
        public LightType lightType;
        public Color lightColor;
        public float lightIntensity;
        public float lightSpotAngle;
        public float lightHorizontalRotation;
        public float lightVerticalRotation;
    }
}
