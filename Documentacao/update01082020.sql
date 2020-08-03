-- MySQL Workbench Synchronization
-- Generated: 2020-08-01 00:37
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
DROP FOREIGN KEY `fk_situacaopessoavirusbacteria_empresaexame1`;

ALTER TABLE `monitorasus`.`configuracaoNotificar` 
DROP FOREIGN KEY `fk_configuracaoNotificar_estado1`,
DROP FOREIGN KEY `fk_configuracaoNotificar_municipio1`;

ALTER TABLE `monitorasus`.`Internacao` 
DROP FOREIGN KEY `fk_pessoa_has_empresaexame_empresaexame1`;

ALTER TABLE `monitorasus`.`empresaexame` 
ADD COLUMN `cnes` VARCHAR(20) NOT NULL DEFAULT '' AFTER `numeroLeitosUTIDisponivel`,
ADD INDEX `INDEX_CNES` (`cnes` ASC);
;

ALTER TABLE `monitorasus`.`exame` 
DROP COLUMN `ehProfissionalSaude`,
ADD COLUMN `outroSintomas` VARCHAR(100) NULL DEFAULT NULL AFTER `perdaOlfatoPaladar`,
ADD COLUMN `idAreaAtuacao` INT(11) NOT NULL DEFAULT 0 AFTER `outroSintomas`,
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC) ,
ADD INDEX `fk_exame_AreaAtuacao1_idx` (`idAreaAtuacao` ASC) ,
ADD INDEX `fk_exame_CodigoColeta` (`codigoColeta` ASC);
;

ALTER TABLE `monitorasus`.`pessoa` 
ADD COLUMN `outrosSintomas` VARCHAR(100) NULL DEFAULT NULL AFTER `perdaOlfatoPaladar`,
ADD COLUMN `cns` VARCHAR(15) NULL DEFAULT NULL AFTER `outrosSintomas`,
ADD COLUMN `idAreaAtuacao` INT(11) NOT NULL AFTER `dataObito`,
ADD COLUMN `profissao` VARCHAR(50) NOT NULL DEFAULT 'Não Informada' AFTER `idAreaAtuacao`,
ADD INDEX `fk_pessoa_AreaAtuacao1_idx` (`idAreaAtuacao` ASC),
ADD INDEX `INDEX_CNS` (`cns` ASC);
;

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
ADD INDEX `fk_situacaopessoavirusbacteria_pessoa1_idx` (`idGestor` ASC),
DROP INDEX `fk_situacaopessoavirusbacteria_empresaexame1_idx` ;
;

CREATE TABLE IF NOT EXISTS `monitorasus`.`AreaAtuacao` (
  `idAreaAtuacao` INT(11) NOT NULL,
  `descricao` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`idAreaAtuacao`),
  UNIQUE INDEX `idProfissao_UNIQUE` (`idAreaAtuacao` ASC) )
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

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

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` 
DROP FOREIGN KEY `fk_situacaopessoavirusbacteria_pessoa1`;

ALTER TABLE `monitorasus`.`situacaopessoavirusbacteria` ADD CONSTRAINT `fk_situacaopessoavirusbacteria_pessoa1`
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

ALTER TABLE `monitorasus`.`Internacao` 
DROP FOREIGN KEY `fk_pessoa_has_empresaexame_pessoa1`;

ALTER TABLE `monitorasus`.`Internacao` ADD CONSTRAINT `fk_pessoa_has_empresaexame_pessoa1`
  FOREIGN KEY (`idpessoa`)
  REFERENCES `monitorasus`.`pessoa` (`idpessoa`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_pessoa_has_empresaexame_empresaexame1`
  FOREIGN KEY (`idEmpresa`)
  REFERENCES `monitorasus`.`empresaexame` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
