using System;
using System.Collections.Generic;
using System.Text;

namespace JDKDownloader.Core.Config
{
   /// <seealso cref="https://api.adoptopenjdk.net/swagger-ui/#/Assets/get_v3_assets_feature_releases__feature_version___release_type_"/>
   public class AdoptOpenJDKConfig
   {
      public const string DEFAULT_BASE_URL = "https://api.adoptopenjdk.net/v3/assets/feature_releases/{feature_version}/{release_type}";
      public const int DEFAULT_FEATURE_VERSION = Defaults.DEFAULT_JAVA_VERSION;

      /// <summary>
      /// Remote URL
      /// </summary>
      public string RemoteBaseURL { get; set; } = DEFAULT_BASE_URL;

      /// <summary>
      /// Version
      /// </summary>
      /// <remarks>
      /// feature_version
      /// </remarks>
      /// <example>
      /// 8, ... 11, 12, ...
      /// </example>
      public int FeatureVersion { get; set; } = DEFAULT_FEATURE_VERSION;

      /// <summary>
      /// ReleaseType
      /// </summary>
      /// <remarks>
      /// release_type 
      /// </remarks>
      /// <example>
      /// ea, ga
      /// </example>
      public string ReleaseType { get; set; } = "ga";

      /// <summary>
      /// Architecture; if null: automatically detected
      /// </summary>
      /// <remarks>
      /// architecture
      /// </remarks>
      /// <example>
      /// x64, arm, x86
      /// </example>
      public string Architecture { get; set; } = null;

      /// <summary>
      /// HeapSize
      /// </summary>
      /// <remarks>
      /// heap_size
      /// </remarks>
      /// <example>
      /// normal, large
      /// </example>
      public string HeapSize { get; set; } = "normal";

      /// <summary>
      /// ImageType
      /// </summary>
      /// <remarks>
      /// image_type
      /// </remarks>
      /// <example>
      /// jdk, jre, testimage
      /// </example>
      public string ImageType { get; set; } = "jdk";

      /// <summary>
      /// JVM Implementation
      /// </summary>
      /// <remarks>
      /// jvm_impl
      /// </remarks>
      /// <example>
      /// hotspot, openj9
      /// </example>
      public string JVMImpl { get; set; } = "hotspot";

      /// <summary>
      /// Operating System; if null: automatically detected
      /// </summary>
      /// <remarks>
      /// os
      /// </remarks>
      /// <example>
      /// windows, mac, linux, ...
      /// </example>
      public string OS { get; set; } = null;

      /// <summary>
      /// Pagination page size (number of results)
      /// </summary>
      /// <remarks>
      /// page_size
      /// </remarks>
      public int PageSize { get; set; } = 1;

      /// <summary>
      /// Project
      /// </summary>
      /// <remarks>
      /// project
      /// </remarks>
      /// <example>
      /// jdk, valhalla, ...
      /// </example>
      public string Project { get; set; } = "jdk";

      /// <summary>
      /// Sort Order; Use latest with DESC
      /// </summary>
      /// <remarks>
      /// sort_order
      /// </remarks>
      /// <example>
      /// ASC, DESC
      /// </example>
      public string SortOrder { get; set; } = "DESC";

      /// <summary>
      /// Vendor
      /// </summary>
      /// <remarks>
      /// vendor
      /// </remarks>
      /// <example>
      /// jdk, valhalla, ...
      /// </example>
      public string Vendor { get; set; } = "adoptopenjdk";
   }
}
