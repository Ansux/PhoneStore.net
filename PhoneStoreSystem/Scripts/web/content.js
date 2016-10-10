function ifie(){
	return document.all;
}

function SetHome(obj,vrl,info){
	if(!ifie()){
		alert(info);
		return false;
	}
	try{
		obj.style.behavior='url(#default#homepage)';obj.setHomePage(vrl);
	}catch(e){
		if(window.netscape){
			try{
				netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
			}catch(e){
				alert("Your Browser does not support");
			}
			var prefs=Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
				prefs.setCharPref('browser.startup.homepage',vrl);
		}
	}
}

function addFavorite(info){
	if(!ifie()){
		alert(info);
		return false;
	}
	var vDomainName=window.location.href;
	var description=document.title;
	try{
		window.external.AddFavorite(vDomainName,description);
	}catch(e){
		window.sidebar.addPanel(description,vDomainName,"");
	}
}

function lazyImgLoad($obj, startTime, speedNum, tab) {
    var i = speedNum;

    $obj.each(function () {
        var $xx = $(this);
            setTimeout(function () {
                if (tab == "backImg")
                    $xx.css("background-image", "url('" + $xx.attr("data-img-type") + "')").hide().fadeIn(500);
                else
                    $xx.attr("src", $xx.attr("data-img-type")).fadeIn("slow");
            }, startTime * i);
            i += speedNum;
    });
}

