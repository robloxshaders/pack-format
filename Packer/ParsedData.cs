namespace Roblox {
    namespace Pack {
        namespace Parser {
            public enum GraphicsApi {
                DirectX, // compiled cannot be edited easily,
                OpenGL, // Super easy to edit
                Vulkan // idek
            }
            public class FileInformation {
                public char[] md5 {get; set;} = new char[32];
                public int offset {get; set;} = 0;
                public int offsetOffset {get; set;} = 0;

                public int sizeOffset {get; set;} = 0;
                public short size {get; set;} = 0;
                public string name {get; set;} = "";
                public byte[]? data {get; set;} = null;
            }
            public class PackData {
                public string packName  {get; set;} = "";
                public Dictionary<string, byte[]?> files {get; set;} = new();
                public List<FileInformation> fileInformation = new List<FileInformation>();
                // appearently roblox pack files cannot hold more than 255 files
                public short totalFiles  {get; set;} = 0;
                // offsetStrategy
                public short totalOffsets {get; set;} = 0;
                public int fileDataStart {get; set;} = 0;
                public GraphicsApi graphicsApi  {get; set;} // ;
                // those offsets are unknown and which purpose they serve also not known as in the time of writing this there are 3128 offsets
                public List<FileInformation> oddOffsets = new List<FileInformation>();
                public int whereOffsetsBegin {get; set;} = 0;
                public int whereWeirdDataStart {get; set;} = 0;

            
            }
        }
    }
}