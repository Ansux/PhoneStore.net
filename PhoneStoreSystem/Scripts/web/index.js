


jQuery(function($) {
 

    /*
	$('img').lazyload({
		effect:'fadeIn',
		event:'sporty'
	});

    */

    /*主页焦点图js*/

    jQuery(function ($) {
        var start = 0;
        var end = $(".jdt .li").length - 1;
        var speed = 3000;
        var current = 0;
        var slideVar = null;
        var clickYd = -1;
        //var $li=$("li"); $(".line:eq(3)")
        function startSlideInterval() {
            $(".jdt .li:eq(" + current + ")").fadeOut("slow");//淡出
            $(".ydUl .yd:eq(" + current + ")").removeClass('currentYd');	//移除圆点class效果

            if (current == end) {
                current = -1;
            }
            if (clickYd != -1) {
                current = (clickYd - 1);
            }
            $temp = $(".jdt .li:eq(" + (current + 1) + ")");
            if ($temp.css("background-image") == 'none') //当为第一次加载图片时，下面的才执行。
                $temp.css("background-image", "url('" + $temp.attr("data-img-type") + "')").fadeIn("slow");
            else
                $temp.fadeIn("slow"); //淡入

            $(".ydUl .yd:eq(" + (current + 1) + ")").addClass('currentYd');

            current++;

            if (current > end) {
                current = start;
            }

            clickYd = -1;
        }

        slideVar = setInterval(startSlideInterval, speed);

        $(".ydUl>.yd").hover(function () {

            if (current != $(this).attr("data-tab-type")) {
                clearInterval(slideVar);
                clickYd = $(this).attr("data-tab-type");
                startSlideInterval();
                slideVar = setInterval(startSlideInterval, speed)
            }
        })

    });






	var rollNewsCurrent=0;
	var rollNewsEnd=$(".rollNews ol").height()/30-1;
	setInterval(
		function(){
			rollNewsCurrent++;
			if(rollNewsCurrent>rollNewsEnd){
				rollNewsCurrent=0;	
				$(".rollNews ol").animate({marginTop:0},1000);	//滚动文字
			}else
				$(".rollNews ol").animate({marginTop:-30*rollNewsCurrent},1000);//滚动文字
		}
	,4000);
	
	
	

    
	var currentScroll=0;
	var scrollDiv;
	function scrollLeftMove() {
	    currentScroll++;       
	    //console.log(currentScroll);
		$("#wfScroll").animate({scrollLeft:301*currentScroll},400);	//移动滚动位置
		//$("#wfScroll").scrollLeft(220*current);
		if (currentScroll == ($("#wfScroll ol li").length/2)) {
		    currentScroll = 0;            
			//用于当正常的图片显示完毕后，并且显示在克隆图片（用于显示无缝效果）处的时候，就执行0，重新回到最开始的状态，替换掉克隆的部分
            
            setTimeout(function () { $("#wfScroll").scrollLeft(currentScroll); }, 500);//由于上面的移动使用了动画，这里就需要延迟设置值，等上面动画完毕才执行设0
		}
		
	}
	function scrollRightMove() {	 
	    $("#wfScroll").animate({ scrollLeft: 301 * currentScroll }, 400);	//移动滚动位置	  
	}

	scrollDiv = setInterval(scrollLeftMove, 3000);

	$("#wfScroll li").hover(function(){
        clearInterval(scrollDiv);
    },function(){
        scrollDiv = setInterval(scrollLeftMove, 3000);
    });
	
	function bindClickNext() {
	    clearInterval(scrollDiv);
	    scrollLeftMove();
	    scrollDiv = setInterval(scrollLeftMove, 3000);
	    $("#rightScroll").unbind('click');
	    setTimeout(function () {
	        $("#rightScroll").bind('click', function () { bindClickNext() });
	    }, 600);
	}
	$("#rightScroll").bind('click', function () {
	    bindClickNext();
	});
	
	function bindClickPrev() {
	    if (currentScroll != 0) {
	        clearInterval(scrollDiv);
	        currentScroll--;
	        scrollRightMove();
	        scrollDiv = setInterval(scrollLeftMove, 3000);

	    }
	    $("#leftScroll").unbind('click');
	    setTimeout(function () {
	        $("#leftScroll").bind('click', function () { bindClickPrev() });
	    }, 600);
	}
	$("#leftScroll").bind('click', function () {
	    bindClickPrev();
	});


    



	$(".hotProduct .productType li:not(:last-child)").click(function () {
	    $(".hotProduct .productType li").removeClass("currentLi");
	    $(this).addClass("currentLi");
	    var i = $(this).attr("data-li-num");
	    $(".hotProduct .productUl:visible").hide(0);
	    $(".hotProduct .productUl:eq(" + i + ")").show(0);

	    if ($(".hotProduct .productUl:eq(" + i + ") a").css("background-image") == 'none') {
	        lazyImgLoad($(".hotProduct .productUl:visible a.img"), 100, 1/*1-10*/, "backImg");
	    }
	});

	$(".newProduct .productType li:not(:last-child)").click(function () {
	    $(".newProduct .productType li").removeClass("currentLi");
	    $(this).addClass("currentLi");
	    var i = $(this).attr("data-li-num");
	    $(".newProduct .productUl:visible").hide(0);
	    $(".newProduct .productUl:eq(" + i + ")").show(0);

	    if ($(".newProduct .productUl:eq(" + i + ") a").css("background-image") == 'none') {
	        lazyImgLoad($(".newProduct  .productUl:visible a.img"), 100, 1/*1-10*/, "backImg");
	    }

	});




});