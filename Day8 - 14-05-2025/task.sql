-- Create a stored procedure that inserts rental data on the primary server, and verify that changes replicate to the standby server. Add a logging mechanism to track each operation.
-- Create a table on the primary:

CREATE TABLE rental_log (
    log_id SERIAL PRIMARY KEY,
    rental_time TIMESTAMP,
    customer_id INT,
    film_id INT,
    amount NUMERIC,
    logged_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Write a stored procedure to: Insert a new rental log entry, Accept customer_id, film_id, amount as inputs 
-- Wrap logic in a transaction with error handling (BEGIN...EXCEPTION...END)

CREATE OR REPLACE PROCEDURE sp_add_rental_log(
    p_customer_id INT,
    p_film_id INT,
    p_amount NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO rental_log (rental_time, customer_id, film_id, amount)
    VALUES (CURRENT_TIMESTAMP, p_customer_id, p_film_id, p_amount);
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
END;
$$;

-- Creating a Rental log trigger table 

CREATE TABLE rental_log_update_audit (
    audit_id SERIAL PRIMARY KEY,
    old_log_id INT,
    old_rental_time TIMESTAMP,
    old_customer_id INT,
    old_film_id INT,
    old_amount NUMERIC,
    updated_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- Creating a trigger that updates the log on the rental_log_update_audit when ever the new log added inside the rental_log

CREATE OR REPLACE FUNCTION fn_log_rental_insert_update()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- For INSERT, log the new values
    IF TG_OP = 'INSERT' THEN
        INSERT INTO rental_log_update_audit (
            old_log_id,
            old_rental_time,
            old_customer_id,
            old_film_id,
            old_amount
        )
        VALUES (
            NEW.log_id,
            NEW.rental_time,
            NEW.customer_id,
            NEW.film_id,
            NEW.amount
        );

    -- For UPDATE, log the old values
    ELSIF TG_OP = 'UPDATE' THEN
        INSERT INTO rental_log_update_audit (
            old_log_id,
            old_rental_time,
            old_customer_id,
            old_film_id,
            old_amount
        )
        VALUES (
            OLD.log_id,
            OLD.rental_time,
            OLD.customer_id,
            OLD.film_id,
            OLD.amount
        );
    END IF;

    RETURN NEW;
END;
$$;


CREATE TRIGGER trg_rental_log_update
AFTER INSERT OR UPDATE ON rental_log
FOR EACH ROW
EXECUTE FUNCTION fn_log_rental_insert_update();


-- Calling the procedure on the primary server (port 5432):

CALL sp_add_rental_log(1, 100, 4.99);

-- On the standby (port 5433): Confirm that the new record appears in rental_log

SELECT * FROM rental_log ORDER BY log_id DESC LIMIT 1;

-- To view the trigger that inserted a log on rental_log_update_audit

SELECT * FROM rental_log_update_audit;

