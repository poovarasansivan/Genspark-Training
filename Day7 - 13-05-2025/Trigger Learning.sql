select * from products
select * from products_stock_log

create table products_stock_log(
log_id serial primary key,
product_name varchar(100),
old_stock int,
new_stock int,
updated_at timestamp default current_timestamp
)

/*
 Trigger Example that inserts a log where ever the product details is updated along with product name, old and new stocks.
*/

create or replace function products_update()
returns trigger
as $$
begin
	insert into products_stock_log(product_name,old_stock,new_stock,updated_at) values('Laptop',OLD.stock,NEW.stock,current_timestamp);
	return new;
end;
$$
language plpgsql;

drop function products_update();

create trigger trg_products_update
before update
on products
for each row
execute function products_update();

update products set stock = stock + 1 where product_id = 1;

/*
	Trigger example with dynamic parameters. with after update function 
*/

create table audit_log
(audit_id serial primary key,
table_name text,
field_name text,
old_value text,
new_value text,
updated_date Timestamp default current_Timestamp)


create or replace function Update_Audit_log()
returns trigger 
as $$
declare 
   col_name text := TG_ARGV[0];
   tab_name text := TG_ARGV[1];
   o_value text;
   n_value text;
begin
    EXECUTE FORMAT('select ($1).%I::TEXT', COL_NAME) INTO O_VALUE USING OLD;
    EXECUTE FORMAT('select ($1).%I::TEXT', COL_NAME) INTO N_VALUE USING NEW;
	if o_value is distinct from n_value then
		Insert into audit_log(table_name,field_name,old_value,new_value,updated_date) 
		values(tab_name,col_name,o_value,n_value,current_Timestamp);
	end if;
	return new;
end;
$$ language plpgsql

create trigger trg_log_customer_email_Change
after update
on customer
for each row
execute function Update_Audit_log('last_name','customer');

update customer set last_name = 'Johnsons' where customer_id = 2

select * from audit_log;

select * from customer order by customer_id