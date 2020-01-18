using System;
using Microsoft.EntityFrameworkCore;
using SearchApi.Config;
using SearchApi.Entity;


namespace SearchApi.Repositories
{
    public class SearchRepository : DbContext
    {
        private const string database = "search";
        
        private readonly AppSettings appSettings;

        private readonly bool isEncript;
        
        public string DebugConString { get; set; }

        public DbSet<SearchGoods> searchGoods { get; set; }

        public SearchRepository(DbContextOptions<SearchRepository> options,
            AppSettings _appSettings) : base(options)
        {
            appSettings = _appSettings;            
            if (!appSettings.DBConnection.Contains("localhost"))
            {
                isEncript = true;
            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbOption = "";
                string dbConnectionString = string.Empty;
                if (isEncript)
                {
                    throw new NotImplementedException("암호화 모듈을 적용하시오~");
                }
                else
                {
                    dbConnectionString = appSettings.DBConnection + $"database={database};" + dbOption;
                }                
                optionsBuilder.UseMySql(dbConnectionString);
            }
        }
    }
}
