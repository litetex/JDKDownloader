using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.CMDOptions.Download
{
   [Verb("adoptopenjdk", isDefault: true, HelpText = "Downloads from AdoptOpenJDK")]
   public class AdoptOpenJDKOptions // TODO: extend from config
   {
   }
}
