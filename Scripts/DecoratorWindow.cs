using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Decorator
{
    public class DecoratorWindow : DaggerfallPopupWindow
    {
        #region Fields

        //Mainpanel
        private Rect mainPanelRect = new Rect(0.0f, 0.0f, 120f, 20.0f);

        private Rect listPanelRect = new Rect(0.0f, 22.0f, 120f, 150f);

        private Panel mainPanel;
        private Panel listPanel;
        private LeftRightSpinner pageSpinner;

        // Transform panel
        private Rect transformPanelRect = new Rect(121.0f, 0.0f, 108f, 53f);
        private Rect transformSubPanel1Rect = new Rect(0.0f, 0.0f, 108f, 41f);
        private Rect transformSubPanel2Rect = new Rect(0.0f, 41f, 108f, 12f);
        private Rect upButtonRect = new Rect(48f, 0f, 11f, 10f);
        private Rect downButtonRect = new Rect(48f, 31f, 11f, 10f);
        private Rect leftButtonRect = new Rect(30f, 15f, 10f, 11f);
        private Rect rightButtonRect = new Rect(66f, 15f, 11f, 11f);
        private Rect rotateXZRightButtonRect = new Rect(61f, 6f, 9f, 9f);
        private Rect rotateXZLeftButtonRect = new Rect(37f, 6f, 9f, 9f);
        private Rect rotateLeftButtonRect = new Rect(37f, 26f, 9f, 9f);
        private Rect rotateRightButtonRect = new Rect(61f, 26f, 9f, 9f);
        private Rect acceptButtonRect = new Rect(50f, 17f, 7f, 7f);

        private Rect lowButtonRect = new Rect(7f, 42f, 29f, 10f);
        private Rect medButtonRect = new Rect(36f, 42f, 32f, 10f);
        private Rect highButtonRect = new Rect(68f, 42f, 32f, 10f);

        private Rect resetButtonRect = new Rect(0.0f, 35.0f, 20f, 7f);
        private Rect deleteButtonRect = new Rect(2.0f, 10.0f, 18f, 7f);

        private Texture2D transformSubPanelTexture1;
        private Texture2D transformSubPanelTexture2;

        private Panel transformPanel;
        private Panel transformSubPanel1;
        private Panel transformSubPanel2;

        private Button upButton;
        private Button downButton;
        private Button leftButton;
        private Button rightButton;
        private Button rotateXZRightButton;
        private Button rotateXZLeftButton;
        private Button rotateLeftButton;
        private Button rotateRightButton;
        private Button acceptButton;
        private Button resetButton;
        private Button deleteButton;

        private Button lowButton;
        private Button medButton;
        private Button highButton;

        private Button debugButton;

        private Checkbox scaleCheckBox;
        private Checkbox snapCheckbox;
        private Checkbox editCheckBox;
        private Checkbox lightCheckbox;
        private Checkbox containerCheckbox;
        private Checkbox potionMakerCheckbox;
        private Checkbox spellMakerCheckbox;
        private Checkbox itemMakerCheckbox;

        // Light panel
        private Rect lightPanelRect = new Rect(230.0f, 0.0f, 90.0f, 53.0f);

        private Panel lightPanel;
        private Checkbox lightSpotCheckbox;

        //Checkbox       lightFlickerCheckbox;
        private HorizontalSlider lightIntensitySlider;

        private HorizontalSlider lightSpotAngleSlider;
        private HorizontalSlider lightHorizontalRotationSlider;
        private HorizontalSlider lightVerticalRotationSlider;
        private Button colorPicker;

        //Scale panel
        private Rect scalePanelRect = new Rect(230.0f, 54.0f, 90.0f, 40.0f);

        private Rect scaleResetButtonRect = new Rect(0.0f, 0.0f, 25.0f, 10.0f);

        private Panel scalePanel;
        private Button scaleResetButton;
        private HorizontalSlider scaleXSlider;
        private HorizontalSlider scaleYSlider;
        private HorizontalSlider scaleZSlider;

        private Transform Parent;
        private GameObject previewGo;
        private Light previewLight;
        private BoxCollider previewCollider;
        private PlacedObjectData_v2 lastPlacedObjectData;
        private PlayerMouseLook playerMouseLook;
        private PlayerActivate playerActivate;

        private Vector3 defaultPosition = new Vector3(0.0f, 0.1f, 2f);
        private Vector3 lastPosition = Vector3.zero;

        private Ray snapRay = new Ray();
        private RaycastHit snapRayHit = new RaycastHit();

        private bool editMode;
        private int goHeight = 2;
        private int goRotation = 0;
        private int pages = 1;

        private KeyCode hideWindowKey;
        private bool mouselookToggle = false;
        private bool colorPickerEnabled;

        private bool spellRank;
        private bool potionRank;
        private bool itemRank;

        private Dictionary<string, string> common = new Dictionary<string, string>()
        {
            {"-1", "Common" },
            {"41100", "Chair 1 , Wide" },
            {"41101", "Chair 1, Narrow" },
            {"41102", "Chair w. Armrest" },
            {"41103", "Chair 2, Wide" },
            {"41122", "Throne, Narrow" },
            {"41123", "Throne, Wide" },
            {"41104", "Throne, Green" },

            {"51100", "Chair, Red" },
            {"51101", "Chair, Thin" },

            {"41113", "Stool, Small" },
            {"41114", "Stool, Large" },

            {"41105", "Bench, Thick" },
            {"41106", "Bench, Thin" },
            {"41126", "Bench w Backrest" },
            {"43307", "Bench, Park" },

            {"41108", "Table 1" },
            {"41109", "Table 2" },
            {"41110", "Table 3" },
            {"41111", "Table 4" },
            {"41112", "Table 5" },
            {"41130", "Table 6" },
            {"41121", "Table 7" },

            {"41000", "Large Bed" },
            {"41001", "Small Bed" },
            {"41002", "Small Bed w. Top" },
        };

        private Dictionary<string, string> containers = new Dictionary<string, string>()
        {
            {"-1", "Containers" },
            {"41803", "Small Dresser" },
            {"41802", "Small Cabinent" },
            {"41051", "Small Cabinent w. Cab" },
            {"41032", "Small Dresser w. Cab" },
            {"41035", "Small Dresser w. Alch" },
            {"41036", "Small Dresser w. Cloth" },
            {"41037", "Small Dresser w. Misc" },
            {"41800", "Wardrobe" },
            {"41801", "Double Wardrobe" },
            {"41030", "Shelves, Empty" },
            {"41016", "Shelves, Books" },
            {"41124", "Shelves, Keg and Bottles" },
            {"41010", "Shelves, Clothes" },
            {"41027", "Shelves, Misc" },
            {"41041", "Shelves, Alchemy" },
            {"41047", "Shelves, Books w Weapons" },
            {"41045", "Shelves, Books w Misc" },

            {"41821", "Small Crate 1" },
            {"41824", "Small Crate 2" },

            {"41826", "Medium Crate 1" },
            {"41817", "Medium Crate 2" },
            {"41833", "Large Crate 1" },

            {"41811", "Chest 1" },
            {"41812", "Chest 2" },
        };

        private Dictionary<string, string> lights = new Dictionary<string, string>()
        {
            {"-1", "Lights" },
            {"210.27", "Lantern 1"},
            {"210.24", "Lantern 1 w. Chain, Long" },
            {"210.25", "Lantern 1 w. Chain, Medium" },
            {"210.26", "Lantern 1 w. Chain, Short" },
            {"210.22", "Lantern 2 w. Chain, Short" },

            {"210.10", "Fancy Candle Chandolier, Unlit" },
            {"210.9", "Fancy Candle Chandolier, Lit" },

            {"210.16", "Torch 1" },
            {"210.17", "Torch 2" },
            {"210.18", "Torch 3" },

            {"210.0", "Small Brazier" },
            {"210.19", "Large Brazier" },

            {"210.7", "Candle Chandolier, Unlit" },
            {"210.23", "Candle Chandolier, Lit" },

            {"210.2", "Skull Candle" },
            {"210.3", "Candle" },
            {"210.4", "Candle w. Base, Small" },
            {"210.21", "Candle w. Base, Large" },
            {"210.5", "Three Candles w. Base" },

            {"210.20", "Tiki Torch" },
            {"210.6", "Skull Tiki Torch" },
        };

        private Dictionary<string, string> wall = new Dictionary<string, string>()
        {
            {"-1", "Wall" },
            {"51115", "Painting 1" },
            {"51116", "Painting 2" },
            {"51117", "Painting 3" },
            {"51118", "Painting 4" },
            {"51119", "Painting 5" },
            {"51120", "Painting 6" },
            {"42500", "Banner 1" },
            {"42501", "Banner 2" },
            {"42502", "Banner 3" },
            {"42503", "Banner 4" },
            {"42504", "Banner 5 " },
            {"42505", "Banner 6 " },
            {"42506", "Banner 7 " },
            {"42507", "Banner 8 " },
            {"42508", "Banner 9 " },
            {"42509", "Banner 10 " },
            {"42510", "Banner 11 " },
            {"42511", "Banner 12 " },
            {"42520", "Banner 13 " },
            {"42521", "Banner 14 " },
            {"42522", "Banner 15 " },
            {"42523", "Banner 16 " },
            {"42532", "Banner 17 " },
            {"42533", "Banner 18 " },
            {"42534", "Banner 19 " },
            {"42535", "Banner 20 " },
            {"42536", "Tapestry 1" },
            {"42537", "Tapestry 2" },
            {"42538", "Tapestry 3" },
            {"42539", "Tapestry 4" },
            {"42540", "Tapestry 5" },
            {"42541", "Tapestry 6" },
            {"42542", "Tapestry 7" },
            {"42543", "Tapestry 8" },
            {"42544", "Tapestry 9" },
            {"42545", "Tapestry 10" },
            {"42546", "Tapestry 11" },
            {"42547", "Tapestry 12" },
            {"42548", "Tapestry 13" },
            {"42549", "Tapestry 14" },
            {"42550", "Tapestry 15" },
            {"42551", "Tapestry 16" },
            {"42552", "Tapestry 17" },
            {"42553", "Tapestry 18" },
            {"42554", "Tapestry 19" },
            {"42555", "Tapestry 20" },
            {"42556", "Tapestry 21" },
            {"42557", "Tapestry 22" },
            {"42558", "Tapestry 23" },
            {"42559", "Tapestry 24" },
            {"42560", "Tapestry 25" },
            {"42567", "Tapestry 26" },
            {"42568", "Tapestry 27" },
            {"42569", "Tapestry 28" },
            {"42570", "Tapestry 29" },
            {"42571", "Tapestry 30" },
        };

        private Dictionary<string, string> library = new Dictionary<string, string>()
        {
            {"-1", "Library" },
            {"211.1", "Quill and Ink" },
            {"208.0", "Globe" },
            {"208.1", "Magnifying Glass" },
            {"209.0", "Three Books, Stacked" },
            {"209.1", "Two Books, Stacked" },
            {"209.2", "Book, Brown" },
            {"209.3", "Book, Green" },
            {"209.4", "Book, Green, Large" },
            {"209.5", "a Scroll" },
            {"209.6", "Stack of Scrolls" },
            {"209.7", "Piece of Paper 1" },
            {"209.8", "Piece of Paper 2" },
            {"209.10", "Stack of Paper" },
            {"216.40", "Scrolls w Books" },
            {"209.11", "Stone Tablet 1, Brown" },
            {"209.12", "Stone Tablet 1, Grey" },
            {"209.13", "Stone Tablet 2, Brown" },
            {"209.14", "Stone Tablet 2, Grey" },
            {"209.15", "Stack of Stone Tablets" },
        };

        private Dictionary<string, string> misc1 = new Dictionary<string, string>()
        {
            {"-1", "Misc 1" },
            {"208.4", "Telescope" },
            {"208.3", "Weight Scales" },
            {"208.5", "Handheld Mirror" },
            {"208.6", "Hourglass" },
            {"211.0", "Wrappings" },
            {"211.3", "Dung" },
            {"211.4", "Chain, Short" },
            {"211.7", "Chain, Long" },
            {"211.5", "Chains, Long" },
            {"211.6", "Chains, Short and Long" },
            {"211.47", "Bell" },
            {"211.48", "Necklace" },
            {"211.49", "Holy Water" },
            {"211.50", "Talisman" },
            {"211.56", "Finger" },
            {"211.54", "a Baby" },
            {"211.20", "Scarecrow" },
            {"211.21", "Rocking Horse" },
            {"211.22", "Noose" },
            {"211.24", "Pipe, Short" },
            {"211.25", "Pipes, Short" },
            {"211.23", "Pipes, Long" },
            {"200.11", "Pillow, White" },
            {"200.13", "Pillow, Pink" },
            {"200.19", "Wooden Bucket" },
            {"205.0", "Wooden Barrel" },
            {"205.8", "Basket, Small" },
            {"205.9", "Basket, Large" },
            {"205.10", "Basket of fish" },
            {"211.9", "a Fish, Blue" },
            {"211.8", "Pile of Fish, Blue" },
            {"211.10", "a Fish, Grey" },
            {"211.11", "Fish, Grey, Hanging" },
            {"205.17", "Sack 1" },
            {"205.19", "Sack 2" },
            {"205.20", "Sack 3" },
            {"205.21", "Chest 1" },
            {"205.22", "Chest 2" },
            {"205.23", "Chest 3" },
            {"205.24", "Chest 4" },
            {"205.25", "Chests 1"},
            {"205.26", "Chests 2" },
        };

        private Dictionary<string, string> misc2 = new Dictionary<string, string>()
        {
            {"-1", "Misc 2" },
            {"41009", "Spinning Wheel" },
            {"41120", "Organ" },
            {"41116", "Fireplace" },
            {"41117", "Fireplace, Corner" },
            {"41118", "Anvil" },
            {"51111", "Altar" },
            {"74224", "Sword 1" },
            {"74227", "Sword 2" },
            {"74095", "Sword 3" },
            {"74225", "Axe" },
            {"74219", "Sickle" },
            {"74221", "Crossbow" },
            {"99800", "Arrow" },
            {"74226", "Suit of Armor" },
            {"74231", "Marble Diamond" },
            {"74230", "Decoration" },
            {"41700", "Stocks" },
            {"41300", "Torture 1" },
            {"41301", "Torture 2" },
            {"41302", "Torture 3" },
            {"41303", "Torture 4" },
            {"41312", "Cage" },
            {"41209", "Water Trough, Filled" },
            {"41210", "Water Trough, Empty" },
            {"41220", "Fountain 1" },
            {"41222", "Fountain 2" },
            {"41239", "Cart" },
            {"41238", "Divider" },
            {"21003", "Fence, Wood" },
            {"60606", "Gate" },
            {"41125", "Wooden Stand" },
            {"62315", "Column, Marble" },
            {"61127", "Wooded Post" },
            {"41409", "Ladder" },
            {"61132", "Steering Wheel" },
            {"41020", "Podium 1" },
            {"41021", "Podium 2" },
            {"41739", "Sign" },
            {"41703", "Booth" },

            {"60719", "Rock 1" },
            {"60720", "Rock 2" },
            {"60712", "Rock 3" },
            {"60612", "Rock 4" },
            {"60715", "Rock 5" },
            {"60716", "Rock 6" },
            {"60613", "Rock 7" }
        };

        private Dictionary<string, string> alchemy = new Dictionary<string, string>()
        {
            {"-1", "Alchemy" },
            {"205.31", "Colorful Bottles" },
            {"205.11", "Bottle 1, Empty" },
            {"205.12", "Bottle 1, Half-full" },
            {"205.13", "Bottle 1, Full" },
            {"211.2",  "Bottle 2, Empty" },
            {"205.32", "Bottle, Blue" },
            {"205.33", "Bottle, Pink" },
            {"205.34", "Bottle, Orange" },
            {"205.35", "Bottle, Green" },
            {"205.1", "Flask w Yellow Liquid" },
            {"208.2", "Flask over Candle 1" },
            {"253.41", "Flask over Candle 2" },
            {"205.3", "Empty Bottle" },
            {"205.2", "Bottle of Eyes" },
            {"205.4", "Bottle w Brain" },
            {"205.5", "Bottle w Orange Liquid" },
            {"205.7", "Bottle with Purple Liquid" },
            {"205.6", "Bottle w Red Liquid" },
            {"205.43", "Small Bottle w Green Liquid" },
            {"254.34", "Flask w Red Liquid 1, Corked" },
            {"254.36", "Flask w Red Liquid 2, Corked" },
            {"254.49", "Flask w Black Liquid, Corked" },

            {"254.0", "Ruby" },
            {"254.1", "Emerald" },
            {"254.2", "Sapphire" },
            {"254.3", "Diamond" },
            {"254.4", "Jade" },
            {"254.5", "Turqoise" },
            {"254.6", "Pearl" },
            {"254.7", "Malachite" },
            {"254.8", "Amber" },
            {"254.9", "Twigs" },
            {"254.10", "Green Leaves" },
            {"254.11", "Red Flowers" },
            {"254.12", "Yellow Flowers" },
            {"254.13", "Root Tendrils" },
            {"254.14", "Root Bulb" },
            {"254.15", "Pine Branch" },
            {"254.16", "Green Berries" },
            {"254.17", "Red Berries" },
            {"254.18", "Yellow Berries" },
            {"254.19", "Clover" },
            {"254.20", "Gingko Leaves" },
            {"254.21", "Bamboo" },
            {"254.22", "Palm" },
            {"254.23", "Aloe" },
            {"254.24", "Fig" },
            {"254.25", "Cactus" },
            {"254.26", "Red Rose" },
            {"254.27", "Yellow Rose" },
            {"254.28", "Black Rose" },
            {"254.29", "White Rose" },
            {"254.30", "Red Poppy" },
            {"254.31", "Black Poppy" },
            {"254.32", "Golden Poppy" },
            {"254.33", "White Poppy" },
            {"254.35", "Ingredient" },
            {"254.37", "Fairy Dragon's Scales" },
            {"254.38", "Giant Scorpion Stinger" },
            {"254.39", "Small Scorpion Stinger" },
            {"254.40", "Ingredient 2" },
            {"254.41", "Mummy Wrappings" },
            {"254.42", "Wereboar's Tusk" },
            {"254.43", "Unicorn Horn" },
            {"254.44", "Ectoplasm" },
            {"254.45", "Ghoul's Tongue" },
            {"254.46", "Spider's Venom" },
            {"254.47", "Wraith Essence" },
            {"254.48", "Dragon's Scales" },
            {"254.50", "Basilisk's Eye" },
            {"254.51", "Daedra's Heart" },
            {"254.52", "Ichor/Rain Water/Snake Venom" },
            {"254.53", "Gryphon's Feather" },
            {"254.54", "Gorgon Snake" },
            {"254.55", "Nymph Hair" },
            {"254.56", "Holy Relic" },
            {"254.57", "Big Tooth" },
            {"254.58", "Medium Tooth" },
            {"254.59", "Small Tooth" },
            {"254.61", "Mercury" },
            {"254.62", "Gold" },
            {"254.63", "Iron" },
            {"254.64", "Tin" },
            {"254.65", "Silver/Platinum" },
            {"254.66", "Brass/Copper" },
            {"254.67", "Sulphur" },
            {"254.68", "Ingredient 3" },
            {"254.69", "Ingredient 4" },
            {"254.70", "Ivory" },
            {"254.71", "Ingredient 5" },
        };

        private Dictionary<string, string> bio = new Dictionary<string, string>()
        {
            {"-1", "Bio" },
            {"201.0", "Horse, Brown" },
            {"201.1", "Horse, Black" },
            {"201.2", "Camel" },
            {"201.3", "Cow 1" },
            {"201.4", "Cow 2" },
            {"201.5", "Pig 1" },
            {"201.6", "Pig, 2" },
            {"201.7", "Cat 1" },
            {"201.8", "Cat 2" },
            {"201.9", "Dog 1" },
            {"201.10", "Dog 2" },
            {"201.11", "Seagull" },

            {"205.37", "Plant 1" },
            {"205.38", "Plant 2" },
            {"205.39", "Plant 3" },
            {"205.40", "Plant 4" },

            {"213.0","Orange" },
            {"213.1","Apple" },
            {"213.2","Potted Plant 1" },
            {"213.4","Potted Plant 2" },
            {"213.5","Potted Plant 3" },
            {"213.6","Potted Plant 4" },
            {"213.13","Potted Plant, Hanging 1" },
            {"213.14","Potted Plant, Hannging 2" },
            {"213.3","Plant 1" },
            {"213.15","Plant 2" },
            {"213.16","Plant 3" },
            {"213.17","Plant 4" },
            {"213.7","Vine" },
            {"213.8","Vines 1" },
            {"213.9","Vines 2" },
            {"213.10","Vines 3" },
            {"213.11","Logs 1" },
            {"213.12","Logs 2" },

            {"41735", "Large Log 1" },
            {"41736", "Large Log 1" },
            {"41737", "Large Log 1" },
            {"41738", "Large Log 1" },
            {"41237", "a Hedge" },
        };

        private Dictionary<string, string> treasure = new Dictionary<string, string>()
        {
            {"-1", "Treasure" },
            {"200.0", "Silver Goblet" },
            {"200.4", "Silver Goblet, jeweled" },
            {"200.1", "Gold Goblet" },
            {"200.5", "Gold Goblet, jeweled" },
            {"200.3", "Gold Goblet, filled" },
            {"200.6", "Copper Goblet" },
            {"216.0", "Bag on Gold Pile" },
            {"216.37", "Pot on Gold Pile" },
            {"216.1", "Pile of Gold" },
            {"216.39", "Pile of Gold w Gems, Large" },
            {"216.34", "Pile of Gold w Gems, Small" },
            {"216.3", "Gold Ingot" },
            {"216.4", "Gold Coin" },
            {"216.5", "Silver Coin" },
            {"216.6", "Gold Crown" },
            {"216.7", "Silver Crown" },
            {"216.8", "Silver Tiara" },
            {"216.9", "Gold Tiara" },
            {"216.10", "Gem 1" },
            {"216.11", "Gem 2" },
            {"216.12", "Gem 3" },
            {"216.13", "Gem 4" },
            {"216.14", "Gem 5" },
            {"216.15", "Gem 6" },
            {"216.16", "Gem 7" },
            {"216.17", "Gem 8" },
            {"216.18", "Gems 1" },
            {"216.19", "Gems 2" },
            {"216.46", "Chest w Gold 1" },
            {"216.47", "Chest w Gold 2" },
        };

        private Dictionary<string, string> statues = new Dictionary<string, string>()
        {
            {"-1", "Statues" },
            {"97.0", "Statue 1" },
            {"97.1", "Statue 2" },
            {"97.2", "Statue 3" },
            {"97.3", "Statue 4" },
            {"97.4", "Statue 5" },
            {"97.5", "Statue 6" },
            {"97.6", "Statue 7" },
            {"97.7", "Statue 8" },
            {"97.8", "Statue 9" },
            {"97.9", "Statue 10" },
            {"97.10", "Statue 11" },
            {"97.11", "Statue 12" },
            {"97.13", "Statue 13" },
            {"97.14", "Statue 14" },
            {"97.15", "Statue 15" },
            {"97.16", "Statue 16" },
            {"97.17", "Statue 17" },
            {"97.18", "Statue 18" },
            {"97.19", "Statue 19" },
            {"97.20", "Statue 20" },
            {"97.21", "Statue 21" },
        };

        private List<Dictionary<string, string>> dictionaryList = new List<Dictionary<string, string>>();

        private Dictionary<string, string> currentDictionary;

        #endregion Fields

        #region Properties

        private Transform Player
        { get { return GameManager.Instance.PlayerObject.transform; } }
        private AudioClip ClickSound
        { get { return DecoratorManager.Instance.DecoratorAudio.GetAudioClip(360); } }

        #endregion Properties

        #region Constructor

        public DecoratorWindow(IUserInterfaceManager uiManager, Transform parent)
        : base(uiManager)
        {
            ParentPanel.BackgroundColor = Color.clear;
            PauseWhileOpen = false;

            Parent = parent;

            dictionaryList.AddRange(new List<Dictionary<string, string>>() { common, containers, lights, wall, library, misc1, misc2, alchemy, bio, treasure, statues });
        }

        #endregion Constructor

        #region Setup

        protected override void Setup()
        {
            LoadTextures();

            // Main Panel
            mainPanel = DaggerfallUI.AddPanel(mainPanelRect);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Left;
            mainPanel.BackgroundColor = Color.black;
            mainPanel.Outline.Enabled = true;
            mainPanel.VerticalAlignment = VerticalAlignment.Top;

            listPanel = DaggerfallUI.AddPanel(listPanelRect);
            listPanel.HorizontalAlignment = HorizontalAlignment.Left;
            listPanel.BackgroundColor = Color.black;
            listPanel.Outline.Enabled = true;

            pageSpinner = new LeftRightSpinner();
            pageSpinner.Value = 1;
            pageSpinner.HorizontalAlignment = HorizontalAlignment.Left;
            pageSpinner.VerticalAlignment = VerticalAlignment.Top;
            pageSpinner.OnLeftButtonClicked += Page_OnLeftButtonClicked;
            pageSpinner.OnRightButtonClicked += Page_OnRightButtonClicked;
            listPanel.Components.Add(pageSpinner);

            //Transform Panel
            transformPanel = DaggerfallUI.AddPanel(transformPanelRect);
            transformPanel.VerticalAlignment = VerticalAlignment.Top;

            transformSubPanel1 = DaggerfallUI.AddPanel(transformSubPanel1Rect, transformPanel);
            transformSubPanel1.BackgroundTexture = transformSubPanelTexture1;

            transformSubPanel2 = DaggerfallUI.AddPanel(transformSubPanel2Rect, transformPanel);
            transformSubPanel2.BackgroundTexture = transformSubPanelTexture2;

            debugButton = DaggerfallUI.AddButton(new Vector2(73, 46), new Vector2(7, 5), transformPanel);
            debugButton.HorizontalAlignment = HorizontalAlignment.Right;
            debugButton.ClickSound = ClickSound;
            debugButton.OnMouseClick += DebugButton_OnMouseClick;
            debugButton.Label.Text = "DBG";
            debugButton.Label.TextScale = 0.7f;
            debugButton.Enabled = DecoratorManager.Instance.DecoratorDebug;

            lowButton = DaggerfallUI.AddButton(lowButtonRect, transformPanel);
            lowButton.ClickSound = ClickSound;
            lowButton.OnMouseClick += LowButton_OnMouseClick;

            medButton = DaggerfallUI.AddButton(medButtonRect, transformPanel);
            medButton.ClickSound = ClickSound;
            medButton.OnMouseClick += MedButton_OnMouseClick;

            highButton = DaggerfallUI.AddButton(highButtonRect, transformPanel);
            highButton.ClickSound = ClickSound;
            highButton.OnMouseClick += HighButton_OnMouseClick;

            rotateLeftButton = DaggerfallUI.AddButton(rotateLeftButtonRect, transformPanel);
            rotateLeftButton.ClickSound = ClickSound;
            rotateLeftButton.OnMouseClick += RotateLeftButton_OnMouseClick;

            rotateRightButton = DaggerfallUI.AddButton(rotateRightButtonRect, transformPanel);
            rotateRightButton.ClickSound = ClickSound;
            rotateRightButton.OnMouseClick += RotateRightButton_OnMouseClick;

            upButton = DaggerfallUI.AddButton(upButtonRect, transformPanel);
            upButton.ClickSound = ClickSound;
            upButton.OnMouseClick += UpButton_OnMouseClick;

            downButton = DaggerfallUI.AddButton(downButtonRect, transformPanel);
            downButton.ClickSound = ClickSound;
            downButton.OnMouseClick += DownButton_OnMouseClick;

            leftButton = DaggerfallUI.AddButton(leftButtonRect, transformPanel);
            leftButton.ClickSound = ClickSound;
            leftButton.OnMouseClick += LeftButton_OnMouseClick;

            rightButton = DaggerfallUI.AddButton(rightButtonRect, transformPanel);
            rightButton.ClickSound = ClickSound;
            rightButton.OnMouseClick += RightButton_OnMouseClick;

            rotateXZRightButton = DaggerfallUI.AddButton(rotateXZRightButtonRect, transformPanel);
            rotateXZRightButton.ClickSound = ClickSound;
            rotateXZRightButton.OnMouseClick += RotateXZRightButton_OnMouseClick;

            rotateXZLeftButton = DaggerfallUI.AddButton(rotateXZLeftButtonRect, transformPanel);
            rotateXZLeftButton.ClickSound = ClickSound;
            rotateXZLeftButton.OnMouseClick += RotateXZLeftButton_OnMouseClick;

            acceptButton = DaggerfallUI.AddButton(acceptButtonRect, transformPanel);
            acceptButton.ClickSound = ClickSound;
            acceptButton.OnMouseClick += AcceptButton_OnMouseClick;

            snapCheckbox = DaggerfallUI.AddCheckbox(new Vector2(3.0f, 10.0f), false, transformPanel);
            snapCheckbox.Label.Text = "Snap";

            editCheckBox = DaggerfallUI.AddCheckbox(new Vector2(3.0f, 2.0f), false, transformPanel);
            editCheckBox.Label.Text = "Edit";
            editCheckBox.OnMouseClick += EditButton_OnMouseClick;

            resetButton = DaggerfallUI.AddButton(resetButtonRect, transformPanel);
            resetButton.Label.Text = "Reset";
            resetButton.ClickSound = ClickSound;
            resetButton.HorizontalAlignment = HorizontalAlignment.Left;
            resetButton.OnMouseClick += ResetObjectButton_OnMouseClick;

            deleteButton = DaggerfallUI.AddButton(deleteButtonRect, transformPanel);
            deleteButton.Label.Text = "Delete";
            deleteButton.ClickSound = ClickSound;
            deleteButton.OnMouseClick += DeleteButton_OnMouseClick;
            deleteButton.Enabled = false;

            lightCheckbox = DaggerfallUI.AddCheckbox(new Vector2(3.0f, 18.0f), false, transformPanel);
            lightCheckbox.Label.Text = "Light";

            scaleCheckBox = DaggerfallUI.AddCheckbox(new Vector2(3.0f, 26.0f), false, transformPanel);
            scaleCheckBox.Label.Text = "Scale";

            containerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(75.0f, 2.0f), false, transformPanel);
            containerCheckbox.Label.Text = "Container";
            containerCheckbox.OnMouseClick += ContainerCheckbox_OnMouseClick;

            potionMakerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 10.0f), false, transformPanel);
            potionMakerCheckbox.Label.Text = "Potion";
            potionMakerCheckbox.OnMouseClick += PotionMakerCheckbox_OnMouseClick;

            if (!potionRank)
                potionMakerCheckbox.Enabled = false;

            spellMakerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 18.0f), false, transformPanel);
            spellMakerCheckbox.Label.Text = "Spell";
            spellMakerCheckbox.OnMouseClick += SpellMakerCheckbox_OnMouseClick;

            if (!spellRank)
                spellMakerCheckbox.Enabled = false;

            itemMakerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 26.0f), false, transformPanel);
            itemMakerCheckbox.Label.Text = "Item";
            itemMakerCheckbox.OnMouseClick += ItemMakerCheckbox_OnMouseClick;

            if (!itemRank)
                itemMakerCheckbox.Enabled = false;

            // LightPanel
            lightPanel = DaggerfallUI.AddPanel(lightPanelRect);
            lightPanel.BackgroundColor = Color.black;
            lightPanel.Outline.Enabled = true;
            lightPanel.Enabled = false;

            lightSpotCheckbox = DaggerfallUI.AddCheckbox(new Vector2(1.0f, 1.0f), false, lightPanel);
            lightSpotCheckbox.Label.Text = "Spot";

            //lightFlickerCheckbox = DaggerfallUI.AddCheckbox(Vector2.zero, false, lightPanel);
            //lightFlickerCheckbox.HorizontalAlignment = HorizontalAlignment.Right;
            //lightFlickerCheckbox.Label.Text = "Flicker";

            colorPicker = DaggerfallUI.AddColorPicker(new Vector2(0.0f, 18.0f), Color.white, uiManager, this, lightPanel);
            colorPicker.HorizontalAlignment = HorizontalAlignment.Right;
            colorPicker.VerticalAlignment = VerticalAlignment.Top;
            colorPicker.Label.Text = "Light Color";
            colorPicker.Label.TextColor = Color.black;
            colorPicker.Label.ShadowColor = colorPicker.BackgroundColor;
            colorPicker.Scale = new Vector2(10.0f, 10.0f);

            colorPicker.OnMouseClick += (sender, position) =>
            {
                colorPickerEnabled = true;
            };

            float numOffset = -60.0f;

            TextLabel lightIntensityLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 9.0f), "Intensity", lightPanel);
            lightIntensityLabel.ShadowColor = Color.black;
            lightIntensityLabel.HorizontalAlignment = HorizontalAlignment.Center;

            TextLabel lightSpotAngleLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 20.0f), "Spot Angle", lightPanel);
            lightSpotAngleLabel.ShadowColor = Color.black;
            lightSpotAngleLabel.HorizontalAlignment = HorizontalAlignment.Center;

            TextLabel lightVerticalRotationLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 31.0f), "Rotation Vert", lightPanel);
            lightVerticalRotationLabel.ShadowColor = Color.black;
            lightVerticalRotationLabel.HorizontalAlignment = HorizontalAlignment.Center;

            TextLabel lightHorizontalRotationLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 42.0f), "Rotation Horiz", lightPanel);
            lightHorizontalRotationLabel.ShadowColor = Color.black;
            lightHorizontalRotationLabel.HorizontalAlignment = HorizontalAlignment.Center;

            lightIntensitySlider = DaggerfallUI.AddSlider(new Vector2(0.0f, 15.0f), (x) => x.SetIndicator(0f, 5f, 1f), 0.9f, lightPanel);
            lightIntensitySlider.Indicator.HorzPixelScrollOffset = numOffset;
            lightIntensitySlider.HorizontalAlignment = HorizontalAlignment.Center;
            lightIntensitySlider.OnRightMouseClick += Slider_OnRightMouseClick;

            lightSpotAngleSlider = DaggerfallUI.AddSlider(new Vector2(0.0f, 26f), (x) => x.SetIndicator(0f, 180f, 90f), 0.9f, lightPanel);
            lightSpotAngleSlider.Indicator.HorzPixelScrollOffset = numOffset;
            lightSpotAngleSlider.HorizontalAlignment = HorizontalAlignment.Center;
            lightSpotAngleSlider.OnRightMouseClick += Slider_OnRightMouseClick;

            lightVerticalRotationSlider = DaggerfallUI.AddSlider(new Vector2(0.0f, 37f), (x) => x.SetIndicator(0f, 360f, 90f), 0.9f, lightPanel);
            lightVerticalRotationSlider.Indicator.HorzPixelScrollOffset = numOffset;
            lightVerticalRotationSlider.HorizontalAlignment = HorizontalAlignment.Center;
            lightVerticalRotationSlider.OnRightMouseClick += Slider_OnRightMouseClick;

            lightHorizontalRotationSlider = DaggerfallUI.AddSlider(Vector2.zero, (x) => x.SetIndicator(0f, 360f, 90f), 0.9f, lightPanel);
            lightHorizontalRotationSlider.Indicator.HorzPixelScrollOffset = numOffset;
            lightHorizontalRotationSlider.HorizontalAlignment = HorizontalAlignment.Center;
            lightHorizontalRotationSlider.VerticalAlignment = VerticalAlignment.Bottom;
            lightHorizontalRotationSlider.OnRightMouseClick += Slider_OnRightMouseClick;

            // Scale Panel
            scalePanel = DaggerfallUI.AddPanel(scalePanelRect);
            scalePanel.BackgroundColor = Color.black;
            scalePanel.Outline.Enabled = true;
            scalePanel.Enabled = false;

            scaleResetButton = DaggerfallUI.AddButton(scaleResetButtonRect, scalePanel);
            scaleResetButton.HorizontalAlignment = HorizontalAlignment.Center;
            scaleResetButton.VerticalAlignment = VerticalAlignment.Top;
            scaleResetButton.Label.Text = "Reset";
            scaleResetButton.OnMouseClick += ScaleResetButton_OnMouseClick;

            TextLabel scaleXLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 8.0f), "X Scale", scalePanel);
            scaleXLabel.TextScale = 0.8f;
            scaleXLabel.ShadowColor = Color.black;
            scaleXLabel.HorizontalAlignment = HorizontalAlignment.Center;

            TextLabel scaleYLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 19.0f), "Y Scale", scalePanel);
            scaleYLabel.TextScale = 0.8f;
            scaleYLabel.ShadowColor = Color.black;
            scaleYLabel.HorizontalAlignment = HorizontalAlignment.Center;

            TextLabel scaleZLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 30.0f), "Z Scale", scalePanel);
            scaleZLabel.TextScale = 0.8f;
            scaleZLabel.ShadowColor = Color.black;
            scaleZLabel.HorizontalAlignment = HorizontalAlignment.Center;

            scaleXSlider = DaggerfallUI.AddSlider(new Vector2(0.0f, 14f), (x) => x.SetIndicator(0.1f, 3.0f, 1.0f), 0.9f, scalePanel);
            scaleXSlider.Indicator.HorzPixelScrollOffset = numOffset;
            scaleXSlider.HorizontalAlignment = HorizontalAlignment.Center;
            scaleXSlider.OnRightMouseClick += Slider_OnRightMouseClick;

            scaleYSlider = DaggerfallUI.AddSlider(new Vector2(0.0f, 25f), (x) => x.SetIndicator(0.1f, 3.0f, 1.0f), 0.9f, scalePanel);
            scaleYSlider.Indicator.HorzPixelScrollOffset = numOffset;
            scaleYSlider.HorizontalAlignment = HorizontalAlignment.Center;
            scaleYSlider.OnRightMouseClick += Slider_OnRightMouseClick;

            scaleZSlider = DaggerfallUI.AddSlider(Vector2.zero, (x) => x.SetIndicator(0.1f, 3.0f, 1.0f), 0.9f, scalePanel);
            scaleZSlider.Indicator.HorzPixelScrollOffset = numOffset;
            scaleZSlider.HorizontalAlignment = HorizontalAlignment.Center;
            scaleZSlider.VerticalAlignment = VerticalAlignment.Bottom;
            scaleZSlider.OnRightMouseClick += Slider_OnRightMouseClick;

            NativePanel.Components.Add(mainPanel);
            NativePanel.Components.Add(listPanel);
            NativePanel.Components.Add(transformPanel);
            NativePanel.Components.Add(lightPanel);
            NativePanel.Components.Add(scalePanel);

            GenerateButtons(dictionaryList);
        }

        private void DebugButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            PopWindow();

            DaggerfallUI.UIManager.PushWindow(new DecoratorDebugWindow(DaggerfallUI.UIManager, Parent));
        }

        private void LoadTextures()
        {
            Rect arrowsRect = new Rect(0, 25, 108, 41);
            transformSubPanelTexture1 = ImageReader.GetSubTexture(ImageReader.GetTexture("CNFG04I0.IMG"), arrowsRect);

            Rect positionsRect = new Rect(0, 92, 108, 12);
            transformSubPanelTexture2 = ImageReader.GetSubTexture(ImageReader.GetTexture("CNFG04I0.IMG"), positionsRect);
        }

        #endregion Setup

        #region Unity

        public override void Update()
        {
            base.Update();

            // Prevent activating containers etc. with window open.
            playerActivate.SetClickDelay(1.0f);

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (colorPickerEnabled)
                {
                    colorPickerEnabled = false;
                }
            }

            if (Input.GetKeyUp(hideWindowKey))
                mouselookToggle = !mouselookToggle;

            SetMouselook(mouselookToggle);

            if (editMode)
                EditMode();

            if (previewGo != null)
            {
                if (lightCheckbox.IsChecked)
                {
                    if (mouselookToggle)
                        lightPanel.Enabled = false;
                    else
                        lightPanel.Enabled = true;

                    if (previewLight != null)
                    {
                        if (lightSpotCheckbox.IsChecked)
                            previewLight.type = LightType.Spot;
                        else
                            previewLight.type = LightType.Point;

                        previewLight.color = colorPicker.BackgroundColor;
                        previewLight.intensity = lightIntensitySlider.GetValue();
                        previewLight.spotAngle = lightSpotAngleSlider.GetValue();
                        previewLight.transform.localEulerAngles = new Vector3(lightVerticalRotationSlider.GetValue(), lightHorizontalRotationSlider.GetValue(), 0.0f);

                        previewLight.enabled = true;
                    }
                }
                else
                {
                    lightPanel.Enabled = false;

                    if (previewLight != null)
                        previewLight.enabled = false;
                }

                if (scaleCheckBox.IsChecked)
                {
                    if (mouselookToggle)
                        scalePanel.Enabled = false;
                    else
                        scalePanel.Enabled = true;

                    previewGo.transform.localScale = new Vector3(scaleXSlider.GetValue(), scaleYSlider.GetValue(), scaleZSlider.GetValue());
                }
                else
                {
                    scalePanel.Enabled = false;
                    previewGo.transform.localScale = Vector3.one;
                }

                // TODO: Allow snap horizontal and vertical separately.
                if (snapCheckbox.IsChecked)
                {
                    SetObjectHeight(goHeight);
                    SetObjectPosition();
                }
            }
        }

        public override void OnPush()
        {
            base.OnPush();

            GuildManager guildManager = GameManager.Instance.GuildManager;

            if (DecoratorManager.Instance.GuildRestriction)
            {
                if (guildManager.GetGuild(FactionFile.GuildGroups.MagesGuild).CanAccessService(GuildServices.MakeSpells) ||
                    guildManager.GetGuild(FactionFile.GuildGroups.HolyOrder).CanAccessService(GuildServices.MakeSpells))
                    spellRank = true;

                if (guildManager.GetGuild(FactionFile.GuildGroups.HolyOrder).CanAccessService(GuildServices.MakePotions) ||
                    guildManager.GetGuild(FactionFile.GuildGroups.DarkBrotherHood).CanAccessService(GuildServices.MakePotions))
                    potionRank = true;

                if (guildManager.GetGuild(FactionFile.GuildGroups.HolyOrder).CanAccessService(GuildServices.MakeMagicItems) ||
                   guildManager.GetGuild(FactionFile.GuildGroups.MagesGuild).CanAccessService(GuildServices.MakeMagicItems))
                    itemRank = true;
            }
            else
            {
                spellRank = true;
                potionRank = true;
                itemRank = true;
            }

            playerActivate = GameManager.Instance.PlayerActivate;
            playerMouseLook = GameManager.Instance.PlayerMouseLook;
            mouselookToggle = false;

            hideWindowKey = InputManager.Instance.GetBinding(InputManager.Actions.Sneak);
        }

        public override void OnPop()
        {
            base.OnPop();

            if (previewGo != null)
            {
                if (editMode)
                    DecoratorHelper.SetPlacedObject(lastPlacedObjectData, previewGo);
                else
                    GameObject.Destroy(previewGo);
            }

            previewLight = null;
            previewCollider = null;

            listPanel.Components.Clear();

            SetMouselook(true);
        }

        public override void OnReturn()
        {
            base.OnReturn();

            GameManager.Instance.PauseGame(false);
        }

        #endregion Unity

        #region Private Methods

        private void EditMode()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CheckClick())
                    return;

                Camera mainCamera = GameManager.Instance.MainCamera;
                Vector3 screenPos = Input.mousePosition;

                if (DaggerfallUnity.Settings.RetroRenderingMode > 0)
                {
                    // Need to scale viewport position to match actual screen area when retro rendering enabled
                    float screenHeight = Screen.height;
                    if (DaggerfallUI.Instance.DaggerfallHUD != null && DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled && DaggerfallUnity.Settings.LargeHUDDocked)
                        screenHeight = Screen.height - DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ScreenHeight;
                    float xm = screenPos.x / mainCamera.targetTexture.width;
                    float ym = screenPos.y / mainCamera.targetTexture.height;
                    screenPos = new Vector3(Screen.width * xm, screenHeight - screenHeight * ym, Input.mousePosition.z);
                }

                RaycastHit hit;
                Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(screenPos);

                if (Physics.Raycast(ray, out hit, 15f))
                {
                    PlacedObject placedObject;

                    if (placedObject = hit.transform.GetComponent<PlacedObject>())
                    {
                        PlacedObjectData_v2 placedObjectData = placedObject.GetData();

                        // Revert last object un-set changes
                        if (previewGo != null)
                            DecoratorHelper.SetPlacedObject(lastPlacedObjectData, previewGo);

                        lastPlacedObjectData = placedObject.GetData();

                        // Set all light checkboxes and sliders to selected objects settings
                        if (placedObjectData.isLight)
                        {
                            if (placedObjectData.lightType == LightType.Point)
                                lightSpotCheckbox.IsChecked = false;
                            else
                                lightSpotCheckbox.IsChecked = true;

                            colorPicker.BackgroundColor = placedObjectData.lightColor;
                            lightIntensitySlider.SetValue(placedObjectData.lightIntensity);
                            lightSpotAngleSlider.SetValue(placedObjectData.lightSpotAngle);
                            lightHorizontalRotationSlider.SetValue(placedObjectData.lightHorizontalRotation);
                            lightVerticalRotationSlider.SetValue(placedObjectData.lightVerticalRotation);

                            lightCheckbox.IsChecked = true;
                        }
                        else
                        {
                            // Set isLight to true so EditMode can apply new light settings
                            lightCheckbox.IsChecked = false;
                            ResetLight();
                            placedObjectData.isLight = true;
                        }

                        // Set all scale sliders and checkboxes to selected object settings
                        if (placedObjectData.localScale != Vector3.one)
                        {
                            scaleCheckBox.IsChecked = true;
                            scaleXSlider.SetValue(placedObjectData.localScale.x);
                            scaleYSlider.SetValue(placedObjectData.localScale.y);
                            scaleZSlider.SetValue(placedObjectData.localScale.z);
                        }
                        else
                        {
                            scaleCheckBox.IsChecked = false;
                            ResetScale();
                        }

                        // Set all special checkboxes
                        containerCheckbox.IsChecked = placedObjectData.isContainer;
                        potionMakerCheckbox.IsChecked = placedObjectData.isPotionMaker;
                        spellMakerCheckbox.IsChecked = placedObjectData.isSpellMaker;
                        itemMakerCheckbox.IsChecked = placedObjectData.isItemMaker;

                        DecoratorHelper.SetPlacedObject(placedObjectData, placedObject.gameObject);

                        previewGo = placedObject.gameObject;
                        previewCollider = placedObject.GetComponent<BoxCollider>();
                        previewLight = placedObject.transform.GetComponentInChildren<Light>();

                        IgnoreRaycasts(previewGo);
                    }
                }
            }
        }

        private void SetPreviewGameObject(PlacedObjectData_v2 data, Transform parent)
        {
            if (previewGo != null)
            {
                if (data == lastPlacedObjectData)
                    return;

                GameObject.Destroy(previewGo);
                ResetPreview();
            }

            data.lightColor = colorPicker.BackgroundColor;
            data.lightIntensity = lightIntensitySlider.GetValue();
            data.lightSpotAngle = lightSpotAngleSlider.GetValue();

            if (lightSpotCheckbox.IsChecked)
                data.lightType = LightType.Spot;
            else
                data.lightType = LightType.Point;

            data.lightHorizontalRotation = lightHorizontalRotationSlider.GetValue();
            data.lightVerticalRotation = lightVerticalRotationSlider.GetValue();

            previewGo = DecoratorHelper.CreatePlacedObject(data, parent, true);

            previewCollider = previewGo.GetComponent<BoxCollider>();
            previewLight = previewGo.transform.GetComponentInChildren<Light>();

            IgnoreRaycasts(previewGo);

            ResetTransform();

            lastPlacedObjectData = data;
        }

        private void GenerateButtons(List<Dictionary<string, string>> dictionaryList)
        {
            float xPosition = 0f;
            float yPosition = 0f;
            float scale;

            if (DaggerfallUnity.Settings.SDFFontRendering)
                scale = 0.7f;
            else
                scale = 0.5f;

            foreach (Dictionary<string, string> dictionary in dictionaryList)
            {
                string name = DecoratorHelper.Parse("-1", dictionary).name;
                float xSize = name.Length * 3f;

                if (xPosition + xSize > mainPanel.Size.x)
                {
                    xPosition = 0f;
                    yPosition = 10f;
                }

                Vector2 position = new Vector2(xPosition, yPosition);
                Vector2 size = new Vector2(xSize, 9f);

                Button button = DaggerfallUI.AddButton(position, size, mainPanel);
                button.Label.Text = name;
                button.Label.TextScale = scale;
                button.Label.ShadowColor = Color.black;

                button.OnMouseClick += (sender, pos) =>
                {
                    currentDictionary = dictionary;
                    PopulateList(dictionary, 1);
                };

                xPosition += xSize;
            }
        }

        private void PopulateList(Dictionary<string, string> dictionary, int page)
        {
            float listPosition = 10f;
            float yPos = 8.0f;
            int items = 0;
            pages = 1;
            pageSpinner.Value = page;

            listPanel.Components.Clear();
            listPanel.Components.Add(pageSpinner);

            foreach (KeyValuePair<string, string> entry in dictionary)
            {
                if (entry.Key == "-1")
                    continue;

                // TODO: Ugly, horrible, redo pages

                int itemsPerPage = 16;

                if (page == 1)
                {
                    if (items > itemsPerPage)
                    {
                        pages = 2;
                        break;
                    }
                }
                else if (page == 2)
                {
                    if (items <= itemsPerPage)
                    {
                        items++;
                        pages = 2;
                        continue;
                    }
                    else if (items > itemsPerPage * 2)
                    {
                        pages = 3;
                        break;
                    }
                }
                else if (page == 3)
                {
                    if (items <= itemsPerPage * 2)
                    {
                        items++;
                        pages = 3;
                        continue;
                    }
                    else if (items > itemsPerPage * 3)
                    {
                        pages = 4;
                        break;
                    }
                }
                else if (page == 4)
                {
                    if (items <= itemsPerPage * 3)
                    {
                        items++;
                        pages = 4;
                        continue;
                    }
                    else if (items > itemsPerPage * 4)
                    {
                        pages = 5;
                        break;
                    }
                }
                else if (page == 5)
                {
                    if (items <= itemsPerPage * 4)
                    {
                        items++;
                        pages = 5;
                        continue;
                    }
                    else if (items > itemsPerPage * 5)
                    {
                        pages = 6;
                        break;
                    }
                }
                else if (page == 6)
                {
                    if (items <= itemsPerPage * 5)
                    {
                        items++;
                        pages = 5;
                        continue;
                    }
                    else if (items > itemsPerPage * 6)
                    {
                        pages = 6;
                        break;
                    }
                }

                PlacedObjectData_v2 data = new PlacedObjectData_v2();

                data = DecoratorHelper.Parse(entry.Key, dictionary);

                Vector2 position = new Vector2(1.0f, listPosition);
                Vector2 size = new Vector2((data.name.Length * 5f), 5f);
                Rect rect = new Rect(position, size);

                Button button = DaggerfallUI.AddButton(rect, listPanel);
                button.Label.Text = data.name;
                button.Label.ShadowColor = Color.black;
                button.ClickSound = ClickSound;

                button.Label.HorizontalAlignment = HorizontalAlignment.Left;

                button.OnMouseClick += (sender, pos) =>
                {
                    SetPreviewGameObject(data, Player);
                };

                listPosition += yPos;
                items++;
            }
        }

        private void SetObjectPosition()
        {
            snapRay.origin = Player.position;
            snapRay.direction = Player.forward;

            if (Physics.Raycast(snapRay, out snapRayHit, 5.0f))
            {
                if (previewCollider.size.z > 0)
                    GetRotation();

                Vector3 snapPosition = Player.InverseTransformPoint(snapRayHit.point);

                snapPosition.y = previewGo.transform.localPosition.y;

                snapPosition.z -= GetOffset();

                previewGo.transform.localPosition = snapPosition;
                lastPosition = snapPosition;
            }
            else
                previewGo.transform.localPosition = lastPosition;
        }

        private void SetObjectHeight(int setting)
        {
            if (previewGo == null)
                return;

            if (setting == 0)
                return;

            Vector3 origin = previewCollider.transform.TransformPoint(previewCollider.center);
            Vector3 originOffset = previewCollider.transform.position - origin;
            Vector3 direction;

            //TODO: Medium button finds mid-point between cieling and floor. For now add a bit to default spot.
            if (setting == 2)
            {
                Vector3 newPos = defaultPosition;
                newPos.y += 0.3f;

                previewGo.transform.localPosition = lastPosition = newPos;

                return;
            }

            float yOffset = (previewCollider.size.y / 2) * previewCollider.transform.localScale.y;

            if (previewCollider.size.z == 0)
                yOffset += 0.01f;

            if (setting == 1)
                direction = Vector3.down;
            else
                direction = Vector3.up;

            Ray ray = new Ray(origin, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5.0f))
            {
                Vector3 position = hit.point;

                if (setting == 1)
                    position.y += yOffset;
                else
                    position.y -= yOffset;

                position += originOffset;

                if (!editMode)
                    previewGo.transform.localPosition = lastPosition = Player.InverseTransformPoint(position);
                else
                    previewGo.transform.position = position;
            }
        }

        private float GetOffset()
        {
            float offset;
            float xScale = previewCollider.transform.localScale.x;
            float zScale = previewCollider.transform.localScale.z;

            if (previewCollider.size.z > 0)
            {
                float xOffset = (previewCollider.size.x / 2) * xScale;
                float zOffset = (previewCollider.size.z / 2) * zScale;

                float xCenter = previewCollider.center.x * xScale;
                float zCenter = previewCollider.center.z * zScale;

                if (goRotation == 0 || goRotation == 2)
                    offset = zOffset + zCenter;
                else
                    offset = xOffset + xCenter;
            }
            else
                offset = (previewCollider.size.x / 2) * xScale;

            offset += 0.2f;

            return offset;
        }

        private void GetRotation()
        {
            if (goRotation == 0)
                previewGo.transform.forward = snapRayHit.normal;
            else if (goRotation == 1)
                previewGo.transform.forward = Vector3.Cross(snapRayHit.normal, Vector3.up);
            else if (goRotation == 2)
                previewGo.transform.forward = -snapRayHit.normal;
            else
                previewGo.transform.forward = Vector3.Cross(snapRayHit.normal, -Vector3.up);
        }

        private void ResetTransform()
        {
            if (previewGo == null)
                return;

            goRotation = 0;
            goHeight = 2;

            if (!editMode)
            {
                previewGo.transform.localPosition = lastPosition = defaultPosition;
                previewGo.transform.forward = -Player.forward;
            }
            else
            {
                DecoratorHelper.SetPlacedObject(lastPlacedObjectData, previewGo);
                IgnoreRaycasts(previewGo);
            }
        }

        private bool CheckClick()
        {
            if (transformPanel.MouseOverComponent ||
                mainPanel.MouseOverComponent ||
                lightPanel.MouseOverComponent ||
                listPanel.MouseOverComponent ||
                scalePanel.MouseOverComponent ||
                transformSubPanel1.MouseOverComponent ||
                transformSubPanel2.MouseOverComponent)
            {
                return true;
            }

            return false;
        }

        private void ResetScale()
        {
            scaleXSlider.SetValue(1.0f);
            scaleYSlider.SetValue(1.0f);
            scaleZSlider.SetValue(1.0f);
        }

        private void ResetPreview()
        {
            previewGo = null;
            previewLight = null;
            previewCollider = null;
        }

        private void ResetLight()
        {
            lightIntensitySlider.SetValue(1.0f);
            lightSpotAngleSlider.SetValue(90.0f);
            lightHorizontalRotationSlider.SetValue(90.0f);
            lightVerticalRotationSlider.SetValue(90.0f);
            colorPicker.BackgroundColor = Color.white;

            lightSpotCheckbox.IsChecked = false;
        }

        private void SetMouselook(bool setting)
        {
            playerMouseLook.enableMouseLook = setting;
            playerMouseLook.lockCursor = setting;
            playerMouseLook.simpleCursorLock = !setting;

            if (!editMode)
            {
                mainPanel.Enabled = !setting;
                transformPanel.Enabled = !setting;
                listPanel.Enabled = !setting;
            }
        }

        private void IgnoreRaycasts(GameObject placedObject)
        {
            placedObject.layer = 2;

            if (placedObject.transform.childCount > 0)
                foreach (Transform child in placedObject.transform)
                    IgnoreRaycasts(child.gameObject);
        }

        #endregion Private Methods

        #region Events

        #region Transform Panel

        private void AcceptButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            PlacedObject placedObject = previewGo.GetComponent<PlacedObject>();
            PlacedObjectData_v2 data = placedObject.GetData();

            if (lightCheckbox.IsChecked)
            {
                data.isLight = true;
                data.lightColor = colorPicker.BackgroundColor;
                data.lightIntensity = lightIntensitySlider.GetValue();

                if (lightSpotCheckbox.IsChecked)
                    data.lightType = LightType.Spot;
                else
                    data.lightType = LightType.Point;

                data.lightHorizontalRotation = lightHorizontalRotationSlider.GetValue();
                data.lightVerticalRotation = lightVerticalRotationSlider.GetValue();
                data.lightSpotAngle = lightSpotAngleSlider.GetValue();
            }
            else
            {
                data.isLight = false;
            }

            if (containerCheckbox.IsChecked)
                data.isContainer = true;
            else if (data.isContainer == true)
            {
                string message = "You must use the Delete button to remove containers.";

                DaggerfallMessageBox mb = new DaggerfallMessageBox(uiManager, this);
                mb.ParentPanel.BackgroundColor = Color.clear;
                mb.ClickAnywhereToClose = true;

                mb.SetText(message);
                mb.Show();

                containerCheckbox.IsChecked = true;
                potionMakerCheckbox.IsChecked = false;
                spellMakerCheckbox.IsChecked = false;
                itemMakerCheckbox.IsChecked = false;
            }

            data.isPotionMaker = potionMakerCheckbox.IsChecked;
            data.isSpellMaker = spellMakerCheckbox.IsChecked;
            data.isItemMaker = itemMakerCheckbox.IsChecked;

            if (scaleCheckBox.IsChecked)
                data.localScale = new Vector3(scaleXSlider.GetValue(), scaleYSlider.GetValue(), scaleZSlider.GetValue());
            else
                data.localScale = Vector3.one;

            if (!editMode)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                PlayerGPS playerGPS = GameManager.Instance.PlayerGPS;
                int amount = DecoratorManager.Instance.PlaceObjectCost;
                int playerGold = playerEntity.GetGoldAmount();
                int accountGold = DaggerfallBankManager.BankAccounts[playerGPS.CurrentRegionIndex].accountGold;

                if (playerGold + accountGold >= amount)
                {
                    amount = playerEntity.DeductGoldAmount(amount);
                    DaggerfallBankManager.BankAccounts[playerGPS.CurrentRegionIndex].accountGold -= amount;

                    // Set data position and rotation to the correct Parent, since it's currently in relation to the Player.
                    data.localPosition = Parent.InverseTransformPoint(previewGo.transform.position);
                    data.localRotation = Quaternion.Inverse(Parent.rotation) * previewGo.transform.rotation;

                    DecoratorHelper.CreatePlacedObject(data, Parent);
                }
                else
                    DaggerfallUI.MessageBox("Not enough gold.");
            }
            else
            {
                DecoratorHelper.SetPlacedObject(data, previewGo);
                ResetPreview();
            }
        }

        private void UpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            if (!editMode)
            {
                Vector3 goPosition = previewGo.transform.localPosition;

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    goPosition.y += 0.1f;
                }
                else
                    goPosition.z += 0.1f;

                previewGo.transform.localPosition = lastPosition = goPosition;
            }
            else
            {
                Vector3 goPosition = previewGo.transform.InverseTransformPoint(previewGo.transform.position);

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    goPosition.y += 0.1f;
                }
                else
                    goPosition.z += 0.1f;

                previewGo.transform.position = previewGo.transform.TransformPoint(goPosition);
            }
        }

        private void DownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            if (!editMode)
            {
                Vector3 goPosition = previewGo.transform.localPosition;

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                    goPosition.y -= 0.1f;
                else
                    goPosition.z -= 0.1f;

                previewGo.transform.localPosition = lastPosition = goPosition;
            }
            else
            {
                Vector3 goPosition = previewGo.transform.InverseTransformPoint(previewGo.transform.position);

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                    goPosition.y -= 0.1f;
                else
                    goPosition.z -= 0.1f;

                previewGo.transform.position = previewGo.transform.TransformPoint(goPosition);
            }
        }

        private void LeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            if (!editMode)
            {
                Vector3 goPosition = previewGo.transform.localPosition;
                goPosition.x -= 0.1f;

                previewGo.transform.localPosition = lastPosition = goPosition;
            }
            else
            {
                Vector3 goPosition = previewGo.transform.InverseTransformPoint(previewGo.transform.position);
                goPosition.x += 0.1f;

                previewGo.transform.position = previewGo.transform.TransformPoint(goPosition);
            }
        }

        private void RightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            if (!editMode)
            {
                Vector3 goPosition = previewGo.transform.localPosition;
                goPosition.x += 0.1f;

                previewGo.transform.localPosition = lastPosition = goPosition;
            }
            else
            {
                Vector3 goPosition = previewGo.transform.InverseTransformPoint(previewGo.transform.position);
                goPosition.x -= 0.1f;

                previewGo.transform.position = previewGo.transform.TransformPoint(goPosition);
            }
        }

        private void RotateXZLeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            Quaternion rotation = previewGo.transform.rotation;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                rotation *= Quaternion.Euler(5.0f, 0.0f, 0.0f);
            else
                rotation *= Quaternion.Euler(0.0f, 0.0f, -5.0f);

            previewGo.transform.rotation = rotation;
        }

        private void RotateXZRightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            Quaternion rotation = previewGo.transform.rotation;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                rotation *= Quaternion.Euler(-5.0f, 0.0f, 0.0f);
            else
                rotation *= Quaternion.Euler(0.0f, 0.0f, 5.0f);

            previewGo.transform.rotation = rotation;
        }

        private void RotateLeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            if (snapCheckbox.IsChecked)
            {
                if (goRotation - 1 == -1)
                    goRotation = 3;
                else
                    goRotation = Mathf.Max(0, goRotation - 1);
            }
            else
            {
                Quaternion rotation = previewGo.transform.rotation;

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                    rotation *= Quaternion.Euler(0.0f, 10.0f, 0.0f);
                else
                    rotation *= Quaternion.Euler(0.0f, 5.0f, 0.0f);

                previewGo.transform.rotation = rotation;
            }
        }

        private void RotateRightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            if (snapCheckbox.IsChecked)
            {
                if (goRotation + 1 == 4)
                    goRotation = 0;
                else
                    goRotation = Mathf.Min(3, goRotation + 1);
            }
            else
            {
                Quaternion rotation = previewGo.transform.rotation;

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                    rotation *= Quaternion.Euler(0.0f, -10.0f, 0.0f);
                else
                    rotation *= Quaternion.Euler(0.0f, -5.0f, 0.0f);

                previewGo.transform.rotation = rotation;
            }
        }

        private void EditButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (editCheckBox.IsChecked)
            {
                if (previewGo != null)
                {
                    GameObject.Destroy(previewGo);
                    ResetPreview();
                }

                editMode = true;

                listPanel.Components.Clear();

                mainPanel.Enabled = false;
                listPanel.Enabled = false;

                snapCheckbox.IsChecked = false;
                snapCheckbox.Enabled = false;
                deleteButton.Enabled = true;
            }
            else
            {
                if (previewGo != null)
                {
                    DecoratorHelper.SetPlacedObject(lastPlacedObjectData, previewGo);
                    ResetPreview();
                }

                editMode = false;

                mainPanel.Enabled = true;
                listPanel.Enabled = true;

                snapCheckbox.Enabled = true;
                deleteButton.Enabled = false;
            }
        }

        private void ContainerCheckbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (containerCheckbox.IsChecked)
            {
                potionMakerCheckbox.IsChecked = false;
                spellMakerCheckbox.IsChecked = false;
                itemMakerCheckbox.IsChecked = false;
            }
        }

        private void SpellMakerCheckbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (spellMakerCheckbox.IsChecked)
            {
                potionMakerCheckbox.IsChecked = false;
                containerCheckbox.IsChecked = false;
                itemMakerCheckbox.IsChecked = false;
            }
        }

        private void PotionMakerCheckbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (potionMakerCheckbox.IsChecked)
            {
                spellMakerCheckbox.IsChecked = false;
                containerCheckbox.IsChecked = false;
                itemMakerCheckbox.IsChecked = false;
            }
        }

        private void ItemMakerCheckbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (itemMakerCheckbox.IsChecked)
            {
                spellMakerCheckbox.IsChecked = false;
                containerCheckbox.IsChecked = false;
                potionMakerCheckbox.IsChecked = false;
            }
        }

        private void DeleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            string message;

            if (previewGo.GetComponent<PlacedObject>().GetData().isContainer)
                message = "Are you sure? All items will be lost.";
            else
                message = "Are you sure?";

            DaggerfallMessageBox mb = new DaggerfallMessageBox(uiManager);
            mb.ParentPanel.BackgroundColor = Color.clear;

            mb.SetText(message);
            mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);

            mb.OnButtonClick += (_sender, button) =>
            {
                _sender.CloseWindow();
                if (button == DaggerfallMessageBox.MessageBoxButtons.Yes)
                {
                    GameObject.Destroy(previewGo);
                    ResetPreview();
                }
            };
            mb.Show();
        }

        private void LowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            goHeight = 1;
            SetObjectHeight(1);
        }

        private void MedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            goHeight = 2;
            SetObjectHeight(2);
        }

        private void HighButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            goHeight = 3;
            SetObjectHeight(3);
        }

        private void ResetObjectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ResetTransform();
        }

        private void ScaleResetButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ResetScale();
        }

        private void Slider_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            HorizontalSlider slider = (HorizontalSlider)sender;

            DaggerfallInputMessageBox imb = new DaggerfallInputMessageBox(uiManager, this);

            imb.SetTextBoxLabel("Value:");
            imb.TextPanelDistanceX = 5;
            imb.TextPanelDistanceY = 9;
            imb.TextBox.Text = slider.GetValue().ToString("0.0");
            imb.TextBox.Numeric = true;
            imb.TextBox.NumericMode = NumericMode.Float;
            imb.TextBox.MaxCharacters = 5;

            imb.OnGotUserInput += (_sender, input) =>
            {
                float value;

                bool result = float.TryParse(input, out value);
                if (!result)
                    return;

                slider.SetValue(value);
            };

            imb.Show();
        }

        #endregion Transform Panel

        #region Main Panel

        private void Page_OnRightButtonClicked()
        {
            if (currentDictionary == null)
                return;

            pageSpinner.Value = Math.Min(pages, pageSpinner.Value + 1);
            PopulateList(currentDictionary, pageSpinner.Value);
        }

        private void Page_OnLeftButtonClicked()
        {
            if (currentDictionary == null)
                return;

            pageSpinner.Value = Math.Max(1, pageSpinner.Value - 1);
            PopulateList(currentDictionary, pageSpinner.Value);
        }

        #endregion Main Panel

        #endregion Events
    }

    public class DecoratorDebugWindow : DaggerfallPopupWindow
    {
        private PlayerMouseLook playerMouseLook;
        private bool mouselookToggle;
        private KeyCode hideWindowKey;

        private Rect mainPanelRect = new Rect(0.0f, 0.0f, 70f, 85f);
        private Panel mainPanel;

        private Button upButton;
        private Button downButton;

        private Button setIncrementButton;

        private Button ImportOldDataButton;

        private TextLabel incrementLabel;
        private TextLabel incrementValue;

        private TextLabel lowestYLabel;
        private TextLabel lowestYValue;
        private TextLabel lowestYDesc;

        private TextLabel currentYLabel;
        private TextLabel currentYValue;

        private Transform Parent;

        private float increment = 0.1f;
        private float lowestY;

        private float fontScale = 0.9f;

        public DecoratorDebugWindow(IUserInterfaceManager uiManager, Transform parent)
        : base(uiManager)
        {
            ParentPanel.BackgroundColor = Color.clear;
            PauseWhileOpen = false;
            Parent = parent;
        }

        protected override void Setup()
        {
            mainPanel = DaggerfallUI.AddPanel(mainPanelRect);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Top;
            mainPanel.BackgroundColor = Color.black;
            mainPanel.Outline.Enabled = true;

            upButton = DaggerfallUI.AddButton(new Vector2(2.0f, 0.0f), new Vector2(15, 10), mainPanel);
            upButton.HorizontalAlignment = HorizontalAlignment.Center;
            upButton.VerticalAlignment = VerticalAlignment.Top;
            upButton.Label.Text = "Up";
            upButton.Label.TextScale = fontScale;
            upButton.Outline.Enabled = true;
            upButton.OnMouseClick += upButton_OnMouseClick;

            downButton = DaggerfallUI.AddButton(new Vector2(2.0f, 12.0f), new Vector2(15, 10), mainPanel);
            downButton.HorizontalAlignment = HorizontalAlignment.Center;
            downButton.Label.Text = "Down";
            downButton.Label.TextScale = fontScale;
            downButton.Outline.Enabled = true;
            downButton.OnMouseClick += downButton_OnMouseClick;

            setIncrementButton = DaggerfallUI.AddButton(new Vector2(0.0f, 35.0f), new Vector2(37, 9), mainPanel);
            setIncrementButton.HorizontalAlignment = HorizontalAlignment.Center;
            setIncrementButton.Label.Text = "Set Increment";
            setIncrementButton.Label.TextScale = fontScale;
            setIncrementButton.Outline.Enabled = true;
            setIncrementButton.OnMouseClick += setIncrementButton_OnMouseClick;

            incrementLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 24.0f), "Increment", mainPanel);
            incrementLabel.HorizontalAlignment = HorizontalAlignment.Center;
            incrementLabel.TextScale = fontScale;

            incrementValue = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0.0f, 29.0f), "", mainPanel);
            incrementValue.TextScale = fontScale;
            incrementValue.HorizontalAlignment = HorizontalAlignment.Center;

            lowestYLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(1.0f, 48), "Lowest Y: ", mainPanel);
            lowestYLabel.TextScale = fontScale;

            lowestYValue = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(43.0f, 48), "", mainPanel);
            lowestYValue.TextScale = fontScale;

            currentYLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(1.0f, 55), "Target Y: ", mainPanel);
            currentYLabel.TextScale = fontScale;

            currentYValue = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(43.0f, 55), GameManager.Instance.PlayerEnterExit.Interior.transform.position.y.ToString("0.00"), mainPanel);
            currentYValue.TextScale = fontScale;

            NativePanel.Components.Add(mainPanel);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(hideWindowKey))
                mouselookToggle = !mouselookToggle;

            SetMouselook(mouselookToggle);

            incrementValue.Text = increment.ToString("0.00");
            lowestYValue.Text = lowestY.ToString("0.00");
        }

        public override void OnPush()
        {
            base.OnPush();

            if (!DaggerfallUnity.Settings.SDFFontRendering)
                fontScale = 0.7f;

            lowestY = GetLowestY();

            playerMouseLook = GameManager.Instance.PlayerMouseLook;
            mouselookToggle = false;

            hideWindowKey = InputManager.Instance.GetBinding(InputManager.Actions.Sneak);
        }

        public override void OnPop()
        {
            base.OnPop();

            //foreach (Transform child in Parent)
            //{
            //    PlacedObject placedObject = child.GetComponent<PlacedObject>();
            //    PlacedObjectData_v2 data = placedObject.GetData();
            //    child.GetComponent<PlacedObject>().SetData(data);
            //}

            SetMouselook(true);
        }

        private float GetLowestY()
        {
            float lowest = float.MaxValue;

            if (Parent.childCount > 0)
            {
                foreach (Transform child in Parent)
                {
                    if (child.position.y < lowest)
                        lowest = child.position.y;
                }
            }
            else
                lowest = float.NaN;

            return lowest;
        }

        private void SetMouselook(bool setting)
        {
            playerMouseLook.enableMouseLook = setting;
            playerMouseLook.lockCursor = setting;
            playerMouseLook.simpleCursorLock = !setting;
        }

        private void upButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            foreach (Transform child in Parent)
            {
                Vector3 newPosition = child.position;
                newPosition.y += increment;
                child.position = newPosition;
            }

            lowestY = GetLowestY();
        }

        private void downButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            foreach (Transform child in Parent)
            {
                Vector3 newPosition = child.position;
                newPosition.y -= increment;
                child.position = newPosition;
            }

            lowestY = GetLowestY();
        }

        private void setIncrementButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox imb = new DaggerfallInputMessageBox(uiManager, this);

            imb.SetTextBoxLabel("Value:");
            imb.TextPanelDistanceX = 5;
            imb.TextPanelDistanceY = 9;
            imb.TextBox.Text = increment.ToString("0.00");
            imb.TextBox.Numeric = true;
            imb.TextBox.NumericMode = NumericMode.Float;
            imb.TextBox.MaxCharacters = 6;

            imb.OnGotUserInput += (_sender, input) =>
            {
                float value;

                bool result = float.TryParse(input, out value);
                if (!result)
                    return;

                increment = value;
            };

            imb.Show();
        }
    }
}