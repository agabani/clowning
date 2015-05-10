using System;
using System.Threading;
using NUnit.Framework;

namespace clowning.blyncclient.tests.Color
{
    [TestFixture]
    public class Rgb
    {
        private const int MaxIntensity = 0xFF;
        private const int MinIntensity = 0;
        private const int IntensityStep = 0x80;
        private BlyncClient _blyncClient;
        private bool _successful;

        [Test]
        public void Should_be_able_to_cycle_through_all_colors()
        {
            Given_that_I_have_a_client();
            When_I_cycle_through_all_colors();
            Then_I_should_have_cycled_through_all_colors();
        }

        [Test]
        [TestCase(MinIntensity - 1, 0x80, 0x80)]
        [TestCase(MaxIntensity + 1, 0x80, 0x80)]
        [TestCase(0x80, MinIntensity - 1, 0x80)]
        [TestCase(0x80, MaxIntensity + 1, 0x80)]
        [TestCase(0x80, 0x80, MaxIntensity + 1)]
        [TestCase(0x80, 0x80, MaxIntensity + 1)]
        public void Should_not_be_able_to_display_invalid_colors(int red, int green, int blue)
        {
            Given_that_I_have_a_client();
            Then_I_should_get_an_arguement_out_of_range_exception(When_I_turn_on_color(red, green, blue));
        }

        private void Given_that_I_have_a_client()
        {
            _blyncClient = new BlyncClient();
        }

        private void When_I_cycle_through_all_colors()
        {
            _successful = true;

            for (var red = 0; red < MaxIntensity; red += IntensityStep)
            {
                for (var green = 0; green < MaxIntensity; green += IntensityStep)
                {
                    for (var blue = 0; blue < MaxIntensity; blue += IntensityStep)
                    {
                        for (var device = 0; device < _blyncClient.NumberOfDevices; device++)
                        {
                            _successful &= _blyncClient.SetColor(device, red, green, blue);
                        }
                    }
                }
            }
        }

        private TestDelegate When_I_turn_on_color(int red, int green, int blue)
        {
            return () => _blyncClient.SetColor(0, red, green, blue);
        }

        private void Then_I_should_have_cycled_through_all_colors()
        {
            Assert.That(_successful, Is.True);
        }

        private void Then_I_should_get_an_arguement_out_of_range_exception(TestDelegate action)
        {
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }
    }
}