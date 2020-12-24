using JDKDownloader.Base.Provider;
using JDKDownloader.Provider.AdoptOpenJDK.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Provider.AdoptOpenJDK
{
   public class AdoptOpenJDK : IJdkProvider<AdoptOpenJDKConfig>
   {
      public IJdkDownloader<AdoptOpenJDKConfig> JDKDownloader => new AdoptOpenJDKDownloader();
   }
}
