using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Util
{
	/// <summary>
	/// Validação customizada para CPF
	/// </summary>
	public class TelefoneCelularAttribute : ValidationAttribute
	{
		/// <summary>
		/// Construtor
		/// </summary>
		public TelefoneCelularAttribute() { }

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
			if (value.ToString().Length != 11)
				return false;
			if (value.ToString().StartsWith("0"))
				return false;
			return true;
		}

		public string GetErrorMessage() =>
			$"Celular Inválido";
	}
}
