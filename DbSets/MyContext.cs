﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DbAdm.DbSets
{
    public partial class MyContext : DbContext
    {
        public MyContext()
        {
        }

        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Code> Code { get; set; }
        public virtual DbSet<Column> Column { get; set; }
        public virtual DbSet<Crud> Crud { get; set; }
        public virtual DbSet<CrudEitem> CrudEitem { get; set; }
        public virtual DbSet<CrudEtable> CrudEtable { get; set; }
        public virtual DbSet<CrudQitem> CrudQitem { get; set; }
        public virtual DbSet<CrudRitem> CrudRitem { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Table> Table { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Db;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Code>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.Code1 });

                entity.ToTable("_Code");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Code1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Code");

                entity.Property(e => e.Ext)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Column>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DataType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DefaultValue)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TableId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Crud>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.AuthType).HasComment("權限種類, 0(無), 1(Ctrl), 2(Action)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.ProgCode)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ProgName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ProjectId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ReadSql)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Revised).HasColumnType("datetime");

                entity.Property(e => e.TableAs)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrudEitem>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CheckData)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CheckType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ColumnId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DefaultValue)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EtableId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.InputData)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.InputType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LayoutCols)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PlaceHolder)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PosGroup)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Etable)
                    .WithMany(p => p.CrudEitem)
                    .HasForeignKey(d => d.EtableId)
                    .HasConstraintName("FK_CrudEitem_CrudEtable");
            });

            modelBuilder.Entity<CrudEtable>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Col4)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CrudId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Kid)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MapFid)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.OrderBy)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TableId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Crud)
                    .WithMany(p => p.CrudEtable)
                    .HasForeignKey(d => d.CrudId)
                    .HasConstraintName("FK_CrudEtable_Crud");
            });

            modelBuilder.Entity<CrudQitem>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ColumnId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CrudId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ExtInfo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InputData)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InputType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LayoutCols)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Op)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PosGroup)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TableAs)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Crud)
                    .WithMany(p => p.CrudQitem)
                    .HasForeignKey(d => d.CrudId)
                    .HasConstraintName("FK_CrudQitem_Crud");
            });

            modelBuilder.Entity<CrudRitem>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ColumnCode)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CrudId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ExtInfo)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.RitemType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Crud)
                    .WithMany(p => p.CrudRitem)
                    .HasForeignKey(d => d.CrudId)
                    .HasConstraintName("FK_CrudRitem_Crud");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ConnectStr)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DbName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectPath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ProjectId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
