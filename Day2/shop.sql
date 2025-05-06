/*	Design the database for a shop which sells products
	  Points for consideration
	  1) One product can be supplied by many suppliers
	  2) One supplier can supply many products
	  3) All customers details have to present
	  4) A customer can buy more than one product in every purchase
	  5) Bill for every purchase has to be stored
	  6) These are just details of one shop
 */

create database ecommerce_store;

create table customer_status_master(
status_id varchar(200) primary key,
status_msg varchar(250)
)

create table state_master(
 state_id varchar(200) primary key,
 state_name varchar(200)
)

create table city_master(
 city_id varchar(200) primary key,
 city_name varchar(200),
 state_id varchar(200),
 foreign key (state_id) references state_master(state_id)
)

create table address_details(
 address_id varchar(200) primary key,
 door_no int,
 street_name varchar (200),
 city_id varchar (200),
 pin_code int,
 landmark varchar(200)
 foreign key (city_id) references city_master(city_id)
)

 create table customers_details(
 customer_id varchar(200) primary key,
 customer_name varchar(200),
 phone_no varchar(20),
 email varchar(200),
 address_id varchar(200),
 status_id varchar(200)
 FOREIGN KEY (address_id) REFERENCES address_details(address_id),
 FOREIGN KEY (status_id) REFERENCES customer_status_master(status_id)
 )


create table category_master(
 category_id varchar(200) primary key,
 category_name varchar(200)
)

create table products_details(
product_id varchar(200) primary key,
category_id varchar(200),
product_name varchar(200),
product_description text,
product_price int,
product_rating int,
foreign key (category_id) references category_master(category_id)
)

create table supplier_details(
 supplier_id varchar(200) primary key,
 supplier_name varchar(200),
 supplier_mobile_no varchar(100),
 city_id varchar(200),
 status_id varchar(200),
 foreign key (city_id) references city_master(city_id),
 foreign key (status_id) references customer_status_master(status_id)
)

create table order_status(
order_status_id varchar(200) primary key,
order_status_msg varchar(200)
)

create table payment_modes(
pay_mode_id varchar (200) primary key,
mode_type varchar(200)
)

create table payment_status(
 pay_status_id varchar(200) primary key,
 pay_status_msg varchar(200)
)

create table payment_details(
pay_id varchar(200) primary key,
pay_mode_id varchar(200),
total_amt int,
pay_status_id varchar(200),
foreign key (pay_mode_id) references payment_modes(pay_mode_id),
foreign key (pay_status_id) references payment_status(pay_status_id)
)

create table order_details(
order_id varchar(200) primary key,
customer_id varchar(200),
address_id varchar(200),
delivery_date datetime,
pay_id varchar(200),
order_status_id varchar(200)
foreign key (customer_id) references customers_details(customer_id),
foreign key (address_id) references address_details(address_id),
foreign key (pay_id) references payment_details(pay_id),
foreign key (order_status_id) references order_status(order_status_id)
)


CREATE TABLE order_items (
order_item_id VARCHAR(200) PRIMARY KEY,
order_id VARCHAR(200),
product_id VARCHAR(200),
quantity INT,
product_price INT, 
date_of_order datetime,
FOREIGN KEY (order_id) REFERENCES order_details(order_id),
FOREIGN KEY (product_id) REFERENCES products_details(product_id)
);

CREATE TABLE product_supplier_map (
product_id VARCHAR(200),
supplier_id VARCHAR(200),
PRIMARY KEY (product_id, supplier_id),
FOREIGN KEY (product_id) REFERENCES products_details(product_id),
FOREIGN KEY (supplier_id) REFERENCES supplier_details(supplier_id)
);

CREATE TABLE billing_details (
billing_id VARCHAR(200) PRIMARY KEY,
order_id VARCHAR(200),
billing_date DATE,
total_amount INT,
tax_amount INT,
discount INT,
net_amount INT,
FOREIGN KEY (order_id) REFERENCES order_details(order_id)
);
