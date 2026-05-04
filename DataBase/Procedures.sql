-- Realizo un Cambio

-- --------------------------
-- Procedimientos
-- --------------------------
-- create_user
-- create_student
-- create_professor
-- create_course
-- create_assignment
-- enroll_student
-- create_group
-- create_submission
-- get_user_by_email
-- get_student_courses
-- get_course_groups
-- get_group_members
-- get_assignments_by_course
-- get_submissions_by_assignment
-- get_assignment_with_submissions
-- join_group
-- leave_group
-- grade_submission
-- login_user


-- ------------------------
-- INSERTS
-- ------------------------


--  
-- create_user
--  

DELIMITER $$

CREATE PROCEDURE create_user (
    IN p_email_user VARCHAR(100),
    IN p_password_user VARCHAR(100),
    IN p_name VARCHAR(50),
    IN p_lastname VARCHAR(50)
)
BEGIN
    INSERT INTO User (email_user, password_user, name_user, lastname_user)
    VALUES (p_email_user, p_password_user, p_name, p_lastname);
END$$

DELIMITER ;

--  
-- create_student
--  

DELIMITER $$

CREATE PROCEDURE create_student (
    IN p_id_user INT
)
BEGIN
    INSERT INTO Student (id_user)
    VALUES (p_id_user);
END$$

DELIMITER ;

--  
-- create_professor
--  

DELIMITER $$

CREATE PROCEDURE create_professor (
    IN p_id_user INT
)
BEGIN
    INSERT INTO Professor (id_user)
    VALUES (p_id_user);
END$$

DELIMITER ;

--  
-- create_course
--  

DELIMITER $$

CREATE PROCEDURE create_course (
    IN p_code_course VARCHAR(50),
    IN p_id_professor INT,
    IN p_name VARCHAR(100),
    IN p_description TEXT
)
BEGIN
    INSERT INTO Course (code_course, id_professor, name_course, description_course)
    VALUES (p_code_course, p_id_professor, p_name, p_description);
END$$

DELIMITER ;

--  
-- create_assignment
--  

DELIMITER $$

CREATE PROCEDURE create_assignment (
    IN p_code_course VARCHAR(50),
    IN p_name VARCHAR(100),
    IN p_description TEXT,
    IN p_deadline DATETIME,
    IN p_is_allowed BOOLEAN
)
BEGIN
    INSERT INTO Assignment (
        code_course,
        name_assignment,
        description_assignment,
        deadline,
        is_allowed_after_deadline
    )
    VALUES (
        p_code_course,
        p_name,
        p_description,
        p_deadline,
        p_is_allowed
    );
END$$

DELIMITER ;

--  
-- enroll_student
--  

DELIMITER $$

CREATE PROCEDURE enroll_student (
    IN p_id_student INT,
    IN p_code_course VARCHAR(50)
)
BEGIN
    -- Evitar duplicados
    IF EXISTS (
        SELECT 1 FROM Enrollment
        WHERE id_student = p_id_student
          AND code_course = p_code_course
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El estudiante ya está matriculado en este curso';
    END IF;

    INSERT INTO Enrollment (id_student, code_course, id_group)
    VALUES (p_id_student, p_code_course, NULL);
END$$

DELIMITER ;

--  
-- create_group
--  

DELIMITER $$

CREATE PROCEDURE create_group (
    IN p_id_student INT,
    IN p_code_course VARCHAR(50),
    IN p_group_number INT
)
BEGIN
    DECLARE v_id_group INT;

    -- Verificar matrícula
    IF NOT EXISTS (
        SELECT 1 FROM Enrollment
        WHERE id_student = p_id_student
          AND code_course = p_code_course
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El estudiante no está matriculado';
    END IF;

    -- Verificar que no tenga grupo
    IF EXISTS (
        SELECT 1 FROM Enrollment
        WHERE id_student = p_id_student
          AND code_course = p_code_course
          AND id_group IS NOT NULL
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ya pertenece a un grupo';
    END IF;

    -- Crear grupo
    INSERT INTO `Group` (code_course, group_number, id_student)
    VALUES (p_code_course, p_group_number, p_id_student);

    SET v_id_group = LAST_INSERT_ID();

    -- Asignarlo
    UPDATE Enrollment
    SET id_group = v_id_group
    WHERE id_student = p_id_student
      AND code_course = p_code_course;
END$$

DELIMITER ;

--  
-- create_submission
--  

DELIMITER $$

CREATE PROCEDURE create_submission (
    IN p_id_group INT,
    IN p_id_assignment INT,
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
        id_group,
        id_assignment,
        submitted_at,
        project_blob
    )
    VALUES (
        p_id_group,
        p_id_assignment,
        NOW(),
        p_blob
    );

END$$

DELIMITER ;

-- ------------------------
-- GETTERS
-- ------------------------

--
-- get_user_by_email
--
DELIMITER $$

CREATE PROCEDURE get_user_by_email_user (
    IN p_email_user VARCHAR(100)
)
BEGIN
    SELECT *
    FROM User
    WHERE email_user = p_email_user;
END$$

DELIMITER ;

--
-- get_student_courses
--
DELIMITER $$

CREATE PROCEDURE get_student_courses (
    IN p_id_student INT
)
BEGIN
    SELECT c.*
    FROM Course c
    JOIN Enrollment e ON c.code_course = e.code_course
    WHERE e.id_student = p_id_student;
END$$

DELIMITER ;

--
-- get_course_groups
--
DELIMITER $$

CREATE PROCEDURE get_course_groups (
    IN p_code_course VARCHAR(50)
)
BEGIN
    SELECT *
    FROM `Group`
    WHERE code_course = p_code_course;
END$$

DELIMITER ;

--
-- get_group_members
--
DELIMITER $$

CREATE PROCEDURE get_group_members (
    IN p_id_group INT
)
BEGIN
    SELECT s.*, u.name_user, u.lastname_user, u.email_user
    FROM Enrollment e
    JOIN Student s ON e.id_student = s.id_student
    JOIN User u ON s.id_user = u.id_user
    WHERE e.id_group = p_id_group;
END$$

DELIMITER ;

--
-- get_assignments_by_course
--
DELIMITER $$

CREATE PROCEDURE get_assignments_by_course (
    IN p_code_course VARCHAR(50)
)
BEGIN
    SELECT *
    FROM Assignment
    WHERE code_course = p_code_course;
END$$

DELIMITER ;

--
-- get_submissions_by_assignment
--
DELIMITER $$

CREATE PROCEDURE get_submissions_by_assignment (
    IN p_id_assignment INT
)
BEGIN
    SELECT *
    FROM Submission
    WHERE id_assignment = p_id_assignment
    ORDER BY submitted_at DESC;
END$$

DELIMITER ;

--
-- get_assignment_with_submissions
--
DELIMITER $$

CREATE PROCEDURE get_assignment_with_submissions (
    IN p_id_assignment INT
)
BEGIN
    SELECT 
        g.id_group,
        g.group_number,
        g.code_course,

        s.submitted_at,
        s.grade,
        s.feedback,
        s.project_blob,

        -- Marca si es la última versión
        CASE 
            WHEN s.submitted_at = (
                SELECT MAX(s2.submitted_at)
                FROM Submission s2
                WHERE s2.id_group = s.id_group
                  AND s2.id_assignment = s.id_assignment
            )
            THEN 1 ELSE 0
        END AS is_latest

    FROM `Group` g
    LEFT JOIN Submission s 
        ON g.id_group = s.id_group
       AND s.id_assignment = p_id_assignment

    WHERE g.code_course = (
        SELECT code_course
        FROM Assignment
        WHERE id_assignment = p_id_assignment
    )

    ORDER BY g.group_number, s.submitted_at DESC;

END$$

DELIMITER ;

-- ------------------------
-- OPERATIONS
-- ------------------------

--
-- join_group
--

DELIMITER $$

CREATE PROCEDURE join_group (
    IN p_id_student INT,
    IN p_code_course VARCHAR(50),
    IN p_id_group INT
)
BEGIN
    DECLARE v_group_course VARCHAR(50);

    -- 1. Verificar grupo existe
    SELECT code_course INTO v_group_course
    FROM `Group`
    WHERE id_group = p_id_group;

    IF v_group_course IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El grupo no existe';
    END IF;

    -- 2. Verificar mismo curso
    IF v_group_course <> p_code_course THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El grupo no pertenece a este curso';
    END IF;

    -- 3. Verificar matrícula
    IF NOT EXISTS (
        SELECT 1 FROM Enrollment
        WHERE id_student = p_id_student
          AND code_course = p_code_course
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No está matriculado';
    END IF;

    -- 4. Verificar que no tenga grupo
    IF EXISTS (
        SELECT 1 FROM Enrollment
        WHERE id_student = p_id_student
          AND code_course = p_code_course
          AND id_group IS NOT NULL
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ya tiene grupo';
    END IF;

    -- 5. Asignar grupo
    UPDATE Enrollment
    SET id_group = p_id_group
    WHERE id_student = p_id_student
      AND code_course = p_code_course;

END$$

DELIMITER ;

--
-- leave_group
--

DELIMITER $$

CREATE PROCEDURE leave_group (
    IN p_id_student INT,
    IN p_code_course VARCHAR(50)
)
BEGIN
    -- Verificar que tenga grupo
    IF NOT EXISTS (
        SELECT 1 FROM Enrollment
        WHERE id_student = p_id_student
          AND code_course = p_code_course
          AND id_group IS NOT NULL
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No pertenece a ningún grupo';
    END IF;

    -- Quitar del grupo
    UPDATE Enrollment
    SET id_group = NULL
    WHERE id_student = p_id_student
      AND code_course = p_code_course;

END$$

DELIMITER ;

--
-- grade_submission
--

DELIMITER $$

CREATE PROCEDURE grade_submission (
    IN p_id_group INT,
    IN p_id_assignment INT,
    IN p_grade DECIMAL(5,2),
    IN p_feedback TEXT
)
BEGIN
    UPDATE Submission
    SET grade = p_grade,
        feedback = p_feedback
    WHERE id_group = p_id_group
      AND id_assignment = p_id_assignment;
END$$

DELIMITER ;

--
-- login_user
--

DELIMITER $$

CREATE PROCEDURE login_user (
    IN p_email_user VARCHAR(100),
    IN p_password_user VARCHAR(100)
)
BEGIN
    SELECT *
    FROM User
    WHERE email_user = p_email_user
      AND password_user = p_password_user;
END$$

DELIMITER ;

--
-- get_student_courses
--

DELIMITER $$

CREATE PROCEDURE get_student_courses (
    IN p_id_student INT
)
BEGIN
    SELECT c.*
    FROM Course c
    JOIN Enrollment e ON c.code_course = e.code_course
    WHERE e.id_student = p_id_student;
END$$

DELIMITER ;
