using Unicorn.VS.Models;

namespace Unicorn.VS.Helpers
{
    public static class UnicornConnectionHelper
    {
        public static HttpHelper Get(this UnicornConnection connection, string command)
        {
            return new HttpHelper(connection, command);
        }
    }
}