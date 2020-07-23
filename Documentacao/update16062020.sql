-- MySQL Workbench Synchronization
-- Generated: 2020-06-16 23:47
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`empresaexame` 
ADD COLUMN `cnes` VARCHAR(20) NOT NULL DEFAULT '' AFTER `numeroLeitosUTIDisponivel`;

ALTER TABLE `monitorasus`.`exame` 
ADD COLUMN `outroSintomas` VARCHAR(100) AFTER `idAreaAtuacao`,
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC),
ADD INDEX `fk_exame_AreaAtuacao1_idx` (`idAreaAtuacao` ASC);
;

ALTER TABLE `monitorasus`.`pessoa` 
ADD COLUMN `outrosSintomas` VARCHAR(100) AFTER `perdaOlfatoPaladar`,
ADD COLUMN `cns` VARCHAR(15) AFTER `outrosSintomas`,
ADD COLUMN `idAreaAtuacao` INT(11) NOT NULL AFTER `dataObito`,
ADD COLUMN `profissao` VARCHAR(50) NOT NULL DEFAULT 'Não Informada' AFTER `idAreaAtuacao`,
ADD INDEX `fk_pessoa_AreaAtuacao1_idx` (`idAreaAtuacao` ASC);
;

CREATE TABLE IF NOT EXISTS `monitorasus`.`AreaAtuacao` (
  `idAreaAtuacao` INT(11) NOT NULL,
  `descricao` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`idAreaAtuacao`),
  UNIQUE INDEX `idProfissao_UNIQUE` (`idAreaAtuacao` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

INSERT INTO `monitorasus`.`AreaAtuacao` VALUES (0, 'Não Informada');
INSERT INTO `monitorasus`.`AreaAtuacao` VALUES (1, 'Saúde');
INSERT INTO `monitorasus`.`AreaAtuacao` VALUES (2, 'Segurança Pública');
INSERT INTO `monitorasus`.`AreaAtuacao` VALUES (3, 'Limpeza Pública');

ALTER TABLE `monitorasus`.`exame` 
ADD CONSTRAINT `fk_exame_AreaAtuacao1`
  FOREIGN KEY (`idAreaAtuacao`)
  REFERENCES `monitorasus`.`AreaAtuacao` (`idAreaAtuacao`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `monitorasus`.`pessoa` 
ADD CONSTRAINT `fk_pessoa_AreaAtuacao1`
  FOREIGN KEY (`idAreaAtuacao`)
  REFERENCES `monitorasus`.`AreaAtuacao` (`idAreaAtuacao`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
