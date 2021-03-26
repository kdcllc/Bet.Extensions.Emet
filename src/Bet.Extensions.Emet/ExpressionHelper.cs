using System.Linq;

namespace Bet.Extensions.Emet
{
    public static class ExpressionHelper
    {
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
}
