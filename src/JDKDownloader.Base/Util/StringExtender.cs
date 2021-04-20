using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JDKDownloader.Base.Util
{
   /// <seealso cref="https://stackoverflow.com/a/187394"/>
   public static class StringExtender
   {
      public static int NthIndexOf(this string target, char value, int n)
      {
         return NthIndexOf(target, value.ToString(), n);
      }

      public static int NthIndexOf(this string target, string value, int n)
      {
         Match m = Regex.Match(target, "((" + Regex.Escape(value) + ").*?){" + n + "}");

         if (m.Success)
            return m.Groups[2].Captures[n - 1].Index;
         else
            return -1;
      }
   }
}
