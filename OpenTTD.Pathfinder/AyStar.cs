namespace OpenTTD.Pathfinder;

public static partial class Pathfinder
{
    public const int AYSTAR_DEF_MAX_SEARCH_NODES = 10000; // Reference limit for AyStar.maxSearchNodes

    public const int AYSTAR_INVALID_NODE = -1; // Item is not valid (for example, not walkable)
}

/// <summary>
/// Return status of <see cref="AyStar"/> methods.
/// </summary>
public enum AyStarStatus : byte
{
    FoundEndNode, // An end node was found
    EmptyOpenList, // All items are tested, and no path has been found
    StillBusy, // Some checking was done, but no path found yet, and there are still items left to try
    NoPath, // No path to the goal was found
    LimitReached, // The AyStar.maxSearchNodes limit has been reached, aborting search
    Done, // Not an end tile, or wrong direction
}

/// <summary>
/// AyStar search algorithm <see langword="struct"/>.<br/>
/// Before calling <see cref="Init"/>, fill <see cref="CalculateG"/>, <see cref="CalculateH"/>, <see cref="GetNeighbours"/>, <see cref="EndNodeCheck"/>, and <see cref="FoundEndNode"/>.<br/>
/// If you want to change them after calling <see cref="Init"/>, first call <see cref="Free"/>!<br/>
/// <br/>
/// The <see cref="userPath"/>, <see cref="userTarget"/>, and <see cref="userData"/> are intended to be used by the user routines. The data not accessed by the <see cref="AyStar"/> code itself.<br/>
/// The user routines can change any moment they like.
/// </summary>
public struct AyStar
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aystar"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public delegate AyStarStatus EndNodeCheck( AyStar aystar, PathNode current );
}