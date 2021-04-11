
using that2dollar.Models;
using Microsoft.EntityFrameworkCore;

namespace that2dollar.Data
{

    public class CommonDBContext : DbContext
    {

    
        public CommonDBContext(DbContextOptions<CommonDBContext> options)
            : base(options)
        {
        }


        public DbSet<RateToUsd> Rates { get; set; }
        //public DbSet<RateToUsd> Rates { get; set; }
        public DbSet<GlobalQuote> GlobalQuotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            _ = options.UseSqlite("Data Source=./db/tousd.sqlite");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RateToUsd>(entity =>
            {

                entity.ToTable("RateUsd", "toUSD");

                entity.HasKey(e => e.Code)
                    .HasName("PK_CODE");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar")
                    .HasMaxLength(30);


                entity.Property(e => e.Rate)
                    .HasColumnName("rate");
                entity.Property(e => e.Bid)
                     .HasColumnName("bid");
                entity.Property(e => e.Ask)
                     .HasColumnName("ask");
                entity.Property(e => e.Stored)
                     .HasColumnName("stored");
                entity.Property(e => e.LastRefreshed)
                     .HasColumnName("lastRefreshed");
            });

            //    modelBuilder.Entity<Emp>(entity =>
            //    {
            //        entity.HasKey(e => e.Empno)
            //            .HasName("PK_EMP");

            //        entity.ToTable("EMP", "DEMOBASE");

            //        entity.Property(e => e.Deptno).HasColumnName("DEPTNO");

            //        entity.Property(e => e.Ename)
            //            .HasColumnName("ENAME")
            //            .HasColumnType("varchar")
            //            .HasMaxLength(10);

            //        entity.Property(e => e.Hiredate)
            //            .HasColumnType("date");

            //        entity.Property(e => e.Job)
            //            .HasColumnType("varchar")
            //            .HasMaxLength(9);

            //        entity.HasOne(d => d.DeptnoNavigation)
            //             .WithMany(p => p.Emp)
            //             .HasForeignKey(d => d.Deptno)
            //    });
            //}
        }

    }
   
}
