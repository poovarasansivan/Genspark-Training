/*
1. Create a stored procedure to encrypt a given text
Task: Write a stored procedure sp_encrypt_text that takes a plain text input (e.g., email or mobile number) and returns an encrypted version using PostgreSQL's pgcrypto extension.
*/

create extension if not exists pgcrypto;

create or replace procedure proc_encrypt_text(p_text text)
language plpgsql
as $$
declare 
	v_encrypted BYTEA;
	v_key text := 'this-is-a-secret-key';
begin
	v_encrypted := pgp_sym_encrypt(p_text,v_key,'cipher-algo=aes256');
    raise notice 'Encrypted Text (Base64): %', encode(v_encrypted, 'base64');
end;
$$;

call proc_encrypt_text('poovarasan');

-- Function that returns the encryted email

create function proc_encrypt_email(p_text TEXT)
returns TEXT
language plpgsql
as $$
declare 
    v_encrypted BYTEA;
    v_key TEXT := 'this-is-a-secret-key';
begin
    v_encrypted := pgp_sym_encrypt(p_text, v_key, 'cipher-algo=aes256');
    return encode(v_encrypted, 'base64');
end;
$$;

/*
2. Create a stored procedure to compare two encrypted texts
Task: Write a procedure sp_compare_encrypted that takes two encrypted values and checks if they decrypt to the same plain text.
*/

create or replace procedure sp_compare_encrypted(p_encrypted1 BYTEA, p_encrypted2 BYTEA)
language plpgsql
as $$
declare 
	p_decrypted1 text;
	p_decrypted2 text;
	v_key text := 'this-is-a-secret-key';
begin
	p_decrypted1 := pgp_sym_decrypt(p_encrypted1, v_key);
	p_decrypted2 := pgp_sym_decrypt(p_encrypted2, v_key);

	if p_decrypted1 = p_decrypted2 then
	raise notice 'Match found for % !', p_decrypted1;
	else
	raise notice 'No Match found for % !',p_decrypted1;
	end if;
end;
$$;

-- 1. ww0ECQMCiZeZgT52LO9u0jsBz9bwaifWidVZrYlG9BtnNlekpYZtovug314T8sW1bZhUmeHLQysAqDkEmRb3L2ZL+isGdVpj+igU+Q== 
-- 2. ww0ECQMCUsh89NlHY1p30jsBWM5gfzbLKIsuSpgzSI7kx+KRSyqhzC7E8Q9AexAr7yEymyoqdNf5pT9M3gb33m76eAMb6mZgqowfpA==

call sp_compare_encrypted(
    decode('ww0ECQMCiZeZgT52LO9u0jsBz9bwaifWidVZrYlG9BtnNlekpYZtovug314T8sW1bZhUmeHLQysAqDkEmRb3L2ZL+isGdVpj+igU+Q==', 'base64'),
    decode('ww0ECQMCUsh89NlHY1p30jsBWM5gfzbLKIsuSpgzSI7kx+KRSyqhzC7E8Q9AexAr7yEymyoqdNf5pT9M3gb33m76eAMb6mZgqowfpA==', 'base64')
);


/*
3. Create a stored procedure to partially mask a given text
Task: Write a procedure sp_mask_text that:
*/

create or replace procedure sp_mask_text_proc(p_text TEXT)
language plpgsql
as $$
declare
    text_len INT := length(p_text);
	masked_text text;
begin
	if text_len <= 4 then
	masked_text := p_text;
	elseif text_len >4 then
    masked_text := left(p_text, 2) || repeat('*', text_len - 4) || right(p_text, 2);
	end if;
	raise notice 'Masked Text: %', masked_text;
end;
$$;

call sp_mask_text_proc('poovarasan');


create or replace function sp_mask_text(p_text text)
returns text
language plpgsql
as $$
declare
    text_len INT := length(p_text);
	v_masked text;
begin
	v_masked := left(p_text,2) || repeat('*',text_len - 4) || right(p_text,2);
	return v_masked;
end;
$$;

select sp_mask_text('poovarasan');

/*
4. Create a procedure to insert into customer with encrypted email and masked name
Task:

Call sp_encrypt_text for email

Call sp_mask_text for first_name

Insert masked and encrypted values into the customer table

*/

create table shop_customer(
customer_id serial primary key,
customer_name text,
customer_email text,
last_update timestamp default current_timestamp
);

create or replace procedure pro_insert_customer(p_customer_name text,p_customer_email text)
language plpgsql
as $$
declare
	en_email text;
	masked_name text;
begin
	en_email := proc_encrypt_email(p_customer_email);
	masked_name := sp_mask_text(p_customer_name);

	if en_email is not null and masked_name is not null then
	insert into shop_customer (customer_name,customer_email,last_update) 
	values (masked_name,en_email,now());
	raise notice 'New Customer is added!';
	else
	raise notice 'Issues in adding a customer';
	end if;
end;
$$;

call pro_insert_customer('Praveen','praveen@gmail.com');
call pro_insert_customer('Jp','jp@gmail.com');

select * from shop_customer;


/*


5. Create a procedure to fetch and display masked first_name and decrypted email for all customers
Task:
Write sp_read_customer_masked() that:

Loops through all rows

Decrypts email

Displays customer_id, masked first name, and decrypted email
*/

create or replace procedure sp_read_customer_masked()
language plpgsql
as $$
declare
	rec record;
	decry_email text;
	v_key text := 'this-is-a-secret-key';
begin
	for rec in select customer_id,customer_name, customer_email from shop_customer
	loop
        decry_email := pgp_sym_decrypt(decode(rec.customer_email, 'base64'), v_key);
		Raise notice 'Customer Id: % , Customer Name: % , Customer Email: %', rec.customer_id, rec.customer_name, decry_email;
	end loop;
end;
$$;

call sp_read_customer_masked();

select * from shop_customer;