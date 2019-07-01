exports.initialize = () => {
  GameWindow.children[0].hide()
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
          GameWindow = GameCover
          GameWindow.show()
          GameWindow.children[0].show()
          console.log(res)
        }
      )
      wx.onSocketError((res) => {
        console.log("Socket error:")
        GameWindow = GameCover
        GameWindow.show()
        GameWindow.children[0].show()
        console.log(res)
      })
      wx.onSocketMessage((res) => {
        if (typeof (res.data) == "object") {
          var bytes = new Uint8Array(res.data)
          GameMainUI.drawScene(bytes)
          setTimeout(Refresh, 5) 
        } else {
          console.log("msg recevied:")
          console.log(res)
          parseMessage(JSON.parse(res.data))
        }
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
      case _g.COMMAND.JOINGAME:
        {
          if (msg.data === "OK") {
            let nextUI = require('/UI/main.js')
            nextUI.initialize()
            Refresh()
          } else {
            let nextUI = require('/UI/makename.js')
            nextUI.initialize()
          }
        }
        break
      case _g.COMMAND.CREATECHARACTER:
        _g.Player = JSON.parse(msg.data)
        console.log(_g.Player)
        if (_g.Player.Name === "@") {
          let nextUI = require('/UI/makename.js')
          nextUI.initialize()
        } else {
          var data = {
            command: _g.COMMAND.JOINGAME,
            data: ""
          }
          wx.sendSocketMessage({
            data: JSON.stringify(data)
          })
        }
        break
      case _g.COMMAND.MAKENAMES:
        _g.Player.NameList = JSON.parse(msg.data)
        let nextUI = require('/UI/makename.js')
        nextUI.UpdateName()
        break
      case _g.COMMAND.LOGIN:
        _g.Player = JSON.parse(msg.data)
        wx.getUserInfo({
          success(res) {
            _g.Player.nickName = res.userInfo.nickName
            _g.Player.avatarUrl = res.userInfo.avatarUrl
            console.log(_g.Player)
            if (_g.Player.Name === "@") {
              let nextUI = require('/UI/makename.js')
              nextUI.initialize()

            } else {
              var data = {
                command: _g.COMMAND.JOINGAME,
                data: ""
              }
              wx.sendSocketMessage({
                data: JSON.stringify(data)
              })
            }
          }
        })

        break
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

function Refresh() {
  if (GameWindow === GameMainUI) {
    var data = {
      command: _g.COMMAND.REFRESH,
      data: ""
    }
    wx.sendSocketMessage({
      data: JSON.stringify(data)
    })
  }
}