-- DAY 4 - 08-05-2025

-- Example for Out Parameter in stored procedures.

select * from Products

-- Creating a stored procedure that takes the input for where condition and returns the output through the outparameter
create procedure proc_OutParameterexample(@psec varchar(20),@pcount int out)
as
begin
	set @pcount=(select count(*) from Products where TRY_CAST(JSON_VALUE(details,'$.Spec.ram') as nvarchar(20))=@psec)
end

-- Declared a variable count to store the result of stored procedure out parameter.
declare @count int
exec proc_OutParameterexample '36 GB', @count out
print concat('The Product count is ',@count)


-- Working with CSV file Bulk Import of data

create table students(
id varchar(50) primary key,
name varchar(100),
year varchar(100),
batch varchar(100)
);

create or alter procedure proc_BulkInsetStudents(@filepath varchar(200))
as
begin
	declare @insertstatement nvarchar(max)
	set @insertstatement = 'BULK INSERT students from ''' + @filepath +'''
	with(
	FIRSTROW=2,
	FIELDTERMINATOR='','',
	ROWTERMINATOR=''\n'')'
	exec sp_executesql @insertstatement
end

exec proc_BulkInsetStudents 'C:\Training\Day4 - 08-05-2025\data.csv'

drop procedure proc_BulkInsetStudents


-- Now working with a exception handling of the bulk insert procedure

create table  logs(
logid int identity(1,1) primary key,
filepath varchar(300),
status varchar(100) constraint chk_status Check(status in('Success','Failed')),
message varchar(600),
createdon datetime default getdate()
)

create procedure proc_BulkInsetStudents(@filepath varchar(200))
as
begin
	begin try
	declare @insertstatement nvarchar(max)
	set @insertstatement = 'BULK INSERT students from ''' + @filepath +'''
	with(
	FIRSTROW=2,
	FIELDTERMINATOR='','',
	ROWTERMINATOR=''\n'')'
	exec sp_executesql @insertstatement
	insert into logs(filepath,status,message) values(@filepath,'Success','Bulk Data inserted successfully')
	end try
	begin catch
		insert into logs(filepath,status,message) values(@filepath,'Failed',ERROR_MESSAGE())
	end catch
end

exec proc_BulkInsetStudents 'C:\Training\Day4 - 08-05-2025\data.csv'

select * from logs

select * from students

-- Working With CTE (Common Table Expression)

select * from authors

with cte_authors as (
select au_id, concat(au_fname,au_lname) as author_name, state from authors
)

select * from cte_authors

-- create a sp that will take the page number and size as param and print the books

create or alter procedure proc_pageniation (@pageno int, @pagerecord int)
as
begin
declare @page int = @pageno, @pagesize int =@pagerecord;

with cte_pagination as
(
select title_id,title,price, ROW_NUMBER() over (order by price desc) as Row_Num from titles
)
select * from cte_pagination where Row_Num between((@page-1)*@pagesize+1) and (@page*@pagesize)
end

execute proc_pageniation 2,10

-- Pagination using offset

create procedure proc_offset(@start int, @end int)
as
begin
select title_id,title,price from titles order by price desc offset @start rows fetch next @end rows only
end

exec proc_offset 10,10


select title_id,title,price from titles order by price desc offset 10 rows fetch next 10 rows only


-- Working with functions
-- There is two type of function one is scalar value function and another one is table value function

/*
	Scalar Value Function
	Here we will use begin and end in the function.
	To return the value we will use the function inside the select query as below example.
*/
create function  fn_CalculateTax(@baseprice float, @tax float)
  returns float
  as
  begin
     return (@baseprice +(@baseprice*@tax/100))
  end

  select dbo.fn_CalculateTax(2992,10) as taxamt


/*
	Table Value Function
	Here we will not use begin and end in the function.
	To return the value we will use the select to return table value inside the function we have create.
*/

create function fn_tablevaludefunc(@min int)
returns table
as
return select title,price from titles where price>=@min


select * from dbo.fn_tablevaludefunc(2)



