using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DatabaseBuilder
{
    public class TableDefinition
    {
        private string[] _primaryKey;
        public string Schema { get; set; }
        public string Name { get; set; }
        public List<ColumnDefinition> Columns { get; }

        public TableDefinition(string name) : this("dbo", name) { }

        public TableDefinition(string schema, string name)
        {
            Schema = schema;
            Name = name;
            Columns = new List<ColumnDefinition>();
        }

        public TableDefinition Column<T>(string fieldname)
        {
            var column = new ColumnDefinition {TableName = $"{Schema}_{Name}", ColumnName = fieldname, ColumnType = typeof(T)};
            Columns.Add(column);
            return this;
        }


        public TableDefinition Column<T>(string fieldname, Action<ColumnDefinition> definition)
        {
            var column = new ColumnDefinition {TableName = $"{Schema}_{Name}", ColumnName = fieldname, ColumnType = typeof(T)};
            definition?.Invoke(column);
            Columns.Add(column);
            return this;
        }

        public string ToScript()
        {
            _primaryKey = Columns.Where(c => c.PrimaryKey).Select(c => c.ColumnName).ToArray();
            var script = new StringBuilder()
                         .AppendLine($"CREATE TABLE {Schema}.{Name}")
                         .AppendLine("(")
                         .AppendLine(Columns.Aggregate(string.Empty, (c, n) => $"{c}{n.ToScript()},{Environment.NewLine}"))
                         // TODO Primary and other keys
                         .AppendLine($"CONSTRAINT pk_{Schema}_{Name} PRIMARY KEY ({string.Join(",",_primaryKey)})")
                         .AppendLine(");")
                         .AppendLine("GO")
                ;

            return script.ToString();
        }
    }
}
