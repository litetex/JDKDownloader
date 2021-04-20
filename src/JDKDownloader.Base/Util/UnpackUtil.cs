using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using System.Diagnostics.Contracts;

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
      public static void ExtractZIPMinDepth(string zipArchiveName, string destFolder, int skipDepth = 1)
      {
         Contract.Requires(skipDepth > 0, $"{nameof(skipDepth)} has to be greater than 0");

         using(var archive = ZipFile.OpenRead(zipArchiveName))
         {
            foreach (var entry in archive.Entries
               .Where(e => !string.IsNullOrEmpty(e.Name))
               .Where(e => e.FullName.Contains('/') && e.FullName[(e.FullName.NthIndexOf('/', skipDepth) + 1)..].Length > 0)
               )
            {
               string name = entry.FullName[(entry.FullName.NthIndexOf('/', skipDepth) + 1)..];
               var destFile = Path.Combine(destFolder, name);

               if (DoPathTraversalCheck(destFolder, destFile))
               {
                  Log.Warn($"Parent traversal in paths is not allowed! File='{name}'");
                  continue;
               }

               Directory.CreateDirectory(Path.GetDirectoryName(destFile));
               entry.ExtractToFile(destFile);
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
         using var inStream = File.OpenRead(gzArchiveName);
         using var gzipStream = new GZipInputStream(inStream);
         using var tarArchive = TarArchive.CreateInputTarArchive(gzipStream, null);

         tarArchive.ExtractContents(destFolder);
      }

      /// <summary>
      /// Extracts a zipFile into a folder
      /// </summary>
      /// <param name="zipArchiveName"></param>
      /// <param name="destFolder"></param>
      /// <seealso cref="https://github.com/icsharpcode/SharpZipLib/blob/master/src/ICSharpCode.SharpZipLib/Tar/TarArchive.cs#L645"/>
      public static void ExtractTGZMinDepth(string gzArchiveName, string destFolder, int skipDepth = 1)
      {
         Contract.Requires(skipDepth > 0, $"{nameof(skipDepth)} has to be greater than 0");

         using var inStream = File.OpenRead(gzArchiveName);
         using var gzipStream = new GZipInputStream(inStream);
         using var tarStream = new TarInputStream(gzipStream, null);

         TarEntry tarEntry;
         while ((tarEntry = tarStream.GetNextEntry()) != null)
         {
            if (tarEntry.IsDirectory)
               continue;

            string name = tarEntry.Name;
            if (Path.IsPathRooted(name))
            {
               // NOTE:
               // for UNC names...  \\machine\share\zoom\beet.txt gives \zoom\beet.txt
               name = name[Path.GetPathRoot(name).Length..];
            }

            // Check if we have to skip, because of the skip depth
            if(!name.Contains('/') || name[(name.NthIndexOf('/', skipDepth) + 1)..].Length == 0)
               continue;

            // Rewrite the path according to skipDepth
            name = name[(name.NthIndexOf('/', skipDepth) + 1)..];

            // Converts the unix forward slashes in the filenames to os specific backslashes
            name = name.Replace('/', Path.DirectorySeparatorChar);

            string destFile = Path.Combine(destFolder, name);

            if (DoPathTraversalCheck(destFolder, destFile))
            {
               Log.Warn($"Parent traversal in paths is not allowed! File='{name}'");
               continue;
            }

            string directoryName = Path.GetDirectoryName(destFile);

            // Does nothing if directory exists
            Directory.CreateDirectory(directoryName);

            using (FileStream outStr = new FileStream(destFile, FileMode.Create))
               tarStream.CopyEntryContents(outStr);

            // Set the modification date/time. This approach seems to solve timezone issues.
            DateTime myDt = DateTime.SpecifyKind(tarEntry.ModTime, DateTimeKind.Utc);
            File.SetLastWriteTime(destFile, myDt);
         }
      }

      /// <summary>
      /// Checks for path traversals (e.g. abc/../../../overrideSystemBinary.exe)
      /// </summary>
      /// <param name="destFolder"></param>
      /// <param name="destFile"></param>
      /// <returns><code>true</code> when a path traversal occured</returns>
      static bool DoPathTraversalCheck(string destFolder, string destFile)
      {
         return !Path.GetFullPath(destFile).StartsWith(destFolder, StringComparison.InvariantCultureIgnoreCase);
      }

   }
}
