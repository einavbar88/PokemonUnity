using System.Collections.Generic;

public static class DataPersistance
{
    public static Dictionary<string, bool> dp = new()
    {
        { "", false},
        { "Squirtle", false },
        { "TaliaCastleKey", false },
        { "TaliaCastleGate", false },
        { "Talia", false },
        { "DemoEnd", false }
    };

    public static bool Get(string fieldName)
    {
        return dp[fieldName];
    }
}
