
namespace InteractiveTable.Core.Data.Capture
{
    /// <summary>
    /// Values of detected stone used for serialization
    /// </summary>
    public class RockDesc
    {
        public const byte ROCKDESC_TYPE_GRAVITON = 1;
        public const byte ROCKDESC_TYPE_MAGNETON = 2;
        public const byte ROCKDESC_TYPE_GENERATOR = 3;
        public const byte ROCKDESC_TYPE_BLACKHOLE = 4;

        public short positionX;
        public short positionY;
        public byte rockType;
        public double rate;
        public double angle;
        public double scale;

        /// <summary>
        /// Intensity of stone - used for smooth fade-in
        /// </summary>
        public byte intensity;

        public RockDesc()
        {

        }

        public RockDesc(FoundTemplateDesc template)
        {
            this.rate = template.rate;
            this.angle = template.angle;
            this.scale = template.scale;
        }
    }
}
