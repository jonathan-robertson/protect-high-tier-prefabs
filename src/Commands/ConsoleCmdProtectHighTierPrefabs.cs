//using System.Collections.Generic;
//using System.Linq;

//namespace ProtectHighTierPrefabs.Commands
//{
//    internal class ConsoleCmdProtectHighTierPrefabs : ConsoleCmdAbstract
//    {

//        private static readonly string[] commands = new string[] {
//            "ProtectHighTierPrefabs",
//            "phtp"
//        };
//        private readonly string help;

//        public ConsoleCmdProtectHighTierPrefabs()
//        {
//            var dict = new Dictionary<string, string>() {
//                { "debug", "enable/disable debugging" },
//            };

//            var i = 1; var j = 1;
//            help = $"Usage:\n  {string.Join("\n  ", dict.Keys.Select(command => $"{i++}. {GetCommands()[0]} {command}").ToList())}\nDescription Overview\n{string.Join("\n", dict.Values.Select(description => $"{j++}. {description}").ToList())}";
//        }

//        protected override string[] getCommands()
//        {
//            return commands;
//        }

//        protected override string getDescription()
//        {
//            return "Manage the ProtectHighTierPrefabs mod.";
//        }

//        public override string GetHelp()
//        {
//            return help;
//        }

//        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
//        {
//            switch (_params.Count)
//            {
//                case 1:
//                    switch (_params[0].ToLower())
//                    {
//                        case "debug":
//                            ModApi.DebugMode = !ModApi.DebugMode;
//                            SdtdConsole.Instance.Output($"debug mode {(ModApi.DebugMode ? "enabled" : "disabled")}");
//                            return;
//                    }
//                    break;
//            }
//            SdtdConsole.Instance.Output("Invalid command");
//        }
//    }
//}
