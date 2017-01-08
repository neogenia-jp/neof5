FROM microsoft/dotnet:1.0.3-sdk-projectjson

MAINTAINER WATARU MAEDA <w.maeda@neogenia.co.jp>

ENV ASPNETCORE_ENVIRONMENT=Production \
    server.urls=http://*:80

EXPOSE 80

ADD src/Neof5WebSite/ /root/app
WORKDIR /root/app
RUN dotnet restore && dotnet build

CMD dotnet run
