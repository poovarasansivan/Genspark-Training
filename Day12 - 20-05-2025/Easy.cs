using System;

class EmployeePromotion
{
    static List<string> empnames = new List<string>();

    static void GetNames()
    {
        Console.WriteLine("Please enter the employee names in the order of their eligibility for promotion.");
        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Employee Name should not be blank or empty.");
                break;
            }
            empnames.Add(input);
        }
    }

    static void TrimExcessMemory()
    {
        int? size = empnames.Capacity;
        Console.WriteLine($"The current size of the collection is {size}");
        empnames.TrimExcess();
        int? nsize = empnames.Capacity;
        Console.WriteLine($"The size after removing the extra space is {nsize}");
    }
    static void FindEmpPosition()
    {
        Console.WriteLine("Please enter the name of the employee to check promotion position.");
        string? searchname = Console.ReadLine();

        int? index = empnames.IndexOf(searchname);

        if (index != -1)
            Console.WriteLine($"{searchname} is the position {index + 1} for the promotion.");
        else
            Console.WriteLine($"{searchname} is not present in the promotion list");
    }

    static void SortList()
    {
        empnames.Sort();
        Console.WriteLine("Employee Promotion list after sorting it on Ascending order.");
        foreach (var name in empnames)
        {
            Console.WriteLine($"{name}");
        }
    }
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nChoose One Option Below:");
            Console.WriteLine("1. Add Promotion List");
            Console.WriteLine("2. Find Promotion List Position of the Employee");
            Console.WriteLine("3. Trim Excess Memory of the List");
            Console.WriteLine("4. Sort the List");
            Console.WriteLine("5. Exit");

            Console.Write("Enter your choice: ");
            int opt;

            if (!int.TryParse(Console.ReadLine(), out opt))
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                continue;
            }

            switch (opt)
            {
                case 1:
                    GetNames();
                    break;
                case 2:
                    FindEmpPosition();
                    break;
                case 3:
                    TrimExcessMemory();
                    break;
                case 4:
                    SortList();
                    break;
                case 5:
                    Console.WriteLine("Exiting the application...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please choose from 1 to 5.");
                    break;
            }
        }
    }
}