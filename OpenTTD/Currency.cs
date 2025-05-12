namespace OpenTTD;

public static partial class OpenTTD
{
    public static readonly TimerGameCalendar.Year NoEuro = new TimerGameCalendar.Year(0);
    public static readonly TimerGameCalendar.Year IsEuro = new TimerGameCalendar.Year(1);
    public static readonly TimerGameCalendar.Year MinEuroYear = new TimerGameCalendar.Year(2000);

    public static CurrencySpec[] CurrencySpecs = new CurrencySpec[(int)Currencies.End];

    public static CurrencySpec[] OriginCurrencySpecs =
    {
        new CurrencySpec(1, "", NoEuro, "\u00a3", "", "GBP", 0, "GBP"),

    };

    /// <summary>
    /// Get the custom currency.
    /// </summary>
    /// <returns>Reference to the custom currency.</returns>
    public static CurrencySpec GetCustomCurrency()
    {
        return CurrencySpecs[(int)Currencies.Custom];
    }

    /// <summary>
    /// Get the currently selected currency.
    /// </summary>
    /// <returns>Read-only reference to the current currency.</returns>
    public static CurrencySpec GetCurrency()
    {
        return CurrencySpecs[GetGameSettings().Locale.Currency];
    }
}

/// <summary>
/// This <see langword="enum"/> gives the currencies a unique ID which must be maintained for<br/>
/// savegame compatibility and in order to refer to them quickly, especially<br/>
/// for referencing the custom one.
/// </summary>
public enum Currencies : byte
{
    GBP, // British pound
    USD, // US dollar
    EUR, // Euro
    JPY, // Japanese Yen
    ATS, // Australian Schilling
    BEF, // Belgian Franc
    CHF, // Swiss Franc
    CZK, // Czech Koruna
    DEM, // Deutsche Mark
    DKK, // Danish Krona
    ESP, // Spanish Peseta
    FIM, // Finnish Mark
    FRF, // French Franc
    GRD, // Greek Drachma
    HUF, // Hungarian Forint
    ISK, // Icelandic Krona
    ITL, // Italian Lira
    NLG, // Dutch Gulden
    NOK, // Norwegian Krone
    PLN, // Polish Zloty
    RON, // Romanian Leu
    RUR, // Russian Rouble
    SIT, // Slovenian Tolar
    SEK, // Swedish Krona
    YTL, // Turkish Lira
    SKK, // Slovak Kornuna
    BRL, // Brazilian Real
    EEK, // Estonian Krooni
    LTL, // Lithuanian Litas
    KRW, // South Korean Won
    ZAR, // South African Rand
    Custom, // Custom currency
    GEL, // Georgian Lari
    IRR, // Iranian Rial
    RUB, // New Russian Ruble
    MXN, // Mexican Peso
    NTD, // New Taiwan Dollar
    CNY, // Chinese Renminbi
    HKD, // Hong Kong Dollar
    INR, // Indian Rupee
    IDR, // Indonesian Rupiah
    MYR, // Malaysian Ringgit
    LVL, // Latvian Lats
    PTE, // Portuguese Escudo
    UAH, // Ukranian Hryvnia
    End // Always the same last item
}

/// <summary>
/// Specifications for a currency.
/// </summary>
public struct CurrencySpec
{
    public ushort Rate; // The conversion rate compared to the base currency
    public string Separator; // The thousands separator for this currency
    public TimerGameCalendar.Year ToEuro; // Year of switching to the Euro, may also be OpenTTD.NOEURO or OpenTTD.ISEURO
    public string Prefix; // Prefix to apply when formatting money in this currency
    public string Suffix; // Suffix to apply when formatting money in this currency
    public string Code; // 3 letter untranslated code to identify the currency

    /// <summary>
    /// The currency symbol is represented by two possible values, prefix and suffix.<br/>
    /// Usage of one or the other is determined by <see cref="SymbolPos"/>.<br/>
    /// 0 = Prefix<br/>
    /// 1 = Suffix<br/>
    /// 2 = Both : Special occurrence for custom currency, it is not a spec from NewGRF, rather a way to let users do what they want with custom currency.
    /// </summary>
    public byte SymbolPos;

    public string Name;

    public CurrencySpec()
    {

    }

    public CurrencySpec(ushort rate, string separator, TimerGameCalendar.Year toEuro, string prefix, string suffix, string code, byte symbolPos, string name)
    {
        Rate = rate;
        Separator = separator;
        ToEuro = toEuro;
        Prefix = prefix;
        Suffix = suffix;
        Code = code;
        SymbolPos = symbolPos;
        Name = name;
    }
}