using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Demo
{
    #region Ignore. Supporting code to make things compile.

    class AcmeDatabase : IDisposable
    {
        public void Dispose()
        {}

        public List<Customer> Customers
        {
            get
            {
                return new List<Customer>();
            }
        }

    }

    class Customer
    {
        public int TotalOrdersPlaced = 0;   
        public string FirstName = "";
        public string LastName = "";
    }

    class DoNotDoThatException : Exception
    {

    }

    class ThingThatShouldBlowUp
    {

    }
    #endregion

    class Program
    {

        #region Ignore
 
        static void Main(string[] args)
        {
            Console.WriteLine("This app does nothing!");
        }

        // This is just a stub to stand in for a real database object. Just ignore this.
        public AcmeDatabase database()
        {
            return new AcmeDatabase();
        }
        
        #endregion


        #region Generics

        public static List<Customer> customers = new List<Customer>()
        {
            new Customer {FirstName = "Foo", LastName = "Bar"},
            new Customer {FirstName = "Pete", LastName = "Whatever"},
        };

        #endregion


        #region LINQ

        public List<Customer> AwesomeCustomers()
        {
            using (AcmeDatabase db = database())
            {
                return (from c in db.Customers
                       where c.TotalOrdersPlaced > 500
                       orderby c.FirstName
                       select c).ToList();
            }
        }

        // Show join?

        #endregion


        #region Lambdas

        // Best-looking lambdas ever. Suck it, Haskell! :)

        public List<Customer> CheapestCustomers(List<Customer> customers)
        {
            return customers.FindAll(c => c.TotalOrdersPlaced < 5);
        }

        // The example that I didn't show, that wouldn't make much sense
        // in a pure functional language is a zero-parameter lambda, as in 
        // this unit test:
        public void TestSomething()
        {
            // 'Assert.Throws' is a method in NUnit that executes a lambda (right param) 
            // and then verifies that an exception of the type specified (left param)
            // is thrown.
            
            // Assert.Throws(typeof(DoNotDoThatException), () => new ThingThatShouldBlowUp());
        }

        #endregion


        #region Extended Collections
        
        // This was here mostly as a talking point, which the IDE could inspect
        // and show methods for. It could probably use some parametric awesomeness 
        // like the InvertDictionary method was given.
        public Dictionary<string, int> CustomersToDictionary(List<Customer> customers)
        {
            return customers.ToDictionary<Customer, string, int>(c => c.LastName, c => c.TotalOrdersPlaced);
        }
        
        #endregion


        #region Type Inference 1

        public List<string> BestCustomerNames(List<Customer> customers)
        {
            var bestCustomers = customers.Where(c => c.TotalOrdersPlaced > 100);

            var names = bestCustomers.Select(c => c.FirstName + " " + c.LastName);

            return names.ToList();
        }

        #endregion


        #region Type Inference 2
        // Or...
        public List<string> BetterCustomerNames(List<Customer> customers)
        {
            return customers
                   .Where(c => c.TotalOrdersPlaced > 100)
                   .Select(c => c.FirstName + " " + c.LastName)
                   .ToList();
        }

        #endregion


        #region Type Inference 3
        // Or...
        public List<string> BestestCustomerNames(List<Customer> customers)
        {
            return (from c in customers
                    where c.TotalOrdersPlaced > 100
                    select c.FirstName + " " + c.LastName).ToList();
        }

        #endregion


        #region Misc Oddness 1 - Dictionary

        // Original with explicitly-typed madness everywhere.
        private Dictionary<string, string> InvertDictionary(Dictionary<string, string> d)
        {
            return d.ToDictionary<KeyValuePair<string, string>, string, string>(entry => entry.Value, entry => entry.Key);
        }

        // And new version, as created by the hive mind of PDX Func :)
        // Looks much better. 
        private Dictionary<V, K> InvertDictionaryPlusPlus<K, V>(Dictionary<K, V> d)
        {
            return d.ToDictionary(entry => entry.Value, entry => entry.Key);
        }
        #endregion
        

        #region Misc Oddness 2 - Didn't get time for this one.
        // The point is that it is hard to dynamically build a query with LINQ and/or 
        // the extended classes that support LINQ.
        public List<Customer> CustomerQuery(string name, int minOrderCount)
        {
            List<Expression<Func<Customer, bool>>> conditions = new List<Expression<Func<Customer, bool>>>();

            if (name != null) conditions.Add(t => t.FirstName == "Steve");
            if (minOrderCount > 0) conditions.Add(t => t.TotalOrdersPlaced > minOrderCount);

            using (AcmeDatabase db = database())
            {
                var customers = db.Customers.AsQueryable();

                foreach (var condition in conditions)
                {
                    customers = customers.Where(condition);
                }

                return customers.ToList();
            }
        }

        #endregion
    }
}
