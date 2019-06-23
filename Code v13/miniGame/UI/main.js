var UI_makename = require('makename.js')
exports.show = show

function show() {

  ctxUI.drawImage(RIMG_UI, 0, 0, 188, 300, 0, 0, 188, 300)

  if (_g.Player.Name == "@") {
    UI_makename.show()
  }

  ctxDevice.drawImage(canvasUI, 0, 0, canvasUI.width, canvasUI.height, 0, UI_VPOS, canvasDevice.width, UI_HEIGHT)
}

exports.Initialize = () => {
  GameGlobal.RIMG_UI = wx.createImage();
  GameGlobal.UI_VPOS = (canvasDevice.height - canvasDevice.width * 8 / 5) / 2;
  GameGlobal.UI_HEIGHT = canvasDevice.width * 8 / 5

  RIMG_UI.src = "\images/UI.png"
  RIMG_UI.onload = () => {
    show()
  }
}