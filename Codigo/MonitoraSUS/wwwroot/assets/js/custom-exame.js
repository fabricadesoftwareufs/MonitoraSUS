// Mostra modal com mensagem de erro
$(document).ready(function () {

    let identificador = document.getElementById('input-cpf').value;
    identificador = identificador.replace("-", "").replace(".", "").replace(".", "");
    if ($.isNumeric(identificador)) {
        $('#input-cpf').mask('AAA.AAA.AAA-AA');
    } else {
        mascaraGenericaRG(identificador);
    }

    $('#input-cep').mask('00000-000', { reverse: true });
    $('#input-telefone').mask('(00) 0000 - 0000');
    $('#input-celular').mask('(00) 00000 - 0000');

});

// Pegando o cpf enquanto o usuario digita e submetendo quando terminar
var input = document.getElementById('input-cpf');
input.addEventListener("keyup", function () {

    let identificador = document.getElementById('input-cpf').value;
    identificador = identificador.replace('.', '').replace('.', '').replace('-', '');
    if ($.isNumeric(identificador)) {

        if (identificador.length == 11) {
            submitAutomaticoForm();
        }

        $('#input-cpf').mask('AAA.AAA.AAA-AA');
    }
    else
    {
        if (identificador.length >= 3) {
            var uf = identificador.substring(identificador.length - 2, identificador.length);
            if (verificaRGCompleto(uf.toLowerCase())) {
                submitAutomaticoForm();
            }

            mascaraGenericaRG(identificador);
        }
    }
});

function submitAutomaticoForm() {
    var url_atual = window.location.href.toLowerCase();
    if (url_atual.includes('exame/create')) {
        $('#modal-espera').modal('show');
        document.getElementById('PesquisarCpf').value = 1;
        document.forms["form-exame"].submit();
    }
}

function mascaraGenericaRG(identificador) {

    var mask = '';
    for (let i = 0; i < identificador.length - 2; i++) {
        mask += 'A';
    }

    mask += '-AA';

    $('#input-cpf').mask(mask);
}

function verificaRGCompleto(uf) {
    switch (uf) {
        case 'ac': return true; break;
        case 'al': return true; break;
        case 'ap': return true; break;
        case 'am': return true; break;
        case 'ba': return true; break;
        case 'ce': return true; break;
        case 'df': return true; break;
        case 'es': return true; break;
        case 'go': return true; break;
        case 'ma': return true; break;
        case 'mt': return true; break;
        case 'ms': return true; break;
        case 'mg': return true; break;
        case 'pa': return true; break;
        case 'pb': return true; break;
        case 'pr': return true; break;
        case 'pe': return true; break;
        case 'pi': return true; break;
        case 'rj': return true; break;
        case 'rn': return true; break;
        case 'rs': return true; break;
        case 'ro': return true; break;
        case 'rr': return true; break;
        case 'sc': return true; break;
        case 'sp': return true; break;
        case 'se': return true; break;
        case 'to': return true; break;
        default: return false;
    }
}
