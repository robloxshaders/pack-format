using Roblox.Pack.Parser;

namespace Roblox {
    namespace Pack {
        namespace Packer {
            // Overwrite packing until I figure it out properly
            public static class PackerTool {
                public static byte[] Pack(PackData packData, byte[] existing) {
                    PackData existingPackData = ParserTool.ParsePack(existing);
                    PackData bufferPack = new PackData();
                    List<int> sizeTemp = new List<int>();
                    for(int i = 0; i < existingPackData.files.Count; i++) {
                        bufferPack.files.Add(existingPackData.files.ElementAt(i).Key, packData.files[existingPackData.files.ElementAt(i).Key]);
                    }
                    List<byte> data = new List<byte>(existing);
                    if(!data.Take(4).ToArray().SequenceEqual(new byte[] {(byte)'R', (byte)'B', (byte)'X', (byte)'S'})) {
                        throw new Exception("File signature RBXS is not found");
                    }


                    packData.files.Clear();
                    for(int i = 0; i < existingPackData.files.Count; i++) {
                        
                        packData.files.Add(existingPackData.files.ElementAt(i).Key, bufferPack.files[existingPackData.files.ElementAt(i).Key]);
                    }

                    int shiftCounter = 0;
                    for(int i = 0; i < packData.files.Count; i++) {
                        string fileName = packData.files.ElementAt(i).Key;
                        byte[]? content = packData.files.ElementAt(i).Value;
                        int shiftCounterTemp = 0;
                        if(content != null) {

                           int oldOffset = existingPackData.fileInformation[i].offset;
                            int oldSize = existingPackData.fileInformation[i].size;
                            int offsetOffset = existingPackData.fileInformation[i].offsetOffset;
                            int sizeOffset = existingPackData.fileInformation[i].sizeOffset;
                            string name = existingPackData.fileInformation[i].name;

                            int stopOffset = 0;

                            if(content.Length < oldSize) {
                                Array.Resize(ref content, oldSize);
                                
                            }
                            sizeTemp.Add(content.Length);
                        

                            data[sizeOffset]   = (byte)content.Length;
                            data[sizeOffset + 1]   = (byte)(content.Length >> 8);     
                            
                            existingPackData.fileInformation[i].size =  (short)content.Length;
                        
                            
                        }
                    }

                    // offset strategy

                    int offset = existingPackData.fileDataStart;

                    data.RemoveRange(existingPackData.fileDataStart, data.Count - existingPackData.fileDataStart);
                    for(int i = 0; i < existingPackData.totalFiles; i++) {
                        int offsetOffset = existingPackData.fileInformation[i].offsetOffset;

                        data[offsetOffset]  = (byte)(offset);                            
                        data[offsetOffset + 1]  = (byte)((offset) >> 8);
                        data[offsetOffset + 2]  = (byte)((offset) >> 16);
                        data[offsetOffset + 3]  = (byte)((offset) >> 24); 
           
                        // data.RemoveRange(offset, existingPackData.fileInformation[i].size);
                        data.AddRange(packData.files.ElementAt(i).Value);
                        offset += packData.files.ElementAt(i).Value.Length;
                                                     
                    }
                    
                  

                    data.AddRange(existing[offset..existing.Length]);
                    
                    return data.ToArray();
                }
            }
        }
    }
}