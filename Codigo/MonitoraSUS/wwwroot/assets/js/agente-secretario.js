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

$('#empresaModal').on('show.bs.modal', function(e) {
    var cpf = e.relatedTarget.dataset.cpf;
    $(".modal-body #modal-cpf").text(cpf);
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
    let cpf = document.getElementById('input-cpf')

    $.post(url, {cpf: cpf.value }, function (data) {
        if (!data) {
	        swtAlert('error', 'Falha', 'O CPF informado não está cadastrado no sistema.');
        } else {
            $("#empresaModal").modal('show');
        }
    });
});

function associar(idEmpresa, action) {
	var cpf = document.getElementById('input-cpf').value;
	if (cpf == "") {
		cpf = $("#modal-cpf").text();
		cpf = cpf.replace(/[^0-9]/g, "");
	}
	//var url = '@Url.Action("Activate", "AgenteSecretario")';
	let url = "/AgenteSecretario/Activate";
	if (action == 'Agente')
		window.location.href = url + '/Agente/' + cpf + "/" + idEmpresa;
	else if (action == 'Gestor')
		window.location.href = url + '/Gestor/' + cpf + "/" + idEmpresa;
}


function actionDel() {
    var action = $("#modal-funcao").text();
    var idPessoa = $("#modal-id").text();

	//var url = '@Url.Action("Delete", "AgenteSecretario")';
	let url = "/AgenteSecretario/Activate";

    if (action == 'Agente')
        window.location.href = url + '/Agente/' + idPessoa;
    else if(action == 'Gestor')
        window.location.href = url + '/Gestor/' + idPessoa;
}
