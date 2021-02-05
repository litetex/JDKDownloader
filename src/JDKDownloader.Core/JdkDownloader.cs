using JDKDownloader.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Core
{
   public static class JdkDownloader
   {
      public static void Download<P,C>(C config, DownloadConfig downloadConfig = null, IProgress<ProgressData> progress = null)
         where P : IJdkProvider<C>
         where C : IJdkProviderConfig
      {
         P instance = (P)Activator.CreateInstance(typeof(P));

         var downloader = instance.JDKDownloaderSupplier();
         downloader.UseConfig(config);
         if(downloadConfig != null)
            downloader.UseDownloadConfig(downloadConfig);

         downloader.Download(progress);
      }
   }
}
