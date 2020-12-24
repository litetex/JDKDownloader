﻿using JDKDownloader.Base.Download;
using JDKDownloader.Base.Provider;
using JDKDownloader.Base.Util;
using JDKDownloader.Base.Util.Download;
using JDKDownloader.Provider.AdoptOpenJDK.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.Provider.AdoptOpenJDK
{
   public class AdoptOpenJDKDownloader : IJdkDownloader<AdoptOpenJDKConfig>
   {
      private AdoptOpenJDKConfig Config { get; set; }
      private DownloadConfig DownloadConfig { get; set; }

      public void UseConfig(AdoptOpenJDKConfig config)
      {
         Config = config;
      }

      public void UseDownloadConfig(DownloadConfig downloadConfig)
      {
         DownloadConfig = downloadConfig;
      }

      public void Download()
      {
         if (string.IsNullOrWhiteSpace(Config.OS))
            Config.OS = GetOS();

         if (string.IsNullOrWhiteSpace(Config.Architecture))
            Config.Architecture = GetArch();

         using var wc = new WebClient();

         var downloadMetaUri = GetDownloadURI();

         Log.Info($"Downloading metadata from '{downloadMetaUri}'");

         var metaJSON = wc.DownloadString(downloadMetaUri);

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

         var downloadLocation = DownloadConfig.UseTemp ? Path.Combine(Path.GetTempPath(), name) : name;

         CheckSumDownloader.SHA256.RunDownloadAsync(link, downloadLocation, size, DownloadConfig.PerformCheckSumCheck ? sha256checksum : null).Wait();

         ExtractToOutputDir(downloadLocation);
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
         switch (RuntimeInformation.OSArchitecture)
         {
            case Architecture.X64:
               return "x64";
            case Architecture.X86:
               return "x32";
            case Architecture.Arm:
               return "arm";
            case Architecture.Arm64:
               return "aarch64";
            default:
               throw new InvalidOperationException("Could not find runtime arch");
         }
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

      private void ExtractToOutputDir(string packedFile)
      {
         var tempDir = "temp-" + DownloadConfig.OutputDir;

         if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

         Directory.CreateDirectory(tempDir);

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

         var targetInfo = new DirectoryInfo(DownloadConfig.OutputDir);
         foreach (var dir in new DirectoryInfo(tempDir).GetDirectories())
         {
            Log.Info($"Moving internal (layer-1) directory '{dir}' into '{targetInfo}'");
            IOUtil.CopyFilesRecursively(dir, targetInfo);

            deleteTasks.Add(
                  Task.Run(() =>
                  {
                     Log.Info($"Deleting internal folder '{dir.FullName}'");
                     Directory.Delete(dir.FullName, true);
                     Log.Info($"Deleted '{dir.FullName}'");
                  })
               );
         }

         Log.Info($"Deleting tempfolder '{tempDir}'");
         Directory.Delete(tempDir, true);
         Log.Info($"Deleted '{tempDir}'");

         Log.Info("Waiting for DeleteTasks");
         Task.WaitAll(deleteTasks.ToArray());
         Log.Info("All DeleteTasks finished");

         Log.Info("Extracting finished");
      }
   }
}