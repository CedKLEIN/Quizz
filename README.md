dotnet publish -c Release -r win-x64 --self-contained true

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=false -p:UseAppHost=true

PublishReadyToRun => genere code optimise qu ipeut etre responsable de problem sur l ordi de coco