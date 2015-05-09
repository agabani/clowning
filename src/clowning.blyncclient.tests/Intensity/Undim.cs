using System;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Intensity
{
    [TestFixture]
    public class Undim
    {
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_undim_light()
        {
            Given_that_I_have_a_client();
            When_I_undim_the_device();
            Then_the_device_should_be_undim();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100000)]
        public void Should_not_be_able_to_undim_invalid_device(int deviceNumber)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_undim(deviceNumber));
        }

        private void Given_that_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_undim_the_device()
        {
            _successful = true;

            for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
            {
                _successful &= _blyncClient.SetDim(device, false);
            }
        }

        private TestDelegate When_I_undim(int deviceNumber)
        {
            return () => _blyncClient.SetDim(deviceNumber, false);
        }

        private void Then_the_device_should_be_undim()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}