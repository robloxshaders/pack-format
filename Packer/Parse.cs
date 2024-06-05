using System.Text;
using System.Text.Json;

namespace Roblox {
    namespace Pack {
        namespace Parser {
            public static class ParserTool {
                public static Roblox.Pack.Parser.PackData ParsePack(byte[] data) {
                    PackData packData = new PackData();
                    if(data.LongLength > 4) {
                        string? header = System.Text.Encoding.ASCII.GetString(data.Take(4).ToArray());
                        if(header == null) {
                            throw new Exception("header variable ended up being null");
                        }
                        if(header != "RBXS") {
                            throw new Exception("RBXS Header was not present in the given input file");
                        }

                    }

                    short totalData = (short)(data?[9] << 8 | (data?[8]) ?? throw new Exception("total entities number located at offset 9 is found out to be non existent"));
                    short totalOffsets = (short)(data?[11] << 8 | (data?[10]) ?? throw new Exception("total offsets were not found"));
                    
                    packData.totalFiles = totalData;

                    bool hasCompletedEntry = false;
                    bool hasStartedOffsets = false;
                    StringBuilder fileNameTemp = new StringBuilder();
   
                    void AddFileName(string fileName) {
                        packData.files.Add(fileName, null);
                    }

                    List<byte> tempBytes = new List<byte>();
                    short entityCounter = -1;
                    short entryCounter = 0;
                    int vsh = 0;

                    // inefficient exist just for prototyping refactor it in future
                    for (long i = 16; i < data?.LongLength; i++) {
                        if(hasStartedOffsets) {
                            packData.whereOffsetsBegin = (int)i;
                            // opengl only support for the parser, requiring the version string to detect where to stop
                            if(data?[i] == '#' && data?[i + 1] == 'v' && data?[i + 2] == 'e' ) {
                                packData.fileDataStart = (int)i;
                                break;
                            }


                            
                            tempBytes.Add(data[i]);
                                                        
                            if(tempBytes.Count == 64) {

                                // let's see what's in it
                                // tempBytes.Skip(10);
                                byte[] offsetData = tempBytes.Skip(15).Take(4).ToArray();
                                int offset = (offsetData[3] << 24 | offsetData[2] << 16 | offsetData[1] << 8 | offsetData[0]);
                                // if(entityCounter == 0)
                                // Console.WriteLine(offset);
                                byte[] sizeOffsetData= tempBytes.Skip(19).Take(2).ToArray();
                                
                                short size = (short)((sizeOffsetData[1]) << 8 | (byte)(sizeOffsetData[0]));
                                // if(packData.files.ElementAt(entityCounter + 1).Key == "defaultFixFog")
                                // Console.WriteLine(i - 64 + 19);
                                // Console.WriteLine( offset + " " +  vsh + " / " + size);

                                if(entityCounter + 1 < packData.files.Count) {
                                    packData.fileInformation.Add(new FileInformation() {
                                        offset = offset,
                                        size = size,
                                        offsetOffset = (int)(i - 64 + 16),
                                        sizeOffset = (int)(i - 64 + 20),
                                        name = packData.files.ElementAt(vsh).Key
                                    });     
                                    packData.files[packData.files.ElementAt(++entityCounter).Key] = data.Skip(offset).Take(size).ToArray();
                                                                   
                                }
                                else {
                                    if(entityCounter == packData.files.Count) packData.whereWeirdDataStart = (int)i;
                                    // höristik kullanarak isim tahmini dosya içeriğinden vesika diom 31 cek
                                    string heuristicNaming = "";
                                    /*string content = System.Text.UTF8Encoding.UTF8.GetString(data[offset..(offset + size)]);
                                    if(content.Contains("Camera")) {
                                        heuristicNaming = "camera_related";
                                    }else if(content.Contains("plastic")) {
                                        heuristicNaming = "material_related_glass";
                                    }
                                    else if(content.Contains("glass")) {
                                        heuristicNaming = "material_related_glass";
                                    }
                                    else if(content.Contains("noise")) {
                                        heuristicNaming = "noise";
                                    }*/

                                    packData.oddOffsets.Add(new FileInformation() {
                                        offset = offset,
                                        size = size,
                                        offsetOffset = (int)(i - 64 + 16),                                        
                                        sizeOffset = (int)(i - 64 + 20) 
                                    });
                                }
                                
                                tempBytes.Clear();
                            }


                            
                        }
                        if(hasCompletedEntry) {
                            
                            // we completed the entry, now determine where are the offsets will be located
                            // warning: the 0x11 is by coincidance you have to find the byte manually for every update!
                            if(data[i] == 0x11 && !hasStartedOffsets) {
                                // this is where offset data starts
                                // not sure about what those are bytes are, md5?
                                hasStartedOffsets = true;
                            }

                        }


                        if(!hasCompletedEntry) {
                            tempBytes.Add(data[i]);
                            int sizing = entryCounter < 4 ? 64 : 68;
                            if(tempBytes.Count >= sizing) {
                                string fileName = System.Text.Encoding.ASCII.GetString(tempBytes.ToArray()).Split('\0').Where(fname => !string.IsNullOrEmpty(fname)).OrderByDescending(fname => fname.Length).ToArray()[0];
                                // string bruh = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(fileName.ToArray().Skip(entryCounter < 4 ? 0 : 4).ToArray()));

                                if(fileName.Length > 1)
                                AddFileName(fileName);                               
                                tempBytes.Clear();
                                // Console.WriteLine(bruh);
                                entryCounter++;
                            }

                            if(packData.files.Count >= packData.totalFiles) {
                                hasCompletedEntry = true;
                                tempBytes.Clear();
                            }
                        }

                    }

                    return packData;
                }

            }
        }
    }
}