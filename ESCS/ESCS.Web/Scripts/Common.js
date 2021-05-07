//获取浏览器中页面的可视高度
function getTotalHeight() {
    if (!$.support.leadingWhitespace) {//如果浏览器是IE
        //如果是兼容性模式则用document.documentElement.clientHeight试获取
        return document.compatMode == "CSS1Compat" ? document.documentElement.clientHeight :
                 document.body.clientHeight;
    } else {//其它浏览器
        return self.innerHeight;
    }
}

//获取浏览器中页面的可视宽度
function getTotalWidth() {
    if (!$.support.leadingWhitespace) {//如果浏览器是IE
        //如果是兼容性模式则用document.documentElement.clientWidth
        return document.compatMode == "CSS1Compat" ? document.documentElement.clientWidth :
                 document.body.clientWidth;
    } else {//其它浏览器
        return self.innerWidth;
    }
}