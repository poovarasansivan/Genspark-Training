/*
Locking Mechanism

 PostgreSQL automatically applies locks, but we can control it manually when it needed.

MVCC VS Locks

MVCC allows readers and writers to work together without blocking.
Locks are needed when multiple writers try to touch the same row or table.

Simple Rule of Locks

Readers don’t block each other.
Writers block other writers on the same row.

Types of Locks

1. Explict/Implict ROW level locks
	- Lock specific rows to prevent concurrent updates.
	- Only the selected rows are locked.
	- SELECT ... FOR UPDATE or FOR UPDATE.
	- Postgresql have both explict and implict row level locks. Implict lock is the default behaviour of Postgresql.
	- Implict locks appear when we execute DML statements like below.
	- Update, Select...for update, delete.
*/

-- Examples of implict(default behaviour of Postgresql) ROW LEVEL Locks
-- Consider there is a two transaction A and B, both this transaction tries to update the stock in the products table.

/*
 In the above example when we run the transaction A will lock the row with product id 1, so when we run the transaction B it will wait until the 
 transaction B either should be commit or rollback.
*/

-- Trans A
BEGIN;
UPDATE products SET stock = stock + 1 WHERE product_id = 1; -- Trans A locks this product details row.

-- Trans B: Tries to update the same row used by a Trans A.
BEGIN;
UPDATE products SET stock = stock - 1 WHERE product_id = 1;


/*
 Table Level Locks
 
 - Lock the entire table to control concurrent access.
 - COMMAND: LOCK TABLE table_name IN lock_mode
 - PostgreSQL automatically acquires the appropriate table lock when running operations like SELECT, INSERT, UPDATE, etc.

 Modes In Table level locks

 1. ACCESS SHARE 2. ROW SHARE 3. ROW EXCLUSIVE 4. SHARE UPDATE EXCLUSIVE 5. SHARE 6. SHARE ROW EXCLUSIVE 7. EXCLUSIVE 8. ACCESS EXCLUSIVE
*/

-- ACCESS SHARE
-- Allows read and write operation
-- Allows other SELECTS, even INSERT/DELETE at the same time.
-- It only products schema changes.

BEGIN;
LOCK TABLE products IN ACCESS SHARE MODE;

-- 2. ROW SHARE MODE
-- It locks selected rows to prevent other transactions from modifying them until the transaction ends.

BEGIN;
LOCK TABLE products
IN ROW SHARE MODE;

-- 3. ROW EXCLUSIVE
-- Allows concurrent reads and writes but prevents other transactions from acquiring certain higher-level locks.
-- Blocks writes (INSERT, UPDATE, DELETE) but allows reads (SELECT)

-- Trans A
BEGIN;
LOCK TABLE products
IN EXCLUSIVE MODE;
COMMIT;

-- Trying to insert a new product but it has been locked by the transaction A. Only able to insert into the able after the trans A release the lock.
insert into products(product_id,name,stock,price) values(3,'Table',10,2000);


-- 4. SHARE UPDATE EXCLUSIVE
-- Allows concurrent reads and writes but prevents schema changes and certain maintenance operations.
-- Blocks everything, Used by ALTER TABLE, DROP TABLE, TRUNCATE, 
-- Internally used by DDL.

-- TRANS A
BEGIN;
LOCK TABLE products IN ACCESS EXCLUSIVE MODE;

COMMIT;

-- B
SELECT * FROM products;
-- B will wait until A commits or rollbacks.

-- 5. SHARE
-- Allows concurrent reads but blocks writes.
-- Allowed in other sessions:
-- SELECT * FROM products;

-- Blocked in other sessions: INSERT, UPDATE, DELETE

BEGIN;
LOCK TABLE products IN SHARE MODE;


-- 6. SHARE ROW EXCLUSIVE
-- Allows concurrent reads but prevents other transactions from acquiring certain locks that could lead to conflicting modifications.
-- Allowed: SELECT from another session
-- Blocked: INSERT, UPDATE, DELETE from other sessions
-- Blocked: Any operation that takes SHARE/EXCLUSIVE lock

BEGIN;
LOCK TABLE products IN SHARE ROW EXCLUSIVE MODE;

-- 7. EXCLUSIVE LOCK
-- Prevents other transactions from acquiring locks that would allow data modifications.
-- Allowed: SELECT from other sessions
-- Blocked: INSERT, UPDATE, DELETE, ALTER TABLE, CREATE INDEX, etc.

BEGIN;
LOCK TABLE products IN EXCLUSIVE MODE;

-- 8. ACCESS EXCLUSIVE LOCK
-- Ensures that the transaction has exclusive access to the table, blocking all other operations.
-- Blocks everything: SELECT, INSERT, UPDATE, DELETE, etc.

BEGIN;
LOCK TABLE products IN ACCESS EXCLUSIVE MODE;


/*
DEADLOCK
  - It occurs when two or more transactions waiting for the same row or table that first transaction acquired the lock. 
  - All the transactions except the transaction A will be at waiting state until the A transaction release the lock by commit or rollback the transaction.
  - Below example shows that both transaction A and B tries to update the same product and catch in dead lock.
*/

-- Trans A
BEGIN;
	UPDATE products SET stock = stock + 1 WHERE product_id = 1;

-- Trans B
BEGIN;
	UPDATE products SET stock = stock + 1 WHERE product_id = 2;

-- Trans A
BEGIN;
	UPDATE products SET stock = stock - 1 WHERE product_id = 2
	
-- Trans B
BEGIN;
	UPDATE products SET stock = stock - 1 WHERE product_id = 1;


SELECT * FROM products

/*
-- Learning Transaction Tasks	
2. Write a query using SELECT...FOR UPDATE and check how it locks row.
3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.
4. Use pg_locks query to monitor active locks.

*/

-- 1. Try two concurrent updates to same row → see lock in action.

SELECT * FROM products;

-- Trans A

BEGIN;
	UPDATE products SET price = 25000 WHERE product_id = 1;

-- Trans B
BEGIN;
	UPDATE products SET price = 20000 WHERE product_id = 1;

-- 2. Write a query using SELECT...FOR UPDATE and check how it locks row.

-- Trans A
BEGIN;
	SELECT * FROM products WHERE product_id =1 FOR UPDATE; -- Acquired the lock

-- Trans B
BEGIN;
	UPDATE products SET price = 15000 WHERE product_id = 1;

-- 3. Intentionally create a deadlock and observe PostgreSQL cancel one transaction.

BEGIN;
UPDATE products SET price = 21000 WHERE product_id = 1;
-- Now the row with product_id = 1 is locked by trans A

UPDATE products SET price = 17000 WHERE product_id = 2;

BEGIN;
UPDATE products SET price = 16000 WHERE product_id = 2;
-- Now the row with product_id = 2 is locked by trans B

UPDATE products SET price = 22000 WHERE product_id = 1;

COMMIT;

ABORT;

-- 4. Use pg_locks query to monitor active locks.

SELECT pid, relation::regclass, mode, granted
FROM pg_locks
WHERE relation IS NOT NULL;
SELECT * FROM products
