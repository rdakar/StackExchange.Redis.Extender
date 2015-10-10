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
            connectionRedis = ConnectionMultiplexer.Connect("pub-redis-13919.us-east-1-2.5.ec2.garantiadata.com:13919,password=salamandraverde");

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
        public async Task TestSetAsync()
        {
            IDatabase database = connectionRedis.GetDatabase();
            Task<bool> x = database.SetAsync("customer:" + customer.Id, customer);
            Thread.Sleep(100);
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
    }
}
