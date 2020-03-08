using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decorator
{
    public class DecoratorManager : MonoBehaviour
    {
        #region Instances

        static DecoratorManager instance;
        static DecoratorSaveData saveInstance;

        public static DecoratorManager Instance
        {
            get { return instance ?? (instance = FindObjectOfType<DecoratorManager>()); }
        }

        public static DecoratorSaveData SaveInstance
        {
            get { return saveInstance ?? (saveInstance = new DecoratorSaveData()); }
        }

        #endregion

        #region Fields

        Dictionary<int, PlacedObjectData_v2[]> playerHomeObjects = new Dictionary<int, PlacedObjectData_v2[]>();
        PlacedObjectData_v2[] playerShipObjects;
        PlacedObjectData_v2[] playerShipObjectsExterior;

        PlayerEnterExit playerEnterExit;
        PlayerGPS playerGPS;
        Transform Parent;

        int hotKeyOption;
        string hotKey = string.Empty;

        string placeObjectCost;

        KeyCode hotKeyKeyCode = KeyCode.None;

        //private static readonly string[] shipExteriorSceneNames = new string[] {
        //    StreamingWorld.GetSceneName(shipCoords[0].X, shipCoords[0].Y),
        //    StreamingWorld.GetSceneName(shipCoords[1].X, shipCoords[1].Y),
        //};

        int lastBuildingKey;

        bool inPlayerHome;
        bool inPlayerShip;
        bool inPlayerShipExterior;
        bool loading;      

        #endregion

        #region Properties

        public Transform PlayerHome { get { return transform.GetChild(0); } }
        public Transform PlayerShip { get { return transform.GetChild(1); } }
        public Transform PlayerShipExterior { get { return transform.GetChild(2); } }
        public DaggerfallAudioSource DecoratorAudio { get; private set; }

        string Ship1Interior { get { return DaggerfallInterior.GetSceneName(1050578, BuildingDirectory.buildingKey0); } }
        string Ship2Interior { get { return DaggerfallInterior.GetSceneName(2102157, BuildingDirectory.buildingKey0); } }
        int CurrentBuildingKey { get { return playerEnterExit.BuildingDiscoveryData.buildingKey; } }
        public int PlaceObjectCost { get; private set; }
        public bool GuildRestriction { get; private set; }

        #endregion

        #region Unity

        void Start()
        {
            playerEnterExit = GameManager.Instance.PlayerEnterExit;
            playerGPS = GameManager.Instance.PlayerGPS;
            DecoratorAudio = GetComponent<DaggerfallAudioSource>();

            GetModSettings();

            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            PlayerEnterExit.OnTransitionInterior += PlayerEnterExit_OnTransitionInterior;
            PlayerEnterExit.OnTransitionExterior += PlayerEnterExit_OnTransitionExterior;

            DecoratorModLoader.Mod.MessageReceiver = (string message, object data, DFModMessageCallback callBack) =>
            {
                if (message == "SetParent")
                {
                    if (!(data as Transform))
                        return;

                    Parent = (Transform)data;
                }

                if (message == "PushWindow")
                {
                    if (!(data as Transform))
                        return;

                    PushWindow((Transform)data);

                    return;
                }

                if (message == "GetObjects")
                {
                    if (!(data as Transform))
                        return;

                    object obj = GetObjectData((Transform)data);

                    callBack("ObjectRequest", obj);
                }

                if (message == "CreateObjects")
                {
                    PlacedObjectData_v2[] dataArray = data as PlacedObjectData_v2[];

                    CreatePlacedObjects(dataArray, Parent);

                    //ModManager.Instance.SendModMessage("Airships", "ParentRequest", null, (string messageBack, object dataParent) =>
                    // {
                         
                    // });
                }

                if (message == "HotkeyRequest")
                {
                    callBack("HotkeyReturn", hotKeyKeyCode);
                }
            };
        }

        void GetModSettings()
        {
            ModSettings settings = DecoratorModLoader.Mod.GetSettings();

            hotKeyOption    = settings.GetValue<int>("Options", "HotKeyOptions");
            hotKey          = settings.GetValue<string>("Options", "HotKey");
            placeObjectCost = settings.GetValue<string>("Options", "PlaceObjectCost");

            GuildRestriction = settings.GetValue<bool>("Options", "GuildRestriction");

            if (hotKeyOption == 0)
                hotKeyKeyCode = KeyCode.Slash;
            else if (hotKeyOption == 1)
            {
                try
                {
                    hotKeyKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), hotKey, true);
                }
                catch (ArgumentException)
                {
                    hotKeyKeyCode = KeyCode.Slash;
                }
            }

            try
            {
                PlaceObjectCost = int.Parse(placeObjectCost);

                if (PlaceObjectCost < 0)
                    PlaceObjectCost = 100;
            }
            catch (FormatException)
            {
                PlaceObjectCost = 100;
            }
            catch (OverflowException)
            {
                PlaceObjectCost = 100;
            }
        }

        void Update()
        {
            if (GameManager.IsGamePaused)
                return;

            if (Input.GetKeyUp(hotKeyKeyCode))
            {
                Transform parent = null;

                if (inPlayerHome)
                    parent = PlayerHome;
                else if (inPlayerShip)
                    parent = PlayerShip;
                else if (inPlayerShipExterior)
                    parent = PlayerShipExterior;

                if (parent != null)
                    PushWindow(parent);
            }
        }

        #endregion

        #region Private Methods

        void PushWindow(Transform parent)
        {
            if (GameManager.Instance.IsPlayerOnHUD)
                DaggerfallUI.Instance.UserInterfaceManager.PushWindow(new DecoratorWindow(DaggerfallUI.UIManager, parent));
        }

        IEnumerator Loading(PlayerEnterExit.TransitionType transitionType)
        {
            yield return new WaitUntil(() => loading == false);

            Reset();

            if (transitionType == PlayerEnterExit.TransitionType.ToBuildingInterior ||
                transitionType == PlayerEnterExit.TransitionType.ToDungeonInterior)
            {
                CheckInterior();
            }
            else
            {
                inPlayerHome = false;
                inPlayerShip = false;
                inPlayerShipExterior = false;
            }
        }

        void CheckInterior()
        {
            if (CurrentBuildingKey == 0)
            {
                Debug.LogWarning("CurrentBuildingKey == 0");
                return;
            }
            else
            {
                if (!DaggerfallBankManager.IsHouseOwned(CurrentBuildingKey))
                {
                    if (playerHomeObjects.ContainsKey(CurrentBuildingKey))
                        playerHomeObjects.Remove(CurrentBuildingKey);

                    if (DaggerfallInterior.GetSceneName(playerGPS.CurrentMapID, BuildingDirectory.buildingKey0) == Ship1Interior ||
                        DaggerfallInterior.GetSceneName(playerGPS.CurrentMapID, BuildingDirectory.buildingKey0) == Ship2Interior)
                    {
                        CreatePlacedObjects(playerShipObjects, PlayerShip);
                        inPlayerHome = false;
                        inPlayerShip = true;
                        inPlayerShipExterior = false;
                    }
                    else
                    {
                        inPlayerHome = false;
                        inPlayerShip = false;
                        inPlayerShipExterior = false;
                    }
                }
                else
                {
                    PlacedObjectData_v2[] value = null;

                    if (playerHomeObjects.ContainsKey(CurrentBuildingKey))
                    {
                        playerHomeObjects.TryGetValue(CurrentBuildingKey, out value);
                        CreatePlacedObjects(value, PlayerHome);
                    }
                    else
                        playerHomeObjects.Add(CurrentBuildingKey, value);

                    inPlayerHome = true;
                    inPlayerShip = false;
                    inPlayerShipExterior = false;
                }

                lastBuildingKey = CurrentBuildingKey;
            }
        }

        //void CheckExterior()
        //{
        //    inPlayerHome = false;
        //    inPlayerShip = false;
        //    inPlayerShipExterior = false;

        //    Debug.LogWarning("Check Exterior");

        //    if (DaggerfallBankManager.OwnsShip)
        //    {
        //        Debug.LogWarning("OwnsShip");
        //        if (playerGPS.CurrentMapPixel == DaggerfallBankManager.GetShipCoords())
        //        {
        //            Debug.LogWarning("ShipExterior");
        //            CreatePlacedObjects(playerShipObjectsExterior, PlayerShipExterior);
        //            inPlayerHome = false;
        //            inPlayerShip = false;
        //            inPlayerShipExterior = true;
        //        }
        //        else
        //        {
        //            Debug.LogWarning("Not at ship coords");
        //        }
        //    }
        //}

        void Reset()
        {
            SetData();
            DestroyPlacedObjects();
        }

        void SetData()
        {
            if (!DaggerfallBankManager.OwnsShip)
            {
                playerShipObjects = null;
                playerShipObjectsExterior = null;
            }

            if (DaggerfallBankManager.Houses.Length < 1)
                playerHomeObjects.Clear();

            if (PlayerHome.childCount != 0)
            {
                List<PlacedObjectData_v2> playerHome = new List<PlacedObjectData_v2>();

                foreach (Transform child in PlayerHome)
                {
                    PlacedObjectData_v2 data = child.GetComponent<PlacedObject>().GetData();
                    playerHome.Add(data);
                }

                int key = lastBuildingKey;
                PlacedObjectData_v2[] value = playerHome.ToArray();

                if (playerHomeObjects.ContainsKey(key))
                    playerHomeObjects[key] = value;
                else
                    playerHomeObjects.Add(key, value);
            }

            if (PlayerShip.childCount != 0)
                playerShipObjects = GetObjectData(PlayerShip);

            if (PlayerShipExterior.childCount != 0)
                playerShipObjectsExterior = GetObjectData(PlayerShipExterior);
        }

        PlacedObjectData_v2[] GetObjectData(Transform parent)
        {
            List<PlacedObjectData_v2> objectList = new List<PlacedObjectData_v2>();
            
            foreach (Transform child in parent)
            {
                PlacedObject placedObject;

                if (placedObject = child.GetComponent<PlacedObject>())
                    objectList.Add(placedObject.GetData());
            }

            return objectList.ToArray();
        }

        void CreatePlacedObjects(PlacedObjectData_v2[] dataArray, Transform parent)
        {
            if (dataArray != null)
                foreach (PlacedObjectData_v2 data in dataArray)
                {
                    //// Temporary. Used to update to newer localPosition rather than using world position to place objects.
                    //if (data.position != parent.InverseTransformPoint(data.position))
                    //    data.localPosition = parent.InverseTransformPoint(data.position);

                    DecoratorHelper.CreatePlacedObject(data, parent);
                }
        }

        void DestroyPlacedObjects()
        {
            if (PlayerHome.childCount > 0)
                foreach (Transform child in PlayerHome)
                    Destroy(child.gameObject);

            if (PlayerShip.childCount > 0)
                foreach (Transform child in PlayerShip)
                    Destroy(child.gameObject);

            if (PlayerShipExterior.childCount > 0)
                foreach (Transform child in PlayerShipExterior)
                    Destroy(child.gameObject);
        }

        #endregion

        #region Events

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            loading = true;
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            loading = false;
        }

        private void PlayerEnterExit_OnTransitionExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            StartCoroutine(Loading(args.TransitionType));
        }

        private void PlayerEnterExit_OnTransitionInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            StartCoroutine(Loading(args.TransitionType));
        }

        #endregion

        #region Serialization/Deserialization

        public DecoratorSaveData GetSaveData()
        {
            DecoratorSaveData saveData = new DecoratorSaveData();

            SetData();

            saveData.playerHome = playerHomeObjects;
            saveData.playerShip = playerShipObjects;
            saveData.playerShipExterior = playerShipObjectsExterior;

            return saveData;
        }

        public void RestoreSaveData(DecoratorSaveData saveData)
        {
            DestroyPlacedObjects();

            if (saveData.playerHome != null)
                playerHomeObjects = saveData.playerHome;
            else
                playerHomeObjects = null;

            if (saveData.playerShip != null)
                playerShipObjects = saveData.playerShip;
            else
                playerShipObjects = null;

            if (saveData.playerShipExterior != null)
                playerShipObjectsExterior = saveData.playerShipExterior;
            else
                playerShipObjectsExterior = null;
        }

        #endregion
    }

    public class PlacedObject : MonoBehaviour , IPlayerActivable
    {
        uint modelID;
        int archive;
        int record;
        bool isLight;
        bool isContainer;
        bool isPotionMaker;
        bool isSpellMaker;
        bool isItemMaker;

        Color lightColor;
        float lightIntensity;
        float lightSpotAngle;
        LightType lightType;
        float lightHorizontalRotation;
        float lightVerticalRotation;

        object lootData;

        public void SetData(PlacedObjectData_v2 data)
        {
            transform.localPosition = data.localPosition;
            transform.localRotation = data.localRotation;
            transform.localScale = data.localScale;

            name = data.name;
            modelID = data.modelID;
            archive = data.archive;
            record = data.record;
            isLight = data.isLight;
            isContainer = data.isContainer;
            isPotionMaker = data.isPotionMaker;
            isSpellMaker = data.isSpellMaker;
            isItemMaker = data.isItemMaker;

            lightColor = data.lightColor;
            lightIntensity = data.lightIntensity;
            lightSpotAngle = data.lightSpotAngle;
            lightType = data.lightType;
            lightHorizontalRotation = data.lightHorizontalRotation;
            lightVerticalRotation = data.lightVerticalRotation;

            lootData = data.lootData;
        }

        public PlacedObjectData_v2 GetData()
        {
            PlacedObjectData_v2 data = new PlacedObjectData_v2();

            data.localPosition = transform.localPosition;
            data.localRotation = transform.localRotation;
            data.localScale = transform.localScale;
            data.name = name;

            data.modelID = modelID;
            data.archive = archive;
            data.record = record;
            data.isLight = isLight;
            data.isContainer = isContainer;
            data.isPotionMaker = isPotionMaker;
            data.isSpellMaker = isSpellMaker;
            data.isItemMaker = isItemMaker;

            data.lightColor = lightColor;
            data.lightIntensity = lightIntensity;
            data.lightType = lightType;
            data.lightSpotAngle = lightSpotAngle;

            foreach (Transform child in transform)
            {
                if (child.name == name + " Light")
                {
                    data.lightHorizontalRotation = child.localEulerAngles.y;
                    data.lightVerticalRotation = child.localEulerAngles.x;
                }
            }

            if (isContainer)
            {
                SerializableLootContainer lootContainer;

                if (lootContainer = GetComponent<SerializableLootContainer>())
                    data.lootData = lootContainer.GetSaveData();
            }
            else
                data.lootData = null;

            return data;
        }

        public void Activate(RaycastHit hit)
        {
            if (!GameManager.Instance.IsPlayerOnHUD)
                return;

            if (isPotionMaker)
                DaggerfallUI.UIManager.PushWindow(new DaggerfallPotionMakerWindow(DaggerfallUI.UIManager));
            else if (isSpellMaker)
                DaggerfallUI.UIManager.PushWindow(new DaggerfallSpellMakerWindow(DaggerfallUI.UIManager));
            else if (isItemMaker)
                DaggerfallUI.UIManager.PushWindow(new DaggerfallItemMakerWindow(DaggerfallUI.UIManager));
        }
    }
}
