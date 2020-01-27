using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace DatabaseBuilder
{
    public class DatabaseBuilder
    {
        private string _serverName;
        private int _integratedSecurity = 0;
        private SecureString _securePassword;
        private string _userId = null;
        private string _databaseName = null;
        private DatabaseSettings _settings;
        private readonly List<TableDefinition> _tables;

        public DatabaseBuilder()
        {
            _tables = new List<TableDefinition>();
        }
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
            _userId = null;
            _securePassword = null;
            return this;
        }

        public DatabaseBuilder WithSqlServerLogin(string userId, SecureString password)
        {
            if (_integratedSecurity == 1)
            {
                throw new ApplicationException("Connection has already been set with integration security.");
            }
            _integratedSecurity = 2;
            _userId = userId;
            _securePassword = password;
            return this;
        }

        public DatabaseBuilder CreateDatabase(string name, Action<DatabaseSettings> settings)
        {
            _databaseName = name;
            _settings = new DatabaseSettings();
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
                           DataSource = _serverName,
                           IntegratedSecurity = _integratedSecurity == 1,
                           InitialCatalog = "master"
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
