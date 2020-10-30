using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H5Encryption.Models.DB
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool IsDone { get; set; }
        public User User { get; set; }
        //public int UserId { get; set; }

    }
}
