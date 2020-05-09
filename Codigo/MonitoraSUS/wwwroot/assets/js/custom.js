$(document).ready(function(){
    var aplication_name='MonitoraSUS';
	$('#input-data-nascimento').mask('00-00-0000', { reverse: true });
    $('#input-cpf').mask('000.000.000-00', {reverse: true});
    $('#input-cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('#postal_code').mask('00000-000');
    $('#input-telefone').mask('(00) 0000 - 0000');
    $('#input-celular').mask('(00) 00000 - 0000');
  
});