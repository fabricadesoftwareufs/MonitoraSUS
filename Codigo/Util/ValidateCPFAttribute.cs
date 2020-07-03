using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Util
{
	/// <summary>
	/// Validação customizada para CPF
	/// </summary>
	public class ValidateCPFAttribute : ValidationAttribute
	{
		/// <summary>
		/// Construtor
		/// </summary>
		public ValidateCPFAttribute() { }

		/// <summary>
		/// Validação server
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool IsValid(object value)
		{
			if (value == null || string.IsNullOrEmpty(value.ToString()))
				return true;

			bool valido = Methods.ValidarCpf(value.ToString());
			return valido;
		}

		public string GetErrorMessage() =>
			$"CPF Inválido";
	}
}
