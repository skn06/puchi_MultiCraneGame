# puchi_MultiCraneGame

Unity 2022.3.4f1。使用シーン: `Assets/Scenes/MultiCraneGame.unity`。

## 入力モードと割り当て

Hierarchyの`CraneController`を選択し、`Crane Operation Manager`の
`Input Mode`を`Keyboard`または`Joystick`に変更する。
シーンの初期設定は`Keyboard`。選択したモードの入力だけを受け付ける。
カメラの入力設定とは独立している。

| 機能 | Keyboard初期値 | Joystick初期値 | Inspector項目 |
| --- | --- | --- | --- |
| 前後移動 | E: +Z / D: -Z | JoyStick2Vertical | Forward Key / Backward Key、Joy Stick Vertical |
| 左右移動 | S: +X / F: -X | JoyStick2Horizontal | Positive X Key / Negative X Key、Joy Stick Horizontal |
| 降下開始／降下中の停止 | T | Joystick1Button0 | Lift Toggle Key / Joystick Lift Toggle Button |
| 爪の手動開閉 | G | Joystick1Button1 | Claw Toggle Key / Joystick Claw Toggle Button |
| 結果画面から次ゲーム開始 | Space | Joystick1Button2 | Next Game Key / Joystick Next Game Button |

座標方向はクレーンの移動軸を表す。画面上の方向はカメラの向きによって変わる。

- ボタンは押した瞬間に1回動作する。押し続ける必要はない。
- ボタン欄はKeyCodeの選択リスト。未割り当てにする場合は`None`を選ぶ。
- 軸欄にはInput Managerに登録済みの軸名を入力する。空欄にするとその方向のジョイスティック入力を無効にできる。
- 現在の`JoyStick2Horizontal`と`JoyStick2Vertical`は、名前に「2」が付いているが1台目のジョイスティックのX/Y軸。
- スティックを倒した量に応じて速度が変わる。`Dead Zone`は初期値0.1。Input Manager側にもデッドゾーン設定がある。
- 動く方向が逆の場合は`Invert Horizontal`または`Invert Vertical`を有効にする。
- 実物のボタンと番号の対応は機種・OSの認識順に依存する。使用機器に合わせてInspectorで選択する。

## ゲームの動作

ゲーム開始から15秒間、前後左右を操作できる。
Tまたは割り当てた降下ボタンを押すか、制限時間を過ぎると爪を開いて自動降下する。

降下中に同じボタンを押す、爪や上部連結部が景品・停止対象の床や台に接触する、
または下限に到達すると停止する。
1秒待って爪を閉じ、さらに1秒待って自動上昇する。
閉じ終わったときに2本以上の爪が同じ景品に接触していれば、その景品を保持する。

上端で1秒待って取出口へ自動移動し、到着から1秒後に爪を開いて景品を放す。
成功・失敗を表示し、クレーンが初期位置へ戻ってから次ゲーム開始を受け付ける。
次ゲームでは景品をランダムに再配置し、制限時間をリセットする。

爪の手動開閉は通常操作中のみ有効。
自動動作中および爪の開閉中は前後左右の操作を受け付けない。
再開ボタンも結果表示と初期位置への復帰が完了するまで無効。

## 速度と移動範囲

`MainCrane1`の`Crane Unit`で設定する。

| Inspector項目 | 対象 | 現在のシーン設定 |
| --- | --- | --- |
| Forward Back Speed | MainCrane1の前後移動 | 200 m/min |
| Left Right Speed | MainLifMagの左右移動 | 200 m/min |
| Up Down Speed | LifMag(Center)の上下移動 | 200 m/min |
| Play Time Limit | 操作制限時間 | 15秒 |
| Min Z / Max Z | MainCrane1のローカルZ範囲 | -3.5 / 3.5 |
| Min X / Max X | MainLifMagのローカルX範囲 | -0.185 / 0.185 |
| Min Y / Max Y | LifMag(Center)のローカルY範囲 | -7.15 / 2.5 |

1 Unity unit = 1 mとして親TransformのScaleを補正する。
上下は指定速度で自動移動する。各待機時間も`Crane Unit`で変更できる。

## カメラ

カメラの操作は今回のクレーン入力モードに連動しない。
`DebugFreeCamera`は従来どおり、↑/↓で前後移動、左Shiftで移動速度3倍、
左ドラッグで平行移動、右ドラッグで回転する。
有効な2台の自由操作カメラは同じ入力で同時に動く。
