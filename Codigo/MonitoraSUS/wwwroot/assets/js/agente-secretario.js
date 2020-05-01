$(document).ready(function () {
        var msg = document.getElementById("msg");
    if(msg != null)
        msg.click();
})

function swtAlert(type,title,message){
    Swal.fire({
        icon: type,
        title: title,
        text: message,
    });
}

$('#agentModal').on('show.bs.modal', function(e) {
    var id = e.relatedTarget.dataset.id;
    var nome = e.relatedTarget.dataset.nome;
    var funcao = e.relatedTarget.dataset.funcao;
    $(".modal-body #modal-id").text(id);
    $(".modal-body #modal-nome").text(nome);
    $(".modal-body #modal-funcao").text(funcao);
});

$('#btnAssociaAgente').on('click', function () {
    let url = "/AgenteSecretario/ExistsAgent";
    let cpf = document.getElementById('input-cpf')

    $.post(url, {cpf: cpf.value }, function (data) {
        if (!data) {
            swtAlert('error', 'Falha', 'O notificador com esse CPF não existe!');
        } else {
            $("#empresaModal").modal('show');
        }
    });
});

function associaAgente(idEmpresa) {
    let cpf = document.getElementById('input-cpf').value;

    let url = '@Url.Action("AssignAgentToCorp", "AgenteSecretario")';

    window.location.href = url + "/" + idEmpresa + "/" + cpf;
}


function actionDel() {
    var action = $("#modal-funcao").text();
    var idPessoa = $("#modal-id").text();

    var url = '@Url.Action("excludeAgent", "AgenteSecretario")';

    if (action == 'Agente')
        window.location.href = url + '/Agente/' + idPessoa;
    else if(action == 'Gestor')
        window.location.href = url + '/Gestor/' + idPessoa;
}
