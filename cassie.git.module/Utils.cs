using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Security.AccessControl;
using System.IO;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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
                if (Directory.Exists(path)) return true;

                // Get directory access info
                var di = Directory.CreateDirectory(path);
                
                
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

        public static bool Mkfile(string path)
        {
            try
            {
                // Make sure directory exists
                if (File.Exists(path)) return true;

                var directory = System.IO.Path.GetDirectoryName(path);
                if(!Directory.Exists(directory)) Mkdir(directory);
                // Get directory access info
                if(!File.Exists(path)) File.Create(path);
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


        public static string ToRfc3339String(DateTime? dateTime)
        {
            return ((DateTime)dateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }

        public static string CleanFileName(string toCleanPath)
        {
            var replaceWith = "";
            //get just the filename - can't use Path.GetFileName since the path might be bad!  
            string[] pathParts = toCleanPath.Split(new char[] { '\\' });
            //get just the path  
            string newPath = toCleanPath;
            //clean bad path chars  
            foreach (char badChar in Path.GetInvalidPathChars())
            {
                newPath = newPath.Replace(badChar.ToString(), replaceWith);
            }
            //return new, clean path:  
            return newPath ;
        }

        public static void CopyAllTo<T>(T source, T target) where T : new()
        {
            var type = typeof(T);
            foreach (var sourceProperty in type.GetProperties())
            {
                var targetProperty = type.GetProperty(sourceProperty.Name);
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
            foreach (var sourceField in type.GetFields())
            {
                var targetField = type.GetField(sourceField.Name);
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }

       public static string GetImageType(byte[] bytes)
        {
            string headerCode = GetHeaderInfo(bytes).ToUpper();

            if (headerCode.StartsWith("FFD8FFE0"))
            {
                return "JPG";
            }
            else if (headerCode.StartsWith("49492A"))
            {
                return "TIFF";
            }
            else if (headerCode.StartsWith("424D"))
            {
                return "BMP";
            }
            else if (headerCode.StartsWith("474946"))
            {
                return "GIF";
            }
            else if (headerCode.StartsWith("89504E470D0A1A0A"))
            {
                return "PNG";
            }
            else
            {
                return ""; //UnKnown
            }
        }

        public static string GetHeaderInfo(byte[] bytes)
        {
            byte[] buffer = bytes.Take(8).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private class ByteSearchStore
        {
            public int Index { get; set; }
            public int StartingIndex { get; set; }
        }
    }

    public enum ImageFormat
    {
        Bmp,
        Jpeg,
        Gif,
        Tiff,
        Png,
        Unknown
    }
}
