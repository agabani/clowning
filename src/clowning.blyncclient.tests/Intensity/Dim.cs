using System;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Intensity
{
    [TestFixture]
    public class Dim
    {
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_dim_light()
        {
            Given_that_I_have_a_client();
            When_I_dim_the_device();
            Then_the_device_should_be_dim();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100000)]
        public void Should_not_be_able_to_dim_invalid_device(int deviceNumber)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_dim(deviceNumber));
        }

        private void Given_that_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_dim_the_device()
        {
            _successful = true;

            for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
            {
                _successful &= _blyncClient.DimLight(device, true);
            }
        }

        private TestDelegate When_I_dim(int deviceNumber)
        {
            return () => _blyncClient.DimLight(deviceNumber, true);
        }

        private void Then_the_device_should_be_dim()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}