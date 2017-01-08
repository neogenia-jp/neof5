FROM microsoft/dotnet:1.0.3-sdk-projectjson

MAINTAINER WATARU MAEDA <w.maeda@neogenia.co.jp>

EXPOSE 80

ADD src/Neof5WebSite/ /root/app
WORKDIR /root/app
RUN dotnet restore && dotnet build

ARG gmail_addr=neogenia.dev@gmail.com
ARG gmail_passwd

ENV ASPNETCORE_ENVIRONMENT=Production \
    server.urls=http://*:80 \
    GMAIL_ADDR=$gmail_addr \
    GMAIL_PASSWD=$gmail_passwd

CMD dotnet run
