using System;
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
            var animalsById = db.Animals.Where(a => a.AnimalId == iD).Single();
            return animalsById;
        }


        internal static void UpdateAdoption(bool v, Adoption adoption) //FIX
        {
            throw new NotImplementedException();
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
                            animals = animals.Where(a => a.Name == criteria.Value).ToList();
                            break;

                        case 3:
                            int age = int.Parse(criteria.Value);
                            animals = animals.Where(a => a.Age == age).ToList();
                            break;

                        case 4:
                            animals = animals.Where(a => a.Demeanor == criteria.Value).ToList();
                            break;

                        case 5:
                            
                            if(criteria.Value.ToLower() == "yes")
                            {
                                animals = animals.Where(a => a.KidFriendly == true).ToList();
                            }
                            else
                            {
                                animals = animals.Where(a => a.KidFriendly == false).ToList();
                            }
                            break;

                        case 6:
                            if (criteria.Value.ToLower() == "yes")
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

            var adoptionFromDb = db.Adoptions.Where(a => a.ApprovalStatus == "Pending").ToList();
            return adoptionFromDb;
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException("Sorry, we dont recognize that");           
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
            var shotsFromDb = db.AnimalShots.ToList();
            return shotsFromDb;





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
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            AnimalShot animalShot = new AnimalShot();
            animalShot.AnimalId = animal.AnimalId;
            int id = int.Parse(v);
            animalShot.ShotId = id;
            animalShot.DateReceived = DateTime.Now;
            
            //var newAnimalShot = db.Animals.Where(x => x. = animal.AnimalId);
            db.AnimalShots.InsertOnSubmit(animalShot);
            try
            {
                db.SubmitChanges();

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            //var UnVaccedAnimal = db.Animals.GetAnimalByID();


            //AnimalShot newAnimalShot = new AnimalShot();            
            //db.AnimalShots.In


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

        internal static void EnterAnimalUpdate(Animal animals, Dictionary<int, string> updates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room room = new Room();
            animals = db.Animals.Where(a => a.AnimalId == animals.AnimalId).FirstOrDefault();

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

                        if (criteria.Value.ToLower() == "yes")
                        {
                            animals.KidFriendly = true;
                        }
                        else
                        {
                            animals.KidFriendly = false;
                        }
                        break;

                    case 6:

                        if (criteria.Value.ToLower() == "yes")
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

                    case 8:
                        room = db.Rooms.Where(r => r.AnimalId == animals.AnimalId).FirstOrDefault();
                        if(room.RoomNumber != null)
                        {
                            UserInterface.DisplayUserOptions("That room is already in use.");
                        }
                        else
                        {
                            room.RoomNumber = int.Parse(criteria.Value);
                        }
                        
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

        internal static void RemoveAnimal(Animal animal) //FIX
        {
            throw new NotImplementedException();
        }

        internal static int? GetCategoryId(string type) 
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            int category = db.Categories.Where(a => a.Name == type).Select(a=>a.CategoryId).FirstOrDefault();

            if(category == 0)
            {
                CreateCategoryId(type);
            }

            return category;
        }

        internal static void CreateCategoryId(string type)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            UserInterface.DisplayUserOptions("That animal does not exist in the database, would you like to add it?");
            string input = UserInterface.GetUserInput();

            if (input == "yes" || input == "y")
            {
                Category newcategory = new Category();
                newcategory.Name = type;
                db.Categories.InsertOnSubmit(newcategory);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            GetCategoryId(type);
        }

        internal static int? GetDietPlanId(string type)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            int dietPlan = db.DietPlans.Where(a => a.Name == type ).Select(a => a.DietPlanId).FirstOrDefault();

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
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}