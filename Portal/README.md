# ネオ富豪ポータルサイト

## Docker環境

簡単な使い方

```
docker build -t neof5portal --build-arg gmail_addr='xxx@gmail.com' gmail_passwd='xxxxxxxx' .
docker run -d -p 5000:80 neof5portal
```

これでWebブラウザにて `http://localhost:5000/` にアクセスするとトップページが表示されます。

GMailアドレスおよびパスワードは、Dockerfileの引数です。メール送信機能を使用しない場合は引数を省略しても問題ありません。

```
docker build -t neof5portal .
```

### ASP.NET Core

バージョン: 1.0.3

### 説明

Dockerfileをビルドすると、ASP.NET Core 1.0 SDKが使用可能な環境が構築されます。
ソースツリーは、Docker内の `/root/app` に配置され、パッケージリストア、ビルドまで行われます。

Webアプリを起動せず、ソースツリーをマウントして手動でdotnetを起動する場合は以下のようにします。

```
cd Portal
docker run -p 5000:80 -v $(pwd)/src/Neof5WebSite:/mnt -ti <container_id> bash
# ここからはコンテナ内で実行する
cd /mnt
dotnet restore  # パッケージリストア
dotnet run      # 実行
```

### バインディングアドレス変更

バインディングアドレスは `hosting.json` に設定がありますが、アプリ内で環境変数で上書きしているため、反映されません。

変更するには2つの方法があります。
例えば `http://*:8080` にバインディングしたい場合、

1. 環境変数を設定する

  Dockerfileにて以下のように設定する
  ```
  ENV server.urls=http://*:8080
  ```
  または、dotnetコマンドを手動で実行している場合は環境変数を渡して実行する
  ```
  server.urls=http://*:8080 dotnet run
  ```

2. 環境変数をクリアし、hosting.json を変更する

  hosting.json を変更し、Dockerfileにて `server.urls` の設定をやめ、コンテナをビルドしなおす。
  
  ※注意
  `http://localhost:5000` のように localhost にバインディングした場合は、他のホストからアクセスできないため、Dockerコンテナとしての機能を果たさなくなります。

  ※参考リンク
  http://qiita.com/husky774rr/items/256609e9d126653274f0#_reference-2a50db2589ec9b141c22

