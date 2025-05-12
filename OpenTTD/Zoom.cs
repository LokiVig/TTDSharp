namespace OpenTTD;

/// <summary>
/// All zoom levels we know of.
/// </summary>
public enum ZoomLevel : byte
{
    Begin = 0, // Begin for iteration
    In4x = 0, // Zoomed 4 times in
    In2x, // Zoomed 2 times in
    Normal, // The normal zoom level
    Out2x, // Zoomed 2 times out
    Out4x, // Zoomed 4 times out
    Out8x, // Zoomed 8 times out
    End, // End for iteration

    Viewport = Normal, // Default zoom level for viewports
    News = Normal, // Default zoom level for the news messages
    Industry = Out2x, // Default zoom level for the industry view
    Town = Normal, // Default zoom level for the town view
    Aircraft = Normal, // Default zoom level for the aircraft view
    Ship = Normal, // Default zoom level for the ship view
    Train = Normal, // Default zoom level for the train view
    RoadVehicle = Normal, // Default zoom level for the road vehicle view
    WorldScreenshot = Normal, // Default zoom level for the world screenshot

    Detail = Out2x, // All zoom levels below or equal to this will result in details on the screen, like roadwork, etc.
    TextEffect = Out2x, // All zoom levels above this will not show text effects

    Min = In4x, // Minimum zoom level
    Max = Out8x, // Maximum zoom level
}