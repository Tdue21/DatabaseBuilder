using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public virtual string ToScript()
        {
            return new StringBuilder().Append(ColumnName)
                                      .Append(" ")
                                      .Append(GetColumnType())
                                      .Append(Identity ? $" IDENTITY({Seed}, {Increment})" : "")
                                      .Append(Nullable ? " NULL" : " NOT NULL")
                                      .Append(!string.IsNullOrWhiteSpace(DefaultValue) && !Identity ? $" CONSTRAINT df_{TableName}_{ColumnName} DEFAULT {DefaultValue}" : "")
                                      .ToString();
        }

        protected string GetColumnType()
        {
            string GetPrecision() => $"({Length},{Precision})";

            var typeName = ColumnType.FullName?.ToLowerInvariant();

            switch (typeName)
            {
                case "system.boolean": return "bit";

                case "system.byte": return "tinyint";
                case "system.int16": return "smallint";
                case "system.int32": return "int";
                case "system.int64": return "bigint";

                case "system.char": return "nchar(1)";
                case "system.char[]":
                case "system.string": return $"nvarchar({(Length > 0 ? Length.ToString() : "max")})";

                case "system.datetime": return "datetime";

                case "system.single": return "real" + GetPrecision();
                case "system.double": return "float" + GetPrecision();
                case "system.decimal": return "numeric" + GetPrecision();

                case "system.guid": return "uniqueidentifier";

                default: throw new NotSupportedException($"The type '{ColumnType.FullName}' is not supported for column '{ColumnName}'.");
            }
        }
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
    }
}
