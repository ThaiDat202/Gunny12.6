var isGuest=false;
var isPay=false;
window["MzBrowser"]=
{
};
(function()
{
  if(MzBrowser.platform) return;
  var ua = window.navigator.userAgent;
  MzBrowser.platform = window.navigator.platform;
  MzBrowser.firefox = ua.indexOf("Firefox")>0;
  MzBrowser.opera = typeof(window.opera)=="object";
  MzBrowser.ie = !MzBrowser.opera && ua.indexOf("MSIE")>0;
  MzBrowser.mozilla = window.navigator.product == "Gecko";
  MzBrowser.netscape= window.navigator.vendor=="Netscape";
  MzBrowser.safari= ua.indexOf("Safari")>-1;
  if(MzBrowser.firefox) var re = /Firefox(\s|\/)(\d+(\.\d+)?)/;
  else if(MzBrowser.ie) var re = /MSIE( )(\d+(\.\d+)?)/;
  else if(MzBrowser.opera) var re = /Opera(\s|\/)(\d+(\.\d+)?)/;
  else if(MzBrowser.netscape) var re = /Netscape(\s|\/)(\d+(\.\d+)?)/;
  else if(MzBrowser.safari) var re = /Version(\/)(\d+(\.\d+)?)/;
  else if(MzBrowser.mozilla) var re = /rv(\:)(\d+(\.\d+)?)/;
  if("undefined"!=typeof(re)&&re.test(ua))
  MzBrowser.version = parseFloat(RegExp.$2);
}
)();
function checkScreen()
{
  s=700;
  if(MzBrowser.ie)
  {
    s=document.body.offsetHeight;
  }
  if(MzBrowser.firefox)
  {
    s=window.innerHeight;
  }
  if(MzBrowser.opera)
  {
    s=document.body.clientHeight;
  }
  if(s<630)
  {
    window.moveTo(14,14);   
    window.resizeTo(screen.width-28,screen.height-28);   
  }
  if(MzBrowser.ie)
  {
    s=document.body.offsetHeight;
  }
  if(MzBrowser.firefox)
  {
    s=window.innerHeight;
  }
  if(MzBrowser.opera)
  {
    s=document.body.clientHeight;
  }
  if (s<630)
  {
    alert('您的游戏画面显示不全，建议您使用全屏显示（按F11）');
  }
}

function addfavorite(www,name)
{
  window.external.addFavorite(www,name);
}

function askUserLeave( e )
{
	if(allowLeave==1)
	{
	}

	if(allowLeave==2||allowLeave==3)
	{
		if(!flashCall)
		{      
		 //这里是flash的Id
			var message='';
			if(dailyTask != -1 && !isGuest)
			{
			message+=dailyTask>0?'日常任务还有'+dailyTask+'个未完成\n':'日常任务：全部完成\n';
			message+=dailyActivity>0?'活动任务还有'+dailyActivity+'个未完成\n':'活动任务：全部完成\n';
			//message+=dailyEmail>0?'还有'+dailyEmail+'封未读邮件\n':'邮件：全部读取\n';			
			}else if ( isGuest ) {
				message="还没有注册,你确定离开吗?"	
			}
		
			var browser = navigator.appName;
			if (browser == "Netscape") {
				e.preventDefault();
				return message;
			} else {
				window.event.returnValue =message; 
			}	  
		} 
		flashCall=false;
	} 
}

function setUserID($userID) {
	//userID = $userID;
	isGuest = true;		
} 

function setUserRegisterOK() { 
	isGuest = false;
} 	

function hideGame(){
	document.getElementById("gameContainer").style.height = '0px';
	//alert("hide");
	return;
}

function showGame(){
	if (isGuest) {
		jQuery('#HomeDivRegister').remove();
	}
	if (isPay) {
		jQuery('#HomeDiv').remove();
		isPay = false
	}	
	document.getElementById("gameContainer").style.height = '600px';
	//alert("show");
	return;
}
