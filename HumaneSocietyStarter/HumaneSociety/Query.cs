﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {
        

        internal static List<USState> GetStates()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = zipCode;
                newAddress.USStateId = stateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            Address clientAddress = clientWithUpdates.Address;

            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = clientAddress.Zipcode;
                newAddress.USStateId = clientAddress.USStateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            clientFromDb.AddressId = updatedAddress.AddressId;

            db.SubmitChanges();
        }

        internal static void RunEmployeeQueries(Employee employee, string v)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            switch (v)
            {
                case "delete":
                    List<Animal> animals = db.Animals.Where(a => a.EmployeeId == employee.EmployeeNumber).ToList();
                    employee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    foreach (Animal pets in animals)
                    {
                        pets.EmployeeId = null;
                    }

                    db.Employees.DeleteOnSubmit(employee);
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    break;

                case "read":
                    employee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    UserInterface.DisplayUserOptions(" First Name: " + employee.FirstName + "\n Last Name: " + employee.LastName + "\n UserName: " + employee.UserName + "\n Password: " + employee.Password + "\n Employee Nmber: " + employee.EmployeeNumber + "\n Email: " + employee.Email + "\n Press any key to continue.");
                    Console.ReadLine();
                    break;

                case "update":
                    Employee updatedEmployee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    updatedEmployee.FirstName = employee.FirstName;
                    updatedEmployee.LastName = employee.LastName;
                    updatedEmployee.EmployeeNumber = employee.EmployeeNumber;
                    updatedEmployee.Email = employee.Email;

                    break;

                case "create":
                    Employee newEmployee = new Employee();
                    newEmployee = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    if(newEmployee == null)
                    {
                        
                        db.Employees.InsertOnSubmit(employee);
                        UserInterface.DisplayUserOptions("Employee addition successful. Press any key to continue.");
                        Console.ReadLine();
                    }
                    else
                    {
                        UserInterface.DisplayUserOptions("That employee number is already in use, press any key to go back to the main menu.");
                        Console.ReadLine();
                    }
                    break;
            }
            try
            {
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
       
        internal static Room GetRoom(int animalId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var roomNumber = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();

            return roomNumber;
        }

        internal static void Adopt(Animal animal, Client client) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Adoption adoption = new Adoption();
            animal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = true;
            animal.AdoptionStatus = "Pending";

            db.Adoptions.InsertOnSubmit(adoption);
            try
            {
                db.SubmitChanges(); 
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static Animal GetAnimalByID(int iD)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            try
            {
                var animalsById = db.Animals.Where(a => a.AnimalId == iD).Single();
                return animalsById;
            }
            catch
            {                
                return null;
            }
        }

        internal static void UpdateAdoption(bool v, Adoption adoption)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Animal animal = new Animal();
            adoption = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
            animal = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
            if (v)
            {
                adoption.ApprovalStatus = "Adopted";
                animal.AdoptionStatus = "Adopted";
            }
            else
            {
                db.Adoptions.DeleteOnSubmit(adoption);
                animal.AdoptionStatus = "Not Adopted";
            }
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        internal static List<Animal> SearchForAnimalByMultipleTraits(Dictionary<int,string> searchCriteria)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            List<Animal> animals = new List<Animal>();
            animals = db.Animals.Where(a => a.AnimalId != 0).ToList();

            foreach(KeyValuePair<int,string> criteria in searchCriteria)
            {
                if (animals.Count() == 0)
                {
                    return animals;
                }
                else
                {
                    switch (criteria.Key)
                    {
                        case 1:
                            int newId = int.Parse(criteria.Value);
                            animals = animals.Where(a=> a.CategoryId == newId).ToList();
                            break;

                        case 2:
                            animals = animals.Where(a => a.Name.ToLower() == criteria.Value.ToLower()).ToList();
                            break;

                        case 3:
                            int age = int.Parse(criteria.Value);
                            animals = animals.Where(a => a.Age == age).ToList();
                            break;

                        case 4:
                            animals = animals.Where(a => a.Demeanor.ToLower() == criteria.Value.ToLower()).ToList();
                            break;

                        case 5:
                            if(criteria.Value.ToLower() == "true")
                            {
                                animals = animals.Where(a => a.KidFriendly == true).ToList();
                            }
                            else
                            {
                                animals = animals.Where(a => a.KidFriendly == false).ToList();
                            }
                            break;

                        case 6:
                            if (criteria.Value.ToLower() == "true")
                            {
                                animals = animals.Where(a => a.PetFriendly == true).ToList();
                            }
                            else
                            {
                                animals = animals.Where(a => a.PetFriendly == false).ToList();
                            }
                            break;

                        case 7:
                            int weight = int.Parse(criteria.Value);
                            animals = animals.Where(a => a.Weight == weight).ToList();
                            break;

                        case 8:
                            int id = int.Parse(criteria.Value);
                            animals = animals.Where(a => a.AnimalId == id).ToList();
                            break;
                    }
                }
            }
            return animals;
        }

        internal static List<Adoption> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            var adoptionFromDb = db.Adoptions.Where(a => a.ApprovalStatus.ToLower() == "pending").ToList();
            return adoptionFromDb;
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException("Sorry, we don`t recognize that");           
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static List<AnimalShot> GetShots(Animal animal) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var shotsFromDb = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId).ToList();
            
            return shotsFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            if(employeeWithUserName == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal static void UpdateShot(int v, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            AnimalShot animalShots = new AnimalShot();
            Shot checkShots = new Shot();
            
            animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId && a.ShotId == v).FirstOrDefault();
            checkShots = db.Shots.Where(a => a.ShotId == v).FirstOrDefault();
            
            if (checkShots == null)
            {
                UserInterface.DisplayUserOptions("This shot does not exist. Press any key to continue.");
                Console.ReadLine();
                return;
            }
            else if (animalShots == null)
            {
                AnimalShot animalShot = new AnimalShot();
                animalShot.ShotId = v;
                animalShot.DateReceived = DateTime.Now;
                animalShot.AnimalId = animal.AnimalId;
                db.AnimalShots.InsertOnSubmit(animalShot);
                try
                {
                    db.SubmitChanges();
                    Console.WriteLine("Shots have been updated.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else if(animalShots != null)
            {
                UserInterface.DisplayUserOptions("Animal already has this shot, press any key to continue.");
                Console.ReadLine();
            }
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static void EnterAnimalUpdate(Animal animals, Dictionary<int, string> updates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room room = new Room();
            animals = db.Animals.Where(a => a.AnimalId == animals.AnimalId).FirstOrDefault();
            if(animals.AdoptionStatus.ToLower() == "adopted")
            {
                Console.WriteLine("This animal has been adopted. No changes can be made. Press any key to continue.");
                Console.ReadLine();
                return;
            }
            foreach (KeyValuePair<int, string> criteria in updates)
            {
                switch (criteria.Key)
                {
                    case 1:                        
                        animals.CategoryId = int.Parse(criteria.Value);
                        break;

                    case 2:
                        animals.Name = criteria.Value;
                        break;

                    case 3:
                        animals.Age = int.Parse(criteria.Value);
                        break;

                    case 4:
                        animals.Demeanor = criteria.Value;
                        break;

                    case 5:
                        if (criteria.Value.ToLower() == "true")
                        {
                            animals.KidFriendly = true;
                        }
                        else
                        {
                            animals.KidFriendly = false;
                        }
                        break;

                    case 6:
                        if (criteria.Value.ToLower() == "true")
                        {
                            animals.PetFriendly = true;
                        }
                        else
                        {
                            animals.PetFriendly = false;
                        }
                        break;

                    case 7:
                        animals.Weight = int.Parse(criteria.Value);
                        break;
                }
            }
            try
            {
                db.SubmitChanges();
                UserInterface.DisplayUserOptions("Update complete.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static void PlaceAnimalIntoRoom(int animalId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room room = new Room();

            room = db.Rooms.Where(r => r.AnimalId == null).FirstOrDefault();
            room.AnimalId = animalId;

            try
            {
                db.SubmitChanges();
                UserInterface.DisplayUserOptions("This animal was put in room " + room.RoomNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static bool CheckIfEmptyRoom()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room room = new Room();

            room = db.Rooms.Where(r => r.AnimalId == null).FirstOrDefault();
            if (room == null)
            {
                return false;
            }
            return true;
        }

        internal static void MoveAnimal(Animal animal, int newRoomNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room room = new Room();
            room = db.Rooms.Where(r => r.RoomNumber == newRoomNumber).FirstOrDefault();

            if (room == null)
            {
                UserInterface.DisplayUserOptions("That room does not exist. Press any key to continue.");
                Console.ReadLine();
                return;
            }
            else if (room.AnimalId == null && room.RoomNumber != null)
            {
                Room changeRoom = new Room();
                changeRoom = db.Rooms.Where(r => r.AnimalId == animal.AnimalId).FirstOrDefault();
                if (changeRoom != null)
                {
                    changeRoom.AnimalId = null;
                }
                room.AnimalId = animal.AnimalId;
            }
            else
            {
                UserInterface.DisplayUserOptions("That room is already in use. Press any key to continue.");
                Console.ReadLine();
                return;
            }

            try
            {
                db.SubmitChanges();
                UserInterface.DisplayUserOptions("Animal has been moved.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static void RemoveAnimal(Animal animal) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room room = new Room();
            Adoption adoption = new Adoption();
            List<AnimalShot> animalShot = new List<AnimalShot>();
            animalShot = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).ToList();
            adoption = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            animal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            room = db.Rooms.Where(r => r.AnimalId == animal.AnimalId).FirstOrDefault();

            if(room != null)
            {
                room.AnimalId = null;
            }
            if(adoption != null)
            {
                db.Adoptions.DeleteOnSubmit(adoption);
            }
            if(animalShot != null)
            {
                foreach(AnimalShot shot in animalShot)
                {
                    db.AnimalShots.DeleteOnSubmit(shot);
                }
            }

            db.Animals.DeleteOnSubmit(animal);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static int? GetCategoryId(string type) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            int category = db.Categories.Where(a => a.Name.ToLower() == type.ToLower()).Select(a=>a.CategoryId).FirstOrDefault();

            if(category == 0)
            {
                int? newCategory = CreateCategoryId(type);
                return newCategory;
            }
            return category;
        }

        internal static int? CreateCategoryId(string type)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            UserInterface.DisplayUserOptions("That animal does not exist in the database, would you like to add it?");
            string input = UserInterface.GetUserInput();

            if (input.ToLower() == "yes" || input.ToLower() == "y")
            {
                Category NewCategory = new Category();
                NewCategory.Name = type;
                db.Categories.InsertOnSubmit(NewCategory);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return NewCategory.CategoryId;
            }
            UserInterface.DisplayUserOptions("The animal needs a category to be added into the database. Press any key to return.");
            Console.ReadLine();
            return null;
        }

        internal static int? GetDietPlanId(string type)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            int dietPlan = db.DietPlans.Where(a => a.Name == type ).Select(a => a.DietPlanId).FirstOrDefault();

            if(dietPlan == 0)
            {
                return null;
            }

            return dietPlan;
        }

        internal static void AddAnimal(Animal animal) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            db.Animals.InsertOnSubmit(animal);

            try
            {
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static void AddDiet(DietPlan diet)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            
            db.DietPlans.InsertOnSubmit(diet);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static void ChangeAnimalDiet(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Animal newAnimal = new Animal();
            DietPlan diet = new DietPlan();
            UserInterface.DisplayUserOptions("What DietPlanId would you like to change the animal to?");
            int newDiet = UserInterface.GetIntegerData();

            diet = db.DietPlans.Where(d => d.DietPlanId == newDiet).FirstOrDefault();

            if(diet == null)
            {
                UserInterface.DisplayUserOptions("That diet plan does not exist. Press any key to continue.");
                Console.ReadLine();
                return;
            }

            newAnimal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            newAnimal.DietPlanId = newDiet;

            try
            {
                db.SubmitChanges();
                UserInterface.DisplayUserOptions("Diet plan changed. Press any key to continue.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static void UpdateDiet(int dietId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            DietPlan diet = new DietPlan();

            diet = db.DietPlans.Where(d => d.DietPlanId == dietId).FirstOrDefault();

            if(diet == null)
            {
                UserInterface.DisplayUserOptions("That DietId does not exist. Press any button to continue.");
                Console.ReadLine();
                return;
            }

            diet.Name = UserInterface.GetStringData("name?", "the updated diet`s");
            diet.FoodType = UserInterface.GetStringData("food type?", "the updated diet`s");
            diet.FoodAmountInCups = UserInterface.GetIntegerData("serving amount?", "the updated diet`s");

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}