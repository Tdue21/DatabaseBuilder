// MIT License
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

using System;
using System.Text;

namespace DatabaseBuilder
{
    public static class ColumnDefinitionExtensions
    {
        public static ColumnDefinition IsIdentity(this ColumnDefinition definition, int seed, int increment)
        {
            definition.Identity  = true;
            definition.Seed      = seed;
            definition.Increment = increment;
            return definition;
        }

        public static ColumnDefinition IsUnique(this ColumnDefinition definition)
        {
            definition.Unique = true;
            return definition;
        }

        public static ColumnDefinition IsNullable(this ColumnDefinition definition)
        {
            definition.Nullable = true;
            return definition;
        }

        public static ColumnDefinition Default(this ColumnDefinition definition, string defaultValue)
        {
            definition.DefaultValue = defaultValue;
            return definition;
        }

        public static ColumnDefinition IsPrimaryKey(this ColumnDefinition definition)
        {
            definition.PrimaryKey = true;
            return definition;
        }

        public static ColumnDefinition SetLength(this ColumnDefinition definition, int length, int precision = 0)
        {
            definition.Length    = length;
            definition.Precision = precision;
            return definition;
        }


        public static string ToScript(this ColumnDefinition column) =>
            new StringBuilder().Append($"[{column.ColumnName}]")
                               .Append(" ")
                               .Append(GetColumnType(column))
                               .Append(column.Identity ? $" IDENTITY({column.Seed}, {column.Increment})" : "")
                               .Append(column.Nullable ? " NULL" : " NOT NULL")
                               .Append(!string.IsNullOrWhiteSpace(column.DefaultValue) && !column.Identity
                                           ? $" CONSTRAINT df_{column.TableName}_{column.ColumnName} DEFAULT {column.DefaultValue}"
                                           : "")
                               .ToString();

        private static string GetColumnType(ColumnDefinition column)
        {
            string GetPrecision() => $"({column.Precision},{column.Scale})";

            var typeName = column.ColumnType.FullName?.ToLowerInvariant();

            switch (typeName)
            {
                case "system.boolean": return "BIT";

                case "system.byte":  return "TINYINT";
                case "system.int16": return "SMALLINT";
                case "system.int32": return "INT";
                case "system.int64": return "BIGINT";

                case "system.char": return "NCHAR(1)";
                case "system.char[]":
                case "system.string": return $"NVARCHAR({(column.Length > 0 ? column.Length.ToString() : "MAX")})";

                case "system.datetime": return "DATETIME";

                case "system.single":  return "REAL" + GetPrecision();
                case "system.double":  return "FLOAT" + GetPrecision();
                case "system.decimal": return "NUMERIC" + GetPrecision();

                case "system.guid": return "UNIQUEIDENTIFIER";

                default: throw new NotSupportedException($"The type '{column.ColumnType.FullName}' is not supported for column '{column.ColumnName}'.");
            }
        }
    }
}
