$(document).ready(function () {
    var msg = document.getElementById("msg");
    if (msg != null)
        msg.click();
})

function swtAlert(type, title, message) {
    Swal.fire({
        icon: type,
        title: title,
        text: message,
    });
}

$('#ativarModal').on('show.bs.modal', function (e) {
	let action = e.relatedTarget.dataset.action;
	let cpf = e.relatedTarget.dataset.cpf;
	let idEmpresa = e.relatedTarget.dataset.idempresa;
	confirmarAssociacao(action, cpf, idEmpresa);
});

$('#agentModal').on('show.bs.modal', function (e) {
    var id = e.relatedTarget.dataset.id;
    var nome = e.relatedTarget.dataset.nome;
    var funcao = e.relatedTarget.dataset.funcao;
    $(".modal-body #modal-id").text(id);
    $(".modal-body #modal-nome").text(nome);
    $(".modal-body #modal-funcao").text(funcao);
});

$('#btnActivate').on('click', function () {
    let url = "/AgenteSecretario/ExistePessoa";
    let cpf = $('#input-cpf').val().replace(/[^0-9]/g, "");;
    let idEmpresa = $('#selectEmpresa').val();
    let action = $('#entidade').text();

    $.post(url, { cpf: cpf }, function (data) {
        if (!data) {
            swtAlert('error', 'Falha', 'O CPF informado não está cadastrado no sistema.');
        } else if (idEmpresa == null) {
            swtAlert('error', 'Falha', 'Por favor, selecione uma empresa.');
        }
		else {
			confirmarAssociacao(action, cpf, idEmpresa);     
        }
    });
});


function confirmarAssociacao(action, cpf, idEmpresa) {
	Swal.fire({
		title: 'Confirmar associação?',
		type: 'warning',
		showCancelButton: true,
		confirmButtonText: 'Sim ',
		cancelButtonText: 'Não'
	}).then(function (result) {
		if (result.value) {

			let url = "/AgenteSecretario/Activate";
			if (action == 'Agente')
				window.location.href = url + '/Agente/' + cpf + "/" + idEmpresa;
			else if (action == 'Gestor')
				window.location.href = url + '/Gestor/' + cpf + "/" + idEmpresa;
		}
	});
}

function actionDel() {
    var action = $("#modal-funcao").text();
    var idPessoa = $("#modal-id").text();

    //var url = '@Url.Action("Delete", "AgenteSecretario")';
    let url = "/AgenteSecretario/Delete";

    if (action == 'Agente')
        window.location.href = url + '/Agente/' + idPessoa;
    else if (action == 'Gestor')
        window.location.href = url + '/Gestor/' + idPessoa;
}
