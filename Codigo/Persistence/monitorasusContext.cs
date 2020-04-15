using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Persistence
{
    public partial class monitorasusContext : DbContext
    {
        public monitorasusContext()
        {
        }

        public monitorasusContext(DbContextOptions<monitorasusContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Exame> Exame { get; set; }
        public virtual DbSet<Municipio> Municipio { get; set; }
        public virtual DbSet<Pessoa> Pessoa { get; set; }
        public virtual DbSet<Pessoatrabalhaestado> Pessoatrabalhaestado { get; set; }
        public virtual DbSet<Pessoatrabalhamunicipio> Pessoatrabalhamunicipio { get; set; }
        public virtual DbSet<Situacaopessoavirusbacteria> Situacaopessoavirusbacteria { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Virusbacteria> Virusbacteria { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("estado", "monitorasus");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CodigoUf).HasColumnType("int(11)");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Regiao).HasColumnType("int(11)");

                entity.Property(e => e.Uf)
                    .IsRequired()
                    .HasColumnType("char(2)");
            });

            modelBuilder.Entity<Exame>(entity =>
            {
                entity.HasKey(e => e.IdExame);

                entity.ToTable("exame", "monitorasus");

                entity.HasIndex(e => e.EstadoRealizacao)
                    .HasName("fk_exame_estado1_idx");

                entity.HasIndex(e => e.IdAgenteSaude)
                    .HasName("fk_exame_pessoa2_idx");

                entity.HasIndex(e => e.IdPaciente)
                    .HasName("fk_exame_pessoa1_idx");

                entity.HasIndex(e => e.IdVirusBacteria)
                    .HasName("fk_exame_virusBacteria1_idx");

                entity.HasIndex(e => e.MunicipioId)
                    .HasName("fk_exame_municipio1_idx");

                entity.Property(e => e.IdExame)
                    .HasColumnName("idExame")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DataExame)
                    .HasColumnName("dataExame")
                    .HasColumnType("date");

                entity.Property(e => e.DataInicioSintomas)
                    .HasColumnName("dataInicioSintomas")
                    .HasColumnType("date");

                entity.Property(e => e.EstadoRealizacao)
                    .HasColumnName("estadoRealizacao")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdAgenteSaude)
                    .HasColumnName("idAgenteSaude")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdPaciente)
                    .HasColumnName("idPaciente")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdVirusBacteria)
                    .HasColumnName("idVirusBacteria")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IgG)
                    .IsRequired()
                    .HasColumnName("igG")
                    .HasColumnType("enum('S','N','I')")
                    .HasDefaultValueSql("N");

                entity.Property(e => e.IgM)
                    .IsRequired()
                    .HasColumnName("igM")
                    .HasColumnType("enum('S','N','I')")
                    .HasDefaultValueSql("N");

                entity.Property(e => e.MunicipioId)
                    .HasColumnName("municipio_Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Pcr)
                    .IsRequired()
                    .HasColumnName("pcr")
                    .HasColumnType("enum('S','N','I')")
                    .HasDefaultValueSql("N");

                entity.HasOne(d => d.EstadoRealizacaoNavigation)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.EstadoRealizacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_estado1");

                entity.HasOne(d => d.IdAgenteSaudeNavigation)
                    .WithMany(p => p.ExameIdAgenteSaudeNavigation)
                    .HasForeignKey(d => d.IdAgenteSaude)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_pessoa2");

                entity.HasOne(d => d.IdPacienteNavigation)
                    .WithMany(p => p.ExameIdPacienteNavigation)
                    .HasForeignKey(d => d.IdPaciente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_pessoa1");

                entity.HasOne(d => d.IdVirusBacteriaNavigation)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.IdVirusBacteria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_virusBacteria1");

                entity.HasOne(d => d.Municipio)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.MunicipioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_municipio1");
            });

            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.ToTable("municipio", "monitorasus");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Codigo).HasColumnType("int(11)");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Uf)
                    .IsRequired()
                    .HasColumnType("char(2)");
            });

            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.Idpessoa);

                entity.ToTable("pessoa", "monitorasus");

                entity.Property(e => e.Idpessoa)
                    .HasColumnName("idpessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bairro)
                    .IsRequired()
                    .HasColumnName("bairro")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Cancer)
                    .HasColumnName("cancer")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Cardiopatia)
                    .HasColumnName("cardiopatia")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Cep)
                    .IsRequired()
                    .HasColumnName("cep")
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Cidade)
                    .IsRequired()
                    .HasColumnName("cidade")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Complemento)
                    .HasColumnName("complemento")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasColumnName("cpf")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.DataNascimento)
                    .HasColumnName("dataNascimento")
                    .HasColumnType("date");

                entity.Property(e => e.Diabetes)
                    .HasColumnName("diabetes")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DoencaRespiratoria)
                    .HasColumnName("doencaRespiratoria")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FoneCelular)
                    .IsRequired()
                    .HasColumnName("foneCelular")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FoneFixo)
                    .HasColumnName("foneFixo")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Hipertenso)
                    .HasColumnName("hipertenso")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Imunodeprimido)
                    .HasColumnName("imunodeprimido")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Numero)
                    .HasColumnName("numero")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Obeso)
                    .HasColumnName("obeso")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasColumnName("rua")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Sexo)
                    .IsRequired()
                    .HasColumnName("sexo")
                    .HasColumnType("enum('M','F')")
                    .HasDefaultValueSql("M");
            });

            modelBuilder.Entity<Pessoatrabalhaestado>(entity =>
            {
                entity.HasKey(e => new { e.IdPessoa, e.IdEstado });

                entity.ToTable("pessoatrabalhaestado", "monitorasus");

                entity.HasIndex(e => e.IdEstado)
                    .HasName("fk_pessoa_has_estado_estado1_idx");

                entity.HasIndex(e => e.IdPessoa)
                    .HasName("fk_pessoa_has_estado_pessoa1_idx");

                entity.Property(e => e.IdPessoa)
                    .HasColumnName("idPessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdEstado)
                    .HasColumnName("idEstado")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EhResponsavel)
                    .HasColumnName("ehResponsavel")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.EhSecretario)
                    .HasColumnName("ehSecretario")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SituacaoCadastro)
                    .IsRequired()
                    .HasColumnName("situacaoCadastro")
                    .HasColumnType("enum('S','A','I')")
                    .HasDefaultValueSql("S");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Pessoatrabalhaestado)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_estado_estado1");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Pessoatrabalhaestado)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_estado_pessoa1");
            });

            modelBuilder.Entity<Pessoatrabalhamunicipio>(entity =>
            {
                entity.HasKey(e => new { e.IdPessoa, e.IdMunicipio });

                entity.ToTable("pessoatrabalhamunicipio", "monitorasus");

                entity.HasIndex(e => e.IdMunicipio)
                    .HasName("fk_pessoa_has_municipio_municipio1_idx");

                entity.HasIndex(e => e.IdPessoa)
                    .HasName("fk_pessoa_has_municipio_pessoa1_idx");

                entity.Property(e => e.IdPessoa)
                    .HasColumnName("idPessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdMunicipio)
                    .HasColumnName("idMunicipio")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EhResponsavel)
                    .HasColumnName("ehResponsavel")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.EhSecretario)
                    .HasColumnName("ehSecretario")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SituacaoCadastro)
                    .IsRequired()
                    .HasColumnName("situacaoCadastro")
                    .HasColumnType("enum('S','A','I')")
                    .HasDefaultValueSql("S");

                entity.HasOne(d => d.IdMunicipioNavigation)
                    .WithMany(p => p.Pessoatrabalhamunicipio)
                    .HasForeignKey(d => d.IdMunicipio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_municipio_municipio1");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Pessoatrabalhamunicipio)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_municipio_pessoa1");
            });

            modelBuilder.Entity<Situacaopessoavirusbacteria>(entity =>
            {
                entity.HasKey(e => new { e.IdVirusBacteria, e.Idpessoa });

                entity.ToTable("situacaopessoavirusbacteria", "monitorasus");

                entity.HasIndex(e => e.IdVirusBacteria)
                    .HasName("fk_virusBacteria_has_pessoa_virusBacteria1_idx");

                entity.HasIndex(e => e.Idpessoa)
                    .HasName("fk_virusBacteria_has_pessoa_pessoa1_idx");

                entity.Property(e => e.IdVirusBacteria)
                    .HasColumnName("idVirusBacteria")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idpessoa)
                    .HasColumnName("idpessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UltimaSituacaoSaude)
                    .IsRequired()
                    .HasColumnName("ultimaSituacaoSaude")
                    .HasColumnType("enum('P','N','A','I','C')")
                    .HasDefaultValueSql("N");

                entity.HasOne(d => d.IdVirusBacteriaNavigation)
                    .WithMany(p => p.Situacaopessoavirusbacteria)
                    .HasForeignKey(d => d.IdVirusBacteria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_virusBacteria_has_pessoa_virusBacteria1");

                entity.HasOne(d => d.IdpessoaNavigation)
                    .WithMany(p => p.Situacaopessoavirusbacteria)
                    .HasForeignKey(d => d.Idpessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_virusBacteria_has_pessoa_pessoa1");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.ToTable("usuario", "monitorasus");

                entity.HasIndex(e => e.IdPessoa)
                    .HasName("fk_usuario_pessoa1_idx");

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("idUsuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasColumnName("cpf")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.IdPessoa)
                    .HasColumnName("idPessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Senha)
                    .IsRequired()
                    .HasColumnName("senha")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.TipoUsuario)
                    .HasColumnName("tipoUsuario")
                    .HasColumnType("tinyint(4)");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Usuario)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_usuario_pessoa1");
            });

            modelBuilder.Entity<Virusbacteria>(entity =>
            {
                entity.HasKey(e => e.IdVirusBacteria);

                entity.ToTable("virusbacteria", "monitorasus");

                entity.Property(e => e.IdVirusBacteria)
                    .HasColumnName("idVirusBacteria")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });
        }
    }
}
