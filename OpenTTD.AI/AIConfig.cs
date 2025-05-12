using System.ComponentModel.Design;
using System.Diagnostics;

using OpenTTD.Script;

namespace OpenTTD.AI;

public class AIConfig : ScriptConfig
{

    public AIConfig() : base()
    {

    }

    public AIConfig( AIConfig other ) : base( other )
    {

    }

    /// <summary>
    /// Get the config of a company.
    /// </summary>
    public static AIConfig GetConfig( CompanyID company, ScriptSettingSource source = ScriptSettingSource.Default )
    {
        Debug.Assert( company < MAX_COMPANIES );

        if ( gameMode == GameMode.Menu )
        {
            source = ScriptSettingSource.ForceNewgame;
        }

        if ( source == ScriptSettingSource.Default )
        {
            Company c = Company.GetIfValid( company );

            if ( c != null && c.aiConfig != null )
            {
                return c.aiConfig.Get();
            }
        }

        // TODO: Change this from var to its proper variable type!
        AIConfig config = ( source == ScriptSettingSource.ForceNewgame ) ? settingsNewgame.scriptConfig.ai[company] : settingsGame.scriptConfig.ai[company];
        
        if ( config == null )
        {
            config = new AIConfig();
        }

        return config;
    }

    public AIInfo GetInfo()
    {
        return (AIInfo)ScriptConfig.GetInfo();
    }

    /// <summary>
    /// Whenever the AI Scanner is reloaded, all infos become invalid. This<br/>
    /// function tells AIConfig about this.
    /// </summary>
    /// <param name="forceExactMatch">If <see langword="true"/> try to find the exact same version as specified. If <see langword="false"/> any version is ok.</param>
    /// <returns><see langword="true"/> if the reset was successful, <see langword="false"/> if the AI was no longer found.</returns>
    public bool ResetInfo( bool forceExactMatch )
    {
        info = (ScriptInfo)AI.FindInfo( name, forceExactMatch ? version : -1, forceExactMatch );
        return info != null;
    }

    protected override ScriptInfo FindInfo( string name, int version, bool forceExactMatch )
    {
        return (ScriptInfo)AI.FindInfo( name, version, forceExactMatch );
    }
}