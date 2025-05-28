using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Username { get; set; }
        public int Score { get; set; }
    }
}
