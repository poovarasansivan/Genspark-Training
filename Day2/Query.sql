CREATE DATABASE emp_management;

CREATE TABLE department (
dept_name VARCHAR(200) PRIMARY KEY NOT NULL,
floor INT,
phone_no INT
);

CREATE TABLE emp (
emp_no INT PRIMARY KEY NOT NULL,   
emp_name VARCHAR(200),
salary INT,    
dept_name VARCHAR(200) NOT NULL,
FOREIGN KEY (dept_name) REFERENCES department(dept_name)
);

ALTER TABLE department
ADD manager_id INT,
    CONSTRAINT fk_manager FOREIGN KEY (manager_id) REFERENCES emp(emp_no);

ALTER TABLE emp
ADD boss_no INT NULL,
	CONSTRAINT fk_boss FOREIGN KEY (boss_no) REFERENCES emp(emp_no);

CREATE TABLE item(
item_name VARCHAR(200) PRIMARY KEY NOT NULL,
item_type VARCHAR(200) NOT NULL,
item_color VARCHAR(200) NOT NULL
);

CREATE TABLE sales(
sales_no INT PRIMARY KEY NOT NULL,
sale_qunty INT,
item_name VARCHAR(200) NOT NULL,
dept_name VARCHAR(200) NOT NULL,
FOREIGN KEY (item_name) REFERENCES item(item_name),
FOREIGN KEY (dept_name) REFERENCES department(dept_name)
);

INSERT INTO department(dept_name,floor,phone_no)
VALUES('Management',5,34),
('Books',1,81),
('Clothes',2,24),
('Equipment',3,57),
('Furniture',4,14),
('Navigation', 1,41),
('Recreation',2,29),
('Accounting',5,35),
('Purchasing',5,36),
('Personnel',5,37),
('Marketing',5,38);


INSERT INTO emp (emp_no, emp_name, salary, dept_name, boss_no) VALUES
(1, 'Alice', 1500, 'Management', NULL),
(2, 'Ned', 45000, 'Marketing', 1),
(3, 'Andrew', 25000, 'Marketing', 2),
(4, 'Clare', 22000, 'Marketing', 2),
(5, 'Todd', 38000, 'Accounting', 1),
(6, 'Nancy', 22000, 'Accounting', 5),
(7, 'Brier', 43000, 'Purchasing', 1),
(8, 'Sarah', 56000, 'Purchasing', 7),
(9, 'Sophile', 35000, 'Personnel', 1),
(10, 'Sanjay', 15000, 'Navigation', 3),
(11, 'Rita', 15000, 'Books', 4),
(12, 'Gigi', 16000, 'Clothes', 4),
(13, 'Maggie', 11000, 'Clothes', 4),
(14, 'Paul', 15000, 'Equipment', 3),
(15, 'James', 15000, 'Equipment', 3),
(16, 'Pat', 15000, 'Furniture', 3),
(17, 'Mark', 15000, 'Recreation', 3);


UPDATE department SET manager_id = '1' WHERE dept_name = 'Management';
UPDATE department SET manager_id = '2' WHERE dept_name = 'Marketing';
UPDATE department SET manager_id = '5' WHERE dept_name = 'Accounting';
UPDATE department SET manager_id = '7' WHERE dept_name = 'Purchasing';
UPDATE department SET manager_id = '4' WHERE dept_name = 'Books';
UPDATE department SET manager_id = '4' WHERE dept_name = 'Clothes';
UPDATE department SET manager_id = '4' WHERE dept_name = 'Recreation';
UPDATE department SET manager_id = '3' WHERE dept_name = 'Equipment';
UPDATE department SET manager_id = '3' WHERE dept_name = 'Navigation';
UPDATE department SET manager_id = '3' WHERE dept_name = 'Furniture';
UPDATE department SET manager_id = '9' WHERE dept_name = 'Personnel';


INSERT INTO item(item_name,item_type,item_color)
VALUES('Pocket Knife-Nile', 'E', 'Brown'),
('Pocket Knife-Avon', 'E', 'Brown'),
('Compass', 'N', NULL),
('Geo positioning system', 'N', NULL),
('Elephant Polo stick', 'R', 'Bamboo'),
('Camel Saddle', 'R', 'Brown'),
('Sextant', 'N', NULL),
('Map Measure', 'N', NULL),
('Boots-snake proof', 'C', 'Green'),
('Pith Helmet', 'C', 'Khaki'),
('Hat-polar Explorer', 'C', 'White'),
('Exploring in 10 Easy Lessons', 'B', NULL),
('Hammock', 'F', 'Khaki'),
('How to win Foreign Friends', 'B', NULL),
('Map case', 'E', 'Brown'),
('Safari Chair', 'F', 'Khaki'),
('Safari cooking kit', 'F', 'Khaki'),
('Stetson', 'C', 'Black'),
('Tent - 2 person', 'F', 'Khaki'),
('Tent -8 person', 'F', 'Khaki');

INSERT INTO sales(sales_no,sale_qunty,item_name,dept_name)
values(101, 2, 'Boots-snake proof', 'Clothes'),
(102, 1, 'Pith Helmet', 'Clothes'),
(103, 1, 'Sextant', 'Navigation'),
(104, 3, 'Hat-polar Explorer', 'Clothes'),
(105, 5, 'Pith Helmet', 'Equipment'),
(106, 2, 'Pocket Knife-Nile', 'Clothes'),
(107, 3, 'Pocket Knife-Nile', 'Recreation'),
(108, 1, 'Compass', 'Navigation'),
(109, 2, 'Geo positioning system', 'Navigation'),
(110, 5, 'Map Measure', 'Navigation'),
(111, 1, 'Geo positioning system', 'Books'),
(112, 1, 'Sextant', 'Books'),
(113, 3, 'Pocket Knife-Nile', 'Books'),
(114, 1, 'Pocket Knife-Nile', 'Navigation'),
(115, 1, 'Pocket Knife-Nile', 'Equipment'),
(116, 1, 'Sextant', 'Clothes'),
(117, 1, 'Pocket Knife-Nile', 'Equipment'),
(118, 1, 'Pocket Knife-Nile', 'Recreation'),
(119, 1, 'Pocket Knife-Nile', 'Furniture'),
(120, 1, 'Pocket Knife-Nile', 'Clothes'),
(121, 1, 'Exploring in 10 Easy Lessons', 'Books'),
(122, 1, 'How to win Foreign Friends', 'Clothes'),
(123, 1, 'Compass', 'Clothes'),
(124, 1, 'Pith Helmet', 'Clothes'),
(125, 1, 'Elephant Polo stick', 'Recreation'),
(126, 1, 'Camel Saddle', 'Recreation');


SELECT * FROM emp;

SELECT * FROM department;

SELECT * FROM item;

SELECT * FROM sales;