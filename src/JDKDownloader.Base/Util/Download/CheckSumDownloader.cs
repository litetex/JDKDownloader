using JDKDownloader.Base.Util.Download;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JDKDownloader.Base.Util.Download
{
   public class CheckSumDownloader : BasicDownloader
   {
      private Func<HashAlgorithm> HashAlgorithmSupplier { get; set; }

      public CheckSumDownloader(Func<HashAlgorithm> hashAlgorithmSupplier)
      {
         HashAlgorithmSupplier = hashAlgorithmSupplier;
      }

      public Task DownloadAsync(
         string srcURL,
         string targetPath,
         long size,
         bool trykeepExisting,
         string checksum = null,
         int retrys = 3,
         IProgress<RetryDownloadProgress> progress = null)
      {
         if (trykeepExisting && File.Exists(targetPath))
         {
            Log.Debug($"Trying to keep existing file[='{targetPath}']");

            if (!CheckFile(targetPath, size, checksum))
            {
               Log.Info($"Existing file[='{targetPath}'] is faulty! Downloading new one...");
               return RunDownloadAsync(srcURL, targetPath, size, checksum, retrys, progress);
            }

            Log.Info($"File[='{targetPath}']'s hash and size are ok! No Download required!");
            return Task.FromResult(0);
         }

         return RunDownloadAsync(srcURL, targetPath, size, checksum, retrys, progress);

      }

      public Task RunDownloadAsync(
         string srcURL,
         string targetPath,
         long size,
         string checksum = null,
         int retrys = 3,
         IProgress<RetryDownloadProgress> progress = null)
      {
         return Task.Run(() =>
         {
            bool isok = true;
            int attemptNumber = 1;
            do
            {
               progress?.Report(new RetryDownloadProgress()
               {
                  AttemptNumber = attemptNumber,
                  Step = "Starting download",
               });

               Downloader
                  .Download(
                     srcURL,
                     targetPath,
                     onDownloadProgressChanged: ev => progress?.Report(new RetryDownloadProgress()
                     {
                        AttemptNumber = attemptNumber,
                        Step = "Downloading",
                        DownloadData = new DownloadData()
                        {
                           DownloadProgressPercentage = ev.ProgressPercentage,
                           DownloadBytesReceived = ev.BytesReceived,
                           DownloadTotalBytesToReceive = ev.TotalBytesToReceive
                        }
                     }))
                  .Wait();

               progress?.Report(new RetryDownloadProgress()
               {
                  AttemptNumber = attemptNumber,
                  Step = "Validating download"
               });
               isok = CheckFile(targetPath, size, checksum);
               retrys--;

               if (!isok)
               {
                  Log.Warn($"'{targetPath}'[URL='{srcURL}'] was faulty! Trys left: {retrys}");
                  attemptNumber++;
               }

            } while (!isok && retrys > 0);

            if (!isok)
            {
               progress?.Report(new RetryDownloadProgress()
               {
                  AttemptNumber = attemptNumber,
                  Step = "Failure"
               });
               throw new InvalidOperationException($"Download of '{targetPath}'[URL='{srcURL}'] was faulty");
            }

            progress?.Report(new RetryDownloadProgress()
            {
               AttemptNumber = attemptNumber,
               Step = "Download successful"
            });
         });
      }

      public bool CheckFile(string targetPath, long size, string checksum = null)
      {
         var exists = File.Exists(targetPath);
         Log.Debug($"'{targetPath}' EXISTS={exists}");

         if (!exists)
            return false;

         var fileInfo = new FileInfo(targetPath);
         Log.Debug($"'{targetPath}' SIZE[EXP='{size}';ACT='{fileInfo.Length}']");

         if (fileInfo.Length != size)
            return false;

         if (checksum != null)
         {
            string hash = null;
            using (var fileStream = File.OpenRead(targetPath))
            using (var cryptoProvider = HashAlgorithmSupplier.Invoke())
            {
               hash = BitConverter
                       .ToString(cryptoProvider.ComputeHash(fileStream)).Replace("-", "").ToLower();
            }

            Log.Debug($"'{targetPath}' SHA1[EXP='{checksum}';ACT='{hash}']");

            if (hash != checksum)
               return false;
         }

         return true;
      }

      public static CheckSumDownloader SHA1 => new CheckSumDownloader(() => new SHA1CryptoServiceProvider());

      public static CheckSumDownloader SHA256 => new CheckSumDownloader(() => new SHA256CryptoServiceProvider());

   }
}
