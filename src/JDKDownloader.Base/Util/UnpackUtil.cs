using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

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
      /// Extracts a zipFile into a folder
      /// </summary>
      /// <param name="zipArchiveName"></param>
      /// <param name="destFolder"></param>
      public static void ExtractZIPMinDepth(string zipArchiveName, string destFolder, int mindepth = 0)
      {
         using(var archive = ZipFile.OpenRead(zipArchiveName))
         {
            foreach (var entry in archive.Entries
               .Where(e => !string.IsNullOrEmpty(e.Name))
               .Where(e => e.FullName.Contains('/') && e.FullName[(e.FullName.IndexOf('/') + 1)..].Length > 0)
               )
            {
               var destPath = Path.Combine(destFolder, entry.FullName[(entry.FullName.IndexOf('/') + 1)..]);
               Directory.CreateDirectory(Path.GetDirectoryName(destPath));
               entry.ExtractToFile(destPath);
            } 
         }
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

      /// <summary>
      /// Extracts a zipFile into a folder
      /// </summary>
      /// <param name="zipArchiveName"></param>
      /// <param name="destFolder"></param>
      public static void ExtractTGZMinDepth(string gzArchiveName, string destFolder, int mindepth = 0)
      {
         //using (var archive = TarArchive.CreateInputTarArchive(zipArchiveName))
         //{
         //   archive.
         //   foreach (var entry in archive.Entries
         //      .Where(e => !string.IsNullOrEmpty(e.Name))
         //      //.Where(e => e.FullName)
         //      )
         //      Console.WriteLine(entry.FullName);
         //   //entry.ExtractToFile(Path.Combine(destFolder, entry.FullName));


         //}
      }
   }
}
