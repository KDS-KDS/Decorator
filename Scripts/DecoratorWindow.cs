using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game;
using System;
using System.Collections.Generic;

namespace Decorator
{
    public class DecoratorWindow : DaggerfallPopupWindow
    {
        #region Fields

        //Mainpanel
        Rect mainPanelRect = new Rect(0.0f, 0.0f, 120f, 20.0f);
        Rect listPanelRect = new Rect(0.0f, 22.0f, 120f, 150f);

        Panel mainPanel;
        Panel listPanel;
        LeftRightSpinner pageSpinner;

        // Transform panel
        Rect transformPanelRect = new Rect(121.0f, 0.0f, 108f, 53f);
        Rect transformSubPanel1Rect = new Rect(0.0f, 0.0f, 108f, 41f);
        Rect transformSubPanel2Rect = new Rect(0.0f, 41f, 108f, 12f);
        Rect upButtonRect = new Rect(48f, 0f, 11f, 10f);
        Rect downButtonRect = new Rect(48f, 31f, 11f, 10f);
        Rect leftButtonRect = new Rect(30f, 15f, 10f, 11f);
        Rect rightButtonRect = new Rect(66f, 15f, 11f, 11f);
        Rect rotateXZRightButtonRect = new Rect(61f, 6f, 9f, 9f);
        Rect rotateXZLeftButtonRect = new Rect(37f, 6f, 9f, 9f);
        Rect rotateLeftButtonRect = new Rect(37f, 26f, 9f, 9f);
        Rect rotateRightButtonRect = new Rect(61f, 26f, 9f, 9f);
        Rect acceptButtonRect = new Rect(50f, 17f, 7f, 7f);

        Rect lowButtonRect = new Rect(7f, 42f, 29f, 10f);
        Rect medButtonRect = new Rect(36f, 42f, 32f, 10f);
        Rect highButtonRect = new Rect(68f, 42f, 32f, 10f);

        Rect resetButtonRect = new Rect(0.0f, 26.0f, 20f, 7f);
        Rect deleteButtonRect = new Rect(2.0f, 34.5f, 18f, 7f);

        Texture2D transformSubPanelTexture1;
        Texture2D transformSubPanelTexture2;

        Panel transformPanel;
        Panel transformSubPanel1;
        Panel transformSubPanel2;

        Button upButton;
        Button downButton;
        Button leftButton;
        Button rightButton;
        Button rotateXZRightButton;
        Button rotateXZLeftButton;
        Button rotateLeftButton;
        Button rotateRightButton;
        Button acceptButton;
        Button resetButton;
        Button deleteButton;

        Button lowButton;
        Button medButton;
        Button highButton;

        Checkbox scaleCheckBox;
        Checkbox snapCheckbox;
        Checkbox editCheckBox;
        Checkbox lightCheckbox;
        Checkbox containerCheckbox;
        Checkbox potionMakerCheckbox;
        Checkbox spellMakerCheckbox;

        // Light panel
        Rect lightPanelRect = new Rect(230.0f, 0.0f, 90.0f, 53.0f);

        Panel lightPanel;
        Checkbox lightSpotCheckbox;
        //Checkbox       lightFlickerCheckbox;
        HorizontalSlider lightIntensitySlider;
        HorizontalSlider lightSpotAngleSlider;
        HorizontalSlider lightHorizontalRotationSlider;
        HorizontalSlider lightVerticalRotationSlider;
        Button colorPicker;

        //Scale panel
        Rect scalePanelRect = new Rect(230.0f, 54.0f, 90.0f, 40.0f);
        Rect scaleResetButtonRect = new Rect(0.0f, 0.0f, 25.0f, 10.0f);

        Panel scalePanel;
        Button scaleResetButton;
        HorizontalSlider scaleXSlider;
        HorizontalSlider scaleYSlider;
        HorizontalSlider scaleZSlider;

        Transform Parent;
        GameObject previewGo;
        Light previewLight;
        BoxCollider previewCollider;
        PlacedObjectData_v1 lastPlacedObjectData;
        PlayerMouseLook playerMouseLook;

        Vector3 defaultPosition = new Vector3(0.0f, 0.1f, 2f);
        Vector3 lastPosition = Vector3.zero;

        Ray snapRay = new Ray();
        RaycastHit snapRayHit = new RaycastHit();

        bool editMode;
        int goHeight = 2;
        int goRotation = 0;
        int pages = 1;

        KeyCode hideWindowKey;
        bool mouselookToggle = true;

        bool colorPickerEnabled;

        Dictionary<string, string> common = new Dictionary<string, string>()
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

        Dictionary<string, string> containers = new Dictionary<string, string>()
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

        Dictionary<string, string> lights = new Dictionary<string, string>()
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

        Dictionary<string, string> wall = new Dictionary<string, string>()
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

        Dictionary<string, string> library = new Dictionary<string, string>()
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

        Dictionary<string, string> misc1 = new Dictionary<string, string>()
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

        Dictionary<string, string> misc2 = new Dictionary<string, string>()
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
            {"74226", "Suit of Armor" },
            {"74231", "Marble Diamond" },
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
            {"41125", "Wooden Stand" },
            {"41409", "Ladder" },
            {"41020", "Podium 1" },
            {"41021", "Podium 2" },
            {"41739", "Sign" },
            {"41703", "Booth" },
        };

        Dictionary<string, string> alchemy = new Dictionary<string, string>()
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

        Dictionary<string, string> bio = new Dictionary<string, string>()
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

        Dictionary<string, string> treasure = new Dictionary<string, string>()
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

        List<Dictionary<string, string>> dictionaryList = new List<Dictionary<string, string>>();

        Dictionary<string, string> currentDictionary;

        #endregion

        #region Properties

        Transform Player { get { return GameManager.Instance.PlayerObject.transform; } }
        AudioClip ClickSound { get { return DecoratorManager.Instance.DecoratorAudio.GetAudioClip(360); } }
        
        #endregion

        #region Constructor

        public DecoratorWindow(IUserInterfaceManager uiManager, Transform parent)
        : base(uiManager)
        {
            ParentPanel.BackgroundColor = Color.clear;
            PauseWhileOpen = false;

            Parent = parent;

            dictionaryList.AddRange(new List<Dictionary<string, string>>() { common, containers, lights, wall, library, misc1, misc2, alchemy, bio, treasure });

            hideWindowKey = InputManager.Instance.GetBinding(InputManager.Actions.Sneak);
        }

        #endregion

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

            lightCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 2.0f), false, transformPanel);
            lightCheckbox.Label.Text = "Light";

            containerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 10.0f), false, transformPanel);
            containerCheckbox.Label.Text = "Container";
            containerCheckbox.OnMouseClick += ContainerCheckbox_OnMouseClick;

            potionMakerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 18.0f), false, transformPanel);
            potionMakerCheckbox.Label.Text = "Potion";
            potionMakerCheckbox.OnMouseClick += PotionMakerCheckbox_OnMouseClick;

            spellMakerCheckbox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 26.0f), false, transformPanel);
            spellMakerCheckbox.Label.Text = "Spell";
            spellMakerCheckbox.OnMouseClick += SpellMakerCheckbox_OnMouseClick;

            scaleCheckBox = DaggerfallUI.AddCheckbox(new Vector2(78.0f, 34.0f), false, transformPanel);
            scaleCheckBox.Label.Text = "Scale";

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

            colorPicker = DaggerfallUI.AddColorPicker(new Vector2(0.0f, 18.0f), new Color32(255, 255, 255, 1), uiManager, this, lightPanel);
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

        void LoadTextures()
        {
            Rect arrowsRect = new Rect(0, 25, 108, 41);
            transformSubPanelTexture1 = ImageReader.GetSubTexture(ImageReader.GetTexture("CNFG04I0.IMG"), arrowsRect);

            Rect positionsRect = new Rect(0, 92, 108, 12);
            transformSubPanelTexture2 = ImageReader.GetSubTexture(ImageReader.GetTexture("CNFG04I0.IMG"), positionsRect);
        }

        #endregion

        #region Unity

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(exitKey))
            {
                if (!colorPickerEnabled)
                    CloseWindow();
                else
                    colorPickerEnabled = false;
            }

            if (Input.GetKeyUp(hideWindowKey))
                mouselookToggle = !mouselookToggle;

            if (mouselookToggle)
                SetMouselook(true);
            else
                SetMouselook(false);

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

            playerMouseLook = GameManager.Instance.PlayerMouseLook;
            mouselookToggle = false;
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

        #endregion

        #region Private Methods

        void EditMode()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit, 15f))
                {
                    PlacedObject placedObject;

                    if (placedObject = hit.transform.GetComponent<PlacedObject>())
                    {
                        PlacedObjectData_v1 placedObjectData = placedObject.GetData();

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
                        if (placedObjectData.scale != Vector3.one)
                        {
                            scaleCheckBox.IsChecked = true;
                            scaleXSlider.SetValue(placedObjectData.scale.x);
                            scaleYSlider.SetValue(placedObjectData.scale.y);
                            scaleZSlider.SetValue(placedObjectData.scale.z);
                        }
                        else
                        {
                            scaleCheckBox.IsChecked = false;
                            ResetScale();
                        }

                        if (placedObjectData.isContainer)
                            containerCheckbox.IsChecked = true;
                        else
                            containerCheckbox.IsChecked = false;

                        if (placedObjectData.isPotionMaker)
                            potionMakerCheckbox.IsChecked = true;
                        else
                            potionMakerCheckbox.IsChecked = false;

                        if (placedObjectData.isSpellMaker)
                            spellMakerCheckbox.IsChecked = true;
                        else
                            spellMakerCheckbox.IsChecked = false;

                        DecoratorHelper.SetPlacedObject(placedObjectData, placedObject.gameObject);

                        previewGo = placedObject.gameObject;
                        previewCollider = placedObject.GetComponent<BoxCollider>();
                        previewLight = placedObject.transform.GetComponentInChildren<Light>();

                        IgnoreRaycasts(previewGo);
                    }
                }
            }
        }

        void SetPreviewGameObject(PlacedObjectData_v1 data, Transform parent)
        {
            if (previewGo != null)
            {
                if (data == lastPlacedObjectData)
                {
                    Debug.LogWarning("Same");
                    return;
                }

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

        void GenerateButtons(List<Dictionary<string, string>> dictionaryList)
        {
            float xPosition = 0f;
            float yPosition = 0f;
            float scale = 0.7f;

            foreach (Dictionary<string, string> dictionary in dictionaryList)
            {
                string name = DecoratorHelper.Parse("-1", dictionary).name;
                float xSize = name.Length * 3f;

                if (xPosition + xSize > 120)
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

        void PopulateList(Dictionary<string, string> dictionary, int page)
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

                PlacedObjectData_v1 data = new PlacedObjectData_v1();

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

        void SetObjectHeight(int setting)
        {
            if (previewGo == null)
                return;

            //TODO: Medium button finds mid-point between cieling and floor. For now add a bit to default spot.
            if (setting == 2)
            {
                Vector3 newPos = defaultPosition;
                newPos.y += 3.0f;

                previewGo.transform.localPosition = lastPosition = newPos;

                return;
            }

            Vector3 origin = previewCollider.transform.TransformPoint(previewCollider.center);
            Vector3 originOffset = previewCollider.transform.position - origin;
            Vector3 direction;

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

        float GetOffset()
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

        void GetRotation()
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

        void ResetTransform()
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

        void ResetScale()
        {
            scaleXSlider.SetValue(1.0f);
            scaleYSlider.SetValue(1.0f);
            scaleZSlider.SetValue(1.0f);
        }

        void ResetPreview()
        {
            previewGo = null;
            previewLight = null;
        }

        void ResetLight()
        {
            lightIntensitySlider.SetValue(1.0f);
            lightSpotAngleSlider.SetValue(90.0f);
            lightHorizontalRotationSlider.SetValue(90.0f);
            lightVerticalRotationSlider.SetValue(90.0f);
            colorPicker.BackgroundColor = Color.white;

            lightSpotCheckbox.IsChecked = false;
        }

        void SetMouselook(bool setting)
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

        #endregion

        #region Events

        #region Transform Panel

        void AcceptButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (previewGo == null)
                return;

            PlacedObject placedObject = previewGo.GetComponent<PlacedObject>();
            PlacedObjectData_v1 data = placedObject.GetData();

            if (scaleCheckBox.IsChecked)
                data.scale = new Vector3(scaleXSlider.GetValue(), scaleYSlider.GetValue(), scaleZSlider.GetValue());
            else
                data.scale = Vector3.one;

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
            else
            {
                if (data.isContainer == true)
                {
                    string message = "You must use the Delete button to remove containers.";

                    DaggerfallMessageBox mb = new DaggerfallMessageBox(uiManager);
                    mb.ParentPanel.BackgroundColor = Color.clear;
                    mb.ClickAnywhereToClose = true;

                    mb.SetText(message);
                    mb.Show();

                    if (potionMakerCheckbox.IsChecked)
                        potionMakerCheckbox.IsChecked = false;

                    if (spellMakerCheckbox.IsChecked)
                        spellMakerCheckbox.IsChecked = false;
                }
            }

            if (potionMakerCheckbox.IsChecked)
                data.isPotionMaker = true;
            else
                data.isPotionMaker = false;

            if (spellMakerCheckbox.IsChecked)
                data.isSpellMaker = true;
            else
                data.isSpellMaker = false;

            if (!editMode)
                DecoratorHelper.CreatePlacedObject(data, Parent);
            else
            {
                DecoratorHelper.SetPlacedObject(data, previewGo);
                ResetPreview();
            }
        }

        void UpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void DownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void LeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void RightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void RotateXZLeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void RotateXZRightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void RotateLeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        void RotateRightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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
            }
        }

        private void SpellMakerCheckbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (spellMakerCheckbox.IsChecked)
            {
                potionMakerCheckbox.IsChecked = false;
                containerCheckbox.IsChecked = false;
            }
        }

        private void PotionMakerCheckbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (potionMakerCheckbox.IsChecked)
            {
                spellMakerCheckbox.IsChecked = false;
                containerCheckbox.IsChecked = false;
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

        void LowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            goHeight = 1;
            SetObjectHeight(1);
        }

        void MedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            goHeight = 2;
            SetObjectHeight(2);
        }

        void HighButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
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

        #endregion

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

        #endregion

        #endregion
    }
}