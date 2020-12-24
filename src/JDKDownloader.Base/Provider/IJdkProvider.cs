using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Base.Provider
{
   public interface IJdkProvider<in C> where C : IJdkProviderConfig
   {
      IJdkDownloader<C> JDKDownloader { get; }
   }
}
