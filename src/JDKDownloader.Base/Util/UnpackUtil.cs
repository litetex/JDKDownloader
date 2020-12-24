using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace JDKDownloader.Base.Util
{
   public static class UnpackUtil
   {
      /// <summary>
      /// Extracts a zipFile into a folder
      /// </summary>
      /// <param name="zipArchiveName"></param>
      /// <param name="destFolder"></param>
      public static void ExtractZIP(string zipArchiveName, string destFolder)
      {
         ZipFile.ExtractToDirectory(zipArchiveName, destFolder);
      }

      /// <summary>
      /// Extracts the gzFile into a folder
      /// </summary>
      /// <param name="gzArchiveName"></param>
      /// <param name="destFolder"></param>
      /// <example>
      /// ExtractTGZ(@"c:\temp\test.tar.gz", @"C:\DestinationFolder")
      /// </example>
      public static void ExtractTGZ(string gzArchiveName, string destFolder)
      {
         using (var inStream = File.OpenRead(gzArchiveName))
         using (var gzipStream = new GZipInputStream(inStream))
         using (var tarArchive = TarArchive.CreateInputTarArchive(gzipStream, null))
            tarArchive.ExtractContents(destFolder);
      }
   }
}
