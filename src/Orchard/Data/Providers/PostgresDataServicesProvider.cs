using System;
using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;

namespace Orchard.Data.Providers {
    sealed class FixedPostgreSQL82Dialect : PostgreSQL82Dialect {
        // Works around a bug in NHibernate PostgreSQL82 dialect which overrides the 
        // GetIdentityColumnString method but fails to override the IdentityColumnString,
        // which eventually leads to exception being thrown.
        public override string IdentityColumnString {
            get { return "serial"; }
        }

        // Avoid to quote any identifiers for PostgreSQL. Doing that will fold all of them into
        // lower case which will then make it easier to issue queries. When an identifier (e.g.
        // a table name) is enclosed in quotes when creating it you have to always use quotes
        // _and_ the correct case when referring to it thereafter.
        
        protected override string Quote (string name) {
            return name;
        }

        // PostgreSQL does not accept the default Dialect's 0 or 1 value for boolean columns.
        public override string ToBooleanValueString (bool value) {
            return value ? "'t'" : "'f'";
        }
    }

    sealed class FixedPostgreSQLConfiguration : PostgreSQLConfiguration {
        public PostgreSQLConfiguration PostgreSQL82Fixed {
            get { return Dialect <FixedPostgreSQL82Dialect> (); }
        }
    }

    public class PostgresDataServicesProvider : AbstractDataServicesProvider {
        private readonly string _dataFolder;
        private readonly string _connectionString;

        public PostgresDataServicesProvider(string dataFolder, string connectionString) {
            _dataFolder = dataFolder;
            _connectionString = connectionString;
        }
        
        public static string ProviderName {
            get { return "Postgres"; }
        }
        
        public override IPersistenceConfigurer GetPersistenceConfigurer(bool createDatabase) {
            var persistence = new FixedPostgreSQLConfiguration().PostgreSQL82Fixed;
            if (string.IsNullOrEmpty(_connectionString)) {
                throw new ArgumentException("The connection string is empty");
            }
            persistence = persistence.ConnectionString(_connectionString);
            
            return persistence;
        }
    }
}
