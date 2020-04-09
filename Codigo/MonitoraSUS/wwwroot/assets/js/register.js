$(document).ready(function(){
    var aplication_name='MonitoraSUS';

    Swal.fire({
        icon: 'success',
        title: 'Solicitação Realizada com SUCESSO!',
        text: 'Por favor, aguarde a notificação de aprovação pelo Administrador do ' + aplication_name + 
              '. Após a aprovação, sua senha de acesso será enviada por email, SMS e WhatsApp cadastrado.' ,
    })
});