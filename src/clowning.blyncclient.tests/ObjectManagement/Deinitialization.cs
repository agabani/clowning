using System;
using NUnit.Framework;

namespace clowning.blyncclient.tests.ObjectManagement
{
    [TestFixture]
    public class Deinitialization
    {
        private BlyncClient _blyncClient;

        [Test]
        public void Should_be_able_to_uninitalize_device_correcty()
        {
            When_I_initalize_a_new_disposable_client();
            Then_the_client_should_be_disposed_correctly();
        }

        [Test]
        public void Should_not_be_able_to_access_device_types()
        {
            When_I_initalize_a_new_disposable_client();
            Then_I_should_get_an_invalid_operation_exception(When_I_access_the_device_types());
        }

        [Test]
        public void Should_not_be_able_to_access_device_type()
        {
            When_I_initalize_a_new_disposable_client();
            Then_I_should_get_an_invalid_operation_exception(When_I_access_a_device_type());
        }

        private void When_I_initalize_a_new_disposable_client()
        {
            using (_blyncClient = new BlyncClient())
            {
            }
        }

        private TestDelegate When_I_access_the_device_types()
        {
            return () => _blyncClient.GetDeviceTypes();
        }

        private TestDelegate When_I_access_a_device_type()
        {
            return () => _blyncClient.GetDeviceType(0);
        }

        private void Then_the_client_should_be_disposed_correctly()
        {
            Assert.That(_blyncClient.NumberOfDevices, Is.EqualTo(0));
        }

        private void Then_I_should_get_an_invalid_operation_exception(TestDelegate action)
        {
            Assert.Throws<InvalidOperationException>(action);
        }
    }
}