
-- Cursor task
-- 1. Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.	

create table summary(
id serial primary key,
customer_name text,
total_rentals int
)

do $$
declare 
	cu_rental_rec record;
	cur_ren_cus cursor for
	select concat(cu.first_name,' ',cu.last_name) as customer_name, count(*) as total_rents from customer cu join rental r on r.customer_id = cu.customer_id
	group by cu.customer_id;
begin
	open cur_ren_cus;
	loop
	fetch cur_ren_cus into cu_rental_rec;
	exit when not found;

	insert into summary(customer_name,total_rentals) values(cu_rental_rec.customer_name,cu_rental_rec.total_rents);

	end loop;
	close cur_ren_cus;
end;
$$;

select * from summary;

-- 2. Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.

do $$
declare 
	film_cur_rec record;
	film_cat_cur cursor for
	select f.title from film f join film_category fc on f.film_id = fc.film_id join category c on fc.category_id = c.category_id
	join inventory inr on inr.film_id = f.film_id join rental r on r.inventory_id = inr.inventory_id
	where c.name = 'Comedy' group by f.film_id, f.title having count(r.rental_id) > 10;
begin
	open film_cat_cur;
	loop
	fetch film_cat_cur into film_cur_rec;
	exit when not found;

	raise notice 'Comedy Film: %',film_cur_rec.title;
	end loop;
	close film_cat_cur;
end;
$$;

-- 3. Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.

CREATE TABLE reports (
    report_id serial PRIMARY KEY,
    store_id INT UNIQUE,
    no_of_films INT
);

DO $$
DECLARE
    store_rec RECORD;
    film_count INT;

    store_cur CURSOR FOR
        SELECT store_id FROM store;
BEGIN
    OPEN store_cur;

    LOOP
        FETCH store_cur INTO store_rec;
        EXIT WHEN NOT FOUND;

        SELECT COUNT(DISTINCT i.film_id)
        INTO film_count
        FROM inventory i
        WHERE i.store_id = store_rec.store_id;

        INSERT INTO reports(store_id, no_of_films)
        VALUES (store_rec.store_id, film_count)
        ON CONFLICT (store_id) DO UPDATE SET no_of_films = EXCLUDED.no_of_films;

        RAISE NOTICE 'Store: %, Film Count: %', store_rec.store_id, film_count;
    END LOOP;

    CLOSE store_cur;
END;
$$;

SELECT * FROM reports


-- 4. Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.

CREATE TABLE incative_customers(
customer_id serial PRIMARY KEY,
customer_name text
)

DO $$
DECLARE
    cust RECORD;
    last_rent DATE;
BEGIN
    FOR cust IN
        SELECT c.customer_id, c.first_name, c.last_name, c.email
        FROM customer c
    LOOP
        SELECT MAX(r.rental_date)
        INTO last_rent
        FROM rental r
        WHERE r.customer_id = cust.customer_id;

        IF last_rent IS NULL OR last_rent < CURRENT_DATE - INTERVAL '6 months' THEN
            INSERT INTO inactive_customers (customer_id, first_name, last_name, email, last_rental_date)
            VALUES (cust.customer_id, cust.first_name, cust.last_name, cust.email, last_rent);
        END IF;
    END LOOP;
END;
$$;


-- Transactions 
-- 1. Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.

BEGIN;
	INSERT INTO customer(customer_id,store_id,first_name,last_name,email,address_id,activebool,create_date, last_update,active)
	VALUES(606,1,'Poovarasan','S','p@gmail.com',102,true,CURRENT_DATE,NOW(),1);

	INSERT INTO rental(rental_date,inventory_id,customer_id,return_date,staff_id,last_update) 
	VALUES(CURRENT_DATE,1132,606,NULL,1,NOW());

	INSERT INTO payment(payment_id,customer_id,staff_id,rental_id,amount,payment_date)
	VALUES(14567,606,1,11111,1.89,NOW());
COMMIT;


SELECT * FROM customer WHERE customer_id = 606

-- 2. Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.

BEGIN;
	INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
	VALUES (2, 1, 1000, 5.99, NOW());
	
	UPDATE rental
	SET return_date = NOW()
	WHERE rental_id = -9999; 
	
	DO $$
	BEGIN
	  IF NOT EXISTS (SELECT 1 FROM rental WHERE rental_id = -9999) THEN
	    RAISE EXCEPTION 'Invalid rental_id. Rolling back.';
		ROLLBACK;
	  END IF;
	END $$;
	
	UPDATE customer
	SET last_update = NOW()
	WHERE customer_id = 2;

COMMIT;

-- 3. Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.

BEGIN;
	INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date) VALUES(341,1,1001,1.5,NOW());

	SAVEPOINT payment1;

	INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date) VALUES(342,1,1002,2.5,NOW());

	SAVEPOINT payment2;

	INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date) VALUES(343,1,1003,1.5,NOW());

	ROLLBACK TO SAVEPOINT payment1;

COMMIT;


-- 4. Perform a transaction that transfers inventory from one store to another (delete + insert) safely.

FOREIGN KEY (inventory_id) REFERENCES inventory(inventory_id) ON DELETE CASCADE

BEGIN;

	SAVEPOINT before_change;
	
	DELETE FROM inventory WHERE inventory_id = 124 AND store_id = 1;

	INSERT INTO inventory (film_id,store_id,last_update) VALUES ((SELECT film_id FROM inventory WHERE inventory_id = 124),2,NOW());

COMMIT;

-- 5. Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.

BEGIN;

	DELETE FROM payment
	WHERE customer_id = 601;
	
	DELETE FROM rental
	WHERE customer_id = 601;
	
	DELETE FROM customer
	WHERE customer_id = 601;

COMMIT;

SELECT * FROM customer WHERE customer_id = 601 -- null


-- TRIGGERS
-- 1. Create a trigger to prevent inserting payments of zero or negative amount.

CREATE OR REPLACE FUNCTION prevent_zero()
RETURNS TRIGGER
AS $$
BEGIN
		IF 	new.amount <= 0 THEN
		RAISE EXCEPTION 'Payment Amount should be greater than 0';
	END IF;
	RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER trg_prevent_zero
BEFORE INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION prevent_zero();

INSERT INTO payment (customer_id,staff_id,rental_id,amount,payment_date) VALUES (341,1,1005,0,NOW());

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date) VALUES (341, 1, 1005, 2.5, NOW());


-- 2. Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.

CREATE OR REPLACE FUNCTION film_tbl_update()
RETURNS TRIGGER
AS $$
BEGIN
	IF NEW.title IS DISTINCT FROM OLD.title OR
       NEW.rental_rate IS DISTINCT FROM OLD.rental_rate THEN
        NEW.last_update := NOW();
        RAISE NOTICE 'Last update date is updated for film id: %', NEW.film_id;
    END IF;
	RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER film_update
BEFORE UPDATE ON film
FOR EACH ROW 
EXECUTE FUNCTION film_tbl_update();

UPDATE film SET rental_rate = 5.00 WHERE film_id = 8;

-- 3. Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.

CREATE TABLE rental_log (
log_id SERIAL PRIMARY KEY,
film_id INT,
rental_count INT,
log_date TIMESTAMP DEFAULT NOW()
)

CREATE FUNCTION insert_rental_log()
RETURNS TRIGGER
AS $$
DECLARE 
	v_film_id INT;
	v_count INT;
BEGIN
	SELECT film_id INTO v_film_id FROM inventory WHERE inventory_id = NEW.inventory_id;

	SELECT COUNT(*) INTO v_count FROM rental r JOIN inventory i on r.inventory_id = i.inventory_id 
	WHERE i.film_id = v_film_id AND r.rental_date >= NOW() - INTERVAL '7 days';

	IF v_count > 3 THEN
	INSERT INTO rental_log (film_id, rental_count) VALUES (v_film_id, v_count);
	END IF;
	RETURN NEW;
END;
$$
LANGUAGE plpgsql;


CREATE TRIGGER trg_rental_log
AFTER INSERT ON rental
FOR EACH ROW 
EXECUTE FUNCTION insert_rental_log();

INSERT INTO rental (rental_date,inventory_id,customer_id,return_date,staff_id,last_update) 
VALUES (NOW(),1525,121,NULL,1,NOW());

SELECT * FROM rental_log;
