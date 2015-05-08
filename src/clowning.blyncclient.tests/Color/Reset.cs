using System;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Color
{
    [TestFixture]
    public class Reset
    {
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_reset_light()
        {
            Given_that_I_have_a_client();
            When_I_reset_the_color_of_all_devices();
            Then_color_should_have_been_reset();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100000)]
        public void Should_not_be_able_to_reset_invalid_device(int deviceNumber)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_reset_color(deviceNumber));
        }

        private void Given_that_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_reset_the_color_of_all_devices()
        {
            _successful = true;

            for (int device = 0; device < _blyncClient.NumberOfDevices; device++)
            {
                ResetColor(device);
            }
        }

        private TestDelegate When_I_reset_color(int deviceNumber)
        {
            return () => ResetColor(deviceNumber);
        }

        private void Then_color_should_have_been_reset()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }

        private void ResetColor(int device)
        {
            _successful &= _blyncClient.ResetLight(device);
        }
    }
}