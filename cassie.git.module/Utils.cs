using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Security.AccessControl;
using System.IO;
using System.Security.Principal;

namespace cassie.git.module
{
    public class Utils
    {
        private char escapedSlash= '\\';
        private string regularSlash = "\\";
        private char escapedTab = '\t';
        private string regularTab = @"\t";
        public static ConcurrentDictionary<string,object> ObjectCache = new ConcurrentDictionary<string, object>();
        public Utils(){}

        // IsDir returns true if given path is a directory,
        // or returns false when it's a file or does not exist.
        public static bool IsDir(string dir)
        {
            var di = new DirectoryInfo(dir);
            return di.Exists;
        }

        // IsFile returns true if given path is a file,
        // or returns false when it's a directory or does not exist.
        public static bool IsFile(string file)
        {
            var fi = new FileInfo(file);
            return fi.Exists;
        }

        // IsExist checks whether a file or directory exists.
        // It returns false when the file or directory does not exist.
        public static bool IsExist(string path)
        {
            return IsDir(path) && IsFile(path);
        }

        public static DateTime UnixTimeStampToDateTime(Int64 unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime LongDateToDateTime(string date,string culture="en-US")
        {
            var cultureInfo = string.IsNullOrEmpty(culture)?CultureInfo.InvariantCulture:new CultureInfo(culture);
            return DateTime.ParseExact(date, "ddd MMM d HH:mm:ss yyyy zz00", cultureInfo);
        }

        public static bool Mkdir(string path)
        {
            try
            {
                // Make sure directory exists
                if (Directory.Exists(path) == false)
                    throw new Exception(string.Format("Directory {0} does not exist, so permissions cannot be set.", path));

                // Get directory access info
                var di = new DirectoryInfo(path);
                
                var dSecurity = di.GetAccessControl();

                // Add the FileSystemAccessRule to the security settings. 
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                // Set the access control
                di.SetAccessControl(dSecurity);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string EscapePath(string path)
        {
            if(string.IsNullOrEmpty(path)) return path;
            if(path[0]==':') path = @"\"+path;
            return path;
        }

        // public static string UnescapeChars(string str)
        // {
        //     if(str.Contains("\\\t")) return str;
        //     var out
        // }


        public static string ToRfc3339String(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
