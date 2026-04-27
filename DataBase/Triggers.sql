
DELIMITER $$

CREATE TRIGGER trg_check_group_course_insert
BEFORE INSERT ON Enrollment
FOR EACH ROW
BEGIN
    DECLARE v_group_course VARCHAR(50);

    -- Solo validar si hay grupo asignado
    IF NEW.id_group IS NOT NULL THEN
        
        -- Obtener el curso del grupo
        SELECT code_course INTO v_group_course
        FROM `Group`
        WHERE id_group = NEW.id_group;

        -- Validar consistencia
        IF v_group_course IS NULL THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'El grupo no existe';
        END IF;

        IF v_group_course <> NEW.code_course THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'El grupo no pertenece al mismo curso';
        END IF;

    END IF;
END$$

DELIMITER ;


DELIMITER $$

CREATE TRIGGER trg_check_group_course_update
BEFORE UPDATE ON Enrollment
FOR EACH ROW
BEGIN
    DECLARE v_group_course VARCHAR(50);

    IF NEW.id_group IS NOT NULL THEN
        
        SELECT code_course INTO v_group_course
        FROM `Group`
        WHERE id_group = NEW.id_group;

        IF v_group_course IS NULL THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'El grupo no existe';
        END IF;

        IF v_group_course <> NEW.code_course THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'El grupo no pertenece al mismo curso';
        END IF;

    END IF;
END$$

DELIMITER ;