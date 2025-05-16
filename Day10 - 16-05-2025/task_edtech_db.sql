/*
1. You are tasked with building a PostgreSQL-backed database for an EdTech company that manages online training and certification programs for individuals across various technologies.

Tables to Design (Normalized to 3NF):

1. students

   `student_id (PK)`, `name`, `email`, `phone`

2. courses

   `course_id (PK)`, `course_name`, `category`, `duration_days`

3. trainers

   `trainer_id (PK)`, `trainer_name`, `expertise`

4. enrollments

   `enrollment_id (PK)`, `student_id (FK)`, `course_id (FK)`, `enroll_date`

5. certificates

   `certificate_id (PK)`, `enrollment_id (FK)`, `issue_date`, `serial_no`

6. course_trainers (Many-to-Many if needed)

   `course_id`, `trainer_id`
*/

create database edtech;


create table students(
student_id serial primary key,
name text not null,
email text not null,
phone text not null
);


create table courses(
course_id serial primary key,
course_name text not null,
category text not null,
duration text not null
);

create table trainers(
trainer_id serial primary key,
trainer_name text not null,
expertise text not null
);


create table enrollments(
enrollment_id serial primary key, 
student_id int not null,
course_id int not null,
enroll_date timestamp default current_timestamp,
foreign key (student_id) references students(student_id)
);


create table certificates(
certificate_id serial primary key,
enrollment_id int not null,
issue_date timestamp default current_timestamp,
serial_no int unique,
foreign key (enrollment_id) references enrollments(enrollment_id)
);

create table course_trainers (
course_id INT not null,
trainer_id INT not null,
primary key (course_id, trainer_id),
foreign key (course_id) references courses(course_id),
foreign key (trainer_id) references trainers(trainer_id)
);


/*
Phase 2: DDL & DML

* Create all tables with appropriate constraints (PK, FK, UNIQUE, NOT NULL)
* Insert sample data using `INSERT` statements
* Create indexes on `student_id`, `email`, and `course_id`

*/

insert into students (name, email, phone) 
values ('Poovarasan S', 'poovarasan@gamil.com', '8667536856'),
('Praveen R', 'praveen@gmail.com', '9808623545'),
('Prakash P', 'prakash@gmail.com', '9675092498'),
('Jayaprakash P', 'jp@gamil.com', '1234556677');

select * from students;


insert into courses (course_name, category, duration) 
values ('Java Programming', 'Programming', '30 Days'),
('MERN Stack Website Development', 'Web Development', '60 Days'),
('Devops For Beginners','Cloud', '60 Days');

select * from courses;


insert into trainers (trainer_name, expertise) 
values ('Ajay A', 'Devops and Cloud Services'),
('Rajesh P', 'Java Programming'),
('Sathish S', 'Full Stack Website Developement');

select * from trainers;

insert into enrollments (student_id, course_id, enroll_date)
values (1,1,now()),
(2,3,'2025-05-15 10:54:26.13354'),
(4,2,now()),
(3,1,now());

select * from enrollments;

insert into certificates (enrollment_id, issue_date, serial_no)
values (1, now(), '902345579201134'),
(2, '2025-05-15 10:54:26.13351', '720239018091091'),
(3, now(),'720239018091092');

select * from certificates;

insert into course_trainers (course_id, trainer_id) 
values (1,2),
(2,3),
(3,1);

select * from course_trainers;

-- Create indexes on `student_id`, `email`, and `course_id`

create index idx_student_id on students(student_id);

create index idx_student_email on students(email);

create index idx_course_id on courses(course_id);

create index idx_en_student_id on enrollments(student_id);

create index idx_en_course_id on enrollments(course_id);

create index idx_trainer_course_id on course_trainers(course_id);

/*
Phase 3: SQL Joins Practice

Write queries to:

1. List students and the courses they enrolled in
2. Show students who received certificates with trainer names
3. Count number of students per course

*/

select s.student_id, s.name, s.email, s.phone, e.enrollment_id, c.course_id, c.course_name, c.category, c.duration from students s 
join enrollments e on s.student_id = e.student_id join courses c on e.course_id = c.course_id order by s.student_id

select s.student_id, s.name, s.email, cu.course_name, cu.category, cu.duration, c.certificate_id, c.issue_date, c.serial_no, t.trainer_name from students s
join enrollments e on s.student_id = e.student_id
join certificates c on c.enrollment_id = e.enrollment_id
join courses cu on cu.course_id = e.course_id
join course_trainers ct on ct.course_id = cu.course_id
join trainers t on t.trainer_id = ct.trainer_id
order by s.student_id

select c.course_id, c.course_name, count(c.course_id) as students_count from students s 
join enrollments e on s.student_id = e.student_id
join courses c on c.course_id = e.course_id group by c.course_id, c.course_name


/*
Phase 4: Functions & Stored Procedures

Function:

Create `get_certified_students(course_id INT)`
→ Returns a list of students who completed the given course and received certificates.

Stored Procedure:

Create `sp_enroll_student(p_student_id, p_course_id)`
→ Inserts into `enrollments` and conditionally adds a certificate if completed (simulate with status flag).
→ Insert into enrollments and certification details both when complete the course.
*/

create function func_get_certified_students(p_course_id int)
returns table (
	student_id INT,
    student_name TEXT,
    course_id INT,
    course_name TEXT,
    certificate_id INT,
    serial_no TEXT,
    issue_date timestamp
) 
as $$
begin
	return query select s.student_id, s.name, c.course_id, c.course_name, cf.certificate_id, cf.serial_no, cf.issue_date from students s 
	join enrollments e on e.student_id = s.student_id
	join courses c on c.course_id = e.course_id
	join certificates cf on cf.enrollment_id = e.enrollment_id
	where cf.issue_date is not null and e.course_id = p_course_id;
end;
$$ language plpgsql;

select * from func_get_certified_students(1);


CREATE OR REPLACE PROCEDURE proc_sp_enroll_student(p_student_id INT, p_course_id INT)
LANGUAGE plpgsql
AS $$
DECLARE 
    certificate_no TEXT;
BEGIN
    IF EXISTS (
        SELECT 1 FROM enrollments 
        WHERE student_id = p_student_id AND course_id = p_course_id
    ) THEN
        IF EXISTS (
            SELECT 1 FROM certificates 
            WHERE enrollment_id IN (
                SELECT enrollment_id 
                FROM enrollments 
                WHERE student_id = p_student_id AND course_id = p_course_id
            )
        ) THEN
            RAISE NOTICE 'Student % already completed the course -% and certification received', p_student_id, p_course_id;
        ELSE
            -- Generate 15-digit serial number as text
            certificate_no := lpad(floor(random() * 1e15)::BIGINT::TEXT, 15, '0');

            INSERT INTO certificates (enrollment_id, issue_date, serial_no)
            SELECT enrollment_id, CURRENT_DATE, certificate_no
            FROM enrollments
            WHERE student_id = p_student_id AND course_id = p_course_id;

            RAISE NOTICE 'Certificate added for the student';
        END IF;
    ELSE
        INSERT INTO enrollments (student_id, course_id, enroll_date, course_status)
        VALUES (p_student_id, p_course_id, NOW(), 'Not Completed');

        RAISE NOTICE 'Student has been enrolled in a course';
    END IF;
END;
$$;

		
call proc_sp_enroll_student(7, 3);

select * from certificates;

/*
Phase 5: Cursor

Use a cursor to:

- Loop through all students in a course
- Print name and email of those who do not yet have certificates

*/


do $$
declare

	student record;
	student_course cursor for select s.name, s.email from students s
	join enrollments e on e.student_id = s.student_id
	left join certificates c on c.enrollment_id = e.enrollment_id
	where c.enrollment_id is null;
	
begin
	open student_course;
	loop
		fetch student_course into student;
		exit when not found;

		raise notice 'Not Ceritified Student Name % and Email %', student.name, student.email;

		end loop;
		close student_course;
end;
$$;

/*
 Phase 6: Security & Roles

1. Create a `readonly_user` role:

   * Can run `SELECT` on `students`, `courses`, and `certificates`
   * Cannot `INSERT`, `UPDATE`, or `DELETE`

2. Create a `data_entry_user` role:

   * Can `INSERT` into `students`, `enrollments`
   * Cannot modify certificates directly
*/

create user readonly_user with password 'read12345';

grant connect on database edtech to readonly_user;

grant usage on schema public to readonly_user;

grant select on public.certificates, public.courses, public.students to readonly_user;


revoke select on public.certificates,public.courses, public.students from readonly_user;

revoke usage on schema public from readonly_user;

revoke connect on database edtech from readonly_user;



create user data_entry_user with password 'data12345';

grant connect on database edtech to data_entry_user;

grant usage on schema public to data_entry_user;

grant insert on public.students, public.enrollments to data_entry_user;



revoke insert on public.students, public.enrollments from data_entry_user;

revoke usage on schema public from data_entry_user;

revoke connect on database edtech from data_entry_user;


/*

Phase 7: Transactions & Atomicity

Write a transaction block that:

 - Enrolls a student
 - Issues a certificate
 - Fails if certificate generation fails (rollback)

*/

select * from certificates

create  function enroll_and_certify_student(p_student_id int, p_course_id int )
returns void
language plpgsql
as $$
declare 
	v_eroll_id int;
	v_serial_no text;
begin
	
	insert into enrollments (student_id, course_id,enroll_date, course_status)
	values (p_student_id, p_course_id, now(), 'Not Completed') 
	returning enrollment_id into v_eroll_id;

	v_serial_no := lpad(floor(random() * 1e15)::bigint::text, 15, '0');
	
	insert into certificates (enrollment_id, issue_date, serial_no)
    values (v_eroll_id, NOW(), v_serial_no); 

	raise notice 'Student % enrolled and certified successfully.', p_student_id;
	
	exception
		when others then
		 raise notice 'Error occurred. Rolling back: %', SQLERRM;
		 raise;
end;
$$;

select enroll_and_certify_student (9,1);

select enroll_and_certify_student (12,1)
