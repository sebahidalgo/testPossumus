using ApiRRHH.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRRHH.Services.Context
{
    public partial class ApiRRHHContext : DbContext
    {
        protected ApiRRHHContext()
        {
        }

        public ApiRRHHContext(DbContextOptions<ApiRRHHContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Candidato> Candidatos { get; set; } = null!;
        public virtual DbSet<Empleo> Empleos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidato>(entity =>
            {
                entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
                entity.Property(e => e.Dni).ValueGeneratedNever();
                entity.Property(e => e.Nombre).HasMaxLength(50);
                entity.Property(e => e.Apellido).HasMaxLength(50);

            });
            modelBuilder.Entity<Empleo>(entity =>
            {
                entity.HasOne(d => d.Candidato).WithMany(p => p.Empleos)
                .HasForeignKey(d => d.CandidatoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleos_Candidatos");

            });

            modelBuilder.Entity<Candidato>()
                .HasData(
                    new Candidato
                    {
                         Dni = 28574087,
                         Apellido = "HIDALGO",
                         Nombre = "SEBASTIAN",
                         Email = "sebahidalgo@hotmail.com",
                         FechaNacimiento = DateTime.Parse("1981-02-02"),
                         Telefono = "3492669439"
                             
                    },
                    new Candidato
                    {
                        Dni = 31333444,
                        Apellido = "PEREZ",
                        Nombre = "LORENZO",
                        Email = "lperez@hotmail.com",
                        FechaNacimiento = DateTime.Parse("1985-12-02"),
                        Telefono = "34926694444"

                    },
                    new Candidato
                    {
                        Dni = 18222444,
                        Apellido = "GOMEZ",
                        Nombre = "JUAN",
                        Email = "lperez@hotmail.com",
                        FechaNacimiento = DateTime.Parse("1985-12-02"),
                        Telefono = "34926694444"

                    },
                    new Candidato
                    {
                        Dni = 15111444,
                        Apellido = "PEREZ",
                        Nombre = "NICOLAS",
                        Email = "lperez@hotmail.com",
                        FechaNacimiento = DateTime.Parse("1985-12-02"),
                        Telefono = "34926694444"

                    }

                );

            modelBuilder.Entity<Empleo>()
                .HasData(
                    new Empleo
                    {
                        Id = 1,
                        NombreEmpresa = "Megatone.Net",
                        Periodo = "2021-02 2021-06",
                        CandidatoId = 28574087

                    },
                    new Empleo
                    {
                        Id = 2,
                        NombreEmpresa = "Megatone.Net",
                        Periodo = "2021-07 2021-12",
                        CandidatoId = 28574087
                    },
                    new Empleo
                    {
                        Id = 3,
                        NombreEmpresa = "Megatone.Net",
                        Periodo = "2022-01 2022-06",
                        CandidatoId = 28574087
                    },
                    new Empleo
                    {
                        Id = 4,
                        NombreEmpresa = "MegaCash",
                        Periodo = "2021-02 2021-06",
                        CandidatoId = 15111444

                    },
                    new Empleo
                    {
                        Id = 5,
                        NombreEmpresa = "Megatone.Net",
                        Periodo = "2021-07 2021-12",
                        CandidatoId = 15111444
                    },
                    new Empleo
                    {
                        Id = 6,
                        NombreEmpresa = "Megatone.Net",
                        Periodo = "2022-01 2022-06",
                        CandidatoId = 18222444
                    }
            );

            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
