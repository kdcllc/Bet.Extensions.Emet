using System.Linq;

namespace Bet.Extensions.Emet;

public static class ExpressionHelper
{
    /// <summary>
    /// Simple check for values in the list.
    /// </summary>
    /// <param name="check"></param>
    /// <param name="valList"></param>
    /// <returns></returns>
    public static bool CheckContains(string check, string valList)
    {
        if (string.IsNullOrEmpty(check) || string.IsNullOrEmpty(valList))
        {
            return false;
        }

        var list = valList.Split(',').ToList();
        return list.Contains(check);
    }
}
