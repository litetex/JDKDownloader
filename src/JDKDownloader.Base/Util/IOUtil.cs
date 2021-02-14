﻿using CoreFramework.Base.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDKDownloader.Base.Util
{
   public static class IOUtil
   {
      public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target, bool overwrite = true)
      {
         foreach (DirectoryInfo dir in source.GetDirectories())
            CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name), overwrite);
         foreach (FileInfo file in source.GetFiles())
            file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
      }

      public static string GenerateTempDir(string requestedTempDir)
      {
         var tempDir = requestedTempDir;
         if (tempDir == null)
         {
            tempDir = Path.GetTempFileName();
            File.Delete(tempDir);
         }
         else
         {
            tempDir = Path.Combine(tempDir, Path.GetRandomFileName());
         }
         DirUtil.EnsureCreatedAndClean(tempDir);
         return tempDir;
      }
   }
}
