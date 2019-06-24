var rate_X = GameWindow.width / _g.DEVICEWIDTH
var rate_Y = GameWindow.height / (_g.DEVICEHEIGHT - 2 * UI_VPOS)

wx.onTouchStart(
  onTouch
)

wx.onTouchEnd(
  onTouchEnd
)

function findUIObj(x = 0, y = 0, obj = GameWindow) {
  var ret = undefined
  let GameXY = obj.getGameXY()
  if (!obj.isHidden&&obj.isTouchable&&GameXY.x <= x && GameXY.y <= y && (GameXY.x + obj.width) > x && (GameXY.y + obj.height) > y) ret = obj
  obj.children.forEach(function(child, index, array) {
    let childRet = findUIObj(x, y, child)
    if (childRet)
      ret = childRet
  })
  return ret
}

var TouchingObj

function onTouch(res) {
  let X = Math.round((res.changedTouches[0].clientX) * rate_X)
  let Y = Math.round((res.changedTouches[0].clientY - UI_VPOS) * rate_Y)
  TouchingObj = findUIObj(X, Y)
  if (TouchingObj)
    TouchingObj.startTouch()
}


function onTouchEnd(res) {
  let X = Math.round((res.changedTouches[0].clientX) * rate_X)
  let Y = Math.round((res.changedTouches[0].clientY - UI_VPOS) * rate_Y)
  if (TouchingObj)
  {
    TouchingObj.endTouch()
    if (findUIObj(X, Y)===TouchingObj)
    {
      TouchingObj.onClick()
    }
    TouchingObj=undefined
  }
}