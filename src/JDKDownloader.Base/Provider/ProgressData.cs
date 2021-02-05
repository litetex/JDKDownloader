using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Provider
{
   public class ProgressData
   {
      // Min=0; Max=1
      public double Percent { get; set; }

      public long? DownloadSpeedBytePerSecond { get; set; }

      public string Phase { get; set; }

      public override string ToString()
      {
         if (DownloadSpeedBytePerSecond == null)
            return Phase;

         return $"{Phase} {SizeSuffix(DownloadSpeedBytePerSecond.Value,2)}";
      }

      static readonly string[] Speed =
                  { "B/s", "KB/s", "MB/s", "GB/s" };

      static string SizeSuffix(long value, int decimalPlaces = 1)
      {
         if (value < 0) { return "-" + SizeSuffix(-value); }

         int i = 0;
         decimal dValue = (decimal)value;
         while (Math.Round(dValue, decimalPlaces) >= 1000)
         {
            dValue /= 1000;
            i++;
         }

         return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, Speed[i]);
      }

   }
}
