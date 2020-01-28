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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using DatabaseBuilder.Attributes;

namespace DatabaseBuilder
{
    public class DatabaseBuilder
    {
        private readonly List<TableDefinition> _tables;
        private          string                _databaseName;
        private          int                   _integratedSecurity;
        private          SecureString          _securePassword;
        private          string                _serverName;
        private          DatabaseSettings      _settings;
        private          string                _userId;

        public DatabaseBuilder() => _tables = new List<TableDefinition>();

        public DatabaseBuilder ConnectToServer(string serverName)
        {
            _serverName = serverName;
            return this;
        }

        public DatabaseBuilder WithIntegratedSecurity()
        {
            if (_integratedSecurity == 2)
            {
                throw new ApplicationException("Connection has already been set with SQL Server Login.");
            }

            _integratedSecurity = 1;
            _userId             = null;
            _securePassword     = null;
            return this;
        }

        public DatabaseBuilder WithSqlServerLogin(string userId, SecureString password)
        {
            if (_integratedSecurity == 1)
            {
                throw new ApplicationException("Connection has already been set with integration security.");
            }

            _integratedSecurity = 2;
            _userId             = userId;
            _securePassword     = password;
            return this;
        }

        public DatabaseBuilder CreateDatabase(string name, Action<DatabaseSettings> settings)
        {
            _databaseName = name;
            _settings     = new DatabaseSettings();
            settings?.Invoke(_settings);
            return this;
        }

        public DatabaseBuilder CreateTable(string schema, string tableName, Action<TableDefinition> definition)
        {
            var defi = new TableDefinition(schema, tableName);
            definition?.Invoke(defi);
            _tables.Add(defi);
            return this;
        }

        public DatabaseBuilder CreateTable<T>() where T : new()
        {
            var tableDef = typeof(T).GetCustomAttributes(typeof(TableDefinitionAttribute), false).Cast<TableDefinitionAttribute>().FirstOrDefault();
            if (tableDef != null)
            {
                var schemaDef = tableDef.GetType().GetProperty("Schema");
                if (schemaDef != null && tableDef.Schema == null)
                {
                    var schemaDefVal = schemaDef.GetCustomAttributes(typeof(DefaultValueAttribute), false).Cast<DefaultValueAttribute>().FirstOrDefault();
                    if (schemaDefVal != null)
                    {
                        tableDef.Schema = schemaDefVal.Value.ToString();
                    }
                }

                var table = new TableDefinition(tableDef.Schema ?? "XX", tableDef.Name ?? typeof(T).Name);
                var fields = typeof(T).GetProperties();
                foreach (var field in fields)
                {
                    var fieldDef = field.GetCustomAttributes(typeof(ColumnDefinitionAttribute), true).Cast<ColumnDefinitionAttribute>().FirstOrDefault();
                    if (fieldDef != null)
                    {
                        table.Columns.Add(new ColumnDefinition
                                          {
                                              ColumnName = fieldDef.Name ?? field.Name,
                                              ColumnType = field.PropertyType,
                                              DefaultValue = fieldDef.DefaultValue,
                                              Length = fieldDef.Length,
                                              Precision = fieldDef.Precision,
                                              Scale = fieldDef.Scale,
                                              Identity = fieldDef.Identity,
                                              Seed = fieldDef.Seed,
                                              Increment = fieldDef.Increment,
                                              Nullable = fieldDef.Nullable,
                                              PrimaryKey = fieldDef.PrimaryKey,
                                              TableName = $"{table.Schema}_{table.Name}",
                                              Unique = fieldDef.IsUnique
                                          });
                    }

                    var foreignDefs = field.GetCustomAttributes(typeof(ForeignKeyDefinitionAttribute), true).Cast<ForeignKeyDefinitionAttribute>().FirstOrDefault();
                    if (foreignDefs != null)
                    {
                        table.ForeignKeys.Add(new ForeignKeyDefinition
                                              {
                                                  FieldNames = new [] { field.Name},
                                                  Table = foreignDefs.Table,
                                                  ReferenceFields = new []{ foreignDefs.Field},
                                                  OnUpdate = foreignDefs.OnUpdate,
                                                  OnDelete = foreignDefs.OnDelete
                                              });
                    }
                }
                _tables.Add(table);
            }

            return this;
        }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(_serverName))
            {
                throw new ApplicationException("Unable to connect to database server. No server defined.");
            }

            if (_integratedSecurity == 0)
            {
                throw new ApplicationException("Unable to connect to database server. No authentication method defined.");
            }

            var conn = new SqlConnectionStringBuilder
                       {
                           DataSource         = _serverName,
                           IntegratedSecurity = _integratedSecurity == 1,
                           InitialCatalog     = "master"
                       };
            var credential = _integratedSecurity == 2 ? new SqlCredential(_userId, _securePassword) : null;

            using (var db = new SqlConnection(conn.ConnectionString, credential))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = ToScript();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ToFile(string fileName, Encoding encoding)
        {
            var content = ToScript();
            File.WriteAllText(fileName, content, encoding);
        }

        public string ToScript()
        {
            var script = new StringBuilder()
                         .AppendLine($"CREATE DATABASE {_databaseName} COLLATE {_settings.Collation}")
                         .AppendLine("GO")
                         .AppendLine()
                         .AppendLine($"ALTER DATABASE {_databaseName} SET COMPATIBILITY_LEVEL = {_settings.Compatibilitylevel}")
                         .AppendLine("GO")
                         .AppendLine()
                         .AppendLine($"ALTER DATABASE {_databaseName} SET ALLOW_SNAPSHOT_ISOLATION {(_settings.AllowSnapshotIsolation ? "ON" : "OFF")}")
                         .AppendLine("GO")
                         .AppendLine()
                         .AppendLine($"ALTER DATABASE {_databaseName} SET READ_COMMITTED_SNAPSHOT {(_settings.ReadCommittedSnapshot ? "ON" : "OFF")}")
                         .AppendLine("GO")
                         .AppendLine()
                         .AppendLine($"USE {_databaseName}")
                         .AppendLine("GO")
                         .AppendLine()
                         .AppendLine(_tables.Aggregate(string.Empty, (c, n) => c + n.ToScript()))
                ;
            return script.ToString();
        }
    }
}
