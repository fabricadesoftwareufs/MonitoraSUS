// Mostra modal com mensagem de erro
$(document).ready(function () {
    if (document.querySelector('#mensagem-retorno'))
        document.getElementById("mensagem-retorno").click();
});

// detectando submit via tecla enter
$(window).keydown(function (event) {
    if (event.keyCode == 13) {
        teclaPressionada = event.keyCode;
        event.preventDefault();
        return false;
    }
});

//quando o usuario der submit no exame
$('#btn-submit').on('click', function () {

    // evitando submit equivocado com a tecla enter
    if ($("#input-cpf").val().length > 0) {
        $("#input-cpf").unmask();
    }
    else {
        $("#input-cpf").prop("disabled", true);
        $("#input-cpf").val("");
    }


    $('#modal-confirmar').modal('hide');

    $('#PesquisarCpf').val('0');

    if ($('#input-nome').val() && $('#input-data-nascimento').val().length == 10 && $('#postal_code').val() && $('#route').val() &&
        $('#sublocality_level_1').val() && $('#administrative_area_level_2').val() && $('#administrative_area_level_1').val() &&
        $('#input-celular').val().length == 17 && $('#input-data-exame').val() && $('#input-data-sintomas').val() && $('#input-codigo-coleta').val()) {
        submitForm();
    }

});

// submete o formulário completo
function submitForm() {
    $('#modal-espera').modal('show');
    document.forms["form-exame"].submit();
}
// mostra modal de confirmaocao
function mensagemResultado() {

    var cpf = document.getElementById('input-cpf').value;
    var nome = document.getElementById('input-nome').value;
    var idVirus = document.getElementById('input-virus-bacteria').value;
    var virus = document.getElementById('input-virus-bacteria')[idVirus - 1].text;

    var mensagem = verificaCampoVazio();

    $('#ok-model-form').hide();
    $('#acoes-model-form').hide();

    if (mensagem.length > 0) {
        $('#texto-erro').text(mensagem);
        $('#ok-model-form').show();
    } else {

        $('#cpf-paciente').text(cpf.length == 0 ? 'Não consta' : cpf);
        $('#nome-paciente').text(nome);
        $('#resultado-paciente').text(resultadoExame());
        $('#virus-paciente').text(virus);

        $('#acoes-model-form').show();
    }
    $('#modal-confirmar').modal('show');
}


function verificaCampoVazio() {
    var mensagem = "";
    if ($('#input-nome').val().length == 0)
        mensagem = "Preencha o campo NOME!";
    else if ($('#input-data-nascimento').val().length != 10)
        mensagem = "Preencha A Data De Nascimento corretamente!";
    else if ($('#postal_code').val().length == 0)
        mensagem = "Preencha o campo CEP!";
    else if ($('#route').val().length == 0)
        mensagem = "Preencha o campo RUA!";
    else if ($('#sublocality_level_1').val().length == 0)
        mensagem = "Preencha o campo BAIRRO!";
    else if ($('#administrative_area_level_2').val().length == 0)
        mensagem = "Preencha o campo ESTADO!";
    else if ($('#administrative_area_level_1').val().length == 0)
        mensagem = "Preencha o campo CIDADE!";
    else if ($('#input-celular').val().length != 17)
        mensagem = "Preencha o campo CELELULAR corretamente!";
    else if ($('#input-data-exame').val().length == 0)
        mensagem = "Preencha o campo Data do Exame!";
    else if ($('#input-data-sintomas').val().length == 0)
        mensagem = "Preencha o campo Inicio dos Sintomas!";
    else if ($('#input-codigo-coleta').val().length == 0)
        mensagem = "Preencha o campo Código da Coleta!";


    return mensagem;
}

// Calcula o resultado do exame
function resultadoExame() {
    var igg = $("input[name='IgG']:checked").val();
    var igm = $("input[name='IgM']:checked").val();
    var pcr = $("input[name='Pcr']:checked").val();
    var resultado = "Indetermiando";
    if (pcr === "S" || igm === "S") {
        resultado = "Positivo";
    } else if (pcr === "I" || igm === "I") {
        resultado = "Indeterminado";
    } else if (igg === "S") {
        resultado = "Curado";
    } else if (pcr === "N" || igm === "N") {
        resultado = "Negativo";
    }

    return resultado;
}
