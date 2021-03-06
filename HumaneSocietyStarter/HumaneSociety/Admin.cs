﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class Admin : User
    {
        private delegate void crudOperationsEmployees();

        public override void LogIn()
        {
            UserInterface.DisplayUserOptions("What is your password?");
            string password = UserInterface.GetUserInput();
            if (password.ToLower() != "pass")
            {
                UserInterface.DisplayUserOptions("Incorrect password please try again or type exit");
            }
            else
            {
                RunUserMenus();
                
            }
        }

        protected override void RunUserMenus()
        {
            Console.Clear();
            List<string> options = new List<string>() { "Admin log in successful.", "What would you like to do?", "1. Create new employee", "2. Delete employee", "3. Read employee info ", "4. Update employee info", "(type 1, 2, 3, 4,  create, read, update, or delete)" };
            UserInterface.DisplayUserOptions(options);
            string input = UserInterface.GetUserInput();
            RunInput(input);
        }
        protected void RunInput(string input)
        {
            crudOperationsEmployees editEmployees;

            if(input == "1" || input.ToLower() == "create")
            {
                editEmployees = AddEmployee;
            }
            else if(input == "2" || input.ToLower() == "delete")
            {
                editEmployees = RemoveEmployee;
            }
            else if(input == "3" || input.ToLower() == "read")
            {
                editEmployees = ReadEmployee;
            }
            else if (input == "4" || input.ToLower() == "update")
            {
                editEmployees = UpdateEmployee;
            }
            else
            {
                UserInterface.DisplayUserOptions("Input not recognized press any key to go back to the main menu.");
                Console.ReadLine();
                editEmployees = null;
                RunUserMenus();
            }
            if(editEmployees != null)
            {
                editEmployees();
                RunUserMenus();
            }
        }

        private void UpdateEmployee()
        {
            Employee employee = new Employee();
            employee.EmployeeNumber = UserInterface.GetIntegerData("employee number", "the employee's");
            employee.FirstName = UserInterface.GetStringData("updated first name", "the employee's");
            employee.LastName = UserInterface.GetStringData("updated last name", "the employee's");
            employee.Email = UserInterface.GetStringData("updated email", "the employee's");
            try
            {
                Query.RunEmployeeQueries(employee, "update");
                UserInterface.DisplayUserOptions("Employee update successful. Press any key to continue.");
                Console.ReadLine();
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee update unsuccessful press any key to continue.");
                Console.ReadLine();
                return;
            }
        }

        private void ReadEmployee()
        {
            try
            {
                Employee employee = new Employee();
                employee.EmployeeNumber = int.Parse(UserInterface.GetStringData("employee number", "the employee's"));
                Query.RunEmployeeQueries(employee, "read");
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee not found. Press any key to return to the main menu.");
                Console.ReadLine();
                return;
            }
        }

        private void RemoveEmployee()
        {
            Employee employee = new Employee();
            employee.LastName = UserInterface.GetStringData("last name", "the employee's"); ;
            employee.EmployeeNumber = UserInterface.GetIntegerData("employee number", "the employee's");
            try
            {
                Console.Clear();
                Query.RunEmployeeQueries(employee, "delete");
                UserInterface.DisplayUserOptions("Employee successfully removed. Press any key to continue.");
                Console.ReadLine();
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee removal unsuccessful please try again or type exit");
                RemoveEmployee();
            }
        }

        private void AddEmployee()
        {
            Employee employee = new Employee();
            employee.FirstName = UserInterface.GetStringData("first name", "the employee's");
            employee.LastName = UserInterface.GetStringData("last name", "the employee's");
            employee.EmployeeNumber = UserInterface.GetIntegerData("employee number", "the employee's");
            employee.Email = UserInterface.GetStringData("email", "the employee's"); ;
            try
            {
                Query.RunEmployeeQueries(employee, "create");
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee addition unsuccessful press any key to go back to the main menu.");
                Console.ReadLine();
                return;
            }
        }

    }
}
