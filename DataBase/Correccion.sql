DROP PROCEDURE IF EXISTS create_submission;
DROP PROCEDURE IF EXISTS grade_submission;
DROP TABLE Submission;

-- ************************************
-- 8. SUBMISSION
-- ************************************
CREATE TABLE Submission (
    id_student INT NOT NULL,
    id_assignment INT NOT NULL,
    submitted_at DATETIME NOT NULL,
    grade DECIMAL(5,2),
    feedback TEXT,
    project_name VARCHAR(100) NOT NULL,
    project_blob LONGBLOB NOT NULL,

    PRIMARY KEY (id_student, id_assignment),

    FOREIGN KEY (id_student)
        REFERENCES Student(id_student)
        ON DELETE CASCADE,

    FOREIGN KEY (id_assignment)
        REFERENCES Assignment(id_assignment)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- grade_submission
--

DELIMITER $$

CREATE PROCEDURE grade_submission (
    IN p_id_student INT,
    IN p_id_assignment INT,
    IN p_grade DECIMAL(5,2),
    IN p_feedback TEXT
)
BEGIN
    UPDATE Submission
    SET grade = p_grade,
        feedback = p_feedback
    WHERE id_student = p_id_student
      AND id_assignment = p_id_assignment;
END$$

DELIMITER ;

--  
-- create_submission
--  

DELIMITER $$

CREATE PROCEDURE create_submission (
    IN p_id_student INT,
    IN p_id_assignment INT,
    IN p_name VARCHAR(100),
    IN p_blob LONGBLOB
)
BEGIN
    DECLARE v_deadline DATETIME;
    DECLARE v_allowed BOOLEAN;

    -- 1. Obtener datos del assignment
    SELECT deadline, is_allowed_after_deadline
    INTO v_deadline, v_allowed
    FROM Assignment
    WHERE id_assignment = p_id_assignment;

    -- 2. Validar que exista
    IF v_deadline IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La asignación no existe';
    END IF;

    -- 3. Validar deadline
    IF NOW() > v_deadline AND v_allowed = FALSE THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Entrega fuera de tiempo no permitida';
    END IF;

    -- 4. Insertar submission
    INSERT INTO Submission (
        id_student,
        id_assignment,
        submitted_at,
        project_name,
        project_blob
    )
    VALUES (
        p_id_student,
        p_id_assignment,
        NOW(),
        p_name,
        p_blob
    );

END$$

