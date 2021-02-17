using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDKDownloader
{
   public class DownloadConfig
   {
      /// <summary>
      /// Specifies the the directory, where the content should be downloaded into; Default = workingdir/download
      /// </summary>
      public virtual string OutputDir { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "java");

      /// <summary>
      /// Perform a checksum check
      /// </summary>
      public virtual bool PerformCheckSumCheck { get; set; } = true;

      /// <summary>
      /// Uses the tempdir for downloading; if not set the packed file is downloaded into the outputdir
      /// </summary>
      public virtual bool UseTempDir { get; set; }
   }
}
