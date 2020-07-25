// Mostra modal com mensagem de erro
$(document).ready(function () {
    if (document.querySelector('#mensagem-retorno'))
        document.getElementById("mensagem-retorno").click();

    ocultaViewTipoExame();
    ocultaViewSintomas();
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
        $('#input-celular').val().length == 17 && $('#input-data-exame').val() && $('#input-data-sintomas').val()) {
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

    var IgG = $("input[name='Exame.IgG']:checked").val();
    var IgM = $("input[name='Exame.IgM']:checked").val();
    var IgGIgM = $("input[name='Exame.IgGIgM']:checked").val();
    var Pcr = $("input[name='Exame.Pcr']:checked").val();
    var aguardando = $("input[name='Exame.AguardandoResultado']:checked").val();

    var cpf = document.getElementById('input-cpf').value;
    var nome = document.getElementById('input-nome').value;
    var idVirus = document.getElementById('input-virus-bacteria').value;
    var virus = document.getElementById('input-virus-bacteria')[idVirus - 1].text;
    var mensagem = ""; //verificaCampoVazio();

    $('#ok-model-form').hide();
    $('#acoes-model-form').hide();

    if (mensagem.length > 0) {
        $('#texto-erro').text(mensagem);
        $('#ok-model-form').show();
    } else {

        $('#cpf-paciente').text(cpf.length == 0 ? 'Não consta' : cpf);
        $('#nome-paciente').text(nome);
        $('#virus-paciente').text(virus);

        $.post("/Exame/CalculaResultadoExame", { aguardandoResultado: aguardando, iggIgm: IgGIgM, igm: IgM, igg: IgG, pcr: Pcr }, function (data) {
            if (data.length > 0)
                $('#resultado-paciente').text(data);
            else
                $('#resultado-paciente').text("Indeterminado");
        });

        $('#acoes-model-form').show();
    }
    $('#modal-confirmar').modal('show');
}


function verificaCampoVazio() {
    var mensagem = "";
    if ($('#input-nome').val().length == 0)
        mensagem = "Preencha o campo NOME!";
    else if ($('#input-data-nascimento').val().length != 10)
        mensagem = "Preencha a DATA DE NASCIMENTO corretamente!";
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
        mensagem = "Preencha o campo CELULAR corretamente!";
    else if ($('#input-data-exame').val().length == 0)
        mensagem = "Preencha o campo DATA DO EXAME!";
    else if ($('#input-data-sintomas').val().length == 0)
        mensagem = "Preencha o campo INICIO DOS SINTOMAS!";
    //else if ($('#input-codigo-coleta').val().length == 0)
    //    mensagem = "Preencha o campo CÓDIGO DA COLETA!";


    return mensagem;
}

function ocultaViewTipoExame() {
    var metodo = $("input[name='Exame.MetodoExame']:checked").val();

    switch (metodo) {

        case 'P':
            document.getElementById("div-igm").hidden = true;
            document.getElementById("div-igg").hidden = true;
            document.getElementById("div-iggigm").hidden = true;
            document.getElementById("div-pcr").hidden = false;
            break;
        case 'F':
            document.getElementById("div-igm").hidden = false;
            document.getElementById("div-igg").hidden = false;
            document.getElementById("div-pcr").hidden = true;
            document.getElementById("div-iggigm").hidden = true;
            break;
        case 'C':
            document.getElementById("div-igm").hidden = false;
            document.getElementById("div-igg").hidden = false;
            document.getElementById("div-iggigm").hidden = false;
            document.getElementById("div-pcr").hidden = true;
            break;
    }
}


function ocultaViewSintomas() {
    var relatouSintomas = $("input[name='Exame.RelatouSintomas']:checked").val();

    if (relatouSintomas == 'True') {
        document.getElementById('container-sintomas').hidden = false;
        // document.getElementById('contaner-inicio-sintomas').hidden = false;
    } else if (relatouSintomas == 'False') {
        document.getElementById('container-sintomas').hidden = true;
        //document.getElementById('contaner-inicio-sintomas').hidden = true;
    }
}
