-- MySQL Workbench Synchronization
-- Generated: 2020-05-27 17:09
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`empresaexame` 
ADD COLUMN `ehPublico` TINYINT(4) NOT NULL DEFAULT 0 AFTER `email`,
ADD COLUMN `fazMonitoramento` TINYINT(4) NOT NULL DEFAULT 0 AFTER `ehPublico`;

ALTER TABLE `monitorasus`.`exame` 
ADD COLUMN `aguardandoResultado` TINYINT(4) NOT NULL DEFAULT 0 AFTER `idNotificacao`,
ADD COLUMN `metodoExame` ENUM('C', 'F', 'P') NOT NULL DEFAULT 'F' AFTER `aguardandoResultado`,
ADD COLUMN `igMigG` ENUM('S', 'N', 'I') NOT NULL DEFAULT 'N' AFTER `pcr`,
ADD COLUMN `relatouSintomas` TINYINT(4) NOT NULL DEFAULT '0' AFTER `metodoExame`,
ADD COLUMN `febre` TINYINT(4) NOT NULL DEFAULT '0' AFTER `relatouSintomas`,
ADD COLUMN `tosse` TINYINT(4) NOT NULL DEFAULT '0' AFTER `febre`,
ADD COLUMN `coriza` TINYINT(4) NOT NULL DEFAULT '0' AFTER `tosse`,
ADD COLUMN `dificuldadeRespiratoria` TINYINT(4) NOT NULL DEFAULT '0' AFTER `coriza`,
ADD COLUMN `dorGarganta` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dificuldadeRespiratoria`,
ADD COLUMN `diarreia` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dorGarganta`,
ADD COLUMN `dorOuvido` TINYINT(4) NOT NULL DEFAULT '0' AFTER `diarreia`,
ADD COLUMN `nausea` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dorOuvido`,
ADD COLUMN `dorAbdominal` TINYINT(4) NOT NULL DEFAULT '0' AFTER `nausea`,
ADD COLUMN `perdaOlfatoPaladar` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dorAbdominal`,
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC);
;

ALTER TABLE `monitorasus`.`pessoa` 
ADD COLUMN `doencaRenal` TINYINT(4) NOT NULL DEFAULT '0' AFTER `doencaRespiratoria`,
ADD COLUMN `epilepsia` TINYINT(4) NOT NULL DEFAULT '0' AFTER `doencaRenal`,
ADD COLUMN `febre` TINYINT(4) NOT NULL DEFAULT '0' AFTER `situacaoSaude`,
ADD COLUMN `tosse` TINYINT(4) NOT NULL DEFAULT '0' AFTER `febre`,
ADD COLUMN `coriza` TINYINT(4) NOT NULL DEFAULT '0' AFTER `tosse`,
ADD COLUMN `dificuldadeRespiratoria` TINYINT(4) NOT NULL DEFAULT '0' AFTER `coriza`,
ADD COLUMN `dorGarganta` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dificuldadeRespiratoria`,
ADD COLUMN `diarreia` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dorGarganta`,
ADD COLUMN `dorOuvido` TINYINT(4) NOT NULL DEFAULT '0' AFTER `diarreia`,
ADD COLUMN `nausea` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dorOuvido`,
ADD COLUMN `dorAbdominal` TINYINT(4) NOT NULL DEFAULT '0' AFTER `nausea`,
ADD COLUMN `perdaOlfatoPaladar` TINYINT(4) NOT NULL DEFAULT '0' AFTER `dorAbdominal`,
ADD COLUMN `dataObito` DATETIME NULL DEFAULT NULL AFTER `perdaOlfatoPaladar`;

CREATE TABLE IF NOT EXISTS `monitorasus`.`Internacao` (
  `idInternacao` INT(11) NOT NULL,
  `idpessoa` INT(11) NOT NULL,
  `idEmpresa` INT(11) NOT NULL,
  `dataInicio` DATETIME NOT NULL,
  `dataFim` DATETIME NULL DEFAULT NULL,
  `usoO2` ENUM('V', 'C', 'M', 'A') NOT NULL DEFAULT 'A',
  PRIMARY KEY (`idInternacao`),
  INDEX `fk_pessoa_has_empresaexame_empresaexame1_idx` (`idEmpresa` ASC),
  INDEX `fk_pessoa_has_empresaexame_pessoa1_idx` (`idpessoa` ASC),
  CONSTRAINT `fk_pessoa_has_empresaexame_pessoa1`
    FOREIGN KEY (`idpessoa`)
    REFERENCES `monitorasus`.`pessoa` (`idpessoa`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_pessoa_has_empresaexame_empresaexame1`
    FOREIGN KEY (`idEmpresa`)
    REFERENCES `monitorasus`.`empresaexame` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
