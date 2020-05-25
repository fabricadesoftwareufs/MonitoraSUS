-- MySQL Workbench Synchronization
-- Generated: 2020-05-24 12:19
-- Model: New Model
-- Version: 1.0
-- Project: Name of the project
-- Author: abraa

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

ALTER TABLE `monitorasus`.`exame` 
CHANGE COLUMN `statusNotificacao` `statusNotificacao` ENUM('N', 'S', 'E', 'P') NOT NULL DEFAULT 'N' ,
DROP INDEX `fk_exame_municipio1_idx` ,
ADD INDEX `fk_exame_municipio1_idx` (`idMunicipio` ASC);
;

DROP TABLE IF EXISTS `monitorasus`.`municipiosgeo` ;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
