using System;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Color
{
    [TestFixture]
    public class Predefined
    {
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_cycle_through_all_colors()
        {
            Given_I_have_a_client();
            When_I_cycle_through_all_the_predefined_colors();
            Then_I_should_have_cycled_through_all_the_predefined_colors();
        }

        [Test]
        [TestCase(BlyncClient.Color.Blue)]
        [TestCase(BlyncClient.Color.Cyan)]
        [TestCase(BlyncClient.Color.Green)]
        [TestCase(BlyncClient.Color.Magenta)]
        [TestCase(BlyncClient.Color.Orange)]
        [TestCase(BlyncClient.Color.Red)]
        [TestCase(BlyncClient.Color.White)]
        [TestCase(BlyncClient.Color.Yellow)]
        public void Should_be_able_to_turn_on_predefined_color(BlyncClient.Color color)
        {
            Given_I_have_a_client();
            When_I_turn_on_predefined_color(color);
            Then_I_should_have_successfully_turned_on_predefined_color();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(1000000)]
        public void Should_fail_if_invalid_device_number_is_used(int device)
        {
            Given_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_Turn_On_Device(device));
        }

        private void Given_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_cycle_through_all_the_predefined_colors()
        {
            _successful = true;
            var values = Enum.GetValues(typeof (BlyncClient.Color));
            foreach (var value in values)
            {
                for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
                {
                    TurnOnColor(device, value);
                }
            }
        }

        private void When_I_turn_on_predefined_color(BlyncClient.Color color)
        {
            _successful = true;
            for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
            {
                _successful &= _blyncClient.TurnOnLight(device, color);
            }
        }

        private TestDelegate When_I_Turn_On_Device(int device)
        {
            return () => TurnOnColor(device, BlyncClient.Color.Blue);
        }

        private void Then_I_should_have_cycled_through_all_the_predefined_colors()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_have_successfully_turned_on_predefined_color()
        {
            Assert.That(_successful, Is.EqualTo(true));
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }

        private void TurnOnColor(int device, object value)
        {
            _successful &= _blyncClient.TurnOnLight(device, (BlyncClient.Color) value);
        }
    }
}