// 1) create a program that will take name from user and greet the user

using System;
class Task1
{
    static void Main(string[] args)
    {
        string? name;
        name = Console.ReadLine();

        Console.WriteLine($"Hello, {name}! Welcome to Csharp Tutorial.");
    }
 
}


 // 2) Take 2 numbers from user and print the largest

using System;

class Task2
{
    static void Main(string[] args)
    {
        double num1 = Convert.ToDouble(Console.ReadLine());
        double num2 = Convert.ToDouble(Console.ReadLine());
        double largest = (num1 > num2) ? num1 : num2;

        Console.WriteLine($"The largest number is: {largest}");
    }
}

// 3) Take 2 numbers from user, check the operation user wants to perform (+,-,*,/). Do the operation and print the result

using System;

class Task3
{
    static void Main()
    {
        double num1 = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter second number: ");
        double num2 = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter operation (+, -, *, /): ");
        string operation = Console.ReadLine();

        double result;

        switch (operation)
        {
            case "+":
                result = num1 + num2;
                Console.WriteLine($"Result: {result}");
                break;
            case "-":
                result = num1 - num2;
                Console.WriteLine($"Result: {result}");
                break;
            case "*":
                result = num1 * num2;
                Console.WriteLine($"Result: {result}");
                break;
            case "/":
                if (num2 != 0)
                {
                    result = num1 / num2;
                    Console.WriteLine($"Result: {result}");
                }
                else
                {
                    Console.WriteLine("Cannot divide by zero.");
                }
                break;
            default:
                Console.WriteLine("Invalid operation.");
                break;
        }
    }
}

using System;

class Task4
{
    static void Main()
    {
        int attempts = 0;

        while (attempts < 3)
        {
            Console.Write("Enter username: ");
            string? username = Console.ReadLine();

            Console.Write("Enter password: ");
            string? password = Console.ReadLine();

            if (username == "Admin" && password == "pass")
            {
                Console.WriteLine("Login successful!");
                return;
            }
            else
            {
                Console.WriteLine("Invalid username or password.\n");
                attempts++;
            }
        }

        Console.WriteLine("Invalid attempts for 3 times. Exiting....");
    }
}

using System;

class Program
{
    static void Main()
    {
        int count = 0;

        for (int i = 1; i <= 10; i++)
        {
            Console.Write($"Enter number {i}: ");
            int num = Convert.ToInt32(Console.ReadLine());

            if (num % 7 == 0)
            {
                count++;
            }
        }

        Console.WriteLine($"\nNumbers divisible by 7: {count}");
    }
}


using System;
using System.Collections.Generic;

class Program
{

    static Dictionary<int, int> CountFrequencies(int[] arr)
    {
        Dictionary<int, int> freqC = new Dictionary<int, int>();

        foreach (int num in arr)
        {
            if (freqC.ContainsKey(num))
            {
                freqC[num]++;
            }
            else
            {
                freqC[num] = 1;
            }
        }
        return freqC;
    }

    static void PrintFrequencies(Dictionary<int, int> freq)
    {
        foreach (var n in freq)
        {
            Console.WriteLine($"{n.Key} occurs {n.Value} times");
        }
    }

    static void Main(string[] args)
    {
        int[] numbers = { 1, 2, 2, 3, 4, 4, 4 };

        Dictionary<int, int> freq = CountFrequencies(numbers);

        PrintFrequencies(freq);
    }
}


// 7) create a program to rotate the array to the left by one position.
// Input: {10, 20, 30, 40, 50}
// Output: {20, 30, 40, 50, 10}

using System;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter size of the array: ");
        int size = Convert.ToInt32(Console.ReadLine());

        int[] arr = new int[size];
        Console.WriteLine("Enter array elements:");
        for (int i = 0; i < size; i++)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            if(n>0)
            arr[i] = n;            
        }

        int[] rotatedElements = RotateLeft(arr, size);

        Console.WriteLine("Array after left rotation:");
        PrintElements(rotatedElements);
    }

    static int[] RotateLeft(int[] arr, int size)
    {
        int[] rotated = new int[size];
        for (int i = 1; i < size; i++)
        {
            rotated[i - 1] = arr[i];
        }
        rotated[size - 1] = arr[0];
        return rotated; 
    }

    static void PrintElements(int[] arr)
    {
        foreach (int a in arr)
        {
            Console.Write(a + " "); 
        }
        Console.WriteLine();
    }
}



// 8) Given two integer arrays, merge them into a single array.
// Input: {1, 3, 5} and {2, 4, 6}
// Output: {1, 3, 5, 2, 4, 6}

using System;

class Program
{
    static int[] Mergearray(int[] arr1, int size1, int[] arr2, int size2)
    {
        int total = size1 + size2;
        int[] merged = new int[total];
        int j = 0;
        for (int i = 0; i < size1; i++)
        {
            merged[j++] = arr1[i];
        }
        for (int i = 0; i < size2; i++)
        {
            merged[j++] = arr2[i];
        }
        return merged;
    }

    static void printMergedArray(int[] arr)
    {
        foreach (int i in arr)
        {
            Console.Write(i + " ");
        }
    }
    static void Main(string[] args)
    {
        Console.WriteLine("Enter Array 1 Size..");
        int size1 = Convert.ToInt32(Console.ReadLine());
        int[] arr1 = new int[size1];
        Console.WriteLine("Enter Array 1 Elements..");
        for (int i = 0; i < size1; i++)
        {
            arr1[i] = Convert.ToInt32(Console.ReadLine());
        }

        Console.WriteLine("Enter Array 2 Size..");
        int size2 = Convert.ToInt32(Console.ReadLine());
        int[] arr2 = new int[size2];
        for (int i = 0; i < size2; i++)
        {
            arr2[i] = Convert.ToInt32(Console.ReadLine());
        }

        int[] Mergedarray = Mergearray(arr1, size1, arr2, size2);

        printMergedArray(Mergedarray);
    }
}


// 9) Write a program that:
// Has a predefined secret word (e.g., "GAME").
// Accepts user input as a 4-letter word guess.
// Compares the guess to the secret word and outputs:
// X Bulls: number of letters in the correct position.
// Y Cows: number of correct letters in the wrong position.
// Continues until the user gets 4 Bulls (i.e., correct guess).
// Displays the number of attempts.
// Bull = Correct letter in correct position.
// Cow = Correct letter in wrong position.
// Secret Word	User Guess	Output	Explanation
// GAME	GAME	4 Bulls, 0 Cows	Exact match
// GAME	MAGE	1 Bull, 3 Cows	A in correct position, MGE misplaced
// GAME	GUYS	1 Bull, 0 Cows	G in correct place, rest wrong
// GAME	AMGE	2 Bulls, 2 Cows	A, E right; M, G misplaced
// NOTE	TONE	2 Bulls, 2 Cows	O, E right; T, N misplaced

using System;

class Program
{

    static void Main()
    {
        string secret = "GAME";
        int attempts = 0;

        while (true)
        {
            attempts++;
            Console.Write("Enter your 4-letter guess: ");
            string guess = Console.ReadLine().ToUpper();

            if (guess.Length != 4)
            {
                Console.WriteLine("Please enter exactly 4 letters.");
                continue;
            }

            int bulls = 0;
            int cows = 0;

            bool[] secretMatched = new bool[4];
            bool[] guessMatched = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secret[i])
                {
                    bulls++;
                    secretMatched[i] = true;
                    guessMatched[i] = true;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (guessMatched[i]) continue;

                for (int j = 0; j < 4; j++)
                {
                    if (secretMatched[j]) continue;

                    if (guess[i] == secret[j])
                    {
                        cows++;
                        secretMatched[j] = true;
                        break;
                    }
                }
            }

            Console.WriteLine($"{bulls} Bulls, {cows} Cows");

            if (bulls == 4)
            {
                Console.WriteLine($"Congratulations! You guessed the word in {attempts} attempts.");
                break;
            }
        }
    }
}


// 10) write a program that accepts a 9-element array representing a Sudoku row.
// Validates if the row:
// Has all numbers from 1 to 9.
// Has no duplicates.
// Displays if the row is valid or invalid.

using System;
using System.Collections.Generic;

class SudokuRowValidator
{
    static void Main()
    {
        int[] row = new int[9];
        Console.WriteLine("Enter 9 numbers for the Sudoku row (1 to 9):");

        for (int i = 0; i < 9; i++)
        {
            Console.Write($"Element {i + 1}: ");
            int input = Convert.ToInt32(Console.ReadLine());

            if (input < 1 || input > 9)
            {
                Console.WriteLine("Invalid input. All numbers must be between 1 and 9.");
                return;
            }

            row[i] = input;
        }

        if (IsValidSudokuRow(row))
        {
            Console.WriteLine(" Valid Sudoku row.");
        }
        else
        {
            Console.WriteLine(" Invalid Sudoku row.");
        }
    }

    static bool IsValidSudokuRow(int[] row)
    {
        HashSet<int> seen = new HashSet<int>();

        foreach (int num in row)
        {
            if (seen.Contains(num))
                return false;

            seen.Add(num);
        }

        return seen.Count == 9;
    }
}


// 11. Extended version sudoku board.

using System;
using System.Collections.Generic;

class SudokuValidator
{
    static void Main()
    {
        int[,] board = new int[9, 9];

        Console.WriteLine("Enter the Sudoku board row by row (9 numbers per row, separated by spaces):");

        for (int i = 0; i < 9; i++)
        {
            Console.Write($"Row {i + 1}: ");
            string[] rowInput = Console.ReadLine().Split();

            if (rowInput.Length != 9)
            {
                Console.WriteLine("Each row must have exactly 9 numbers.");
                return;
            }

            for (int j = 0; j < 9; j++)
            {
                int num = Convert.ToInt32(rowInput[j]);

                if (num < 1 || num > 9)
                {
                    Console.WriteLine("Each number must be between 1 and 9.");
                    return;
                }

                board[i, j] = num;
            }
        }

        // Validate each row
        bool isValid = true;
        for (int i = 0; i < 9; i++)
        {
            if (!IsValidRow(board, i))
            {
                Console.WriteLine($"Row {i + 1} is invalid.");
                isValid = false;
            }
            else
            {
                Console.WriteLine($"Row {i + 1} is valid.");
            }
        }

        if (isValid)
        {
            Console.WriteLine("\n All rows are valid.");
        }
        else
        {
            Console.WriteLine("\n Sudoku board has invalid rows.");
        }
    }

    static bool IsValidRow(int[,] board, int rowIndex)
    {
        HashSet<int> seen = new HashSet<int>();

        for (int col = 0; col < 9; col++)
        {
            int num = board[rowIndex, col];

            if (seen.Contains(num))
                return false;

            seen.Add(num);
        }

        return seen.Count == 9;
    }
}



/*
12) Write a program that:
Takes a message string as input (only lowercase letters, no spaces or symbols).
Encrypts it by shifting each character forward by 3 places in the alphabet.
Decrypts it back to the original message by shifting backward by 3.
Handles wrap-around, e.g., 'z' becomes 'c'.
Examples
Input:     hello
Encrypted: khoor
Decrypted: hello 
*/

using System;

class Program
{
    static string Encryptword(char[] arr)
    {
        string? encryptedtext = "";
        foreach(char c in arr)
        {
            encryptedtext += (char)('a' + ((c - 'a' +3) % 26));

        }
        return encryptedtext;
    }

    static string Decryptword(char[] arr)
    {
        string? decryptedtext = "";
        foreach(char c in arr)
        {
            decryptedtext += (char)('a' + ((c - 'a') % 26));
        }
        return decryptedtext;
    }
    
    static void Printresults(string str1,string str2)
    {
        Console.WriteLine($"Encrypted Text: {str1} ");
        
        Console.WriteLine($"Decrypted Text: {str2} ");
    }

    static void Main(string[] args)
    {
        string? word = Console.ReadLine().ToLower();
        char[] warr = word.ToCharArray();
        string encryptext = Encryptword(warr);
        string decryptext = Decryptword(warr);
        Printresults(encryptext, decryptext);
    }
}