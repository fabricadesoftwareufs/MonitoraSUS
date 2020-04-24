// Mostra modal com mensagem de erro
window.onload = function () {
    $('#modal-mensagem-retorno').modal('show');

    if ($('#input-cpf').val() == "") {
        $('#input-cpf').focus();
    } else if ($('#input-cpf').val() != "" && $('#input-nome').val() != "") {
        window.location.href = "#input-virus-bacteria";
    }
};
// Pegando o cpf enquanto o usuario digita e submetendo quando terminar
var input = document.getElementById('input-cpf');
input.addEventListener("keyup", function () {
    if (document.getElementById('input-cpf').value.length == 14) {
        $('#modal-espera').modal('show');
        document.getElementById('PesquisarCpf').value = 1;
        document.forms["form-exame"].submit();
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
    var dataNasc = document.getElementById('input-data-nascimento').value;
    var cep = document.getElementById('postal_code').value;
    var numero = document.getElementById('street_number').value;
    var rua = document.getElementById('route').value;
    var bairro = document.getElementById('input-bairro').value;
    var cidade = document.getElementById('administrative_area_level_2').value;
    var estado = document.getElementById('administrative_area_level_1').value;
    var foneCelular = document.getElementById('input-celular').value;
    var dataExame = document.getElementById('input-data-exame').value;
    var dataSintomas = document.getElementById('input-data-exame').value;
    var mensagem = "Os demais campos precisam ser preenchidos ";

    $('#ok-model-form').hide();
    $('#acoes-model-form').hide();

    if (cpf === "" || nome === "" || dataNasc === "" || cep === "" ||
        rua === "" || bairro === "" || cidade === "" || estado === "" ||
        foneCelular === "" || dataExame === "" || dataSintomas === "" || numero === "") {
        if (cpf === "")
            mensagem += "- CPF ";
        if (nome === "")
            mensagem += "- NOME ";
        if (dataNasc === "")
            mensagem += "- DATA DE NASCIMENTO ";
        if (cep === "")
            mensagem += "- CEP ";
        if (rua === "")
            mensagem += "- RUA ";
        if (numero === "")
            mensagem += "- NUMERO";
        if (bairro === "")
            mensagem += "- BAIRRO ";
        if (cidade === "")
            mensagem += "- CIDADE ";
        if (estado === "")
            mensagem += "- ESTADO ";
        if (foneCelular === "")
            mensagem += "- CELULAR ";
        if (dataExame === "")
            mensagem += "- DATA DO EXAME ";
        if (dataSintomas === "")
            mensagem += "- DATA DOS SINTOMAS ";
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