
# UniRmmzとは
RPGツクールMZで作成したプロジェクトデータを、Unity上で動かすためのライブラリです。
これまでRPGツクールでゲームを作っていたが、Unityでのゲーム制作に挑戦したい人の足掛かりになればと思います。

自身のツクール製ゲームのコンシューマ移植にも役立てるかもしれませんが、現状、実績はありません。

# 開発状況
ツクールのゲームデータを実行するための基本的な機能は実装済みです。
ただし、UniRmmzはまだα版です。仕様や内部実装が大きく変わる可能性があります。
また、最適化や不具合修正もまだこれからという状況です。


# 導入方法
## 1. UniRmmzのダウンロード
https://github.com/UniRmmz/UniRmmz
Code > Download ZIP
からUniRmmzの最新のコードをダウンロードし、てきとうなフォルダに解凍します

## 2. ツクールプロジェクトをコピー
Unityで動かしたいゲームのprojectフォルダを
Assets/StreamingAssets
に丸ごとコピーします

## 3. Unityを起動
現在、UniRmmzは Unity6.9.23f1 で開発されていますので、インストールしてください。
ダウンロードしたフォルダを Unity で開きます。


## 4. コンバート用のコマンドを実行
最初に、いくつかのコンバート処理が必要です。
Unityのメニュー > UniRmmz > Tools から下記のコマンドを実行してください

### Convert Effekseer
ツクールMZ形式のアニメーションエフェクト（Effekseer）をUnityのResourcesフォルダにコピーし、assetファイルを生成します。
Effekseerを使用していない場合、このコマンドはスキップ可能です。

### Generate Font Asset
UniRmmzはテキスト描画に TextMeshPro を採用しており、デフォルトのフォント以外はこのコマンドでテキストassetファイルを生成する必要があります。
コマンドを実行すると、フォントデータ毎にFont Asset Creatorダイアログが表示されるので、
1. Generate Font Atlas
2. Save
3. ダイアログを閉じる
の順に操作します。

尚、対応しているフォントのフォーマットは .ttf と .otf です。

## 4. プラグインなどのjavascriptコードの移植（スキップ可）
RPGツクールMZのゲームは javascript で動いていますが、Unity では C# を使用します。
UniRmmz はコアスクリプト部分については C# に移植してますが、ツクール製ゲームを完全に移植するには、他にも
- プラグイン
- ダメージ計算式
- スクリプトコマンド
の移植が必要です。
これらの移植をスキップした場合でも、不完全ながらもゲームの実行は可能です（警告ログが出ます）

### プラグインの移植
UniRmmzでは、プラグインについては基本ユーザー自身に移植してもらう想定です。
ただし、UniRmmz本体のコードを直接あちこち弄らなくても、自作のクラスのコードに差し替える仕組みを用意しています。
Assets/Scripts/UniRmmz/UniRmmzFactory.cs をご覧ください。

TODOサンプル

### ダメージ計算式の移植
UniRmmzはツクールのデータベースに設定されているダメージ計算式を読み取って、”ある程度”自動で C# に変換する仕組みを用意しています。
Unityのメニュー > UniRmmz > Tools から GenerateDamageFormula コマンドを実行してください。
成功すると、Assets/Scripts/UniRmmz/Generated/RmmzDamageFormula.Generated.cs　に自動生成コードが出力されます。
ビルドエラーが出るなど、変換がうまくいかなかった場合は、正しいコードを修正してください。

### スクリプトコマンドの移植
UniRmmzはツクールのデータベースに設定されているスクリプトコマンドを読み取って、”ある程度”自動で C# に変換する仕組みを用意しています。
Unityのメニュー > UniRmmz > Tools から GenerateScriptCommand コマンドを実行してください。
成功すると、Assets/Scripts/UniRmmz/Generated/RmmzScriptCommand.Generated.cs に自動生成コードが出力されます。
ビルドエラーが出るなど、変換がうまくいかなかった場合は、正しいコードを修正してください。

## 5. ゲームの実行
Assets/Scenes/SampleScene.unityを開き、シーンを再生してください

# 注意事項
- UniRmmzは、RPG Maker MZを提供している株式会社 Gotcha Gotcha Games の公式プロジェクトではありません。
- UniRmmzが動かすゲームデータの作成には、別途 RPGツクールMZ が必要です

# 既知の問題
- 音声が正しくループしない（終端でループする）
- フォントファイルに差分が出る
- リソースロードのリトライに未対応