using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Base.Util.Download
{
   public class RetryDownloadProgress
   {
      public int AttemptNumber { get; set; }

      public string Step { get; set; }

      
      public DownloadData DownloadData { get; set; }
   }

   public class DownloadData
   {
      public double DownloadProgressPercentage { get; set; }

      public long DownloadBytesReceived { get; set; }

      public long DownloadTotalBytesToReceive { get; set; }
   }
}
