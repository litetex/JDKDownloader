using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDKDownloader.Base.Util
{
   public static class IOUtil
   {
      public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
      {
         foreach (DirectoryInfo dir in source.GetDirectories())
            CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
         foreach (FileInfo file in source.GetFiles())
            file.CopyTo(Path.Combine(target.FullName, file.Name));
      }
   }
}
