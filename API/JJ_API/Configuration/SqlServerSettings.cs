namespace JJ_API.Service
{

    public class SqlServerSettings : IEquatable<SqlServerSettings>
    {
        private string user = string.Empty;
        private string pass = string.Empty;
        private string dataSource = string.Empty;
        private string dataBase = string.Empty;
        private bool windowsAuthentication = false;

        public SqlServerSettings()
        {
            this.user = "sa";
            this.pass = "5540";
            this.dataSource = "localhost\\sqljj";
            this.dataBase = "JJDB";
            this.windowsAuthentication = false;
        }
        public SqlServerSettings(string user, string pass, string ds, string db, bool windowsAuthentication)
        {
            this.user = user;
            this.pass = pass;
            this.dataSource = ds;
            this.dataBase = db;
            this.windowsAuthentication = windowsAuthentication;
        }

        public string DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }
        public string DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }
        public bool WindowsAuthentication
        {
            get { return this.windowsAuthentication; }
            set { this.windowsAuthentication = value; }
        }
        public string ConnectionString
        {
            get
            {
                string connectionString = string.Empty;

                if (this.windowsAuthentication)
                {
                    connectionString = ("Server=tcp:jjserver.database.windows.net,1433;Initial Catalog=JJDB;Persist Security Info=False;User ID=jjsa;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                }
                else
                {
                    connectionString = string.Format("Data Source=\"{0}\";Initial Catalog={1};User={2};Password = {3};Persist Security Info=True;Pooling=False;TrustServerCertificate=true", dataSource, dataBase, user, pass);
                }

                return connectionString;
            }
        }

        public string ConnectionStringDatabase(string database)
        {
            string connectionString = string.Empty;

            if (this.windowsAuthentication)
            {
                connectionString = string.Format("Data Source=\"{0}\";Initial Catalog={1};Integrated Security=False;Pooling=False;TrustServerCertificate=true", dataSource, database);
            }
            else
            {
                connectionString = string.Format("Data Source=\"{0}\";Initial Catalog={1};User={2};Password = {3};Persist Security Info=True;Pooling=False;TrustServerCertificate=true", dataSource, database, user, pass);
            }

            return connectionString;
        }
        public SqlServerSettings Clone()
        {
            return Clone(this.dataBase);
        }
        public SqlServerSettings Clone(string Db)
        {
            return new SqlServerSettings(this.user, this.pass, this.dataSource, Db, this.windowsAuthentication);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SqlServerSettings settingObj = obj as SqlServerSettings;

            if (settingObj == null)
                return false;
            else
                return Equals(settingObj);
        }
        public bool Equals(SqlServerSettings other)
        {
            return this.user == other.user && this.pass == other.pass && this.dataSource == other.dataSource && this.dataBase == other.dataBase && this.windowsAuthentication == other.windowsAuthentication;
        }
        public override int GetHashCode()
        {
            return this.user.GetHashCode() ^ this.pass.GetHashCode() ^ this.dataBase.GetHashCode() ^ this.dataSource.GetHashCode() ^ this.windowsAuthentication.GetHashCode();
        }
    }

}
