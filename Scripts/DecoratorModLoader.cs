using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            new GameObject("Player Home").transform.parent = go.transform;
            new GameObject("Player Ship").transform.parent = go.transform;
            new GameObject("Player Ship Exterior").transform.parent = go.transform;

            Mod.SaveDataInterface = DecoratorManager.SaveInstance;
            Mod.IsReady = true;
        }
    }

    [fsObject("v1")]
    public class DecoratorSaveData : IHasModSaveData
    {
        public Dictionary<int, PlacedObjectData_v1[]> playerHome;
        public PlacedObjectData_v1[] playerShip;
        public PlacedObjectData_v1[] playerShipExterior;

        public Type SaveDataType
        {
            get { return typeof(DecoratorSaveData); }
        }

        public object GetSaveData()
        {
            return DecoratorManager.Instance.GetSaveData();
        }

        public object NewSaveData()
        {
            return new DecoratorSaveData
            {
                playerHome = new Dictionary<int, PlacedObjectData_v1[]>(),
                playerShip = null,
                playerShipExterior = null,
            };
        }

        public void RestoreSaveData(object saveData)
        {
            var myModSaveData = (DecoratorSaveData)saveData;

            DecoratorManager.Instance.RestoreSaveData(myModSaveData);
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
}
