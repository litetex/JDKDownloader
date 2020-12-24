using JDKDownloader.Core;
using JDKDownloader.Provider.AdoptOpenJDK;
using JDKDownloader.Provider.AdoptOpenJDK.Config;
using System;

namespace JDKDownloader
{
   class Program
   {
      static void Main(string[] args)
      {
         var config = new AdoptOpenJDKConfig()
         {

         };

         JdkDownloader.Download<AdoptOpenJDK,AdoptOpenJDKConfig>(config);
      }
   }
}
