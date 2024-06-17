namespace SRGame;

public enum ScrollMode : byte
{
    None = 0,

    /// <summary>
    ///     Unable to move.
    /// </summary>
    Resurrect = 1,

    /// <summary>
    ///     Able to move.
    /// </summary>
    ThiefDen = 2
}