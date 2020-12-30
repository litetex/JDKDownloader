using JDKDownloader.Base.Download;
using JDKDownloader.Core;
using JDKDownloader.Provider.AdoptOpenJDK;
using JDKDownloader.Provider.AdoptOpenJDK.Config;
using System;

namespace JDKDownloader
{
   static class Program
   {
      static void Main(string[] args)
      {
         var config = new AdoptOpenJDKConfig()
         {
            ImageType = "jre",
         };

         var dwlConfig = new DownloadConfig()
         {
            OutputDir = "output",
         };

         JdkDownloader.Download<AdoptOpenJDK,AdoptOpenJDKConfig>(config, dwlConfig);
      }
   }
}
