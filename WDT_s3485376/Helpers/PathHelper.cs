using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace WDT_s3485376.Helpers
{
    // Helper for shortening paths
    public static class PathHelper
    {
        public static readonly string BASEPATH = Path.GetFullPath(
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
    }
}
