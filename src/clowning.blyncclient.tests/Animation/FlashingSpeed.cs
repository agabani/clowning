using System;
using System.Linq;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Animation
{
    [TestFixture]
    public class FlashingSpeed
    {
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_set_flashing_speed()
        {
            Given_that_I_have_a_client();
            When_I_set_the_flashing_speed();
            Then_the_device_flashing_speed_should_be_set();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100000)]
        public void Should_not_be_able_to_set_flashing_speed_for_invalid_device(int deviceNumber)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_flashing_speed(deviceNumber, BlyncClient.Speed.High));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(100000)]
        public void Should_not_be_able_to_set_invalid_flashing_speed_for_device(BlyncClient.Speed speed)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_flashing_speed(0, speed));
        }

        private void Given_that_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_set_the_flashing_speed()
        {
            _successful = true;

            var speeds = Enum.GetValues(typeof (BlyncClient.Speed)).Cast<BlyncClient.Speed>();

            foreach (var speed in speeds)
            {
                for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
                {

                    _successful &= _blyncClient.SetFlashSpeed(device, speed);
                }
            }
        }

        private TestDelegate When_I_flashing_speed(int deviceNumber, BlyncClient.Speed speed)
        {
            return () => _blyncClient.SetFlashSpeed(deviceNumber, speed);
        }

        private void Then_the_device_flashing_speed_should_be_set()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}