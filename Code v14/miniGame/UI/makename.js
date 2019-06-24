var sexSelectItem
var btnSex1
var btnSex2
var nameSelectItem
var btnName0
var btnName1
var btnName2
var btnName3
var btnSubmit
var btnRename
exports.initialize = () => {
  if (GameGlobal.GameMakenameUI === undefined) {
    GameGlobal.GameMakenameUI = new UIObj.UIGame()
    let image = wx.createImage();
    image.src = "images/UI.png"
    image.onload = () => {
      GameMakenameUI.background = image
      createMakenameUI()
    }
  } else {
    showMakenameUI()
  }
}

function createMakenameUI() {
  var label1 = new UIObj.UILabel(GameMakenameUI, 44, 8)
  label1.text = "设定角色"

  var label2 = new UIObj.UILabel(GameMakenameUI, 8, 50, 60)
  label2.text = "性别："

  var label3 = new UIObj.UILabel(GameMakenameUI, 8, 80, 60)
  label3.text = "姓名："

  btnSex1 = new UIObj.UIButton(GameMakenameUI, 60, 50, 30)
  btnSex1.text = "男"
  btnSex1.name = 1
  btnSex1.select()
  btnSex1.onClick = () => {
    if (sexSelectItem) sexSelectItem.unselect()
    btnSex1.select()
    sexSelectItem = btnSex1
  }
  sexSelectItem = btnSex1

  btnSex2 = new UIObj.UIButton(GameMakenameUI, 110, 50, 30)
  btnSex2.text = "女"
  btnSex2.name = 0
  btnSex2.onClick = () => {
    if (sexSelectItem) sexSelectItem.unselect()
    btnSex2.select()
    sexSelectItem = btnSex2
  }

  btnName0 = new UIObj.UIButton(GameMakenameUI, 60, 90, 100)
  btnName0.text = _g.Player.NameList[0]
  btnName0.name = 0
  btnName0.select()
  btnName0.onClick = () => {
    if (nameSelectItem) nameSelectItem.unselect()
    btnName0.select()
    nameSelectItem = btnName0
  }
  nameSelectItem = btnName0

  btnName1 = new UIObj.UIButton(GameMakenameUI, 60, 120, 100)
  btnName1.text = _g.Player.NameList[1]
  btnName1.name = 1
  btnName1.onClick = () => {
    if (nameSelectItem) nameSelectItem.unselect()
    btnName1.select()
    nameSelectItem = btnName1
  }

  btnName2 = new UIObj.UIButton(GameMakenameUI, 60, 150, 100)
  btnName2.text = _g.Player.NameList[2]
  btnName2.name = 2
  btnName2.onClick = () => {
    if (nameSelectItem) nameSelectItem.unselect()
    btnName2.select()
    nameSelectItem = btnName2
  }

  btnName3 = new UIObj.UIButton(GameMakenameUI, 60, 180, 100)
  btnName3.text = _g.Player.NameList[3]
  btnName3.name = 3
  btnName3.onClick = () => {
    if (nameSelectItem) nameSelectItem.unselect()
    btnName3.select()
    nameSelectItem = btnName3
  }

  btnRename = new UIObj.UIButton(GameMakenameUI, 44, 235)
  btnRename.text = "换一批名字"
  btnRename.onClick = () => {
    var data = {
      command: _g.COMMAND.MAKENAMES,
      data: _g.Player.PlayerID
    }
    wx.sendSocketMessage({
      data: JSON.stringify(data)
    })
  }

  btnSubmit = new UIObj.UIButton(GameMakenameUI, 44, 265)
  btnSubmit.text = "确定"
  btnSubmit.onClick = () => {
    btnSubmit.hide()
    btnRename.hide()
    var data = {
      command: _g.COMMAND.CREATECHARACTER,
      data: {
        nameidx: nameSelectItem.name,
        sexidx: sexSelectItem.name
      }
    }
    wx.sendSocketMessage({
      data: JSON.stringify(data)
    })
  }
  showMakenameUI()
}

exports.UpdateName = () => {
  btnName0.text = _g.Player.NameList[0]
  btnName1.text = _g.Player.NameList[1]
  btnName2.text = _g.Player.NameList[2]
  btnName3.text = _g.Player.NameList[3]
  btnSubmit.show()
  btnRename.show()
  GameWindow.show()
}

function showMakenameUI() {
  btnName0.onClick()
  btnSex1.onClick()
  btnSubmit.show()
  btnRename.show()
  GameWindow = GameMakenameUI
  GameWindow.show()
}