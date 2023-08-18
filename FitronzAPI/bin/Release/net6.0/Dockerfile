#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.



FROM mcr.microsoft.com/dotnet/sdk:6.0
ADD bin/Release/net6.0/ .
CMD ["dotnet", "FitronzAPI.dll"]

EXPOSE 80
EXPOSE 8080
EXPOSE 5175
#EXPOSE 443
ENV PORT 8080
ENV HOST 0.0.0.0