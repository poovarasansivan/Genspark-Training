/*
Transactions : Concurrency and Locking

ACID Properties of Transactions:
1. Atomicity
2. Consistency
3. Isolation
4. Durability

Basic Transaction Commands

1. Begin or Start Transactions - Starts a new transactions
2. RollBack - Undoes all changes made in the transaction
3. Commit - Saves all changes made in the transaction
4. Save Point - Sets a point within a transaction to roll back to partially
*/

create table accounts(
account_no serial primary key,
account_holder varchar(100),
account_balance numeric(10,2)
)

select * from accounts

insert into accounts(account_holder,account_balance) values('Alice',5000),('Bob',3000)

-- Example 1 for transaction that transfers money from one to another. Performs transaction and commit.

begin transaction;
	update accounts set account_balance = account_balance - 500 where account_holder = 'Alice';
	update accounts set account_balance = account_balance + 500 where account_holder = 'Bob';
commit;

select * from accounts

-- Example 2: Rollback on error. Simulation of rollback by making a mistakes in the query statment
-- In the below query amount trasfer is failed because of incorrect reciever and debited amount is again added to the alice account 
-- with help of rollback

begin;
	update accounts set account_balance = account_balance - 100 where account_holder = 'Alice';
	update account set account_balance = account_balance + 100 where account_holder = 'Bob';
rollback;

/*
	Example 3: Savepoint : Partial Rollback
	In this example we are using a two accounts to tranfer amount with savepoint.
	SAVEPOINT marks a safe rollback point.
	Below example initially getting the amount from alice and try to update it on bob but due to mistake of table name we are again rollback to 
	savepoint to avoid the complete changes removeable.
*/

begin;
 update accounts set account_balance = account_balance - 500 where account_holder = 'Alice';
 savepoint trans1;
 update account set account_balance = account_balance + 500 where account_holder = 'Bob';
 rollback to trans1;
 update accounts set account_balance = account_balance + 500 where account_holder = 'Bob';
commit;

abort;

select * from accounts;

/*
	Example 4: RAISE NOTICE with Conditional ROLLBACK
	In this example we are going to use a conditional based rollback.
	Initially it check for enough balance if not it will rasie a notice and rollback by updating the debited amount to the acc1.
*/

begin;
do $$
declare current_balance numeric;
begin
	select account_balance into current_balance from accounts where account_holder = 'Alice';

	if current_balance >=3000 then
	update accounts set account_balance = account_balance - 3000 where account_holder = 'Alice';
	update accounts set account_balance = account_balance + 3000 where account_holder = 'Bob';
	else 
	raise notice 'Insufficent fund';
	rollback;
	end if;
end;
$$

select * from accounts;


/*
Best Practices of Transactions

1. Keep transactions short -> Improve Concurreny and Reduce Locking
2. Use savepoints for complex flows -> Safer partial rollbacks
3. Log failed trans for audits -> Traceability and Degugging
4. Avoid user inputs during transactions -> Prevent long trans
5. In production code, always wrap db ops inside try-catch with explicit commit and rollback.


Concurrency
PostgreSQL handles concurrency using:

1. MVCC (Multi-Version Concurrency Control):
	MVCC allows multiple versions of the same data (rows) to exist temporarily,
	so readers and writers don't block each other.
	Readers don't block writers and Writers don't block readers.
*/

-- Example of MVCC : Two transactions one is reading and other is updating.

begin;
	select account_balance from accounts where account_holder = 'Alice'; -- Balance will be 2500.

begin;
	update accounts set account_balance = account_balance + 500 where account_holder = 'Alice'; -- Adds 500rs

select * from accounts

/*
2. Isolation Levels : 4 --> Concurrency
   1. READ UNCOMMITTED -> PSQL not supported
   2. READ COMMITTED -> Default; MVCC
   MVCC is ACID-Compliant.
   Read Committed is powered by MVCC.
   3. Repeatable Read -> Ensures repeatabe reads
   4. Serializable -> Full isolation (safe but slow, no dirty reads, no lost updates, no repeatable reads, no phantom reads)
*/

-- Repeatable Read
-- Trans A
begin isolation level repeatable read;
select account_balance from accounts where account_holder = 'Alice'; -- 2500

-- Trans B
begin;
	update accounts set account_balance = account_balance + 500 where account_holder = 'Alice';
commit;

-- Trans A

select account_balance from accounts where account_holder = 'Alice'; -- 3000
Commit;

select * from accounts


/*
Problems without proper Concurrency Control:
1. Inconsistent Reads/Dirty Reads: Reading Uncommitted data from another transaction, which might later disappear.
Transaction A updates a row but doesn’t commit yet.
Transaction B reads that updated row.
Transaction A rolls back the update.
Now Transaction B has read data that never officially existed — that’s a dirty read.

Why Dirty Reads Happen:
They occur in databases running at low isolation levels, particularly:
Read Uncommitted isolation level → allows dirty reads.
Higher isolation levels like Read Committed, Repeatable Read, or Serializable
prevent dirty reads but come with performance trade-offs (like more locks or slower concurrency).

*/

-- Example for Incosistency Reads/Dirty Reads
-- Trans A
begin;
	select account_balance from accounts where account_holder = 'Alice'; -- balance: 4000

-- Trans B
begin;
	update accounts set account_balance = account_balance + 100 where account_holder = 'Alice';
commit;

-- Trans A
begin;
	select account_balance from accounts where account_holder = 'Alice'; -- 4100
commit;

/*
2. Lost Update
Transaction A reads a record.
Transaction B reads the same record.
Transaction A updates and writes it back.
Transaction B (still holding the old value) writes back its version, overwriting A’s changes.
*/
-- Trans A
begin;
	select account_balance from accounts where account_holder = 'Alice'; -- 4100

-- Trans B
begin;
	select account_balance from accounts where account_holder = 'Alice'; -- 4100
	update accounts set account_balance = account_balance + 100 where account_holder = 'Alice'; -- 4100+100 = 4200
commit;

-- Trans A
update accounts set account_balance = account_balance - 200 where account_holder = 'Alice'; -- 4200 - 200 = 4000
commit;

/*
3. Non-Repeatable Reads

A transaction reads the same row twice but gets different results because another transaction modified and 
committed the data in between the two reads.

4. Phatom Reads
A transaction runs a query (e.g., SELECT) and gets a set of rows. Another transaction inserts new rows that match the same criteria, 
and the first transaction runs the query again — now the result set has new rows (phantoms).
*/

-- Example for Phatom Reads

-- Trans A
begin;
	select * from accounts where account_balance >=5000; -- Returns 1 row

-- Trans B
begin;
	insert into accounts(account_no,account_holder,account_balance) values(5,'Jhon',5000);

-- Trans A
	select * from accounts where account_balance >=5000; -- Returns 2 row, A new row appeared


select * from accounts;