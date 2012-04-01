CREATE SCHEMA `gestures`;CREATE  TABLE `gestures`.`neural_gestures` (
  `idneural_gestures` INT NOT NULL AUTO_INCREMENT ,
  `frame_number` INT NULL ,
  `interval` INT NULL ,
  `params_xml` TEXT NULL ,
  `learn_xml` TEXT NULL ,
  PRIMARY KEY (`idneural_gestures`) );