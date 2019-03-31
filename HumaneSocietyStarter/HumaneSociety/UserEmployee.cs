using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class UserEmployee : User
    {
        Employee employee;
                
        public override void LogIn()
        {
            if (CheckIfNewUser())
            {
                CreateNewEmployee();
                LogInPreExistingUser();
            }
            else
            {
                Console.Clear();
                LogInPreExistingUser();
            }
            RunUserMenus();
        }
        protected override void RunUserMenus()
        {
            List<string> options = new List<string>() { "What would you like to do? (select number of choice)", "1. Add animal", "2. Remove Anmial", "3. Check Animal Status",  "4. Approve Adoption", "5. Create New / Edit Existing Diet Plan", "6. Upload a CSV file" };
            UserInterface.DisplayUserOptions(options);
            string input = UserInterface.GetUserInput();
            RunUserInput(input);
        }

        private void RunUserInput(string input)
        {
            switch (input)
            {
                case "1":
                    AddAnimal();
                    RunUserMenus();
                    break;
                case "2":
                    RemoveAnimal();
                    RunUserMenus();
                    break;
                case "3":
                    CheckAnimalStatus();
                    RunUserMenus();
                    break;
                case "4":
                    CheckAdoptions();
                    RunUserMenus();
                    break;
                case "5":
                    DietDecision();
                    RunUserMenus();
                    break;
                case "6":
                    UserInterface.DisplayUserOptions("Please enter the location of the file: ");
                    string location = Console.ReadLine();
                    AddCSVFile(location);
                    break;
                default:
                    UserInterface.DisplayUserOptions("Input not accepted please try again");
                    RunUserMenus();
                    break;
            }
        }

        private void CheckAdoptions()
        {
            Console.Clear();
            List<string> adoptionInfo = new List<string>();
            int counter = 1;
            var adoptions = Query.GetPendingAdoptions().ToList();
            if(adoptions.Count > 0)
            {
                foreach(Adoption adoption in adoptions)
                {
                    adoptionInfo.Add($"{counter}. {adoption.Client.FirstName} {adoption.Client.LastName}, {adoption.Animal.Name} {adoption.Animal.Category}");
                    counter++;
                }
                UserInterface.DisplayUserOptions(adoptionInfo);
                UserInterface.DisplayUserOptions("Enter the number of the adoption you would like to approve");
                int input = UserInterface.GetIntegerData();
                ApproveAdoption(adoptions[input - 1]);
            }
            else
            {
                Console.WriteLine("There are no pending adoptions currently available. Please consult our animals database before setting up an adoption.");
            }

        }

        private void ApproveAdoption(Adoption adoption)
        {
            UserInterface.DisplayAnimalInfo(adoption.Animal);
            UserInterface.DisplayClientInfo(adoption.Client);
            UserInterface.DisplayUserOptions("Do you want to approve this adoption?");
            if ((bool)UserInterface.GetBitData())
            {
                Query.UpdateAdoption(true, adoption);
            }
            else
            {
                Query.UpdateAdoption(false, adoption);
            }
        }

        private void CheckAnimalStatus()
        {
            Console.Clear();

            Dictionary<int,string> searchDictionary =  UserInterface.GetAnimalCriteria();

            var animals = Query.SearchForAnimalByMultipleTraits(searchDictionary).ToList();
            if(animals.Count > 1)
            {
                UserInterface.DisplayUserOptions("Several animals found");
                UserInterface.DisplayAnimals(animals);
                UserInterface.DisplayUserOptions("Enter the ID of the animal you would like to check");
                int ID = UserInterface.GetIntegerData();
                CheckAnimalStatus(ID);
                return;
            }
            if(animals.Count == 0)
            {
                UserInterface.DisplayUserOptions("Animal not found please use different search criteria");
                return;
            }
            RunCheckMenu(animals[0]);
        }

        private void RunCheckMenu(Animal animal)
        {
            bool isFinished = false;
            Console.Clear();
            while(!isFinished){
                List<string> options = new List<string>() { "Animal found:", animal.Name, animal.Category.Name, "Would you like to:", "1. Get Info", "2. Update Info", "3. Check shots","4. Move animal","5. Change diet plan", "6. Return" };
                UserInterface.DisplayUserOptions(options);
                int input = UserInterface.GetIntegerData();
                if (input == 6)
                {
                    isFinished = true;
                    continue;
                }
                RunCheckMenuInput(input, animal);
            }
        }

        private void RunCheckMenuInput(int input, Animal animal)
        {
            
            switch (input)
            {
                case 1:
                    UserInterface.DisplayAnimalInfo(animal);
                    Console.Clear();
                    break;
                case 2:
                    UpdateAnimal(animal);
                    Console.Clear();
                    break;
                case 3:
                    CheckShots(animal);
                    Console.Clear();
                    break;
                case 4:
                    UserInterface.DisplayUserOptions("What room would you like to move the animal to?");
                    int newRoom = UserInterface.GetIntegerData();
                    Query.MoveAnimal(animal, newRoom);

                    break;
                case 5:
                    Query.ChangeAnimalDiet(animal);
                    break;
                default:
                    UserInterface.DisplayUserOptions("Input not accepted please select a menu choice");
                    break;
            }
        }

        private void CheckShots(Animal animal)
        {
            List<string> shotInfo = new List<string>();
            var shots = Query.GetShots(animal);
            foreach(AnimalShot shot in shots.ToList())
            {
                shotInfo.Add($"{shot.Shot.Name} Date: {shot.DateReceived}");
            }

            if (shotInfo.Count > 0)
            {
                UserInterface.DisplayUserOptions(shotInfo);
            }
            else
            {
                UserInterface.DisplayUserOptions("There are no shots available for this animal.");
            }
            if (UserInterface.GetBitData("Would you like to Update shots?"))
            {
                Console.WriteLine("Which shot does your animal need?");
                int userInput = UserInterface.GetIntegerData();
               
                Query.UpdateShot(userInput, animal);

                UserInterface.DisplayUserOptions("Press enter to return to menu.");
                Console.ReadLine();
            }
        }

        public void UpdateAnimal(Animal animal, Dictionary<int, string> updates = null)
        {
            if(updates == null)
            {
                updates = new Dictionary<int, string>();
            }

            List<string> options = new List<string>() { "Select Update:", "1. Category", "2. Name", "3. Age", "4. Demeanor", "5. Kid friendly", "6. Pet friendly", "7. Weight", "8. Finished", "You will be prompted again for any additional updates." };
            UserInterface.DisplayUserOptions(options);
            string input = UserInterface.GetUserInput();
            if(input.ToLower() == "8" ||input.ToLower() == "finished")
            {
                Query.EnterAnimalUpdate(animal, updates);
            }
            else
            {
                updates = UserInterface.EnterSearchCriteria(updates, input);
                UpdateAnimal(animal, updates);
            }
        }

        private void CheckAnimalStatus(int iD)
        {
            Console.Clear();
            var animals = SearchForAnimal(iD).ToList();
            if (animals.Count == 0)
            {
                UserInterface.DisplayUserOptions("Animal not found please use different search criteria");
                return;
            }
            RunCheckMenu(animals[0]);
        }

        private IQueryable<Animal> SearchForAnimal(int iD)
        {
            HumaneSocietyDataContext context = new HumaneSocietyDataContext();
            var animals = (from animal in context.Animals where animal.AnimalId == iD select animal);
            return animals;
        }       

        private void RemoveAnimal()
        {

            Dictionary<int, string> searchDictionary = UserInterface.GetAnimalCriteria();
            var animals = Query.SearchForAnimalByMultipleTraits(searchDictionary).ToList();
            while (animals.Count > 1)
            {
                UserInterface.DisplayUserOptions("Several animals found please refine your search.");
                UserInterface.DisplayAnimals(animals);
                UserInterface.DisplayUserOptions("Press enter to continue searching");
                Console.ReadLine();
                searchDictionary = UserInterface.GetAnimalCriteria();
                animals = Query.SearchForAnimalByMultipleTraits(searchDictionary).ToList();
            }
            if (animals.Count < 1)
            {
                UserInterface.DisplayUserOptions("Animal not found please use different search criteria");
                return;
            }
            var animal = animals[0];
            List<string> options = new List<string>() { "Animal found:", animal.Name, animal.Category.Name, "would you like to delete?" };
            if ((bool)UserInterface.GetBitData(options))
            {
                Query.RemoveAnimal(animal);
            }
            else
            {
                UserInterface.DisplayUserOptions("Animal was not deleted. Press any key to continue.");
                Console.ReadLine();
            }
        }

        public void AddAnimal()
        {
            Console.Clear();
            Animal animal = CreateAnimal();
            if (Query.CheckIfEmptyRoom())
            {
                Query.AddAnimal(animal);
                Query.PlaceAnimalIntoRoom(animal.AnimalId);
            }
            else
            {
                UserInterface.DisplayUserOptions("There are no open rooms at this time. The animal has not been admitted. Press any key to continue.");
                Console.ReadLine();
            }
            
        }

        public void AddCSVFile(string fileName)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(@fileName))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    while (!parser.EndOfData)
                    {
                        string[] animalTrait = parser.ReadFields();

                        Animal animalToAdd = new Animal();
                        animalToAdd.Name = animalTrait[0].Trim('"');
                        int? intParseValue = UserInterface.GetCsvIntData(animalTrait[2]);
                        animalToAdd.Weight = intParseValue;
                        string testedInput = UserInterface.CsvNullChecker(animalTrait[1]);
                        while (testedInput == null)
                        {
                            UserInterface.DisplayUserOptions("Please enter the animal`s species: ");
                            string input = Console.ReadLine();
                            intParseValue = Query.GetCategoryId(input);
                            if (intParseValue != null)
                            {
                                testedInput = "Not null";
                            }
                        }
                        animalToAdd.CategoryId = intParseValue;
                        intParseValue = UserInterface.GetCsvIntData(animalTrait[3]);
                        animalToAdd.Age = intParseValue;
                        intParseValue = UserInterface.GetCsvIntData(animalTrait[4]);
                        animalToAdd.DietPlanId = intParseValue;
                        animalToAdd.Demeanor = animalTrait[5].Trim('"');
                        bool? newFriendlyStatus = UserInterface.GetCsvBoolData(animalTrait[6]);
                        animalToAdd.KidFriendly = newFriendlyStatus;
                        newFriendlyStatus = UserInterface.GetCsvBoolData(animalTrait[7]);
                        animalToAdd.PetFriendly = newFriendlyStatus;
                        animalToAdd.Gender = animalTrait[8];
                        animalToAdd.AdoptionStatus = animalTrait[9].Trim('"');
                        intParseValue = UserInterface.GetCsvIntData(animalTrait[10]);
                        animalToAdd.EmployeeId = intParseValue;

                        if (Query.CheckIfEmptyRoom())
                        {
                            Query.AddAnimal(animalToAdd);
                            Query.PlaceAnimalIntoRoom(animalToAdd.AnimalId);
                        }
                        else
                        {
                            UserInterface.DisplayUserOptions("There are no open rooms at this time. The animal has not been admitted. Press any key to continue.");
                            Console.ReadLine();
                            RunUserMenus();
                        }
                    }
                    RunUserMenus();
                }
            }
            catch
            {
                UserInterface.DisplayUserOptions("There is something wrong with the file you are attempting to load or the file does not exist. Please check it and try again. Press any key to continue.");
                Console.ReadLine();
                RunUserMenus();
            }
        }

        private Animal CreateAnimal()
        {
            Animal animal = new Animal();
            string type = UserInterface.GetStringData("species", "the animal`s");
            animal.CategoryId = Query.GetCategoryId(type);
            if (animal.CategoryId == null) { RunUserMenus(); }
            animal.Name = UserInterface.GetStringData("name", "the animal's");
            animal.Age = UserInterface.GetIntegerData("age", "the animal's");
            animal.Demeanor = UserInterface.GetStringData("demeanor", "the animal's");
            animal.KidFriendly = UserInterface.GetBitData("the animal", "child friendly");
            animal.PetFriendly = UserInterface.GetBitData("the animal", "pet friendly");
            animal.Weight = UserInterface.GetIntegerData("the animal", "the weight of the");
            animal.Gender = UserInterface.GetStringData("gender", "the animal's");
            animal.AdoptionStatus = UserInterface.GetStringData("adoption status", "the animal's");
            animal.EmployeeId = employee.EmployeeNumber;
            animal.DietPlanId = Query.GetDietPlanId(type);
            return animal;
        }

        private void DietDecision()
        {
            UserInterface.DisplayUserOptions("Would you like to update a diet plan or create a new one? Type update or create.");
            string decisions = UserInterface.GetUserInput();
            if(decisions.ToLower() == "update")
            {
                UpdateDiet();
            }
            if(decisions.ToLower() == "create")
            {
                CreateDiet();
            }
            
        }    
        
        private void UpdateDiet()
        {
            UserInterface.DisplayUserOptions("What is the Diet Id you would like to edit?");
            int dietId = UserInterface.GetIntegerData();
            Query.UpdateDiet(dietId);
        }

        private void CreateDiet()
        {
            DietPlan Diet = new DietPlan();
            Diet.Name = UserInterface.GetStringData("name", "the diet's");
            Diet.FoodType = UserInterface.GetStringData("food type", "the animal's");
            UserInterface.DisplayUserOptions("How much food does the animal need per serving?");
            int foodinCups = UserInterface.GetIntegerData();
            Diet.FoodAmountInCups = foodinCups;
            Query.AddDiet(Diet);
            UserInterface.DisplayUserOptions("You have added a new dietplan. The name of the diet plan is " + Diet.Name + ". The food type is " + Diet.FoodType + ". " + Diet.FoodAmountInCups + " cups per serving is the recommended amount.");
            UserInterface.DisplayUserOptions("Press enter to continue.");
            Console.ReadLine();
        }

        protected override void LogInPreExistingUser()
        {
            List<string> options = new List<string>() { "Please log in", "Enter your username" };
            UserInterface.DisplayUserOptions(options);
            userName = UserInterface.GetUserInput();
            UserInterface.DisplayUserOptions("Enter your password: ");
            string password = UserInterface.GetUserInput();
            Console.Clear();
            employee = Query.EmployeeLogin(userName, password);
            if (employee == null)
            {
                UserInterface.DisplayUserOptions("Wrong username or password. Please try again, create a new user, or contact your administrator.");
                LogIn();
            }
            UserInterface.DisplayUserOptions("Login successfull. Welcome.");
        }

        private void CreateNewEmployee()

        {
            Console.Clear();
            string email = UserInterface.GetStringData("email", "your");
            int employeeNumber = int.Parse(UserInterface.GetStringData("employee number", "your"));
            try
            {
                employee = Query.RetrieveEmployeeUser(email, employeeNumber);
            }
            catch
            {
                UserInterface.DisplayUserOptions("Employee not found please contact your administrator");
                PointOfEntry.Run();
            }
            if (employee.Password != null)
            {
                UserInterface.DisplayUserOptions("User already in use please log in or contact your administrator");
                LogIn();
                return;
            }
            else
            {
                UpdateEmployeeInfo();
            }
        }

        private void UpdateEmployeeInfo()
        {
            GetUserName();
            GetPassword();
            Query.AddUsernameAndPassword(employee);
        }

        private void GetPassword()
        {
            UserInterface.DisplayUserOptions("Please enter your password: (CaSe SeNsItIvE)");
            employee.Password = UserInterface.GetUserInput();
        }

        private void GetUserName()
        {
            Console.Clear();
            string username = UserInterface.GetStringData("username", "your");
            if (Query.CheckEmployeeUserNameExist(username))
            {
                UserInterface.DisplayUserOptions("Username already in use please try another username.");
                GetUserName();
            }
            else
            {
                employee.UserName = username;
                UserInterface.DisplayUserOptions("Username successful");
            }
        }
    }
}
