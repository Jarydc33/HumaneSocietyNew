using System;
using System.Collections.Generic;
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

            // if the address isn't found in the Db, create and insert it
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

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
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

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void RunEmployeeQueries(Employee employee, string v) //FIX
        {
            throw new NotImplementedException();
        }

        internal static Room GetRoom(int animalId) //FIX
        {
            //HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            //var RoomNumber = db.Rooms.Where(a => a.AnimalId == animalId).Select(RoomNumber).Single();

            //return animalsById;
            throw new NotImplementedException();
        }

        internal static void Adopt(object animal, Client client) //FIX
        {
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int iD)
        {
            //db is the database
            //Animals is the animals table
            //Where is filtering by a boolean condition
            //Single is grabbing that single instance
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalsById = db.Animals.Where(a => a.AnimalId == iD).Single();
            return animalsById;
        }


        internal static void UpdateAdoption(bool v, Adoption adoption) //FIX
        {
            throw new NotImplementedException();
        }

        internal static List<Animal> SearchForAnimalByMultipleTraits() //FIX
        {
            throw new NotImplementedException();
        }

        internal static List<Adoption> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            var adoptionFromDb = db.Adoptions.Where(a => a.ApprovalStatus == "pending").ToList();
            return adoptionFromDb;
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException("Sorry, we dont recognize that");   //FIX          
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

        internal static List<AnimalShot> GetShots(Animal animal) //FIX
        {
            throw new NotImplementedException();
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void UpdateShot(string v, Animal animal) //FIX
        {
            throw new NotImplementedException();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates) //FIX
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal) //FIX
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD

        internal static Category GetCategoryId(int iD)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Console.WriteLine("Please search for a category");
            Console.ReadLine();
            var CategoryId = db.Categories.Where(c => c.CategoryId == iD).Single();
        
            return CategoryId;
=======
        internal static int? GetCategoryId(string type) //FIX
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            int category = db.Categories.Select(a => a.CategoryId).FirstOrDefault();
            return category;
>>>>>>> ef7c2ff497a631a236e502e809fe178a8c2185d8
        }


        internal static DietPlan GetDietPlanId(int iD)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var DietPlanId = db.DietPlans.Where(d => d.DietPlanId == iD).Single();
            return DietPlanId;
        }


        internal static void AddAnimal(Animal animal) //FIX
        {
            throw new NotImplementedException();
        }
    }
}