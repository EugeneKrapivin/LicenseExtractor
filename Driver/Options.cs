using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace LicenseExtractor
{
    partial class Program
    {
        public class Options
        {
            [Option('t', "target", Default = "paket.lock", HelpText="Target dependency file format", SetName = "directory")]
            public string Target { get; set; }
            
            [Option('d', "dir", HelpText="Directory to scan for target", SetName = "directory")]
            public string DirectoryPath { get; set; }
            
            [Option('f', "file", HelpText="File to scan", SetName = "file")]
            public string FilePath { get; set; }
            
            [Option('e', "export", Separator = ',', Default = new [] {"csv"}, Required = false, HelpText="Export format for the report, default: csv with | seperator")]
            public IEnumerable<string> ExportFormat { get; set; }
            
            [Option("fields", Separator = ',', Default = new [] { "name", "version", "license" }, Required = false, HelpText = "Fields to export")]
            public IEnumerable<string> ExportFields { get; set; }
  
            [Usage(ApplicationAlias = "lex")]
            public static IEnumerable<Example> Examples 
            {
                get 
                {
                    yield return new Example("Directory scan", new Options { DirectoryPath = @"d:\path\to\your\project", Target="Paket", ExportFormat=new [] {"csv"}});
                    yield return new Example("Directory scan with export", new Options { DirectoryPath = @"d:\path\to\your\project", Target="Paket", ExportFormat=new [] {"csv"}, ExportFields=new [] { "name", "version", "license" }});
                    yield return new Example("File scan", new Options { DirectoryPath = @"d:\path\to\your\project\paket.lock", ExportFormat=new [] {"csv", "json"} });
                }
            }
        }
    }
}
