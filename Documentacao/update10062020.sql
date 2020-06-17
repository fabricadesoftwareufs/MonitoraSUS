-- MySQL Workbench Synchronization
-- Generated: 2020-06-10 23:21
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`exame` 
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC),
ADD INDEX `fk_exame_profissao1_idx` (`idProfissao` ASC);
;

ALTER TABLE `monitorasus`.`pessoa` 
ADD COLUMN `idProfissao` INT(11) NOT NULL AFTER `dataObito`,
ADD INDEX `fk_pessoa_profissao1_idx` (`idProfissao` ASC);
;

CREATE TABLE IF NOT EXISTS `monitorasus`.`profissao` (
  `idProfissao` INT(11) NOT NULL,
  `profissao` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`idProfissao`),
  UNIQUE INDEX `idProfissao_UNIQUE` (`idProfissao` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

ALTER TABLE `monitorasus`.`exame` 
ADD CONSTRAINT `fk_exame_profissao1`
  FOREIGN KEY (`idProfissao`)
  REFERENCES `monitorasus`.`profissao` (`idProfissao`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `monitorasus`.`pessoa` 
ADD CONSTRAINT `fk_pessoa_profissao1`
  FOREIGN KEY (`idProfissao`)
  REFERENCES `monitorasus`.`profissao` (`idProfissao`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
