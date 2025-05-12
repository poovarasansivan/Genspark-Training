/*
 Transaction : Online Retail Inventory and Order Management scenario
 Performing a transaction in the inventory management.
*/

-- Creating a sample table

CREATE TABLE products (
    product_id INT PRIMARY KEY,
    name VARCHAR(100),
    stock INT
);

CREATE TABLE orders (
    order_id INT PRIMARY KEY,
    product_id INT,
    customer_id INT,
    quantity INT
);

CREATE TABLE shipping_details (
    shipping_id SERIAL PRIMARY KEY,
    order_id INT,
    address VARCHAR(255)
);

CREATE TABLE email_logs (
    log_id SERIAL PRIMARY KEY,
    email VARCHAR(255),
    subject VARCHAR(255),
    sent BOOLEAN
);


-- Adding some products into the table

insert into products(product_id,name,stock) values(1,'Laptop',10),(2,'Mobile',15);

/*
Transaction Commands:
1. Begin
2. Commit
3. Savepoint
4. Rollback

In the below example we are going to simulate the transcation commands.
*/

begin;

-- Step 1: Update product stock
update products 
set stocks = stocks - 1 
where product_id = 1;

-- Step 2: Insert into orders
insert into orders (order_id, product_id, customer_id, quantity)
values (1, 1, 3001, 1);

-- Step 3: Savepoint before risky operations
savepoint order_point;

-- Step 4: Try shipping and email logs
-- Shipping
insert into shipping_details (order_id, address)
values (1, '123 ABC Street');

-- Email log
insert into email_logs (email, subject, sent)
values ('customer@example.com', 'Order Confirmation', TRUE);

-- Commit whole transaction
commit;
