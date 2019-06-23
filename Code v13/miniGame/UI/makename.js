exports.show = () => {
  ctxUI.font = "12px simhei";
  ctxUI.strokeStyle = "#fff"
  ctxUI.strokeText("你好", 10, 15)
  drawText(ctxUI, "你好", 10, 50)
}