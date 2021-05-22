using System.CommandLine;

namespace MLS.WasmCodeRunner
{
    public class MyCommand : Command
    {
        public MyCommand(string name, string description = null) : base(name, description)
        {
        }
    }
}