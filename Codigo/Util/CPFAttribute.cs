using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Util
{
	/// <summary>
	/// Validação customizada para CPF
	/// </summary>
	public class CPFAttribute : ValidationAttribute
	{
		/// <summary>
		/// Construtor
		/// </summary>
		public CPFAttribute() { }

		/// <summary>
		/// Validação server
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool IsValid(object value)
		{
			if (value == null || string.IsNullOrEmpty(value.ToString()))
				return true;
			value = Methods.RemoveSpecialsCaracts((string)value);
			bool valido = Methods.ValidarCpf(value.ToString());
			return valido;
		}

		public string GetErrorMessage() =>
			$"CPF Inválido";
	}
}
