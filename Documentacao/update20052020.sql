-- MySQL Workbench Synchronization
-- Generated: 2020-05-20 08:01
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`pessoa` 
ADD COLUMN `situacaoSaude` ENUM('S', 'I', 'H', 'U', 'O') NOT NULL DEFAULT 'S' AFTER `outrasComorbidades`;

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
ADD COLUMN `dataUltimoMonitoramento` DATE NULL DEFAULT NULL AFTER `ultimaSituacaoSaude`,
ADD COLUMN `descricao` VARCHAR(5000) NULL DEFAULT NULL AFTER `dataUltimoMonitoramento`,
ADD COLUMN `idGestor` INT(11) NULL DEFAULT NULL AFTER `descricao`,
ADD INDEX `fk_situacaopessoavirusbacteria_pessoa1_idx` (`idGestor` ASC);
;

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
ADD CONSTRAINT `fk_situacaopessoavirusbacteria_pessoa1`
  FOREIGN KEY (`idGestor`)
  REFERENCES `monitorasus`.`pessoa` (`idpessoa`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `monitorasus`.`configuracaoNotificar` 
DROP FOREIGN KEY `fk_configuracaoNotificar_empresaexame1`;

ALTER TABLE `monitorasus`.`configuracaoNotificar` ADD CONSTRAINT `fk_configuracaoNotificar_estado1`
  FOREIGN KEY (`idEstado`)
  REFERENCES `monitorasus`.`estado` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_configuracaoNotificar_municipio1`
  FOREIGN KEY (`idMunicipio`)
  REFERENCES `monitorasus`.`municipio` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_configuracaoNotificar_empresaexame1`
  FOREIGN KEY (`idEmpresaExame`)
  REFERENCES `monitorasus`.`empresaexame` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
