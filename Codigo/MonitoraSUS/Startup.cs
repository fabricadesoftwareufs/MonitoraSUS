using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Repository;
using Repository.Interfaces;
using Service;
using Service.Interface;

namespace MonitoraSUS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Login";
            options.AccessDeniedPath = "/Login/AcessDenied";

        });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 52428800;
            });

            services.AddDbContext<monitorasusContext>(options =>
                options.UseMySQL(
                    Configuration.GetConnectionString("MySqlConnection")));

            // Serviços
            services.AddSingleton<IVirusBacteriaService, VirusBacteriaService>();
            services.AddSingleton<IExameService, ExameService>();
            services.AddSingleton<IPessoaService, PessoaService>();
            services.AddSingleton<IEstadoService, EstadoService>();
            services.AddSingleton<ISituacaoVirusBacteriaService, SituacaoVirusBacteriaService>();
            services.AddSingleton<IMunicipioService, MunicipioService>();
            services.AddSingleton<IPessoaTrabalhaMunicipioService, PessoaTrabalhaMunicipioService>();
            services.AddSingleton<IPessoaTrabalhaEstadoService, PessoaTrabalhaEstadoService>();
            services.AddSingleton<IUsuarioService, UsuarioService>();
            services.AddSingleton<IRecuperarSenhaService, RecuperarSenhaService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IEmpresaExameService, EmpresaExameService>();
            services.AddSingleton<IInternacaoService, InternacaoService>();
			services.AddSingleton<IAreaAtuacaoService, AreaAtuacaoService>();
            services.AddSingleton<IMunicipioGeoService, MunicipioGeoService>();

            // Repositorios
            services.AddSingleton<IExameRepository, ExameRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Culture = new System.Globalization.CultureInfo("pt-BR");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseExceptionHandler("/Error/500");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCookiePolicy();
        }
    }
}
