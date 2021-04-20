using CommandLine;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.CMDOptions
{
   public class BaseCmdOptions
   {
      [Option("non-interactive", HelpText = "Will not show dynamic stuff, like download progress on the console")]
      public bool NonInteractive { get; set; }

      [Option("errors", HelpText = "Only show errors")]
      public bool Errors { get; set; }

      [Option("warnings", HelpText = "Only show warnings")]
      public bool Warnings { get; set; }

      [Option('v', "verbose", HelpText = "More logs (for debugging)")]
      public bool Verbose { get; set; }

      // Cast fails with InvalidCastExeception when using LogLeventLevel directly...
      [Option("loglevel", HelpText = "Log level (ignores all other log level options); Available: fatal, error, warning, information, debug or verbose")]
      public string LogLevel { get; set; } = null;
   }
}
