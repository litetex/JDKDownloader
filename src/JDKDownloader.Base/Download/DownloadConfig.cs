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
      public string OutputDir { get; set; } = Directory.GetCurrentDirectory();

      /// <summary>
      /// Perform a checksum check
      /// </summary>
      public bool PerformCheckSumCheck { get; set; } = true;

      /// <summary>
      /// Use the tempdir for downloading
      /// </summary>
      public bool UseTemp { get; set; } = false;
   }
}
