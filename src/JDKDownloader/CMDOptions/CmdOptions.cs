using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.CMDOptions
{
   class CmdOptions
   {
      [Value(0)]
      public IEnumerable<string> SubArgs { get; set; }
   }
}
