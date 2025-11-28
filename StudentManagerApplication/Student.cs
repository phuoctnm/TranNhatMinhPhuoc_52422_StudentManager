using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagerApplication
{
    public class Student
    {
        // Định nghĩa các thuộc tính dựa trên thiết kế 
        public string ID { get; set; }
        public string FullName { get; set; }
        public string ClassName { get; set; }
        public string Gender { get; set; }
        public double Score { get; set; }
    }
}