SELECT * FROM monitorasus2.pessoa
order by bairro;

SELECT cpf, cep, cidade, estado, bairro, latitude, longitude FROM monitorasus2.pessoa;
UPDATE `monitorasus2`.`pessoa` SET `latitude` = '-10.6636969', `longitude` = '-37.4525515' WHERE (`idpessoa` = '1576');
UPDATE `monitorasus2`.`pessoa` SET `latitude` = '-28.2958', `longitude` = '-37.9995' WHERE (`idpessoa` = '462');
UPDATE `monitorasus2`.`pessoa` SET `latitude` = '-10.6859', `longitude` = '-37.8626' WHERE (`idpessoa` = '37');
UPDATE `monitorasus2`.`pessoa` SET `latitude` = '-11.3536', `longitude` = '-37.4586' WHERE (`idpessoa` = '685');
UPDATE `monitorasus2`.`pessoa` SET `latitude` = '-11.0084', `longitude` = '-37.2044' WHERE (`idpessoa` = '1267');

update monitorasus2.pessoa
set
latitude = -10.9127216,
longitude = -37.0561228
where cidade = "Aracaju" and (latitude < -12 or latitude = 0 or latitude > -4)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -10.9278513,
longitude = -37.1166339
where cep = 49100000 and bairro like "eduardo%" and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -10.9965769,
longitude = -37.2120224
where cep = 49100000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;


update monitorasus2.pessoa
set
latitude = -10.8589536,
longitude = -37.0930044
where cep = 49160000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -10.2156554,
longitude = -37.4212807
where cep = 49680000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -10.6577178,
longitude = -37.309936
where cep = 49570000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -10.9204745,
longitude = -37.67039
where cep = 49400000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -11.2750411,
longitude = -37.7879303
where cep = 49290000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -11.2702154,
longitude = -37.4340985
where cep = 49200000 and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;

update monitorasus2.pessoa
set
latitude = -10.684314,
longitude = -37.4276838
where cidade = "Itabaiana" and (latitude < -13 or latitude = 0 or latitude > 0)  and idpessoa > 1;




UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro like "N/I%" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro like "NAO %" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro like "NÃO %" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro like "NI" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro = "" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro = "" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = "NÃO INFORMADO"
WHERE bairro like "--%" AND idPessoa > 1;

UPDATE `monitorasus2`.`pessoa`
SET
`bairro` = TRIM(BAIRRO)
WHERE idPessoa > 1;