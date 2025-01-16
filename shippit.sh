rm -rf ./pub-linux
rm -rf ./pub-windows
rm -rf ./pub-mac
dotnet publish ./src/TitsUp/TitsUUp.vbproj -o ./pub-linux -c Release --sc -r linux-x64
dotnet publish ./src/TitsUp/TitsUUp.vbproj -o ./pub-windows -c Release --sc -r win-x64
dotnet publish ./src/TitsUp/TitsUUp.vbproj -o ./pub-mac -c Release --sc -r osx-x64
#butler push pub-windows thegrumpygamedev/tits-uup:windows
#butler push pub-linux thegrumpygamedev/tits-uup:linux
#butler push pub-mac thegrumpygamedev/tits-uup:mac
#git add -A
#git commit -m "shipped it!"