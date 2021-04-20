using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.CMDOptions
{
   [Verb("download", isDefault: true, HelpText = "Executes a download")]
   public class DownloadOptions : BaseCmdOptions
   {
      [Value(0)]
      public IEnumerable<string> SubArgs { get; set; }

      [Option('o', "outputdirectory", HelpText = "Specifies the output directory; if not set = current working directory + \"java\"")]
      public string OutputDir { get; set; }

      [Option("noCheckSumCheck", HelpText = "Perform NO checksum check against the remote server; Be careful")]
      public bool NoCheckSumCheck { get; set; }

      /// <remarks>
      /// May downgrade performance if enabled e.g. when using multiple harddrives: %TEMPDIR% is located on Disk1, but the files should be extracted to Disk2
      /// </remarks>
      [Option("noSystemTempDir", HelpText = "uses a relative temporary directory instead of the system tempdir (e.g. %TEMPDIR%)")]
      public bool NotUseSystemTempDir { get; set; } = true;
   }
}
