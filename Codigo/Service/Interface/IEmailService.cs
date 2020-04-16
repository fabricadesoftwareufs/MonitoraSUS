using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailDestino, string assunto, string mensagem);
    }
}
