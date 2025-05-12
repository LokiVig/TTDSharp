using System.Collections.Generic;

using OpenTTD.Network.Core;

namespace OpenTTD.AI;

/// <summary>
/// Main AI class. Contains all functions needed to start, stop, save and load AIs.
/// </summary>
public class AI
{
    private static uint frameCounter; // Tick counter for the AI code
    private static AIScannerInfo scannerInfo; // ScriptScanner instance that is used to find AIs
    private static AIScannerLibrary scannerLibrary; // ScriptScanner instance that is used to find AI Libraries

    /// <summary>
    /// Is it possible to start a new AI company?
    /// </summary>
    /// <returns><see langword="true"/> if a new AI company can be started.</returns>
    public static bool CanStartNew()
    {

    }

    /// <summary>
    /// Start a new AI company.
    /// </summary>
    /// <param name="company">At which slot the AI company should start.</param>
    public static void StartNew( CompanyID company )
    {

    }

    /// <summary>
    /// Called every game-tick to let AIs do something.
    /// </summary>
    public static void GameLoop()
    {

    }

    /// <summary>
    /// Get the current AI tick.
    /// </summary>
    public static uint GetTick()
    {
        
    }

    /// <summary>
    /// Stop a company to be controlled by an AI.
    /// </summary>
    /// <param name="company">The company from which the AI needs to detach.</param>
    public static void Stop( CompanyID company )
    {

    }

    /// <summary>
    /// Suspend the AI and then pause execution of the script. The script<br/>
    /// will not be resumed from its suspended state until the script has<br/>
    /// been unpaused.
    /// </summary>
    /// <param name="company">The company for which the AI should be paused.</param>
    public static void Pause( CompanyID company )
    {

    }

    /// <summary>
    /// Resume execution of the AI. This function will not actually execute<br/>
    /// the script, but set a flag so that the script is executed by the usual<br/>
    /// mechanism that executes the script.
    /// </summary>
    /// <param name="company">The company for which the AI should be unpaused.</param>
    public static void Unpause( CompanyID company )
    {

    }

    /// <summary>
    /// Checks if the AI is paused.
    /// </summary>
    /// <param name="company">The company for which to check if the AI is paused.</param>
    /// <returns><see langword="true"/> if the AI is paused, otherwise <see langword="false"/>.</returns>
    public static bool IsPaused( CompanyID company )
    {

    }

    /// <summary>
    /// Kill any and all AIs we manage.
    /// </summary>
    public static void KillAll()
    {

    }

    /// <summary>
    /// Initialize the AI system.
    /// </summary>
    public static void Initialize()
    {

    }

    /// <summary>
    /// Uninitialize the AI system.
    /// </summary>
    /// <param name="keepConfig">Should we keep AIConfigs, or can we free that memory?</param>
    public static void Uninitialize( bool keepConfig )
    {

    }

    /// <summary>
    /// Reset all AIConfigs, and make them reload their AIInfo.<br/>
    /// If the AIInfo could no longer be found, an error is reported to the user.
    /// </summary>
    public static void ResetConfig()
    {

    }

    /// <summary>
    /// Queue a new event for the AI.
    /// </summary>
    public static void NewEvent( CompanyID company, ScriptEvent scriptEvent )
    {

    }

    /// <summary>
    /// Broadcast a new event to all active AIs.
    /// </summary>
    public static void BroadcastNewEvent( ScriptEvent scriptEvent, CompanyID skipCompany = CompanyID.Invalid() )
    {

    }

    /// <summary>
    /// Save data from an AI to a savegame.
    /// </summary>
    public static void Save( CompanyID company )
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.GetAIConsoleList()"/>.
    /// </summary>
    public static void GetConsoleList( List<string> outputIterator, bool newestOnly )
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.GetAIConsoleLibraryList()"/>.
    /// </summary>
    public static void GetConsoleLibraryList( List<string> outputIterator )
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.GetAIInfoList()"/>.
    /// </summary>
    public static ScriptInfoList GetInfoList()
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.GetUniqueAIInfoList()"/>.
    /// </summary>
    public static ScriptInfoList GetUniqueInfoList()
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.FindInfo()"/>.
    /// </summary>
    public static AIInfo FindInfo( string name, int version, bool forceExactMatch )
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.FindLibrary()"/>.
    /// </summary>
    public static AILibrary FindLibrary( string library, int version )
    {

    }

    /// <summary>
    /// Rescans all searchpaths for available AIs. If a used AI is no longer<br/>
    /// found it is removed from the config.
    /// </summary>
    public static void Rescan()
    {

    }

    /// <summary>
    /// Gets the ScriptScanner instance that is used to find AIs.
    /// </summary>
    public static AIScannerInfo GetScannerInfo()
    {

    }

    /// <summary>
    /// Gets the ScriptScanner instance that is used to find AI Libraries.
    /// </summary>
    public static AIScannerLibrary GetScannerLibrary()
    {

    }

    /// <summary>
    /// Wrapper function for <see cref="AIScanner.HasAI()"/>.
    /// </summary>
    public static bool HasAI( ContentInfo ci, bool md5sum )
    {

    }

    public static bool HasAILibrary( ContentInfo ci, bool md5sum )
    {

    }
}