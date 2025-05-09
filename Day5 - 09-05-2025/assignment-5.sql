/*
1. Write a cursor that loops through all films and prints titles longer than 120 minutes.
*/
DO $$
DECLARE 
  TITLE_CURSOR CURSOR FOR
    SELECT TITLE FROM FILM WHERE LENGTH > 120;
  REC RECORD;
BEGIN
  OPEN TITLE_CURSOR;
  LOOP
    FETCH TITLE_CURSOR INTO REC;
    EXIT WHEN NOT FOUND;
    RAISE NOTICE 'Film: %', REC.TITLE;
  END LOOP;
  CLOSE TITLE_CURSOR;
END;
$$;

/*
2. Create a cursor that iterates through all customers and counts how many rentals each made.
*/
DO $$
DECLARE 
  CUSTOMER_CURSOR CURSOR FOR
    SELECT CU.CUSTOMER_ID, CONCAT(CU.FIRST_NAME, ' ', CU.LAST_NAME) AS CUSTOMER_NAME, COUNT(*) AS TOTAL_RENTALS 
    FROM CUSTOMER CU 
    JOIN RENTAL R ON R.CUSTOMER_ID = CU.CUSTOMER_ID 
    GROUP BY CU.CUSTOMER_ID;
  REC RECORD;
BEGIN
  OPEN CUSTOMER_CURSOR;
  LOOP
    FETCH CUSTOMER_CURSOR INTO REC;
    EXIT WHEN NOT FOUND;
    RAISE NOTICE 'Customer ID: %, Name: %, Total Rentals: %', REC.CUSTOMER_ID, REC.CUSTOMER_NAME, REC.TOTAL_RENTALS;
  END LOOP;
  CLOSE CUSTOMER_CURSOR;
END;
$$;

/*
3. Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.
*/
DO $$
DECLARE 
  REC RECORD;
  FILM_CURSOR CURSOR FOR
    SELECT F.FILM_ID 
    FROM FILM F 
    JOIN INVENTORY I ON F.FILM_ID = I.FILM_ID 
    JOIN RENTAL R ON R.INVENTORY_ID = I.INVENTORY_ID 
    GROUP BY F.FILM_ID 
    HAVING COUNT(R.RENTAL_ID) < 5;
BEGIN
  OPEN FILM_CURSOR;
  LOOP
    FETCH FILM_CURSOR INTO REC;
    EXIT WHEN NOT FOUND;
    UPDATE FILM SET RENTAL_RATE = RENTAL_RATE + 1 WHERE FILM_ID = REC.FILM_ID;
    RAISE NOTICE 'Updated rental rate for FILM_ID %', REC.FILM_ID;
  END LOOP;
  CLOSE FILM_CURSOR;
END;
$$;

/*
4. Create a function using a cursor that collects titles of all films from a particular category.
*/
CREATE FUNCTION GET_FILM_NAME_BYCATEGORY(CAT_NAME VARCHAR(200))
RETURNS TABLE(TITLE TEXT)
LANGUAGE PLPGSQL
AS $$
DECLARE 
  REC RECORD;
  FILM_CURSOR CURSOR FOR
    SELECT F.TITLE 
    FROM FILM F 
    JOIN FILM_CATEGORY FC ON FC.FILM_ID = F.FILM_ID 
    JOIN CATEGORY C ON FC.CATEGORY_ID = C.CATEGORY_ID 
    WHERE C.NAME = CAT_NAME;
BEGIN
  OPEN FILM_CURSOR;
  LOOP
    FETCH FILM_CURSOR INTO REC;
    EXIT WHEN NOT FOUND;
    TITLE := REC.TITLE;
    RETURN NEXT;
  END LOOP;
  CLOSE FILM_CURSOR;
END;
$$;

SELECT * FROM GET_FILM_NAME_BYCATEGORY('Action');

/*
5. Loop through all stores and count how many distinct films are available in each store using a cursor.
*/
DO $$
DECLARE 
  REC RECORD;
  STORE_CURSOR CURSOR FOR SELECT STORE_ID FROM STORE;
  FILM_COUNT INT;
BEGIN
  OPEN STORE_CURSOR;
  LOOP
    FETCH STORE_CURSOR INTO REC;
    EXIT WHEN NOT FOUND;
    SELECT COUNT(DISTINCT FILM_ID) INTO FILM_COUNT FROM INVENTORY WHERE STORE_ID = REC.STORE_ID;
    RAISE NOTICE 'Store ID: %, Distinct Films: %', REC.STORE_ID, FILM_COUNT;
  END LOOP;
  CLOSE STORE_CURSOR;
END;
$$;

/*
6. Write a trigger that logs whenever a new customer is inserted.
*/
CREATE TABLE CUSTOMER_LOGS (
  LOG_ID SERIAL PRIMARY KEY,
  CUSTOMER_ID INTEGER,
  LOG_TIME TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE FUNCTION LOG_NEW_CUSTOMER()
RETURNS TRIGGER
LANGUAGE PLPGSQL
AS $$
BEGIN
  INSERT INTO CUSTOMER_LOGS(CUSTOMER_ID) VALUES (NEW.CUSTOMER_ID);
  RETURN NEW;
END;
$$;

CREATE TRIGGER AFTER_CUSTOMER_INSERTION
AFTER INSERT ON CUSTOMER
FOR EACH ROW
EXECUTE FUNCTION LOG_NEW_CUSTOMER();

INSERT INTO CUSTOMER(CUSTOMER_ID, STORE_ID, FIRST_NAME, LAST_NAME, EMAIL, ADDRESS_ID, ACTIVEBOOL, CREATE_DATE, LAST_UPDATE, ACTIVE)
VALUES (600, 1, 'POOVARASAN', 'S', 'POO@GMAI.COM', 603, TRUE, '2025-05-09', NOW(), 1);

SELECT * FROM CUSTOMER_LOGS;

/*
7. Create a trigger that prevents inserting a payment of amount 0.
*/
CREATE FUNCTION PREVENT_ZERO_PAY()
RETURNS TRIGGER
LANGUAGE PLPGSQL
AS $$
BEGIN
  IF NEW.AMOUNT = 0 THEN
    RAISE EXCEPTION 'Payment amount should be greater than 0';
  END IF;
  RETURN NEW;
END;
$$;

CREATE TRIGGER BLOCK_ZEROPAY
BEFORE INSERT ON PAYMENT
FOR EACH ROW
EXECUTE FUNCTION PREVENT_ZERO_PAY();

INSERT INTO PAYMENT(PAYMENT_ID, CUSTOMER_ID, STAFF_ID, RENTAL_ID, AMOUNT) 
VALUES (32099, 341, 2, 16100, 0);

/*
8. Set up a trigger to automatically set last_update on the film table before update.
*/
CREATE FUNCTION LAST_UPDATE_FILM()
RETURNS TRIGGER
LANGUAGE PLPGSQL
AS $$
BEGIN
  NEW.LAST_UPDATE := NOW();
  RETURN NEW;
END;
$$;

CREATE TRIGGER SET_LAST_UPDATE
BEFORE UPDATE ON FILM
FOR EACH ROW
EXECUTE FUNCTION LAST_UPDATE_FILM();

UPDATE FILM SET RENTAL_RATE = RENTAL_RATE + 0.50 WHERE FILM_ID = 1;

SELECT * FROM FILM WHERE FILM_ID = 1;

/*
9. Create a trigger to log changes in the inventory table (insert/delete).
*/

CREATE TABLE inventory_logs (
  log_id serial PRIMARY KEY,
  inventory_id INT,
  log_message VARCHAR(200),
  log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE OR REPLACE FUNCTION log_inventory()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
DECLARE 
  action_text TEXT;
  inv_id INT;
BEGIN
  action_text := TG_OP;
  IF TG_OP = 'INSERT' THEN
    inv_id := NEW.inventory_id;
  ELSIF TG_OP = 'DELETE' THEN
    inv_id := OLD.inventory_id;
  END IF;
  INSERT INTO inventory_logs(inventory_id, log_message)
  VALUES (inv_id, 'Inventory ' || action_text || ' operation');
  RETURN NULL;
END;
$$;


CREATE TRIGGER update_inventory_logs
AFTER INSERT OR DELETE ON inventory
FOR EACH ROW
EXECUTE FUNCTION log_inventory();


INSERT INTO inventory (inventory_id, film_id, store_id, last_update)
VALUES (4582, 123, 1, NOW());

DELETE FROM inventory WHERE inventory_id = 4582;

SELECT MAX(inventory_id) FROM inventory

SELECT * FROM inventory_logs

/*
10. Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.
*/

CREATE OR REPLACE FUNCTION prevent_rental_if_debt_high()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
DECLARE
  total_paid NUMERIC;
BEGIN
  SELECT COALESCE(SUM(amount), 0)
  INTO total_paid
  FROM payment
  WHERE customer_id = NEW.customer_id;

  IF total_paid < 50 THEN
    RAISE EXCEPTION 'Customer % has outstanding payments and cannot rent more films.', NEW.customer_id;
  END IF;

  RETURN NEW;
END;
$$;

CREATE TRIGGER block_rental_if_debt_high
BEFORE INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION prevent_rental_if_debt_high();


SELECT customer_id, SUM(amount) AS total_paid
FROM payment
GROUP BY customer_id
HAVING SUM(amount) < 50;

INSERT INTO rental (rental_id, rental_date, inventory_id, customer_id, return_date, staff_id, last_update)
VALUES (16049, NOW(), 1, 110, NULL, 1, NOW());

/*
11. Write a transaction that inserts a customer and an initial rental in one atomic operation.
*/

DO $$
DECLARE
  new_customer_id INTEGER;
BEGIN
  INSERT INTO customer (store_id, first_name, last_name, email, address_id, activebool, create_date, last_update, active)
  VALUES (1, 'John', 'Doe', 'john.doe@example.com', 1, TRUE, CURRENT_DATE, NOW(), 1)
  RETURNING customer_id INTO new_customer_id;

  INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id, last_update)
  VALUES (NOW(), 1, new_customer_id, 1, NOW());
EXCEPTION
  WHEN OTHERS THEN
    RAISE NOTICE 'Transaction failed, rolling back';
END;
$$;

/*
12. Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.
*/

DO $$
DECLARE
  fake_film_id INT := 10000;
BEGIN
  UPDATE film SET rental_rate = rental_rate + 1 WHERE film_id = 1;
  INSERT INTO inventory (film_id, store_id, last_update)
  VALUES (fake_film_id, 1, NOW());
  RAISE NOTICE 'Transaction completed successfully';

EXCEPTION
  WHEN OTHERS THEN
    RAISE NOTICE 'Transaction failed. Rolling back.';
    RAISE;
END;
$$;

/*
13. Create a transaction that transfers an inventory item from one store to another.
*/

DO $$
DECLARE 
	inv_id INT := 1000;
	from_store_id INT := 1;
	to_store_id INT := 2;
BEGIN
	IF NOT EXISTS (
	SELECT 1 FROM inventory 
	WHERE inventory_id = inv_id and store_id = from_store_id
	) THEN
	RAISE EXCEPTION 'Inventory item % not found in store %', inv_id, from_store_id;
	END IF;

	UPDATE inventory
	SET store_id = to_store_id,
	last_update = NOW() where inventory_id = inv_id;
	 RAISE NOTICE 'Inventory item % successfully transferred from store % to store %',
    inv_id, from_store_id, to_store_id;
END;
$$;

SELECT * FROM inventory

/*
14. Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.
*/

BEGIN;
	UPDATE payment SET amount = amount + 1, payment_date=NOW() WHERE payment_id = 17503;
	SAVEPOINT sp1;

	UPDATE payment SET amount = amount + 5, payment_date=NOW() WHERE payment_id = 17504;
	ROLLBACK TO SAVEPOINT sp1;

	UPDATE payment SET amount = amount + 2, payment_date=NOW() WHERE payment_id = 17505;
COMMIT;


SELECT * FROM payment WHERE payment_id BETWEEN 17503 AND 17505;

/*
15. Write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.
*/

BEGIN;
	DELETE FROM payment WHERE customer_id = 1;

	DELETE FROM rental WHERE customer_id = 1;

	DELETE FROM customer WHERE customer_id = 1;
COMMIT;

SELECT * FROM customer WHERE customer_id BETWEEN 1 and 5
