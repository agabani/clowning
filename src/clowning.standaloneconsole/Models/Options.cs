using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace clowning.standaloneconsole.Models
{
    internal class Options
    {
        [OptionList('i', "Device", Required = false, HelpText = "Device id.", DefaultValue = null)]
        public IList<string> Device { get; set; }

        [Option('r', "RGB", Required = false, HelpText = "RGB Color", DefaultValue = null)]
        public string Rgb { get; set; }

        [Option('c', "Color", Required = false, HelpText = "RGB Color", DefaultValue = null)]
        public string Color { get; set; }

        [Option('d', "Dim", Required = false, HelpText = "Dim Light", DefaultValue = null)]
        public bool? Dim { get; set; }

        [Option('f', "Flash", Required = false, HelpText = "Flash Light", DefaultValue = null)]
        public bool? Flash { get; set; }

        [Option('s', "FlashSpeed", Required = false, HelpText = "Flash FlashSpeed", DefaultValue = null)]
        public int? FlashSpeed { get; set; }

        [Option('v', "Verbose", Required = false, HelpText = "Verbose", DefaultValue = false)]
        public bool Verbose { get; set; }

        [Option('z', "Reset", Required = false, HelpText = "Reset Light", DefaultValue = false)]
        public bool Reset { get; set; }

        [Option('q', "Quit", Required = false, HelpText = "Quit", DefaultValue = false)]
        public bool Quit { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Usage: *.exe [-i id] [-c color] [-d enable] [-f enable] [-s speed]");
            usage.AppendLine("             [-z] [-v] [-q]");
            usage.AppendLine("");
            usage.AppendLine("Options:");
            usage.AppendLine("    -i id     Device id to target.");
            usage.AppendLine("    -c value  Change device color to one of following values:");
            usage.AppendLine("              Red,Green,Blue,Cyan,Magenta,Yellow,White,Orange");
            usage.AppendLine("    -d enable Dim light.");
            usage.AppendLine("    -f enable Flash light.");
            usage.AppendLine("    -s speed  Set flash speed to one of the following values:");
            usage.AppendLine("              1:Low,2:Medium,3:High");
            usage.AppendLine("    -z        Resets light.");
            usage.AppendLine("    -v        Verbose.");
            usage.AppendLine("    -q        Quit.");
            return usage.ToString();
        }
    }
}

/*
 * -i --Device n
 * -r --RGB r g b
 * -c --Color Red,Green,Blue,Cyan,Magenta,Yellow,White,Orange
 * -d --Dim true,false
 * -f --Flash true,false
 * -s --FlashSpeed low/1,medium/2,high/3
 * -z --Reset
 */