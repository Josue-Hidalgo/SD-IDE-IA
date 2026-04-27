-- ************************************
-- 1. USER
-- ************************************
CREATE TABLE User (
    id_user INT AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    name_user VARCHAR(50),
    lastname_user VARCHAR(50)
) ENGINE=InnoDB;

-- ************************************
-- 2. STUDENT
-- ************************************
CREATE TABLE Student (
    id_student INT AUTO_INCREMENT PRIMARY KEY,
    id_user INT NOT NULL,
    
    FOREIGN KEY (id_user)
        REFERENCES User(id_user)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- ************************************
-- 3. PROFESSOR
-- ************************************
CREATE TABLE Professor (
    id_professor INT AUTO_INCREMENT PRIMARY KEY,
    id_user INT NOT NULL,
    
    FOREIGN KEY (id_user)
        REFERENCES User(id_user)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- ************************************
-- 4. COURSE
-- ************************************
CREATE TABLE Course (
    code_course VARCHAR(50) PRIMARY KEY,
    id_professor INT NOT NULL,
    name_course VARCHAR(100),
    description_course TEXT,

    FOREIGN KEY (id_professor)
        REFERENCES Professor(id_professor)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- ************************************
-- 5. ASSIGNMENT
-- ************************************
CREATE TABLE Assignment (
    id_assignment INT AUTO_INCREMENT PRIMARY KEY,
    code_course VARCHAR(50) NOT NULL,
    name_assignment VARCHAR(100),
    description_assignment TEXT,
    deadline DATETIME,
    is_allowed_after_deadline BOOLEAN,

    FOREIGN KEY (code_course)
        REFERENCES Course(code_course)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- ************************************
-- 6. GROUP
-- ************************************
CREATE TABLE `Group` (
    id_group INT AUTO_INCREMENT PRIMARY KEY,
    code_course VARCHAR(50) NOT NULL,
    group_number INT NOT NULL,
    owner_id_student INT NOT NULL,

    -- Evita grupos duplicados dentro de un curso
    UNIQUE (code_course, group_number),

    FOREIGN KEY (code_course)
        REFERENCES Course(code_course)
        ON DELETE CASCADE,

    FOREIGN KEY (owner_id_student)
        REFERENCES Student(id_student)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- ************************************
-- 7. ENROLLMENT
-- ************************************
CREATE TABLE Enrollment (
    id_student INT NOT NULL,
    code_course VARCHAR(50) NOT NULL,
    id_group INT,

    PRIMARY KEY (id_student, code_course),

    FOREIGN KEY (id_student)
        REFERENCES Student(id_student)
        ON DELETE CASCADE,

    FOREIGN KEY (code_course)
        REFERENCES Course(code_course)
        ON DELETE CASCADE,

    FOREIGN KEY (id_group)
        REFERENCES `Group`(id_group)
        ON DELETE SET NULL
) ENGINE=InnoDB;

-- ************************************
-- 8. SUBMISSION
-- ************************************
CREATE TABLE Submission (
    id_group INT NOT NULL,
    id_assignment INT NOT NULL,
    submitted_at DATETIME,
    grade DECIMAL(5,2),
    feedback TEXT,
    project_blob LONGBLOB,

    PRIMARY KEY (id_group, id_assignment),

    FOREIGN KEY (id_group)
        REFERENCES `Group`(id_group)
        ON DELETE CASCADE,

    FOREIGN KEY (id_assignment)
        REFERENCES Assignment(id_assignment)
        ON DELETE CASCADE
) ENGINE=InnoDB;