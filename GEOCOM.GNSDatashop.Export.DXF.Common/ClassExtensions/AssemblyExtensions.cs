using System;
using System.IO;
using System.Reflection;

namespace GEOCOM.GNSDatashop.Export.DXF.Common.ClassExtensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get the load directory (Full path withouut assembly name and it's extension)
        /// Call to get load from directory by Assembly.GetExecutingAssembly().LoadedFromDirectory()
        /// </summary>
        /// <param name="assy">The assy from whom to get the loaded from path/directory</param>
        /// <returns></returns>
        public static string LoadedFromDirectory(this Assembly assy)
        {
            UriBuilder uri = new UriBuilder(assy.CodeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Get build date.
        /// he most reliable method turns out to be retrieving the linker timestamp from the PE 
        /// header embedded in the executable file -- some C# code (by Joe Spivey) for that from 
        /// the comments to Jeff's article:
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }
}
