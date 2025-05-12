namespace OpenTTD.Network;

/// <summary>
/// Company information stored at the client side.
/// </summary>
public class NetworkCompanyInfo : NetworkCompanyStats
{
    public string companyName; // Company name
    public TimerGameCalendar.Year inauguratedYear; // What year the company started in
    public Money companyValue; // The company value
    public Money money; // The amount of money the company has
    public Money income; // How much did this company earn last year?
    public ushort performance; // What was their performance last month?
    public bool usePassword; // Is there a password?
    public string clients; // The clients that control this company (Name1, Name2, ...)
}

public enum NetworkRelayWindowCloseData : byte
{
    Unhandled = 0, // Relay request is unhandled
    Handled = 1, // Relay request is handled, either by user or by timeout
}

public static partial class Network
{
    public static void ShowNetworkNeedsPassword( NetworkAuthenticationPasswordRequest request )
    {

    }

    public static void ShowNetworkChatQueryWindow( DestType type, int dest )
    {

    }

    public static void ShowJoinStatusWindow()
    {

    }

    public static void ShowNetworkGameWindow()
    {

    }

    public static void ShowClientList()
    {

    }

    public static void ShowNetworkAskRelay( string serverConnectionString, string relayConnectionString, string token )
    {

    }

    public static void ShowNetworkAskSurvey()
    {

    }

    public static void ShowSurveyResultTextfileWindow()
    {

    }
}