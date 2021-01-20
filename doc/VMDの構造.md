# VMDファイルの構造

* フレームNoなど整数型は符号なし整数型として考えて記載してます。
* 1フレームは 50ms 、30 fps の設定です。
* Locationなどの位置情報は [cm]/[MMD長さ]＝1/8 (0.125)説を採用してます。

## Header

| No.  | データ名      | サイズ |   型   | 備考                             |
| :--- | :------------ | :----: | :----: | :------------------------------- |
| 1    | FileSignature |   30   | String | "Vocaloid Motion Data 0002" 固定 |
| 2    | ModelName     |   20   | String | 保存した際のモデル名             |


## Motion (Bone)

Boneの角度情報等が集まった一連の動作をMotionと表現している。

| No.  | データ名   |    サイズ    |     型      | 備考 |
| :--- | :--------- | :----------: | :---------: | :--- |
| 1    | Count      |      4       |    uint     |      |
| 2    | MotionData | 111  * Count | ST_BoneData |      |

* ST_BoneData

| No.   | データ名             | サイズ |        型        | 備考                                                                                  |
| :---- | :------------------- | :----: | :--------------: | :------------------------------------------------------------------------------------ |
| 1     | BoneName             |   15   |      string      |                                                                                       |
| 2     | FrameNo              |   4    |       uint       |                                                                                       |
| 3     | Location             |   12   | three_dimensions |                                                                                       |
| 3-1   | -> X                 |   4    |      float       |                                                                                       |
| 3-2   | -> Y                 |   4    |      float       |                                                                                       |
| 3-3   | -> Z                 |   4    |      float       |                                                                                       |
| 4     | Rotatation           |   16   |    Quaternion    | Y,X,Z,W として扱うとMMD表示上の数値と合うので、たぶんこの順番                         |
| 4-2   | -> Y                 |   4    |      float       |                                                                                       |
| 4-1   | -> X                 |   4    |      float       | X軸のオイラー角度はMMDが０度の時、計算結果は１８０度となる                            |
| 4-3   | -> Z                 |   4    |      float       |                                                                                       |
| 4-4   | -> W                 |   4    |      float       |                                                                                       |
| 5     | Interpolation        |   64   |  vector * 4 * 2  | 並び順のルールはバージョンによって違う模様。<br>2020/09/26 時点の解析結果を次章に記す |
| 5-1-1 | -> Xaxis.Start (x,y) |   8    |      vector      |                                                                                       |
| 5-1-2 | -> Xaxis.End (x,y)   |   8    |      vector      |                                                                                       |
| 5-2-1 | -> Yaxis.Start (x,y) |   8    |      vector      |                                                                                       |
| 5-2-2 | -> Yaxis.End (x,y)   |   8    |      vector      |                                                                                       |
| 5-3-1 | -> Zaxis.Start (x,y) |   8    |      vector      |                                                                                       |
| 5-3-2 | -> Zaxis.End (x,y)   |   8    |      vector      |                                                                                       |
| 5-4-1 | -> Raxis.Start (x,y) |   8    |      vector      |                                                                                       |
| 5-4-2 | -> Raxis.End (x,y)   |   8    |      vector      |                                                                                       |


## Skin

| No.  | データ名 |   サイズ    |     型      | 備考 |
| :--- | :------- | :---------: | :---------: | :--- |
| 1    | Count    |      4      |    uint     |      |
| 2    | SkinData | 23  * Count | ST_SkinData |      |

* ST_SkinData

| No.  | データ名 | サイズ |   型   | 備考 |
| :--- | :------- | :----: | :----: | :--- |
| 1    | SkinName |   15   | string |      |
| 2    | FlameNo  |   4    |  uint  |      |
| 3    | Weight   |   4    | float  |      |


## Camera

| No.  | データ名   |   サイズ    |      型       | 備考 |
| :--- | :--------- | :---------: | :-----------: | :--- |
| 1    | Count      |      4      |      int      |      |
| 2    | CameraData | 61  * Count | ST_CameraData |      |

* ST_CameraData

MMDのカメラはLocationの位置を原点に、原点からの距離とカメラの角度で決めている。

| No.  | データ名               | サイズ |        型        | 備考                                               |
| :--- | :--------------------- | :----: | :--------------: | :------------------------------------------------- |
| 1    | FlameNo                |   4    |       uint       |                                                    |
| 2    | Length                 |   4    |      float       | VMDファイル内の値は、MMDの表示から「-1」かけた数値 |
| 3    | Location               |   12   | three_dimensions |                                                    |
| 3-1  | -> X                   |   4    |      float       |                                                    |
| 3-2  | -> Y                   |   4    |      float       |                                                    |
| 3-3  | -> Z                   |   4    |      float       |                                                    |
| 4    | Rotatation             |   12   | three_dimensions |                                                    |
| 4-1  | -> X                   |   4    |      float       | MMDの座標系のX軸は符号が反転しているので注意       |
| 4-2  | -> Y                   |   4    |      float       |                                                    |
| 4-3  | -> Z                   |   4    |      float       |                                                    |
| 5    | Interpolation          |   24   |     byte[24]     |                                                    |
| 5-1  | -> XAxis.stat.X        |   1    |       byte       |                                                    |
| 5-2  | -> XAxis.end.X         |   1    |       byte       |                                                    |
| 5-3  | -> XAxis.stat.Y        |   1    |       byte       |                                                    |
| 5-4  | -> XAxis.end.Y         |   1    |       byte       |                                                    |
| 5-5  | -> YAxis.stat.X        |   1    |       byte       |                                                    |
| 5-6  | -> YAxis.end.X         |   1    |       byte       |                                                    |
| 5-7  | -> YAxis.stat.Y        |   1    |       byte       |                                                    |
| 5-8  | -> YAxis.end.Y         |   1    |       byte       |                                                    |
| 5-9  | -> ZAxis.stat.X        |   1    |       byte       |                                                    |
| 5-10 | -> ZAxis.end.X         |   1    |       byte       |                                                    |
| 5-11 | -> ZAxis.stat.Y        |   1    |       byte       |                                                    |
| 5-12 | -> ZAxis.end.Y         |   1    |       byte       |                                                    |
| 5-13 | -> Rotation.stat.X     |   1    |       byte       |                                                    |
| 5-14 | -> Rotation.end.X      |   1    |       byte       |                                                    |
| 5-15 | -> Rotation.stat.Y     |   1    |       byte       |                                                    |
| 5-16 | -> Rotation.end.Y      |   1    |       byte       |                                                    |
| 5-17 | -> Length.stat.X       |   1    |       byte       |                                                    |
| 5-18 | -> Length.end.X        |   1    |       byte       |                                                    |
| 5-19 | -> Length.stat.Y       |   1    |       byte       |                                                    |
| 5-20 | -> Length.end.Y        |   1    |       byte       |                                                    |
| 5-21 | -> ViewingAngle.stat.X |   1    |       byte       |                                                    |
| 5-22 | -> ViewingAngle.end.X  |   1    |       byte       |                                                    |
| 5-23 | -> ViewingAngle.stat.Y |   1    |       byte       |                                                    |
| 5-24 | -> ViewingAngle.end.Y  |   1    |       byte       |                                                    |
| 6    | ViewingAngle           |   4    |       uint       |                                                    |
| 7    | Perspective            |   1    |       bool       |                                                    |

## Illumination

| No.  | データ名         |   サイズ    |         型          | 備考 |
| :--- | :--------------- | :---------: | :-----------------: | :--- |
| 1    | Count            |      4      |         int         |      |
| 2    | IlluminationData | 28  * Count | ST_IlluminationData |      |

* ST_IlluminationData

| No.  | データ名 | サイズ |        型        | 備考                               |
| :--- | :------- | :----: | :--------------: | :--------------------------------- |
| 1    | FlameNo  |   4    |       uint       |                                    |
| 2    | Length   |   16   |      color       |                                    |
| 2-1  | -> R     |   4    |      float       | 計算式： RGB値=VMDファイルの値*256 |
| 2-2  | -> G     |   4    |      float       | 計算式： RGB値=VMDファイルの値*256 |
| 2-3  | -> B     |   4    |      float       | 計算式： RGB値=VMDファイルの値*256 |
| 3    | Location |   12   | three_dimensions |                                    |
| 3-1  | -> X     |   4    |      float       |                                    |
| 3-2  | -> Y     |   4    |      float       |                                    |
| 3-3  | -> Z     |   4    |      float       |                                    |


## SelfShadow

| No.  | データ名       |   サイズ    |        型         | 備考 |
| :--- | :------------- | :---------: | :---------------: | :--- |
| 1    | Count          |      4      |        int        |      |
| 2    | SelfShadowData | 61  * Count | ST_SelfShadowData |      |

* ST_SelfShadowData

| No.  | データ名 | サイズ |  型   | 備考                             |
| :--- | :------- | :----: | :---: | :------------------------------- |
| 1    | FlameNo  |   4    | uint  |                                  |
| 2    | Mode     |   4    | uint  | 0:影なし、1:モード１、2:モード２ |
| 3    | Distance |   12   | float | 計算式： (0.1 - dist) / 0.00001  |

## IK

| No.  | データ名      |                   サイズ                    |        型        | 備考 |
| :--- | :------------ | :-----------------------------------------: | :--------------: | :--- |
| 1    | Count         |                      4                      |       int        |      |
| 2    | IKVisibleData | 12 * Count<br>+ 21 * ST_IKVisibleData.Count | ST_IKVisibleData |      |

* ST_IKVisibleData

| No.  | データ名 |   サイズ   | 型        | 備考 |
| :--- | -------- | :--------: | :-------- | :--- |
| 1    | FlameNo  |     4      | uint      |      |
| 1    | Visible  |     4      | int       |      |
| 1    | Count    |     4      | int       |      |
| 2    | IKData   | 21 * Count | ST_IKData |      |



* ST_IKData （IKデータ用構造）

| No.  | データ名 | サイズ |   型   | 備考 |
| :--- | :------- | :----: | :----: | :--- |
| 1    | BoneName |   20   | string |      |
| 2    | Enabled  |   1    |  bool  |      |

## その他


### Expansion

VMDファイルとは関係なく、本ツール独自の設定などの情報

| No.  | データ名         | サイズ |   型   | 備考                                                                                                           |
| :--- | :--------------- | :----: | :----: | :------------------------------------------------------------------------------------------------------------- |
| 1    | Version          |   -    |  int   | HeaderのFileSignature の最後の数値を数値化したもの                                                             |
| 2    | TargetID         |   -    |  int   | 出力したJSONファイルを特定できるように、引数「-T」で設定したID。<br>未設定時は "-1"                            |
| 3    | StartFrame       |   -    |  int   | VMDファイルは"0"から始まっているが、スタートフレーム番号を改変した際に設定した引数「-S」で設定したフレーム番号 |
| 4    | CoordinateSystem |   -    | string | 出力している座標系                                                                                             |
| 5    | FileType         |   -    | string | (予約) "VMD"固定。 VPD対応した際に変換元のデータタイプを把握できるようにするための値                           |
| 6    | GroupType        |   -    | string | モーションデータのグルーピング。MMD標準だと"NONE"で出力。                                                      |

---

## モーション Interpolationの解析結果

調査: 2020/09/25

Ver 9.32 (64bit ver) だとモーションの補完曲線のデータ[６４バイト]の並び順は下記のようになっていた。

```
 X  Y  Z  R  X  Y  Z  R  X  Y  Z  R  X  Y  Z  R
 x  x  x  x  y  y  y  y  x  x  x  x  y  y  y  y
start------------------ end-------------------
06 15 00 00 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
   15 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
   00 21 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F
   00 00 30 49 56 5F 69 0E 1A 27 39 51 5A 63 6F 00 00 00
```

上記のことから補完曲線のデータは **「１６バイト、先頭[X.x]がない１５バイト×３、"00 00 00" 」** で出力されていると思われる。

バージョンによってデータが"00"になる欠けが違うようなので、
取り出し方は、先頭からバイトを取り出して"00"以外ならば その値は確定という判断のやり方が良いと思われる。

