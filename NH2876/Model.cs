using System;

namespace NHibernate.Test.NHSpecificTest.NH2876
{
    public class Customer
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual CustomerData ExtendedData { get; set; }
    }

    public class CustomerData
    {
        public Guid CustomerId { get; set; }
        public string SomeData { get; set; }
    }
}
