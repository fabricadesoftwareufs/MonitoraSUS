// Mostra modal com mensagem de erro
$(document).ready(function () {
    if ($('#input-cpf').val() == "") {
        $('#input-cpf').focus();
    } else if ($('#input-cpf').val() != "" && $('#input-nome').val() != "") {
        window.location.href = "#input-virus-bacteria";
    }

    document.getElementById("mensagem-retorno").click();
});

// Pegando o cpf enquanto o usuario digita e submetendo quando terminar
var input = document.getElementById('input-cpf');
input.addEventListener("keyup", function () {
    if (document.getElementById('input-cpf').value.length == 14) {
        $('#modal-espera').modal('show');
        document.getElementById('PesquisarCpf').value = 1;
        document.forms["form-exame"].submit();
    }
});

//quando o usuario der submit no exame
$('#btn-submit').on('click', function () {

    $('#modal-confirmar').modal('hide');

    var cpf = $('#input-cpf').val();
    var nome = $('#input-nome').val();
    var dataNasc = $('#input-data-nascimento').val();
    var cep = $('#postal_code').val();
    var rua = $('#route').val();
    var bairro = $('#sublocality_level_1').val();
    var cidade = $('#administrative_area_level_2').val();
    var estado = $('#administrative_area_level_1').val();
    var foneCelular = $('#input-celular').val();
    var dataExame = $('#input-data-exame').val();
    var dataSintomas = $('#input-data-sintomas').val();
    var outrasComorbidades = $('#input-outrasComorbidades').val();

    if (!(cpf === "" || nome === "" || dataNasc === "" || cep === "" || rua === "" || bairro === "" ||
        cidade === "" || estado === "" || foneCelular === "" || outrasComorbidades === "" || dataExame === "" || dataSintomas === "")) {
        $('#modal-espera').modal('show');
    }

});

// submete o formulário completo
function submitForm() {
    $('#modal-espera').modal('show');
    document.forms["form-exame"].submit();
}
// mostra modal de confirmaocao
function mensagemResultado() {
    var mensagem = verificaCampoRequired();
    $('#texto-confirmacao').text(mensagem);
    $('#modal-confirmar').modal('show');
}
// verifica se todos os campos estão preenchidos (diretiva required parou de funcionar)
function verificaCampoRequired() {
    var cpf = document.getElementById('input-cpf').value;
    var nome = document.getElementById('input-nome').value;

    var mensagem = "";

    $('#ok-model-form').hide();
    $('#acoes-model-form').hide();

    if (cpf === "" || nome === "") {
        mensagem = "Nenhum paciente foi informado.";
        $('#ok-model-form').show();
    } else {
        var mensagem = "Cpf: " + cpf + "\n" +
            "Paciente: " + nome + "\n" +
            "Resultado: " + resultadoExame();
        $('#acoes-model-form').show();
    }
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

function swtAlert(type, title, message) {
    Swal.fire({
        icon: type,
        title: title,
        text: message,
    });
}

function swtAlertRedirectIndex(type, title, message, url) {
    Swal.fire({
        icon: type,
        title: title,
        text: message,
        footer: 
            '<a href="'+url+'" class="btn btn-success">OK</a>',
        showConfirmButton: false,
    });
}

