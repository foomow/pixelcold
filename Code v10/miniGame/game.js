var globaldata = require('global.js')
var net = require('net.js')
globaldata.initialize()

wx.getSystemInfo({
  success(res) {
    _g.DEVICEWIDTH = res.windowWidth;
    _g.DEVICEHEIGHT = res.windowHeight;

    wx.getSetting({
      success(res) {
        if (!res.authSetting['scope.userInfo']) {
          let button = wx.createUserInfoButton({
            type: 'text',
            text: '请允许我们使用您的基本资料',
            style: {
              left: _g.DEVICEWIDTH/2-150,
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
            if (res.errMsg == "getUserInfo:ok")
            {
              button.hide()
              net.initialize()
            }
          })
        }
        else{
          net.initialize()
        }
      }
    })
  }
})
