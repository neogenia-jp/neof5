/**
 * main.js
 * Web大富豪クライアントログラム
 */
enchant();

window.onload = function () {
    // ============================== Variables ==============================
    var gRoomid = document.getElementById('roomid').value;
    var gPlayername = document.getElementById('name').value;
    var gRule = document.getElementById('rule').value;
    var gAutostart = document.getElementById('autostart').value;
    var gMonitor = document.getElementById('monitor').value;

    // ============================== Variables ==============================

    var GAME_W = 800;
    var GAME_H = 440;
    var GAME_FPS = 8;

    var DECK_CARD_W = 36;
    //var DECK_CARD_H = 100;
    var DECK_W = 240;
    var DECK_H = 90;
    var MYDECK_W = 400;
    var MYDECK_H = 108;

    var INFOBOX_W = 64;
    var INFOBOX_H = 64;

    var TWEETBOX_W = 380;
    var TWEETBOX_H = 35;

    // プレイヤー情報ボックス
    var INFOBOX_BACKGROUND_COLOR     = "#cae7f2";  // 塗りつぶしの色 
    var INFOBOX_BORDER_COLOR         = "#5b686d";  // 枠線の色
    var INFOBOX_BACKGROUND_HICOLOR   = "#fbe481";  // 塗りつぶしの色（ハイライト）
    var INFOBOX_BORDER_HICOLOR       = "#e06a3b";  // 枠線の色（ハイライト）

	// ツイート
    var TWEET_BACKGROUND_COLOR       = "#f7f7f7";  // 背景色

    // 山
    var YAMA_BORDER_COLOR            = "#5b686d";  // 塗りつぶしの色
    var YAMA_BORDER_HICOLOR          = "#e06a3b";

    var RANKING_TITLE = { "n-2": "大貧民", "n-1": "貧民", "n0": "平民", "n1": "富豪", "n2": "大富豪" };

    var socket;  // WebSocket

    function rand(max) { return Math.random() * max; }

    var game_ = new Game(GAME_W, GAME_H); // 表示領域の大きさを設定

    game_.fps = GAME_FPS;                 // ゲームの進行スピードを設定
    game_.preload('/Content/imgs/trump2.png');
    game_.onload = function() { // ゲームの準備が整ったらメインの処理を実行します

        // ============================== Extentions  ==============================

        function drawBoxLine(surface, color) {
            var canvas = surface.context;
            //canvas.beginPath();
            canvas.strokeStyle = color || '#a0a0a0';
            canvas.lineWidth = 4;
            canvas.strokeRect(0, 0, surface.width, surface.height);
            /*
            canvas.moveTo(0, 0);
            canvas.lineTo(0, surface.height);
            canvas.stroke();
            canvas.moveTo(0, surface.height);
            canvas.lineTo(surface.width, surface.height);
            canvas.stroke();
            canvas.moveTo(surface.width, 0);
            canvas.lineTo(surface.width, surface.height);
            canvas.stroke();
            canvas.moveTo(0, 0);
            canvas.lineTo(surface.width, 0);
            canvas.stroke();
            */
        }

        // Groupを囲む罫線を引くメソッド
        Group.prototype.drawBoxLine = function (color) {
            if (!this.scene) return;
            if (!this._surface) {
                this._boxSprite = new Sprite(this.width, this.height);
                //this.scene.addChild(this._boxSprite);
                this.scene.insertBefore(this._boxSprite, this);
                this._boxSprite.x = this.x;
                this._boxSprite.y = this.y;
                this._surface = new Surface(this.width, this.height);
                this._boxSprite.image = this._surface;
            }
            drawBoxLine(this._surface, color);
        };

        // ============================== Classes ==============================

        /**
         * カードスプライト
         */
        Card = Class.create(Sprite, {
            initialize: function (cardStr, scale) {
                Sprite.call(this, /*130, 193*/ 588 / 7, 960 / 8);
                this.image = game_.assets['/Content/imgs/trump2.png'];
                this.cardStr = cardStr;
                this.setCardStr(cardStr);
                if (scale!=undefined) this.scale(scale, scale);
            },

            // カードの設定（文字列で指定）
            setCardStr: function (cardStr) {
                this.cardStr = cardStr;
                var f = 0
                if (cardStr == '' || cardStr == '--') {
                    f = 13;
                } else if (cardStr == "JK") {
                    f = 7 * 8 - 1;
                } else {
                    switch (cardStr.substring(0, 1)) {
                        case "S": f = 0; break;
                        case "H": f = 2 * 7; break;
                        case "C": f = 4 * 7; break;
                        case "D": f = 6 * 7; break;
                    }

                    var n = cardStr.substring(1, 2);
                    switch (n) {
                        case "A": break;
                        case "0": f += 10 - 1; break;
                        case "J": f += 11 - 1; break;
                        case "Q": f += 12 - 1; break;
                        case "K": f += 13 - 1; break;
                        default: f += n - 1;
                    }
                }
                this.frame = f;
                return f;
            }
        });

        /**
         * デッキ（手札）
         */
        Deck = Class.create(Group, {
            initialize: function (cardStr, scale) {
                Group.call(this);
                this.cardStr = cardStr;
                this.cardArr = Array();
                this.cardSprs = new Group();  // カードスプライトを入れるためのグループ
                this.addChild(this.cardSprs);
                this.isBackside = true;
                this.height = DECK_H;
                this.width = DECK_W;
                this.selectedCardArr = Array();
                this.touchEnable = false;
                this.tebanMark = null;
                this.isTeban = false;
                this.cardScale = scale == undefined ? 1.0 : scale;
            },

            // 手番マークを表示・非表示
            setTeban: function(yesno) {
                if (this.isTeban == yesno) return;
                this.isTeban = yesno;
                if (yesno && !this.tebanMark){
                    this.tebanMark = new Sprite(this.width, 3);
                    this.tebanMark.image = new Surface(this.tebanMark.width, this.tebanMark.height);
                    this.tebanMark.image.context.fillStyle = "red";
                    this.tebanMark.image.context.fillRect(0, 0, this.tebanMark.width, this.tebanMark.height);
                    this.addChild(this.tebanMark);
                    console.debug("addTebanMark width=", this.width);
                } 
                this.tebanMark.tl.moveTo(0, yesno ? 50 : 99999, 3);
            },

            // 表向き表示にする
            replaceCards: function (deckstr) {
                this.cardArr = deckstr=='' ? [] : deckstr.split(' ');
                this.isBackside = false;
                this.updateSprite();
            },

            // 裏返しのカードで枚数指定
            toBacksideAll: function (num) {
            	if (this.cardArr.length == num && this.isBackside) return;  // カード枚数が同じですでに裏返しなら何もすることはない
                this.cardArr.length = num;
                for (var i = 0; i < num; i++) this.cardArr[i] = '--';
                this.isBackside = true;
                this.updateSprite();
            },

            // スプライトに反映
            updateSprite: function () {
                var w = 0;
                var card_w = Math.min(DECK_CARD_W, this.width / this.cardArr.length);
                // スプライトをセット
                for (var i = 0; i < this.cardArr.length; i++) {
                    var c = this.isBackside ? '--' : this.cardArr[i];
                    if (!this.cardSprs.childNodes[i]) {
                        var c = new Card(c, this.cardScale);
                        //c.scaleX = this.cardScale;
                        //c.scaleY = this.cardScale;
                        var _this = this;
                        this.cardSprs.addChild(c);
                        c.addEventListener(Event.TOUCH_END, function () { _this.onTouchEnd(this); });
                    } else {
                        this.cardSprs.childNodes[i].setCardStr(c);
                    }
                    var s = this.cardSprs.childNodes[i];
                    s.tl.moveTo(card_w * i, 0, 3, enchant.Easing.CIRC_EASEIN);
                    w = card_w * i + s.width;
                }
                // 不要なスプライトを削除する
                while (this.cardArr.length < this.cardSprs.childNodes.length) {
                    this.cardSprs.removeChild(this.cardSprs.lastChild);
                }
                //this.width = w;
            },

        	// カードを含む最小の幅に設定する
            setToMinWidth: function () {
                var card_w = Math.min(DECK_CARD_W, this.width / this.cardArr.length);
                this.width = card_w * this.cardArr.length;
            },

            // カードをタップしたとき
            onTouchEnd: function (card) {
                if (!this.touchEnable || this.isBackside) return;  // 裏向きならなにもしない
                // カードの選択状態を反映
                if (card.y != 0) {
                    card.tl.moveBy(0, 10, 3);
                } else {
                    card.tl.moveBy(0, -10, 3);
                }
            },

            // 選択されているカードを取得
            getSelectedCards: function () {
                var ret = Array();
                for (var i = 0; i < this.cardSprs.childNodes.length; i++) {
                    if (this.cardSprs.childNodes[i].y != 0) ret.push(this.cardArr[i]);
                }
                return ret;
            },

        	// 拡大縮小
            setCardScale: function (b) {
            	if (b!=undefined) this.cardScale = b;
                for (var i = 0; i < this.cardSprs.childNodes.length; i++) {
                	this.cardSprs.childNodes[i].scaleX = this.cardScale;
                	this.cardSprs.childNodes[i].scaleY = this.cardScale;
                }
            }
        });

        /**
         * 山を表すクラス
         */
        Yama = Class.create(Group, {
            initialize: function (width, height) {
                Group.call(this);
                this.width = width;
                this.height = height;
                this.yama_cards = new Group();
                this.addChild(this.yama_cards);
                this.ba_cards = new Group();
                this.addChild(this.ba_cards);
            },

            setYamaCards: function(yama) {
            	if (this.yama_cards.childNodes.length != 0) return;  // カードがすでに設定されていれば処理省略。

                for (var i=0; i<yama.length; i++) {
                    if (!this.yama_cards.childNodes[i]) {
                    	var node = new Deck('', 0.7);
                        this.yama_cards.addChild(node);
                    }
                    var node = this.yama_cards.childNodes[i];
                    node.replaceCards(yama[i]);
                    node.setToMinWidth();
                    if (node.x <= 0) {
                        // 山になかったカードは、山に置く。ランダムに位置の揺らぎをつける
                    	var xx = this.width * 4 / 5 + rand(28) - this.x - this.width / 2 + 40;
                        var yy = this.height / 2 + 50 + rand(28) - this.y;
                        node.tl.tween({ x: xx, y: yy, rotation: rand(360), time: 1, easing: enchant.Easing.QUAD_EASEOUT});
                        //node.moveTo(xx, yy);
                    }
                }
                // 余分なスプライトは消す
                while (yama.length < this.yama_cards.childNodes.length) {
                    this.yama_cards.removeChild(this.yama_cards.childNodes[this.yama_cards.childNodes.length-1]);
                }
            },

            setBaCardsWithAnim: function (ba, deck, withAnim) { this.setBaCards(ba, deck, true); },

            setBaCards: function (ba, deck, withAnim) {
                // 子のスプライトにカード文字列を設定。
                for (var i=0; i<ba.length; i++) {
                    if (!this.ba_cards.childNodes[i]) {
                    	var node = new Deck('', 0.7);
                        node.y = -999;
                        this.ba_cards.addChild(node);
                    }
                    var node = this.ba_cards.childNodes[i];
                    node.replaceCards(ba[i]);
                    node.setToMinWidth();
                    if (node.x <= 0) {
                        // 場になかったカードは、場に置く。ランダムに位置の揺らぎをつける
                        var xx = this.width * 4 / 5 + rand(28) - this.x;
                        var yy = this.height / 2 + 50 + rand(28) - this.y;
                        if (withAnim && i==ba.length-1 && deck) {
                            var x0 = deck.x + deck.width / 2 - this.x;
                            var y0 = deck.y + deck.height / 2 - this.y;
                            node.x = x0; node.y = y0;
                            node.tl.tween({ x: xx, y: yy, rotation: rand(360), time: 6, easing: enchant.Easing.QUAD_EASEOUT});
                        } else {
                            node.tl.tween({ x: xx, y: yy, rotation: rand(360), time: 1, easing: enchant.Easing.QUAD_EASEOUT});
                        }
                    }
                }
                // 余分なスプライトは消す
                while (ba.length < this.ba_cards.childNodes.length) {
                    this.ba_cards.removeChild(this.ba_cards.childNodes[this.ba_cards.childNodes.length-1]);
                }
            },

            // 流れた時の処理
            nagare: function () {
                while (0<this.ba_cards.childNodes.length) {
                    var spl = this.ba_cards.childNodes[0];
                    this.ba_cards.removeChild(spl);
                    this.yama_cards.addChild(spl);
                    spl.tl.moveBy(-this.width / 2 + 40, 0, 8, enchant.Easing.BACK_EASEOUT);
                }
            },

            clear: function () {
                while (0<this.yama_cards.childNodes.length) {
                    this.yama_cards.removeChild(this.yama_cards.childNodes[0]);
                }
                while (0<this.ba_cards.childNodes.length) {
                    this.ba_cards.removeChild(this.ba_cards.childNodes[0]);
                }
            }
        });
  
        /**
         * プレイヤー
         */
        DaifugoPlayer = Class.create(Group, {
            initialize: function (id, info) {
                Group.call(this);
                this.id = id;
                this.info = info;
                this.deck = new Deck('');
                this.addChild(this.deck);
            }
        });

        /**
         * プレイヤー情報スプライトグループおよび情報管理クラス
         */
        PlayerInfoBox = Class.create(Group, {
            initialize: function (width, height) {
                Group.call(this);
                this.width = width;
                this.height = height;

                this._info = null;
                this._boxSprite = new Sprite(this.width, this.height);
                this.addChild(this._boxSprite);
                this._surface = new Surface(this.width, this.height);
                this._boxSprite.image = this._surface;

                {
                    var tmp = this.name = new Label();
                    tmp.font = '10pt sans-serif';
                    tmp.x = 5; tmp.y = 5;
                    this.addChild(tmp);
                }
                {
                    var tmp = this.cardCount = new Label();
                    tmp.font = '9pt sans-serif';
                    tmp.x = 5; tmp.y = 25;
                    this.addChild(tmp);
                }
                {
                    var tmp = this.rank = new Label();  
                    tmp.font = '9pt sans-serif';
                    tmp.x = 5; tmp.y = 45;
                    this.addChild(tmp);
                }
                {
                    var tmp = this.order = new Label();
                    tmp.font = '10pt sans-serif';
                    tmp.x = 38; tmp.y = 45;
                    this.addChild(tmp);
                }
            },

            setInfo: function (info, isTeban) {
                this._info = info;
                this.name.text = info.Name;
                this.cardCount.text = "残り " + info.HavingCardCount + "枚";
                this.rank.text = RANKING_TITLE['n'+info.Ranking];
                this.order.text = info.OrderOfFinish ? info.OrderOfFinish + "位" : '';
                this._boxSprite.backgroundColor = isTeban ? INFOBOX_BACKGROUND_HICOLOR : INFOBOX_BACKGROUND_COLOR;
                drawBoxLine(this._surface, isTeban ? INFOBOX_BORDER_HICOLOR : INFOBOX_BORDER_COLOR);
            }

        });

        /**
         * Tweet表示クラス
         */
        TweetBox = Class.create(Group, {
            initialize: function (width, height) {
                Group.call(this);
                this.width = width;
                this.height = height;

                this._boxSprite = new Sprite(this.width, this.height);
                this._surface = new Surface(this.width, this.height);
                this._boxSprite.image = this._surface;
                this._boxSprite.backgroundColor = TWEET_BACKGROUND_COLOR;
                drawBoxLine(this._surface, INFOBOX_BORDER_COLOR);

                this.addChild(this._boxSprite);
                this._boxSprite.visible = false;

                {
                    var tmp = this.tweet = new Label();
                    tmp.font = '16pt sans-serif';
                    tmp.x = 8; tmp.y = 6;
                    this.addChild(tmp);
                }
            },

            showMessage: function (message) {
                this.tweet.text = message;
                this._boxSprite.visible = true;
                var _this = this;
                setTimeout(function () {
                	_this.tweet.text = "";
					_this._boxSprite.visible = false;
                }, 5000);
            }

        });

        /**
         * プレイヤー管理クラス
         */
        PlayerManager = Class.create(Group, {
            initialize: function (width, height) {
                Group.call(this);
                this.width = width;
                this.height = height;
                this.players = Array();
                this.playerInfoBoxes = Array();
                this.tweetBoxes = Array();
                this.mainPlayerNum = 0;
            },

            // プレイヤーの追加登録
            addPlayers: function (playerInfoArr, mainPlayerNum) {
            	this.mainPlayerNum = mainPlayerNum;
                for (var i = 0; i < playerInfoArr.length; i++) {
                    var player = new DaifugoPlayer(i, playerInfoArr[i]);
                    this.players[i] = player;
                    var d = player.deck;
                    if (i == this.mainPlayerNum) { d.setCardScale(0.8); }
                    else { d.setCardScale(0.5); }
                    this.addChild(player);

                    var box = new PlayerInfoBox(INFOBOX_W, INFOBOX_H);
                    this.playerInfoBoxes[i] = box;
                    this.addChild(box);

                    box = new TweetBox(TWEETBOX_W, TWEETBOX_H);
                    this.tweetBoxes[i] = box;
                    this.addChild(box);
                }
            },

            // プレイヤー情報の設定（上書き）
            setPlayerInfo: function (playerInfoArr, deckObj, teban) {
                for (var i = 0; i < playerInfoArr.length; i++) {
                    var player = this.players[i];
                    if (!player) return;
                    player.info = playerInfoArr[i];
                    if (Array.isArray(deckObj)) {
                    	player.deck.replaceCards(deckObj[i]);
                    } else {
                    	if (i == this.mainPlayerNum && deckObj) {
                    		player.deck.replaceCards(deckObj);
                    	} else {
                    		player.deck.toBacksideAll(playerInfoArr[i].HavingCardCount);
                    	}
                    }
                    this.playerInfoBoxes[i].setInfo(playerInfoArr[i], teban==i);
                }
            },

            clear: function() {
                //this.players.length = 0;
            },

            findPlayerById: function (id) {
                return this.players[id];
            },

            // オブジェクトの配置を行う
            doLayout: function () {
				// メインプレイヤーの手札のサイズ
                var myd = this.players[this.mainPlayerNum].deck;
                myd.width = MYDECK_W;
				myd.height = MYDECK_H;
                // 手札の座標（センターのXY座標）と回転角
                var xyr = [
                    GAME_W/2, GAME_H - MYDECK_H/2, 0,
                    DECK_H/2, DECK_H + INFOBOX_H + DECK_W/2, 90,
                    DECK_W/2 + 30, DECK_H / 2, 180,
                    GAME_W - DECK_H - INFOBOX_W - DECK_W/2, DECK_H / 2, 180,
                    GAME_W - DECK_H/2, GAME_H - DECK_H - INFOBOX_H - DECK_W /2, 270];
                for (var i = 0; i < this.players.length; i++) {
                    var d = this.players[(i + this.mainPlayerNum) % (this.players.length)].deck;
                    if (i < 5) {
                        d.x = xyr[i * 3] - d.width/2;
                        d.y = xyr[i * 3 + 1] - d.height/2;
                        d.rotation = xyr[i * 3 + 2];
                    } else {
                        // 5人目以降は表示できない
                    	d.visible = false;
                    }
                }

                // 情報ボックスの座標（左上のXY座標）
                xyr = [
                    DECK_H, GAME_H - INFOBOX_H, 0,
                    0, DECK_H + 18, 0,
                    DECK_W + 30, 0, 0,
                    GAME_W - DECK_H - INFOBOX_W, 0, 0,
                    GAME_W - INFOBOX_W, GAME_H - DECK_H - INFOBOX_H - 18, 0
                ];
                for (var i = 0; i < this.players.length; i++) {
                    var d = this.playerInfoBoxes[(i + this.mainPlayerNum) % (this.players.length)];
                    if (i < 5) {
                        d.x = xyr[i * 3];
                        d.y = xyr[i * 3 + 1];
                        d.rotation = xyr[i * 3 + 2];
                    } else {
                        // 5人目以降は表示できない
                    	d.visible = false;
                    }
                }

				// ツイートの表示位置（中央のXY座標）
                xyr = [
                    DECK_H + TWEETBOX_W/2, GAME_H - INFOBOX_H - 80 + TWEETBOX_H/2, 0,
                    INFOBOX_W + TWEETBOX_H/2, DECK_H + 18 + TWEETBOX_W/2, 90,
                    0 + TWEETBOX_W/2, INFOBOX_H + TWEETBOX_H/2, 0,
                    GAME_W / 2 + TWEETBOX_W/2, INFOBOX_H + TWEETBOX_H/2, 0,
                    GAME_W - INFOBOX_W - TWEETBOX_H/2, GAME_H - DECK_H - 18 - TWEETBOX_W/2, -90
                ];
                for (var i = 0; i < this.players.length; i++) {
                	var d = this.tweetBoxes[(i + this.mainPlayerNum) % (this.players.length)];
                	d.x = xyr[i * 3] - d.width/2;
                	d.y = xyr[i * 3 + 1] - d.height/2;
                	d.rotation = xyr[i * 3 + 2];
                }
            },

        	// ツイートの表示
            showTweet: function (playerNum, message) {
                var d = this.tweetBoxes[playerNum];
                d.showMessage(this.playerInfoBoxes[playerNum].name.text + "：「" + message + "」");
            }
        });


        // ============================== Scenes ==============================

        /**
         * タイトルシーン
         */
        var titleScene = function() {
            var scene = new Scene();                                // 新しいシーンを作る
            scene.backgroundColor = '#e6855e';                      // シーンの背景色を設定
            // スタート画像設定
            var startImage = new Card('HQ');                   // スプライトを作る
            startImage.x = 42;                                      // 横位置調整
            startImage.y = 136;                                     // 縦位置調整
            scene.addChild(startImage);                             // シーンに追加
            // タイトルラベル設定
            var title = new Label('ネオ大富豪');                     // ラベルを作る
            //title.textAlign = 'center';                             // 文字を中央寄せ
            title.color = '#ffffff';                                // 文字を白色に
            title.x = 120;                                            // 横位置調整
            title.y = 60;                                           // 縦位置調整
            title.font = '32 sans-serif';
            scene.addChild(title);                                  // シーンに追加

            // 接続フォーム
            var lb1 = new Label('ニックネーム:');                     // ラベルを作る
            lb1.color = '#ffffff';                                // 文字を白色に
            lb1.x = 120;                                            // 横位置調整
            lb1.y = 110;                                           // 縦位置調整
            lb1.font = '14 sans-serif';                         // 28pxのゴシック体にする
            scene.addChild(lb1);                                  // シーンに追加
            // 入力ボックス
            var inputTextBox = new InputTextBox(); // テキストボックスを作成
            inputTextBox.width = 200;
            inputTextBox.x = 120;                                            // 横位置調整
            inputTextBox.y = 130;                                       // 縦位置調整
            inputTextBox.font = 'sans-serif';                      // 14pxのゴシック体にする
            inputTextBox.size = 12;
            scene.addChild(inputTextBox);                               // シーンに追加
            // ボタン
            var button = new Button("接続", "light");
            button.x = 330;
            button.y = 130;
            button.font = 'sans-serif';                      // 14pxのゴシック体にする
            button.size = 12;
            button.ontouchend = function () {
                myname = inputTextBox.value;
                if (myname == '') {
                    alert('ニックネームを入力してください。');
                    return;
                }
                game_.replaceScene(mainScene());    // 現在表示しているシーンをゲームシーンに置き換える
            };
            scene.addChild(button);

            return scene;
        };

        /**
         * メインシーン
         */
        var mainScene = function() {
            var scene = new Scene();                            // 新しいシーンを作る
            scene.backgroundColor = '#a6e39d';

            // 山
            var yama = new Yama();
            yama.width = GAME_W - (DECK_H + 20)*2;
            yama.height = GAME_H - (DECK_H + 20)*2;
            yama.x = DECK_H + 20;
            yama.y = DECK_H + 20;
            scene.addChild(yama);

            // 情報表示ラベル
            var gameInfo = new Label('ネオ大富豪');
            scene.addChild(gameInfo);
            gameInfo.width = 100;
            gameInfo.x = GAME_W - 100;
            gameInfo.y = GAME_H - 20;
            gameInfo.textAlign = 'right';
            gameInfo.font = '9pt sans-serif';

            // プレイヤー管理クラス
            var pm = new PlayerManager(GAME_W, GAME_H);
            scene.addChild(pm);

            // ＯＫボタン
            var okbutton = new Button("ＯＫ", "light");
            //okbutton.font = '12pt sans-serif';
            okbutton.x = -10000;
            scene.addChild(okbutton);

            // 開始ボタン
            var startButton = new Button("開始", "light");
            startButton.x = GAME_W / 2;
            startButton.y = GAME_H / 2;
            //startButton.font = '28pt sans-serif';
            startButton.ontouchend = function () { SendStart(); }
            if (!gMonitor) scene.addChild(startButton);
           
			// 開始処理
            function SendStart() {
                var obj = new Object();
                obj.Kind = 'Start';

                var jsonString = JSON.stringify(obj);
                socket.send(jsonString);
                console.debug('send: ' + jsonString);
            }

            // 手番を表すマーク
            var tebanMark = new Sprite(INFOBOX_W, INFOBOX_H);
            var tebanMarkSF = new Surface(tebanMark.width, tebanMark.height);
            drawBoxLine(tebanMarkSF, INFOBOX_BORDER_HICOLOR);
            tebanMark.image = tebanMarkSF;
            tebanMark.x = -100000;
            scene.addChild(tebanMark);

            // 手番マークを移動させる関数
            function setTebanFunc(pm, teban) {
                tebanMark.tl.tween({
                    x: pm.playerInfoBoxes[teban].x,
                    y: pm.playerInfoBoxes[teban].y,
                    time: 3,
                    easing: enchant.Easing.QUINT_EASEOUT
                });
            }

            // トーストメッセージ 
            var toast = new Label();
            toast.width = GAME_W;
            toast.y = GAME_H / 2;
            toast.textAlign = 'center';
            toast.font = '32px sans-serif';
            //toast.size = 32;
            toast.color = 'red';
            scene.addChild(toast);
            function showToast(msg) {
                toast.text = msg;
                toast.x = GAME_W;
                toast.tl.tween({
                    x: 16,
                    time: 4
                }).tween({
                    x: -16,
                    time: 8,
                }).tween({
                    x: -GAME_W,
                    time: 4
                });
            }

            // 革命中かどうかのフラグ
            var isKakumei = false;
            
            // ゲーム終了かどうかのフラグ
            //var isFinish = false;

            // Webソケット接続
            try {
            	var path = gMonitor ? '/monitor/' + gRoomid
					                : '/play/' + gRule + '/' + gRoomid + '?name=' + gPlayername;
                socket = new WebSocket('ws://' + location.hostname + ':' + (location.port||80) + path);
                socket.onerror = onError;
                socket.onopen = onOpen;
                socket.onclose = onClose;
                socket.onmessage = onMessage;
            } catch (e) { alert(e.message); }

            // Webソケットハンドラ
            function onOpen(evt) { console.debug('connected.'); }
            function onClose(evt) { console.debug('closed.'); gameInfo.text = '接続されていません'; }
            function onError(evt) { console.error('websocket error! '); }
            function onMessage(evt) {
                console.debug("receive data=%O", evt.data);
                var obj = JSON.parse(evt.data);

                if (obj.Kind == "Exception") {
                    gameInfo.text = 'エラー';
                    alert(obj.Message);
                } else if (obj.Kind == "Tweet") {
                	pm.showTweet(obj.PlayerNum, obj.Message);
                } else if (obj.Kind == "Start") {
                	//startButton.tl.fadeOut(3).and().moveBy(0, 40, 4);
                	gameInfo.text = 'ゲーム開始';
                	yama.clear();
                	pm.clear();
                } else {
                	startButton.visible = false;
                	if (!pm.players.length) {
                		yama.drawBoxLine(YAMA_BORDER_COLOR);
                		pm.addPlayers(obj.PlayerInfo, obj.YourNum);
                		pm.doLayout();  // 手札の配置
                	}

                	pm.setPlayerInfo(obj.PlayerInfo, obj.AllDeck||obj.Deck, obj.Teban);  // 手札、プレイヤー情報を更新
                	yama.setYamaCards(obj.Yama.split(' '));

                	var withAnim = false;

                	if (obj.Kind == "Finish") {
                		okbutton.tl.fadeOut(3).moveTo(-10000, 0, 1);
                		startButton.y = GAME_H / 2 - 40;
                		startButton.tl.fadeIn(3).and().moveBy(0, -40, 4);
                		startButton.visible = true;
                		//var deck = pm.players[obj.Teban].deck;
                		//tebanMark.tl.tween({
                		//    x: deck.x, y: deck.y,
                		//    rotation: deck.rotation,
                		//    time: 3
                		//});

                	} else if (obj.Kind == "CardDistributed") {

                	} else if (obj.Kind == "CardsArePut") {
                		if (obj.Teban == obj.YourNum) {
                			okbutton.tl.fadeOut(3).and().moveBy(0, 40, 4).moveTo(-10000, 0, 1);
                			yama.drawBoxLine(YAMA_BORDER_COLOR);
                			//tebanMark.tl.fadeOut(3);
                		}
                		gameInfo.text = obj.IsKakumei ? "革命中" : '';
                		withAnim = true;
                		//yama.setBaCardsWithAnim(obj.Ba, pm.findPlayerById(obj.Teban).deck);
                		//for (var i = 0; i < pm.players.length; i++) {
                		//    pm.players[i].deck.setTeban(i == obj.Teban);  // 手番マークを設定
                		//    //pm.players[i].deck.setTeban(true);  // 手番マークを設定
                		//}
                		setTebanFunc(pm, obj.Teban);
                	} else if (obj.Kind == "Thinking") {
                		setTebanFunc(pm, obj.Teban);
                	} else if (obj.Kind == "Nagare") {
                		gameInfo.text = "流れました";
                		showToast("流れました");
                		//yama.setBaCardsWithAnim(obj.Ba, pm.findPlayerById(obj.Teban).deck);
                		yama.nagare();
                		setTebanFunc(pm, obj.Teban);
                	} else if (obj.Kind == "ProcessTurn") {
                		gameInfo.text = "あなたの番です";
                		setTebanFunc(pm, obj.Teban);
                		// 山の罫線を赤くする
                		yama.drawBoxLine(YAMA_BORDER_HICOLOR);
                		var mydeck = pm.findPlayerById(obj.YourNum).deck;
                		mydeck.touchEnable = true;

                		okbutton.x = yama.x + yama.width + 6;
                		okbutton.y = yama.y + yama.height + 46;
                		okbutton.tl.fadeIn(3).and().moveBy(0, -40, 4);

                		// OKタッチされた時のイベントハンドラを設定
                		okbutton.ontouchend = function () {
                			var selCards = mydeck.getSelectedCards();
                			var obj = new Object();
                			obj.Kind = 'Put';
                			obj.Cards = selCards.join(' ');
                			var jsonString = JSON.stringify(obj);
                			socket.send(jsonString);
                			console.debug('send: ' + jsonString);
                			//mydeck.touchEnable = false;
                			//okbutton.tl.fadeOut(3);
                		};

                	} else if (obj.Kind == "Kakumei") {
                		showToast(obj.IsKakumei ? '革命！' : '革命返し！');
                	} else if (obj.Kind == "Agari") {
                		var pi = obj.PlayerInfo[obj.Teban];
                		showToast(pi.Name + 'さん あがり！ (' + pi.OrderOfFinish + '位)');
                	}

                	yama.setBaCards(obj.Ba, pm.findPlayerById(obj.Teban).deck, withAnim);
                }
                
            }

            if (gAutostart) setTimeout(function () { SendStart(); }, 300);   // 自動スタート
            return scene;
        };

        if (gMonitor || gPlayername && gRoomid) {
        	game_.replaceScene(mainScene());
        } else {
        	game_.replaceScene(titleScene());
        }
    }

    game_.start(); // ゲームをスタートさせます
};

