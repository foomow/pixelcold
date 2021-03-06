exports.initialize = () => {
  const fs = wx.getFileSystemManager()
  fs.readFile({
    filePath: "/12.bin",
    success: (res) => {
      FontData = new Uint8Array(res.data)
      var canvasDevice = wx.createCanvas()
      canvasDevice.width = _g.DEVICEWIDTH
      canvasDevice.height = _g.DEVICEHEIGHT
      var ctxDevice = canvasDevice.getContext('2d')
      var canvasUI = wx.createCanvas()
      canvasUI.width = 188
      canvasUI.height = 300
      var ctxUI = canvasUI.getContext('2d')
      ctxUI.font = "12px simhei";
      ctxUI.strokeStyle = "#fff"
      ctxUI.strokeText("你好", 10, 15)
      drawText(ctxUI, "你好", 10, 50)
      ctxDevice.drawImage(canvasUI, 0, 0, canvasUI.width, canvasUI.height, 0, 100, canvasDevice.width, canvasDevice.width * 8 / 5)
    }
  })
  
  wx.connectSocket({
    url: _g.WEBSOCKET_URL,
    method: "GET",
    success(res) {
      console.log("Socket successed:")
      console.log(res)
      wx.onSocketOpen(
        (res) => {
          console.log("Socket opened:")
          console.log(res)
        }
      )
      wx.onSocketClose(
        (res) => {
          console.log("Socket closed:")
          console.log(res)
        }
      )
      wx.onSocketError((res) => {
        console.log("Socket error:")
        console.log(res)
      })
      wx.onSocketMessage((res) => {
        console.log(res)
        parseMessage(JSON.parse(res.data))
      })
    },
    fail(res) {
      console.log("Socket failed:")
      console.log(res)
    }
  })
}

function parseMessage(msg) {
  if (msg.command == 1) //这是初次连接成功传回的结果
  {
    _g.COMMAND = JSON.parse(msg.data);
    login()
  } else {
    switch (msg.command) //其他命令的处理结果
    {
      case _g.COMMAND.LOGIN:        
        _g.Player=JSON.parse(msg.data)
        wx.getUserInfo({
          success(res){
            _g.Player.nickName = res.userInfo.nickName
            _g.Player.avatarUrl = res.userInfo.avatarUrl
          }
        })
        console.log(_g.Player)
        
        break;
    }
  }
}

function login() {
  wx.login({
    success(res) {
      console.log(res)
      var data = {
        command: _g.COMMAND.LOGIN,
        data: res.code
      }
      wx.sendSocketMessage({
        data: JSON.stringify(data)
      })
    }
  })
}

