using System.Collections.Generic;
using System.Linq;

namespace ConfigRewriter
{
    public class Parameters
    {
        public string ConfigFilePath { get; set; }
        public List<AppSetting> AppSettings { get; set; }

        public bool Valid => !string.IsNullOrWhiteSpace(ConfigFilePath);

        public void ParseCommandLine(string[] args)
        {
            if (args.Length > 0)
                ConfigFilePath = args[0];

            AppSettings = args.Skip(1).Select(ParseAppSetting).Where(x => x != null).ToList();
        }

        private AppSetting ParseAppSetting(string arg)
        {
            var ind = arg.IndexOf('=');

            if (ind < 0)
                return null;

            return new AppSetting
            {
                Key = arg.Substring(0, ind),
                Value = arg.Substring(ind + 1)
            };
        }

        public string Dump()
        {
            return $"File: {ConfigFilePath}, AppSettings: {string.Join(", ", AppSettings.Select(x => $"[{x}]"))}";
        }

        public string Help()
        {
            return "arguments: <configFile>  <key>=<value>[ <key>=<value>]* " + "\r\n" +
                   @"example: configrewriter c:\project\app.cofnig key1=value1 key2=value2 key3=234";
        }
    }
}