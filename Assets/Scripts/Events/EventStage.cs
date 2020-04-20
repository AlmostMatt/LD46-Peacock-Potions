// An enum to refer to stages in an event.
public enum EventStage
{
    // These names are shared across all events.
    // Feel free to add (or reuse) anything

    START,
    S1,
    S2,
    S3,
    S4,
    S5,
    S6,
    S7,

    HAS_POTION,
    NO_POTION,

    DECIDE,
    ACCEPT,
    REFUSE,
    UNABLE,

    LET_GO,
    MAKE_EXAMPLE,
    BACK_OFF,
    STICK_TO,

    GO_CLEAN,
    GO_OUTSIDE,
}