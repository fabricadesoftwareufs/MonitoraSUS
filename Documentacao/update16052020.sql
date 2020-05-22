-- MySQL Workbench Synchronization
-- Generated: 2020-05-16 23:04
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`exame` 
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC);
;

ALTER TABLE `monitorasus`.`pessoa` 
ADD COLUMN `situacaoSaude` ENUM('S', 'I', 'H', 'U', 'O') NOT NULL DEFAULT 'S' AFTER `outrasComorbidades`;

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
ADD COLUMN `dataUltimoMonitoramento` DATE NULL DEFAULT CURDATE() AFTER `ultimaSituacaoSaude`,
ADD COLUMN `descricao` VARCHAR(5000) NULL DEFAULT NULL AFTER `dataUltimoMonitoramento`,
ADD COLUMN `idGestor` INT(11) NULL DEFAULT NULL AFTER `descricao`,
ADD INDEX `fk_situacaopessoavirusbacteria_empresaexame1_idx` (`idGestor` ASC);
;

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
ADD CONSTRAINT `fk_situacaopessoavirusbacteria_empresaexame1`
  FOREIGN KEY (`idGestor`)
  REFERENCES `monitorasus`.`empresaexame` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
