using System.Linq;
using NUnit.Framework;

namespace clowning.blyncclient.tests.ObjectManagement
{
    [TestFixture]
    public class Initialization
    {
        private BlyncClient _blyncClient;

        [Test]
        public void Should_be_able_to_initialize_device_correctly()
        {
            When_I_initalize_a_new_client();
            Then_the_client_should_be_initialized_correctly();
        }

        [Test]
        public void Should_be_able_to_get_device_types()
        {
            When_I_initalize_a_new_client();
            Then_I_should_get_a_list_of_all_device_types_connected();
        }

        private void When_I_initalize_a_new_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void Then_the_client_should_be_initialized_correctly()
        {
            Assert.That(_blyncClient.NumberOfDevices, Is.GreaterThan(0));
        }

        private void Then_I_should_get_a_list_of_all_device_types_connected()
        {
            Assert.That(_blyncClient.GetDeviceTypes().Count(), Is.GreaterThan(0));
            Assert.That((int)_blyncClient.GetDeviceType(0), Is.GreaterThan(0));
        }
    }
}