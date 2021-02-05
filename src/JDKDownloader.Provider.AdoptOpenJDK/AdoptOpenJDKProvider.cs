using JDKDownloader.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Provider.AdoptOpenJDK
{
   public class AdoptOpenJDKProvider : IJdkProvider<AdoptOpenJDKConfig>
   {
      public Func<IJdkDownloader<AdoptOpenJDKConfig>> JDKDownloaderSupplier => () => JDKDownloaderFactory();

      public static Func<IJdkDownloader<AdoptOpenJDKConfig>> JDKDownloaderFactory => () => new AdoptOpenJDKDownloader();
   }
}
