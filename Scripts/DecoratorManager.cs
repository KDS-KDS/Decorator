using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wenzil.Console;

namespace Decorator
{
    public class DecoratorManager : MonoBehaviour
    {
        #region Instances

        private static DecoratorManager instance;
        private static DecoratorSaveData saveInstance;

        public static DecoratorManager Instance
        {
            get { return instance ?? (instance = FindObjectOfType<DecoratorManager>()); }
        }

        public static DecoratorSaveData SaveInstance
        {
            get { return saveInstance ?? (saveInstance = new DecoratorSaveData()); }
        }

        #endregion Instances

        #region Fields

        public KeyCode HotKeyKeyCode = KeyCode.None;
        public int PlaceObjectCost;
        public bool GuildRestriction;
        public bool DecoratorDebug;

        private List<DecoratorData> decoratorData = new List<DecoratorData>();

        private PlayerEnterExit playerEnterExit;
        private PlayerGPS playerGPS;
        private Transform Parent;

        private int hotKeyOption;
        private string hotKey = string.Empty;
        private string placeObjectCost;
        private bool isGameloading;

        #endregion Fields

        #region Properties

        public Transform InteriorTransform { get { return GameManager.Instance.PlayerEnterExit.Interior.transform; } }
        public DaggerfallAudioSource DecoratorAudio { get; private set; }

        public bool IsInHome
        {
            get
            {
                return playerEnterExit.IsPlayerInside && DaggerfallBankManager.IsHouseOwned(CurrentBuildingKey);
            }
        }

        public bool IsInShip
        {
            get
            {
                DFLocation location = playerGPS.CurrentLocation;

                if (location.Loaded && location.Name == "Your Ship")
                {
                    if (playerEnterExit.IsPlayerInside)
                        return true;
                }

                return false;
            }
        }

        //private bool IsInShipExterior
        //{
        //    get
        //    {
        //        DFLocation location = playerGPS.CurrentLocation;

        //        if (location.Loaded && location.Name == "Your Ship")
        //        {
        //            if (!playerEnterExit.IsPlayerInside)
        //                return true;
        //        }

        //        return false;
        //    }
        //}

        private int CurrentBuildingKey { get { return playerEnterExit.BuildingDiscoveryData.buildingKey; } }

        #endregion Properties

        #region Unity

        private void Start()
        {
            playerEnterExit = GameManager.Instance.PlayerEnterExit;
            playerGPS = GameManager.Instance.PlayerGPS;
            DecoratorAudio = GetComponent<DaggerfallAudioSource>();

            GetModSettings();

            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            PlayerEnterExit.OnTransitionExterior += PlayerEnterExit_OnTransitionExterior;
            PlayerEnterExit.OnTransitionInterior += PlayerEnterExit_OnTransitionInterior;
            //PlayerEnterExit.OnRespawnerComplete += PlayerEnterExit_OnRespawnerComplete;
            //StreamingWorld.OnTeleportToCoordinates += StreamingWorld_OnTeleportToCoordinates;

            DaggerfallBankManager.OnSellHouse += DaggerfallBankManager_OnSellHouse;
            DaggerfallBankManager.OnSellShip += DaggerfallBankManager_OnSellShip;

            ConsoleCommandsDatabase.RegisterCommand(ToggleDecoratorDebug.name,
                                                    ToggleDecoratorDebug.description,
                                                    ToggleDecoratorDebug.usage,
                                                    ToggleDecoratorDebug.Execute);

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

                    if (dataArray != null)
                    {
                        foreach (PlacedObjectData_v2 objectData in dataArray)
                            DecoratorHelper.CreatePlacedObject(objectData, Parent);
                    }
                }

                if (message == "HotkeyRequest")
                {
                    callBack("HotkeyReturn", HotKeyKeyCode);
                }
            };
        }

        private void Update()
        {
            if (GameManager.IsGamePaused)
                return;

            if (Input.GetKeyUp(HotKeyKeyCode))
            {
                if (IsInHome || IsInShip)
                {
                    Transform parent = GetActive();

                    if (parent != null)
                        PushWindow(parent);
                }
            }
        }

        #endregion Unity

        #region Private Methods

        private Transform GetActive()
        {
            Transform parent = null;

            foreach (DecoratorData data in decoratorData)
            {
                CheckOldData(data);

                if (data.IsActive)
                {
                    parent = data.Parent;
                    break;
                }
            }

            // You are in a home or ship, but there is no data yet. Create new data.
            if (parent == null)
            {
                DecoratorData newData = new DecoratorData
                {
                    RegionIndex = playerGPS.CurrentRegionIndex,
                    BuildingKey = CurrentBuildingKey,
                    IsShip = IsInShip,
                };

                decoratorData.Add(newData);
                newData.SetActive(true);

                parent = newData.Parent;
            }

            return parent;
        }

        private void CheckOldData(DecoratorData data)
        {
            if (data.RegionIndex == -1 && (CurrentBuildingKey == data.BuildingKey || (IsInShip && data.IsShip)))
            {
                DecoratorDebug = true;

                data.RegionIndex = playerGPS.CurrentRegionIndex;
                data.BuildingKey = CurrentBuildingKey;

                data.SetActive(true);
            }
        }

        private HouseData_v1[] GetOwnedHouses()
        {
            List<HouseData_v1> list = new List<HouseData_v1>();

            foreach (HouseData_v1 houseData in DaggerfallBankManager.Houses)
            {
                if (houseData.buildingKey > 0)
                {
                    list.Add(houseData);
                }
            }

            return list.ToArray();
        }

        private void GetModSettings()
        {
            ModSettings settings = DecoratorModLoader.Mod.GetSettings();

            hotKeyOption = settings.GetValue<int>("Options", "HotKeyOptions");
            hotKey = settings.GetValue<string>("Options", "HotKey");
            placeObjectCost = settings.GetValue<string>("Options", "PlaceObjectCost");

            GuildRestriction = settings.GetValue<bool>("Options", "GuildRestriction");

            if (hotKeyOption == 0)
            {
                HotKeyKeyCode = KeyCode.Slash;
            }
            else if (hotKeyOption == 1)
            {
                try
                {
                    HotKeyKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), hotKey, true);
                }
                catch (ArgumentException)
                {
                    HotKeyKeyCode = KeyCode.Slash;
                }
            }

            try
            {
                PlaceObjectCost = int.Parse(placeObjectCost);

                if (PlaceObjectCost < 0)
                    PlaceObjectCost = 100;
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is OverflowException)
                {
                    PlaceObjectCost = 100;
                }
            }
        }

        private void PushWindow(Transform parent)
        {
            if (GameManager.Instance.IsPlayerOnHUD)
            {
                DaggerfallUI.UIManager.PushWindow(new DecoratorWindow(DaggerfallUI.UIManager, parent));
            }
            else if (DaggerfallUI.UIManager.TopWindow.GetType() == typeof(DecoratorWindow) ||
                    DaggerfallUI.UIManager.TopWindow.GetType() == typeof(DecoratorDebugWindow))
            {
                DaggerfallUI.UIManager.PopWindow();
            }
        }

        private void CheckLocation()
        {
            if (IsInHome || IsInShip)
            {
                foreach (DecoratorData data in decoratorData)
                {
                    if (data.RegionIndex == playerGPS.CurrentRegionIndex &&
                        data.BuildingKey == CurrentBuildingKey)
                    {
                        data.SetActive(true);
                    }
                    else if (data.IsActive)
                    {
                        data.SetActive(false);
                    }
                }
            }
        }

        private void SetOffsets()
        {
            if (IsInHome || IsInShip)
            {
                float offset = InteriorTransform.position.y - gameObject.transform.position.y;
                gameObject.transform.position += new Vector3(0.0f, offset, 0.0f);
            }
            //else if (IsInShipExterior)
            //{
            //    float offset = GameManager.Instance.StreamingWorld.CurrentPlayerLocationObject.transform.position.y - gameObject.transform.position.y;
            //    gameObject.transform.position += new Vector3(0.0f, offset, 0.0f);
            //}
        }

        private PlacedObjectData_v2[] GetObjectData(Transform parent)
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

        #endregion Private Methods

        #region Events

        private void PlayerEnterExit_OnTransitionInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (isGameloading)
                return;

            SetOffsets();
            CheckLocation();
        }

        private void PlayerEnterExit_OnTransitionExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (isGameloading)
                return;

            RemoveInteriors();
        }

        private void RemoveInteriors()
        {
            foreach (DecoratorData data in decoratorData)
            {
                if (data.IsActive)
                {
                    data.SetActive(false);
                }
            }
        }

        private void SetSaveData()
        {
            foreach (DecoratorData data in decoratorData)
            {
                if (data.IsActive)
                {
                    data.SaveData();
                }
            }
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            ResetDecorator();
            isGameloading = true;
        }

        private void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            isGameloading = false;
            SetOffsets();
            CheckLocation();
        }

        private void ResetDecorator()
        {
            DecoratorDebug = false;
            decoratorData.Clear();

            if (transform.childCount > 0)
                foreach (Transform child in transform)
                    Destroy(child.gameObject);
        }

        private void DaggerfallBankManager_OnSellShip(TransactionType type, TransactionResult result, int amount)
        {
            if (decoratorData.Count > 0)
            {
                foreach (DecoratorData data in decoratorData)
                {
                    if (data.IsShip)
                    {
                        decoratorData.Remove(data);
                    }
                }
            }
        }

        private void DaggerfallBankManager_OnSellHouse(TransactionType type, TransactionResult result, int amount)
        {
            HouseData_v1[] houses = GetOwnedHouses();

            bool foundHouse;
            List<DecoratorData> dataToRemove = new List<DecoratorData>();

            foreach (DecoratorData decoratorData in decoratorData)
            {
                if (decoratorData.RegionIndex != -1 && !decoratorData.IsShip)
                {
                    foundHouse = false;

                    foreach (HouseData_v1 houseData in houses)
                    {
                        if (decoratorData.RegionIndex == houseData.regionIndex &&
                            decoratorData.BuildingKey == houseData.buildingKey)
                        {
                            foundHouse = true;
                        }
                    }

                    if (foundHouse)
                        continue;
                    else
                        dataToRemove.Add(decoratorData);
                }
            }

            if (dataToRemove.Count > 0)
            {
                foreach (DecoratorData data in dataToRemove)
                    decoratorData.Remove(data);
            }
        }

        #endregion Events

        #region Serialization/Deserialization

        public object GetSaveData()
        {
            SetSaveData();

            DecoratorSaveData saveData = new DecoratorSaveData();

            saveData.decoratorData = decoratorData.ToArray();

            return saveData;
        }

        public void RestoreSaveData(object saveData)
        {
            DecoratorSaveData decSaveData = saveData as DecoratorSaveData;

            if (decSaveData.decoratorData != null)
                decoratorData = decSaveData.decoratorData.ToList();

            if (decSaveData.playerHome != null)
            {
                foreach (var pair in decSaveData.playerHome)
                {
                    DecoratorData data = new DecoratorData()
                    {
                        RegionIndex = -1,
                        BuildingKey = pair.Key,
                        ObjectData = pair.Value.ToList()
                    };

                    decoratorData.Add(data);
                }

                decSaveData.playerHome = null;
            }

            if (decSaveData.playerShip != null)
            {
                DecoratorData data = new DecoratorData()
                {
                    RegionIndex = -1,
                    BuildingKey = -1,
                    IsShip = true,
                    ObjectData = decSaveData.playerShip.ToList()
                };

                decoratorData.Add(data);

                decSaveData.playerShip = null;
            }
        }

        #endregion Serialization/Deserialization

        public class DecoratorData
        {
            public List<PlacedObjectData_v2> ObjectData = new List<PlacedObjectData_v2>();
            public Transform Parent;

            public bool IsActive;
            public int RegionIndex;
            public int BuildingKey;
            public bool IsShip;
            public bool IsShipExterior;

            public void SetActive(bool active)
            {
                IsActive = active;

                if (active)
                {
                    GameObject go = new GameObject(GetName());
                    go.transform.SetParent(instance.transform, false);
                    Parent = go.transform;

                    if (ObjectData.Count > 0)
                    {
                        foreach (PlacedObjectData_v2 data in ObjectData)
                            DecoratorHelper.CreatePlacedObject(data, go.transform);
                    }
                }
                else
                {
                    ObjectData.Clear();

                    foreach (Transform child in Parent)
                    {
                        PlacedObjectData_v2 data = child.GetComponent<PlacedObject>().GetData();
                        ObjectData.Add(data);

                        Destroy(child.gameObject);
                    }

                    Destroy(Parent.gameObject);
                }
            }

            public void SaveData()
            {
                ObjectData.Clear();

                foreach (Transform child in Parent)
                {
                    PlacedObjectData_v2 data = child.GetComponent<PlacedObject>().GetData();
                    ObjectData.Add(data);
                }
            }

            public string GetName()
            {
                return RegionIndex.ToString() + " " + BuildingKey.ToString();
            }
        }
    }

    public class PlacedObject : MonoBehaviour, IPlayerActivable
    {
        private uint modelID;
        private int archive;
        private int record;
        private bool isLight;
        private bool isContainer;
        private bool isPotionMaker;
        private bool isSpellMaker;
        private bool isItemMaker;

        private Color lightColor;
        private float lightIntensity;
        private float lightSpotAngle;
        private LightType lightType;
        private float lightHorizontalRotation;
        private float lightVerticalRotation;

        private object lootData;

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

    public static class ToggleDecoratorDebug
    {
        public static readonly string name = "DecoratorDebug";
        public static readonly string description = "Toggles visibility of the Decorator debug button.";
        public static readonly string usage = "DecoratorDebug";

        public static string Execute(params string[] args)
        {
            DecoratorManager.Instance.DecoratorDebug = !DecoratorManager.Instance.DecoratorDebug;

            return "DecoratorDebug is now " + DecoratorManager.Instance.DecoratorDebug;
        }
    }
}