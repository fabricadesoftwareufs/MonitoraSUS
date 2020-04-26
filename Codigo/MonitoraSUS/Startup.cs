using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
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

            services.AddDbContext<monitorasusContext>(options =>
                options.UseMySQL(
                    Configuration.GetConnectionString("MySqlConnection")));

            services.AddScoped<IVirusBacteriaService, VirusBacteriaService>();
            services.AddScoped<IExameService, ExameService>();
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IEstadoService, EstadoService>();
            services.AddScoped<ISituacaoVirusBacteriaService, SituacaoVirusBacteriaService>();
            services.AddScoped<IMunicipioService, MunicipioService>();
            services.AddScoped<IPessoaTrabalhaMunicipioService, PessoaTrabalhaMunicipioService>();
            services.AddScoped<IPessoaTrabalhaEstadoService, PessoaTrabalhaEstadoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IRecuperarSenhaService, RecuperarSenhaService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmpresaExameService, EmpresaExameService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
