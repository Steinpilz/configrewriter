using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigRewriter
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var parameters = new Parameters();

                parameters.ParseCommandLine(args);
                Console.WriteLine($"arguments are: [{parameters.Dump()}]");

                if (parameters.Valid)
                {
                    if (!File.Exists(parameters.ConfigFilePath))
                    {
                        Console.WriteLine($"File {parameters.ConfigFilePath} doesn't exist");
                        return 1;
                    }

                    if (!parameters.AppSettings.Any())
                        return 0;

                    var file = File.ReadAllText(parameters.ConfigFilePath);

                    var configRewriter = new ConfigFileRewriter(file);
                    configRewriter.RewriteAppSettings(parameters.AppSettings);

                    var rewrittenFile = configRewriter.Result();

                    File.WriteAllText(parameters.ConfigFilePath, rewrittenFile);

                    Console.WriteLine("Done");
                }
                else
                {
                    Console.WriteLine(parameters.Help());
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
