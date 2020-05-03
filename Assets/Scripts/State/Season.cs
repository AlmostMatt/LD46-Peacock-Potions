public enum Season
{
    SPRING,
    SUMMER,
    FALL,
    WINTER
}

public static class SeasonExtensions
{
    public static string GetImage(this Season season)
    {
        switch (season)
        {
            case Season.SPRING:
                return "spring";
            case Season.SUMMER:
                return "summer";
            case Season.FALL:
                return "fall";
            case Season.WINTER:
                return "winter";
            default:
                return "summer";
        }
    }

    public static string GetName(this Season season)
    {
        switch (season)
        {
            case Season.SPRING:
                return "Spring";
            case Season.SUMMER:
                return "Summer";
            case Season.FALL:
                return "Fall";
            case Season.WINTER:
                return "Winter";
            default:
                return "Smarch";
        }
    }

    public static string GetNextSeasonName(this Season season)
    {
        switch (season)
        {
            case Season.SPRING:
                return "Summer";
            case Season.SUMMER:
                return "Fall";
            case Season.FALL:
                return "Winter";
            case Season.WINTER:
                return "Spring";
            default:
                return "Smarch";
        }
    }
}