using CommandLine;
using JDKDownloader.Provider.AdoptOpenJDK.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDKDownloader.CMDOptions.Download
{
   [Verb("adoptopenjdk", isDefault: true, HelpText = "Downloads from AdoptOpenJDK")]
   public class AdoptOpenJDKOptions : AdoptOpenJDKConfig
   {
      [Option(nameof(RemoteBaseURL), Default = DEFAULT_BASE_URL)]
      public override string RemoteBaseURL { get => base.RemoteBaseURL; set => base.RemoteBaseURL = value; }

      [Option(nameof(FeatureVersion), Default = DEFAULT_FEATURE_VERSION)]
      public override int FeatureVersion { get => base.FeatureVersion; set => base.FeatureVersion = value; }

      [Option(nameof(ReleaseType), Default = DEFAULT_REALEASE_TYPE)]
      public override string ReleaseType { get => base.ReleaseType; set => base.ReleaseType = value; }

      [Option(nameof(Architecture), HelpText = "if not set automatically detected")]
      public override string Architecture { get => base.Architecture; set => base.Architecture = value; }

      [Option(nameof(HeapSize), Default = DEFAULT_HEAP_SIZE)]
      public override string HeapSize { get => base.HeapSize; set => base.HeapSize = value; }

      [Option(nameof(ImageType), Default = DEFAULT_IMAGE_TYPE)]
      public override string ImageType { get => base.ImageType; set => base.ImageType = value; }

      [Option(nameof(JVMImpl), Default = DEFAULT_JVM_IMPL)]
      public override string JVMImpl { get => base.JVMImpl; set => base.JVMImpl = value; }

      [Option(nameof(OS), HelpText = "if not set automatically detected")]
      public override string OS { get => base.OS; set => base.OS = value; }

      [Option(nameof(PageSize), Default = DEFAULT_PAGE_SIZE)]
      public override int PageSize { get => base.PageSize; set => base.PageSize = value; }

      [Option(nameof(Project), Default = DEFAULT_PROJECT)]
      public override string Project { get => base.Project; set => base.Project = value; }

      [Option(nameof(SortOrder), Default = DEFAULT_SORT_ORDER)]
      public override string SortOrder { get => base.SortOrder; set => base.SortOrder = value; }

      [Option(nameof(Vendor), Default = DEFAULT_VENDOR)]
      public override string Vendor { get => base.Vendor; set => base.Vendor = value; }
   }
}
