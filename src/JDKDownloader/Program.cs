using CommandLine;
using JDKDownloader.CMDOptions;
using JDKDownloader.CMDOptions.Download;
using JDKDownloader.Provider;
using JDKDownloader.Provider.AdoptOpenJDK;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JDKDownloader
{
   static class Program
   {
      static void Main(string[] args)
      {
         Run(args);

         //var downloader = AdoptOpenJDKProvider.JDKDownloaderFactory();
         //downloader.UseConfig(new AdoptOpenJDKConfig()
         //{
         //   ImageType = "jre"
         //});
         //downloader.UseDownloadConfig(new DownloadConfig()
         //{
         //   OutputDir = "output"
         //});
         //downloader.Download(new ConsoleDownloadProgressBar());
      }

      public static void Run(string[] args)
      {
         Serilog.Log.Logger = GetDefaultLoggerConfiguration().CreateLogger();

         AppDomain.CurrentDomain.ProcessExit += (s, ev) =>
         {
            Log.Debug("Shutting down logger; Flushing...");
            Serilog.Log.CloseAndFlush();
         };

#if !DEBUG
         try
         {
            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
               try
               {
                  if (ev?.ExceptionObject is Exception ex)
                  {
                     Log.Fatal("An unhandled error occured", ex);
                     return;
                  }

                  Log.Fatal($"An unhandled error occured {ev}");
                  
               }
               catch (Exception ex)
               {
                  Console.Error.WriteLine($"Failed to catch unhandled error '{ev?.ExceptionObject ?? ev}': {ex}");
               }
            };
#endif
         GetDefaultParser()
            .ParseArguments<DownloadOptions>(args)
            .WithParsed(notWorkingOpts => {
               var fixedArgs = new List<string>(args);
               foreach (var x in notWorkingOpts.SubArgs.Skip(1))
                  fixedArgs.Remove(x);

               GetDefaultParser()
                  .ParseArguments<DownloadOptions>(fixedArgs)
                  .WithParsed(dwlOpts =>
                  {
                     SetBaseOptions(dwlOpts);
                     GetDefaultParser()
                        .ParseArguments<AdoptOpenJDKOptions>(args.Skip(1))
                        .WithParsed<AdoptOpenJDKOptions>(opt => Download(dwlOpts, opt, new AdoptOpenJDKProvider()))
                        .WithNotParsed(ParserFail);
                  })
                  .WithNotParsed(ParserFail);
            })
            .WithNotParsed(ParserFail);
#if !DEBUG
         }
         catch (Exception ex)
         {
            Log.Fatal(ex);
         }
#endif
      }

      private static void SetBaseOptions(BaseCmdOptions baseCmdOptions)
      {
         var conf = GetDefaultLoggerConfiguration();

         if (baseCmdOptions.LogLevel != null)
         {
            if (Enum.TryParse(baseCmdOptions.LogLevel, true, out LogEventLevel logEventLevel))
            {
               conf = SetMinimumLevel(conf, logEventLevel);
            }
            else
            {
               Log.Warn($"Failed to parse ${nameof(baseCmdOptions.LogLevel)}");
            }
         }
         else if (baseCmdOptions.Verbose)
            conf = conf.MinimumLevel.Debug();
         else if (baseCmdOptions.Errors)
            conf = conf.MinimumLevel.Error();
         else if (baseCmdOptions.Warnings)
            conf = conf.MinimumLevel.Warning();

         Serilog.Log.Logger = conf.CreateLogger();
      }

      private static LoggerConfiguration SetMinimumLevel(LoggerConfiguration conf, LogEventLevel level) =>
         level switch
         {
            LogEventLevel.Verbose => conf.MinimumLevel.Verbose(),
            LogEventLevel.Debug => conf.MinimumLevel.Debug(),
            LogEventLevel.Information => conf.MinimumLevel.Information(),
            LogEventLevel.Warning => conf.MinimumLevel.Warning(),
            LogEventLevel.Error => conf.MinimumLevel.Error(),
            LogEventLevel.Fatal => conf.MinimumLevel.Fatal(),
            _ => conf.MinimumLevel.ControlledBy(new Serilog.Core.LoggingLevelSwitch(level))
         };

      private static void Download<C,P>(DownloadOptions downloadOptions, C providerOptions, P provider) 
         where C : IJdkProviderConfig
         where P : IJdkProvider<C>
      {
         var downloader = provider.JDKDownloaderSupplier();
         var downloadConfig = new DownloadConfig()
         {
            PerformCheckSumCheck = !downloadOptions.NoCheckSumCheck,
            UseTempDir = downloadOptions.UseTempDir
         };
         if(!string.IsNullOrWhiteSpace(downloadOptions.OutputDir))
            downloadConfig.OutputDir = downloadOptions.OutputDir;

         downloader.UseDownloadConfig(downloadConfig);
         downloader.UseConfig(providerOptions);

         downloader.Download(downloadOptions.NonInteractive ? null : new ConsoleDownloadProgressBar());
      }

      private static Parser GetDefaultParser()
      {
         return new Parser(settings =>
         {
            settings.IgnoreUnknownArguments = true;
            settings.CaseSensitive = false;
            settings.CaseInsensitiveEnumValues = true;
         });
      }

      private static void ParserFail(IEnumerable<Error> errors)
      {
         if (errors.All(err =>
                           new ErrorType[]
                           {
                           ErrorType.HelpRequestedError,
                           ErrorType.HelpVerbRequestedError,
                           ErrorType.UnknownOptionError
                           }.Contains(err.Tag))
                     )
            return;

         foreach (var error in errors)
            Log.Error($"Failed to parse: {error.Tag}");

         Log.Fatal("Failed to process args");
      }

      private static LoggerConfiguration GetDefaultLoggerConfiguration()
      {
         return new LoggerConfiguration()
            .Enrich.WithThreadId()
            .MinimumLevel
#if DEBUG
               .Debug()
#else
               .Information()
#endif
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}");
      }
   }
}
