var globaldata = require('global.js')
var ziku = require('han.js')
var net = require('net.js')
globaldata.initialize()
GameGlobal.perf = wx.getPerformance()
wx.getSystemInfo({
  success(res) {
    _g.DEVICEWIDTH = res.windowWidth;
    _g.DEVICEHEIGHT = res.windowHeight;
    GameGlobal.UIObj = require('UIOBJECT.js')
    wx.getSetting({
      success(res) {
        if (!res.authSetting['scope.userInfo']) {
          let button = wx.createUserInfoButton({
            type: 'text',
            text: '请允许我们使用您的基本资料',
            style: {
              left: _g.DEVICEWIDTH / 2 - 150,
              top: 200,
              width: 300,
              height: 40,
              lineHeight: 40,
              backgroundColor: '#ff0000',
              color: '#ffffff',
              textAlign: 'center',
              fontSize: 16,
              borderRadius: 4
            }
          })
          button.onTap((res) => {
            if (res.errMsg == "getUserInfo:ok") {
              button.hide()
              loadziku()
            }
          })
        } else {
          loadziku()
        }
      }
    })
  }
})

function loadziku() {
  ziku.install(
    function() {
      let image = wx.createImage();
      image.src = "images/cover.png"
      image.onload = () => {
        GameGlobal.GameCover = new UIObj.UIGame()
        GameCover.background = image        
       
        var btn = new UIObj.UIButton(GameCover)
        btn.x = 44
        btn.y = 180
        btn.text = "开始游戏"
        btn.onClick=net.initialize
        
        GameGlobal.GameWindow=GameCover
        GameWindow.show()
        require("gesture.js")
      }
    })
}