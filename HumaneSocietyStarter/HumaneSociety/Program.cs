﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class Program
    {
        static void Main(string[] args)
        {
            //UserEmployee ue = new UserEmployee();
            //ue.UpdateAnimal();
            //ue.UpdateAnimal();
            UserEmployee ue = new UserEmployee();
            ue.ReadCSVFile();

            PointOfEntry.Run();
        }
    }
}
