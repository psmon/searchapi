FROM microsoft/dotnet:2.2-sdk
WORKDIR /app
EXPOSE 5000

RUN mkdir -p /app/SearchApi && cp /usr/share/zoneinfo/Asia/Seoul /etc/localtime

WORKDIR /app/SearchApi
COPY . .
RUN dotnet tool install --global dotnet-typegen --version 2.4.8
RUN dotnet tool list -g
RUN dotnet restore
RUN /root/.dotnet/tools/dotnet-typegen generate
RUN dotnet publish -c Release -o out

RUN ls -l /app
RUN ls -l /app/SearchApi
RUN ls -l /app/SearchApi/out
RUN find /app -name *.dll*

ENTRYPOINT ["dotnet", "out/SearchApi.dll"]