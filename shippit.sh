rm -rf ./pub-linux
rm -rf ./pub-windows
rm -rf ./pub-mac
dotnet publish ./src/TitsUp/TitsUp.vbproj -o ./pub-linux -c Release --sc -r linux-x64
dotnet publish ./src/TitsUp/TitsUp.vbproj -o ./pub-windows -c Release --sc -r win-x64
dotnet publish ./src/TitsUp/TitsUp.vbproj -o ./pub-mac -c Release --sc -r osx-x64
butler push pub-windows thegrumpygamedev/tits-up:windows
butler push pub-linux thegrumpygamedev/tits-up:linux
butler push pub-mac thegrumpygamedev/tits-up:mac
git add -A
git commit -m "shipped it!"