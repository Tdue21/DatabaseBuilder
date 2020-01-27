using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DatabaseBuilder
{
    public class TableDefinition
    {
        public TableDefinition(string name) : this("dbo", name) { }

        public TableDefinition(string schema, string name)
        {
            Schema = schema;
            Name = name;
            Columns = new List<ColumnDefinition>();
        }

        public string Schema { get; set; }
        public string Name { get; set; }
        public List<ColumnDefinition> Columns { get; }
    }

    public static class TableDefinitionExtensions
    {
        public static TableDefinition Column<T>(this TableDefinition table, string fieldname)
        {
            var column = new ColumnDefinition {TableName = $"{table.Schema}_{table.Name}", ColumnName = fieldname, ColumnType = typeof(T)};
            table.Columns.Add(column);
            return table;
        }

        public static TableDefinition Column<T>(this TableDefinition table, string fieldname, Action<ColumnDefinition> definition)
        {
            var column = new ColumnDefinition {TableName = $"{table.Schema}_{table.Name}", ColumnName = fieldname, ColumnType = typeof(T)};
            definition?.Invoke(column);
            table.Columns.Add(column);
            return table;
        }

        public static string ToScript(this TableDefinition table)
        {
            var primaryKey = table.Columns.Where(c => c.PrimaryKey).Select(c => c.ColumnName).ToArray();
            var script = new StringBuilder()
                         .AppendLine($"CREATE TABLE {table.Schema}.{table.Name}")
                         .AppendLine("(")
                         .AppendLine(table.Columns.Aggregate(string.Empty, (c, n) => $"{c}{n.ToScript()},{Environment.NewLine}"))
                         // TODO Primary and other keys
                         .AppendLine($"CONSTRAINT pk_{table.Schema}_{table.Name} PRIMARY KEY ({string.Join(",",primaryKey)})")
                         .AppendLine(");")
                         .AppendLine("GO")
                ;

            return script.ToString();
        }
    }
}
