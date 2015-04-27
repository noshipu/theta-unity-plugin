### THETA Unity Plugin

### 説明
360度カメラのTHETAのAPIをUnityから実行するプラグインです。  
このPluginを使用する場合、RICOH THETA SDKが必要となりますので、別途利用規約を確認し、THETAデベロッパーサイトからダウンロードお願いします。   
https://developers.theta360.com/ja/docs/sdk/  

### 備考
すみません。現在、iOSのみで一部のAPIのみ対応してます。(※順次更新予定)  

### 動作確認環境
 ・Unity5  
 ・RICOH THETA SDK for iOS ver0.3.0  
 ・iOS8以上の端末  

### 組み込み手順(iOS)
(1) theta_unity_plugin.unitypackageをご自身のプロジェクトにimportしてください  
(2) THETA DEVELOPERS サイトにてSDKをダウンロード  
https://developers.theta360.com/ja/docs/sdk/  
(3) Assets/ThetaSDK/Plugins/iOS/以下に、(2)でダウンロードしたフォルダに入っているlibフォルダをコピー  
(4) Assets/ThetaSDK/ThetaPlugin.csをゲームオブジェクトにアタッチしGetComponectして使ってください。  
詳細はコードの使い方からどうぞ  
(5) ビルド後、Xcodeでやること  
・以下のライブラリをプロジェクトに追加  
libiconv.dylib  
ImageIO.framework  
・[Build Settings]->[Apple LLVM 6.0 - Language - Objective C]->[Enable Objective-CExceptions]を[Yes]に変更  
http://noshipu.hateblo.jp/entry/2015/04/15/023431  

### コードの使い方
## コンポーネント取得
`thetaPlugin = GetComponent<ThetaPlugin> ();`  
## APIの呼び出し基本
`thetaPlugin.Connect (ip_adress, ()=>{/*成功処理*/ }, ()=>{/*失敗処理*/});`  
## コールバックの設定の基本
`thetaPlugin.SetCallbackObjectAdd ((texture)=>{/*テクスチャ取得後の処理*/ });`  

### Sample Project
SampleProjectにて、接続、解除、撮影、画像の取得を確認することができます。
