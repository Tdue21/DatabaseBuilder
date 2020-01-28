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
using System.Linq;
using System.Text;

namespace DatabaseBuilder
{
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
                         .AppendLine($"CREATE TABLE [{table.Schema}].[{table.Name}]")
                         .AppendLine("(")
                         .AppendLine(table.Columns.Aggregate(string.Empty, (c, n) => $"{c}{n.ToScript()},{Environment.NewLine}"))
                         .AppendLine($"CONSTRAINT pk_{table.Schema}_{table.Name} PRIMARY KEY ([{string.Join("],[", primaryKey)}]),")
                         .AppendLine(table.ForeignKeys.Aggregate(string.Empty, (c,n) => $"{c}{n.ToScript($"{table.Schema}_{table.Name}")},{Environment.NewLine}"));

            var tableScript = script.ToString().Trim();
            if (tableScript.EndsWith(","))
            {
                script = new StringBuilder(tableScript.Substring(0, tableScript.Length - 1));
            }

            script.AppendLine()
                  .AppendLine(");")
                  .AppendLine("GO");

            return script.ToString();
        }
    }
}
