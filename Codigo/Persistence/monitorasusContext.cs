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

        public virtual DbSet<Areaatuacao> Areaatuacao { get; set; }
        public virtual DbSet<Configuracaonotificar> Configuracaonotificar { get; set; }
        public virtual DbSet<Empresaexame> Empresaexame { get; set; }
        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Exame> Exame { get; set; }
        public virtual DbSet<Internacao> Internacao { get; set; }
        public virtual DbSet<Municipio> Municipio { get; set; }
        public virtual DbSet<Municipiosgeo> Municipiosgeo { get; set; }
        public virtual DbSet<Pessoa> Pessoa { get; set; }
        public virtual DbSet<Pessoatrabalhaestado> Pessoatrabalhaestado { get; set; }
        public virtual DbSet<Pessoatrabalhamunicipio> Pessoatrabalhamunicipio { get; set; }
        public virtual DbSet<Recuperarsenha> Recuperarsenha { get; set; }
        public virtual DbSet<Situacaopessoavirusbacteria> Situacaopessoavirusbacteria { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Virusbacteria> Virusbacteria { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Areaatuacao>(entity =>
            {
                entity.HasKey(e => e.IdAreaAtuacao);

                entity.ToTable("areaatuacao", "monitorasus");

                entity.HasIndex(e => e.IdAreaAtuacao)
                    .HasName("idProfissao_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdAreaAtuacao)
                    .HasColumnName("idAreaAtuacao")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Configuracaonotificar>(entity =>
            {
                entity.HasKey(e => e.IdConfiguracaoNotificar);

                entity.ToTable("configuracaonotificar", "monitorasus");

                entity.HasIndex(e => e.IdEmpresaExame)
                    .HasName("fk_configuracaoNotificar_empresaexame1_idx");

                entity.HasIndex(e => e.IdEstado)
                    .HasName("fk_configuracaoNotificar_estado1_idx");

                entity.HasIndex(e => e.IdMunicipio)
                    .HasName("fk_configuracaoNotificar_municipio1_idx");

                entity.Property(e => e.IdConfiguracaoNotificar)
                    .HasColumnName("idConfiguracaoNotificar")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HabilitadoSms)
                    .HasColumnName("habilitadoSMS")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.HabilitadoWhatsapp)
                    .HasColumnName("habilitadoWhatsapp")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IdEmpresaExame)
                    .HasColumnName("idEmpresaExame")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdEstado)
                    .HasColumnName("idEstado")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdMunicipio)
                    .HasColumnName("idMunicipio")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MensagemImunizado)
                    .IsRequired()
                    .HasColumnName("mensagemImunizado")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.MensagemIndeterminado)
                    .IsRequired()
                    .HasColumnName("mensagemIndeterminado")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.MensagemNegativo)
                    .IsRequired()
                    .HasColumnName("mensagemNegativo")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.MensagemPositivo)
                    .IsRequired()
                    .HasColumnName("mensagemPositivo")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.QuantidadeSmsdisponivel)
                    .HasColumnName("quantidadeSMSDisponivel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Sid)
                    .IsRequired()
                    .HasColumnName("sid")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEmpresaExameNavigation)
                    .WithMany(p => p.Configuracaonotificar)
                    .HasForeignKey(d => d.IdEmpresaExame)
                    .HasConstraintName("fk_configuracaoNotificar_empresaexame1");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Configuracaonotificar)
                    .HasForeignKey(d => d.IdEstado)
                    .HasConstraintName("fk_configuracaoNotificar_estado1");

                entity.HasOne(d => d.IdMunicipioNavigation)
                    .WithMany(p => p.Configuracaonotificar)
                    .HasForeignKey(d => d.IdMunicipio)
                    .HasConstraintName("fk_configuracaoNotificar_municipio1");
            });

            modelBuilder.Entity<Empresaexame>(entity =>
            {
                entity.ToTable("empresaexame", "monitorasus");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bairro)
                    .IsRequired()
                    .HasColumnName("bairro")
                    .HasMaxLength(60)
                    .IsUnicode(false);

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

                entity.Property(e => e.Cnes)
                    .IsRequired()
                    .HasColumnName("cnes")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Cnpj)
                    .IsRequired()
                    .HasColumnName("cnpj")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.Complemento)
                    .HasColumnName("complemento")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EhPublico)
                    .HasColumnName("ehPublico")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.EmiteLaudoExame)
                    .HasColumnName("emiteLaudoExame")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FazMonitoramento)
                    .HasColumnName("fazMonitoramento")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.FoneCelular)
                    .IsRequired()
                    .HasColumnName("foneCelular")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FoneFixo)
                    .HasColumnName("foneFixo")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnName("latitude")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnName("longitude")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Numero)
                    .HasColumnName("numero")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NumeroLeitos)
                    .HasColumnName("numeroLeitos")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.NumeroLeitosDisponivel)
                    .HasColumnName("numeroLeitosDisponivel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.NumeroLeitosUti)
                    .HasColumnName("numeroLeitosUTI")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.NumeroLeitosUtidisponivel)
                    .HasColumnName("numeroLeitosUTIDisponivel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.PossuiLeitosInternacao)
                    .HasColumnName("possuiLeitosInternacao")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasColumnName("rua")
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

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

                entity.HasIndex(e => e.IdAgenteSaude)
                    .HasName("fk_exame_pessoa2_idx");

                entity.HasIndex(e => e.IdAreaAtuacao)
                    .HasName("fk_exame_AreaAtuacao1_idx");

                entity.HasIndex(e => e.IdEmpresaSaude)
                    .HasName("fk_exame_empresasaude1_idx");

                entity.HasIndex(e => e.IdEstado)
                    .HasName("fk_exame_estado1_idx");

                entity.HasIndex(e => e.IdMunicipio)
                    .HasName("fk_exame_municipio1_idx");

                entity.HasIndex(e => e.IdPaciente)
                    .HasName("fk_exame_pessoa1_idx");

                entity.HasIndex(e => e.IdVirusBacteria)
                    .HasName("fk_exame_virusBacteria1_idx");

                entity.HasIndex(e => new { e.IdVirusBacteria, e.IdPaciente, e.DataExame })
                    .HasName("fk_exame_diaPessoa")
                    .IsUnique();

                entity.Property(e => e.IdExame)
                    .HasColumnName("idExame")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AguardandoResultado)
                    .HasColumnName("aguardandoResultado")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CodigoColeta)
                    .IsRequired()
                    .HasColumnName("codigoColeta")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Coriza)
                    .HasColumnName("coriza")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DataExame).HasColumnName("dataExame");

                entity.Property(e => e.DataInicioSintomas).HasColumnName("dataInicioSintomas");

                entity.Property(e => e.DataNotificacao)
                    .HasColumnName("dataNotificacao")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Diarreia)
                    .HasColumnName("diarreia")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DificuldadeRespiratoria)
                    .HasColumnName("dificuldadeRespiratoria")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DorAbdominal)
                    .HasColumnName("dorAbdominal")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DorGarganta)
                    .HasColumnName("dorGarganta")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DorOuvido)
                    .HasColumnName("dorOuvido")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Febre)
                    .HasColumnName("febre")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IdAgenteSaude)
                    .HasColumnName("idAgenteSaude")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdAreaAtuacao)
                    .HasColumnName("idAreaAtuacao")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IdEmpresaSaude)
                    .HasColumnName("idEmpresaSaude")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdEstado)
                    .HasColumnName("idEstado")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdMunicipio)
                    .HasColumnName("idMunicipio")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdNotificacao)
                    .IsRequired()
                    .HasColumnName("idNotificacao")
                    .HasMaxLength(20)
                    .IsUnicode(false);

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

                entity.Property(e => e.IgMigG)
                    .IsRequired()
                    .HasColumnName("igMigG")
                    .HasColumnType("enum('S','N','I')")
                    .HasDefaultValueSql("N");

                entity.Property(e => e.MetodoExame)
                    .IsRequired()
                    .HasColumnName("metodoExame")
                    .HasColumnType("enum('C','F','P')")
                    .HasDefaultValueSql("F");

                entity.Property(e => e.Nausea)
                    .HasColumnName("nausea")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.OutroSintomas)
                    .HasColumnName("outroSintomas")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Pcr)
                    .IsRequired()
                    .HasColumnName("pcr")
                    .HasColumnType("enum('S','N','I')")
                    .HasDefaultValueSql("N");

                entity.Property(e => e.PerdaOlfatoPaladar)
                    .HasColumnName("perdaOlfatoPaladar")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.RelatouSintomas)
                    .HasColumnName("relatouSintomas")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.StatusNotificacao)
                    .IsRequired()
                    .HasColumnName("statusNotificacao")
                    .HasColumnType("enum('N','S','E','P')")
                    .HasDefaultValueSql("N");

                entity.Property(e => e.Tosse)
                    .HasColumnName("tosse")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.IdAgenteSaudeNavigation)
                    .WithMany(p => p.ExameIdAgenteSaudeNavigation)
                    .HasForeignKey(d => d.IdAgenteSaude)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_pessoa2");

                entity.HasOne(d => d.IdAreaAtuacaoNavigation)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.IdAreaAtuacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_AreaAtuacao1");

                entity.HasOne(d => d.IdEmpresaSaudeNavigation)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.IdEmpresaSaude)
                    .HasConstraintName("fk_exame_empresasaude1");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_exame_estado1");

                entity.HasOne(d => d.IdMunicipioNavigation)
                    .WithMany(p => p.Exame)
                    .HasForeignKey(d => d.IdMunicipio)
                    .HasConstraintName("fk_exame_municipio1");

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
            });

            modelBuilder.Entity<Internacao>(entity =>
            {
                entity.HasKey(e => e.IdInternacao);

                entity.ToTable("internacao", "monitorasus");

                entity.HasIndex(e => e.IdEmpresa)
                    .HasName("fk_pessoa_has_empresaexame_empresaexame1_idx");

                entity.HasIndex(e => e.Idpessoa)
                    .HasName("fk_pessoa_has_empresaexame_pessoa1_idx");

                entity.Property(e => e.IdInternacao)
                    .HasColumnName("idInternacao")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DataFim).HasColumnName("dataFim");

                entity.Property(e => e.DataInicio).HasColumnName("dataInicio");

                entity.Property(e => e.IdEmpresa)
                    .HasColumnName("idEmpresa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Idpessoa)
                    .HasColumnName("idpessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UsoO2)
                    .IsRequired()
                    .HasColumnName("usoO2")
                    .HasColumnType("enum('V','C','M','A')")
                    .HasDefaultValueSql("A");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Internacao)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_empresaexame_empresaexame1");

                entity.HasOne(d => d.IdpessoaNavigation)
                    .WithMany(p => p.Internacao)
                    .HasForeignKey(d => d.Idpessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_empresaexame_pessoa1");
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

            modelBuilder.Entity<Municipiosgeo>(entity =>
            {
                entity.HasKey(e => e.CodigoIbge);

                entity.ToTable("municipiosgeo", "monitorasus");

                entity.Property(e => e.CodigoIbge)
                    .HasColumnName("codigo_ibge")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Capital)
                    .HasColumnName("capital")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.CodigoUf)
                    .HasColumnName("codigo_uf")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.Idpessoa);

                entity.ToTable("pessoa", "monitorasus");

                entity.HasIndex(e => e.Cpf)
                    .HasName("cpf_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.IdAreaAtuacao)
                    .HasName("fk_pessoa_AreaAtuacao1_idx");

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

                entity.Property(e => e.Cns)
                    .HasColumnName("cns")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Complemento)
                    .HasColumnName("complemento")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Coriza)
                    .HasColumnName("coriza")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasColumnName("cpf")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.DataNascimento)
                    .HasColumnName("dataNascimento")
                    .HasColumnType("date");

                entity.Property(e => e.DataObito).HasColumnName("dataObito");

                entity.Property(e => e.Diabetes)
                    .HasColumnName("diabetes")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Diarreia)
                    .HasColumnName("diarreia")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DificuldadeRespiratoria)
                    .HasColumnName("dificuldadeRespiratoria")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DoencaRenal)
                    .HasColumnName("doencaRenal")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DoencaRespiratoria)
                    .HasColumnName("doencaRespiratoria")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DorAbdominal)
                    .HasColumnName("dorAbdominal")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DorGarganta)
                    .HasColumnName("dorGarganta")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DorOuvido)
                    .HasColumnName("dorOuvido")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Epilepsia)
                    .HasColumnName("epilepsia")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Febre)
                    .HasColumnName("febre")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

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

                entity.Property(e => e.IdAreaAtuacao)
                    .HasColumnName("idAreaAtuacao")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Imunodeprimido)
                    .HasColumnName("imunodeprimido")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnName("latitude")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnName("longitude")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Nausea)
                    .HasColumnName("nausea")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

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

                entity.Property(e => e.OutrasComorbidades)
                    .HasColumnName("outrasComorbidades")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OutrosSintomas)
                    .HasColumnName("outrosSintomas")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PerdaOlfatoPaladar)
                    .HasColumnName("perdaOlfatoPaladar")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Profissao)
                    .IsRequired()
                    .HasColumnName("profissao")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("Não Informada");

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

                entity.Property(e => e.SituacaoSaude)
                    .IsRequired()
                    .HasColumnName("situacaoSaude")
                    .HasColumnType("enum('S','I','H','U','E','O')")
                    .HasDefaultValueSql("S");

                entity.Property(e => e.Tosse)
                    .HasColumnName("tosse")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.IdAreaAtuacaoNavigation)
                    .WithMany(p => p.Pessoa)
                    .HasForeignKey(d => d.IdAreaAtuacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_AreaAtuacao1");
            });

            modelBuilder.Entity<Pessoatrabalhaestado>(entity =>
            {
                entity.HasKey(e => new { e.Idpessoa, e.IdEstado, e.IdEmpresaExame });

                entity.ToTable("pessoatrabalhaestado", "monitorasus");

                entity.HasIndex(e => e.IdEmpresaExame)
                    .HasName("fk_pessoatrabalhaestado_empresaexame1_idx");

                entity.HasIndex(e => e.IdEstado)
                    .HasName("fk_pessoa_has_estado_estado1_idx");

                entity.HasIndex(e => e.Idpessoa)
                    .HasName("fk_pessoa_has_estado_pessoa1_idx");

                entity.Property(e => e.Idpessoa)
                    .HasColumnName("idpessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdEstado)
                    .HasColumnName("idEstado")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdEmpresaExame)
                    .HasColumnName("idEmpresaExame")
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

                entity.HasOne(d => d.IdEmpresaExameNavigation)
                    .WithMany(p => p.Pessoatrabalhaestado)
                    .HasForeignKey(d => d.IdEmpresaExame)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoatrabalhaestado_empresaexame1");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Pessoatrabalhaestado)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_pessoa_has_estado_estado1");

                entity.HasOne(d => d.IdpessoaNavigation)
                    .WithMany(p => p.Pessoatrabalhaestado)
                    .HasForeignKey(d => d.Idpessoa)
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

            modelBuilder.Entity<Recuperarsenha>(entity =>
            {
                entity.ToTable("recuperarsenha", "monitorasus");

                entity.HasIndex(e => e.IdUsuario)
                    .HasName("fk_recuperarsenha_usuario1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.EhValido)
                    .HasColumnName("ehValido")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.FimToken).HasColumnName("fimToken");

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("idUsuario")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InicioToken).HasColumnName("inicioToken");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Recuperarsenha)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_recuperarsenha_usuario1");
            });

            modelBuilder.Entity<Situacaopessoavirusbacteria>(entity =>
            {
                entity.HasKey(e => new { e.IdVirusBacteria, e.Idpessoa });

                entity.ToTable("situacaopessoavirusbacteria", "monitorasus");

                entity.HasIndex(e => e.IdGestor)
                    .HasName("fk_situacaopessoavirusbacteria_pessoa1_idx");

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

                entity.Property(e => e.DataUltimoMonitoramento)
                    .HasColumnName("dataUltimoMonitoramento")
                    .HasColumnType("date");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.IdGestor)
                    .HasColumnName("idGestor")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdGestorNavigation)
                    .WithMany(p => p.SituacaopessoavirusbacteriaIdGestorNavigation)
                    .HasForeignKey(d => d.IdGestor)
                    .HasConstraintName("fk_situacaopessoavirusbacteria_pessoa1");

                entity.HasOne(d => d.IdVirusBacteriaNavigation)
                    .WithMany(p => p.Situacaopessoavirusbacteria)
                    .HasForeignKey(d => d.IdVirusBacteria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_virusBacteria_has_pessoa_virusBacteria1");

                entity.HasOne(d => d.IdpessoaNavigation)
                    .WithMany(p => p.SituacaopessoavirusbacteriaIdpessoaNavigation)
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
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.IdPessoa)
                    .HasColumnName("idPessoa")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Senha)
                    .IsRequired()
                    .HasColumnName("senha")
                    .HasMaxLength(100)
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

                entity.Property(e => e.DiasRecuperacao)
                    .HasColumnName("diasRecuperacao")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });
        }
    }
}
