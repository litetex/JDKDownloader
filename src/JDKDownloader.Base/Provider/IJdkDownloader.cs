using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Base.Provider
{
   public interface IJdkDownloader<in C> where C : IJdkProviderConfig
   {
      void UseConfig(C config);

      void UseDownloadConfig(DownloadConfig downloadConfig);

      void Download(IProgress<ProgressData> progress = null);
   }
}
