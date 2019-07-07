FROM microsoft/dotnet:2.2-sdk as builder
WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /published Jobs/Jobs.csproj


FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /published .

ENTRYPOINT ["dotnet", "Jobs.dll"]
