using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class ExameViewModel
    {
		public ExameViewModel()
		{
			Paciente = new PessoaModel();
			Usuario = new UsuarioModel();
			Exame = new ExameModel()
			{
				IgM = "N",
				IgG = "N",
				Pcr = "N",
				IgGIgM = "N",
				DataExame = DateTime.Now,
				DataInicioSintomas = DateTime.Now,
				AguardandoResultado = false,
				MetodoExame = "F",
				StatusNotificacao = ExameModel.NOTIFICADO_NAO
			};
			PesquisarCpf = 0;
		}
        public PessoaModel Paciente { get; set; }
		public ExameModel Exame { get; set; }
		public UsuarioModel Usuario { get; set; }
		public EmpresaExameModel EmpresaExame { get; set; }
		public int PesquisarCpf { get; set; }
		
	}
}
