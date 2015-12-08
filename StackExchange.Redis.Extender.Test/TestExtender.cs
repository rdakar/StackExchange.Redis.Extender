using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace StackExchange.Redis.Extender.Test
{
    [TestClass]
    public class TestExtender
    {
        private Customer customer;
        private ConnectionMultiplexer connectionRedis;

        [TestInitialize]
        public void Initialize()
        {
            connectionRedis = ConnectionMultiplexer.Connect("localhost:13919,password=senhadoredis");

            customer = new Customer();
            customer.Id = 12;
            customer.Age = 30;
            customer.Name = "Customer";
        }

        [TestCleanup]
        public void Cleanup()
        {
            connectionRedis.Close();
        }

        [TestMethod]
        public void TestConnection()
        {
            Assert.IsTrue(connectionRedis.IsConnected);
        }

        [TestMethod]
        public void TestSet()
        {
            IDatabase database = connectionRedis.GetDatabase();
            Assert.IsTrue(database.Set("customer:" + customer.Id, customer));
        }

        [TestMethod]
        public void TestSetExpiry()
        {
            IDatabase database = connectionRedis.GetDatabase();
            Assert.IsTrue(database.Set("customerexpiry:" + customer.Id, customer, TimeSpan.FromSeconds(120)));

            TimeSpan? timeExpiry = database.KeyTimeToLive("customerexpiry:" + customer.Id);
            Assert.AreEqual(timeExpiry.Value.TotalSeconds, 120, 5);
        }

        [TestMethod]
        public void TestGet()
        {
            IDatabase database = connectionRedis.GetDatabase();
            database.Set("customer:" + customer.Id, customer);
            Customer customerGet = database.Get<Customer>("customer:" + customer.Id);
            Assert.AreEqual(customer.ToString(), customerGet.ToString());
        }

        [TestMethod]
        public void TestGetObject()
        {
            IDatabase database = connectionRedis.GetDatabase();
            database.Set("customer:" + customer.Id, customer);
            object customerGet = database.Get("customer:" + customer.Id);
            string json = JsonConvert.SerializeObject(customer);
            object customerObject = JsonConvert.DeserializeObject<object>(json);
            Assert.AreEqual(customerObject.ToString(), customerGet.ToString());
        }

        [TestMethod]
        public void TestGetSet()
        {
            //set Customer
            IDatabase database = connectionRedis.GetDatabase();
            database.Set("customer:" + customer.Id, customer);
            //get Customer
            Customer customerGet = database.Get<Customer>("customer:" + customer.Id);
            Assert.AreEqual(customer.ToString(), customerGet.ToString());
            //create a New Customer
            Customer newCustomer = new Customer();
            newCustomer.Id = 13;
            newCustomer.Age = 25;
            newCustomer.Name = "New Customer";
            //set New Customer and get Old Customer
            customerGet = database.GetSet<Customer>("customer:" + customer.Id, newCustomer);
            Assert.AreEqual(customer.ToString(), customerGet.ToString());
            //get New Customer
            customerGet = database.Get<Customer>("customer:" + customer.Id);
            Assert.AreEqual(newCustomer.ToString(), customerGet.ToString());
        }

        [TestMethod]
        public async Task TestSetAsync()
        {
            IDatabase database = connectionRedis.GetDatabase();
            Task<bool> x = database.SetAsync("customer:" + customer.Id, customer);
            bool b = await x;
            Assert.IsTrue(b);
        }

        [TestMethod]
        public async Task TestSetExpiryAsync()
        {
            IDatabase database = connectionRedis.GetDatabase();
            bool b = await database.SetAsync("customerexpiry:" + customer.Id, customer, TimeSpan.FromSeconds(120));
            Assert.IsTrue(b);
            TimeSpan? timeExpiry = database.KeyTimeToLive("customerexpiry:" + customer.Id);
            Assert.AreEqual(timeExpiry.Value.TotalSeconds, 120, 5);
        }

        [TestMethod]
        public void TestSetHash()
        {
            IDatabase database = connectionRedis.GetDatabase();
            Assert.IsTrue(database.HashSet("customer", customer.Id.ToString(), customer));
            database.HashDelete("customer", customer.Id.ToString());
        }

        [TestMethod]
        public void TestHashGet()
        {
            IDatabase database = connectionRedis.GetDatabase();
            database.HashSet("customer", customer.Id.ToString(), customer);
            Customer customerGet = database.HashGet<Customer>("customer", customer.Id.ToString());
            Assert.AreEqual(customer.ToString(), customerGet.ToString());
            database.HashDelete("customer", customer.Id.ToString());
        }

        [TestMethod]
        public void TestHashGetAll()
        {
            IDatabase database = connectionRedis.GetDatabase();
            database.HashSet("customer", customer.Id.ToString(), customer);
            var allHashes = database.HashGetAll<Customer>("customer");
            Customer customerGet = null;
            allHashes.TryGetValue("12", out customerGet);
            Assert.AreEqual(customer.ToString(), customerGet.ToString());
            database.HashDelete("customer", customer.Id.ToString());
        }
    }
}
