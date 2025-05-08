/*
 1. List all orders with the customer name and the employee who handled the order.
	(Join Orders, Customers, and Employees)
*/

select ors.OrderID,cu.CompanyName,cu.ContactName,emp.EmployeeID,concat(emp.Firstname,' ',emp.LastName)as Employee_Name from Orders ors inner join Customers cu on cu.CustomerID = ors.CustomerID
inner join Employees emp on emp.EmployeeID=ors.EmployeeID 


/*
2. Get a list of products along with their category and supplier name.
   (Join Products, Categories, and Suppliers)
*/

select pr.ProductID,pr.ProductName,c.CategoryID,c.CategoryName,su.SupplierID,su.CompanyName from Products pr 
join Suppliers su on su.SupplierID=pr.SupplierID
join Categories c on c.CategoryID=pr.CategoryID

/*
3. Show all orders and the products included in each order with quantity and unit price.
   (Join Orders, Order Details, Products)
*/

select ors.OrderID,ors.CustomerID,pd.ProductID,pd.ProductName,ord.Quantity,ord.UnitPrice from Orders ors 
join [Order Details] ord on ord.OrderID=ors.OrderID
join Products pd on pd.ProductID=ord.ProductID

/*
4. List employees who report to other employees (manager-subordinate relationship).
   (Self join on Employees)
*/

select emp.EmployeeID,concat(emp.FirstName,' ',emp.LastName)as Employee_Name,concat(emps.FirstName, ' ', emps.LastName) AS Manager_Name from Employees emp
join Employees emps on emps.EmployeeID=emp.ReportsTo

/*
5. Display each customer and their total order count.
   (Join Customers and Orders, then GROUP BY)
*/

select cu.CustomerID,cu.CompanyName,sum(ors.OrderID)as Total_Orders from Customers cu
join Orders ors on ors.CustomerID=cu.CustomerID group by cu.CustomerID, cu.CompanyName 

/*
6. Find the average unit price of products per category.
   Use AVG() with GROUP BY
*/

select ca.CategoryID,ca.CategoryName, AVG(pd.UnitPrice) as Average_Unit_Price_Category from Categories ca
join Products pd on pd.CategoryID = ca.CategoryID
group by ca.CategoryID,ca.CategoryName

/*
7. Show the top 5 most expensive products.
   Use ORDER BY UnitPrice DESC and TOP 5
*/

select Top 5 ProductID,ProductName,UnitPrice from Products order by UnitPrice DESC

/*
8. List customers where the contact title starts with 'Owner'.
   Use LIKE or LEFT(ContactTitle, 5)
*/

select CustomerID, CompanyName, ContactTitle from Customers where ContactTitle like 'Owner'

/*
9. Return the total sales amount (quantity × unit price) per order.
   Use SUM(OrderDetails.Quantity * OrderDetails.UnitPrice) and GROUP BY
*/

select OrderID, sum(od.Quantity*Od.UnitPrice) as Total_Sale_Amount from [Order Details] od group by OrderID

/*
10. Create a stored procedure that returns all orders for a given customer ID.
	Input: @CustomerID
*/

create procedure proc_customer (@CustomerID nvarchar(100))
as
begin
select ors.OrderID,pd.ProductID,pd.ProductName ,cu.CustomerID,cu.CompanyName from Orders ors join Customers cu on cu.CustomerID = ors.CustomerID 
join [Order Details] ord on ord.OrderID = ors.OrderID
join Products pd on pd.ProductID = ord.ProductID
where ors.CustomerID=@CustomerID
end

exec proc_customer 'VINET'

/*
11. Write a stored procedure that inserts a new product.
	Inputs: ProductName, SupplierID, CategoryID, UnitPrice, etc.
*/

create procedure proc_InsertNewProduct(@Pname varchar(100), @SupID int,@CatID int,@Qunatity varchar(100),
@unitp float,@Unitstock int,@uorder int,@Reorder int,@Discontinued int)
as
begin 
	insert into Products(ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
	values(@Pname,@SupID,@CatID,@Qunatity,@unitp,@Unitstock,@uorder,@Reorder,@Discontinued)
end

EXEC proc_InsertNewProduct 'New Product', 1, 2, '10 boxes', 12.5, 100, 20, 10, 0;


/*
12. Create a stored procedure that returns total sales per employee.
	Join Orders, Order Details, and Employees
*/

create procedure proc_total_sale_emp
as
begin
  select emp.EmployeeID , concat(emp.FirstName,' ',emp.LastName) as Employee_Name,sum(ord.UnitPrice*ord.Quantity) Total_sale_amt, count(ord.OrderID) as Total_Orders from Orders ors
  join [Order Details] ord on ord.OrderID=ors.OrderID
  join Employees emp on emp.EmployeeID=ors.EmployeeID
  group by emp.EmployeeID,emp.FirstName,emp.LastName
  order by emp.EmployeeID 
end

exec proc_total_sale_emp


/*
13. Use a CTE to rank products by unit price within each category.
	Use ROW_NUMBER() or RANK() with PARTITION BY CategoryID
*/

with Product_Ranking as (
 select p.ProductID,p.ProductName, p.CategoryID,c.CategoryName, ROW_NUMBER() OVER (partition by p.CategoryID order by p.UnitPrice desc) as Product_Rank from Products p 
 join Categories c on c.CategoryID = p.CategoryID
)

select * from Product_Ranking order by CategoryID, Product_Rank


/*
14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.

*/

with total_revenue as
(
	select ord.ProductID, p.ProductName, sum(ord.UnitPrice * ord.Quantity * (1 - ord.Discount)) AS TotalRevenue
    from [Order Details] ord
	join Products p on p.ProductID = ord.ProductID 
	group by ord.ProductID, p.ProductName
)
select * from total_revenue where TotalRevenue > 10000 order by TotalRevenue desc

/*
15. Use a CTE with recursion to display employee hierarchy.
	Start from top-level employee (ReportsTo IS NULL) and drill down
*/

with Employee_Hierarchy as(
	select emp.EmployeeID, concat(emp.FirstName,' ',emp.LastName) as Employee_Name, emp.ReportsTo, 1 as Level from Employees emp     WHERE ReportsTo IS NULL
	union all
	select e.EmployeeID,concat(e.FirstName,' ',e.LastName) as Employee_Name, e.ReportsTo, eh.Level+1 from Employees e
	inner join Employee_Hierarchy eh on e.ReportsTo = eh.EmployeeID
)

select EmployeeID, Employee_Name,ReportsTo,Level from Employee_Hierarchy


