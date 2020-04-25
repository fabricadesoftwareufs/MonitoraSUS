var placeSearch, autocomplete, inputHtml, geocoder;
inputHtml = document.getElementById('input-pesquisa');

var span = document.getElementById('spanInvalidCep');

// Objects forms
var componentForm = {
    route: 'long_name',
    street_number: 'short_name',
    administrative_area_level_1: 'short_name',
    administrative_area_level_2: 'long_name',
    postal_code: 'short_name'
};

var formCep = {
    cep: document.getElementById('postal_code'),
    bairro: document.getElementById('input-bairro'),
    localidade: document.getElementById('administrative_area_level_2'),
    uf: document.getElementById('administrative_area_level_1'),
    logradouro: document.getElementById('route')
};

function initAutocomplete() {
    autocomplete = new google.maps.places.Autocomplete(inputHtml, { types: ['geocode'] });

    autocomplete.setFields(['address_component']);
    autocomplete.addListener('place_changed', fillInAddress);
}

function fillInAddress() {
    var place = autocomplete.getPlace();

    ProcuraEndereco(inputHtml.value);

    for (var component in componentForm) {
        document.getElementById(component).value = '';
    }

    for (var i = 0; i < place.address_components.length; i++) {
        var addressType = place.address_components[i].types[0];
        if (componentForm[addressType]) {
            var val = place.address_components[i][componentForm[addressType]];
            document.getElementById(addressType).value = val;

            // DISABLE ONLY INPUTS THAT RECEIVED A VALID VALUE
            if (val != '')
                document.getElementById(addressType).setAttribute('readonly', 'true');
        }
    }

    if ($('#postal_code').val() == "")
        $('#postal_code').focus();
    else
        $('#street_number').focus();

}

function geolocate() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var geolocation = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            var circle = new google.maps.Circle(
                { center: geolocation, radius: position.coords.accuracy });
            autocomplete.setBounds(circle.getBounds());
        });
    }
}

function ProcuraEndereco(endereco) {
    geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': endereco }, function (results, status) {
        if (status === 'OK') {
            let latitude = document.getElementById('latitude');
            let longitude = document.getElementById('longitude');

            // SETTING VALUES
            latitude.value = results[0].geometry.location.lat();
            longitude.value = results[0].geometry.location.lng();
        } else {
            alert('ERRO AO INICIALIZAR O GEOCODE: ' + status);
        }
    });
}

function clearSelect() {
    let selectCities = document.getElementById('select-atuacao-cidade');

    for (let i = 0; i < selectCities.length; i++) {
        selectCities.options.remove(i);
        clearSelect();
    }
}

function cidadeSelect() {
    let estado = document.getElementById('select-atuacao-estado').value

    let url = "/AgenteSecretario/ReturnCities";

    $.post(url, { UF: estado }, function (data) {
        let selectCities = document.getElementById('select-atuacao-cidade');
        selectCities.disabled = true;
        clearSelect();

        for (let i = 0; i < data.length; i++) {
            let option = document.createElement("option");
            option.text = data[i].nome;
            option.value = data[i].id;

            selectCities.add(option);
        }

        selectCities.disabled = false;
    })
}

function loadEstados() {
    let selectEstados = document.getElementById('select-atuacao-estado');
    let url = "/AgenteSecretario/ReturnStates";

    $.get(url, null, function (data) {
        for (let i = 0; i < data.length; i++) {
            let option = document.createElement("option");
            option.text = data[i].nome;
            option.value = data[i].codigoUf;

            selectEstados.add(option);
        }
    })
}

function validaCpf() {
    let cpf = document.getElementById('input-cpf')
    let span = document.getElementById('spanInvalidCpf')

    let url = "/Login/ValidaCpf";

    $.post(url, { cpf: cpf.value }, function (data) {
        if (!data) {
            cpf.className = "form-control form-control-user border-danger cpf";
            cpf.style = "border-width: 2px;";
            span.hidden = false;
        } else {
            cpf.className = "form-control form-control-user cpf";
            cpf.style = "";
            span.hidden = true;
        }
    })
}

function PreencheForm() {
    if ((!isNaN(inputHtml.value) || inputHtml.value.toString().match(/\d{5}-\d{3}/))
        && inputHtml.value !== ''
        && (inputHtml.value.length == 8 || inputHtml.value.length == 9)
    ) {
        BuscaViaCep(inputHtml.value);
    }
}

function BuscaViaCep(cep) {
    let url = `https://viacep.com.br/ws/${cep}/json/`;

    $.get(url, null, function (data) {
        if (!data.erro) {
            for (let input in formCep) {
                span.hidden = true;
                formCep[input].value = '';
                for (let value in data) {
                    if (formCep[input].name.toLowerCase() == value) {
                        formCep[input].value = data[value];
                        formCep[input].setAttribute('readonly', 'true');
                    }
                }
            }
            ProcuraEndereco(formCep.logradouro.value);
        } else
            span.hidden = false;
    })
}

window.onload = function () {

    if ($('#input-cpf').val() == "") {
        $('#input-cpf').focus();
    }

    $('#modal-mensagem-retorno').modal('show');
};

$('#btn-solicitar').on('click', function () {

    var cpf = $('#input-cpf').val();
    var nome = $('#input-nome').val();
    var dataNasc = $('#input-data-nascimento').val();
    var cep = $('#postal_code').val();
    var numero = $('#street_number').val();
    var rua = $('#route').val();
    var bairro = $('#input-bairro').val();
    var cidade = $('#administrative_area_level_2').val();
    var estado = $('#administrative_area_level_1').val();
    var foneCelular = $('#input-celular').val();
    var email = $('#input-email').val();
    var outrasComorbidades = $('#input-outrasComorbidades').val();
    

    if (!(cpf === "" || nome === "" || dataNasc === "" || cep === "" ||
        rua === "" || bairro === "" || cidade === "" || estado === "" ||
        foneCelular === "" || numero === "" || email === "" || outrasComorbidades === "")) {
        $('#modal-espera').modal('show');

    }
});