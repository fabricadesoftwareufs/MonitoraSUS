$(document).ready(function(){
    var aplication_name='MonitoraSUS';
    var cpf_paciente='123.456.789-90';
    var nome_paciente='Paulo David Almeida da Silva';
    var exame='COVID-19';
    var resultado='Negativo';

    Swal.fire({
        title: 'Deseja concluir esta notificação?',
        text: "Paciente: " + nome_paciente + 
              ". CPF: " + cpf_paciente + 
              ". Exame: " + exame + 
              ". Resultado: " + resultado, 
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#2dce89',
        cancelButtonColor: '#f5365c',
        confirmButtonText: 'Sim, Notificar!',
        cancelButtonText: 'Não, Cancelar!'
      }).then((result) => {
        if (result.value) {
          Swal.fire(
            'Notificação realizada com SUCESSO!',
            'Foi enviada uma mensagem por email/SMS/WhatsApp cadastrado para notificação.',
            'success'
          )
        }
      })
});