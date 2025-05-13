/* 
 CURSOR
	A cursor is a database object used to retrieve a small number of rows at a time from a larger result set.
	It's especially useful when dealing with large data or row-by-row processing in stored procedures.
	We are able to use cursor to fetch a data from a table and insert into a new table.
	Able to write a cursor inside the cursor that is called nested cursor.
	
 Why to use cursor?
 	To loop through a result set row by row.
    To reduce memory usage (you don’t load the entire result set at once).
    To handle complex row-level logic in PL/pgSQL blocks.
*/


do $$
declare 
	rental_record record;
	rental_cursor cursor for
		select r.rental_id, c.first_name, c.last_name, r.rental_date
        from rental r
        join customer c on r.customer_id = c.customer_id
        order by r.rental_id;
begin
	open rental_cursor;
	loop
	fetch rental_cursor into rental_record;
	exit when not found;

	raise notice 'rental id: %, customer: % %, date: %',
                  	 rental_record.rental_id,
                     rental_record.first_name,
                     rental_record.last_name,
                     rental_record.rental_date;
	end loop;

	close rental_cursor;
end;
$$;

create table rental_tax_log (
    rental_id int,
    customer_name text,
    rental_date timestamp,
    amount numeric,
    tax numeric
);

do $$
declare
    rec record;
    cur cursor for
        select r.rental_id, 
               c.first_name || ' ' || c.last_name as customer_name,
               r.rental_date,
               p.amount
        from rental r
        join payment p on r.rental_id = p.rental_id
        join customer c on r.customer_id = c.customer_id;
begin
    open cur;

    loop
        fetch cur into rec;
        exit when not found;

        insert into rental_tax_log (rental_id, customer_name, rental_date, amount, tax)
        values (
            rec.rental_id,
            rec.customer_name,
            rec.rental_date,
            rec.amount,
            rec.amount * 0.10
        );
    end loop;

    close cur;
end;
$$;

select * from rental_tax_log


-- Creating a nested cursor.

create table flimdetails(
id serial primary key,
film_name text,
actor_name text
)


select * from film
select * from film_actor
select * from actor


do $$
declare
	film_rec record;
	actor_rec record;

	film_cur cursor for
	select title from film;

	act_cur cursor for
	select a.first_name,a.last_name from film f join film_actor fa on fa.film_id = f.film_id
	join actor a on a.actor_id = fa.actor_id;

begin
	open film_cur;
	loop
	fetch film_cur into film_rec;
	exit when not found;

	open act_cur;
	loop
	fetch act_cur into actor_rec;
	exit when not found;

	insert into flimdetails(film_name,actor_name) values (film_rec.title,actor_rec.first_name);

	end loop;
	close act_cur;

	end loop;
	close film_cur;
end;
$$;

