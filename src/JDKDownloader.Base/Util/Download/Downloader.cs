using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JDKDownloader.Base.Util.Download
{
   public static class Downloader
   {
      public static void Download(
         string srcURL,
         string targetPath, 
         Action<DownloadProgressChangedEventArgs> onDownloadProgressChanged = null, 
         CancellationToken cancellationToken = default)
      {
         Log.Info($"Starting download: '{srcURL}'->'{targetPath}'");

         var sw = Stopwatch.StartNew();

         //Webclient overrides existing files
         using (var webclient = new WebClient())
         {
            bool downloadCancelled = false;

            webclient.DownloadProgressChanged += (s, ev) => onDownloadProgressChanged?.Invoke(ev);
            webclient.DownloadFileCompleted += (s, ev) =>
            {
               downloadCancelled = ev.Cancelled;
               if (ev.Cancelled || ev.Error != null)
               {
                  try
                  {
                     Trace.WriteLine("DELETING DOWNLOADED DATA");
                     File.Delete(targetPath);
                  }
                  catch (Exception ex)
                  {
                     Trace.Fail(ex.ToString());
                     Log.Warn("Unable to cleanup", ex);
                  }
               }
            };

            cancellationToken.Register(() =>
            {
               try
               {
                  webclient.CancelAsync();
               }
               catch (Exception ex)
               {
                  Log.Warn("Failed to cancel webclient", ex);
               }
            });

            try
            {
               webclient.DownloadFileTaskAsync(srcURL, targetPath).Wait();
            }
            catch(AggregateException ex)
            {
               // Do nothing
            }

            if (downloadCancelled)
               throw new OperationCanceledException();
         }

         sw.Stop();
         Log.Info($"Downloading from '{srcURL}' took {sw.ElapsedMilliseconds}ms");
      }

   }
}
