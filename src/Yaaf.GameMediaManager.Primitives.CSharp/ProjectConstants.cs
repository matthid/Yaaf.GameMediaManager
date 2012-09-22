// ----------------------------------------------------------------------------
// This file (ProjectConstants.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.GameMediaManager).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.GameMediaManager
{
    using System;

    public static class ProjectConstants
    {
        public const string DatabaseSchemaVersionString = "1.1.1.0";
        public static readonly Version DatabaseSchemaVersion = new Version(DatabaseSchemaVersionString);
        public const string VersionString = "0.9.3.0";

        public static readonly Version ProjectVersion = new Version(VersionString);

        public const string UrlPrefix = "https://github.com/matthid/Yaaf.GameMediaManager/";

        public const string RawPrefix = "https://raw.github.com/matthid/Yaaf.GameMediaManager/";

#if DEBUG
        public const bool IsRelease = false;
#else
        public const bool IsRelease = true;
#endif

        public static string GetLink(string section)
        {
            return UrlPrefix + section;
        }

        public static string GetRawLink(string file)
        {
            return GetRawLink(file, IsRelease);
        }

        public static string GetRawLink(string file, bool releaseFile)
        {
            return RawPrefix + (releaseFile ? "master" : "core") + "/" + file;
        }
    }
}