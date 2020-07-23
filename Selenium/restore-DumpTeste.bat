@echo on
@echo Iniciando a restauração do dump de teste do MonitoraSUS. Aguarde....

"C:\Program Files (x86)\MySQL\MySQL Server 5.5\bin\mysql.exe" --protocol=tcp --host=127.0.0.1 --user=root --password=123456 --port=3306 --default-character-set=utf8 --comments --database=monitorasus < "monitoraSUS-dump-teste.sql"

@echo on
@echo Dump de Teste restaurado com SUCESSO!

pause