docker build -t bashamer/slipka:debug .
docker ps -a -q --filter "ancestor=bashamer/slipka:debug" --format="{{.ID}}"  | ForEach-Object -Process {docker kill $_ } 
$IPV4=(Test-Connection -ComputerName $env:computername -count 1).ipv4address.IPAddressToString
docker run -p 4445:4445 -p 61710-61920:61710-61920 -e "SLIPKA_ConnectionString=mongodb://${IPV4}:27017/" bashamer/slipka:debug

# Get-Process -Id (Get-NetTCPConnection -LocalPort 61717).OwningProcess  
# docker run -p 4445:4445 -e "SLIPKA_ConnectionString=mongodb://${IPV4}:27017/" bashamer/slipka:debug