using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Common.Persistence;

namespace Consumer.Persistence
{
    public class ConsumerDbContext : DbContext, IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        public ConsumerDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //optionsBuilder.UseInMemoryDatabase("ConsumerDbContext");
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Main"), builder => builder
                .MigrationsAssembly(typeof(ConsumerDbContext).Assembly.GetName().Name));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsumerDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Entities.Customer> Customers { get; set; }
        public virtual DbSet<Entities.Transaction> Transactions { get; set; }
        public virtual DbSet<Entities.Order> Orders { get; set; }

        #region UnitOfWork

        public IQueryable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>().AsQueryable();
        }

        public TEntity FindById<TEntity>(Guid id) where TEntity : class
        {
            return base.Set<TEntity>().Find(id);
        }

        public async Task AddAsync<TEntity>(TEntity entry) where TEntity : class
        {
            await base.AddAsync(entry);
        }

        public Task UpdateAsync<TEntity>(TEntity entry) where TEntity : class
        {
            base.Update(entry);
            return Task.CompletedTask;
        }

        public Task DeleteAsync<TEntity>(TEntity entry) where TEntity : class
        {
            base.Remove(entry);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            this.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            this.Database.RollbackTransaction();
        }

        public Task PurgeData(params string[] collections)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
