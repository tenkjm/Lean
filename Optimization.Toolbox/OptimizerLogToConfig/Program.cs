﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Optimization.Toolbox.OptimizerLogToConfig
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("***********************************************************************************************************************");
            Console.WriteLine("Merges the last generation alpha chromosome from supplied optimizer.log into the supplied optimization.json config file.\nUsage: OptimizerLogToJson logPath configPath");
            Console.WriteLine("***********************************************************************************************************************");

            if (args == null || !args.Any())
            {
                args = new[] {
                    Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"), @"Source\Repos\LeanOptimization\Optimization\bin\Debug\optimizer.txt"),
                    Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"), @"Source\Repos\LeanOptimization\Optimization\optimization.json") };
            }

            string path = args[0];

            var lines = File.ReadAllLines(path);
            string line = null;
            for (var i = lines.Length - 1; i > -1; i--)
            {
                line = lines.ElementAt(i);

                if (line.Contains(", Generation: "))
                {
                    break;
                }
            }

            var split = line.Split(',');

            var reformatted = split.Skip(4).Where(i => !i.StartsWith(" Id"));

            File.Copy(args[1], args[1] + ".bak", true);

            var config = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(args[1]));

            foreach (var item in reformatted)
            {
                var pair = item.Split(":").Select(s => s.Trim());
                var key = config.SelectToken($"$.genes[?(@.key == '{pair.First()}')]");
                if (key == null)
                {
                    throw new Exception("Cound not find key: " + pair.First());
                }

                ((JObject)key).Remove("actual");

                JProperty property = null;

                if (int.TryParse(pair.Last(), out var parsed))
                {
                    property = new JProperty("actual", int.Parse(pair.Last()));
                }
                else
                {
                    property = new JProperty("actual", decimal.Parse(pair.Last()));
                }

                ((JObject)key).Add(property);

            }

            using (JsonTextWriter writer = new JsonTextWriter(File.CreateText(args[1])))
            {
                writer.Formatting = Formatting.Indented;
                config.WriteTo(writer);
            }

            Console.ReadKey();
        }

        private static string Replace(string entry, string separator)
        {
            entry = entry.Trim().Replace(":", $"\"{separator}");
            entry = "{\"" + entry + "},\n";

            return entry;
        }


    }
}
