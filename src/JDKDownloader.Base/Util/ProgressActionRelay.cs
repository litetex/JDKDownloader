using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace JDKDownloader.Base.Util
{
   public class ProgressActionRelay<T> : IProgress<T>
   {
      private Action<T> OnReport { get; set; }

      public ProgressActionRelay(Action<T> onReport)
      {
         if (onReport == null)
            throw new ArgumentNullException(nameof(onReport));

         OnReport = onReport;
      }

      public void Report(T value)
      {
         OnReport?.Invoke(value);
      }

      public void Unbind()
      {
         OnReport = null;
      }
   }
}
