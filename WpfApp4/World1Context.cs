using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp4;

public partial class World1Context : DbContext
{
    public World1Context()
    {
    }

    public World1Context(DbContextOptions<World1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=HOMEASUS\\MSSQLSERVER1;initial catalog=world1;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("city");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.Population).HasDefaultValueSql("('0')");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_city_country");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("country");

            entity.Property(e => e.Code)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.Capital).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Continent)
                .HasMaxLength(255)
                .HasDefaultValue("Asia");
            entity.Property(e => e.Name)
                .HasMaxLength(52)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.Population).HasDefaultValueSql("('0')");
            entity.Property(e => e.SurfaceArea)
                .HasDefaultValueSql("('0.00')")
                .HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
