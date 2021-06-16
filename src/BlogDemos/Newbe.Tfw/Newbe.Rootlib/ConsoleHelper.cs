using System;

namespace Newbe.Rootlib
{
    public static class ConsoleHelper
    {

#if NETSTANDARD2_0
        public static string GetNET2()
        {
            return "net2.0";
        }
#endif
        
#if NET5_0_OR_GREATER
        public static string GetNET5()
        {
            return "net5.0";
        }
#endif
    }
}