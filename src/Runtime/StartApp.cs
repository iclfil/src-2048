using System.Collections;
using Assets.markins._2048.Runtime.Configs;
using Assets.markins._2048.Runtime.Game.Fabrics;
using Markins.Runtime.Game.Storage;
using Markins.Runtime.Game.Storage.Models;
using deVoid.UIFramework;
using InstantGamesBridge;
using Lean.Localization;
using Markins.Runtime.Game;
using Markins.Runtime.Game.Configs;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.Models;
using Markins.Runtime.Game.Store;
using Markins.Runtime.Game.Views;
using UnityEngine;

public class StartApp : MonoBehaviour
{
    [Header("Configs")]
    public UISettings Settings;
    public GameConfig GameConfig;
    public InventoryConfig InventoryConfig;
    public StoreConfig StoreConfig;
    public RewardConfig RewardConfig;
    public FieldConfig FieldConfig;

    public ChipCollection ChipCollection;
    public FieldCollection FieldCollection;

    public FieldController FieldControllerPrefab;

    [Space]
    public InputPlayerView InputPlayerPrefabView;

    public AimView AimViewPrefab;

    [Space] public LeanLocalization Localization;

    public RuntimePlatform RuntimePlatform;

    private ILoadSaveDataService _loadSaveDataService;
    private IChipFabric _chipFabric;

    public GameModel GameModel;
    public StoreModel StoreModel;
    public SettingsModel SettingsModel;
    public InventoryModel InventoryModel;

    [SerializeField]
    private UIManager uiManager;
    private GameStorage _gameStorage;

    private ShootControlService _shootControlService;

    private GameController _gameController;
    private RewardController _rewardController;
    private FieldController _fieldController;
    [SerializeField]
    private SettingsController _settingsController;
    private InventoryController _inventoryController;
    private StoreController _storeController;

    [SerializeField] private bool IsReleaseBuild = false;

    private void Awake()
    {

        if (IsReleaseBuild)
            Debug.unityLogger.logEnabled = false;

        RuntimePlatform = Application.platform;
        Debug.Log("Platform:" + RuntimePlatform);

        if (RuntimePlatform == RuntimePlatform.WebGLPlayer)
        {
            _loadSaveDataService = new WebLoadSaveDataService();
            Debug.Log("WebLoadDataService");
        }
        else
        {
            _loadSaveDataService = new LocalLoadSaveDataService();
            Debug.Log("LocalLoadDataService");
        }

        _shootControlService = new GameObject("PlayerInputService").AddComponent<ShootControlService>();
        _shootControlService.Init(InputPlayerPrefabView, AimViewPrefab);

        uiManager.Init(Settings);
    }

    private void SetLanguage()
    {
        var platformTranslationCode = Bridge.platform.language;

        Debug.Log("PlatfromCode" + platformTranslationCode);
        var localKey = Localization.DefaultLanguage;
        foreach (var localization in LeanLocalization.CurrentLanguages)
        {
            if (platformTranslationCode == localization.Value.TranslationCode)
            {
                localKey = localization.Key;
                Debug.Log("Has Code in Localizations" + localKey);
                break;
            }
        }


        Localization.CurrentLanguage = localKey;
    }

    private void Start()
    {
        _gameStorage = new GameStorage(_loadSaveDataService);
        _chipFabric = new ChipFabric(ChipCollection);

        CreateModels();
        CreateControllers();
        SetLanguage();

        StartCoroutine(_Initializing());
    }

    private void CreateModels()
    {
        GameModel = new GameModel();
        SettingsModel = GameConfig.CloneSettingsModelByName();
        InventoryModel = new InventoryModel();
        StoreModel = StoreConfig.CloneStoreModel();
        SettingsModel = new SettingsModel();
    }

    private void CreateControllers()
    {
        _fieldController = Instantiate(FieldControllerPrefab, transform);

        //_shotChipService = new ShotChipService();
        _gameController = new GameObject("GameController").AddComponent<GameController>();


        _inventoryController = new GameObject("InventoryController").AddComponent<InventoryController>();
        _storeController = new GameObject("StoreController").AddComponent<StoreController>();
        _rewardController = new GameObject("RewardController").AddComponent<RewardController>();
    }

    private IEnumerator _Initializing()
    {
        Debug.Log("_Initializing");
        _gameController.Init(GameModel, GameConfig, _gameStorage);
        _settingsController.Init(SettingsModel);
        _inventoryController.Init(InventoryModel);

        _fieldController.Init(FieldConfig);

        _storeController.Init(StoreModel, _inventoryController);
        _rewardController.Init(RewardConfig);

        GameModel.Init(GameConfig.GetModel());
        InventoryModel.Init(InventoryConfig);
        SettingsModel.Init(Localization, AudioSystem.Instance);

        _gameStorage.Init(GameModel, InventoryModel, SettingsModel, _fieldController);


        Debug.Log("Loading Saved Data");
        yield return StartCoroutine(_gameStorage._Load());
        GameModel.CalculateTargetScore();
        _storeController.CreateStore();
        _gameController.StartGame();

        yield break;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        StartCoroutine(_gameStorage.Save());
    //    }

    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        StartCoroutine(_gameStorage._SaveField(_fieldController.Chips));
    //    }

    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        Debug.Log("Reset ALL Data");
    //        _gameStorage.ResetData();
    //    }
    //}
}