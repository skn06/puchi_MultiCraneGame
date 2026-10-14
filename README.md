# puchi_MultiCraneGame

Unity 2022.3.4f1。使用シーン: `Assets/Scenes/MultiCraneGame.unity`。

## 速度の設定

Hierarchyの`MainCrane1`を選択し、`Crane Unit` → `Speed [m/min]`に直接数値を入力する。

| Inspector項目 | 方向 | シーンの設定値 |
| --- | --- | ---: |
| Forward Back Speed | 前後 | 50 m/min |
| Left Right Speed | 左右 | 50 m/min |
| Up Down Speed | 中央の板の上下 | 15 m/min |

1 Unity unit = 1 mとして、親TransformのScaleを補正して移動する。速度段階を切り替える操作はない。ジョイスティックの前後・左右は、倒した量に応じて設定速度の0～100%で動く。上下は常に指定速度で動く。0を指定するとその方向は停止する。

以前のコードも速度の直接指定だったが、ローカル座標に加算していたため親Scaleが速度に掛かっていた。旧シーンの実際の速度は前後20、左右約171.15、上下1.8 m/min。今回の補正により、同じInspector値でも左右は以前より遅く、上下は以前より速くなる。

## 操作

`CraneController`の`Crane Operation Manager` → `Input Mode`で切り替える。

| 操作 | Keyboard | Joystick |
| --- | --- | --- |
| 前後 | E / D | JoyStick2Vertical（既存設定） |
| 左右 | S / F | JoyStick2Horizontal（既存設定） |
| 下降／上昇の切り替え | T | 1台目のButton 0（トリガー） |

上下操作は押し続ける必要がない。最初の押下で下限まで下降して停止し、次の押下で上限まで上昇して停止する。移動中に押すと方向が反転する。再生直後は元の初期位置で停止しており、最初の押下で下降する。

ジョイスティックのボタンは`Lift Toggle` → `Joystick Lift Toggle Button`で変更できる。ボタン番号と実物の対応は機種およびOSの認識順に依存する。初期値`Joystick1Button0`は、既存の移動軸と同じ1台目を指定している。Keyboard時はT、Joystick時は設定したボタンだけを読む。

## 中央の板と移動範囲

`MainCrane1` → `MainLifMag` → `LifMag(Center)`だけを残している。`MainLifMag`は左右移動用、`LifMag(Center)`は上下移動用。外側4つのリフマグと子部品は削除し、近景・遠景の俯瞰カメラは表示位置を保って中央の板の子へ移している。

`Crane Unit`の`Movement Ranges (local position)`で範囲を変更できる。

- Min Z / Max Z: MainCraneの前後範囲。
- Min X / Max X: MainLifMagの左右範囲。
上下範囲は`Lift Limits (local Y)`で設定する。

- Use Initial Position As Upper Limit: ON（初期設定）なら再生開始時の`LifMag(Center)`の位置を上限として記録する。Tキーで上昇すると、この初期位置で停止する。
- Min Y: 下限のローカルY座標。初期位置より小さい値を指定する。上限より高い値を指定した場合は、上限位置までに制限する。
- Max Y: チェックをOFFにした場合の上限のローカルY座標。ONの場合は使用しない。

初期位置を変える場合は、再生を止めて`LifMag(Center)`のTransform → Position Yを編集してから再生する。InspectorのMin Y / Max Yはワールド座標ではなく、親`MainLifMag`を基準とする座標。リフマグ間の距離調整機能はない。シーンに残っていた削除済みの吸着・センサースクリプト参照も除去している。
