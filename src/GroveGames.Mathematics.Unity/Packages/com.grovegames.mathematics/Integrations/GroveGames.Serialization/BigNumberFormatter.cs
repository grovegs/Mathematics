using GroveGames.Serialization;

namespace GroveGames.Mathematics.Unity
{
    [Formatter]
    public sealed class BigNumberFormatter : IFormatter<BigNumber>
    {
        private static readonly FieldTable s_fields = new("mantissa", "exponent");

        public void Write<TWriter>(ref TWriter writer, BigNumber value) where TWriter : struct, IFormatWriter
        {
            writer.BeginObject(2);
            writer.WriteField(s_fields[0]);
            writer.WriteDouble(value.Mantissa);
            writer.WriteField(s_fields[1]);
            writer.WriteInt64(value.Exponent);
            writer.EndObject();
        }

        public BigNumber Read<TReader>(ref TReader reader) where TReader : struct, IFormatReader
        {
            if (reader.Peek() == TokenType.Null)
            {
                reader.Skip();
                return BigNumber.Zero;
            }

            var mantissa = 0d;
            var exponent = 0L;
            reader.ReadObjectStart();

            while (reader.TryReadField(s_fields, out var index))
            {
                switch (index)
                {
                    case 0:
                        mantissa = reader.ReadDouble();
                        break;
                    case 1:
                        exponent = reader.ReadInt64();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new BigNumber(mantissa, exponent);
        }

        public void Transcode<TReader, TWriter>(ref TReader reader, ref TWriter writer)
            where TReader : struct, IFormatReader
            where TWriter : struct, IFormatWriter
        {
            Write(ref writer, Read(ref reader));
        }
    }
}
