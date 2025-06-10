/// <summary>
/// Codis d'event utilitzats per als esdeveniments de xarxa.
/// </summary>
public static class EventCodes
{
    /// <summary>
    /// El codi d'event per quan el jugador agafa el seu objecte.
    /// </summary>
    public const byte GrabbedMyObject = 1;

    /// <summary>
    /// El codi d'event per quan el jugador crea una escena.
    /// </summary>
    public const byte CreateScene = 2;

    /// <summary>
    /// El codi d'event per quan el jugador finalitza una escena.
    /// </summary>
    public const byte FinishScene = 3;
}
