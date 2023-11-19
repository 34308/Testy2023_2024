using JJ_API.Configuration;

namespace JJ_API.Service
{
    public class PropertiesSingleton
    {
        public SqlServerSettings FactoryLinkDBConnection { get; set; } = new SqlServerSettings();
        public JwtSettings jwtSettings { get; set; } = new JwtSettings();
    }
}
