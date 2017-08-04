using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace LicenseExtractor
{
    public class Options
    {
        [Option('t', "target", Default = "paket.lock", HelpText = "Target dependency file format", SetName = "directory")]
        public string Target { get; set; }

        [Option('d', "dir", Required = true, HelpText = "Directory to scan for target", SetName = "directory")]
        public string DirectoryPath { get; set; }

        [Option('f', "file", Required = true, HelpText = "File to scan", SetName = "file")]
        public string FilePath { get; set; }

        [Option('e', "export", Separator = ',', Default = new[] { "csv" }, Required = false, HelpText = "Export format for the report, default: csv with | seperator")]
        public IEnumerable<string> ExportFormat { get; set; }

        [Option('h', "fields", Separator = ',', Default = new[] { "name", "version", "license" }, Required = false, HelpText = "Fields to export")]
        public IEnumerable<string> ExportFields { get; set; }

        [Usage(ApplicationAlias = "lex")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Directory scan",
                    new UnParserSettings { PreferShortName = true },
                    new Options { DirectoryPath = @"d:\path\to\your\project", Target = "Paket", ExportFormat = new[] { "csv" } });
                yield return new Example("Directory scan with export",
                    new UnParserSettings { PreferShortName = true },
                    new Options { DirectoryPath = @"d:\path\to\your\project", Target = "Paket", ExportFormat = new[] { "csv" }, ExportFields = new[] { "name", "version", "license" } });
                yield return new Example("File scan",
                    new UnParserSettings { PreferShortName = true },
                    new Options { DirectoryPath = @"d:\path\to\your\project\paket.lock", ExportFormat = new[] { "csv", "json" } });
            }
        }
    }
}
