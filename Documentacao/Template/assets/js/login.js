$(document).ready(function(){
    $('.cpf').mask('000.000.000-00', {reverse: true});

    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: 'CPF ou senha incorretos!',
    })
});