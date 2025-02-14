global using WidgetID = int;

namespace OpenTTD;

/// <summary>
/// Window numbers.
/// </summary>
public enum WindowNumberEnum : byte
{
    GameOptionsAI = 0, // AI settings
    GameOptionsGS, // GS settings
    GameOptionsAbout, // About window
    GameOptionsNewGRFState, // NewGRF settings
    GameOptions, // Game options
    GameSettings, // Game settings

    QueryString = 0, // Query string
    QueryStringSign, // Query string for signs

    ConfirmPopupQuery = 0, // Query popup confirm
    ConfirmPopupQueryBootstrap, // Query popup confirm for bootstrap

    NetworkWindowGame = 0, // Network game window
    NetworkWindowContentList, // Network content list
    NetworkWindowStart, // Network start server

    NetworkStatusWindowJoin = 0, // Network join status
    NetworkStatusWindowContentDownload, // Network content download status
}

/// <summary>
/// Window classes.
/// </summary>
public enum WindowClassEnum : ushort
{
    None, // No window, redirects to MainWindow

    /// <summary>
    /// Main window; Window numbers:<br/>
    /// <list type="bullet">
    ///     <item>
    ///         0 = <see cref="MainWidgets"/>
    ///     </item>
    /// </list>
    /// </summary>
    MainWindow = None,

    /// <summary>
    /// Main toolbar (the long bar at the top); Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="ToolbarNormalWidgets"/></see></item>
    ///     <item>0 = <see cref="ToolbarEditorWidgets"/></item>
    /// </list>
    /// </summary>
    MainToolbar,

    /// <summary>
    /// Statusbar (at the bottom of your screen); Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="StatusBarWidgets"/></item>
    /// </list>
    /// </summary>
    StatusBar,

    /// <summary>
    /// Build toolbar; Window numbers:
    /// <list type="bullet">
    ///     <item><see cref="Transport.Rail"/> = <see cref="RailToolbarWidgets"/></item>
    ///     <item><see cref="Transport.Air"/> = <see cref="AirportToolbarWidgets"/></item>
    ///     <item><see cref="Transport.Water"/> = <see cref="DockToolbarWidgets"/></item>
    ///     <item><see cref="Transport.Road"/> = <see cref="RoadToolbarWidgets"/></item>
    /// </list>
    /// </summary>
    BuildToolbar,

    /// <summary>
    /// Scenario build toolbar; Window numbers:
    /// <list type="bullet">
    ///     <item><see cref="Transport.Water"/> = <see cref="DockToolbarWidgets"/></item>
    ///     <item><see cref="Transport.Road"/> = <see cref="RoadToolbarWidgets"/></item>
    /// </list>
    /// </summary>
    ScenarioBuildToolbar,

    /// <summary>
    /// Build trees toolbar; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="BuildTreesWidgets"/></item>
    /// </list>
    /// </summary>
    BuildTrees,

    /// <summary>
    /// Transparency toolbar; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="TransparencyToolbarWidgets"/></item>
    /// </list>
    /// </summary>
    TransparencyToolbar,

    /// <summary>
    /// Build signal toolbar; Window numbers:
    /// <list type="bullet">
    ///     <item><see cref="Transport.Rail"/> = <see cref="BuildSignalWidgets"/></item>
    /// </list>
    /// </summary>
    BuildSignal,

    /// <summary>
    /// Small map; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="SmallMapWidgets"/></item>
    /// </list>
    /// </summary>
    SmallMap,

    /// <summary>
    /// Error message; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="ErrorMessageWidgets"/></item>
    /// </list>
    /// </summary>
    ErrMsg,

    /// <summary>
    /// Tooltip window; Window numbers:
    /// <list type="bullet">
    ///     <item> = <see cref="ToolTipsWidgets"/></item>
    /// </list>
    /// </summary>
    ToolTips,

    /// <summary>
    /// Query string window; Window numbers:
    /// <list type="bullet">
    ///     <item><see cref="WindowNumberEnum.QueryString"/> = <see cref="QueryStringWidgets"/></item>
    ///     <item><see cref="WindowNumberEnum.QueryStringSign"/> = <see cref="QueryEditSignWidgets"/></item>
    /// </list>
    /// </summary>
    QueryString,

    /// <summary>
    /// Popup with confirm question; Window numbers:
    /// <list type="bullet">
    ///     <item><see cref="WindowNumberEnum.ConfirmPopupQuery"/> = <see cref="QueryWidgets"/></item>
    ///     <item><see cref="WindowNumberEnum.ConfirmPopupQueryBootstrap"/> = <see cref="BootstrapAskForDownloadWidgets"/></item>
    /// </list>
    /// </summary>
    ConfirmPopupQuery,

    /// <summary>
    /// Popup with a set of buttons, designed to ask the user a question<br/>
    /// from a GameScript. Window numbers:
    /// <list type="bullet">
    ///     <item>uniqueid = <see cref="GoalQuestionWidgets"/></item>
    /// </list>
    /// </summary>
    GoalQuestion,

    /// <summary>
    /// Saveload window; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="SaveLoadWidgets"/></item>
    /// </list>
    /// </summary>
    SaveLoad,

    /// <summary>
    /// Land info window; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="LandInfoWidgets"/></item>
    /// </list>
    /// </summary>
    LandInfo,

    /// <summary>
    /// Drop down menu; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="DropdownMenuWidgets"/></item>
    /// </list>
    /// </summary>
    DropdownMenu,

    /// <summary>
    /// On Screen Keyboard; Window numbers:
    /// <list type="bullet">
    ///     <item>0 = <see cref="OnScreenKeyboardWidgets"/></item>
    /// </list>
    /// </summary>
    OSK,

    /// <summary>
    /// Set date; Window numbers:
    /// <list type="bullet">
    ///     <item><see cref="VehicleID"/> = <see cref="SetDateWidgets"/></item>
    /// </list>
    /// </summary>
    SetDate,

    ScriptSettings,

    GRFParameters,

    TextFile,

    TownAuthority,

    VehicleDetails,

    VehicleRefit,

    VehicleOrders,

    ReplaceVehicle,

    VehicleTimetable,

    CompanyColour,

    CompanyManagerFace,

    SelectStation,

    NewsWindow,

    TownDirectory,

    SubsidiesList,

    IndustryDirectory,

    MessageHistory,

    SignList,

    ScriptList,

    GoalsList,

    StoryBook,

    StationList,

    TrainsList,

    RoadVehicleList,

    ShipsList,

    AircraftList,

    TownView,

    VehicleView,

    StationView,

    VehicleDepot,

    WaypointView,

    IndustryView,

    Company,

    BuildObject,

    BuildHouse,

    BuildVehicle,

    BuildBridge,

    BuildStation,

    BusStation,

    TruckStation,

    BuildDepot,

    BuildWaypoint,

    FoundTown,

    BuildIndustry,

    SelectGame,

    ScenLandGen,

    GenerateLandscape,

    ModalProgress,

    NetworkWindow,

    ClientList,

    NetworkStatusWindow,

    NetworkAskRelay,

    NetworkAskSurvey,

    SendNetworkMsg,

    IndustryCargoes,

    GraphLegend,

    Finances,

    IncomeGraph,

    OperatingProfit,

    DeliveredCargo,

    PerformanceHistory,

    CompanyValue,

    CompanyLeague,

    PaymentRates,

    PerformanceDetail,

    IndustryProduction,

    CompanyInfrastructure,

    BuyCompany,

    EnginePreview,

    MusicWindow,

    MusicTrackSelection,

    GameOptions,

    CustomCurrency,

    Cheats,

    ExtraViewport,

    Console,

    Bootstrap,

    Highscore,

    EndScreen,

    ScriptDebug,

    NewGRFInspect,

    SpriteAligner,

    LinkgraphLegend,

    SavePreset,

    FramerateDisplay,

    FrametimeGraph,

    Screenshot,

    HelpWin,

    Invalid = 0xFFFF // Invalid window
}

/// <summary>
/// Data value for <see cref="Window.OnInvalidateData()"/> of windows with class <see cref="WindowClass.GameOptions"/>.
/// </summary>
public enum GameOptionsInvalidationData : byte
{
    Default = 0,
    NewGRFRescanned, // NewGRFs were just rescanned
    NewGRFCurrentLoaded, // The current list of active NewGRF has been loaded
    NewGRFListEdited, // List of active NewGRFs is being edited
    NewGRFChangesMade, // Changes have been made to a given NewGRF either through the palette or its parameters
    NewGRFChangesApplied, // The active NewGRF list changes have been applied
}

/// <summary>
/// 
/// </summary>
public struct WindowNumber
{
    private int value = 0;

    public WindowNumber()
    {

    }

    public WindowNumber( int value )
    {
        this.value = value;
    }

    public WindowNumber( ConvertibleThroughBase value )
    {
        this.value = value.Base();
    }

    public static implicit operator int( WindowNumber val )
    {
        return val.value;
    }
}

/// <summary>
/// State of handling an event.
/// </summary>
public enum EventState : byte
{
    Handled, // The passed event is handled
    NotHandled, // The passed event is not handled
}