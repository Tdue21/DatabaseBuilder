using System;

namespace DatabaseBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new DatabaseBuilder()
                              .ConnectToServer("BIT-DES-022")
                              //.WithSqlServerLogin("sa", new NetworkCredential("", "Boyum01").SecurePassword)
                              .WithIntegratedSecurity()

                              .CreateDatabase("TestDB", settings => settings.SetCollation("Latin1_General_100_CI_AS")
                                                                            .SetAllowSnapshotIsolation(false)
                                                                            .SetCompatibilityLevel(150)
                                             )
                              .CreateTable("dbo", "ApplicationVersion", table => table.Column<int>("Id"            , c => c.IsPrimaryKey().IsIdentity(1, 1))
                                                                                      .Column<string>("Item"       , c => c.SetLength(20).IsUnique())
                                                                                      .Column<string>("Description", c => c.SetLength(80))
                                                                                      .Column<int>("Version")
                                                                                      .Column<DateTime>("Created"  , c => c.Default("getdate()"))
                                                                                      .Column<DateTime>("Updated"  , c => c.Default("getdate()"))
                                          )
                              .CreateTable("dbo", "Article", table => table.Column<int>("Id"            , c => c.IsPrimaryKey().IsIdentity(1, 1))
                                                                           .Column<string>("Code"       , c => c.SetLength(20).IsUnique())
                                                                           .Column<string>("Item"       , c => c.SetLength(80).IsNullable())
                                                                           .Column<DateTime>("Created"  , c => c.Default("getdate()"))
                                                                           .Column<DateTime>("Updated"  , c => c.Default("getdate()"))
                                                                           .Column<decimal>("UnitPrice" , c => c.SetLength(12, 6))
                                          )
                              .ToScript()
                             );
            Console.ReadLine();
        }
    }
}
