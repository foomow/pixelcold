var btn
exports.initialize = () => {
  if (GameGlobal.GameMainUI === undefined) {
    GameGlobal.GameMainUI = new UIObj.UIGame()
    let image = wx.createImage();
    image.src = "images/UI.png"
    image.onload = () => {
      GameMainUI.background = image
      btn = new UIObj.UIButton(GameMainUI)
      btn.x = 44
      btn.y = 235
      btn.text = _g.Player.Name
      btn.onClick = () => {
        GameWindow = GameCover
        wx.closeSocket()
      }
    }
  } else {
    btn.text = _g.Player.Name    
  }
  GameWindow = GameMainUI
  GameWindow.show() 
}
