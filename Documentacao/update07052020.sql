-- MySQL Workbench Synchronization
-- Generated: 2020-05-07 23:58
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`exame` 
ADD COLUMN `ehProfissionalSaude` TINYINT(4) NOT NULL DEFAULT 0 AFTER `dataNotificacao`,
ADD COLUMN `codigoColeta` VARCHAR(20) NOT NULL DEFAULT '' AFTER `ehProfissionalSaude`,
ADD COLUMN `statusNotificacao` ENUM('N', 'S', 'E') NOT NULL DEFAULT 'N' AFTER `codigoColeta`,
ADD COLUMN `idNotificacao` VARCHAR(20) NOT NULL DEFAULT '' AFTER `statusNotificacao`,
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC);
;

CREATE TABLE IF NOT EXISTS `monitorasus`.`configuracaoNotificar` (
  `idConfiguracaoNotificar` INT(11) NOT NULL AUTO_INCREMENT,
  `habilitadoSMS` TINYINT(4) NOT NULL DEFAULT '0',
  `habilitadoWhatsapp` TINYINT(4) NOT NULL DEFAULT '0',
  `sid` VARCHAR(100) NOT NULL DEFAULT '',
  `token` VARCHAR(45) NOT NULL DEFAULT '',
  `mensagemPositivo` VARCHAR(2000) NOT NULL DEFAULT '',
  `mensagemNegativo` VARCHAR(2000) NOT NULL DEFAULT '',
  `mensagemImunizado` VARCHAR(2000) NOT NULL DEFAULT '',
  `mensagemIndeterminado` VARCHAR(2000) NOT NULL DEFAULT '',
  `idEstado` INT(11) NULL DEFAULT NULL,
  `idMunicipio` INT(11) NULL DEFAULT NULL,
  `idEmpresaExame` INT(11) NULL DEFAULT NULL,
  `quantidadeSMSDisponivel` INT(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`idConfiguracaoNotificar`),
  INDEX `fk_configuracaoNotificar_estado1_idx` (`idEstado` ASC),
  INDEX `fk_configuracaoNotificar_municipio1_idx` (`idMunicipio` ASC),
  INDEX `fk_configuracaoNotificar_empresaexame1_idx` (`idEmpresaExame` ASC),
  CONSTRAINT `fk_configuracaoNotificar_estado1`
    FOREIGN KEY (`idEstado`)
    REFERENCES `monitorasus`.`estado` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_configuracaoNotificar_municipio1`
    FOREIGN KEY (`idMunicipio`)
    REFERENCES `monitorasus`.`municipio` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_configuracaoNotificar_empresaexame1`
    FOREIGN KEY (`idEmpresaExame`)
    REFERENCES `monitorasus`.`empresaexame` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
