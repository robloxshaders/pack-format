# pack-format
A parser/encoder for Roblox shader pack format

## Usage

**unpack: **
```sh
mkdir output
dotnet run -u C:\Users\iskender\AppData\Local\Roblox\Versions\version-d6abc3b106a04c5c\shaders\shaders_glsl3.pack ./output
```

**pack: **
```sh
dotnet run --p ./output C:\Users\iskender\AppData\Local\Roblox\Versions\version-d6abc3b106a04c5c\shaders\shaders_glsl3.pack
```

**reset: **
```sh
dotnet run --r C:\Users\iskender\AppData\Local\Roblox\Versions\version-d6abc3b106a04c5c\shaders\shaders_glsl3.pack
```

Copyright all rights reserved IP rights belong to roblox, this is not a clean room implementation and includes similar logic found in Roblox's own engine thus this isn't free and open source software know that if you use this for anything serious you might get sued to death just assume worst case scneario for everything law related anyways cya.
