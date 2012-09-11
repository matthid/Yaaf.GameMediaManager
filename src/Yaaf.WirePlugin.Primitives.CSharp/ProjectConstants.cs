// ----------------------------------------------------------------------------
// This file (ProjectConstants.cs) is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package (Yaaf.WirePlugin).
// Last Modified: 2012/09/10 14:08
// Created: 2012/08/26 20:57
// ----------------------------------------------------------------------------

namespace Yaaf.WirePlugin
{
    using System;

    public static class ProjectConstants
    {
        public const string DatabaseSchemaVersionString = "1.1.0.0";
        public static readonly Version DatabaseSchemaVersion = new Version(DatabaseSchemaVersionString);
        public const string VersionString = "0.8.8.0";

        public static readonly Version ProjectVersion = new Version(VersionString);
    }
}