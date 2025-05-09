/*
1. SELECT Queries
List all films with their length and rental rate, sorted by length descending.
Columns: title, length, rental_rate
*/

SELECT title, length, rental_rate
FROM film
ORDER BY length DESC;

/*
2. Find the top 5 customers who have rented the most films.
   Hint: Use the rental and customer tables.
*/

select cu.customer_id, concat(cu.first_name,' ',cu.last_name) as customer_name, count(*) as rental_count from rental ren 
join customer cu on cu.customer_id = ren.customer_id group by cu.customer_id,cu.first_name, cu.last_name limit 5

/*
3. Display all films that have never been rented.
   Hint: Use LEFT JOIN between film and inventory → rental.
*/

select fm.film_id, fm.title from film fm
left join inventory inv on inv.film_id = fm.film_id
left join rental ren on ren.inventory_id = inv.inventory_id
where ren.rental_id is null

/*
4. JOIN Queries
   List all actors who appeared in the film ‘Academy Dinosaur’.
   Tables: film, film_actor, actor
*/

select fm.title, concat(ac.first_name,' ',ac.last_name) as actor_name from film fm
join film_actor fma on fma.film_id = fm.film_id
join actor ac on ac.actor_id = fma.actor_id
where fm.title = 'Academy Dinosaur'

/*
5. List each customer along with the total number of rentals they made and the total amount paid.
   Tables: customer, rental, payment
*/

select cu.customer_id, concat(cu.first_name,' ',cu.last_name) as customer_name, count(p.rental_id) as total_rents, sum(p.amount) as total_amount
from customer cu
join rental ren on ren.customer_id = cu.customer_id 
join payment p on p.customer_id = cu.customer_id
group by cu.customer_id, customer_name

/*
6. CTE-Based Queries
   Using a CTE, show the top 3 rented movies by number of rentals.
   Columns: title, rental_count
*/

with cte_top3movies as
(
select fm.title, count(ren.rental_id) as rental_count from film fm 
join inventory ivn on ivn.film_id = fm.film_id
join rental ren on ren.inventory_id = ivn.inventory_id
group by fm.film_id order by rental_count desc limit 3
)
select * from cte_top3movies

/*
7. Find customers who have rented more than the average number of films.
   Use a CTE to compute the average rentals per customer, then filter.
*/

with rental_counts as
(
  select cu.customer_id, concat(cu.first_name,' ',cu.last_name) as customer_name, count(re.rental_id) as total_rentals from customer cu
  join rental re on re.customer_id = cu.customer_id
  group by cu.customer_id
),
average_rentals as (
select avg(total_rentals * 1.0) as avg_rentals from rental_counts
)
select rc.customer_id, rc.customer_name,rc.total_rentals,a.avg_rentals from rental_counts rc
join average_rentals a on rc.total_rentals>a.avg_rentals
order by rc.total_rentals desc;

/*
8. Function Questions
   Write a function that returns the total number of rentals for a given customer ID.
   Function: get_total_rentals(customer_id INT)
*/

drop function get_total_rentals

create function get_total_rentals(cust_id int)
returns int
as $$
declare 
	total_rentals int;
begin
	select customer_id, count(*) into total_rentals from rental where customer_id = cust_id group by customer_id;
	return total_rentals;
end
$$ language plpgsql;

select get_total_rentals(459);

/*
9. Stored Procedure Questions
   Write a stored procedure that updates the rental rate of a film by film ID and new rate.
   Procedure: update_rental_rate(film_id INT, new_rate NUMERIC)
*/

create or replace procedure update_rental_rate(p_film_id int, new_rate numeric)
language plpgsql
as $$
begin
 update film set rental_rate = new_rate where film_id = p_film_id;
end;
$$;

call update_rental_rate(1,2.00)
select * from film where film_id = 1

/*
10. Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).
    Procedure: get_overdue_rentals() that selects relevant columns.
*/

CREATE OR REPLACE PROCEDURE get_overdue_rentals()
LANGUAGE plpgsql
AS $$
DECLARE
  rec RECORD; 
BEGIN
  RAISE NOTICE 'Overdue Rentals:';

  FOR rec IN
    SELECT
      r.rental_id,
      c.first_name || ' ' || c.last_name AS customer_name,
      f.title AS film_title,
      r.rental_date
    FROM rental r
    JOIN customer c ON c.customer_id = r.customer_id
    JOIN inventory i ON i.inventory_id = r.inventory_id
    JOIN film f ON f.film_id = i.film_id
    WHERE r.return_date IS NULL
      AND r.rental_date < NOW() - INTERVAL '7 days'
  LOOP
    RAISE NOTICE 'Rental ID: %, Customer: %, Film: %, Rented on: %',
      rec.rental_id, rec.customer_name, rec.film_title, rec.rental_date;
  END LOOP;
END;
$$;
call get_overdue_rentals();
