GameGlobal.canvasDevice = wx.createCanvas()
canvasDevice.width = _g.DEVICEWIDTH
canvasDevice.height = _g.DEVICEHEIGHT
GameGlobal.ctxDevice = canvasDevice.getContext('2d')
GameGlobal.UI_VPOS = (canvasDevice.height - canvasDevice.width * 8 / 5) / 2;
GameGlobal.UI_HEIGHT = canvasDevice.width * 8 / 5

function baseUIObj(father, x = 0, y = 0, w = 0, h = 0) {
  this.text = "baseUIObj"
  this.name = "base"
  this.me = this
  this.children = []
  this.father = father || undefined
  if (this.father) {
    this.father.children.push(this)
  }
  this.x = x
  this.y = y
  this.width = w
  this.height = h
  this.isHidden = false
  this.borderWidth = 1
  this.canvas = wx.createCanvas()
  this.canvas.width = w
  this.canvas.height = h
  this.gCtx = this.canvas.getContext('2d')
  this.isRedrawing = false
  this.background
  this.backgroundTouch
  this.color = "#333"
  this.colorOnTouch = "#333"
  this.isTouch = false
  this.onTouch = () => {
    console.log(this.name + " is touched")
  }
  this.onClick = () => {
    console.log(this.name + " is clicked")
  }
  this.isTouchable=false
}
baseUIObj.prototype.redraw = function(caller) {
  if (caller.isTouch)
    caller.gCtx.fillStyle = caller.colorOnTouch
  else
    caller.gCtx.fillStyle = caller.color

  caller.gCtx.fillRect(0, 0, caller.width, caller.height)

  if (caller.background)
    caller.gCtx.drawImage(caller.background, 0, 0, caller.width, caller.height)
  if (caller.backgroundTouch&&caller.isTouch)
    caller.gCtx.drawImage(caller.backgroundTouch, 0, 0, caller.width, caller.height)

  if (caller.borderWidth > 0) {
    caller.gCtx.strokeStyle = '#fff'
    caller.gCtx.lineWidth = caller.borderWidth
    caller.gCtx.strokeRect(caller.borderWidth / 2, caller.borderWidth / 2, caller.width - caller.borderWidth, caller.height - caller.borderWidth)
  }
}
baseUIObj.prototype.show = function(caller = this) {
  caller.isHidden = false
  if (caller.father)
    caller.father.redraw()
  else
    GameWindow.redraw()
}
baseUIObj.prototype.hide = function(caller = this) {
  caller.isHidden = true
  if (caller.father)
    caller.father.redraw()
  else
    GameWindow.redraw()
}
baseUIObj.prototype.render = function(caller = this) {
  let devicePosition = caller.getDevicePosition()
  ctxDevice.drawImage(caller.canvas, devicePosition.x, devicePosition.y, devicePosition.width, devicePosition.height)
}
baseUIObj.prototype.startTouch = function(caller = this) {
  caller.isTouch = true
  caller.onTouch()
  caller.redraw()
}
baseUIObj.prototype.endTouch = function(caller = this) {
  caller.isTouch = false
  console.log(caller.name + " is untouched")
  caller.redraw()
}
baseUIObj.prototype.getGameXY = function(caller = this) {
  if (!caller.father || !caller.father.father)
    return {
      x: caller.x,
      y: caller.y
    }
  else {
    let fatherXY = caller.father.getGameXY()
    return {
      x: caller.x + fatherXY.x,
      y: caller.y + fatherXY.y
    }
  }
}

baseUIObj.prototype.getDevicePosition = function(caller = this) {
  var gameXY = caller.getGameXY()
  var deviceX = gameXY.x * _g.DEVICEWIDTH / GameWindow.width
  var deviceY = gameXY.y * UI_HEIGHT / GameWindow.height + UI_VPOS
  var deviceWidth = caller.width * _g.DEVICEWIDTH / GameWindow.width
  var deviceHeight = caller.height * UI_HEIGHT / GameWindow.height
  return {
    x: deviceX,
    y: deviceY,
    width: deviceWidth,
    height: deviceHeight
  }
}

exports.UIGame = UIGame
function UIGame(x = 0, y = 0, w = 188, h = 300) {
  baseUIObj.call(this, undefined, x, y, w, h)
  this.text = "UIGame"
  this.name = "Game"
  this.borderWidth = 0
}
UIGame.prototype = new baseUIObj()
UIGame.prototype.constructor = UIGame
UIGame.prototype.redraw = function(needrender = true, caller = this) {
  
  if (caller.isRedrawing) return
  caller.isRedrawing = true
  if (!caller.isHidden) {
    baseUIObj.prototype.redraw(caller)
    caller.children.forEach(function(child, index, array) {
      if (!child.isHidden) {
        child.redraw(false)
        caller.gCtx.drawImage(child.canvas, child.x, child.y, child.width, child.height)
      }
    })
  }
  if (needrender)
    caller.render()
  caller.isRedrawing = false
  
}



exports.UIButton = UIButton

function UIButton(father, x = 0, y = 0, w = 100, h = 25) {
  baseUIObj.call(this, father, x, y, w, h)
  this.text = "UIButton"
  this.name = "Button"
  this.colorOnTouch = "#666"
  this.isTouchable = true
}
UIButton.prototype = new baseUIObj()
UIButton.prototype.constructor = UIButton
UIButton.prototype.redraw = function (needrender = true,caller = this) {
  if (caller.isRedrawing) return
  caller.isRedrawing = true
  if (caller.isHidden) {
    if (caller.father)
      caller.father.redraw(caller.father)
  } else {
    baseUIObj.prototype.redraw(caller)
    if (caller.text && caller.text.length > 0)
      drawText(caller.gCtx, caller.text, caller.width / 2 - caller.text.length * 7, (caller.height - 12) / 2)
    caller.children.forEach(function(child, index, array) {
      if (!child.isHidden) {
        child.redraw(false)
        caller.gCtx.drawImage(child.canvas, child.x, child.y, child.width, child.height)
      }
    })
  }
  if (needrender)
    GameWindow.redraw()
  caller.isRedrawing = false
}

UIButton.prototype.select = function (caller = this) {
  caller.color = "#555"
  caller.borderWidth = 2
  caller.redraw()
}

UIButton.prototype.unselect = function (caller = this) {
  caller.color = "#333"
  caller.borderWidth = 1
  caller.redraw()
}

exports.UILabel = UILabel

function UILabel(father, x = 0, y = 0, w = 100, h = 25) {
  baseUIObj.call(this, father, x, y, w, h)
  this.text = "UILabel"
  this.name = "Label"
  this.color = "transparent"
  this.isTouchable = false
  this.borderWidth=0
}
UILabel.prototype = new baseUIObj()
UILabel.prototype.constructor = UILabel
UILabel.prototype.redraw = function (needrender = true, caller = this) {
  if (caller.isRedrawing) return
  caller.isRedrawing = true
  if (caller.isHidden) {
    if (caller.father)
      caller.father.redraw(caller.father)
  } else {
    baseUIObj.prototype.redraw(caller)
    if (caller.text && caller.text.length > 0)
      drawText(caller.gCtx, caller.text, caller.width / 2 - caller.text.length * 7, (caller.height - 12) / 2)
    caller.children.forEach(function (child, index, array) {
      if (!child.isHidden) {
        child.redraw(false)
        caller.gCtx.drawImage(child.canvas, child.x, child.y, child.width, child.height)
      }
    })
  }
  if (needrender)
    GameWindow.redraw()
  caller.isRedrawing = false
}