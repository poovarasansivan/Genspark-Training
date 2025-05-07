-- Joins
use pubs;
go

select * from publishers;

select * from titles;

select au_id, title from titleauthor join titles on titles.title_id = titleauthor.title_id;

select au_id, title from titleauthor right outer join titles on titles.title_id = titleauthor.title_id;

select titleauthor.au_id, au_fname,au_lname, title from titleauthor join titles on titles.title_id = titleauthor.title_id 
join authors on authors.au_id=titleauthor.au_id;

select titleauthor.au_id, au_fname,au_lname, title from titleauthor right outer join titles on titles.title_id = titleauthor.title_id 
right outer join authors on authors.au_id=titleauthor.au_id;

select titleauthor.au_id, CONCAT(au_fname,' ',au_lname) as author_name, title from titleauthor join titles on titles.title_id = titleauthor.title_id 
join authors on authors.au_id=titleauthor.au_id;

select titleauthor.au_id, CONCAT(au_fname,' ',au_lname) as author_name, title from titleauthor right outer join titles on titles.title_id = titleauthor.title_id 
right outer join authors on authors.au_id=titleauthor.au_id;


select pub_name, title Book_Name, ord_date from publishers join titles on titles.pub_id=publishers.pub_id 
join sales on sales.title_id=titles.title_id

--Print the publisher name and the first book sale date for all the publishers
select pub_name Publisher_Name, MIN(ord_date) First_Order_Date from publishers
left outer join titles on titles.pub_id = publishers.pub_id 
left outer join sales on titles.title_id = sales.title_id group by publishers.pub_name order by 1 asc;

--print the bookname and teh store address of the sale

select title Book_Name,stor_address Store_Address from titles left outer join sales on sales.title_id=titles.title_id
left outer join stores on stores.stor_id=sales.stor_id order by 1 asc;


select title Book_Name,stor_address Store_Address from titles join sales on sales.title_id=titles.title_id
join stores on stores.stor_id=sales.stor_id order by 1 asc;

-- Stored Procedures

-- How to Create Stored Procedure and How to run.

CREATE PROCEDURE proc_First_Procedure
AS
BEGIN
	print 'Hello Poovarasan!'
END

EXEC proc_First_Procedure

-- How to Create Stored Procedure with Parameter

CREATE TABLE Products(
id INT IDENTITY(1,1) PRIMARY KEY,
name NVARCHAR(100) NOT NULL,
details NVARCHAR(MAX),
);

GO

CREATE PROCEDURE proc_Product_Procedure(@pname NVARCHAR(100), @pdetails  NVARCHAR(MAX))
AS
BEGIN
	INSERT INTO Products(name,details) values(@pname,@pdetails)
END

GO

EXEC proc_Product_Procedure 'Laptop','{"brand":"DELL","Spec":{"ram":"16GB","Model":"Windows i7"}}'

SELECT * FROM Products;

-- how to use Json Query and Update Json Data.

SELECT JSON_QUERY(details,'$.Spec') AS Products_Specification FROM Products;

-- To Update JSON Data using Stroed Procedure

CREATE PROCEDURE proc_Update_Products(@id INT, @newvalue VARCHAR(20))
AS
BEGIN
	UPDATE Products SET details = JSON_MODIFY(details,'$.Spec.ram',@newvalue) WHERE id=@id
END

EXEC proc_Update_Products 1,'36 GB';

-- How Read the Value of JSON Property

SELECT id as Product_ID, name AS Product_Name, JSON_VALUE(details,'$.brand') AS Brand_Name FROM Products;

-- How to insert BULK JSON Data

CREATE TABLE Posts(
userId INT ,
id int PRIMARY KEY,
title VARCHAR(100),
body  NVARCHAR(MAX)
);


DECLARE @jsondata NVARCHAR(MAX) = '
[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]'

INSERT INTO Posts (userId,id, title, body)
SELECT 
    userId,
	id,
    title,
    body
FROM OPENJSON(@jsondata)
WITH (
    userId INT,
    id INT,
    title VARCHAR(100),
    body NVARCHAR(MAX)
);

-- BY Using Stored Procedure How to insert bulk data.

CREATE PROCEDURE proc_BulkInsert(@jsondata NVARCHAR(MAX))
AS
BEGIN
INSERT INTO Posts(userId,id, title, body)
SELECT 
    userId,
	id,
    title,
    body
FROM OPENJSON(@jsondata)
WITH (
    userId INT,
    id INT,
    title VARCHAR(100),
    body NVARCHAR(MAX)
);
END

EXEC proc_BulkInsert '
[{
    "userId": 1,
    "id": 3,
    "title": "ea molestias quasi exercitationem repellat qui ipsa sit aut",
    "body": "et iusto sed quo iure\nvoluptatem occaecati omnis eligendi aut ad\nvoluptatem doloribus vel accusantium quis pariatur\nmolestiae porro eius odio et labore et velit aut"
  },
  {
    "userId": 1,
    "id": 4,
    "title": "eum et est occaecati",
    "body": "ullam et saepe reiciendis voluptatem adipisci\nsit amet autem assumenda provident rerum culpa\nquis hic commodi nesciunt rem tenetur doloremque ipsam iure\nquis sunt voluptatem rerum illo velit"
  },
  {
    "userId": 1,
    "id": 5,
    "title": "nesciunt quas odio",
    "body": "repudiandae veniam quaerat sunt sed\nalias aut fugiat sit autem sed est\nvoluptatem omnis possimus esse voluptatibus quis\nest aut tenetur dolor neque"
  }]
'

SELECT * FROM Products;

SELECT * From Products Where
TRY_CAST(JSON_VALUE(details,'$.Spec.ram') as NVARCHAR(20))='36 GB';

--create a procedure that brings post by taking the user_id as parameter

CREATE PROCEDURE proc_GetPostByUserid(@id int)
AS
BEGIN
	SELECT * FROM Posts WHERE userId = @id
END

EXEC proc_GetPostByUserid @id = 1