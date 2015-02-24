using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Omnitwork
{
    class Slicer
    {
        public static object Slice(MemoryStream buffer, StateObject state)
        {
            object command = null;
            if (state.waiting_for_header)
            {
                if (buffer.Position >= 4)
                {
                    int buffer_offset = (int)buffer.Position;
                    byte[] tmp = new byte[4];
                    buffer.Position = 0;
                    buffer.Read(tmp, 0, 4);
                    state.expected_bytes = BitConverter.ToInt32(tmp, 0);
                    state.waiting_for_header = false;
                    buffer.Position = buffer_offset;
                }
            }
            if (!state.waiting_for_header)
            {
                if (buffer.Position >= state.expected_bytes + 4)
                {
                    // !!!
                    // ДЕСЕРИАЛИЗАТОР УМНЫЙ, ПЕРЕПИСАТЬ
                    int buffer_offset = (int)buffer.Position;
                    buffer.Position = 4;
                    //buffer.SetLength(state.expected_bytes+4);                    
                    /*
                    byte[] body = new byte[state.expected_bytes];
                    buffer.Position = 4;
                    buffer.Read(body, 0, body.Length);                    
                    int remainder_length = buffer_offset - (int)buffer.Position;
                    MemoryStream mem_stream = new MemoryStream(body.Length);
                    mem_stream.Write(body, 0, body.Length);
                    BinaryFormatter bf = new BinaryFormatter();
                    mem_stream.Position = 0;
                    */
                    BinaryFormatter bf = new BinaryFormatter();
                    command = bf.Deserialize(buffer);
                    int remainder_length = buffer_offset - state.expected_bytes - 4;
                    if (remainder_length > 0)
                    {
                        byte[] remainder = new byte[remainder_length];
                        buffer.Read(remainder, 0, remainder_length);
                        buffer.SetLength(65536);
                        buffer.Position = 0;
                        buffer.Write(remainder, 0, remainder.Length);

                        state.sb.Clear();
                    }
                    else
                    {
                        buffer.Position = 0;
                        buffer.SetLength(65536);
                    }
                    state.waiting_for_header = true;
                }
            }
            return command;
        }
    }
}
