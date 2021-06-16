using System;
using Newbe.Midlib;
using Newbe.Rootlib;
using NUnit.Framework;

namespace Newbe.MyConsole
{
    public class ConsoleHelperTest
    {
        [Test]
        public void GetTFWTest()
        {
            Console.WriteLine(ConsoleHelper.GetNET5());
        }
        
        [Test]
        public void MidConsoleHelperTest()
        {
            Console.WriteLine(MidConsoleHelper.GetTFW());
        }
    }
}