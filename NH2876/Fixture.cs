using System;
using System.Collections.Generic;
using System.Linq;

using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Linq;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2876
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void Configure(Configuration configuration)
        {
            // configuration.AppendListeners(ListenerType.PreUpdate, new [] { new PreUpdateEventListener()});
            base.Configure(configuration);
        }

        [Test]
        public void JoinElement_WithComponent_CanBeCreatedWithNoExtendedInstance()
        {
            var c = new Customer { Name = "Customer" };

            var id = Insert(c);

            AssertValid(id, "Customer");
        }

        [Test]
        public void JoinElement_WithComponent_CanBeUpdatedWithNoExtendedInstance()
        {
            var c = new Customer { Name = "Customer" };

            var id = Insert(c);
            Update(id);

            AssertValid(id, "Updated");
        }

        [Test]
        public void JoinElement_WithComponent_CanBeCreatedWithExtendedInstanceAndNoData()
        {
            var c = new Customer { Name = "Customer", ExtendedData = new CustomerData() };

            var id = Insert(c);

            AssertValid(id, "Customer");
        }

        [Test]
        public void JoinElement_WithComponent_CanBeUpdatedWithExtendedInstanceAndNoData()
        {
            var c = new Customer { Name = "Customer", ExtendedData = new CustomerData() };

            var id = Insert(c);
            Update(id);

            AssertValid(id, "Updated");
        }

        [Test]
        public void JoinElement_WithComponent_CanBeUpdatedWithNoExtendedInstanceAndDataOnUpdateOnly()
        {
            var c = new Customer { Name = "Customer" };

            var id = Insert(c);
            Update(id, new CustomerData { SomeData = "Test" });

            AssertValid(id, "Updated", "Test");
        }

        [Test] //failing
        public void JoinElement_WithComponent_CanBeUpdatedWithExtendedInstanceAndDataOnUpdateOnly()
        {
            var c = new Customer { Name = "Customer", ExtendedData = new CustomerData() };

            var id = Insert(c);
            Update(id, new CustomerData { SomeData = "Test" });

            AssertValid(id, "Updated", "Test");
        }

        [Test]
        public void JoinElement_WithComponent_CanBeCreatedWithExtendedInstanceAndData()
        {
            var c = new Customer { Name = "Customer", ExtendedData = new CustomerData { SomeData = "Test" } };

            var id = Insert(c);

            AssertValid(id, "Customer", "Test");
        }

        [Test]
        public void JoinElement_WithComponent_CanBeUpdatedWithExtendedInstanceAndData()
        {
            var c = new Customer { Name = "Customer", ExtendedData = new CustomerData { SomeData = "Test" } };

            var id = Insert(c);
            Update(id);

            AssertValid(id, "Updated", "Test");
        }

        [Test]
        public void JoinElement_WithComponent_GetCorrectAmountOfRecordsRegardlessOfHowCreated()
        {
            var withoutExtendedData = new Customer { Name = "Customer" };
            var withoutCustomerData = new Customer { Name = "Customer", ExtendedData = new CustomerData() };
            var withData = new Customer { Name = "Customer", ExtendedData = new CustomerData { SomeData = "Test" } };

            Insert(withoutExtendedData);
            Insert(withoutCustomerData);
            Insert(withData);

            AssertCount(3);
        }

        private Guid Insert(Customer c)
        {
            Guid id;
            using (var s = sessions.OpenSession())
            {
                using (ITransaction t = s.BeginTransaction())
                {
                    s.Save(c);
                    id = c.Id;
                    t.Commit();
                }
            }
            return id;
        }

        private void Update(Guid id, CustomerData customerData = null)
        {
            using (var s = sessions.OpenSession())
            {
                using (ITransaction t = s.BeginTransaction())
                {
                    var updated = s.Get<Customer>(id);
                    updated.Name = "Updated";
                    if (customerData != null)
                    {
                        updated.ExtendedData = customerData;
                    }

                    t.Commit();
                }
            }
        }

        private void AssertValid(Guid id, string name, string data = null)
        {
            using (var s = sessions.OpenSession())
            {
                var result = s.Get<Customer>(id);
                Assert.AreEqual(name, result.Name);
                if (data != null)
                {
                    Assert.AreEqual(data, result.ExtendedData.SomeData);
                }
            }
        }

        private void AssertCount(int count)
        {
            using (var s = sessions.OpenSession())
            {
                var result = s.Query<Customer>();
                Assert.AreEqual(count, result.Count());
            }
        }
        
        protected override void OnTearDown()
        {
            using (ISession s = sessions.OpenSession())
            {
                using (ITransaction t = s.BeginTransaction())
                {
                    s.Delete("from Customer");
                    t.Commit();
                }
            }
        }
    }
}