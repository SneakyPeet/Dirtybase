using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dirtybase.Core;
using Dirtybase.Core.Options.Validators;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlImplementations.Helpers
{
    public abstract class DbTestBase<TDbConnection> where TDbConnection : DbConnection
    {
        protected abstract string ConnectionString { get; }
        protected abstract string VersionTableName { get; }
        protected abstract string CreateVerisonTableQuery { get; }
        
        protected DirtybaseApi api = new DirtybaseApi(new TestNotifier());

        protected void CreateVersionTable()
        {
            this.RunCommands(new[] { this.CreateVerisonTableQuery });
        }

        protected void RunCommands(IEnumerable<string> queries)
        {
            
            using (var connection = this.MakeConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand()) 
                {
                    try
                    {
                        foreach(var query in queries)
                        {
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        throw;
                    }
                }
                connection.Close();
            }
        }

        protected void AssertAgainstDatabase(Func<TDbConnection, Errors> validator)
        {
            using (var connection = this.MakeConnection())
            {
                connection.Open();
                try
                {
                    var errors = validator(connection);
                    if(errors.Any())
                    {
                        Assert.Fail(errors.Message);
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }

        private TDbConnection MakeConnection()
        {
            return Activator.CreateInstance(typeof(TDbConnection), new object[] { this.ConnectionString }) as TDbConnection;
        }
    }
}