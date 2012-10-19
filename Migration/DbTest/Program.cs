using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var migDb = new MigDbEntities();

            var query = (from p in migDb.PersonDatas
                         join f in migDb.DataFields on p.GetPersonId() equals f.GetPersonId()

                         where p.C__Id == 10
                         select new { Value = f.Value, PesonId = f.GetPersonId() }).ToList();




            Console.ReadLine();

        }
    }

    internal class Test
    {
        public string Name { get; set; }
    }
}
