-- Coursors 
/* 
	We will use cursors when we need to fetch data row by row processing of data
	Useful for stored procedures
	Cursors can be slow and resource-intensive, especially on large datasets.
*/

-- Cursor Example
select * from titles

declare @title varchar(200), @type varchar(100);

declare title_cursor cursor for
select title, type from titles

open title_cursor
fetch next from title_cursor into @title, @type

while @@FETCH_STATUS =0
begin
	print 'title '+ @title+' '+'type '+@type
	fetch next from title_cursor into @title, @type
end

close title_cursor
deallocate title_cursor

/*
	Transactions
	 All Transaction must succeed together — or fail together.
	 Transaction follows ACID properties to maintain data integrity or consistency

	 BEGIN TRAN
	 It starts the transaction and executes the query. based on the query success or failer rollback or commit will work together.

	 Commit
	 COMMIT saves all the changes made during the transaction to the database permanently.
	
	 Rollback
	 ROLLBACK undoes all changes made during the transaction.
	 Used when there is an error in the sql statements
*/

-- Example for Commit

BEGIN TRANSACTION

UPDATE students SET name='Jayaprakash P' WHERE id='211CS169'

COMMIT


BEGIN TRANSACTION
BEGIN TRY
UPDATE students SET name='Praveenraja P' WHERE id='211CS248';
END TRY
BEGIN CATCH
	PRINT 'Error'+ERROR_MESSAGE();
	ROLLBACK
END CATCH

select * from students
