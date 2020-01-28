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
using DatabaseBuilder;
using TestApp.Definitions;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(new DatabaseBuilder.DatabaseBuilder()
                              .ConnectToServer("BIT-DES-022")
                              //.WithSqlServerLogin("sa", new NetworkCredential("", "Boyum01").SecurePassword)
                              .WithIntegratedSecurity()
                              .CreateDatabase("TestDB", settings => settings.SetCollation("Latin1_General_100_CI_AS")
                                                                            .SetAllowSnapshotIsolation(false)
                                                                            .SetCompatibilityLevel(150)
                                             )
                              .CreateTable<ApplicationVersion>()
                              .CreateTable<Article>()
                              .CreateTable<Order>()

                              //.CreateTable("dbo", "Article", table => table.Column<int>("Id", c => c.IsPrimaryKey().IsIdentity(1, 1))
                              //                                             .Column<string>("Code", c => c.SetLength(20).IsUnique())
                              //                                             .Column<string>("Item", c => c.SetLength(80).IsNullable())
                              //                                             .Column<DateTime>("Created", c => c.Default("getdate()"))
                              //                                             .Column<DateTime>("Updated", c => c.Default("getdate()"))
                              //                                             .Column<decimal>("UnitPrice", c => c.SetLength(12, 6))
                              //            )
                              .ToScript()
                             );
            Console.ReadLine();
        }
    }
}
