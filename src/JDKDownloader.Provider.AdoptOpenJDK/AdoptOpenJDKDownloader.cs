using CoreFramework.Base.IO;
using JDKDownloader.Provider;
using JDKDownloader.Base.Util;
using JDKDownloader.Base.Util.Download;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace JDKDownloader.Provider.AdoptOpenJDK
{
   /// <summary>
   /// Downloader for <see cref="https://adoptopenjdk.net/"/>
   /// </summary>
   public class AdoptOpenJDKDownloader : IJdkDownloader<AdoptOpenJDKConfig>
   {
      protected AdoptOpenJDKConfig Config { get; set; } = new AdoptOpenJDKConfig();
      protected DownloadConfig DownloadConfig { get; set; } = new DownloadConfig();

      public AdoptOpenJDKDownloader(AdoptOpenJDKConfig config = null, DownloadConfig downloadConfig = null)
      {
         if (config != null)
            UseConfig(config);
         if (downloadConfig != null)
            UseDownloadConfig(downloadConfig);
      }

      public void UseConfig(AdoptOpenJDKConfig config)
      {
         Config = config ?? throw new ArgumentNullException(nameof(config));
      }

      public void UseDownloadConfig(DownloadConfig downloadConfig)
      {
         DownloadConfig = downloadConfig ?? throw new ArgumentNullException(nameof(downloadConfig));
      }

      public void Download(IProgress<ProgressData> progress = null)
      {
         progress?.Report(new ProgressData()
         {
            Percent = 0,
            Phase = "Initalizing"
         });

         if (string.IsNullOrWhiteSpace(Config.OS))
            Config.OS = GetOS();

         if (string.IsNullOrWhiteSpace(Config.Architecture))
            Config.Architecture = GetArch();

         using var wc = new WebClient();

         progress?.Report(new ProgressData()
         {
            Percent = 0,
            Phase = "Fetching metadata"
         });

         var downloadMetaUri = GetDownloadURI();

         Log.Info($"Downloading metadata from '{downloadMetaUri}'");

         var metaJSON = wc.DownloadString(downloadMetaUri);

         progress?.Report(new ProgressData()
         {
            Percent = 0,
            Phase = "Processing metadata"
         });

         Log.Info($"Download successful");
         Log.Debug($"Got this: {metaJSON}");

         var results = JArray.Parse(metaJSON);
         if (!results.Any())
            throw new InvalidOperationException("No results");
         else if (results.Count() > 1)
            Log.Warn("Found multiple results in meta-json; taking first");

         var binaries = results.First()["binaries"].Children();
         if (!binaries.Any())
            throw new InvalidOperationException("No binaries found");
         else if (binaries.Count() > 1)
            Log.Warn("Found multiple binaries in meta-json; taking first");

         var downloadPack = binaries.First()["package"];

         var link = downloadPack["link"].ToObject<string>();
         var name = downloadPack["name"].ToObject<string>();
         var size = downloadPack["size"].ToObject<int>();
         var sha256checksum = downloadPack["checksum"].ToObject<string>();

         progress?.Report(new ProgressData()
         {
            Percent = 0,
            Phase = "Preparing download"
         });

         var tempDir = IOUtil.GenerateTempDir(DownloadConfig.TempDir);
         Log.Info($"Tempdir is '{tempDir}'");

         var downloadLocation = Path.Combine(tempDir, name);

         progress?.Report(new ProgressData()
         {
            Percent = 0,
            Phase = "Downloading JDK"
         });


         RetryDownloadProgress latestProgress = null;        

         var dwlProgress = new ProgressActionRelay<RetryDownloadProgress>(p => latestProgress = p);

         var lastReportedTime = DateTimeOffset.UtcNow;
         RetryDownloadProgress lastProgress = null;
         var lastBytesPerSec = new FixedSizedQueue<long>(50);

         var timer = new Timer()
         {
            AutoReset = true,
            Interval = 100,
         };
         timer.Elapsed += (s, ev) =>
         {
            try
            {
               var now = DateTimeOffset.UtcNow;

               var currentProgress = latestProgress;
               if (currentProgress != null)
               {
                  if (currentProgress?.DownloadData != null)
                  {
                     var secDiff = (now - lastReportedTime).TotalSeconds;
                     var receivedBytesDelta = Math.Max(currentProgress.DownloadData.DownloadBytesReceived - lastProgress?.DownloadData?.DownloadBytesReceived ?? 0, 0);
                     var bytesPerSecond = Math.Max((long)Math.Round(receivedBytesDelta / secDiff), 0);
                     lastBytesPerSec.Enqueue(bytesPerSecond);
                  }
                  else
                  {
                     lastBytesPerSec.Clear();
                  }

                  progress?.Report(new ProgressData()
                  {
                     Percent = currentProgress.DownloadData?.DownloadProgressPercentage / 100 ?? 0,
                     Phase = $"Downloading JDK; Try #{currentProgress.AttemptNumber} - {latestProgress.Step}",
                     DownloadSpeedBytePerSecond = lastBytesPerSec.IsEmpty ? 0 : (long)Math.Round(lastBytesPerSec.Average()),
                  });

                  lastProgress = currentProgress;
               }

               lastReportedTime = now;
            }
            catch (Exception ex)
            {
               Log.Error(ex);
            }
         };

         if(progress != null)
            timer.Start();

         
         var ctsCancel = new CancellationTokenSource();
         var tcsCancel = new TaskCompletionSource<bool>();
         EventHandler cleanUpHandler = (s, ev) =>
         {
            try
            {
               timer.Stop();
               dwlProgress.Unbind();
               Trace.WriteLine("UNBIND DONE");
               progress?.Report(new ProgressData()
               {
                  Percent = 0,
                  Phase = "Aborting"
               });
               Trace.WriteLine("REPORT ABORT DONE");

               // Release lock on file
               ctsCancel.Cancel();
               Trace.WriteLine("CTS SENT");

               tcsCancel.Task.Wait();
               Trace.WriteLine("TASK DONE");
            }
            catch (Exception e)
            {
               Trace.Fail(e.ToString());
               Console.Error.WriteLine(e);
            }
         };
         AppDomain.CurrentDomain.ProcessExit += cleanUpHandler;

         bool abort = false;
         try
         {
            CheckSumDownloader.SHA256.RunDownloadAsync(
               link,
               downloadLocation,
               size,
               DownloadConfig.PerformCheckSumCheck ? sha256checksum : null,
               progress: dwlProgress,
               cancellationTokenSource: ctsCancel
               ).Wait();
         }
         catch (OperationCanceledException ex)
         {
            abort = true;
            Trace.WriteLine("CANCEL EXEC");
            Log.Info("Download was aborted", ex);
            try
            {
               Trace.WriteLine("DELETING TEMPDIR");
               DirUtil.DeleteSafe(tempDir);
               Trace.WriteLine("DELETED TEMPDIR");
            }
            catch(Exception e2)
            {
               Trace.WriteLine(e2.ToString());
            }
         }
         catch (Exception ex)
         {
            Trace.WriteLine(ex.ToString());
         }
         finally
         {
            AppDomain.CurrentDomain.ProcessExit -= cleanUpHandler;

            tcsCancel.TrySetResult(true);

            if (progress != null)
               timer.Stop();

            timer.Dispose();
         }

         if (abort)
            return;

         progress?.Report(new ProgressData()
         {
            Percent = 1,
            Phase = "Extracting"
         });

         ExtractToOutputDir(tempDir, downloadLocation);

         DirUtil.DeleteSafe(tempDir);

         progress?.Report(new ProgressData()
         {
            Percent = 1,
            Phase = "Done"
         });
      }


      protected string GetOS()
      {
         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "windows";
         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "linux";
         else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "mac";
         else
            throw new InvalidOperationException("Could not find runtime os");
      }

      protected string GetArch()
      {
         return RuntimeInformation.OSArchitecture switch
         {
            Architecture.X64 => "x64",
            Architecture.X86 => "x32",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "aarch64",
            _ => throw new InvalidOperationException("Could not find runtime arch"),
         };
      }

      protected Uri GetDownloadURI()
      {
         var uri = new Uri(
         Config.RemoteBaseURL
            .Replace("{feature_version}", Config.FeatureVersion.ToString())
            .Replace("{release_type}", Config.ReleaseType)
         );

         if (!string.IsNullOrWhiteSpace(Config.Architecture))
            uri = uri.AddQuery("architecture", Config.Architecture);

         if (!string.IsNullOrWhiteSpace(Config.Project))
            uri = uri.AddQuery("heap_size", Config.HeapSize);

         if (!string.IsNullOrWhiteSpace(Config.ImageType))
            uri = uri.AddQuery("image_type", Config.ImageType);

         if (!string.IsNullOrWhiteSpace(Config.JVMImpl))
            uri = uri.AddQuery("jvm_impl", Config.JVMImpl);

         if (!string.IsNullOrWhiteSpace(Config.OS))
            uri = uri.AddQuery("os", Config.OS);

         if (Config.PageSize >= 1)
            uri = uri.AddQuery("page_size", Config.PageSize.ToString());

         if (!string.IsNullOrWhiteSpace(Config.Project))
            uri = uri.AddQuery("project", Config.Project);

         if (!string.IsNullOrWhiteSpace(Config.SortOrder))
            uri = uri.AddQuery("sort_order", Config.SortOrder);

         if (!string.IsNullOrWhiteSpace(Config.Vendor))
            uri = uri.AddQuery("vendor", Config.Vendor);

         return uri;
      }

      private void ExtractToOutputDir(string generalTempDir, string packedFile)
      {
         var tempDir = Path.Combine(generalTempDir, $"temp-{Path.GetFileName(packedFile)}");
         
         DirUtil.EnsureCreatedAndClean(tempDir);

         Log.Info($"Extracting '{packedFile}'->'{tempDir}'");
         if (packedFile.EndsWith(".zip"))
         {
            Log.Info("ExtractionMethod: ZIP");
            UnpackUtil.ExtractZIP(packedFile, tempDir);
         }
         else if (packedFile.EndsWith(".tar.gz"))
         {
            Log.Info("ExtractionMethod: TAR/GZ");
            UnpackUtil.ExtractTGZ(packedFile, tempDir);
         }
         else
            throw new InvalidOperationException("Unknown compression type");

         List<Task> deleteTasks = new List<Task>
         {
            Task.Run(() =>
               {
                  Log.Info($"Deleting input/tempfile '{packedFile}'");
                  File.Delete(packedFile);
                  Log.Info($"Deleted '{packedFile}'");
               })
         };

         DirUtil.EnsureCreatedAndClean(DownloadConfig.OutputDir);
         var targetInfo = new DirectoryInfo(DownloadConfig.OutputDir);
         foreach (var dir in new DirectoryInfo(tempDir).GetDirectories())
         {
            Log.Info($"Moving internal (layer-1) directory '{dir}' into '{targetInfo}'");
            DirUtil.Copy(dir, targetInfo);

            deleteTasks.Add(
                  Task.Run(() =>
                  {
                     Log.Info($"Deleting internal folder '{dir.FullName}'");
                     Directory.Delete(dir.FullName, true);
                     Log.Info($"Deleted '{dir.FullName}'");
                  })
               );
         }

         Log.Info("Waiting for DeleteTasks");
         Task.WaitAll(deleteTasks.ToArray());
         Log.Info("All DeleteTasks finished");

         Log.Info($"Deleting tempfolder '{tempDir}'");
         Directory.Delete(tempDir, true);
         Log.Info($"Deleted '{tempDir}'");

         Log.Info("Extracting finished");
      }
   }
}
