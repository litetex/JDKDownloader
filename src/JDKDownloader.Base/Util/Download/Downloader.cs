using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.Base.Util.Download
{
   public static class Downloader
   {
      public static async Task Download(string srcURL, string targetPath, Action<DownloadProgressChangedEventArgs> onDownloadProgressChanged = null)
      {
         Log.Info($"Starting download: '{srcURL}'->'{targetPath}'");

         var sw = Stopwatch.StartNew();

         //Webclient overrides existing files
         using (var webclient = new WebClient())
         {
            webclient.DownloadProgressChanged += (s, ev) => onDownloadProgressChanged?.Invoke(ev);
            await webclient.DownloadFileTaskAsync(srcURL, targetPath);
         }

         sw.Stop();
         Log.Info($"Downloading from '{srcURL}' took {sw.ElapsedMilliseconds}ms");
      }

   }
}
