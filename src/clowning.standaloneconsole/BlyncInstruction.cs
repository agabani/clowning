using System.Drawing;

namespace clowning.standaloneconsole
{
    internal class BlyncInstruction
    {
        public bool Flash { get; set; }
        public int FlashSpeed { get; set; }
        public bool Dim { get; set; }
        public Color Rgb { get; set; }
        public int Color { get; set; }
    }
}