-- MySQL Workbench Synchronization
-- Generated: 2020-05-26 23:47
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
CHANGE COLUMN `situacaoSaude` `situacaoSaude` ENUM('S', 'I', 'H', 'U', 'E', 'O') NOT NULL DEFAULT 'S' ;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
