namespace OpenTTD.Timer;

/// <summary>
/// Template class for all TimerGame based timers. As Calendar and Economy are very similar, this class is used to share code between them.<br/>
/// <br/>
/// IntervalTimer and TimeoutTimer based on this Timer are a bit unusual, as their count is always one.<br/>
/// You create those timers based on a transition: a new day, a new month or a new year.<br/>
/// <br/>
/// Additionally, you need to set a priority. To ensure deterministic behaviour, events are executed<br/>
/// in priority. It is important that if you assign NONE, you do not use Random() in your callback.<br/>
/// Other than that, make sure you only set one callback per priority.<br/>
/// <br/>
/// <b>NOTE:</b> Callbacks are executed in the game-thread.
/// </summary>
public class TimerGame<T>
{

}