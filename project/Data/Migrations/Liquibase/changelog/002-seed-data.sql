--liquibase formatted sql

--changeset hr-platform:011-seed-departments
INSERT INTO departments (name, code) VALUES
    ('IT Department', 'IT'),
    ('HR Department', 'HR'),
    ('Finance Department', 'FIN'),
    ('Sales Department', 'SALES')
ON CONFLICT (code) DO NOTHING;

--changeset hr-platform:012-seed-positions
INSERT INTO positions (title, level, description) VALUES
    ('Software Developer', 'Middle', 'Develops and maintains software applications.'),
    ('Senior Software Developer', 'Senior', 'Leads development efforts and mentors junior developers.'),
    ('Junior Developer', 'Junior', 'Assists in software development under supervision.'),
    ('HR Manager', 'Manager', 'Manages human resources operations.'),
    ('Finance Manager', 'Manager', 'Oversees financial operations and reporting.'),
    ('Sales Representative', 'Junior', 'Generates leads and closes sales deals.')
ON CONFLICT DO NOTHING;

--changeset hr-platform:013-seed-employees
INSERT INTO employees (first_name, last_name, email, position_id, department_id, hire_date, status) VALUES
    ('John', 'Doe', 'john.doe@hrplatform.com', 2, 1, '2022-01-15', 'Active'),
    ('Jane', 'Smith', 'jane.smith@hrplatform.com', 1, 1, '2023-03-20', 'Active'),
    ('Bob', 'Johnson', 'bob.johnson@hrplatform.com', 3, 1, '2024-06-01', 'Active'),
    ('Alice', 'Williams', 'alice.williams@hrplatform.com', 4, 2, '2021-09-10', 'Active'),
    ('Charlie', 'Brown', 'charlie.brown@hrplatform.com', 5, 3, '2020-11-05', 'Active'),
    ('Diana', 'Davis', 'diana.davis@hrplatform.com', 6, 4, '2023-07-15', 'Active'),
    ('Eve', 'Miller', 'eve.miller@hrplatform.com', 1, 1, '2024-02-01', 'Active'),
    ('Frank', 'Wilson', 'frank.wilson@hrplatform.com', 6, 4, '2022-05-12', 'OnLeave')
ON CONFLICT (email) DO NOTHING;

--changeset hr-platform:014-seed-user-accounts
INSERT INTO user_accounts (username, hashed_password, employee_id, role) VALUES
    ('admin', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 1, 'Admin'),
    ('jane.smith', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 2, 'Employee'),
    ('bob.johnson', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 3, 'Employee'),
    ('alice.williams', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 4, 'Manager'),
    ('charlie.brown', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 5, 'Manager'),
    ('diana.davis', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 6, 'Employee'),
    ('eve.miller', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 7, 'Employee')
ON CONFLICT (username) DO NOTHING;

--changeset hr-platform:015-seed-skills
INSERT INTO skills (name, category) VALUES
    ('C#', 'Technical'),
    ('ASP.NET Core', 'Technical'),
    ('PostgreSQL', 'Technical'),
    ('Docker', 'Technical'),
    ('JavaScript', 'Technical'),
    ('React', 'Technical'),
    ('Communication', 'Soft'),
    ('Leadership', 'Soft'),
    ('Problem Solving', 'Soft'),
    ('Teamwork', 'Soft'),
    ('SQL', 'Technical'),
    ('Git', 'Technical')
ON CONFLICT DO NOTHING;

--changeset hr-platform:016-seed-employee-skills
INSERT INTO employee_skills (employee_id, skill_id, proficiency_level) VALUES
    (1, 1, 'Advanced'),
    (1, 2, 'Advanced'),
    (1, 3, 'Advanced'),
    (1, 4, 'Intermediate'),
    (1, 8, 'Advanced'),
    (2, 1, 'Intermediate'),
    (2, 2, 'Intermediate'),
    (2, 3, 'Intermediate'),
    (2, 11, 'Intermediate'),
    (3, 1, 'Basic'),
    (3, 2, 'Basic'),
    (3, 12, 'Intermediate'),
    (4, 7, 'Advanced'),
    (4, 8, 'Advanced'),
    (4, 10, 'Advanced'),
    (5, 7, 'Advanced'),
    (5, 8, 'Intermediate'),
    (6, 7, 'Intermediate'),
    (6, 10, 'Intermediate'),
    (7, 1, 'Intermediate'),
    (7, 5, 'Intermediate'),
    (7, 6, 'Basic')
ON CONFLICT (employee_id, skill_id) DO NOTHING;

--changeset hr-platform:017-seed-vacancies
INSERT INTO vacancies (title, description, department_id, position_id, status, salary_range) VALUES
    ('Senior .NET Developer', 'Looking for experienced .NET developer with 5+ years of experience', 1, 2, 'Open', '$80,000 - $120,000'),
    ('Junior Frontend Developer', 'Entry level position for React/JavaScript developer', 1, 3, 'Open', '$50,000 - $70,000'),
    ('HR Specialist', 'HR professional needed for talent acquisition', 2, 4, 'Open', '$60,000 - $85,000'),
    ('Sales Manager', 'Experienced sales manager for expanding team', 4, 6, 'Closed', '$70,000 - $100,000')
ON CONFLICT DO NOTHING;

--changeset hr-platform:018-seed-leave-requests
INSERT INTO leave_requests (employee_id, start_date, end_date, type, status, comment) VALUES
    (2, '2025-01-15', '2025-01-20', 'Annual', 'Approved', 'Family vacation'),
    (3, '2025-02-01', '2025-02-03', 'Sick', 'Pending', 'Medical appointment'),
    (6, '2024-12-20', '2024-12-27', 'Annual', 'Approved', 'Holiday break'),
    (7, '2025-03-10', '2025-03-12', 'Annual', 'Pending', 'Personal time')
ON CONFLICT DO NOTHING;

--changeset hr-platform:019-seed-api-keys
INSERT INTO api_keys (key, name, expiry_date, is_active) VALUES
    ('hr-platform-dev-key-2024-abcdef123456', 'Development API Key', '2025-12-31 23:59:59', true),
    ('hr-platform-test-key-xyz789', 'Test API Key', '2026-11-11 00:00:00', true),
    ('hr-platform-expired-key-123', 'Expired API Key', '2024-01-01 00:00:00', false)
ON CONFLICT (key) DO NOTHING;
