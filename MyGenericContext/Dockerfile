
FROM microsoft/aspnetcore-build
# Create directory for the app source code
WORKDIR /app
# Copy the source and restore dependencies
COPY . /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 5000
ENTRYPOINT ["dotnet", "run"]