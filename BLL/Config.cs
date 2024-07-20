using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.BLL
{
	internal class Config
	{
        public int ServerPort { get; set; }
        public string AdminsPath { get; set; }
        public string DishesPath { get; set; }
        public string ClientsPath { get; set; }
    }
}
