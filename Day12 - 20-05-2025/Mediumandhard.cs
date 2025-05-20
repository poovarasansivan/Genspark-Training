// Combination of Both Medium and Hark Task.

using System;
using System.Collections.Generic;
using System.Linq;

class Employee : IComparable<Employee>
{
    int id, age;
    string name = "";
    double salary;

    public Employee() { }

    public Employee(int id, int age, string name, double salary)
    {
        this.id = id;
        this.age = age;
        this.name = name;
        this.salary = salary;
    }

    public void TakeEmployeeDetailsFromUser()
    {
        Console.WriteLine("Please enter the employee ID");
        id = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Please enter the employee name");
        name = Console.ReadLine();

        Console.WriteLine("Please enter the employee age");
        age = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Please enter the employee salary");
        salary = Convert.ToDouble(Console.ReadLine());
    }

    public override string ToString()
    {
        return $"Employee ID : {id}\nName : {name}\nAge : {age}\nSalary : {salary}";
    }

    public int Id { get => id; set => id = value; }
    public int Age { get => age; set => age = value; }
    public string Name { get => name; set => name = value; }
    public double Salary { get => salary; set => salary = value; }

    public int CompareTo(Employee other)
    {
        return this.salary.CompareTo(other.salary);
    }
}

class Program
{
    static Dictionary<int, Employee> empdict = new Dictionary<int, Employee>();
    static List<Employee> names = new List<Employee>();

    static void AddEmployees()
    {
        Employee emp = new Employee();
        emp.TakeEmployeeDetailsFromUser();
        if (!empdict.ContainsKey(emp.Id))
        {
            empdict.Add(emp.Id, emp);
            names.Add(emp);
        }
        else
        {
            Console.WriteLine("Employee ID already exists");
        }
    }

    static void SortEmployees()
    {
        var sortedList = names.OrderBy(e => e.Salary).ToList();
        Console.WriteLine("\nEmployees sorted by salary:");

        foreach (var e in sortedList)
            Console.WriteLine(e + "\n");
    }

    static void FindEmpID()
    {
        Console.Write("Enter employee ID to search: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var employee = empdict
            .Where(e => e.Key == id)
            .Select(e => e.Value)
            .FirstOrDefault();

        if (employee != null)
        {
            Console.WriteLine(employee);
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }

    static void Findbyname()
    {
        Console.Write("Enter employee name to search: ");
        string? name = Console.ReadLine();
        var matches = names.Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matches.Count == 0)
        {
            Console.WriteLine("No employee found with the given name.");
            return;
        }

        Console.WriteLine($"\nFound {matches.Count} employee(s):");
        foreach (var e in matches)
            Console.WriteLine(e + "\n");
    }

    static void UpdateEmp()
    {
        Console.WriteLine("Enter the Employee ID that needs to be updated..");
        int id = Convert.ToInt32(Console.ReadLine());
        if (empdict.TryGetValue(id, out Employee emp))
        {
            Console.Write("Enter new name: ");
            emp.Name = Console.ReadLine();

            Console.Write("Enter new age: ");
            emp.Age = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter new Salary: ");
            emp.Salary = Convert.ToDouble(Console.ReadLine());

            var index = names.FindIndex(e => e.Id == id);
            if (index != -1)
                names[index] = emp;
        }
        else
        {
            Console.WriteLine("Employee Details for this ID not found.");
        }
    }

    static void DeleteEmp()
    {
        Console.WriteLine("Enter the Employee ID to Delete..");
        int id = Convert.ToInt32(Console.ReadLine());

        if (empdict.Remove(id))
        {
            names.RemoveAll(e => e.Id == id);
            Console.WriteLine($"Employee Details for the Employee Id {id} deleted successfully");
        }
        else
        {
            Console.WriteLine("Employee ID not found.");
        }
    }
    static void FindElderEmp()
    {
        Console.WriteLine("Enter the employee Id to compare the age...");
        int id = Convert.ToInt32(Console.ReadLine());

        if (empdict.TryGetValue(id, out Employee emp))
        {
            var elders = names.Where(e => e.Age > emp.Age).ToList();
            if (elders.Count == 0)
            {
                Console.WriteLine("No employees found elder than the given employee.");
                return;
            }

            Console.WriteLine($"\nEmployees elder than {emp.Name} (Age: {emp.Age}):");
            foreach (var e in elders)
                Console.WriteLine(e + "\n");
        }
        else
        {
            Console.WriteLine("Employee ID not found.");
        }
    }

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nChoose One Option Below:");
            Console.WriteLine("1. Add employee details");
            Console.WriteLine("2. Sort the employee based on salary");
            Console.WriteLine("3. Find employee details using employee ID");
            Console.WriteLine("4. Find all employees with same name");
            Console.WriteLine("5. Find all employees who are elder than given employee");
            Console.WriteLine("6. Update the employee details.");
            Console.WriteLine("7. Delete the employee details.");
            Console.WriteLine("8. Exit");

            Console.Write("Enter your choice: ");
            if (!int.TryParse(Console.ReadLine(), out int opt))
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                continue;
            }

            switch (opt)
            {
                case 1:
                    AddEmployees();
                    break;
                case 2:
                    SortEmployees();
                    break;
                case 3:
                    FindEmpID();
                    break;
                case 4:
                    Findbyname();
                    break;
                case 5:
                    FindElderEmp();
                    break;
                case 6:
                    UpdateEmp();
                    break;
                case 7:
                    DeleteEmp();
                    break;
                case 8:
                    Console.WriteLine("Exiting the application...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please choose from 1 to 6.");
                    break;
            }
        }
    }
}
