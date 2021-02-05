using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDKDownloader
{
   public class DownloadConfig
   {
      /// <summary>
      /// Specifies the the directory, where the content should be downloaded into; Default = current dur
      /// </summary>
      public virtual string OutputDir { get; set; } = Directory.GetCurrentDirectory();

      /// <summary>
      /// Perform a checksum check
      /// </summary>
      public virtual bool PerformCheckSumCheck { get; set; } = true;

      /// <summary>
      /// Use the tempdir for downloading
      /// </summary>
      public virtual bool UseTemp { get; set; } = false;
   }
}
