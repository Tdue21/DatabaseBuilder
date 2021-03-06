﻿// MIT License
// 
// Copyright (c) 2020 Thomas Due
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace DatabaseBuilder
{
    public class DatabaseSettings
    {
        public string Collation              { get; set; }
        public bool   AllowSnapshotIsolation { get; set; }
        public bool   ReadCommittedSnapshot  { get; set; }
        public int    Compatibilitylevel     { get; set; }
    }


    public static class DatabaseSettingsExtensions
    {
        public static DatabaseSettings SetCompatibilityLevel(this DatabaseSettings settings, int compatibilityLevel)
        {
            settings.Compatibilitylevel = compatibilityLevel;
            return settings;
        }

        public static DatabaseSettings SetCollation(this DatabaseSettings settings, string collation)
        {
            settings.Collation = collation;
            return settings;
        }

        public static DatabaseSettings SetAllowSnapshotIsolation(this DatabaseSettings settings, bool allowSnapshotIsolation)
        {
            settings.AllowSnapshotIsolation = allowSnapshotIsolation;
            return settings;
        }
    }
}
