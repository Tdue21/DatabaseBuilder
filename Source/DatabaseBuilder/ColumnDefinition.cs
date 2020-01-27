using System;
using System.Text;

namespace DatabaseBuilder
{
    public class ColumnDefinition
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public string DefaultValue { get; set; }
        public bool Nullable { get; set; }
        public bool Unique { get; set; }
        public bool Identity { get; set; }
        public int Seed { get; set; }
        public int Increment { get; set; }
        public bool PrimaryKey { get; set; }
    }

    public static class ColumnDefinitionExtensions
    {
        public static ColumnDefinition IsIdentity(this ColumnDefinition definition, int seed, int increment)
        {
            definition.Identity = true;
            definition.Seed = seed;
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
            definition.Length = length;
            definition.Precision = precision;
            return definition;
        }
    

        public static string ToScript(this ColumnDefinition column)
        {
            return new StringBuilder().Append(column.ColumnName)
                                      .Append(" ")
                                      .Append(GetColumnType(column))
                                      .Append(column.Identity ? $" IDENTITY({column.Seed}, {column.Increment})" : "")
                                      .Append(column.Nullable ? " NULL" : " NOT NULL")
                                      .Append(!string.IsNullOrWhiteSpace(column.DefaultValue) && !column.Identity ? $" CONSTRAINT df_{column.TableName}_{column.ColumnName} DEFAULT {column.DefaultValue}" : "")
                                      .ToString();
        }

        private static string GetColumnType(ColumnDefinition column)
        {
            string GetPrecision() => $"({column.Length},{column.Precision})";

            var typeName = column.ColumnType.FullName?.ToLowerInvariant();

            switch (typeName)
            {
                case "system.boolean": return "bit";

                case "system.byte": return "tinyint";
                case "system.int16": return "smallint";
                case "system.int32": return "int";
                case "system.int64": return "bigint";

                case "system.char": return "nchar(1)";
                case "system.char[]":
                case "system.string": return $"nvarchar({(column.Length > 0 ? column.Length.ToString() : "max")})";

                case "system.datetime": return "datetime";

                case "system.single": return "real" + GetPrecision();
                case "system.double": return "float" + GetPrecision();
                case "system.decimal": return "numeric" + GetPrecision();

                case "system.guid": return "uniqueidentifier";

                default: throw new NotSupportedException($"The type '{column.ColumnType.FullName}' is not supported for column '{column.ColumnName}'.");
            }
        }
    }
}
