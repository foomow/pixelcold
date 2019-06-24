exports.initialize = () => {
  if (GameGlobal.GameMainUI === undefined) {
    GameGlobal.GameMainUI = new UIObj.UIGame()
    let image = wx.createImage();
    image.src = "images/UI.png"
    image.onload = () => {
      GameMainUI.background = image

      var btn = new UIObj.UIButton(GameMainUI)
      btn.x = 44
      btn.y = 150
      btn.text = "退出游戏"
      btn.onClick = () => {
        wx.closeSocket()
      }

      GameWindow = GameMainUI
      GameWindow.show()
    }
  } else {
    GameWindow = GameMainUI
    GameWindow.show()
  }
}