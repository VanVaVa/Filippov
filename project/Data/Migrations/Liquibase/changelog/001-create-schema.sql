--liquibase formatted sql

--changeset hr-platform:001-create-departments
CREATE TABLE IF NOT EXISTS departments (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    manager_id INTEGER
);

CREATE INDEX IF NOT EXISTS idx_departments_code ON departments(code);

--changeset hr-platform:002-create-positions
CREATE TABLE IF NOT EXISTS positions (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    level VARCHAR(50) NOT NULL,
    description TEXT
);

--changeset hr-platform:003-create-employees
CREATE TABLE IF NOT EXISTS employees (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    position_id INTEGER NOT NULL,
    department_id INTEGER NOT NULL,
    hire_date DATE NOT NULL,
    status VARCHAR(50) NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_employees_email ON employees(email);
CREATE INDEX IF NOT EXISTS idx_employees_department ON employees(department_id);

--changeset hr-platform:003a-add-employees-position-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_employees_position'
ALTER TABLE employees ADD CONSTRAINT fk_employees_position FOREIGN KEY (position_id) REFERENCES positions(id);

--changeset hr-platform:003b-add-employees-department-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_employees_department'
ALTER TABLE employees ADD CONSTRAINT fk_employees_department FOREIGN KEY (department_id) REFERENCES departments(id);

--changeset hr-platform:004-create-skills
CREATE TABLE IF NOT EXISTS skills (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    category VARCHAR(50) NOT NULL
);

--changeset hr-platform:005-create-employee-skills
CREATE TABLE IF NOT EXISTS employee_skills (
    employee_id INTEGER NOT NULL,
    skill_id INTEGER NOT NULL,
    proficiency_level VARCHAR(50) NOT NULL,
    PRIMARY KEY (employee_id, skill_id)
);

--changeset hr-platform:005a-add-employee-skills-employee-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_employee_skills_employee'
ALTER TABLE employee_skills ADD CONSTRAINT fk_employee_skills_employee FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE CASCADE;

--changeset hr-platform:005b-add-employee-skills-skill-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_employee_skills_skill'
ALTER TABLE employee_skills ADD CONSTRAINT fk_employee_skills_skill FOREIGN KEY (skill_id) REFERENCES skills(id) ON DELETE CASCADE;

--changeset hr-platform:006-create-vacancies
CREATE TABLE IF NOT EXISTS vacancies (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    department_id INTEGER NOT NULL,
    position_id INTEGER NOT NULL,
    status VARCHAR(50) NOT NULL,
    salary_range VARCHAR(100)
);

CREATE INDEX IF NOT EXISTS idx_vacancies_status ON vacancies(status);

--changeset hr-platform:006a-add-vacancies-department-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_vacancies_department'
ALTER TABLE vacancies ADD CONSTRAINT fk_vacancies_department FOREIGN KEY (department_id) REFERENCES departments(id);

--changeset hr-platform:006b-add-vacancies-position-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_vacancies_position'
ALTER TABLE vacancies ADD CONSTRAINT fk_vacancies_position FOREIGN KEY (position_id) REFERENCES positions(id);

--changeset hr-platform:007-create-leave-requests
CREATE TABLE IF NOT EXISTS leave_requests (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    type VARCHAR(50) NOT NULL,
    status VARCHAR(50) NOT NULL,
    comment TEXT
);

CREATE INDEX IF NOT EXISTS idx_leave_requests_employee ON leave_requests(employee_id);
CREATE INDEX IF NOT EXISTS idx_leave_requests_status ON leave_requests(status);

--changeset hr-platform:007a-add-leave-requests-employee-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_leave_requests_employee'
ALTER TABLE leave_requests ADD CONSTRAINT fk_leave_requests_employee FOREIGN KEY (employee_id) REFERENCES employees(id);

--changeset hr-platform:008-create-user-accounts
CREATE TABLE IF NOT EXISTS user_accounts (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    hashed_password VARCHAR(255) NOT NULL,
    employee_id INTEGER NOT NULL UNIQUE,
    role VARCHAR(50) NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_user_accounts_username ON user_accounts(username);

--changeset hr-platform:008a-add-user-accounts-employee-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_user_accounts_employee'
ALTER TABLE user_accounts ADD CONSTRAINT fk_user_accounts_employee FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE CASCADE;

--changeset hr-platform:009-create-api-keys
CREATE TABLE IF NOT EXISTS api_keys (
    id SERIAL PRIMARY KEY,
    key VARCHAR(255) NOT NULL UNIQUE,
    name VARCHAR(255) NOT NULL,
    expiry_date TIMESTAMP NOT NULL,
    is_active BOOLEAN NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_api_keys_key ON api_keys(key);

--changeset hr-platform:010-add-departments-fk
--preconditions onFail:CONTINUE
--precondition-sql-check expectedResult:0 SELECT COUNT(*) FROM pg_constraint WHERE conname = 'fk_departments_manager'
ALTER TABLE departments ADD CONSTRAINT fk_departments_manager FOREIGN KEY (manager_id) REFERENCES employees(id) ON DELETE SET NULL;

