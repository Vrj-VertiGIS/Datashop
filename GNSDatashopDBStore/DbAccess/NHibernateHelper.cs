using GEOCOM.GNSD.DBStore.Config;
using GEOCOM.GNSDatashop.Model.UserData;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GEOCOM.GNSD.DBStore.DbAccess
{
    using System.Configuration;
    using Configuration = NHibernate.Cfg.Configuration;

    public class NHibernateHelper
    {

        // für ddl export
        public static Configuration HypernateConfig { get; private set; }

        private static ISessionFactory _sessionFactory;

        /// <summary>
        /// Assembly that provided the NHibernate configuration as its embedded resources.
        /// </summary>
        private static readonly Assembly ConfigResoucesAssembly = typeof(NHibernateHelper).Assembly;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    Configuration cfg = InitNHibernateConfiguration();

                    HypernateConfig = cfg;
                    var dbconconfig = DbStoreConfig.Instance.DbConnection;

                    // general properties
                    cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                    cfg.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, dbconconfig.ConnectionTimeoutSec ?? "600"); // 10 minutes timeout
                    
                    // You can add your own implementation to get the connection but in our case we don't need it anymore.
                    cfg.SetProperty(NHibernate.Cfg.Environment.ShowSql, "False");
                  
                    switch (dbconconfig.Provider.ToLower())
                    {
                        case "system.data.sqlclient":
                            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");

                            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(dbconconfig.Connectionstring);
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, sb.ConnectionString);
                            break;

                        case "system.data.oledb": // Oracle dialtect
                            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.Oracle10gDialect");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.OleDbDriver");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, dbconconfig.Connectionstring);
                            break;

                        case "oracle.dataaccess.client":
                            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.Oracle10gDialect");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.OracleDataClientDriver");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, dbconconfig.Connectionstring);
                            break;

                        case "mysql.data.mysqlclient":
                            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MySQLDialect");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.MySqlDataDriver");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, dbconconfig.Connectionstring);
                            break;

                        case "microsoft.jet.oledb.4.0":
                            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.JetDriver.JetDialect, NHibernate.JetDriver");
                            cfg.SetProperty("connection.driver_class", "NHibernate.JetDriver.JetDialect, NHibernate.JetDriver");
                            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, dbconconfig.Connectionstring);
                            break;

                        default:
                            throw new Exception("connection provider not supported:" + dbconconfig.Provider);
                    }

                    cfg.AddAssembly(typeof(User).Assembly);
                    _sessionFactory = cfg.BuildSessionFactory();

                }
                return _sessionFactory;
            }
        }


        /// <summary>
        /// First attempts to read the NHibernate configuration from the config folder [ApplicationRootPath]\Config\. 
        /// If the attempt fails, it reads the default configuration from the embedded resouces of the DBStore assembly.
        /// </summary>
        private static Configuration InitNHibernateConfiguration()
        {
            const string nHibernateConfigName = "hibernate.cfg.xml";
            const string hmbName = "User.hbm.xml";
            Configuration cfg;

            //attempt to read from the config folder
            var configPath = Path.Combine(ConfigurationManager.AppSettings["ApplicationRootPath"], "Config");
            string hibernateConfigPath = Path.Combine(configPath, nHibernateConfigName);
            string hbmPath = Path.Combine(configPath, hmbName);
            if (File.Exists(hibernateConfigPath) && File.Exists(hbmPath))
            {
                cfg = new Configuration().Configure(hibernateConfigPath);
                cfg.AddFile(hbmPath);
                return cfg;
            }

            //attempt to read from embedded resources of the DBStore dll
            string hibernateConfigResourceName = GetExactEmbeddedResouceName(nHibernateConfigName);
            string hbmResourceName = GetExactEmbeddedResouceName(hmbName);
            if (!string.IsNullOrEmpty(hibernateConfigResourceName) && !string.IsNullOrEmpty(hbmResourceName))
            {
                cfg = new Configuration().Configure(ConfigResoucesAssembly, hibernateConfigResourceName);
                cfg.AddResource(hbmResourceName, ConfigResoucesAssembly);
                return cfg;
            }

            //if no success, throw a detailed exception
            string message =
                string.Format(
                    "The NHibernate configuration 'hibernate.cfg.xml' or the mapping 'User.hbm.xml' could not found." +
                    "Please verify that either files '{0}', '{1}' exist or that the '{2}' assembly contains the configuration as embedded resources.",
                    hibernateConfigPath, hbmPath, ConfigResoucesAssembly.Location);
            throw new ApplicationException(message);
        }

        private static string GetExactEmbeddedResouceName(string searchResourceName)
        {
            string exactName = ConfigResoucesAssembly.GetManifestResourceNames().SingleOrDefault(resName => resName.Contains(searchResourceName));
            return exactName;
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static void CreateSchema(string fileName)
        {
            ISession session = SessionFactory.OpenSession();
            SchemaExport schemaExport = new SchemaExport(HypernateConfig);
            schemaExport.Execute(false, false, false, session.Connection, new StreamWriter(fileName));

            session.Close();
        }

        public static void CreateSchemaOnDB()
        {
            ISession session = SessionFactory.OpenSession();
            SchemaExport schemaExport = new SchemaExport(HypernateConfig);
            schemaExport.Execute(false, true, false, session.Connection, null);
            session.Close();
        }

        public static void DropSchemaOnDB()
        {
            ISession session = SessionFactory.OpenSession();
            SchemaExport schemaExport = new SchemaExport(HypernateConfig);
            schemaExport.Drop(false, true);
            session.Close();
        }

        // throws an exception if not successfull
        public static void TestOracleConnection(string connectionString)
        {
            Configuration cfg = new Configuration();

            // general properties
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty(NHibernate.Cfg.Environment.ShowSql, "True");
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.Oracle10gDialect");
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.OracleDataClientDriver");
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);

            cfg.AddAssembly(typeof(User).Assembly);
            ISessionFactory factory = cfg.BuildSessionFactory();
            ISession session = factory.OpenSession();

            try
            {
                if (!session.IsOpen) throw new Exception("Failed to connect to " + connectionString);

                if (session.Connection.State == ConnectionState.Closed) throw new Exception("Connection failed");
            }
            finally
            {
                session.Close();
            }
        }

        public static void TestMsSqlConnection(SqlConnectionStringBuilder sb)
        {
            Configuration cfg = new Configuration();

            // general properties
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty(NHibernate.Cfg.Environment.ShowSql, "True");
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect");
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, sb.ConnectionString);

            cfg.AddAssembly(typeof(User).Assembly);
            ISessionFactory factory = cfg.BuildSessionFactory();
            ISession session = factory.OpenSession();

            try
            {
                if (!session.IsOpen) throw new Exception("Failed to connect to " + sb.ConnectionString);

                if (session.Connection.State == ConnectionState.Closed) throw new Exception("Connection failed");
            }
            finally
            {
                session.Close();
            }
        }
    }
}

