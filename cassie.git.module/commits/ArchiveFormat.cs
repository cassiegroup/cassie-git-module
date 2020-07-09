using System;
namespace cassie.git.module.commits
{
    public enum ArchiveFormat
    {
        ArchiveZip,
        ArchiveTarGz
    }

    public static class ArchiveFormatExtensions
    {
        public static string ToTypeString(this ArchiveFormat ot)
        {
            switch (ot)
            {
                case ArchiveFormat.ArchiveZip:
                    return "zip";
                case ArchiveFormat.ArchiveTarGz:
                    return "tar.gz";
                default:
                    return "";
            }
        }
    }
}