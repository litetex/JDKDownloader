using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Provider
{
   public interface IJdkProvider<in C> where C : IJdkProviderConfig
   {
      Func<IJdkDownloader<C>> JDKDownloaderSupplier { get; }
   }
}
